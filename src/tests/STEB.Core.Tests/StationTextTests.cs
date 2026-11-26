using System.Linq;
using Xunit;

namespace STEB.Core.Tests;

/// <summary>STEB2.Volume.Tools.TextToStations: the hull/tank description format ("new X" + "Y Z" pairs).</summary>
public class StationTextTests
{
    [Fact]
    public void ParsesStationsAndPoints()
    {
        var st = STEB2.Volume.Tools.TextToStations("new 0\nY = -1 Z = 0\nY = 1 Z = 0\nY = 1 Z = 2\nnew 5\nY = -1 Z = 0\nY = 1 Z = 0\nY = 1 Z = 2");
        Assert.Equal(2, st.Count);
        Assert.Equal(new[] { 0f, 5f }, st.Select(s => s.X).OrderBy(x => x).ToArray());
    }

    [Fact]
    public void OpenContour_IsClosed()
    {
        var st = STEB2.Volume.Tools.TextToStations("new 0\n-1 0\n1 0\n1 2");
        var pts = st[0].StationContur.arrayConturs[0].arrayPoints;
        Assert.Equal(pts[0].X, pts[pts.Length - 1].X);
        Assert.Equal(pts[0].Y, pts[pts.Length - 1].Y);
    }

    [Fact]
    public void Mirror_AddsTheOtherSideWithNegativeY()
    {
        var st = STEB2.Volume.Tools.TextToStations("new 0 mirror\n0 0\n1 0\n1 1");
        var pts = st[0].StationContur.arrayConturs[0].arrayPoints;
        Assert.Contains(pts, p => p.X == -1f && p.Y == 1f);
        Assert.Contains(pts, p => p.X == 1f && p.Y == 1f);
    }

    [Fact]
    public void SameX_IsMergedIntoOneStationWithTwoContours()
    {
        var st = STEB2.Volume.Tools.TextToStations("new 3\n0 0\n1 0\n1 1\nnew 3\n2 0\n3 0\n3 1");
        Assert.Single(st);
        Assert.Equal(2, st[0].StationContur.arrayConturs.Length);
    }

    [Fact]
    public void StationWithOddNumberOfValues_IsSkipped_AsInTheOriginal()
    {
        var st = STEB2.Volume.Tools.TextToStations("new 0\n0 0\n1 0\n1\nnew 1\n0 0\n1 0\n1 1");
        Assert.Single(st);
        Assert.Equal(1f, st[0].X);
    }

    [Fact]
    public void DecimalCommaAndPoint_AreBothAccepted()
    {
        var st = STEB2.Volume.Tools.TextToStations("new 2,5\nY = 0,5 Z = 0\nY = 1.5 Z = 1");
        Assert.Equal(2.5f, st[0].X, 4);
        Assert.Contains(st[0].StationContur.arrayConturs[0].arrayPoints, p => System.Math.Abs(p.X - 1.5f) < 1e-5);
    }
}
