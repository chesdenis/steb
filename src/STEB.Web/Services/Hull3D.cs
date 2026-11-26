using System;
using System.Collections.Generic;
using System.Linq;
using STEB2.DataBase.Json;
using STEB2.Geometry;
using STEB2.GraphicsElement;
using STEB2.Stations;

namespace STEB.Web.Services;

/// <summary>
/// Geometry for the 3D hull view (web addition, display only).
/// Uses the same stations as the control panel (<see cref="GetProjectElements.GetProjectStation"/>)
/// and derives the lines a naval architect would draw: waterlines, sheer, keel and the design waterline.
/// Coordinates: X along the length, Y across (from the centreline), Z up (from the baseline), metres.
/// </summary>
public static class Hull3D
{
    public sealed class Model
    {
        public List<Station3D> Stations { get; } = new();
        /// <summary>Polylines along the hull; each point is [x, y, z].</summary>
        public List<float[][]> Waterlines { get; } = new();
        public List<float[][]> Sheer { get; } = new();
        public List<float[][]> Keel { get; } = new();
        public List<float[][]> DesignWaterline { get; } = new();
        public float L { get; set; }
        public float B { get; set; }
        public float H { get; set; }
        public float DesignDraft { get; set; }
        public float[] Min { get; set; }
        public float[] Max { get; set; }
        /// <summary>Compartments, tanks and cargo items as shaded solids, with their liquid.</summary>
        public List<Solid> Solids { get; } = new();
    }

    public sealed class Solid
    {
        public string Name { get; set; }
        /// <summary>compartment, tank or cargo.</summary>
        public string Kind { get; set; }
        /// <summary>Filling, % of the volume (compartments and tanks).</summary>
        public float Percent { get; set; }
        /// <summary>Liquid level z, m (ship upright), or null when empty.</summary>
        public float? Level { get; set; }
        /// <summary>Polygons of the solid's surface; each point is [x, y, z].</summary>
        public List<float[][]> Faces { get; } = new();
        /// <summary>Polygons of the liquid's surface (same shape cut at the level).</summary>
        public List<float[][]> Liquid { get; } = new();
        /// <summary>Where to put the name label: [x, y, z].</summary>
        public float[] Label { get; set; }
    }

    public sealed class Station3D
    {
        public float X { get; set; }
        /// <summary>Contours of the station; each point is [y, z].</summary>
        public List<float[][]> Contours { get; } = new();
    }

    public static Model Load(string workFile, int waterlineCount = 8)
    {
        using var cn = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + workFile);
        cn.Open();
        string form = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", cn).ToString();
        var stations = GetProjectElements.GetProjectStation(form, cn);
        string[] dims = STEB2.DataBase.Tools.ReturnArrayOfResultByArrayOfFields(
            "SELECT * FROM tblShipForms WHERE ShipFormName='" + form + "';", new[] { "L", "B", "H", "d" }, cn);
        cn.Close();

