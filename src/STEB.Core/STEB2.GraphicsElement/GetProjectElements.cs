using System;
using System.Collections.Generic;
using STEB2.DataBase.Json;
using STEB2.Controls;
using STEB2.Converting;
using STEB2.DataBase;
using STEB2.Geometry;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB2.GraphicsElement;

public class GetProjectElements
{
	public static string GetSectionDescription(ProjectSectionDefinition.SectionType typeSection, JsonDbConnection cnn)
	{
		string text = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", cnn).ToString();
		string text2 = "";
		switch (typeSection)
		{
		case ProjectSectionDefinition.SectionType.Long:
			text2 = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms WHERE ShipFormName = '" + text + "'", "LongSections", cnn).ToString();
			break;
		case ProjectSectionDefinition.SectionType.Transv:
			text2 = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms WHERE ShipFormName = '" + text + "'", "TransverseSections", cnn).ToString();
			break;
		case ProjectSectionDefinition.SectionType.Top:
			text2 = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms WHERE ShipFormName = '" + text + "'", "TopSections", cnn).ToString();
			break;
		}
		if (text2.Trim() == "")
		{
			List<Station> projectStation = GetProjectStation(text, cnn);
			p3d[] boundPointsOfVolume = STEB2.Volume.Tools.GetBoundPointsOfVolume(projectStation);
			switch (typeSection)
			{
			case ProjectSectionDefinition.SectionType.Long:
			{
				float num2 = boundPointsOfVolume[0].Y + (boundPointsOfVolume[1].Y - boundPointsOfVolume[0].Y) / 2f;
				float num3 = boundPointsOfVolume[0].Y + (boundPointsOfVolume[1].Y - boundPointsOfVolume[0].Y) / 2f - (boundPointsOfVolume[1].Y - boundPointsOfVolume[0].Y) / 4f;
				float num4 = boundPointsOfVolume[0].Y + (boundPointsOfVolume[1].Y - boundPointsOfVolume[0].Y) / 2f + (boundPointsOfVolume[1].Y - boundPointsOfVolume[0].Y) / 4f;
				text2 = text2 + "\n" + num2 + ":Сечение по ДП;";
				string text4 = text2;
				text2 = text4 + "\n" + num3 + ":" + num3.ToString("0.000") + ";";
				string text5 = text2;
				text2 = text5 + "\n" + num4 + ":" + num4.ToString("0.000") + ";";
				break;
			}
			case ProjectSectionDefinition.SectionType.Transv:
			{
				for (float num5 = boundPointsOfVolume[0].X; num5 < boundPointsOfVolume[1].X; num5 += (boundPointsOfVolume[1].X - boundPointsOfVolume[0].X) / 10f)
				{
					string text6 = text2;
					text2 = text6 + "\n" + num5 + ":" + num5.ToString("0.000") + ";";
				}
				break;
			}
			case ProjectSectionDefinition.SectionType.Top:
			{
				for (float num = boundPointsOfVolume[0].Z; num < boundPointsOfVolume[1].Z; num += (boundPointsOfVolume[1].Z - boundPointsOfVolume[0].Z) / 5f)
				{
					string text3 = text2;
					text2 = text3 + "\n" + num + ":" + num.ToString("0.000") + ";";
				}
				break;
			}
			}
			return text2;
		}
		return text2;
	}

	public static List<Station> GetProjectStation(string ShipFormName, JsonDbConnection cnn)
	{
		string text = STEB2.DataBase.Tools.ReturnShipFormText(ShipFormName, cnn);
		string[] array = STEB2.DataBase.Tools.ReturnArrayOfResultByArrayOfFields("SELECT * FROM tblShipForms WHERE ShipFormName='" + ShipFormName + "';", new string[7] { "L", "B", "H", "d", "kX", "kY", "kZ" }, cnn);
		return STEB2.Volume.Tools.GetAffinnStations(STEB2.Stations.Tools.TextToStations(text), STEB2.Converting.Converting.TextToFloat(array[0]), STEB2.Converting.Converting.TextToFloat(array[1]), STEB2.Converting.Converting.TextToFloat(array[2]), STEB2.Converting.Converting.TextToFloat(array[3]), STEB2.Converting.Converting.TextToFloat(array[4]), STEB2.Converting.Converting.TextToFloat(array[5]));
	}

