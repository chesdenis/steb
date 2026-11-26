using System.Collections.Generic;
using STEB2.Cisterns;
using STEB2.Compartments;
using STEB2.Containers;
using STEB2.Geometry;
using STEB2.Mass;
using STEB2.Stability;
using STEB2.Stations;

namespace STEB2.Volume;

public class Waterlanding_MassGpos_Compartments_Containers_Cisterns : Waterlanding
{
	public Waterlanding_MassGpos_Compartments_Containers_Cisterns(float baseMass, p3d baseGPos, List<Compartment> compartments, List<Container> containers, List<Cistern> cisterns, List<Station> stations, float Precisy, float aquaDensity)
	{
		List<p2d> list = new List<p2d>();
		Progress.status = "Расчет дифферента...";
		Progress.ProgressMaximum = 0;
		for (float num = -90f; num <= 90f; num += 2f)
		{
			Progress.ProgressMaximum++;
		}
		for (float num2 = -90f; num2 <= 90f; num2 += 2f)
		{
			Progress.ProgressPosition++;
			List<MassTable> list2 = STEB2.Mass.Tools.CalcMassGposByCompartmentsContainersBaseMassCisterns(baseMass, baseGPos, compartments, containers, cisterns, 0f, num2, Precisy);
			Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(list2[0].Mass / aquaDensity, 0f, num2, stations, Precisy, aquaDensity);
			waterlanding_Volume_Heel_Trim.GPos = list2[0].GPosition;
			waterlanding_Volume_Heel_Trim.CalcStabilityArms(waterlanding_Volume_Heel_Trim.GPos);
			list.Add(new p2d(num2, waterlanding_Volume_Heel_Trim.lLongitudinal));
		}
		LongitudinalStaticStability longitudinalStaticStability = new LongitudinalStaticStability(0f, "продольная остойчивость", list);
		float num3 = longitudinalStaticStability.ReturnTrueAngleOnWater();
		List<p2d> list3 = new List<p2d>();
		Progress.status = "Расчет крена...";
		Progress.ProgressMaximum = 0;
		for (float num4 = -200f; num4 <= 200f; num4 += 5f)
		{
			Progress.ProgressMaximum++;
		}
		for (float num5 = -200f; num5 <= 200f; num5 += 5f)
		{
			Progress.ProgressPosition++;
			List<MassTable> list4 = STEB2.Mass.Tools.CalcMassGposByCompartmentsContainersBaseMassCisterns(baseMass, baseGPos, compartments, containers, cisterns, num5, num3, Precisy);
			Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim2 = new Waterlanding_Volume_Heel_Trim(list4[0].Mass / aquaDensity, num5, num3, stations, Precisy, aquaDensity);
			waterlanding_Volume_Heel_Trim2.GPos = list4[0].GPosition;
			waterlanding_Volume_Heel_Trim2.CalcStabilityArms(waterlanding_Volume_Heel_Trim2.GPos);
			list3.Add(new p2d(num5, waterlanding_Volume_Heel_Trim2.lTransverse));
		}
		TransverseStaticStability transverseStaticStability = new TransverseStaticStability(0f, "поперечная остойчивость", list3);
		float num6 = transverseStaticStability.ReturnTrueAngleOnWater();
		List<MassTable> list5 = STEB2.Mass.Tools.CalcMassGposByCompartmentsContainersBaseMassCisterns(baseMass, baseGPos, compartments, containers, cisterns, num6, num3, Precisy);
		Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim3 = new Waterlanding_Volume_Heel_Trim(list5[0].Mass / aquaDensity, num6, num3, stations, Precisy, aquaDensity);
		base.stations = stations;
		base.aquaDensity = aquaDensity;
		V = waterlanding_Volume_Heel_Trim3.V;
		DSPL = waterlanding_Volume_Heel_Trim3.V * aquaDensity;
		TrimAngle = waterlanding_Volume_Heel_Trim3.TrimAngle;
		HeelAngle = waterlanding_Volume_Heel_Trim3.HeelAngle;
		wlPosition = waterlanding_Volume_Heel_Trim3.wlPosition;
		ipWithWaterplane = waterlanding_Volume_Heel_Trim3.ipWithWaterplane;
		immersedStations = waterlanding_Volume_Heel_Trim3.immersedStations;
		CPos.X = waterlanding_Volume_Heel_Trim3.CPos.X;
		CPos.Y = waterlanding_Volume_Heel_Trim3.CPos.Y;
		CPos.Z = waterlanding_Volume_Heel_Trim3.CPos.Z;
		GPos = list5[0].GPosition;
		diagrammLongPoints = list;
		diagrammTransvPoints = list3;
		CalcWaterlineGeometricCharacteristic();
		GetWLGabarite_DNDKDLBDPB();
		CalcCoefficient();
	}
}
