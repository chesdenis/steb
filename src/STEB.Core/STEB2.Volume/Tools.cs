using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using STEB2.Cisterns;
using STEB2.Containers;
using STEB2.Converting;
using STEB2.Geometry;
using STEB2.Stability;
using STEB2.Stations;

namespace STEB2.Volume;

public class Tools
{
	public enum HydroCurvesType
	{
		GryzRazmer,
		XcXf,
		rZc,
		R,
		S,
		coeff
	}

	public static List<Station> MoveVolume(List<Station> inputStations, float dx, float dy, float dz)
	{
		List<Station> list = new List<Station>();
		foreach (Station inputStation in inputStations)
		{
			List<Contur> list2 = new List<Contur>();
			Contur[] arrayConturs = inputStation.StationContur.arrayConturs;
			foreach (Contur contur in arrayConturs)
			{
				List<p2d> list3 = new List<p2d>();
				p2d[] arrayPoints = contur.arrayPoints;
				for (int j = 0; j < arrayPoints.Length; j++)
				{
					p2d p2d2 = arrayPoints[j];
					list3.Add(new p2d(p2d2.X + dy, p2d2.Y + dz));
				}
				Contur item = new Contur(list3);
				list2.Add(item);
			}
			MultipleContur stationContur = new MultipleContur(list2);
			Station item2 = new Station(stationContur, inputStation.X + dx);
			list.Add(item2);
		}
		return STEB2.Stations.Tools.SortStations(list);
	}

	public static List<StabilityCurve> GetHydroCurves(float startDraft, float endDraft, float stepDraft, float aquaDensity, float precisy, List<Station> stations, HydroCurvesType type)
	{
		List<StabilityCurve> list = new List<StabilityCurve>();
		if (startDraft > endDraft)
		{
			return list;
		}
		if (stepDraft <= 0f)
		{
			return list;
		}
		Progress.ProgressMaximum = 0;
		for (float num = startDraft; num <= endDraft; num += stepDraft)
		{
			Progress.ProgressMaximum++;
		}
		List<p2d> list2 = new List<p2d>();
		List<p2d> list3 = new List<p2d>();
		for (float num2 = startDraft; num2 <= endDraft; num2 += stepDraft)
		{
			Progress.ProgressPosition++;
			Progress.status = "Расчет осадки " + num2;
			Waterlanding_WaterlinePosition waterlanding_WaterlinePosition = new Waterlanding_WaterlinePosition(stations, 0f, 0f, new p3d(0f, 0f, num2), aquaDensity);
			waterlanding_WaterlinePosition.GetWLGabarite_DNDKDLBDPB();
			waterlanding_WaterlinePosition.CalcWaterlineGeometricCharacteristic();
			waterlanding_WaterlinePosition.CalcCoefficient();
			switch (type)
			{
			case HydroCurvesType.GryzRazmer:
				list2.Add(new p2d(waterlanding_WaterlinePosition.V, num2));
				list3.Add(new p2d(waterlanding_WaterlinePosition.V * aquaDensity, num2));
				break;
			case HydroCurvesType.XcXf:
				list2.Add(new p2d(waterlanding_WaterlinePosition.CPos.X, num2));
				list3.Add(new p2d(waterlanding_WaterlinePosition.xf, num2));
				break;
			case HydroCurvesType.rZc:
				list2.Add(new p2d(waterlanding_WaterlinePosition.r, num2));
				list3.Add(new p2d(waterlanding_WaterlinePosition.CPos.Z, num2));
				break;
			case HydroCurvesType.R:
				list2.Add(new p2d(waterlanding_WaterlinePosition.R, num2));
				break;
			case HydroCurvesType.S:
				list2.Add(new p2d(waterlanding_WaterlinePosition.S, num2));
				break;
			case HydroCurvesType.coeff:
				list2.Add(new p2d(waterlanding_WaterlinePosition.alpha, num2));
				list3.Add(new p2d(waterlanding_WaterlinePosition.delta, num2));
				break;
			}
		}
		switch (type)
		{
		case HydroCurvesType.GryzRazmer:
		{
			StabilityCurve item9 = new StabilityCurve(0f, "V", list2);
			StabilityCurve item10 = new StabilityCurve(0f, "DSPL", list3);
			list.Add(item9);
			list.Add(item10);
			break;
		}
		case HydroCurvesType.XcXf:
		{
			StabilityCurve item7 = new StabilityCurve(0f, "Xc", list2);
			StabilityCurve item8 = new StabilityCurve(0f, "Xf", list3);
			list.Add(item7);
			list.Add(item8);
			break;
		}
		case HydroCurvesType.rZc:
		{
			StabilityCurve item5 = new StabilityCurve(0f, "r", list2);
			StabilityCurve item6 = new StabilityCurve(0f, "Zc", list3);
			list.Add(item5);
			list.Add(item6);
			break;
		}
		case HydroCurvesType.R:
		{
			StabilityCurve item4 = new StabilityCurve(0f, "R", list2);
			list.Add(item4);
			break;
		}
		case HydroCurvesType.S:
		{
			StabilityCurve item3 = new StabilityCurve(0f, "S", list2);
			list.Add(item3);
			break;
		}
		case HydroCurvesType.coeff:
		{
			StabilityCurve item = new StabilityCurve(0f, "alpha", list2);
			StabilityCurve item2 = new StabilityCurve(0f, "delta", list3);
			list.Add(item);
			list.Add(item2);
			break;
		}
		}
		return list;
	}