	public static Contur GetBroadSection(List<Station> inputStations, float y)
	{
		List<p2d> list = new List<p2d>();
		inputStations = STEB2.Stations.Tools.SortStations(inputStations);
		for (int i = 0; i < inputStations.Count; i++)
		{
			Contur[] arrayConturs = inputStations[i].StationContur.arrayConturs;
			foreach (Contur contur in arrayConturs)
			{
				List<p2d> xPointConturLine = Intersection.GetXPointConturLine(new p2d(y, 0f), new p2d(y, 1f), isExtendLine: true, contur);
				if (xPointConturLine.Count > 1)
				{
					p2d[] boundRect = Mathematics.GetBoundRect(xPointConturLine.ToArray());
					list.Add(new p2d(inputStations[i].X, boundRect[0].Y));
				}
			}
		}
		for (int num = inputStations.Count - 1; num >= 0; num--)
		{
			Contur[] arrayConturs2 = inputStations[num].StationContur.arrayConturs;
			foreach (Contur contur2 in arrayConturs2)
			{
				List<p2d> xPointConturLine2 = Intersection.GetXPointConturLine(new p2d(y, 0f), new p2d(y, 1f), isExtendLine: true, contur2);
				if (xPointConturLine2.Count > 1)
				{
					p2d[] boundRect2 = Mathematics.GetBoundRect(xPointConturLine2.ToArray());
					list.Add(new p2d(inputStations[num].X, boundRect2[1].Y));
				}
			}
		}
		return new Contur(list);
	}

	public static Station GetHullSection(List<Station> inputStations, float x)
	{
		inputStations = STEB2.Stations.Tools.SortStations(inputStations);
		Station result = new Station(new MultipleContur(new Contur[0]), 0f);
		float num = float.NaN;
		foreach (Station inputStation in inputStations)
		{
			if (float.IsNaN(num))
			{
				result = inputStation;
				num = Math.Abs(inputStation.X - x);
			}
			if (num > Math.Abs(inputStation.X - x))
			{
				num = Math.Abs(inputStation.X - x);
				result = inputStation;
			}
		}
		return result;
	}

	public static Contur GetBroadProject(List<Station> inputStations)
	{
		inputStations = STEB2.Stations.Tools.SortStations(inputStations);
		List<p2d> list = new List<p2d>();
		for (int i = 0; i < inputStations.Count; i++)
		{
			if (inputStations[i].StationContur.Area > 0f)
			{
				list.Add(new p2d(inputStations[i].X, inputStations[i].GetSledOnDP()[0].Y));
			}
		}
		for (int num = inputStations.Count - 1; num >= 0; num--)
		{
			if (inputStations[num].StationContur.Area > 0f)
			{
				list.Add(new p2d(inputStations[num].X, inputStations[num].GetSledOnDP()[1].Y));
			}
		}
		if (list.Count > 0)
		{
			list.Add(new p2d(list[0].X, list[0].Y));
		}
		return new Contur(list);
	}

	public static Contur GetTopProject(List<Station> inputStations)
	{
		inputStations = STEB2.Stations.Tools.SortStations(inputStations);
		List<p2d> list = new List<p2d>();
		for (int i = 0; i < inputStations.Count; i++)
		{
			if (inputStations[i].StationContur.Area > 0f)
			{
				list.Add(new p2d(inputStations[i].X, inputStations[i].GetSledOnTop()[0].Y));
			}
		}
		for (int num = inputStations.Count - 1; num >= 0; num--)
		{
			if (inputStations[num].StationContur.Area > 0f)
			{
				list.Add(new p2d(inputStations[num].X, inputStations[num].GetSledOnTop()[1].Y));
			}
		}
		if (list.Count > 0)
		{
			list.Add(new p2d(list[0].X, list[0].Y));
		}
		return new Contur(list);
	}

	public static Contur GetHullProject(List<Station> inputStations)
	{
		return new Contur(inputStations[inputStations.Count / 2].StationContur.arrayConturs[0].arrayPoints);
	}
}
