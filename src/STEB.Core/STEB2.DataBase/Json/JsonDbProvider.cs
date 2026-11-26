using System;
using System.Data;
using System.Text.RegularExpressions;

namespace STEB2.DataBase.Json;

/// <summary>
/// Drop-in replacement for System.Data.OleDb.OleDbConnection used by the original code.
/// Accepts the original Jet connection string and opens the JSON project file named
/// by "Data Source=".
/// </summary>
public sealed class JsonDbConnection : IDisposable
{
    private JsonDatabase _db;

    public JsonDbConnection() { }
    public JsonDbConnection(string connectionString) { ConnectionString = connectionString; }

    public string ConnectionString { get; set; } = "";
    public ConnectionState State { get; private set; } = ConnectionState.Closed;
    public string DataSource => ResolvePath(ConnectionString);
    internal JsonDatabase Database => _db ?? throw new InvalidOperationException("Для ExecuteReader требуется открытое и доступное подключение (Connection). Текущее состояние подключения: закрыто.");

    public void Open()
    {
        if (State == ConnectionState.Open) throw new InvalidOperationException("Подключение не было закрыто. Текущее состояние подключения: открыто.");
        string path = DataSource;
        if (string.IsNullOrWhiteSpace(path)) throw new InvalidOperationException("Не указан файл проекта.");
        _db = JsonDatabase.Open(path);
        State = ConnectionState.Open;
    }

    public void Close()
    {
        if (_db != null && _db.Dirty) _db.Save();
        _db = null;
        State = ConnectionState.Closed;
    }

    public void Dispose() => Close();

    public static string ResolvePath(string connectionString)
    {
        var m = Regex.Match(connectionString ?? "", @"Data Source\s*=\s*(?<p>[^;]*)", RegexOptions.IgnoreCase);
        string p = (m.Success ? m.Groups["p"].Value : connectionString ?? "").Trim().Trim('"');
        return p;
    }
}

/// <summary>Drop-in replacement for System.Data.OleDb.OleDbCommand.</summary>
public sealed class JsonDbCommand : IDisposable
{
    public JsonDbCommand() { }
    public JsonDbCommand(string commandText) { CommandText = commandText; }
    public JsonDbCommand(string commandText, JsonDbConnection connection) { CommandText = commandText; Connection = connection; }

    public string CommandText { get; set; } = "";
    public JsonDbConnection Connection { get; set; }

    public int ExecuteNonQuery()
    {
        var db = Connection?.Database ?? throw new InvalidOperationException("ExecuteNonQuery: свойство Connection не было инициализировано.");
        return JsonSql.Execute(CommandText, db).Affected;
    }

    public JsonDbDataReader ExecuteReader()
    {
        var db = Connection?.Database ?? throw new InvalidOperationException("ExecuteReader: свойство Connection не было инициализировано.");
        return new JsonDbDataReader(JsonSql.Execute(CommandText, db));
    }

    public object ExecuteScalar()
    {
        using var r = ExecuteReader();
        return r.Read() ? r[0] : null;
    }

    public void Dispose() { }
}

/// <summary>Drop-in replacement for System.Data.OleDb.OleDbDataReader (forward-only).</summary>
public sealed class JsonDbDataReader : IDisposable
{
    private readonly JsonSql.Result _r;
    private int _pos = -1;

    internal JsonDbDataReader(JsonSql.Result r) { _r = r; }

    public int FieldCount => _r.Names.Length;
    public bool HasRows => _r.Rows.Count > 0;
    public bool IsClosed { get; private set; }
    public int RecordsAffected => _r.Affected;

    public bool Read()
    {
        if (IsClosed) throw new InvalidOperationException("Недопустимая попытка вызова Read, когда объект чтения данных закрыт.");
        return ++_pos < _r.Rows.Count;
    }

    public object this[int i] => Row[i];

    public object this[string name]
    {
        get
        {
            int i = GetOrdinal(name);
            return Row[i];
        }
    }

    private object[] Row
    {
        get
        {
            if (_pos < 0 || _pos >= _r.Rows.Count) throw new InvalidOperationException("Отсутствуют данные для строки или столбца.");
            return _r.Rows[_pos];
        }
    }

    public int GetOrdinal(string name)
    {
        for (int i = 0; i < _r.Names.Length; i++)
            if (string.Equals(_r.Names[i], name, StringComparison.OrdinalIgnoreCase)) return i;
        // "tblX.Col" from a SELECT * over a join may be requested as just "Col"
        for (int i = 0; i < _r.Names.Length; i++)
        {
            string n = _r.Names[i]; int dot = n.LastIndexOf('.');
            if (dot >= 0 && string.Equals(n.Substring(dot + 1), name, StringComparison.OrdinalIgnoreCase)) return i;
        }
        throw new IndexOutOfRangeException(name);
    }

    public string GetName(int i) => _r.Names[i];
    public bool IsDBNull(int i) => Row[i] is DBNull;
    public object GetValue(int i) => Row[i];
    public string GetString(int i) => (string)Row[i];
    public double GetDouble(int i) => Convert.ToDouble(Row[i]);
    public float GetFloat(int i) => Convert.ToSingle(Row[i]);
    public int GetInt32(int i) => Convert.ToInt32(Row[i]);
    public bool GetBoolean(int i) => Convert.ToBoolean(Row[i]);

    public void Close() { IsClosed = true; }
    public void Dispose() => Close();
}