	public static List<StabilityCurve> GetCurvesArmsTransvStabilityForms(float startVolume, float endVolume, float stepVolume, float startHeelAngle, float endHeelAngle, float stepHeelAngle, float aquaDensity, float precisy, List<Station> stations)
	{
		List<StabilityCurve> list = new List<StabilityCurve>();
		if (startVolume > endVolume)
		{
			return list;
		}
		if (stepVolume <= 0f)
		{
			return list;
		}
		if (startHeelAngle > endHeelAngle)
		{
			return list;
		}
		if (stepHeelAngle <= 0f)
		{
			return list;
		}
		Progress.ProgressMaximum = 0;
		for (float num = startHeelAngle; num <= endHeelAngle; num += stepHeelAngle)
		{
			for (float num2 = startVolume; num2 <= endVolume; num2 += stepVolume)
			{
				Progress.ProgressMaximum++;
			}
		}
		for (float num3 = startHeelAngle; num3 <= endHeelAngle; num3 += stepHeelAngle)
		{
			Progress.status = "Расчет угла крена " + num3;
			List<p2d> list2 = new List<p2d>();
			for (float num4 = startVolume; num4 <= endVolume; num4 += stepVolume)
			{
				Progress.ProgressPosition++;
				Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(num4, num3, 0f, stations, precisy, aquaDensity);
				waterlanding_Volume_Heel_Trim.CalcStabilityArms(new p3d(0f, 0f, 0f));
				list2.Add(new p2d(num4, waterlanding_Volume_Heel_Trim.lfTransverse));
			}
			StabilityCurve item = new StabilityCurve(0f, num3 + " градусов", list2);
			list.Add(item);
		}
		return list;
	}

	public static float GetMaxVolume(List<Station> st)
	{
		try
		{
			st = STEB2.Stations.Tools.SortStations(st);
			p2d[] array = new p2d[1];
			array[0].X = st[0].X;
			array[0].Y = 0f;
			Array.Resize(ref array, array.Length + 1);
			for (int i = 0; i < st.Count; i++)
			{
				array[array.Length - 1].X = st[i].X;
				array[array.Length - 1].Y = st[i].StationContur.Area;
				Array.Resize(ref array, array.Length + 1);
			}
			array[array.Length - 1].X = st[st.Count - 1].X;
			array[array.Length - 1].Y = 0f;
			Contur contur = new Contur(array);
			return Math.Abs(contur.Area);
		}
		catch
		{
			return 0f;
		}
	}

