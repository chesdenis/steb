using System;
using STEB2.Geometry;
using STEB2.Volume;
using Xunit;

namespace STEB.Core.Tests;

/// <summary>Hydrostatics against closed-form results for a box barge and a wedge prism.</summary>
[Collection(EngineCollection.Name)]
public class HydrostaticsTests
{
    private const float L = 20f, B = 6f, H = 4f, T = 2f, Rho = 1.025f;

    private static Waterlanding_WaterlinePosition FloatBoxAt(float draft) =>
        new Waterlanding_WaterlinePosition(TestHulls.Box(L, B, H), 0f, 0f, new p3d(0, 0, draft), Rho);

    [Fact]
    public void Volume_IsLBT()
    {
        var w = FloatBoxAt(T);
        Assert.Equal(L * B * T, w.V, 1);
    }

    [Fact]
    public void Displacement_IsVolumeTimesDensity()
    {
        var w = FloatBoxAt(T);
        Assert.Equal(L * B * T * Rho, w.DSPL, 1);
    }

    [Fact]
    public void CentreOfBuoyancy_IsAtHalfLengthOnCentrelineAtHalfDraft()
    {
        var w = FloatBoxAt(T);
        Assert.Equal(L / 2, w.CPos.X, 2);
        Assert.Equal(0f, w.CPos.Y, 2);
        Assert.Equal(T / 2, w.CPos.Z, 2);
    }

    [Fact]
    public void WaterlineDimensionsAndDrafts()
    {
        var w = FloatBoxAt(T);
        w.GetWLGabarite_DNDKDLBDPB();
        Assert.Equal(L, w.Lwl, 2);
        Assert.Equal(B, w.Bwl, 2);
        Assert.Equal(T, w.d_lb, 2);
        Assert.Equal(T, w.d_pb, 2);
    }

    [Fact]
    public void WaterplaneArea_AndCentre()
    {
        var w = FloatBoxAt(T);
        w.CalcWaterlineGeometricCharacteristic();
        Assert.Equal(L * B, w.S, 0);
        Assert.Equal(L / 2, w.xf, 2);
    }

    [Fact]
    public void TransverseMetacentricRadius_ApproachesBSquaredOver12T()
    {
        // r is computed numerically by Waterlanding.GetMetacentricH (small-heel shift of the centre of buoyancy)
        var w = FloatBoxAt(T);
        w.CalcWaterlineGeometricCharacteristic();
        float expected = B * B / (12 * T);
        Assert.InRange(w.r, expected * 0.9f, expected * 1.1f);
    }

    [Fact]
    public void WedgeVolume_IsHalfOfCircumscribedBox()
    {
        // triangular section: breadth at the waterline = B*T/H, area = Bwl*T/2
        var w = new Waterlanding_WaterlinePosition(TestHulls.Wedge(L, B, H), 0f, 0f, new p3d(0, 0, T), Rho);
        float bwl = B * T / H;
        Assert.Equal(L * bwl * T / 2, w.V, 1);
        Assert.Equal(2f * T / 3f, w.CPos.Z, 2);   // centroid of a triangle with apex at the keel
    }

    [Fact]
    public void DraftFromVolume_BoxBarge()
    {
        float volume = L * B * 1.5f;
        var w = new Waterlanding_Volume_Heel_Trim(volume, 0f, 0f, TestHulls.Box(L, B, H), 0.001f, Rho);
        Assert.Equal(volume, w.V, 0);
        Assert.InRange(w.wlPosition.Z, 1.5f * 0.99f, 1.5f * 1.01f);
    }

    [Fact]
    public void DeeperDraft_GivesMoreVolume()
    {
        Assert.True(FloatBoxAt(2.5f).V > FloatBoxAt(2.0f).V);
    }

    [Fact]
    [Trait("Category", "Slow")]
    public void Equilibrium_CentredWeight_FloatsUprightAndLevelAtTheExpectedDraft()
    {
        // mass = displacement at T; G on the centreline at mid-length, low enough to be stable (KG < KB + BM)
        float mass = L * B * T * Rho;
        var w = new Waterlanding_MassGpos(mass, new p3d(L / 2, 0, 1.0f), TestHulls.Box(L, B, H, 9), 0.001f, Rho);
        Assert.InRange(Math.Abs(w.HeelAngle), 0f, 1f);
        Assert.InRange(Math.Abs(w.TrimAngle), 0f, 1f);
        Assert.InRange(w.V, mass / Rho * 0.99f, mass / Rho * 1.01f);
    }
}
