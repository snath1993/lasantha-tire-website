using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

internal static class CrystalRptDump
{
    private sealed class Options
    {
        public string FolderOrFile = "";
        public bool All;
        public string OutFile;
        public List<string> SpecificFiles = new List<string>();
    }

    public static int Main(string[] args)
    {
        try
        {
            var opt = ParseArgs(args);
            if (string.IsNullOrWhiteSpace(opt.FolderOrFile))
            {
                Console.WriteLine("Usage: CrystalRptDump <folderOrFile> [--all] [--out <file>] [report1.rpt report2.rpt ...]");
                return 2;
            }

            var targets = ResolveTargets(opt);
            if (targets.Count == 0)
            {
                WriteOutput(opt, "No .rpt files found.\n");
                return 0;
            }

            var sw = new StringWriter();
            sw.WriteLine("Crystal RPT Structure Dump");
            sw.WriteLine("Generated: " + DateTime.Now);
            sw.WriteLine();

            foreach (var path in targets)
            {
                DumpOne(path, sw);
                sw.WriteLine();
            }

            WriteOutput(opt, sw.ToString());
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static Options ParseArgs(string[] args)
    {
        var opt = new Options();
        if (args.Length == 0) return opt;

        opt.FolderOrFile = args[0];
        for (int i = 1; i < args.Length; i++)
        {
            var a = args[i];
            if (string.Equals(a, "--all", StringComparison.OrdinalIgnoreCase))
            {
                opt.All = true;
                continue;
            }
            if (string.Equals(a, "--out", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                opt.OutFile = args[i + 1];
                i++;
                continue;
            }

            opt.SpecificFiles.Add(a);
        }
        return opt;
    }

    private static List<string> ResolveTargets(Options opt)
    {
        var folderOrFile = opt.FolderOrFile;
        if (File.Exists(folderOrFile))
        {
            return new List<string> { Path.GetFullPath(folderOrFile) };
        }

        if (!Directory.Exists(folderOrFile))
        {
            return new List<string>();
        }

        var folder = Path.GetFullPath(folderOrFile);

        if (opt.SpecificFiles.Count > 0)
        {
            var list = new List<string>();
            foreach (var f in opt.SpecificFiles)
            {
                var p = Path.IsPathRooted(f) ? f : Path.Combine(folder, f);
                if (File.Exists(p) && p.EndsWith(".rpt", StringComparison.OrdinalIgnoreCase))
                    list.Add(Path.GetFullPath(p));
            }
            return list;
        }

        if (opt.All)
        {
            return Directory.GetFiles(folder, "*.rpt", SearchOption.TopDirectoryOnly)
                .OrderBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // Default: try invoice-related first
        return Directory.GetFiles(folder, "*.rpt", SearchOption.TopDirectoryOnly)
            .Where(p => Path.GetFileName(p).IndexOf("invoice", StringComparison.OrdinalIgnoreCase) >= 0)
            .OrderBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void WriteOutput(Options opt, string content)
    {
        if (!string.IsNullOrWhiteSpace(opt.OutFile))
        {
            var outPath = Path.GetFullPath(opt.OutFile);
            var dir = Path.GetDirectoryName(outPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(outPath, content);
            Console.WriteLine("Wrote: " + outPath);
            return;
        }

        Console.WriteLine(content);
    }

    private static void DumpOne(string rptPath, StringWriter sw)
    {
        sw.WriteLine("=== " + Path.GetFileName(rptPath) + " ===");
        sw.WriteLine("Path: " + rptPath);
        sw.WriteLine("Size: " + new FileInfo(rptPath).Length + " bytes");

        var doc = new ReportDocument();
        try
        {
            doc.Load(rptPath);
        }
        catch (Exception ex)
        {
            sw.WriteLine("LoadError: " + ex.Message);
            return;
        }

        // Parameters
        try
        {
            sw.WriteLine("Parameters:");
            var pfs = doc.DataDefinition.ParameterFields;
            if (pfs == null || pfs.Count == 0)
            {
                sw.WriteLine("  (none)");
            }
            else
            {
                for (int i = 0; i < pfs.Count; i++)
                {
                    var pf = pfs[i];
                    sw.WriteLine("  - " + pf.Name + " (" + pf.ValueType + ")");
                }
            }
        }
        catch (Exception ex)
        {
            sw.WriteLine("ParametersError: " + ex.Message);
        }

        // Tables + fields
        try
        {
            sw.WriteLine("Tables:");
            var tables = doc.Database.Tables;
            if (tables == null || tables.Count == 0)
            {
                sw.WriteLine("  (none)");
            }
            else
            {
                for (int i = 0; i < tables.Count; i++)
                {
                    var t = tables[i];
                    sw.WriteLine("  - " + t.Name + (string.IsNullOrWhiteSpace(t.Location) ? "" : (" @ " + t.Location)));
                    try
                    {
                        if (t.Fields != null && t.Fields.Count > 0)
                        {
                            // Limit to avoid huge dumps
                            var max = Math.Min(t.Fields.Count, 80);
                            for (int f = 0; f < max; f++)
                            {
                                var fd = t.Fields[f];
                                sw.WriteLine("      * " + fd.Name + " (" + fd.ValueType + ")");
                            }
                            if (t.Fields.Count > max)
                                sw.WriteLine("      ... (" + (t.Fields.Count - max) + " more fields)");
                        }
                    }
                    catch
                    {
                        // ignore per-table field issues
                    }
                }
            }
        }
        catch (Exception ex)
        {
            sw.WriteLine("TablesError: " + ex.Message);
        }

        // Formula fields
        try
        {
            sw.WriteLine("FormulaFields:");
            var ffs = doc.DataDefinition.FormulaFields;
            if (ffs == null || ffs.Count == 0)
            {
                sw.WriteLine("  (none)");
            }
            else
            {
                for (int i = 0; i < ffs.Count; i++)
                {
                    var ff = ffs[i];
                    sw.WriteLine("  - " + ff.Name);
                }
            }
        }
        catch (Exception ex)
        {
            sw.WriteLine("FormulaError: " + ex.Message);
        }

        // Sections summary
        try
        {
            sw.WriteLine("Sections:");
            var secs = doc.ReportDefinition.Sections;
            if (secs == null || secs.Count == 0)
            {
                sw.WriteLine("  (none)");
            }
            else
            {
                for (int i = 0; i < secs.Count; i++)
                {
                    var s = secs[i];
                    sw.WriteLine("  - " + s.Name + " (objects: " + s.ReportObjects.Count + ")");
                }
            }
        }
        catch (Exception ex)
        {
            sw.WriteLine("SectionsError: " + ex.Message);
        }

        try { doc.Close(); } catch { }
        try { doc.Dispose(); } catch { }
    }
}
