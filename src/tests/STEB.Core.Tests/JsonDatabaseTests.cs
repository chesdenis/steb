using System;
using System.Collections.Generic;
using System.IO;
using STEB2.DataBase.Json;
using Xunit;

namespace STEB.Core.Tests;

/// <summary>
/// The JSON provider that replaces Access/OleDb. Every test works on its own temporary file
/// (no shared state with other tests), so they can run in parallel and in any order.
/// </summary>
[Collection(EngineCollection.Name)]
public sealed class JsonDatabaseTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), "steb-test-" + Guid.NewGuid().ToString("n") + ".xsteb.json");
    private string Cnn => STEB2.DataBase.Tools.cnnString + _path;

    public JsonDatabaseTests()
    {
        JsonDatabase.Create(_path, new[]
        {
            Table("tblShipForms", ("ShipFormName", JsonColumnType.Text, false), ("L", JsonColumnType.Double, false), ("textHeight", JsonColumnType.Float, false)),
            Table("tblAquatories", ("ID", JsonColumnType.Int32, true), ("Name", JsonColumnType.Text, false), ("AquaDensity", JsonColumnType.Double, false)),
            Table("tblStations", ("ShipFormName", JsonColumnType.Text, false), ("StationID", JsonColumnType.Int32, true), ("X", JsonColumnType.Double, false)),
            Table("tblStationsConturs", ("ConturID", JsonColumnType.Int32, true), ("StationID", JsonColumnType.Int32, false)),
            Table("tblStationsContursPoints", ("ConturID", JsonColumnType.Int32, false), ("PointX", JsonColumnType.Double, false), ("PointY", JsonColumnType.Double, false), ("PointID", JsonColumnType.Int32, true)),
        });
    }

    public void Dispose()
    {
        JsonDatabase.Forget(_path);
        try { File.Delete(_path); } catch { }
    }

    private static JsonTable Table(string name, params (string Name, JsonColumnType Type, bool Auto)[] cols)
    {
        var t = new JsonTable { Name = name };
        foreach (var c in cols) t.Columns.Add(new JsonColumn { Name = c.Name, Type = c.Type, Size = c.Type == JsonColumnType.Text ? 100 : 0, AutoIncrement = c.Auto });
        return t;
    }

    private int Exec(JsonDbConnection cn, string sql) => new JsonDbCommand(sql, cn).ExecuteNonQuery();

    private List<object[]> Query(JsonDbConnection cn, string sql)
    {
        var rows = new List<object[]>();
        using var r = new JsonDbCommand(sql, cn).ExecuteReader();
        while (r.Read()) { var v = new object[r.FieldCount]; for (int i = 0; i < v.Length; i++) v[i] = r[i]; rows.Add(v); }
        return rows;
    }

    [Fact]
    public void InsertThenSelect_WhereIsCaseInsensitive()
    {
        using var cn = new JsonDbConnection(Cnn); cn.Open();
        Exec(cn, "INSERT INTO tblShipForms (ShipFormName, L) VALUES ('Base', 12.5)");
        var rows = Query(cn, "SELECT * FROM tblShipForms WHERE ShipFormName = 'base'");
        Assert.Single(rows);
        Assert.Equal(12.5, (double)rows[0][1]);
    }

    [Fact]
    public void AutoNumber_IncrementsAndMaxReturnsLast()
    {
        using var cn = new JsonDbConnection(Cnn); cn.Open();
        Exec(cn, "INSERT INTO tblAquatories (Name, AquaDensity) VALUES ('Море', 1.025)");
        Exec(cn, "INSERT INTO tblAquatories (Name, AquaDensity) VALUES ('Река', 1.0)");
        var max = STEB2.DataBase.Tools.ReturnScalar("SELECT MAX(ID) AS maxID FROM tblAquatories", "maxID", cn);
        Assert.Equal(2, System.Convert.ToInt32(max));
    }

    [Fact]
    public void TextValueForNumericColumn_IsConvertedLikeJetOnRussianWindows()
    {
        using var cn = new JsonDbConnection(Cnn); cn.Open();
        Exec(cn, "INSERT INTO tblShipForms (ShipFormName, L) VALUES ('base', 1)");
        Exec(cn, "UPDATE tblShipForms SET L = '12,5' WHERE ShipFormName = 'base'");
        Assert.Equal(12.5, (double)STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "L", cn));
    }

    [Fact]
    public void FloatColumn_KeepsSinglePrecisionType()
    {
        using var cn = new JsonDbConnection(Cnn); cn.Open();
        Exec(cn, "INSERT INTO tblShipForms (ShipFormName, textHeight) VALUES ('base', 7)");
        Assert.IsType<float>(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "textHeight", cn));
    }

    [Fact]
    public void DeleteStarWithWhere_RemovesMatchingRowsOnly()
    {
        using var cn = new JsonDbConnection(Cnn); cn.Open();
        Exec(cn, "INSERT INTO tblAquatories (Name, AquaDensity) VALUES ('A', 1)");
        Exec(cn, "INSERT INTO tblAquatories (Name, AquaDensity) VALUES ('B', 1)");
        Assert.Equal(1, Exec(cn, "DELETE * FROM tblAquatories WHERE ID=1"));
        Assert.Single(Query(cn, "SELECT * FROM tblAquatories"));
    }

    [Fact]
    public void OrderBy_SortsAscending()
    {
        using var cn = new JsonDbConnection(Cnn); cn.Open();
        Exec(cn, "INSERT INTO tblStations (ShipFormName, X) VALUES ('base', 5)");
        Exec(cn, "INSERT INTO tblStations (ShipFormName, X) VALUES ('base', 1)");
        var xs = STEB2.DataBase.Tools.ReturnScalarArray("SELECT * FROM tblStations ORDER BY X", "X", cn);
        Assert.Equal(new[] { "1", "5" }, xs);
    }

    [Fact]
    public void ShipFormText_FourTableJoin_RebuildsTheHullDescription()
    {
        using var cn = new JsonDbConnection(Cnn); cn.Open();
        Exec(cn, "INSERT INTO tblShipForms (ShipFormName, L) VALUES ('base', 0)");
        Exec(cn, "INSERT INTO tblStations (ShipFormName, X) VALUES ('base', 0)");
        Exec(cn, "INSERT INTO tblStationsConturs (StationID) VALUES (1)");
        Exec(cn, "INSERT INTO tblStationsContursPoints (ConturID, PointX, PointY) VALUES (1, -1, 0)");
        Exec(cn, "INSERT INTO tblStationsContursPoints (ConturID, PointX, PointY) VALUES (1, 1, 2)");
        string text = STEB2.DataBase.Tools.ReturnShipFormText("base", cn).Replace("\r\n", "\n");
        Assert.Equal("new 0\nY = -1 Z = 0\nY = 1 Z = 2\n", text);
    }

    [Fact]
    public void CreateTable_WithPrimaryKey_RejectsDuplicates()
    {
        using var cn = new JsonDbConnection(Cnn); cn.Open();
        Exec(cn, "CREATE TABLE tblCisterns (Name CHAR CONSTRAINT key1 PRIMARY KEY, x0 FLOAT, TextDecriptionOfStation TEXT)");
        Exec(cn, "INSERT INTO tblCisterns (Name, x0, TextDecriptionOfStation) VALUES ('T1', 1, 'new 0')");
        Assert.Throws<InvalidOperationException>(() => Exec(cn, "INSERT INTO tblCisterns (Name, x0, TextDecriptionOfStation) VALUES ('T1', 2, 'new 0')"));
    }

    [Fact]
    public void UnknownTable_Throws()
    {
        using var cn = new JsonDbConnection(Cnn); cn.Open();
        Assert.Throws<InvalidOperationException>(() => Query(cn, "SELECT * FROM tblNothing"));
    }

    [Fact]
    public void Changes_ArePersistedToTheFileOnClose()
    {
        using (var cn = new JsonDbConnection(Cnn)) { cn.Open(); Exec(cn, "INSERT INTO tblAquatories (Name, AquaDensity) VALUES ('Море', 1.025)"); }
        var reloaded = JsonDatabase.Load(_path);
        Assert.Single(reloaded.GetTable("tblAquatories").Rows);
        Assert.Equal(2, reloaded.GetTable("tblAquatories").NextAutoId);
    }
}
