using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace STEB2.DataBase.Json;

/// <summary>
/// A small interpreter for the Access-SQL subset used by STEB:
/// SELECT (incl. nested INNER JOIN, WHERE, ORDER BY, MAX/MIN/COUNT), INSERT,
/// UPDATE, DELETE [*] and CREATE TABLE. Keywords are case-insensitive and text
/// comparison is case-insensitive, as in Jet.
/// </summary>
internal static class JsonSql
{
    // =================================================================== tokens
    private enum K { Ident, Str, Num, Sym, End }
    private readonly struct Tok
    {
        public readonly K Kind; public readonly string Text;
        public Tok(K k, string t) { Kind = k; Text = t; }
        public bool Is(string kw) => (Kind == K.Ident || Kind == K.Sym) && string.Equals(Text, kw, StringComparison.OrdinalIgnoreCase);
        public override string ToString() => Text;
    }

    private static List<Tok> Lex(string sql)
    {
        var r = new List<Tok>(); int i = 0;
        while (i < sql.Length)
        {
            char c = sql[i];
            if (char.IsWhiteSpace(c)) { i++; continue; }
            if (c == '\'' || c == '"')
            {
                var sb = new StringBuilder(); i++;
                while (true)
                {
                    if (i >= sql.Length) throw new InvalidOperationException("Синтаксическая ошибка: незакрытая строка в выражении.");
                    if (sql[i] == c) { if (i + 1 < sql.Length && sql[i + 1] == c) { sb.Append(c); i += 2; continue; } i++; break; }
                    sb.Append(sql[i++]);
                }
                r.Add(new Tok(K.Str, sb.ToString())); continue;
            }
            if (c == '[')
            {
                int e = sql.IndexOf(']', i);
                if (e < 0) throw new InvalidOperationException("Синтаксическая ошибка в имени.");
                r.Add(new Tok(K.Ident, sql.Substring(i + 1, e - i - 1))); i = e + 1; continue;
            }
            if (char.IsDigit(c) || (c == '.' && i + 1 < sql.Length && char.IsDigit(sql[i + 1])))
            {
                int s = i; while (i < sql.Length && (char.IsDigit(sql[i]) || sql[i] == '.' || sql[i] == 'E' || sql[i] == 'e' || ((sql[i] == '-' || sql[i] == '+') && (sql[i - 1] == 'E' || sql[i - 1] == 'e')))) i++;
                r.Add(new Tok(K.Num, sql.Substring(s, i - s))); continue;
            }
            if (char.IsLetter(c) || c == '_')
            {
                int s = i; while (i < sql.Length && (char.IsLetterOrDigit(sql[i]) || sql[i] == '_')) i++;
                r.Add(new Tok(K.Ident, sql.Substring(s, i - s))); continue;
            }
            if ((c == '<' || c == '>') && i + 1 < sql.Length && (sql[i + 1] == '=' || (c == '<' && sql[i + 1] == '>')))
            { r.Add(new Tok(K.Sym, sql.Substring(i, 2))); i += 2; continue; }
            if ("(),;*=<>.-+".IndexOf(c) >= 0) { r.Add(new Tok(K.Sym, c.ToString())); i++; continue; }
            throw new InvalidOperationException("Синтаксическая ошибка: недопустимый символ '" + c + "'.");
        }
        r.Add(new Tok(K.End, ""));
        return r;
    }

    // =================================================================== AST
    internal abstract class Expr { }
    private sealed class Lit : Expr { public object V; }
    private sealed class Col : Expr { public string Table; public string Name; public override string ToString() => Table == null ? Name : Table + "." + Name; }
    private sealed class Bin : Expr { public string Op; public Expr L, R; }
    private sealed class Not : Expr { public Expr E; }
    private sealed class IsNull : Expr { public Expr E; public bool Negate; }
    private sealed class Agg : Expr { public string Fn; public Expr Arg; }

    private abstract class Source { }
    private sealed class TableSrc : Source { public string Name; }
    private sealed class JoinSrc : Source { public Source L, R; public Expr On; }

    private sealed class SelectItem { public Expr E; public string Alias; public bool Star; }

