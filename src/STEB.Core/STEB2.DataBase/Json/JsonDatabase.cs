using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace STEB2.DataBase.Json;

/// <summary>Column types carried over from the original Access (.xsteb) schema.</summary>
public enum JsonColumnType { Text, Memo, Double, Float, Int32, Int16, Byte, Bool, DateTime }

public sealed class JsonColumn
{
    public string Name;
    public JsonColumnType Type;
    public int Size;
    public bool AutoIncrement;
    public bool PrimaryKey;
}

public sealed class JsonTable
{
    public string Name;
    public List<JsonColumn> Columns = new List<JsonColumn>();
    public List<object[]> Rows = new List<object[]>();
    public int NextAutoId = 1;

    public int IndexOf(string column)
    {
        for (int i = 0; i < Columns.Count; i++)
            if (string.Equals(Columns[i].Name, column, StringComparison.OrdinalIgnoreCase)) return i;
        return -1;
    }
}

/// <summary>
/// The whole project database held in memory and persisted as one JSON file.
/// Replaces the Microsoft Access (Jet 4) .xsteb file of the original program.
/// </summary>
public sealed class JsonDatabase
{
    public const string FormatName = "STEB JSON database";
    public const int FormatVersion = 1;

    private static readonly Dictionary<string, JsonDatabase> _cache = new Dictionary<string, JsonDatabase>(StringComparer.OrdinalIgnoreCase);
    private static readonly object _cacheLock = new object();

    public readonly object SyncRoot = new object();
    public string Path { get; private set; }
    public Dictionary<string, JsonTable> Tables { get; } = new Dictionary<string, JsonTable>(StringComparer.OrdinalIgnoreCase);
    public bool Dirty { get; set; }
    private DateTime _loadedStamp;

    /// <summary>Returns the shared in-memory instance for a file, reloading it if it changed on disk.</summary>
    public static JsonDatabase Open(string path)
    {
        string full = System.IO.Path.GetFullPath(path);
        lock (_cacheLock)
        {
            if (_cache.TryGetValue(full, out var db))
            {
                if (!db.Dirty && File.Exists(full) && File.GetLastWriteTimeUtc(full) != db._loadedStamp)
                {
                    db = Load(full);
                    _cache[full] = db;
                }
                return db;
            }
            db = Load(full);
            _cache[full] = db;
            return db;
        }
    }

    public static JsonDatabase Load(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("Файл проекта не найден", path);
        var root = JsonNode.Parse(File.ReadAllText(path, Encoding.UTF8)) as JsonObject
                   ?? throw new InvalidDataException("Неверный формат файла проекта");
        var db = new JsonDatabase { Path = path };
        if (root["tables"] is JsonObject tables)
        {
            foreach (var kv in tables)
            {
                var t = new JsonTable { Name = kv.Key };
                var to = (JsonObject)kv.Value;
                foreach (JsonObject c in (JsonArray)to["columns"])
                {
                    t.Columns.Add(new JsonColumn
                    {
                        Name = (string)c["name"],
                        Type = ParseType((string)c["type"]),
                        Size = c["size"] is JsonNode s ? (int)s : 0,
                        AutoIncrement = c["autoIncrement"] is JsonNode a && (bool)a,
                        PrimaryKey = c["primaryKey"] is JsonNode pk && (bool)pk,
                    });
                }
                t.NextAutoId = to["nextAutoId"] is JsonNode n ? (int)n : 1;
                if (to["rows"] is JsonArray rows)
                {
                    foreach (JsonObject r in rows)
                    {
                        var row = new object[t.Columns.Count];
                        for (int i = 0; i < t.Columns.Count; i++)
                            row[i] = FromJson(r[t.Columns[i].Name], t.Columns[i].Type);
                        t.Rows.Add(row);
                    }
                }
                db.Tables[t.Name] = t;
            }
        }
        db._loadedStamp = File.GetLastWriteTimeUtc(path);
        return db;
    }

