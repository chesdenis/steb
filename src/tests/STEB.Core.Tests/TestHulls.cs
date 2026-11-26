using System.Collections.Generic;
using STEB2.Geometry;
using STEB2.Stations;

namespace STEB.Core.Tests;

/// <summary>Analytical test hulls (all values in metres).</summary>
internal static class TestHulls
{
    /// <summary>Box barge: rectangular sections B x H at X = 0..L (n stations), keel at z = 0, centred on y = 0.</summary>
    public static List<Station> Box(float L, float B, float H, int n = 11)
    {
        var list = new List<Station>();
        for (int i = 0; i < n; i++)
        {
            float x = L * i / (n - 1);
            var pts = new[]
            {
                new p2d(-B / 2, 0), new p2d(B / 2, 0), new p2d(B / 2, H), new p2d(-B / 2, H), new p2d(-B / 2, 0)
            };
            list.Add(new Station(new MultipleContur(new[] { new Contur(pts) }), x));
        }
        return list;
    }

    /// <summary>Wedge (V-shaped) prism: triangular sections, apex at the keel (z = 0), breadth B at z = H.</summary>
    public static List<Station> Wedge(float L, float B, float H, int n = 11)
    {
        var list = new List<Station>();
        for (int i = 0; i < n; i++)
        {
            float x = L * i / (n - 1);
            var pts = new[] { new p2d(0, 0), new p2d(B / 2, H), new p2d(-B / 2, H), new p2d(0, 0) };
            list.Add(new Station(new MultipleContur(new[] { new Contur(pts) }), x));
        }
        return list;
    }

    /// <summary>The program's default tank shape (unit prism over 11 stations), as written by CisternMainParameter.</summary>
    public static readonly string UnitPrism = BuildPrism();

    private static string BuildPrism()
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i <= 10; i++) sb.Append("new " + i + " \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 ");
        return sb.ToString();
    }
}
