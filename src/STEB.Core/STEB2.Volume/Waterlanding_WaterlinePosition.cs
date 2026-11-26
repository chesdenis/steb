using System;
using System.Collections.Generic;
using STEB2.Geometry;
using STEB2.Stations;

namespace STEB2.Volume;

public class Waterlanding_WaterlinePosition : Waterlanding
{
	public Waterlanding_WaterlinePosition(List<Station> Stations, float TrimAngle, float HeelAngle, p3d wlPosition, float aquaDensity)
	{
		ComputeDefaultValues(Stations, TrimAngle, HeelAngle, wlPosition, aquaDensity);
	}

	private void ComputeDefaultValues(List<Station> Stations, float TrimAngle, float HeelAngle, p3d wlPosition, float aquaDensity)
	{
		if (TrimAngle % 90f == 0f)
		{
			TrimAngle -= 1E-08f;
		}
		base.TrimAngle = TrimAngle;
		base.HeelAngle = HeelAngle;
		base.wlPosition = wlPosition;
		immersedStations = new ImmersedStation[1];
		foreach (Station Station in Stations)
		{
			immersedStations[immersedStations.Length - 1] = new ImmersedStation(Station, TrimAngle, HeelAngle, wlPosition);
			Array.Resize(ref immersedStations, immersedStations.Length + 1);
		}
		Array.Resize(ref immersedStations, immersedStations.Length - 1);
		immersedStations = STEB2.Stations.Tools.SortImmersedStations(immersedStations);
		ipWithWaterplane = new List<p3d>();
		for (int i = 0; i < immersedStations.Length; i++)
		{
			for (int j = 0; j < immersedStations[i].lstWaterplanePoints.Count; j++)
			{
				ipWithWaterplane.Add(new p3d(immersedStations[i].X, immersedStations[i].lstWaterplanePoints[j].X, immersedStations[i].lstWaterplanePoints[j].Y));
			}
		}
		p2d[] array = new p2d[1];
		array[0].X = immersedStations[0].X;
		array[0].Y = 0f;
		Array.Resize(ref array, array.Length + 1);
		p2d[] array2 = new p2d[1];
		p2d[] array3 = new p2d[1];
		p2d[] array4 = new p2d[1];
		int num = 1;
		array2[0].X = (array3[0].X = (array4[0].X = immersedStations[0].X));
		array2[0].Y = (array3[0].Y = (array4[0].Y = 0f));
		Array.Resize(ref array2, num + 1);
		Array.Resize(ref array3, num + 1);
		Array.Resize(ref array4, num + 1);
		num++;
		for (int k = 0; k < immersedStations.Length; k++)
		{
			array[array.Length - 1].X = immersedStations[k].X;
			array[array.Length - 1].Y = immersedStations[k].StationContur.Area;
			Array.Resize(ref array, array.Length + 1);
			array2[num - 1].X = (array3[num - 1].X = (array4[num - 1].X = immersedStations[k].X));
			array2[num - 1].Y = immersedStations[k].StationContur.Area;
			array3[num - 1].Y = immersedStations[k].StationContur.Area * immersedStations[k].StationContur.CenterOfGravity.X;
			array4[num - 1].Y = immersedStations[k].StationContur.Area * immersedStations[k].StationContur.CenterOfGravity.Y;
			Array.Resize(ref array2, num + 1);
			Array.Resize(ref array3, num + 1);
			Array.Resize(ref array4, num + 1);
			num++;
		}
		array[array.Length - 1].X = immersedStations[immersedStations.Length - 1].X;
		array[array.Length - 1].Y = 0f;
		Contur contur = new Contur(array);
		V = Math.Abs(contur.Area);
		DSPL = V * aquaDensity;
		array2[num - 1].X = (array3[num - 1].X = (array4[num - 1].X = immersedStations[immersedStations.Length - 1].X));
		array2[num - 1].Y = 0f;
		array3[num - 1].Y = 0f;
		array4[num - 1].Y = 0f;
		Contur contur2 = new Contur(array2);
		Contur contur3 = new Contur(array3);
		Contur contur4 = new Contur(array4);
		if (V > 0f)
		{
			p3d p3d2 = new p3d(contur2.CenterOfGravity.X, contur3.trueArea / V, contur4.trueArea / V);
			CPos.X = p3d2.X;
			CPos.Y = p3d2.Y;
			CPos.Z = p3d2.Z;
		}
		else
		{
			CPos.X = 0f;
			CPos.Y = 0f;
			CPos.Z = 0f;
		}
		stations = Stations;
		base.aquaDensity = aquaDensity;
	}
}