    // =================================================================== parser
    private sealed class P
    {
        private readonly List<Tok> t; private int p;
        public P(string sql) { t = Lex(sql); }
        public Tok Peek => t[p];
        public Tok Next() => t[p++];
        public bool Accept(string s) { if (t[p].Is(s)) { p++; return true; } return false; }
        public void Expect(string s) { if (!Accept(s)) throw new InvalidOperationException("Синтаксическая ошибка: ожидалось '" + s + "' вместо '" + t[p].Text + "'."); }
        public string Ident() { var k = Next(); if (k.Kind != K.Ident) throw new InvalidOperationException("Синтаксическая ошибка около '" + k.Text + "'."); return k.Text; }
        public void EndOfStatement() { Accept(";"); if (Peek.Kind != K.End) throw new InvalidOperationException("Синтаксическая ошибка: лишние символы в конце инструкции ('" + Peek.Text + "')."); }

        public Expr Expr() => Or();
        private Expr Or() { var l = And(); while (Accept("OR")) l = new Bin { Op = "OR", L = l, R = And() }; return l; }
        private Expr And() { var l = NotE(); while (Accept("AND")) l = new Bin { Op = "AND", L = l, R = NotE() }; return l; }
        private Expr NotE() { if (Accept("NOT")) return new Not { E = NotE() }; return Cmp(); }
        private Expr Cmp()
        {
            var l = Add();
            if (Accept("IS")) { bool neg = Accept("NOT"); Expect("NULL"); return new IsNull { E = l, Negate = neg }; }
            foreach (var op in new[] { "=", "<>", "<=", ">=", "<", ">" })
                if (Accept(op)) return new Bin { Op = op, L = l, R = Add() };
            if (Accept("LIKE")) return new Bin { Op = "LIKE", L = l, R = Add() };
            return l;
        }
        private Expr Add()
        {
            var l = Unary();
            while (true)
            {
                if (Accept("+")) l = new Bin { Op = "+", L = l, R = Unary() };
                else if (Accept("-")) l = new Bin { Op = "-", L = l, R = Unary() };
                else return l;
            }
        }
        private Expr Unary()
        {
            if (Accept("-")) { var e = Unary(); if (e is Lit li && li.V is double d) return new Lit { V = -d }; return new Bin { Op = "-", L = new Lit { V = 0.0 }, R = e }; }
            if (Accept("+")) return Unary();
            return Primary();
        }
        private Expr Primary()
        {
            var k = Peek;
            if (Accept("(")) { var e = Expr(); Expect(")"); return e; }
            if (k.Kind == K.Str) { p++; return new Lit { V = k.Text }; }
            if (k.Kind == K.Num) { p++; return new Lit { V = double.Parse(k.Text, CultureInfo.InvariantCulture) }; }
            if (k.Is("NULL")) { p++; return new Lit { V = DBNull.Value }; }
            if (k.Is("TRUE") || k.Is("YES")) { p++; return new Lit { V = true }; }
            if (k.Is("FALSE") || k.Is("NO")) { p++; return new Lit { V = false }; }
            if (k.Kind == K.Ident)
            {
                p++;
                if (Peek.Is("(") && (k.Is("MAX") || k.Is("MIN") || k.Is("COUNT") || k.Is("SUM") || k.Is("AVG")))
                {
                    p++; Expr arg = Accept("*") ? null : Expr(); Expect(")");
                    return new Agg { Fn = k.Text.ToUpperInvariant(), Arg = arg };
                }
                if (Accept(".")) return new Col { Table = k.Text, Name = Ident() };
                return new Col { Name = k.Text };
            }
            throw new InvalidOperationException("Синтаксическая ошибка около '" + k.Text + "'.");
        }

        public Source Source()
        {
            var l = SourcePrimary();
            while (true)
            {
                if (Accept("INNER")) Expect("JOIN"); else if (!Accept("JOIN")) break;
                var r = SourcePrimary(); Expect("ON");
                l = new JoinSrc { L = l, R = r, On = Expr() };
            }
            return l;
        }
        private Source SourcePrimary()
        {
            if (Accept("(")) { var s = Source(); Expect(")"); return s; }
            return new TableSrc { Name = Ident() };
        }
    }

