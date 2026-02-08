import json
import os
import sys
from typing import Any, Dict


def main() -> int:
  if len(sys.argv) < 2:
    print("Usage: python extract_pdf_layout.py <pdf_path> [out_dir]")
    return 2

  pdf_path = sys.argv[1]
  out_dir = sys.argv[2] if len(sys.argv) >= 3 else os.path.join(os.getcwd(), "tmp")

  os.makedirs(out_dir, exist_ok=True)

  base_name = os.path.splitext(os.path.basename(pdf_path))[0]
  out_txt = os.path.join(out_dir, f"{base_name}_extract.txt")
  out_json = os.path.join(out_dir, f"{base_name}_page1_layout.json")
  out_png = os.path.join(out_dir, f"{base_name}_page1.png")

  result: Dict[str, Any] = {
    "pdf_path": pdf_path,
    "exists": os.path.exists(pdf_path),
    "size": os.path.getsize(pdf_path) if os.path.exists(pdf_path) else None,
    "extractor": None,
    "page_count": None,
    "page1": None,
  }

  text_out = []
  text_out.append(f"pdf_path={pdf_path}")
  text_out.append(f"exists={result['exists']}")
  text_out.append(f"size={result['size']}")

  if not result["exists"]:
    with open(out_txt, "w", encoding="utf-8") as f:
      f.write("\n".join(text_out) + "\n")
    print(out_txt)
    return 1

  try:
    import fitz  # type: ignore

    result["extractor"] = "pymupdf"
    doc = fitz.open(pdf_path)
    result["page_count"] = doc.page_count
    text_out.append(f"extractor=pymupdf")
    text_out.append(f"pages={doc.page_count}")

    if doc.page_count:
      page = doc.load_page(0)
      rect = page.rect
      text_out.append(f"page1_width={rect.width}")
      text_out.append(f"page1_height={rect.height}")

      # Render a PNG for visual reference (user can open locally).
      try:
        pix = page.get_pixmap(dpi=200)
        pix.save(out_png)
        text_out.append(f"rendered_png={out_png}")
      except Exception as e:
        text_out.append(f"render_png_failed={type(e).__name__}:{e}")

      # Text (for quick grep).
      page_text = page.get_text("text")
      text_out.append("\n=== PAGE 1 TEXT ===\n")
      text_out.append(page_text)

      # Layout (for column/position matching).
      blocks = page.get_text("blocks")
      words = page.get_text("words")
      # Sort words top-to-bottom then left-to-right
      words_sorted = sorted(words, key=lambda w: (round(w[1], 1), round(w[0], 1)))

      result["page1"] = {
        "size": {"width": rect.width, "height": rect.height},
        "blocks": [
          {
            "x0": b[0],
            "y0": b[1],
            "x1": b[2],
            "y1": b[3],
            "text": b[4],
            "block_no": b[5],
            "block_type": b[6],
          }
          for b in blocks
        ],
        "words": [
          {
            "x0": w[0],
            "y0": w[1],
            "x1": w[2],
            "y1": w[3],
            "text": w[4],
            "block_no": w[5],
            "line_no": w[6],
            "word_no": w[7],
          }
          for w in words_sorted
        ],
      }

    doc.close()

  except Exception as e:
    result["extractor"] = f"pymupdf_failed:{type(e).__name__}:{e}"
    text_out.append(result["extractor"])

    # Fallback: pdfplumber (word boxes available; rendering may be limited)
    try:
      import pdfplumber  # type: ignore

      result["extractor"] = "pdfplumber"
      text_out.append("extractor=pdfplumber")

      with pdfplumber.open(pdf_path) as pdf:
        result["page_count"] = len(pdf.pages)
        text_out.append(f"pages={len(pdf.pages)}")

        if pdf.pages:
          page = pdf.pages[0]
          text_out.append(f"page1_width={page.width}")
          text_out.append(f"page1_height={page.height}")

          page_text = page.extract_text() or ""
          text_out.append("\n=== PAGE 1 TEXT ===\n")
          text_out.append(page_text)

          # Words with positions
          words = page.extract_words(
            use_text_flow=True,
            keep_blank_chars=False,
          )

          # Normalize shape to match pymupdf-ish keys
          result["page1"] = {
            "size": {"width": page.width, "height": page.height},
            "blocks": [],
            "words": [
              {
                "x0": w.get("x0"),
                "y0": w.get("top"),
                "x1": w.get("x1"),
                "y1": w.get("bottom"),
                "text": w.get("text"),
                "block_no": None,
                "line_no": w.get("top"),
                "word_no": None,
              }
              for w in words
            ],
          }

    except Exception as e2:
      text_out.append(f"pdfplumber_failed:{type(e2).__name__}:{e2}")

      # Fallback: pdfminer.six (line boxes; good enough for column/spacing matching)
      try:
        from pdfminer.high_level import extract_pages  # type: ignore
        from pdfminer.layout import LTTextContainer, LTTextLine  # type: ignore

        result["extractor"] = "pdfminer.six"
        text_out.append("extractor=pdfminer.six")

        page1 = None
        for page_layout in extract_pages(pdf_path, maxpages=1):
          page_bbox = getattr(page_layout, "bbox", None)
          page_width = page_bbox[2] - page_bbox[0] if page_bbox else None
          page_height = page_bbox[3] - page_bbox[1] if page_bbox else None

          text_out.append(f"page1_width={page_width}")
          text_out.append(f"page1_height={page_height}")

          lines = []
          for element in page_layout:
            if isinstance(element, LTTextContainer):
              for line in element:
                if isinstance(line, LTTextLine):
                  t = (line.get_text() or "").replace("\u00a0", " ").strip()
                  if not t:
                    continue
                  x0, y0, x1, y1 = line.bbox
                  # pdfminer uses bottom-left origin; add top-coordinates for easier matching.
                  top0 = (page_height - y1) if page_height is not None else None
                  top1 = (page_height - y0) if page_height is not None else None
                  lines.append(
                    {
                      "x0": float(x0),
                      "y0": float(y0),
                      "x1": float(x1),
                      "y1": float(y1),
                      "top0": float(top0) if top0 is not None else None,
                      "top1": float(top1) if top1 is not None else None,
                      "text": t,
                    }
                  )

          # Sort: top-to-bottom, then left-to-right
          lines.sort(key=lambda r: ((r.get("top0") is None, r.get("top0")), r.get("x0")))

          page1 = {
            "size": {"width": page_width, "height": page_height},
            "blocks": lines,
            "words": [],
          }

          # Also store a plain text concatenation for quick scanning.
          text_out.append("\n=== PAGE 1 TEXT (pdfminer lines) ===\n")
          for ln in lines:
            text_out.append(ln["text"])
          break

        result["page_count"] = result.get("page_count")
        result["page1"] = page1

      except Exception as e3:
        text_out.append(f"pdfminer_failed:{type(e3).__name__}:{e3}")

  with open(out_txt, "w", encoding="utf-8", errors="replace") as f:
    f.write("\n".join(text_out) + "\n")

  with open(out_json, "w", encoding="utf-8") as f:
    json.dump(result, f, ensure_ascii=False, indent=2)

  print(out_txt)
  print(out_json)
  if os.path.exists(out_png):
    print(out_png)

  return 0


if __name__ == "__main__":
  raise SystemExit(main())
