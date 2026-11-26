using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace STEB.Web.Services;

/// <summary>Reads a report HTML file written by the original code and makes it self-contained for the browser.</summary>
public static class ReportFiles
{
    public static string LoadForDisplay(string path)
    {
        if (!File.Exists(path)) return "<p style='font-family:sans-serif;color:#b3261e'>Файл отчета не найден: " + System.Net.WebUtility.HtmlEncode(path) + "</p>";
        string html = File.ReadAllText(path, Encoding.UTF8);
        // <img src="...\name.bmp">: the drawing was saved by Image.Save as name.svg (STEB.Compat) -> embed it.
        html = Regex.Replace(html, "src=\"([^\"]+)\"", m =>
        {
            string src = m.Groups[1].Value;
            foreach (var candidate in new[] { src, Path.ChangeExtension(src, ".svg") })
            {
                if (!File.Exists(candidate)) continue;
                string mime = candidate.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ? "image/svg+xml"
                            : candidate.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png"
                            : candidate.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ? "image/bmp" : "image/jpeg";
                return "src=\"data:" + mime + ";base64," + Convert.ToBase64String(File.ReadAllBytes(candidate)) + "\"";
            }
            return m.Value;
        });
        if (html.IndexOf("<head", StringComparison.OrdinalIgnoreCase) < 0)
            html = html.Replace("<html>", "<html><head><meta charset=\"utf-8\"><style>body{font-family:'Segoe UI',Tahoma,Arial,sans-serif;font-size:13px} table{border-collapse:collapse} td{padding:3px 6px}</style></head>");
        return html;
    }
}
