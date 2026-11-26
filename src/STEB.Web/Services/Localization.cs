using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace STEB.Web.Services;

/// <summary>
/// Display language of the web port (a global, app-wide setting).
/// <para>
/// The program's own texts are Russian (the 2010 code is kept as decompiled). They are translated
/// only where they are shown — page markup, window titles, grid headers, drawings, reports, log —
/// using <c>Resources/i18n.json</c>, whose keys are the original Russian texts. Calculations,
/// stored data and the decompiled code are not affected.
/// </para>
/// </summary>
public static class L
{
    public static readonly IReadOnlyList<(string Code, string Name)> Languages = new[]
    {
        ("en", "English"), ("ru", "Русский"), ("es", "Español"), ("fr", "Français"), ("tr", "Türkçe")
    };

    public const string Default = "en";

    /// <summary>Current language code (en, ru, es, fr, tr).</summary>
    public static string Language { get; private set; } = Default;

    /// <summary>Raised after the language has been changed.</summary>
    public static event Action Changed;

    private static Dictionary<string, Dictionary<string, string>> _dict = new(StringComparer.Ordinal);
    private static Regex _phrases;
    private static string _settingsFile;
    private static readonly Regex Spaces = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex Cyrillic = new("[Ѐ-ӿ]", RegexOptions.Compiled);

    /// <summary>Loads the dictionary and the saved language. Call once at start-up.</summary>
    public static void Init(string contentRoot)
    {
        string file = new[] { Path.Combine(contentRoot, "Resources", "i18n.json"), Path.Combine(AppContext.BaseDirectory, "Resources", "i18n.json") }
            .FirstOrDefault(File.Exists);
        if (file != null) Load(File.ReadAllText(file, Encoding.UTF8));

        _settingsFile = Path.Combine(contentRoot, "settings.json");
        try
        {
            if (File.Exists(_settingsFile) && JsonNode.Parse(File.ReadAllText(_settingsFile))?["language"]?.GetValue<string>() is string saved && IsSupported(saved))
                Language = saved;
        }
        catch { /* a broken settings file just means the default language */ }
    }

    /// <summary>Loads a dictionary from JSON: { "russian text": { "en": "...", "es": "...", ... } }.</summary>
    public static void Load(string json)
    {
        var dict = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
        foreach (var (ru, langs) in JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json))
            dict[Normalize(ru)] = langs;
        _dict = dict;
        // whole words/phrases only: never translate the middle of a longer word
        // (the word-boundary checks apply only at ends that are letters, so ", угол крена" still matches after a number)
        var alternatives = dict.Keys.Where(k => k.Length > 1).OrderByDescending(k => k.Length).Select(k =>
            (char.IsLetter(k[0]) ? @"(?<![\p{L}\p{Nd}])" : "") +
            Regex.Escape(k).Replace(@"\ ", @"\s+") +
            (char.IsLetter(k[^1]) ? @"(?![\p{L}])" : ""));
        _phrases = new Regex(string.Join("|", alternatives), RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }

    public static bool IsSupported(string code) => Languages.Any(l => l.Code == code);

    public static void SetLanguage(string code)
    {
        if (!IsSupported(code) || code == Language) return;
        Language = code;
        try { if (_settingsFile != null) File.WriteAllText(_settingsFile, new JsonObject { ["language"] = code }.ToJsonString()); }
        catch { /* the choice still applies until the server restarts */ }
        Changed?.Invoke();
    }

    /// <summary>
    /// Translates a program text: the whole text if it is a known phrase, otherwise every known
    /// phrase inside it (texts built by the original code, e.g. "Осадка носом, м" + value).
    /// Russian, unknown and non-Russian texts are returned unchanged.
    /// </summary>
    public static string T(string text)
    {
        if (string.IsNullOrEmpty(text) || Language == "ru" || !Cyrillic.IsMatch(text)) return text;
        if (Lookup(Normalize(text)) is string whole) return whole;
        if (_phrases == null) return text;
        return _phrases.Replace(text, m => Lookup(Normalize(m.Value)) ?? m.Value);
    }

    /// <summary>T() for a format string: translates the template, then fills in the arguments.</summary>
    public static string F(string template, params object[] args) => string.Format(T(template), args);

    private static string Lookup(string key) =>
        _dict.TryGetValue(key, out var langs) && langs.TryGetValue(Language, out var t) && !string.IsNullOrEmpty(t) ? t : null;

    private static string Normalize(string s) => Spaces.Replace(s.Trim(), " ");
}