        var m = new Model
        {
            L = STEB2.Converting.Converting.TextToFloat(dims[0]),
            B = STEB2.Converting.Converting.TextToFloat(dims[1]),
            H = STEB2.Converting.Converting.TextToFloat(dims[2]),
            DesignDraft = STEB2.Converting.Converting.TextToFloat(dims[3]),
        };
        var hull = stations.OrderBy(s => s.X).ToList();
        Build(m, hull, waterlineCount);
        AddLoads(m, hull);
        return m;
    }

    // ------------------------------------------------------------------ compartments, tanks, cargo

    private const int Ring = 28; // points per resampled section contour

    /// <summary>
    /// Reads compartments, tanks and cargo with the program's own controls (the same lists the control panel uses),
    /// clips compartments by the hull exactly as the calculation does, and turns their stations into closed solids.
    /// Liquid levels are computed with the program's volume-to-level routine for the ship upright.
    /// </summary>
    private static void AddLoads(Model m, List<Station> hull)
    {
        try
        {
            foreach (var c in new STEB2.Controls.ProjectCompartments().GetCompartment())
            {
                try
                {
                    c.CalcCompartment(hull);
                    float? level = LiquidLevel(c.CompartmentStations, c.percentOfVolume, c.volumeDensity);
                    m.Solids.Add(MakeSolid(c.Name, "compartment", c.percentOfVolume, level, c.CompartmentStations));
                }
                catch { /* a compartment outside the hull simply isn't drawn */ }
            }
        }
        catch { }
        try
        {
            foreach (var t in new STEB2.Controls.ProjectCisterns().GetCisterns())
            {
                try
                {
                    float? level = t.percentOfVolume <= 0f ? null
                        : t.percentOfVolume >= 99.9f ? Top(t.Stations)
                        : t.wtrl?.wlPosition.Z;
                    m.Solids.Add(MakeSolid(t.Name, "tank", t.percentOfVolume, level, t.Stations));
                }
                catch { }
            }
        }
        catch { }
        try
        {
            foreach (var b in new STEB2.Controls.ProjectContainers().GetContainers())
            {
                if (b.l <= 0 || b.b <= 0 || b.h <= 0) continue;
                // position = aft end, centreline of the box, bottom (as in the weight calculation)
                float x0 = b.pos.X, x1 = b.pos.X + b.l, y0 = b.pos.Y - b.b / 2, y1 = b.pos.Y + b.b / 2, z0 = b.pos.Z, z1 = b.pos.Z + b.h;
                var solid = new Solid { Name = b.Name, Kind = "cargo", Label = new[] { (x0 + x1) / 2, (y0 + y1) / 2, z1 } };
                solid.Faces.AddRange(Box(x0, x1, y0, y1, z0, z1));
                m.Solids.Add(solid);
            }
        }
        catch { }
    }

    private static float? LiquidLevel(List<Station> stations, float percent, float density)
    {
        if (percent <= 0f || stations == null || stations.Count < 2) return null;
        if (percent >= 99.9f) return Top(stations);
        float volume = STEB2.Volume.Tools.GetMaxVolume(stations) * percent / 100f;
        var w = new STEB2.Volume.Waterlanding_Volume_Heel_Trim(volume, 0f, 0f, stations, 0.001f, density > 0 ? density : 1f);
        return w.wlPosition.Z;
    }

    private static float Top(List<Station> stations) =>
        stations.SelectMany(s => s.StationContur.arrayConturs).SelectMany(c => c.arrayPoints).Max(p => p.Y);

    private static Solid MakeSolid(string name, string kind, float percent, float? level, List<Station> stations)
    {
        var solid = new Solid { Name = name, Kind = kind, Percent = percent, Level = level };
        var secs = stations.OrderBy(s => s.X)
            .Select(s => (s.X, contours: s.StationContur.arrayConturs.Select(c => c.arrayPoints).Where(Usable).ToList()))
            .Where(s => s.contours.Count > 0).ToList();
        if (secs.Count == 0) return solid;
        solid.Faces.AddRange(Mesh(secs));
        if (level.HasValue)
        {
            float z = level.Value;
            var cut = secs.Select(s => (s.X, contours: s.contours.Select(c => ClipBelow(c, z)).Where(Usable).ToList()))
                          .Where(s => s.contours.Count > 0).ToList();
            solid.Liquid.AddRange(Mesh(cut));
        }
        var all = secs.SelectMany(s => s.contours.SelectMany(c => c.Select(p => (s.X, p.X, p.Y)))).ToList();
        solid.Label = new[] { (all.Min(a => a.Item1) + all.Max(a => a.Item1)) / 2, all.Average(a => a.Item2), all.Max(a => a.Item3) };
        return solid;
    }

    private static bool Usable(p2d[] pts) => pts != null && pts.Length >= 3 && Math.Abs(SignedArea(pts)) > 1e-6f;

    /// <summary>Closed solid from consecutive sections: side quads between matching contours plus end caps.</summary>
    private static IEnumerable<float[][]> Mesh(List<(float X, List<p2d[]> contours)> secs)
    {
        var rings = secs.Select(s => (s.X, rings: s.contours.Select(Resample).OrderBy(r => r.Average(p => p.X)).ToList())).ToList();
        for (int i = 1; i < rings.Count; i++)
        {
            var a = rings[i - 1]; var b = rings[i];
            if (a.rings.Count != b.rings.Count) continue;
            for (int j = 0; j < a.rings.Count; j++)
                for (int k = 0; k < Ring; k++)
                {
                    int n = (k + 1) % Ring;
                    yield return new[]
                    {
                        P(a.X, a.rings[j][k]), P(a.X, a.rings[j][n]), P(b.X, b.rings[j][n]), P(b.X, b.rings[j][k])
                    };
                }
        }
        foreach (var end in new[] { rings[0], rings[^1] })
            foreach (var r in end.rings)
                yield return r.Select(p => P(end.X, p)).ToArray();
    }

    private static float[] P(float x, p2d p) => new[] { x, p.X, p.Y };

    /// <summary>
    /// Resamples a section contour to <see cref="Ring"/> points by arc length, counter-clockwise, starting where the
    /// contour crosses the vertical through its centroid at the bottom — so neighbouring sections line up.
    /// </summary>
    private static p2d[] Resample(p2d[] src)
    {
        var pts = src.ToList();
        if (pts.Count > 1 && pts[0].X == pts[^1].X && pts[0].Y == pts[^1].Y) pts.RemoveAt(pts.Count - 1);
        if (SignedArea(pts.ToArray()) < 0) pts.Reverse();
        float cy = pts.Average(p => p.X), cz = pts.Average(p => p.Y);
        // start: lowest crossing of the vertical y = cy
        int startEdge = -1; float startT = 0, bestZ = float.MaxValue;
        for (int i = 0; i < pts.Count; i++)
        {
            var a = pts[i]; var b = pts[(i + 1) % pts.Count];
            if ((a.X - cy) * (b.X - cy) > 0 || a.X == b.X) continue;
            float t = (cy - a.X) / (b.X - a.X), z = a.Y + t * (b.Y - a.Y);
            if (z < bestZ) { bestZ = z; startEdge = i; startT = t; }
        }
        if (startEdge < 0) { startEdge = pts.IndexOf(pts.OrderBy(p => p.Y).First()); startT = 0; }
        var ring = new List<p2d>();
        var s0 = pts[startEdge]; var s1 = pts[(startEdge + 1) % pts.Count];
        ring.Add(new p2d(s0.X + startT * (s1.X - s0.X), s0.Y + startT * (s1.Y - s0.Y)));
        for (int i = 1; i <= pts.Count; i++) ring.Add(pts[(startEdge + i) % pts.Count]);
        ring[^1] = ring[0];
        // arc-length parameterisation
        var len = new float[ring.Count];
        for (int i = 1; i < ring.Count; i++) len[i] = len[i - 1] + Dist(ring[i - 1], ring[i]);
        float total = len[^1];
        var res = new p2d[Ring];
        int seg = 1;
        for (int k = 0; k < Ring; k++)
        {
            float target = total * k / Ring;
            while (seg < ring.Count - 1 && len[seg] < target) seg++;
            float d = len[seg] - len[seg - 1], t = d > 0 ? (target - len[seg - 1]) / d : 0;
            res[k] = new p2d(ring[seg - 1].X + t * (ring[seg].X - ring[seg - 1].X), ring[seg - 1].Y + t * (ring[seg].Y - ring[seg - 1].Y));
        }
        return res;
    }

    private static float Dist(p2d a, p2d b) => (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

    private static float SignedArea(p2d[] p)
    {
        double s = 0;
        for (int i = 0; i < p.Length; i++) { var a = p[i]; var b = p[(i + 1) % p.Length]; s += (double)a.X * b.Y - (double)b.X * a.Y; }
        return (float)(s / 2);
    }

    /// <summary>Part of a section contour below z = level (Sutherland–Hodgman against one half-plane).</summary>
    private static p2d[] ClipBelow(p2d[] poly, float level)
    {
        var res = new List<p2d>();
        for (int i = 0; i < poly.Length; i++)
        {
            var a = poly[i]; var b = poly[(i + 1) % poly.Length];
            bool ina = a.Y <= level, inb = b.Y <= level;
            if (ina) res.Add(a);
            if (ina != inb)
            {
                float t = (level - a.Y) / (b.Y - a.Y);
                res.Add(new p2d(a.X + t * (b.X - a.X), level));
            }
        }
        return res.ToArray();
    }

    private static IEnumerable<float[][]> Box(float x0, float x1, float y0, float y1, float z0, float z1)
    {
        float[] V(float x, float y, float z) => new[] { x, y, z };
        yield return new[] { V(x0, y0, z0), V(x1, y0, z0), V(x1, y1, z0), V(x0, y1, z0) };
        yield return new[] { V(x0, y0, z1), V(x1, y0, z1), V(x1, y1, z1), V(x0, y1, z1) };
        yield return new[] { V(x0, y0, z0), V(x1, y0, z0), V(x1, y0, z1), V(x0, y0, z1) };
        yield return new[] { V(x0, y1, z0), V(x1, y1, z0), V(x1, y1, z1), V(x0, y1, z1) };
        yield return new[] { V(x0, y0, z0), V(x0, y1, z0), V(x0, y1, z1), V(x0, y0, z1) };
        yield return new[] { V(x1, y0, z0), V(x1, y1, z0), V(x1, y1, z1), V(x1, y0, z1) };
    }

    private static void Build(Model m, List<Station> stations, int waterlineCount)
    {
        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue, minZ = float.MaxValue, maxZ = float.MinValue;
        foreach (var st in stations)
        {
            var s3 = new Station3D { X = st.X };
            foreach (var c in st.StationContur.arrayConturs)
            {
                var pts = c.arrayPoints.Where(p => Finite(p.X) && Finite(p.Y)).Select(p => new[] { p.X, p.Y }).ToArray();
                if (pts.Length < 2) continue;
                s3.Contours.Add(pts);
                foreach (var p in pts)
                {
                    minY = Math.Min(minY, p[0]); maxY = Math.Max(maxY, p[0]);
                    minZ = Math.Min(minZ, p[1]); maxZ = Math.Max(maxZ, p[1]);
                }
            }
            if (s3.Contours.Count == 0) continue;
            minX = Math.Min(minX, st.X); maxX = Math.Max(maxX, st.X);
            m.Stations.Add(s3);
        }
        if (m.Stations.Count == 0) { m.Min = new float[3]; m.Max = new float[3]; return; }
        m.Min = new[] { minX, minY, minZ };
        m.Max = new[] { maxX, maxY, maxZ };

        // waterlines: horizontal cuts at evenly spaced heights, outer breadth on each side
        for (int i = 1; i <= waterlineCount; i++)
        {
            float z = minZ + (maxZ - minZ) * i / (waterlineCount + 1);
            AddCut(m.Waterlines, m.Stations, z);
        }
        if (m.DesignDraft > 0 && m.DesignDraft > minZ && m.DesignDraft < maxZ)
            AddCut(m.DesignWaterline, m.Stations, m.DesignDraft);

        // sheer: the highest point on each side; keel: the lowest point of each station
        var port = new List<float[]>(); var stbd = new List<float[]>(); var keel = new List<float[]>();
        foreach (var s in m.Stations)
        {
            var all = s.Contours.SelectMany(c => c).ToList();
            float top = all.Max(p => p[1]);
            var topPts = all.Where(p => p[1] >= top - (maxZ - minZ) * 0.02f).ToList();
            stbd.Add(new[] { s.X, topPts.Max(p => p[0]), top });
            port.Add(new[] { s.X, topPts.Min(p => p[0]), top });
            var low = all.OrderBy(p => p[1]).ThenBy(p => Math.Abs(p[0])).First();
            keel.Add(new[] { s.X, low[0], low[1] });
        }
        m.Sheer.Add(port.ToArray()); m.Sheer.Add(stbd.ToArray()); m.Keel.Add(keel.ToArray());
    }

    /// <summary>Intersects every station with the plane z = const and joins the outermost points along the length.</summary>
    private static void AddCut(List<float[][]> target, List<Station3D> stations, float z)
    {
        var port = new List<float[]>(); var stbd = new List<float[]>();
        void Flush()
        {
            if (port.Count > 1) target.Add(port.ToArray());
            if (stbd.Count > 1) target.Add(stbd.ToArray());
            port = new List<float[]>(); stbd = new List<float[]>();
        }
        foreach (var s in stations)
        {
            var ys = new List<float>();
            foreach (var c in s.Contours)
                for (int i = 1; i < c.Length; i++)
                {
                    float z0 = c[i - 1][1], z1 = c[i][1];
                    if ((z0 - z) * (z1 - z) > 0 || z0 == z1) continue;
                    float t = (z - z0) / (z1 - z0);
                    ys.Add(c[i - 1][0] + t * (c[i][0] - c[i - 1][0]));
                }
            if (ys.Count == 0) { Flush(); continue; } // the cut leaves the hull: start a new line segment
            port.Add(new[] { s.X, ys.Min(), z });
            stbd.Add(new[] { s.X, ys.Max(), z });
        }
        Flush();
    }

    private static bool Finite(float v) => !float.IsNaN(v) && !float.IsInfinity(v);
}
