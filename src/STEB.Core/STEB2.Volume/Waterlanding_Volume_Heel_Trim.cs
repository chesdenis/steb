using System;
using System.Collections.Generic;
using STEB2.DataBase;
using STEB2.Geometry;
using STEB2.Stations;

namespace STEB2.Volume;

public class Waterlanding_Volume_Heel_Trim : Waterlanding
{
	public Waterlanding_Volume_Heel_Trim(float Volume, float heel_angle, float trim_angle, List<Station> Stations, float Precisy, float aquaDensity)
	{
		bool flag = FindWaterLanding(Volume, heel_angle, trim_angle, Stations, Precisy, aquaDensity);
		if (!flag)
		{
			flag = FindWaterLandingInterpolate(Volume, heel_angle, trim_angle, Stations, aquaDensity);
		}
		if (!flag)
		{
			STEB2.DataBase.Tools.log.writeLog("Невозможно определить посадку судна(V = " + Volume + ", угол крена " + heel_angle + " градусов, угол дифферента " + trim_angle + " градусов)");
		}
	}

	private bool FindWaterLandingInterpolate(float Volume, float heel_angle, float trim_angle, List<Station> Stations, float aquaDensity)
	{
		float maxVolume = Tools.GetMaxVolume(Stations);
		p3d gabariteOfVolume = Tools.GetGabariteOfVolume(Stations);
		p3d[] boundPointsOfVolume = Tools.GetBoundPointsOfVolume(Stations);
		if ((Volume > maxVolume) | (Volume <= 0f))
		{
			return false;
		}
		p3d startPoint = new p3d(boundPointsOfVolume[0].X + gabariteOfVolume.X / 2f, boundPointsOfVolume[0].Y + gabariteOfVolume.Y / 2f, boundPointsOfVolume[0].Z + gabariteOfVolume.Z / 2f);
		p3d startPoint2 = new p3d(boundPointsOfVolume[0].X + gabariteOfVolume.X / 2f, boundPointsOfVolume[0].Y + gabariteOfVolume.Y / 2f, boundPointsOfVolume[0].Z + gabariteOfVolume.Z / 2f);
		float num = gabariteOfVolume.X;
		for (int i = 0; i < 3; i++)
		{
			switch (i)
			{
			case 0:
				num = ((num < gabariteOfVolume.X) ? gabariteOfVolume.X : num);
				break;
			case 1:
				num = ((num < gabariteOfVolume.Y) ? gabariteOfVolume.Y : num);
				break;
			case 2:
				num = ((num < gabariteOfVolume.Z) ? gabariteOfVolume.Z : num);
				break;
			}
		}
		startPoint2 = Transform.OffsetPoint(heel_angle, trim_angle, num * 2f, startPoint2);
		startPoint = Transform.OffsetPoint(heel_angle, trim_angle, (0f - num) * 2f, startPoint);
		List<p2d> list = new List<p2d>();
		float num2 = Mathematics.Get3dVectorLength(startPoint, startPoint2);
		for (float num3 = 0f; num3 < 1f; num3 += 0.01f)
		{
			p3d vectorPoint = new p3d(startPoint.X + num3 * num2, startPoint.Y + num3 * num2, startPoint.Z + num3 * num2);
			Waterlanding_WaterlinePosition waterlanding_WaterlinePosition = new Waterlanding_WaterlinePosition(Stations, trim_angle, heel_angle, vectorPoint, aquaDensity);
			if (vectorPoint.Z < 0f)
			{
				list.Add(new p2d(0f - Mathematics.Get3dVectorLength(vectorPoint), waterlanding_WaterlinePosition.V));
			}
			else if (vectorPoint.Z >= 0f)
			{
				list.Add(new p2d(Mathematics.Get3dVectorLength(vectorPoint), waterlanding_WaterlinePosition.V));
			}
		}
		float num4 = float.NaN;
		for (int j = 1; j < list.Count; j++)
		{
			try
			{
				num4 = Intersection.GetXPoint(new p2d(0f, Volume), new p2d(1f, Volume), list[j - 1], list[j], isExtendFirst: true, isExtendSecond: false).X;
				if (!float.IsNaN(num4) && !float.IsInfinity(num4))
				{
					break;
				}
			}
			catch
			{
			}
		}
		p3d p3d2 = Transform.OffsetPoint(heel_angle, trim_angle, num4);
		Waterlanding_WaterlinePosition waterlanding_WaterlinePosition2 = new Waterlanding_WaterlinePosition(Stations, trim_angle, heel_angle, p3d2, aquaDensity);
		stations = Stations;
		base.aquaDensity = aquaDensity;
		V = waterlanding_WaterlinePosition2.V;
		DSPL = waterlanding_WaterlinePosition2.V * aquaDensity;
		TrimAngle = waterlanding_WaterlinePosition2.TrimAngle;
		HeelAngle = waterlanding_WaterlinePosition2.HeelAngle;
		wlPosition = waterlanding_WaterlinePosition2.wlPosition;
		ipWithWaterplane = waterlanding_WaterlinePosition2.ipWithWaterplane;
		immersedStations = waterlanding_WaterlinePosition2.immersedStations;
		CPos.X = waterlanding_WaterlinePosition2.CPos.X;
		CPos.Y = waterlanding_WaterlinePosition2.CPos.Y;
		CPos.Z = waterlanding_WaterlinePosition2.CPos.Z;
		return true;
	}