    public void Save()
    {
        lock (SyncRoot)
        {
            var tables = new JsonObject();
            foreach (var t in Tables.Values)
            {
                var cols = new JsonArray();
                foreach (var c in t.Columns)
                {
                    var co = new JsonObject { ["name"] = c.Name, ["type"] = TypeName(c.Type) };
                    if (c.Size > 0) co["size"] = c.Size;
                    if (c.AutoIncrement) co["autoIncrement"] = true;
                    if (c.PrimaryKey) co["primaryKey"] = true;
                    cols.Add(co);
                }
                var rows = new JsonArray();
                foreach (var r in t.Rows)
                {
                    var ro = new JsonObject();
                    for (int i = 0; i < t.Columns.Count; i++) ro[t.Columns[i].Name] = ToJson(r[i]);
                    rows.Add(ro);
                }
                tables[t.Name] = new JsonObject { ["columns"] = cols, ["nextAutoId"] = t.NextAutoId, ["rows"] = rows };
            }
            var root = new JsonObject { ["format"] = FormatName, ["formatVersion"] = FormatVersion, ["tables"] = tables };
            string tmp = Path + ".tmp";
            File.WriteAllText(tmp, root.ToJsonString(new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }), new UTF8Encoding(false));
            File.Move(tmp, Path, true);
            _loadedStamp = File.GetLastWriteTimeUtc(Path);
            Dirty = false;
        }
    }

    public JsonTable GetTable(string name)
    {
        if (Tables.TryGetValue(name, out var t)) return t;
        throw new InvalidOperationException("Ядро СУБД не может найти входную таблицу или запрос '" + name + "'.");
    }

    // ------------------------------------------------------------------ values
    public static JsonColumnType ParseType(string s) => s?.ToLowerInvariant() switch
    {
        "text" or "char" or "varchar" => JsonColumnType.Text,
        "memo" or "longtext" => JsonColumnType.Memo,
        "double" or "float8" => JsonColumnType.Double,
        "float" or "single" or "real" => JsonColumnType.Float,
        "int32" or "long" or "integer" or "int" or "counter" or "autoincrement" => JsonColumnType.Int32,
        "int16" or "short" or "smallint" => JsonColumnType.Int16,
        "byte" => JsonColumnType.Byte,
        "bool" or "bit" or "yesno" => JsonColumnType.Bool,
        "datetime" or "date" => JsonColumnType.DateTime,
        _ => JsonColumnType.Text,
    };

    public static string TypeName(JsonColumnType t) => t switch
    {
        JsonColumnType.Text => "text", JsonColumnType.Memo => "memo", JsonColumnType.Double => "double",
        JsonColumnType.Float => "float", JsonColumnType.Int32 => "int32", JsonColumnType.Int16 => "int16",
        JsonColumnType.Byte => "byte", JsonColumnType.Bool => "bool", _ => "datetime",
    };

    private static object FromJson(JsonNode n, JsonColumnType t)
    {
        if (n == null) return t == JsonColumnType.Bool ? (object)false : DBNull.Value;
        var v = n.AsValue();
        switch (t)
        {
            case JsonColumnType.Double: return v.GetValue<double>();
            case JsonColumnType.Float: return (float)v.GetValue<double>();
            case JsonColumnType.Int32: return v.GetValue<int>();
            case JsonColumnType.Int16: return (short)v.GetValue<int>();
            case JsonColumnType.Byte: return (byte)v.GetValue<int>();
            case JsonColumnType.Bool: return v.GetValue<bool>();
            case JsonColumnType.DateTime: return DateTime.Parse(v.GetValue<string>(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            default: return v.GetValue<string>();
        }
    }

    private static JsonNode ToJson(object o) => o switch
    {
        null => null,
        DBNull => null,
        double d => double.IsFinite(d) ? JsonValue.Create(d) : null,
        float f => float.IsFinite(f) ? JsonValue.Create(f) : null,
        int i => JsonValue.Create(i),
        short s => JsonValue.Create(s),
        byte b => JsonValue.Create(b),
        bool b => JsonValue.Create(b),
        DateTime dt => JsonValue.Create(dt.ToString("o", CultureInfo.InvariantCulture)),
        _ => JsonValue.Create(o.ToString()),
    };

    /// <summary>
    /// Converts a SQL literal to a column's type the way the Jet engine did on a
    /// Russian-locale Windows: text such as '12,5' is accepted for numeric columns.
    /// </summary>
    public static object Coerce(object value, JsonColumn col)
    {
        if (value == null || value is DBNull) return col.Type == JsonColumnType.Bool ? (object)false : DBNull.Value;
        switch (col.Type)
        {
            case JsonColumnType.Text:
            case JsonColumnType.Memo:
                string s = value is IFormattable f ? f.ToString(null, CultureInfo.CurrentCulture) : value.ToString();
                if (col.Type == JsonColumnType.Text && col.Size > 0 && s.Length > col.Size)
                    throw new InvalidOperationException("Поле слишком мало для принятия такого количества данных.");
                return s;
            case JsonColumnType.Double: return ToDouble(value);
            case JsonColumnType.Float: return (float)ToDouble(value);
            case JsonColumnType.Int32: return (int)Math.Round(ToDouble(value), MidpointRounding.ToEven);
            case JsonColumnType.Int16: return (short)Math.Round(ToDouble(value), MidpointRounding.ToEven);
            case JsonColumnType.Byte: return (byte)Math.Round(ToDouble(value), MidpointRounding.ToEven);
            case JsonColumnType.Bool:
                if (value is bool b) return b;
                if (value is string bs) return bs.Equals("true", StringComparison.OrdinalIgnoreCase) || bs.Equals("yes", StringComparison.OrdinalIgnoreCase) || (double.TryParse(bs, NumberStyles.Float, CultureInfo.InvariantCulture, out var bd) && bd != 0);
                return ToDouble(value) != 0;
            default:
                return value is DateTime dt ? dt : DateTime.Parse(value.ToString(), CultureInfo.CurrentCulture);
        }
    }

    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

    public static double ToDouble(object value)
    {
        switch (value)
        {
            case double d: return d;
            case float f: return f;
            case int i: return i;
            case short s: return s;
            case byte b: return b;
            case bool bo: return bo ? -1 : 0;
        }
        string str = value.ToString().Trim();
        if (str.Length == 0) throw new InvalidOperationException("Несоответствие типов данных в выражении условия отбора.");
        if (double.TryParse(str, NumberStyles.Float, Ru, out var r)) return r;
        if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out r)) return r;
        throw new InvalidOperationException("Несоответствие типов данных в выражении условия отбора.");
    }

    /// <summary>Creates a new, empty database file with the given tables.</summary>
    public static JsonDatabase Create(string path, IEnumerable<JsonTable> tables)
    {
        var db = new JsonDatabase { Path = System.IO.Path.GetFullPath(path) };
        foreach (var t in tables) db.Tables[t.Name] = t;
        db.Save();
        lock (_cacheLock) _cache[db.Path] = db;
        return db;
    }

    /// <summary>Drops the cached instance (e.g. after the file was replaced).</summary>
    public static void Forget(string path)
    {
        lock (_cacheLock) _cache.Remove(System.IO.Path.GetFullPath(path));
    }
}