    // =================================================================== rows
    /// <summary>A joined row: table name -> (table, row values).</summary>
    private sealed class Ctx
    {
        public readonly List<(JsonTable T, object[] R)> Parts = new List<(JsonTable, object[])>();
        public Ctx With((JsonTable, object[]) part) { var c = new Ctx(); c.Parts.AddRange(Parts); c.Parts.Add(part); return c; }
        public Ctx Concat(Ctx o) { var c = new Ctx(); c.Parts.AddRange(Parts); c.Parts.AddRange(o.Parts); return c; }

        public object Get(Col col)
        {
            foreach (var (t, r) in Parts)
            {
                if (col.Table != null && !string.Equals(t.Name, col.Table, StringComparison.OrdinalIgnoreCase)) continue;
                int i = t.IndexOf(col.Name);
                if (i >= 0) return r[i];
            }
            throw new IndexOutOfRangeException(col.ToString());
        }
    }

    private static List<Ctx> Rows(Source s, JsonDatabase db)
    {
        if (s is TableSrc ts)
        {
            var t = db.GetTable(ts.Name); var res = new List<Ctx>(t.Rows.Count);
            foreach (var r in t.Rows) res.Add(new Ctx().With((t, r)));
            return res;
        }
        var j = (JoinSrc)s;
        var left = Rows(j.L, db); var right = Rows(j.R, db); var outp = new List<Ctx>();
        foreach (var l in left)
            foreach (var r in right)
            {
                var c = l.Concat(r);
                if (Truthy(Eval(j.On, c))) outp.Add(c);
            }
        return outp;
    }

    // =================================================================== eval
    private static object Eval(Expr e, Ctx c)
    {
        switch (e)
        {
            case Lit l: return l.V;
            case Col col: return c.Get(col);
            case Not n: { var v = Eval(n.E, c); return v is DBNull ? DBNull.Value : (object)!Truthy(v); }
            case IsNull isn: { bool isNull = Eval(isn.E, c) is DBNull; return isn.Negate ? !isNull : isNull; }
            case Bin b:
                if (b.Op == "AND") { var lv = Eval(b.L, c); if (lv is bool lb && !lb) return false; var rv = Eval(b.R, c); return Truthy(lv) && Truthy(rv); }
                if (b.Op == "OR") return Truthy(Eval(b.L, c)) || Truthy(Eval(b.R, c));
                var a = Eval(b.L, c); var d = Eval(b.R, c);
                if (a is DBNull || d is DBNull) return DBNull.Value;
                switch (b.Op)
                {
                    case "+": if (a is string || d is string) return Convert.ToString(a, CultureInfo.CurrentCulture) + Convert.ToString(d, CultureInfo.CurrentCulture); return JsonDatabase.ToDouble(a) + JsonDatabase.ToDouble(d);
                    case "-": return JsonDatabase.ToDouble(a) - JsonDatabase.ToDouble(d);
                    case "LIKE": return Like(Convert.ToString(a, CultureInfo.CurrentCulture), Convert.ToString(d, CultureInfo.CurrentCulture));
                }
                int cmp = Compare(a, d);
                return b.Op switch { "=" => cmp == 0, "<>" => cmp != 0, "<" => cmp < 0, ">" => cmp > 0, "<=" => cmp <= 0, ">=" => cmp >= 0, _ => false };
        }
        throw new InvalidOperationException("Агрегатная функция недопустима в этом месте.");
    }

    private static bool Truthy(object v) => v is bool b ? b : !(v is DBNull) && v != null && JsonDatabase.ToDouble(v) != 0;

    private static bool IsNumber(object v) => v is double || v is float || v is int || v is short || v is byte || v is bool;