	private bool FindWaterLanding(float Volume, float heel_angle, float trim_angle, List<Station> Stations, float Precisy, float aquaDensity)
	{
		float maxVolume = Tools.GetMaxVolume(Stations);
		Stations = STEB2.Stations.Tools.SortStations(Stations);
		p3d[] boundPointsOfVolume = Tools.GetBoundPointsOfVolume(Stations);
		p3d gabariteOfVolume = Tools.GetGabariteOfVolume(Stations);
		if ((Volume > maxVolume) | (Volume <= 0f))
		{
			return false;
		}
		if ((Volume > maxVolume) | (Volume <= 0f))
		{
			return false;
		}
		TrimAngle = trim_angle;
		HeelAngle = heel_angle;
		p3d startPoint = new p3d(boundPointsOfVolume[0].X + gabariteOfVolume.X / 2f, boundPointsOfVolume[0].Y + gabariteOfVolume.Y / 2f, boundPointsOfVolume[0].Z + gabariteOfVolume.Z / 2f);
		p3d startPoint2 = new p3d(boundPointsOfVolume[0].X + gabariteOfVolume.X / 2f, boundPointsOfVolume[0].Y + gabariteOfVolume.Y / 2f, boundPointsOfVolume[0].Z + gabariteOfVolume.Z / 2f);
		float num = gabariteOfVolume.X;
		for (int i = 0; i < 3; i++)
		{
			switch (i)
			{
			case 0:
				num = ((num < gabariteOfVolume.X) ? gabariteOfVolume.X : num);
				break;
			case 1:
				num = ((num < gabariteOfVolume.Y) ? gabariteOfVolume.Y : num);
				break;
			case 2:
				num = ((num < gabariteOfVolume.Z) ? gabariteOfVolume.Z : num);
				break;
			}
		}
		startPoint2 = Transform.OffsetPoint(heel_angle, trim_angle, num, startPoint2);
		startPoint = Transform.OffsetPoint(heel_angle, trim_angle, 0f - num, startPoint);
		p3d p3d2 = default;
		float num2 = 0.5f;
		int num3 = 0;
		int num4 = 100;
		Waterlanding_WaterlinePosition waterlanding_WaterlinePosition;
		while (true)
		{
			if (num3 > num4)
			{
				return false;
			}
			p3d2.X = (startPoint2.X - startPoint.X) * num2 + startPoint.X;
			p3d2.Y = (startPoint2.Y - startPoint.Y) * num2 + startPoint.Y;
			p3d2.Z = (startPoint2.Z - startPoint.Z) * num2 + startPoint.Z;
			waterlanding_WaterlinePosition = new Waterlanding_WaterlinePosition(Stations, trim_angle, heel_angle, p3d2, aquaDensity);
			float num5 = Math.Abs(Volume - waterlanding_WaterlinePosition.V);
			num5 = (float)Math.Round(num5, 3);
			if (num5 <= Precisy)
			{
				break;
			}
			if (waterlanding_WaterlinePosition.V < Volume)
			{
				startPoint.X = p3d2.X;
				startPoint.Y = p3d2.Y;
				startPoint.Z = p3d2.Z;
				num3++;
			}
			else
			{
				startPoint2.X = p3d2.X;
				startPoint2.Y = p3d2.Y;
				startPoint2.Z = p3d2.Z;
				num3++;
			}
		}
		stations = Stations;
		base.aquaDensity = aquaDensity;
		V = waterlanding_WaterlinePosition.V;
		DSPL = waterlanding_WaterlinePosition.V * aquaDensity;
		TrimAngle = waterlanding_WaterlinePosition.TrimAngle;
		HeelAngle = waterlanding_WaterlinePosition.HeelAngle;
		wlPosition = waterlanding_WaterlinePosition.wlPosition;
		ipWithWaterplane = waterlanding_WaterlinePosition.ipWithWaterplane;
		immersedStations = waterlanding_WaterlinePosition.immersedStations;
		CPos.X = waterlanding_WaterlinePosition.CPos.X;
		CPos.Y = waterlanding_WaterlinePosition.CPos.Y;
		CPos.Z = waterlanding_WaterlinePosition.CPos.Z;
		return true;
	}
}