	public static List<Station> GetAffinnStations(List<Station> inputStations, float l, float b, float h, float kx, float ky, float kz)
	{
		inputStations = STEB2.Stations.Tools.SortStations(inputStations);
		if (l > 0f && b > 0f && h > 0f)
		{
			p3d gabariteOfVolume = GetGabariteOfVolume(inputStations);
			kx = l / gabariteOfVolume.X;
			ky = b / gabariteOfVolume.Y;
			kz = h / gabariteOfVolume.Z;
		}
		else if ((kx == 0f) | (ky == 0f) | (kz == 0f))
		{
			kx = ((kx == 0f) ? 1f : kx);
			ky = ((ky == 0f) ? 1f : ky);
			kz = ((kz == 0f) ? 1f : kz);
		}
		List<Station> list = new List<Station>();
		foreach (Station inputStation in inputStations)
		{
			list.Add(new Station(new MultipleContur(inputStation.StationContur.arrayConturs), inputStation.X));
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i].X *= kx;
			for (int j = 0; j < list[i].StationContur.arrayConturs.Length; j++)
			{
				for (int k = 0; k < list[i].StationContur.arrayConturs[j].arrayPoints.Length; k++)
				{
					list[i].StationContur.arrayConturs[j].arrayPoints[k].X *= ky;
					list[i].StationContur.arrayConturs[j].arrayPoints[k].Y *= kz;
				}
			}
			list[i] = new Station(new MultipleContur(list[i].StationContur.arrayConturs), list[i].X);
		}
		return list;
	}

	public static p3d GetGabariteOfProject(List<Station> Stations, List<Cistern> Cisterns, List<Container> Containers)
	{
		p3d result = default;
		List<p2d> list = new List<p2d>();
		List<p2d> list2 = new List<p2d>();
		try
		{
			Stations = STEB2.Stations.Tools.SortStations(Stations);
			try
			{
				foreach (Station Station in Stations)
				{
					Contur[] arrayConturs = Station.StationContur.arrayConturs;
					foreach (Contur contur in arrayConturs)
					{
						foreach (p2d listPoint in contur.listPoints)
						{
							list.Add(new p2d(listPoint.X, listPoint.Y));
							list2.Add(new p2d(Station.X, listPoint.Y));
						}
					}
				}
			}
			catch
			{
			}
			try
			{
				foreach (Cistern Cistern in Cisterns)
				{
					foreach (Station station in Cistern.Stations)
					{
						Contur[] arrayConturs2 = station.StationContur.arrayConturs;
						foreach (Contur contur2 in arrayConturs2)
						{
							foreach (p2d listPoint2 in contur2.listPoints)
							{
								list.Add(new p2d(listPoint2.X, listPoint2.Y));
								list2.Add(new p2d(station.X, listPoint2.Y));
							}
						}
					}
				}
			}
			catch
			{
			}
			try
			{
				foreach (Container Container in Containers)
				{
					list.Add(new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z));
					list.Add(new p2d(Container.pos.Y + Container.b / 2f, Container.pos.Z));
					list.Add(new p2d(Container.pos.Y + Container.b / 2f, Container.pos.Z + Container.h));
					list.Add(new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z + Container.h));
					list.Add(new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z));
					list2.Add(new p2d(Container.pos.X, Container.pos.Z));
					list2.Add(new p2d(Container.pos.X + Container.l, Container.pos.Z));
					list2.Add(new p2d(Container.pos.X + Container.l, Container.pos.Z + Container.h));
					list2.Add(new p2d(Container.pos.X, Container.pos.Z + Container.h));
					list2.Add(new p2d(Container.pos.X, Container.pos.Z));
				}
			}
			catch
			{
			}
		}
		catch
		{
		}
		p2d[] boundRect = Mathematics.GetBoundRect(list.ToArray());
		p2d[] boundRect2 = Mathematics.GetBoundRect(list2.ToArray());
		result.X = boundRect2[2].X - boundRect2[0].X;
		result.Y = boundRect[2].X - boundRect[0].X;
		result.Z = boundRect[1].Y - boundRect[0].Y;
		return result;
	}

	public static p3d GetGabariteOfVolume(List<Station> inputStations)
	{
		p3d result = default;
		try
		{
			inputStations = STEB2.Stations.Tools.SortStations(inputStations);
			List<p2d> list = new List<p2d>();
			foreach (Station inputStation in inputStations)
			{
				Contur[] arrayConturs = inputStation.StationContur.arrayConturs;
				foreach (Contur contur in arrayConturs)
				{
					foreach (p2d listPoint in contur.listPoints)
					{
						list.Add(new p2d(listPoint.X, listPoint.Y));
					}
				}
			}
			p2d[] boundRect = Mathematics.GetBoundRect(STEB2.Converting.Converting.ListToArray(list));
			result.X = inputStations[inputStations.Count - 1].X - inputStations[0].X;
			result.Y = boundRect[2].X - boundRect[0].X;
			result.Z = boundRect[1].Y - boundRect[0].Y;
		}
		catch
		{
		}
		return result;
	}

	public static p3d[] GetBoundPointsOfVolume(List<Station> inputStations)
	{
		p3d p3d2 = default;
		p3d p3d3 = default;
		try
		{
			inputStations = STEB2.Stations.Tools.SortStations(inputStations);
			List<p2d> list = new List<p2d>();
			foreach (Station inputStation in inputStations)
			{
				Contur[] arrayConturs = inputStation.StationContur.arrayConturs;
				foreach (Contur contur in arrayConturs)
				{
					foreach (p2d listPoint in contur.listPoints)
					{
						list.Add(new p2d(listPoint.X, listPoint.Y));
					}
				}
			}
			p2d[] boundRect = Mathematics.GetBoundRect(list.ToArray());
			p3d2.X = inputStations[0].X;
			p3d3.X = inputStations[inputStations.Count - 1].X;
			p3d2.Y = boundRect[0].X;
			p3d3.Y = boundRect[2].X;
			p3d2.Z = boundRect[0].Y;
			p3d3.Z = boundRect[1].Y;
			return new p3d[2] { p3d2, p3d3 };
		}
		catch
		{
		}
		return new p3d[2] { p3d2, p3d3 };
	}

	public static List<Station> TextToStations(string Text)
	{
		List<Station> list = new List<Station>();
		new List<Contur>();
		bool flag = false;
		int startIndex = 0;
		int num = 0;
		while (!flag)
		{
			startIndex = Text.IndexOf("new", startIndex);
			if (startIndex == -1)
			{
				break;
			}
			startIndex += "new".Length;
			num = ((Text.IndexOf("new", startIndex) == -1) ? Text.Length : Text.IndexOf("new", startIndex));
			string text = Text.Substring(startIndex, num - startIndex);
			bool flag2 = text.IndexOf("mirror") != -1;
			string text2 = "";
			for (int i = 0; i <= text.Length - 1; i++)
			{
				text2 = text.Substring(i, 1) switch
				{
					"0" => text2 + "0", 
					"1" => text2 + "1", 
					"2" => text2 + "2", 
					"3" => text2 + "3", 
					"4" => text2 + "4", 
					"5" => text2 + "5", 
					"6" => text2 + "6", 
					"7" => text2 + "7", 
					"8" => text2 + "8", 
					"9" => text2 + "9", 
					"." => text2 + ".", 
					"," => text2 + ",", 
					" " => text2 + " ", 
					"-" => text2 + "-", 
					"E-" => text2 + "E-", 
					"E+" => text2 + "E+", 
					_ => text2 + " ", 
				};
			}
			if (text2.Trim() != "")
			{
				string input = text2;
				string[] array = Regex.Split(input, " ");
				int num2 = -1;
				for (int j = 0; j <= array.Length - 1; j++)
				{
					if (array[j] != "")
					{
						num2++;
						array[num2] = array[j];
					}
				}
				Array.Resize(ref array, num2 + 1);
				try
				{
					p2d[] array2 = new p2d[1];
					for (int k = 1; k <= array.Length - 1; k += 2)
					{
						array2[array2.Length - 1].X = STEB2.Converting.Converting.TextToFloat(array[k]);
						array2[array2.Length - 1].Y = STEB2.Converting.Converting.TextToFloat(array[k + 1]);
						Array.Resize(ref array2, array2.Length + 1);
					}
					if (!flag2)
					{
						Array.Resize(ref array2, array2.Length - 1);
					}
					if (flag2)
					{
						for (int num3 = array.Length - 1; num3 >= 1; num3 -= 2)
						{
							array2[array2.Length - 1].X = -1f * STEB2.Converting.Converting.TextToFloat(array[num3 - 1]);
							array2[array2.Length - 1].Y = STEB2.Converting.Converting.TextToFloat(array[num3]);
							Array.Resize(ref array2, array2.Length + 1);
						}
						Array.Resize(ref array2, array2.Length - 1);
					}
					if (array2[array2.Length - 1].X != array2[0].X && array2[array2.Length - 1].Y != array2[0].Y)
					{
						Array.Resize(ref array2, array2.Length + 1);
						array2[array2.Length - 1].X = array2[0].X;
						array2[array2.Length - 1].Y = array2[0].Y;
					}
					Contur contur = new Contur(array2);
					bool flag3 = false;
					foreach (Station item in list)
					{
						if (item.X == STEB2.Converting.Converting.TextToFloat(array[0]))
						{
							item.StationContur.AddContur(contur);
							flag3 = true;
						}
					}
					if (!flag3)
					{
						list.Add(new Station(new MultipleContur(new Contur[1] { contur }), STEB2.Converting.Converting.TextToFloat(array[0])));
					}
				}
				catch
				{
				}
			}
			startIndex = num;
		}
		return list;
	}
}
