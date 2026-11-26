using STEB2.Geometry;
using Xunit;

namespace STEB.Core.Tests;

public class GeometryTests
{
    [Fact]
    public void VectorLength_Is345() =>
        Assert.Equal(5f, Mathematics.GetVectorLength(new[] { new p2d(0, 0), new p2d(3, 4) }), 4);

    [Fact]
    public void Vector3dLength() =>
        Assert.Equal(3f, Mathematics.Get3dVectorLength(new p3d(1, 2, 2)), 4);

    [Fact]
    public void BoundRect_ContainsAllPoints()
    {
        var r = Mathematics.GetBoundRect(new[] { new p2d(-1, 2), new p2d(3, -4), new p2d(0, 5) });
        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
        foreach (var p in r) { minX = System.Math.Min(minX, p.X); maxX = System.Math.Max(maxX, p.X); minY = System.Math.Min(minY, p.Y); maxY = System.Math.Max(maxY, p.Y); }
        Assert.Equal(-1f, minX); Assert.Equal(3f, maxX); Assert.Equal(-4f, minY); Assert.Equal(5f, maxY);
    }

    [Fact]
    public void GabariteOfBox_IsLxBxH()
    {
        var g = STEB2.Volume.Tools.GetGabariteOfVolume(TestHulls.Box(20, 6, 4));
        Assert.Equal(20f, g.X, 3); Assert.Equal(6f, g.Y, 3); Assert.Equal(4f, g.Z, 3);
    }

    [Fact]
    public void MaxVolumeOfBox_IsLBH() =>
        Assert.Equal(480f, STEB2.Volume.Tools.GetMaxVolume(TestHulls.Box(20, 6, 4)), 0);

    [Fact]
    public void AffineTransform_ScalesShapeToMainDimensions()
    {
        // a unit shape scaled to L/B/H, as GetProjectStation does with the project's L, B, H
        var scaled = STEB2.Volume.Tools.GetAffinnStations(TestHulls.Box(1, 1, 1), 50f, 8f, 5f, 0, 0, 0);
        var g = STEB2.Volume.Tools.GetGabariteOfVolume(scaled);
        Assert.Equal(50f, g.X, 2); Assert.Equal(8f, g.Y, 2); Assert.Equal(5f, g.Z, 2);
    }

    [Fact]
    public void AffineTransform_WithoutDimensions_UsesCoefficients()
    {
        var scaled = STEB2.Volume.Tools.GetAffinnStations(TestHulls.Box(1000, 300, 200), 0, 0, 0, 0.001f, 0.001f, 0.001f);
        var g = STEB2.Volume.Tools.GetGabariteOfVolume(scaled);
        Assert.Equal(1f, g.X, 3); Assert.Equal(0.3f, g.Y, 3); Assert.Equal(0.2f, g.Z, 3);
    }
}
