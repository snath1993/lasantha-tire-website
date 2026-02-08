import os
import subprocess
import sys

CSC = r"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"
DLL_ROOT = r"C:\Program Files (x86)\Business Objects\Common\4.0\managed\dotnet2"

REFS = [
    os.path.join(DLL_ROOT, "CrystalDecisions.CrystalReports.Engine.dll"),
    os.path.join(DLL_ROOT, "CrystalDecisions.Shared.dll"),
    os.path.join(DLL_ROOT, "CrystalDecisions.ReportSource.dll"),
    os.path.join(DLL_ROOT, "CrystalDecisions.Windows.Forms.dll"),
]

SRC = os.path.join(os.path.dirname(__file__), "CrystalRptDump.cs")
OUT = os.path.join(os.path.dirname(__file__), "CrystalRptDump.exe")


def main() -> int:
    missing = [p for p in [CSC, SRC, *REFS] if not os.path.exists(p)]
    if missing:
        print("Missing:")
        for p in missing:
            print("  ", p)
        return 2

    cmd = [CSC, "/nologo", "/t:exe", "/platform:x86", f"/out:{OUT}", SRC]
    cmd.extend([f"/r:{r}" for r in REFS])

    print("Running:")
    print(" ".join(f'"{c}"' if " " in c else c for c in cmd))

    proc = subprocess.run(cmd, capture_output=True, text=True)
    if proc.stdout:
        print("\nSTDOUT:\n" + proc.stdout)
    if proc.stderr:
        print("\nSTDERR:\n" + proc.stderr)

    print("ExitCode:", proc.returncode)
    if proc.returncode == 0 and os.path.exists(OUT):
        print("Built:", OUT)
    return proc.returncode


if __name__ == "__main__":
    raise SystemExit(main())
