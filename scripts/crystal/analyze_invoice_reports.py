import os
import re
import sys
from typing import Iterable

try:
    import olefile  # type: ignore
except Exception as exc:  # pragma: no cover
    raise SystemExit(
        "Missing dependency 'olefile'. Install with: python -m pip install olefile --user"
    ) from exc


DEFAULT_NAMES = [
    "CRInvoice.rpt",
    "CRInvoiceNonVat.rpt",
    "CRInvoiceWithDiscount.rpt",
    "CRInvoiceWithDiscountNonVat.rpt",
    "CRInvoiceCreditNonVat.rpt",
    "CRInvoiceTOTINC.rpt",
    "CRInvoiceTax.rpt",
    "CRPOSInvoicePRINT.rpt",
    "CRPOSInvoicePRINTWithDis.rpt",
    "rptInvoices.rpt",
]

TOKEN_RE = re.compile(
    rb"(SELECT|FROM|WHERE|JOIN|GROUP\s+BY|ORDER\s+BY|tbl\w+|"
    rb"InvoiceNo|InvoiceDate|CustomerName|VehicleNo|ItemID|NetTotal|PaidAmount|VAT|NBT|TTType)"
)


def extract_printable_strings(data: bytes, min_len: int = 4) -> list[str]:
    # Extract ASCII-ish strings
    out: list[str] = []
    buf: list[int] = []
    for b in data:
        if 32 <= b <= 126:
            buf.append(b)
        else:
            if len(buf) >= min_len:
                out.append(bytes(buf).decode("latin1", "ignore"))
            buf.clear()
    if len(buf) >= min_len:
        out.append(bytes(buf).decode("latin1", "ignore"))

    # Extract UTF-16LE-ish strings (common in OLE streams)
    if len(data) >= 2:
        try:
            u = data.decode("utf-16le", "ignore")
            # Split on NUL and other control chars
            for part in re.split(r"[\x00-\x1F]+", u):
                if len(part) >= min_len:
                    out.append(part)
        except Exception:
            pass

    return out


def iter_candidates(folder: str) -> Iterable[str]:
    # Prefer known names, but fall back to anything containing Invoice.
    present = []
    for n in DEFAULT_NAMES:
        p = os.path.join(folder, n)
        if os.path.exists(p):
            present.append(p)

    if present:
        return present

    out = []
    for name in os.listdir(folder):
        if not name.lower().endswith(".rpt"):
            continue
        if "invoice" in name.lower():
            out.append(os.path.join(folder, name))
    out.sort()
    return out


def main() -> int:
    folder = sys.argv[1] if len(sys.argv) > 1 else r"E:\\LASANTHA\\LastUpdate\\SMS\\New folder\\Debug SMS\\REPORTS"
    if not os.path.isdir(folder):
        print(f"Folder not found: {folder}")
        return 2

    args = sys.argv[2:]

    if "--list" in args:
        files = []
        for name in sorted(os.listdir(folder)):
            p = os.path.join(folder, name)
            if os.path.isfile(p):
                files.append((name, os.path.getsize(p)))
        print(f"file_count: {len(files)}")
        for name, size in files:
            print(f"{size:10d}  {name}")
        return 0

    analyze_all = "--all" in args
    specific_names = [a for a in args if not a.startswith("--")]
    if specific_names:
        candidates = [os.path.join(folder, n) for n in specific_names]
    elif analyze_all:
        candidates = [
            os.path.join(folder, n)
            for n in sorted(os.listdir(folder))
            if n.lower().endswith(".rpt") and os.path.isfile(os.path.join(folder, n))
        ]
    else:
        candidates = list(iter_candidates(folder))
    if not candidates:
        print("No invoice .rpt files found")
        return 0

    print(f"Folder: {folder}")
    for path in candidates:
        name = os.path.basename(path)
        print(f"\n=== {name} ===")
        print(f"size_bytes: {os.path.getsize(path)}")

        try:
            ole = olefile.OleFileIO(path)
        except Exception as e:
            print(f"OLE open failed: {e}")
            continue

        entries = ole.listdir(streams=True, storages=False)
        print(f"stream_count: {len(entries)}")

        streams = []
        for entry in entries:
            try:
                size = ole.get_size(entry)
            except Exception:
                size = 0
            streams.append((size, entry))
        streams.sort(key=lambda x: x[0], reverse=True)

        print("top_streams:")
        for size, entry in streams[:20]:
            print(f"  {size:8d}  {'/'.join(entry)}")

        # Token scan (best-effort)
        hit_streams = []
        for size, entry in streams[:80]:
            try:
                data = ole.openstream(entry).read()
            except Exception:
                continue
            if TOKEN_RE.search(data):
                hits = sorted({m.group(0).decode("latin1", "ignore") for m in TOKEN_RE.finditer(data)})
                hit_streams.append((size, entry, hits))

        print(f"streams_with_sqlish_tokens_in_top80: {len(hit_streams)}")
        for size, entry, hits in hit_streams[:12]:
            print(f"  {size:8d}  {'/'.join(entry)}")
            print("    tokens:", ", ".join(hits[:25]))

        # Deep scan the most important streams for readable strings.
        key_stream_names = {
            ("Contents",),
            ("QESession",),
            ("ReportInfo",),
            ("Embedding 1", "CONTENTS"),
        }
        interesting_re = re.compile(
            r"(tbl\w+|Invoice|Customer|Vehicle|Item|NetTotal|PaidAmount|VAT|NBT|TTType|WHID|Sales|Return)",
            re.IGNORECASE,
        )
        printed_any = False
        for _, entry in streams:
            if tuple(entry) not in key_stream_names:
                continue
            try:
                data = ole.openstream(entry).read()
            except Exception:
                continue
            strings = extract_printable_strings(data, 5)
            hits = [s.strip() for s in strings if interesting_re.search(s)]
            # De-dupe while preserving order
            seen = set()
            uniq: list[str] = []
            for s in hits:
                if s in seen:
                    continue
                seen.add(s)
                uniq.append(s)

            if not uniq:
                continue

            printed_any = True
            print(f"readable_hits_in_stream: {'/'.join(entry)}")
            for s in uniq[:60]:
                print(f"  {s}")

        if not printed_any:
            print("readable_hits_in_key_streams: none")

        ole.close()

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
