import os
import sys
import glob
import json

CANDIDATE_DLLS = [
    "CrystalDecisions.CrystalReports.Engine.dll",
    "CrystalDecisions.Shared.dll",
    "CrystalDecisions.ReportSource.dll",
    "CrystalDecisions.Windows.Forms.dll",
]

CANDIDATE_ROOTS = [
    r"C:\Program Files (x86)\SAP BusinessObjects",
    r"C:\Program Files (x86)\Business Objects",
    r"C:\Program Files\SAP BusinessObjects",
    r"C:\Program Files\Business Objects",
]


def find_files(root: str, filename: str, limit: int = 10):
    matches = []
    for dirpath, dirnames, files in os.walk(root):
        if filename in files:
            matches.append(os.path.join(dirpath, filename))
            if len(matches) >= limit:
                break
    return matches


def main() -> int:
    print("Python:", sys.version)

    result: dict[str, object] = {
        "python": sys.version,
        "framework_csc": [],
        "crystal_roots": [],
        "dlls": {},
    }

    # csc.exe locations
    print("\n--- csc.exe (Framework) ---")
    fw = glob.glob(r"C:\\Windows\\Microsoft.NET\\Framework*\\v*\\csc.exe")
    fw = sorted(set(fw))
    result["framework_csc"] = fw
    for p in fw[-8:]:
        print(p)
    if not fw:
        print("(none found)")

    # Crystal DLL locations
    print("\n--- Crystal DLLs ---")
    found_any = False
    dll_map: dict[str, list[str]] = {k: [] for k in CANDIDATE_DLLS}
    roots_used: list[str] = []
    for root in CANDIDATE_ROOTS:
        if not os.path.isdir(root):
            continue
        roots_used.append(root)
        print("\nROOT:", root)
        for dll in CANDIDATE_DLLS:
            matches = find_files(root, dll, limit=5)
            if matches:
                found_any = True
                dll_map[dll].extend(matches)
                print(f"  {dll}:")
                for m in matches:
                    print("    ", m)
            else:
                print(f"  {dll}: (not found)")

    result["crystal_roots"] = roots_used
    result["dlls"] = {k: sorted(set(v)) for k, v in dll_map.items()}

    if not found_any:
        print("\nNo CrystalDecisions DLLs found under common install roots.")
        print("If Crystal Reports 2008 is installed, DLLs might be in GAC or a different folder.")

    out_path = os.path.join(os.path.dirname(__file__), "crystal_runtime_locations.json")
    with open(out_path, "w", encoding="utf-8") as f:
        json.dump(result, f, ensure_ascii=False, indent=2)
    print("\nWrote:", out_path)

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
