using System.Collections.Generic;
using STEB2.Cisterns;
using STEB2.Compartments;
using STEB2.Containers;
using STEB2.Geometry;
using Xunit;

namespace STEB.Core.Tests;

/// <summary>Weight summation: lightship + containers + tanks + flooded compartments.</summary>
[Collection(EngineCollection.Name)]
public class MassAndLoadTests
{
    /// <summary>Runs the program's weight summation and returns the "base" table total (mass and centre of gravity).</summary>
    private static (float Mass, p3d G) Total(float baseMass, p3d g, List<Compartment> comps, List<Container> conts, List<Cistern> tanks)
    {
        var t = STEB2.Mass.Tools.CalcMassGposByCompartmentsContainersBaseMassCisterns(baseMass, g, comps, conts, tanks, 0f, 0f, 0.001f);
        return (t[0].Mass, t[0].GPosition);
    }

    [Fact]
    public void LightshipOnly_ReturnsItsMassAndCentre()
    {
        var r = Total(100f, new p3d(10, 0, 2), new List<Compartment>(), new List<Container>(), new List<Cistern>());
        Assert.Equal(100f, r.Mass, 3);
        Assert.Equal(10f, r.G.X, 3); Assert.Equal(0f, r.G.Y, 3); Assert.Equal(2f, r.G.Z, 3);
    }

    [Fact]
    public void Container_AddsMassAtItsBoxCentre()
    {
        // container position = aft-bottom corner on its centreline: centre = (x + l/2, y, z + h/2)
        var c = new Container("C1", 2f, 2f, 1f, new p3d(4, 1, 4), 20f);
        var r = Total(100f, new p3d(10, 0, 2), new List<Compartment>(), new List<Container> { c }, new List<Cistern>());
        Assert.Equal(120f, r.Mass, 3);
        Assert.Equal((100 * 10f + 20 * 5f) / 120f, r.G.X, 3);
        Assert.Equal((20 * 1f) / 120f, r.G.Y, 3);
        Assert.Equal((100 * 2f + 20 * 4.5f) / 120f, r.G.Z, 3);
    }

    [Fact]
    public void Tank_FromBox_HasBoxVolumeAndMass()
    {
        var tank = new Cistern("T1", TestHulls.UnitPrism, new p3d(1, 2, 3), new p3d(3, 3, 4), 0.85f);
        Assert.Equal(2f, tank.maxVolume, 2);          // 2 x 1 x 1 m
        Assert.Equal(1.7f, tank.maxMass, 2);
        Assert.Equal(2f, tank.l, 2); Assert.Equal(1f, tank.b, 2); Assert.Equal(1f, tank.h, 2);
    }

    [Fact]
    public void HalfFullTank_AddsHalfItsMass()
    {
        var tank = new Cistern("T1", TestHulls.UnitPrism, new p3d(1, 2, 3), new p3d(3, 3, 4), 1.0f);
        tank.percentOfVolume = 50f;
        var r = Total(0f, new p3d(0, 0, 0), new List<Compartment>(), new List<Container>(), new List<Cistern> { tank });
        Assert.InRange(r.Mass, 0.99f, 1.01f);         // 50 % of 2 m3 x 1.0 t/m3
        Assert.InRange(r.G.Z, 3.2f, 3.3f);            // half-full: centre a quarter of the height above the bottom
    }

    [Fact]
    public void Compartment_InsideHull_HasBoxVolume()
    {
        var hull = TestHulls.Box(20, 6, 4, 21);
        var comp = new Compartment("K1", 4f, 2f, 1f, new p3d(8, 0, 1), 1.025f, 0f, 1f);
        comp.CalcCompartment(hull);
        Assert.InRange(STEB2.Volume.Tools.GetMaxVolume(comp.CompartmentStations), 7.9f, 8.1f);   // 4 x 2 x 1 m
    }

    [Fact]
    public void Compartment_IsClippedByTheHull()
    {
        var hull = TestHulls.Box(20, 6, 4, 21);
        // 10 m wide box on a 6 m wide hull: only the 6 m inside the hull count
        var comp = new Compartment("K1", 4f, 10f, 1f, new p3d(8, 0, 1), 1.025f, 0f, 1f);
        comp.CalcCompartment(hull);
        Assert.InRange(STEB2.Volume.Tools.GetMaxVolume(comp.CompartmentStations), 23.8f, 24.2f);  // 4 x 6 x 1 m
    }

    [Fact]
    public void FloodedCompartment_AddsWaterTimesPermeability()
    {
        var hull = TestHulls.Box(20, 6, 4, 21);
        var comp = new Compartment("K1", 4f, 2f, 1f, new p3d(8, 0, 1), 1.0f, 50f, 0.8f);
        comp.CalcCompartment(hull);
        var r = Total(0f, new p3d(0, 0, 0), new List<Compartment> { comp }, new List<Container>(), new List<Cistern>());
        Assert.InRange(r.Mass, 8f * 0.5f * 0.8f * 0.98f, 8f * 0.5f * 0.8f * 1.02f);
    }
}