    internal static int Compare(object a, object b)
    {
        if (a is DBNull && b is DBNull) return 0;
        if (a is DBNull) return -1;
        if (b is DBNull) return 1;
        if (IsNumber(a) || IsNumber(b))
        {
            try { return JsonDatabase.ToDouble(a).CompareTo(JsonDatabase.ToDouble(b)); }
            catch (InvalidOperationException) { }
        }
        if (a is DateTime da && b is DateTime db) return da.CompareTo(db);
        return string.Compare(Convert.ToString(a, CultureInfo.CurrentCulture), Convert.ToString(b, CultureInfo.CurrentCulture), StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool Like(string s, string pattern)
    {
        var rx = new StringBuilder("^");
        foreach (char ch in pattern)
            rx.Append(ch == '*' || ch == '%' ? ".*" : ch == '?' || ch == '_' ? "." : System.Text.RegularExpressions.Regex.Escape(ch.ToString()));
        rx.Append('$');
        return System.Text.RegularExpressions.Regex.IsMatch(s ?? "", rx.ToString(), System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
    }

    // =================================================================== execution
    internal sealed class Result
    {
        public string[] Names = new string[0];
        public List<object[]> Rows = new List<object[]>();
        public int Affected;
    }

    public static Result Execute(string sql, JsonDatabase db)
    {
        var p = new P(sql);
        lock (db.SyncRoot)
        {
            if (p.Accept("SELECT")) return Select(p, db);
            if (p.Accept("INSERT")) return Insert(p, db);
            if (p.Accept("UPDATE")) return Update(p, db);
            if (p.Accept("DELETE")) return Delete(p, db);
            if (p.Accept("CREATE")) return Create(p, db);
            if (p.Accept("DROP")) { p.Expect("TABLE"); string n = p.Ident(); p.EndOfStatement(); db.GetTable(n); db.Tables.Remove(n); db.Dirty = true; return new Result(); }
        }
        throw new InvalidOperationException("Недопустимая инструкция SQL; предполагалось 'DELETE', 'INSERT', 'PROCEDURE', 'SELECT' или 'UPDATE'.");
    }

    private static Result Select(P p, JsonDatabase db)
    {
        p.Accept("DISTINCT");
        var items = new List<SelectItem>();
        do
        {
            if (p.Accept("*")) { items.Add(new SelectItem { Star = true }); continue; }
            var e = p.Expr(); string alias = null;
            if (p.Accept("AS")) alias = p.Ident();
            items.Add(new SelectItem { E = e, Alias = alias });
        } while (p.Accept(","));
        p.Expect("FROM");
        var src = p.Source();
        Expr where = null; if (p.Accept("WHERE")) where = p.Expr();
        var order = new List<(Expr E, bool Desc)>();
        if (p.Accept("ORDER")) { p.Expect("BY"); do { var e = p.Expr(); bool desc = p.Accept("DESC"); if (!desc) p.Accept("ASC"); order.Add((e, desc)); } while (p.Accept(",")); }
        p.EndOfStatement();

        var rows = Rows(src, db);
        if (where != null) rows = rows.FindAll(r => Eval(where, r) is bool b && b);
        if (order.Count > 0)
        {
            var keyed = new List<(Ctx C, object[] K, int I)>();
            for (int i = 0; i < rows.Count; i++) { var k = new object[order.Count]; for (int j = 0; j < order.Count; j++) k[j] = Eval(order[j].E, rows[i]); keyed.Add((rows[i], k, i)); }
            keyed.Sort((x, y) =>
            {
                for (int j = 0; j < order.Count; j++) { int c = Compare(x.K[j], y.K[j]); if (c != 0) return order[j].Desc ? -c : c; }
                return x.I.CompareTo(y.I);
            });
            rows = keyed.ConvertAll(k => k.C);
        }

        var res = new Result();
        bool aggregate = items.Exists(i => i.E is Agg);
        if (aggregate)
        {
            var names = new List<string>(); var vals = new List<object>();
            int n = 0;
            foreach (var it in items)
            {
                if (!(it.E is Agg ag)) throw new InvalidOperationException("Попытка выполнить запрос, который не включает указанное выражение как часть агрегатной функции.");
                n++;
                names.Add(it.Alias ?? ("Expr100" + n));
                object acc = DBNull.Value; long count = 0; double sum = 0;
                foreach (var r in rows)
                {
                    object v = ag.Arg == null ? 1 : Eval(ag.Arg, r);
                    if (v is DBNull) continue;
                    count++;
                    if (ag.Fn == "MAX") { if (acc is DBNull || Compare(v, acc) > 0) acc = v; }
                    else if (ag.Fn == "MIN") { if (acc is DBNull || Compare(v, acc) < 0) acc = v; }
                    else if (ag.Fn == "SUM" || ag.Fn == "AVG") sum += JsonDatabase.ToDouble(v);
                }
                vals.Add(ag.Fn switch { "COUNT" => (object)(int)count, "SUM" => count == 0 ? (object)DBNull.Value : sum, "AVG" => count == 0 ? (object)DBNull.Value : sum / count, _ => acc });
            }
            res.Names = names.ToArray(); res.Rows.Add(vals.ToArray());
            return res;
        }

        // column layout
        var cols = new List<(string Name, Func<Ctx, object> Get)>();
        var sample = rows.Count > 0 ? rows[0] : SampleCtx(src, db);
        foreach (var it in items)
        {
            if (it.Star)
            {
                foreach (var (t, _) in sample.Parts)
                    for (int i = 0; i < t.Columns.Count; i++)
                    {
                        int ci = i; var tbl = t;
                        string nm = sample.Parts.Count > 1 ? t.Name + "." + t.Columns[i].Name : t.Columns[i].Name;
                        cols.Add((nm, c => c.Parts.Find(x => x.T == tbl).R[ci]));
                    }
            }
            else
            {
                var e = it.E;
                string nm = it.Alias ?? (e is Col cc ? cc.Name : "Expr100" + (cols.Count + 1));
                cols.Add((nm, c => Eval(e, c)));
            }
        }
        res.Names = cols.ConvertAll(c => c.Name).ToArray();
        foreach (var r in rows) { var v = new object[cols.Count]; for (int i = 0; i < cols.Count; i++) v[i] = cols[i].Get(r); res.Rows.Add(v); }
        return res;
    }

    private static Ctx SampleCtx(Source s, JsonDatabase db)
    {
        if (s is TableSrc ts) { var t = db.GetTable(ts.Name); return new Ctx().With((t, new object[t.Columns.Count])); }
        var j = (JoinSrc)s; return SampleCtx(j.L, db).Concat(SampleCtx(j.R, db));
    }

    private static Result Insert(P p, JsonDatabase db)
    {
        p.Expect("INTO");
        var t = db.GetTable(p.Ident());
        var names = new List<string>();
        if (p.Accept("(")) { do names.Add(p.Ident()); while (p.Accept(",")); p.Expect(")"); }
        else foreach (var c in t.Columns) names.Add(c.Name);
        p.Expect("VALUES"); p.Expect("(");
        var vals = new List<object>();
        var empty = new Ctx();
        do vals.Add(Eval(p.Expr(), empty)); while (p.Accept(","));
        p.Expect(")"); p.EndOfStatement();
        if (vals.Count != names.Count) throw new InvalidOperationException("Число значений запроса не совпадает с числом конечных полей.");

        var row = new object[t.Columns.Count];
        for (int i = 0; i < row.Length; i++) row[i] = t.Columns[i].Type == JsonColumnType.Bool ? (object)false : DBNull.Value;
        var set = new bool[row.Length];
        for (int i = 0; i < names.Count; i++)
        {
            int ci = t.IndexOf(names[i]);
            if (ci < 0) throw new InvalidOperationException("Конечное поле '" + names[i] + "' в инструкции INSERT INTO неизвестно.");
            row[ci] = JsonDatabase.Coerce(vals[i], t.Columns[ci]); set[ci] = true;
        }
        for (int i = 0; i < row.Length; i++)
        {
            var c = t.Columns[i];
            if (c.AutoIncrement)
            {
                if (!set[i] || row[i] is DBNull) row[i] = t.NextAutoId++;
                else if (row[i] is int v && v >= t.NextAutoId) t.NextAutoId = v + 1;
            }
        }
        CheckPrimaryKey(t, row, -1);
        t.Rows.Add(row); db.Dirty = true;
        return new Result { Affected = 1 };
    }

    private static Result Update(P p, JsonDatabase db)
    {
        var t = db.GetTable(p.Ident());
        p.Expect("SET");
        var sets = new List<(int Col, Expr E)>();
        do
        {
            string n = p.Ident(); if (p.Accept(".")) n = p.Ident();
            int ci = t.IndexOf(n);
            if (ci < 0) throw new InvalidOperationException("Слишком мало параметров. Требуется 1."); // Jet treats unknown names as parameters
            p.Expect("="); sets.Add((ci, p.Expr()));
        } while (p.Accept(","));
        Expr where = null; if (p.Accept("WHERE")) where = p.Expr();
        p.EndOfStatement();
        int n2 = 0;
        for (int ri = 0; ri < t.Rows.Count; ri++)
        {
            var row = t.Rows[ri];
            var ctx = new Ctx().With((t, row));
            if (where != null && !(Eval(where, ctx) is bool b && b)) continue;
            var copy = (object[])row.Clone();
            foreach (var (ci, e) in sets) copy[ci] = JsonDatabase.Coerce(Eval(e, ctx), t.Columns[ci]);
            CheckPrimaryKey(t, copy, ri);
            t.Rows[ri] = copy; n2++;
        }
        if (n2 > 0) db.Dirty = true;
        return new Result { Affected = n2 };
    }

    private static Result Delete(P p, JsonDatabase db)
    {
        p.Accept("*");
        p.Expect("FROM");
        var t = db.GetTable(p.Ident());
        Expr where = null; if (p.Accept("WHERE")) where = p.Expr();
        p.EndOfStatement();
        int before = t.Rows.Count;
        if (where == null) t.Rows.Clear();
        else t.Rows.RemoveAll(r => Eval(where, new Ctx().With((t, r))) is bool b && b);
        int n = before - t.Rows.Count;
        if (n > 0) db.Dirty = true;
        return new Result { Affected = n };
    }

    private static Result Create(P p, JsonDatabase db)
    {
        p.Expect("TABLE");
        string name = p.Ident();
        if (db.Tables.ContainsKey(name)) throw new InvalidOperationException("Таблица '" + name + "' уже существует.");
        var t = new JsonTable { Name = name };
        p.Expect("(");
        do
        {
            var c = new JsonColumn { Name = p.Ident() };
            string type = p.Ident();
            c.Type = JsonDatabase.ParseType(type);
            if (type.Equals("COUNTER", StringComparison.OrdinalIgnoreCase) || type.Equals("AUTOINCREMENT", StringComparison.OrdinalIgnoreCase)) c.AutoIncrement = true;
            if (type.Equals("CHAR", StringComparison.OrdinalIgnoreCase) || type.Equals("TEXT", StringComparison.OrdinalIgnoreCase))
            {
                // Jet: CHAR without size = TEXT(255); TEXT without size = MEMO
                c.Type = type.Equals("TEXT", StringComparison.OrdinalIgnoreCase) ? JsonColumnType.Memo : JsonColumnType.Text;
                c.Size = 255;
            }
            if (p.Accept("("))
            {
                var sz = p.Next(); c.Size = int.Parse(sz.Text, CultureInfo.InvariantCulture); p.Expect(")");
                if (c.Type == JsonColumnType.Memo) c.Type = JsonColumnType.Text;
            }
            while (!(p.Peek.Is(",") || p.Peek.Is(")")) && p.Peek.Kind != K.End)
            {
                if (p.Accept("PRIMARY")) { p.Expect("KEY"); c.PrimaryKey = true; }
                else p.Next(); // CONSTRAINT name / NOT NULL ...
            }
            t.Columns.Add(c);
        } while (p.Accept(","));
        p.Expect(")"); p.EndOfStatement();
        db.Tables[name] = t; db.Dirty = true;
        return new Result();
    }

    private static void CheckPrimaryKey(JsonTable t, object[] row, int selfIndex)
    {
        for (int ci = 0; ci < t.Columns.Count; ci++)
        {
            if (!t.Columns[ci].PrimaryKey) continue;
            for (int i = 0; i < t.Rows.Count; i++)
                if (i != selfIndex && Compare(t.Rows[i][ci], row[ci]) == 0)
                    throw new InvalidOperationException("Изменения не были успешно внесены в таблицу, так как они приведут к появлению повторяющихся значений в индексе, первичном ключе или связи.");
        }
    }
}
