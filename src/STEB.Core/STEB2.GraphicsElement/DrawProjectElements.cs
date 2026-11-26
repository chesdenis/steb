using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using STEB2.Cisterns;
using STEB2.Compartments;
using STEB2.Containers;
using STEB2.Converting;
using STEB2.Geometry;
using STEB2.Stability;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB2.GraphicsElement;

public class DrawProjectElements
{
	public enum Planes
	{
		XZ,
		XY,
		YZ
	}

	public static void DrawHullSection(usgModelFrame workspace, List<Station> Stations, List<Container> Containers, List<Cistern> Cisterns, List<Compartment> Compartments, float x, string HeadName, string CenterName, string Tag, string textLong, float heelAngle, float trimAngle)
	{
		Stations = STEB2.Stations.Tools.SortStations(Stations);
		if (x >= Stations[0].X && x <= Stations[Stations.Count - 1].X)
		{
			DrawHullSide(workspace, new List<Station> { GetProjectElements.GetHullSection(Stations, x) }, drawAllStations: false, HeadName, CenterName, DrawAxes: false, Tag, Color.Transparent);
			p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(Stations);
			string[] array = textLong.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.Trim() != "")
				{
					float x2 = STEB2.Converting.Converting.TextToFloat(text.Split(':')[0]);
					string text2 = text.Split(':')[1];
					workspace.Lines.Add(new Line(new PointF[2]
					{
						new PointF(x2, (0f - gabariteOfVolume.Z) / 10f),
						new PointF(x2, gabariteOfVolume.Z / 10f)
					}, DashStyle.Solid, Color.DarkGreen, 1, Tag));
					workspace.Texts.Add(new Text(new PointF[1]
					{
						new PointF(x2, (0f - gabariteOfVolume.Z) / 10f)
					}, workspace.Font, text2, Color.DarkGreen, ContentAlignment.BottomCenter, Tag));
				}
			}
		}
		foreach (Container Container in Containers)
		{
			if (Container.l > 0f && Container.b > 0f && Container.h > 0f)
			{
				if (x >= Container.pos.X && x <= Container.pos.X + Container.l)
				{
					DrawHullSide(workspace, new List<Station>
					{
						new Station(new MultipleContur(new Contur[1]
						{
							new Contur(new p2d[5]
							{
								new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z),
								new p2d(Container.pos.Y + Container.b / 2f, Container.pos.Z),
								new p2d(Container.pos.Y + Container.b / 2f, Container.pos.Z + Container.h),
								new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z + Container.h),
								new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z)
							})
						}), Container.pos.X),
						new Station(new MultipleContur(new Contur[1]
						{
							new Contur(new p2d[5]
							{
								new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z),
								new p2d(Container.pos.Y + Container.b / 2f, Container.pos.Z),
								new p2d(Container.pos.Y + Container.b / 2f, Container.pos.Z + Container.h),
								new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z + Container.h),
								new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z)
							})
						}), Container.pos.X + Container.l)
					}, drawAllStations: false, "", Container.Name, DrawAxes: false, Tag, Color.Red);
				}
			}
			else
			{
				DrawPoint(workspace, new p3d(Container.pos.X, Container.pos.Y, Container.pos.Z), Tag, Planes.YZ, Container.Name, 2f, Color.Red);
			}
		}
		foreach (Cistern Cistern in Cisterns)
		{
			if (x >= Cistern.x0 && x <= Cistern.x1)
			{
				if (Cistern.percentOfVolume > 0f)
				{
					DrawHullSide(workspace, new List<Station> { GetProjectElements.GetHullSection(Cistern.Stations, x) }, drawAllStations: false, Cistern.Name, "", DrawAxes: false, Tag, Color.Transparent);
					Waterlanding_Volume_Heel_Trim wtrl = new Waterlanding_Volume_Heel_Trim(Cistern.maxVolume * Cistern.percentOfVolume / 100f, heelAngle, trimAngle, Cistern.Stations, 0.001f, Cistern.volumeDensity);
					DrawWL(workspace, wtrl, Tag, Planes.YZ, x, Color.Aqua);
				}
				else
				{
					DrawHullSide(workspace, new List<Station> { GetProjectElements.GetHullSection(Cistern.Stations, x) }, drawAllStations: false, Cistern.Name, "", DrawAxes: false, Tag, Color.Transparent);
				}
			}
		}
		foreach (Compartment Compartment in Compartments)
		{
			Compartment.CompartmentStations = STEB2.Stations.Tools.SortStations(Compartment.CompartmentStations);
			if (x >= Compartment.CompartmentStations[0].X && x <= Compartment.CompartmentStations[Compartment.CompartmentStations.Count - 1].X)
			{
				if (Compartment.percentOfVolume > 0f)
				{
					DrawHullSide(workspace, new List<Station> { GetProjectElements.GetHullSection(Compartment.CompartmentStations, x) }, drawAllStations: false, Compartment.Name, "", DrawAxes: false, Tag, Color.Transparent);
					float maxVolume = STEB2.Volume.Tools.GetMaxVolume(Compartment.CompartmentStations);
					Waterlanding_Volume_Heel_Trim wtrl2 = new Waterlanding_Volume_Heel_Trim(maxVolume * Compartment.percentOfVolume / 100f, heelAngle, trimAngle, Compartment.CompartmentStations, 0.001f, Compartment.volumeDensity);
					DrawWL(workspace, wtrl2, Tag, Planes.YZ, x, Color.Aqua);
				}
				else
				{
					DrawHullSide(workspace, new List<Station> { GetProjectElements.GetHullSection(Compartment.CompartmentStations, x) }, drawAllStations: false, Compartment.Name, "", DrawAxes: false, Tag, Color.Transparent);
				}
			}
		}
	}

	public static void DrawBroadSection(usgModelFrame workspace, List<Station> Stations, List<Container> Containers, List<Cistern> Cisterns, List<Compartment> Compartments, float y, string HeadName, string CenterName, string Tag, string textTransv, float heelAngle, float trimAngle)
	{
		workspace.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(GetProjectElements.GetBroadSection(Stations, y).arrayPoints), 1, Color.Blue, Tag));
		p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(Stations);
		string[] array = textTransv.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Trim() != "")
			{
				float x = STEB2.Converting.Converting.TextToFloat(text.Split(':')[0]);
				string text2 = text.Split(':')[1];
				workspace.Lines.Add(new Line(new PointF[2]
				{
					new PointF(x, (0f - gabariteOfVolume.Z) / 10f),
					new PointF(x, gabariteOfVolume.Z / 10f)
				}, DashStyle.Solid, Color.DarkGreen, 1, Tag));
				workspace.Texts.Add(new Text(new PointF[1]
				{
					new PointF(x, (0f - gabariteOfVolume.Z) / 10f)
				}, workspace.Font, text2, Color.DarkGreen, ContentAlignment.BottomCenter, Tag));
			}
		}
		p3d[] boundPointsOfVolume = STEB2.Volume.Tools.GetBoundPointsOfVolume(Stations);
		if (HeadName != "")
		{
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundPointsOfVolume[0].X + (boundPointsOfVolume[1].X - boundPointsOfVolume[0].X) / 2f, boundPointsOfVolume[1].Z)
			}, workspace.Font, HeadName, Color.Black, ContentAlignment.TopCenter, Tag));
		}
		foreach (Container Container in Containers)
		{
			if (Container.l > 0f && Container.b > 0f && Container.h > 0f)
			{
				if (y >= Container.pos.Y - Container.b / 2f && y <= Container.pos.Y + Container.b / 2f)
				{
					workspace.Fillings.Add(new FillingContur(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(new p2d[5]
					{
						new p2d(Container.pos.X, Container.pos.Z),
						new p2d(Container.pos.X + Container.l, Container.pos.Z),
						new p2d(Container.pos.X + Container.l, Container.pos.Z + Container.h),
						new p2d(Container.pos.X, Container.pos.Z + Container.h),
						new p2d(Container.pos.X, Container.pos.Z)
					}), 1, Color.Black, Color.Red, Tag));
					workspace.Texts.Add(new Text(new PointF[1]
					{
						new PointF(Container.pos.X + Container.l / 2f, Container.pos.Z + Container.h / 2f)
					}, workspace.Font, Container.Name, Color.Black, ContentAlignment.MiddleCenter, Tag));
				}
			}
			else
			{
				DrawPoint(workspace, new p3d(Container.pos.X + Container.l / 2f, Container.pos.Y, Container.pos.Z + Container.h / 2f), Tag, Planes.XZ, Container.Name, 2f, Color.Red);
			}
		}
		foreach (Cistern Cistern in Cisterns)
		{
			if (y >= Cistern.y0 && y <= Cistern.y1)
			{
				if (Cistern.percentOfVolume > 0f)
				{
					workspace.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(GetProjectElements.GetBroadSection(Cistern.Stations, y).arrayPoints), 1, Color.Blue, Tag));
					Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(Cistern.maxVolume * Cistern.percentOfVolume / 100f, heelAngle, trimAngle, Cistern.Stations, 0.001f, Cistern.volumeDensity);
					DrawWL(workspace, waterlanding_Volume_Heel_Trim, Tag, Planes.XZ, y, Color.Aqua);
					DrawPoint(workspace, new p3d(waterlanding_Volume_Heel_Trim.CPos.X, waterlanding_Volume_Heel_Trim.CPos.Y, waterlanding_Volume_Heel_Trim.CPos.Z), Tag, Planes.XZ, Cistern.Name + "\n" + Cistern.CalcMass + " т.\n" + Cistern.CalcVolume + " куб.м.\n" + Cistern.percentOfVolume + "%", 2f, Color.Blue);
				}
				else
				{
					workspace.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(GetProjectElements.GetBroadSection(Cistern.Stations, y).arrayPoints), 1, Color.Blue, Tag));
					DrawPoint(workspace, new p3d(Cistern.x0 + Cistern.l / 2f, Cistern.y0 + Cistern.b / 2f, Cistern.z0 + Cistern.h / 2f), Tag, Planes.XZ, Cistern.Name, 2f, Color.Blue);
				}
			}
		}
		foreach (Compartment Compartment in Compartments)
		{
			if (y >= Compartment.pos.Y - Compartment.b / 2f && y <= Compartment.pos.Y + Compartment.b / 2f)
			{
				if (Compartment.percentOfVolume > 0f)
				{
					workspace.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(GetProjectElements.GetBroadSection(Compartment.CompartmentStations, y).arrayPoints), 1, Color.Blue, Tag));
					float maxVolume = STEB2.Volume.Tools.GetMaxVolume(Compartment.CompartmentStations);
					Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim2 = new Waterlanding_Volume_Heel_Trim(maxVolume * Compartment.percentOfVolume / 100f, heelAngle, trimAngle, Compartment.CompartmentStations, 0.001f, Compartment.volumeDensity);
					DrawWL(workspace, waterlanding_Volume_Heel_Trim2, Tag, Planes.XZ, y, Color.Aqua);
					DrawPoint(workspace, new p3d(waterlanding_Volume_Heel_Trim2.CPos.X, waterlanding_Volume_Heel_Trim2.CPos.Y, waterlanding_Volume_Heel_Trim2.CPos.Z), Tag, Planes.XZ, Compartment.Name + "\n" + waterlanding_Volume_Heel_Trim2.DSPL + " т.\n" + waterlanding_Volume_Heel_Trim2.V + " куб.м.\n" + Compartment.percentOfVolume + "%", 2f, Color.Blue);
				}
				else
				{
					workspace.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(GetProjectElements.GetBroadSection(Compartment.CompartmentStations, y).arrayPoints), 1, Color.Blue, Tag));
					DrawPoint(workspace, new p3d(Compartment.pos.X + Compartment.l / 2f, Compartment.pos.Y + Compartment.b / 2f, Compartment.pos.Z + Compartment.h / 2f), Tag, Planes.XZ, Compartment.Name, 2f, Color.Blue);
				}
			}
		}
	}

	public static void DrawHullSection(usgModelFrame workspace, List<Station> Stations, float x, string HeadName, string CenterName, string Tag, string textLong)
	{
		Station item = new Station(new MultipleContur(new Contur[0]), 0f);
		float num = float.NaN;
		foreach (Station Station in Stations)
		{
			if (float.IsNaN(num))
			{
				item = Station;
				num = Math.Abs(Station.X - x);
			}
			if (num > Math.Abs(Station.X - x))
			{
				num = Math.Abs(Station.X - x);
				item = Station;
			}
		}
		DrawHullSide(workspace, new List<Station> { item }, drawAllStations: false, HeadName, CenterName, DrawAxes: false, Tag, Color.White);
		p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(Stations);
		string[] array = textLong.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Trim() != "")
			{
				float x2 = STEB2.Converting.Converting.TextToFloat(text.Split(':')[0]);
				string text2 = text.Split(':')[1];
				workspace.Lines.Add(new Line(new PointF[2]
				{
					new PointF(x2, (0f - gabariteOfVolume.Z) / 10f),
					new PointF(x2, gabariteOfVolume.Z / 10f)
				}, DashStyle.Solid, Color.DarkGreen, 1, Tag));
				workspace.Texts.Add(new Text(new PointF[1]
				{
					new PointF(x2, (0f - gabariteOfVolume.Z) / 10f)
				}, workspace.Font, text2, Color.DarkGreen, ContentAlignment.BottomCenter, Tag));
			}
		}
	}

	public static void DrawHullSection(usgModelFrame workspace, List<Compartment> Compartments, float x, string HeadName, string CenterName, string Tag, float heelAngle, float trimAngle)
	{
		foreach (Compartment Compartment in Compartments)
		{
			if (!(x >= Compartment.pos.X) || !(x <= Compartment.pos.X + Compartment.l))
			{
				continue;
			}
			Station item = new Station(new MultipleContur(new Contur[0]), 0f);
			float num = float.NaN;
			foreach (Station compartmentStation in Compartment.CompartmentStations)
			{
				if (float.IsNaN(num))
				{
					item = compartmentStation;
					num = Math.Abs(compartmentStation.X - x);
				}
				if (num > Math.Abs(compartmentStation.X - x))
				{
					num = Math.Abs(compartmentStation.X - x);
					item = compartmentStation;
				}
			}
			if (Compartment.percentOfVolume > 0f)
			{
				DrawHullSide(workspace, new List<Station> { item }, drawAllStations: false, Compartment.Name, "", DrawAxes: false, Tag, Color.Transparent);
			}
			else
			{
				DrawHullSide(workspace, new List<Station> { item }, drawAllStations: false, HeadName, Compartment.Name, DrawAxes: false, Tag, Color.Transparent);
			}
		}
		foreach (Compartment Compartment2 in Compartments)
		{
			if (x >= Compartment2.pos.X && x <= Compartment2.pos.X + Compartment2.l && Compartment2.percentOfVolume > 0f)
			{
				float maxVolume = STEB2.Volume.Tools.GetMaxVolume(Compartment2.CompartmentStations);
				Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(maxVolume * Compartment2.percentOfVolume / 100f, heelAngle, trimAngle, Compartment2.CompartmentStations, 0.001f, Compartment2.volumeDensity);
				List<Station> list = new List<Station>();
				ImmersedStation[] immersedStations = waterlanding_Volume_Heel_Trim.immersedStations;
				foreach (ImmersedStation immersedStation in immersedStations)
				{
					list.Add(new Station(immersedStation.StationContur, immersedStation.X));
				}
				DrawHullSide(workspace, list, drawAllStations: false, "", "V = " + (waterlanding_Volume_Heel_Trim.V * Compartment2.coefficientPronicaem).ToString("0.000") + " куб.м.", DrawAxes: false, "immersed_" + Tag, Color.Aqua);
			}
		}
	}

	public static void DrawHullSection(usgModelFrame workspace, List<Cistern> Cisterns, float x, string HeadName, string CenterName, string Tag, float heelAngle, float trimAngle)
	{
		foreach (Cistern Cistern in Cisterns)
		{
			p3d[] boundPointsOfVolume = STEB2.Volume.Tools.GetBoundPointsOfVolume(Cistern.Stations);
			if (!(x >= boundPointsOfVolume[0].X) || !(x <= boundPointsOfVolume[1].X))
			{
				continue;
			}
			Station item = new Station(new MultipleContur(new Contur[0]), 0f);
			float num = float.NaN;
			foreach (Station station in Cistern.Stations)
			{
				if (float.IsNaN(num))
				{
					item = station;
					num = Math.Abs(station.X - x);
				}
				if (num > Math.Abs(station.X - x))
				{
					num = Math.Abs(station.X - x);
					item = station;
				}
			}
			if (Cistern.percentOfVolume > 0f)
			{
				DrawHullSide(workspace, new List<Station> { item }, drawAllStations: false, Cistern.Name, "", DrawAxes: false, Tag, Color.Transparent);
			}
			else
			{
				DrawHullSide(workspace, new List<Station> { item }, drawAllStations: false, HeadName, Cistern.Name, DrawAxes: false, Tag, Color.Transparent);
			}
		}
		foreach (Cistern Cistern2 in Cisterns)
		{
			p3d[] boundPointsOfVolume2 = STEB2.Volume.Tools.GetBoundPointsOfVolume(Cistern2.Stations);
			if (x >= boundPointsOfVolume2[0].X && x <= boundPointsOfVolume2[1].X && Cistern2.percentOfVolume > 0f)
			{
				float maxVolume = STEB2.Volume.Tools.GetMaxVolume(Cistern2.Stations);
				Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(maxVolume * Cistern2.percentOfVolume / 100f, heelAngle, trimAngle, Cistern2.Stations, 0.001f, Cistern2.volumeDensity);
				List<Station> list = new List<Station>();
				ImmersedStation[] immersedStations = waterlanding_Volume_Heel_Trim.immersedStations;
				foreach (ImmersedStation immersedStation in immersedStations)
				{
					list.Add(new Station(immersedStation.StationContur, immersedStation.X));
				}
				DrawHullSide(workspace, list, drawAllStations: false, "", "V = " + waterlanding_Volume_Heel_Trim.V.ToString("0.000") + " куб.м.", DrawAxes: false, "immersed_" + Tag, Color.Aqua);
			}
		}
	}

	public static void DrawHullSection(usgModelFrame workspace, List<Container> Containers, float x, string HeadName, string CenterName, string Tag)
	{
		foreach (Container Container in Containers)
		{
			if (x >= Container.pos.X && x <= Container.pos.X + Container.l)
			{
				DrawHullSide(workspace, new List<Station>
				{
					new Station(new MultipleContur(new Contur[1]
					{
						new Contur(new p2d[5]
						{
							new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z),
							new p2d(Container.pos.Y + Container.b / 2f, Container.pos.Z),
							new p2d(Container.pos.Y + Container.b / 2f, Container.pos.Z + Container.h),
							new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z + Container.h),
							new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z)
						})
					}), Container.pos.X),
					new Station(new MultipleContur(new Contur[1]
					{
						new Contur(new p2d[5]
						{
							new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z),
							new p2d(Container.pos.Y + Container.b / 2f, Container.pos.Z),
							new p2d(Container.pos.Y + Container.b / 2f, Container.pos.Z + Container.h),
							new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z + Container.h),
							new p2d(Container.pos.Y - Container.b / 2f, Container.pos.Z)
						})
					}), Container.pos.X + Container.l)
				}, drawAllStations: false, "", Container.Name, DrawAxes: false, Tag, Color.DarkMagenta);
			}
		}
	}

	public static void DrawBroadSection(usgModelFrame workspace, List<Station> Stations, float y, string HeadName, string CenterName, string Tag, string textTransv)
	{
		List<Station> list = new List<Station>();
		foreach (Station Station in Stations)
		{
			List<Contur> list2 = new List<Contur>();
			Contur[] arrayConturs = Station.StationContur.arrayConturs;
			foreach (Contur contur in arrayConturs)
			{
				List<p2d> clippedRegion = Intersection.GetClippedRegion(contur.listPoints, new p2d(y, 0f), Intersection.ClipOrientation.Left);
				clippedRegion = Intersection.GetClippedRegion(clippedRegion, new p2d(y + 0.1f, 0f), Intersection.ClipOrientation.Right);
				list2.Add(new Contur(clippedRegion));
			}
			list.Add(new Station(new MultipleContur(list2), Station.X));
		}
		DrawBroadSide(workspace, list, drawStationsSled: false, HeadName, CenterName, DrawAxes: false, Tag, Color.White);
		p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(Stations);
		string[] array = textTransv.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Trim() != "")
			{
				float x = STEB2.Converting.Converting.TextToFloat(text.Split(':')[0]);
				string text2 = text.Split(':')[1];
				workspace.Lines.Add(new Line(new PointF[2]
				{
					new PointF(x, (0f - gabariteOfVolume.Z) / 10f),
					new PointF(x, gabariteOfVolume.Z / 10f)
				}, DashStyle.Solid, Color.DarkGreen, 1, Tag));
				workspace.Texts.Add(new Text(new PointF[1]
				{
					new PointF(x, (0f - gabariteOfVolume.Z) / 10f)
				}, workspace.Font, text2, Color.DarkGreen, ContentAlignment.BottomCenter, Tag));
			}
		}
	}

	public static void DrawBroadSection(usgModelFrame workspace, List<Cistern> Cisterns, float y, string HeadName, string CenterName, string Tag, float heelAngle, float trimAngle)
	{
		foreach (Cistern Cistern in Cisterns)
		{
			List<Station> list = new List<Station>();
			foreach (Station station in Cistern.Stations)
			{
				List<Contur> list2 = new List<Contur>();
				Contur[] arrayConturs = station.StationContur.arrayConturs;
				foreach (Contur contur in arrayConturs)
				{
					List<p2d> clippedRegion = Intersection.GetClippedRegion(contur.listPoints, new p2d(y, 0f), Intersection.ClipOrientation.Left);
					clippedRegion = Intersection.GetClippedRegion(clippedRegion, new p2d(y + 0.1f, 0f), Intersection.ClipOrientation.Right);
					if (new Contur(clippedRegion).Area > 0f)
					{
						list2.Add(new Contur(clippedRegion));
					}
				}
				if (list2.Count > 0)
				{
					list.Add(new Station(new MultipleContur(list2), station.X));
				}
			}
			if (list.Count <= 0)
			{
				continue;
			}
			if (Cistern.percentOfVolume > 0f)
			{
				DrawBroadSide(workspace, list, drawStationsSled: false, Cistern.Name, "", DrawAxes: false, Tag, Color.Transparent);
			}
			else
			{
				DrawBroadSide(workspace, list, drawStationsSled: false, "", Cistern.Name, DrawAxes: false, Tag, Color.Transparent);
			}
			if (Cistern.percentOfVolume > 0f)
			{
				float maxVolume = STEB2.Volume.Tools.GetMaxVolume(Cistern.Stations);
				Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(maxVolume * Cistern.percentOfVolume / 100f, heelAngle, trimAngle, Cistern.Stations, 0.001f, Cistern.volumeDensity);
				List<Station> list3 = new List<Station>();
				ImmersedStation[] immersedStations = waterlanding_Volume_Heel_Trim.immersedStations;
				foreach (ImmersedStation immersedStation in immersedStations)
				{
					list3.Add(new Station(immersedStation.StationContur, immersedStation.X));
				}
				DrawBroadSide(workspace, list3, drawStationsSled: false, "", "V = " + waterlanding_Volume_Heel_Trim.V.ToString("0.000") + " куб.м.", DrawAxes: false, "immersed_" + Tag, Color.Aqua);
			}
		}
	}

	public static void DrawBroadSection(usgModelFrame workspace, List<Compartment> Compartments, float y, string HeadName, string CenterName, string Tag, float heelAngle, float trimAngle)
	{
		foreach (Compartment Compartment in Compartments)
		{
			List<Station> list = new List<Station>();
			foreach (Station compartmentStation in Compartment.CompartmentStations)
			{
				List<Contur> list2 = new List<Contur>();
				Contur[] arrayConturs = compartmentStation.StationContur.arrayConturs;
				foreach (Contur contur in arrayConturs)
				{
					List<p2d> clippedRegion = Intersection.GetClippedRegion(contur.listPoints, new p2d(y, 0f), Intersection.ClipOrientation.Left);
					clippedRegion = Intersection.GetClippedRegion(clippedRegion, new p2d(y + 0.1f, 0f), Intersection.ClipOrientation.Right);
					if (new Contur(clippedRegion).Area > 0f)
					{
						list2.Add(new Contur(clippedRegion));
					}
				}
				if (list2.Count > 0)
				{
					list.Add(new Station(new MultipleContur(list2), compartmentStation.X));
				}
			}
			if (list.Count <= 0)
			{
				continue;
			}
			if (Compartment.percentOfVolume > 0f)
			{
				DrawBroadSide(workspace, list, drawStationsSled: false, Compartment.Name, "", DrawAxes: false, Tag, Color.Transparent);
			}
			else
			{
				DrawBroadSide(workspace, list, drawStationsSled: false, "", Compartment.Name, DrawAxes: false, Tag, Color.Transparent);
			}
			if (Compartment.percentOfVolume > 0f)
			{
				float maxVolume = STEB2.Volume.Tools.GetMaxVolume(Compartment.CompartmentStations);
				Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(maxVolume * Compartment.percentOfVolume / 100f, heelAngle, trimAngle, Compartment.CompartmentStations, 0.001f, Compartment.volumeDensity);
				List<Station> list3 = new List<Station>();
				ImmersedStation[] immersedStations = waterlanding_Volume_Heel_Trim.immersedStations;
				foreach (ImmersedStation immersedStation in immersedStations)
				{
					list3.Add(new Station(immersedStation.StationContur, immersedStation.X));
				}
				DrawBroadSide(workspace, list3, drawStationsSled: false, "", "V = " + (waterlanding_Volume_Heel_Trim.V * Compartment.coefficientPronicaem).ToString("0.000") + " куб.м.", DrawAxes: false, "immersed_" + Tag, Color.Aqua);
			}
		}
	}

	public static void DrawBroadSection(usgModelFrame workspace, List<Container> Containers, float y, string HeadName, string CenterName, string Tag)
	{
		foreach (Container Container in Containers)
		{
			float num = Container.pos.Y - Container.b / 2f;
			float num2 = Container.pos.Y + Container.b / 2f;
			if (y >= num && y <= num2)
			{
				DrawBroadSide(workspace, new List<Station>
				{
					new Station(new MultipleContur(new Contur[1]
					{
						new Contur(new p2d[5]
						{
							new p2d((0f - Container.b) / 2f, Container.pos.Z),
							new p2d(Container.b / 2f, Container.pos.Z),
							new p2d(Container.b / 2f, Container.pos.Z + Container.h),
							new p2d((0f - Container.b) / 2f, Container.pos.Z + Container.h),
							new p2d((0f - Container.b) / 2f, Container.pos.Z)
						})
					}), Container.pos.X),
					new Station(new MultipleContur(new Contur[1]
					{
						new Contur(new p2d[5]
						{
							new p2d((0f - Container.b) / 2f, Container.pos.Z),
							new p2d(Container.b / 2f, Container.pos.Z),
							new p2d(Container.b / 2f, Container.pos.Z + Container.h),
							new p2d((0f - Container.b) / 2f, Container.pos.Z + Container.h),
							new p2d((0f - Container.b) / 2f, Container.pos.Z)
						})
					}), Container.pos.X + Container.l)
				}, drawStationsSled: false, "", Container.Name, DrawAxes: false, Tag, Color.DarkMagenta);
			}
		}
	}

	public static void DrawCurves(usgModelFrame workspace, List<StabilityCurve> curves, string Tag)
	{
		Random random = new Random();
		workspace.ClearDB();
		float num = curves[0].Points[0].Y;
		float num2 = curves[0].Points[0].Y;
		float num3 = curves[0].Points[0].X;
		float num4 = curves[0].Points[0].X;
		foreach (StabilityCurve curf in curves)
		{
			Color color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
			workspace.Splines.Add(new Spline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(STEB2.Converting.Converting.ListToArray(curf.Points)), 2, color, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(curf.Points[curf.Points.Count - 1].X, curf.Points[curf.Points.Count - 1].Y)
			}, workspace.Font, curf.Name, color, ContentAlignment.TopRight, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(curf.Points[curf.Points.Count / 2].X, curf.Points[curf.Points.Count / 2].Y)
			}, workspace.Font, curf.Name, color, ContentAlignment.TopRight, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(curf.Points[0].X, curf.Points[0].Y)
			}, workspace.Font, curf.Name, color, ContentAlignment.TopRight, Tag));
		}
		foreach (StabilityCurve curf2 in curves)
		{
			foreach (p2d point in curf2.Points)
			{
				num = ((num > point.Y) ? point.Y : num);
				num2 = ((num2 < point.Y) ? point.Y : num2);
				num4 = ((num4 > point.X) ? point.X : num4);
				num3 = ((num3 < point.X) ? point.X : num3);
			}
		}
		workspace.Lines.Add(new Line(new PointF[2]
		{
			new PointF(num4, num),
			new PointF(num4, num2)
		}, DashStyle.Solid, Color.Blue, 1, Tag, LineCap.NoAnchor, LineCap.ArrowAnchor));
		workspace.Lines.Add(new Line(new PointF[2]
		{
			new PointF(num4, num),
			new PointF(num3, num)
		}, DashStyle.Solid, Color.Blue, 1, Tag, LineCap.NoAnchor, LineCap.ArrowAnchor));
		workspace.Texts.Add(new Text(new PointF[1]
		{
			new PointF(num3, num)
		}, workspace.Font, "\nвеличина\nхарактеристики,\nм, м^2, м^3, т", Color.Blue, ContentAlignment.BottomCenter, Tag, drawAlways: true));
		workspace.Texts.Add(new Text(new PointF[1]
		{
			new PointF(num4, num2)
		}, workspace.Font, "осадка, м", Color.Blue, ContentAlignment.TopCenter, Tag, drawAlways: true));
		float inputValue = (float)Math.Round(num3 - num4, 0) / 10f;
		inputValue = Mathematics.GetStandartFloat(inputValue);
		if (inputValue == 0f)
		{
			inputValue = 0.01f;
		}
		int num5 = 100;
		for (float num6 = num4; num6 <= num3 + inputValue; num6 += inputValue)
		{
			num5--;
			if (num5 < 0)
			{
				break;
			}
			workspace.Lines.Add(new Line(new PointF[2]
			{
				new PointF(num6, num),
				new PointF(num6, num2)
			}, DashStyle.Dash, Color.Green, 1, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(num6, num)
			}, workspace.Font, num6.ToString("0.000"), Color.Blue, ContentAlignment.BottomCenter, Tag));
		}
		float inputValue2 = (float)Math.Round(num2 - num, 0) / 10f;
		inputValue2 = Mathematics.GetStandartFloat(inputValue2);
		if (inputValue2 == 0f)
		{
			inputValue2 = 0.01f;
		}
		int num7 = 100;
		for (float num8 = num; num8 <= num2 + inputValue2; num8 += inputValue2)
		{
			num7--;
			if (num7 < 0)
			{
				break;
			}
			workspace.Lines.Add(new Line(new PointF[2]
			{
				new PointF(num4, num8),
				new PointF(num3, num8)
			}, DashStyle.Dash, Color.Green, 1, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(num4, num8)
			}, workspace.Font, num8.ToString("0.000"), Color.Blue, ContentAlignment.MiddleLeft, Tag));
		}
		num7 = 100;
		for (float num9 = num; num9 >= num - inputValue2; num9 -= inputValue2)
		{
			num7--;
			if (num7 < 0)
			{
				break;
			}
			workspace.Lines.Add(new Line(new PointF[2]
			{
				new PointF(num4, num9),
				new PointF(num4, num9)
			}, DashStyle.Dash, Color.Green, 1, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(num4, num9)
			}, workspace.Font, num9.ToString("0.000"), Color.Blue, ContentAlignment.MiddleRight, Tag));
		}
		workspace.ZoomAll();
	}

	public static void DrawPantokarens(usgModelFrame workspace, List<StabilityCurve> stabilities, string Tag)
	{
		Random random = new Random();
		workspace.ClearDB();
		float num = stabilities[0].Points[0].Y;
		float num2 = stabilities[0].Points[0].Y;
		float num3 = stabilities[0].Points[0].X;
		float num4 = stabilities[0].Points[0].X;
		foreach (StabilityCurve stability in stabilities)
		{
			Color color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
			workspace.Splines.Add(new Spline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(STEB2.Converting.Converting.ListToArray(stability.Points)), 2, color, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(stability.Points[stability.Points.Count - 1].X, stability.Points[stability.Points.Count - 1].Y)
			}, workspace.Font, stability.Name, color, ContentAlignment.TopRight, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(stability.Points[stability.Points.Count / 2].X, stability.Points[stability.Points.Count / 2].Y)
			}, workspace.Font, stability.Name, color, ContentAlignment.TopRight, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(stability.Points[0].X, stability.Points[0].Y)
			}, workspace.Font, stability.Name, color, ContentAlignment.TopRight, Tag));
		}
		foreach (StabilityCurve stability2 in stabilities)
		{
			foreach (p2d point in stability2.Points)
			{
				num = ((num > point.Y) ? point.Y : num);
				num2 = ((num2 < point.Y) ? point.Y : num2);
				num4 = ((num4 > point.X) ? point.X : num4);
				num3 = ((num3 < point.X) ? point.X : num3);
			}
		}
		using (List<StabilityCurve>.Enumerator enumerator4 = stabilities.GetEnumerator())
		{
			if (enumerator4.MoveNext())
			{
				StabilityCurve current4 = enumerator4.Current;
				foreach (p2d point2 in current4.Points)
				{
					workspace.Lines.Add(new Line(new PointF[2]
					{
						new PointF(point2.X, num),
						new PointF(point2.X, num2)
					}, DashStyle.Dash, Color.Green, 1, Tag));
					List<Text> texts = workspace.Texts;
					PointF[] pnts = new PointF[1]
					{
						new PointF(point2.X, num)
					};
					Font font = workspace.Font;
					float x = point2.X;
					texts.Add(new Text(pnts, font, x.ToString(), Color.Blue, ContentAlignment.BottomCenter, Tag));
				}
			}
		}
		workspace.Lines.Add(new Line(new PointF[2]
		{
			new PointF(num4, num),
			new PointF(num4, num2)
		}, DashStyle.Solid, Color.Blue, 1, Tag, LineCap.NoAnchor, LineCap.ArrowAnchor));
		workspace.Lines.Add(new Line(new PointF[2]
		{
			new PointF(num4, num),
			new PointF(num3, num)
		}, DashStyle.Solid, Color.Blue, 1, Tag, LineCap.NoAnchor, LineCap.ArrowAnchor));
		workspace.Texts.Add(new Text(new PointF[1]
		{
			new PointF(num3, num)
		}, workspace.Font, "\nобъем, куб.м.", Color.Blue, ContentAlignment.BottomCenter, Tag, drawAlways: true));
		workspace.Texts.Add(new Text(new PointF[1]
		{
			new PointF(num4, num2)
		}, workspace.Font, "плечи остойчивости формы, м", Color.Blue, ContentAlignment.TopCenter, Tag, drawAlways: true));
		float inputValue = (float)Math.Round(num2, 0) / 10f;
		inputValue = Mathematics.GetStandartFloat(inputValue);
		if (inputValue == 0f)
		{
			inputValue = 0.01f;
		}
		int num5 = 100;
		for (float num6 = num; num6 <= num2 + inputValue; num6 += inputValue)
		{
			num5--;
			if (num5 < 0)
			{
				break;
			}
			workspace.Lines.Add(new Line(new PointF[2]
			{
				new PointF(num4, num6),
				new PointF(num3, num6)
			}, DashStyle.Dash, Color.Green, 1, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(num4, num6)
			}, workspace.Font, num6.ToString("0.000"), Color.Blue, ContentAlignment.MiddleLeft, Tag, drawAlways: true));
		}
		num5 = 100;
		for (float num7 = num; num7 >= num - inputValue; num7 -= inputValue)
		{
			num5--;
			if (num5 < 0)
			{
				break;
			}
			workspace.Lines.Add(new Line(new PointF[2]
			{
				new PointF(num4, num7),
				new PointF(num4, num7)
			}, DashStyle.Dash, Color.Green, 1, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(num4, num7)
			}, workspace.Font, num7.ToString("0.000"), Color.Blue, ContentAlignment.MiddleRight, Tag, drawAlways: true));
		}
		workspace.ZoomAll();
	}

	public static void DrawStabilityDiagramm(usgModelFrame workspace, List<p2d> diagrammPoints, List<p2d> diagrammDPoints, float DSPL, string Tag, float startAngle, float endAngle, float stepAngle, bool DrawMetacentrH, bool IsDrawDdiagramm)
	{
		if (MessageBox.Show("Рисовать диаграммы внакладку?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
		{
			workspace.ClearDB();
		}
		else
		{
			workspace.ClearDBExactly(Tag);
			workspace.ReColorGroup(Color.Gray, new string[1] { "Curve_" + Tag });
		}
		if (startAngle > endAngle || stepAngle <= 0f)
		{
			return;
		}
		List<p2d> list = new List<p2d>();
		List<p2d> list2 = new List<p2d>();
		for (int i = 0; i < diagrammPoints.Count; i++)
		{
			if (diagrammPoints[i].X >= startAngle && diagrammPoints[i].X <= endAngle)
			{
				list.Add(new p2d(diagrammPoints[i].X, diagrammPoints[i].Y));
			}
		}
		for (int j = 0; j < diagrammDPoints.Count; j++)
		{
			if (diagrammDPoints[j].X >= startAngle && diagrammDPoints[j].X <= endAngle)
			{
				list2.Add(new p2d(diagrammDPoints[j].X, diagrammDPoints[j].Y));
			}
		}
		diagrammPoints = list;
		diagrammDPoints = list2;
		workspace.Splines.Add(new Spline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(STEB2.Converting.Converting.ListToArray(diagrammPoints)), 2, Color.Red, "Curve_" + Tag));
		if (IsDrawDdiagramm)
		{
			workspace.Splines.Add(new Spline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(STEB2.Converting.Converting.ListToArray(diagrammDPoints)), 2, Color.Blue, "Curve_" + Tag));
		}
		StabilityCurve stabilityCurve = new TransverseStaticStability(DSPL, Tag, diagrammPoints);
		float num = diagrammPoints[0].Y;
		float num2 = diagrammPoints[0].Y;
		float num3 = diagrammPoints[0].X;
		float num4 = diagrammPoints[0].X;
		for (int k = 0; k < diagrammPoints.Count; k++)
		{
			num = ((num > diagrammPoints[k].Y) ? diagrammPoints[k].Y : num);
			num2 = ((num2 < diagrammPoints[k].Y) ? diagrammPoints[k].Y : num2);
			num4 = ((num4 > diagrammPoints[k].X) ? diagrammPoints[k].X : num4);
			num3 = ((num3 < diagrammPoints[k].X) ? diagrammPoints[k].X : num3);
		}
		workspace.Lines.Add(new Line(new PointF[2]
		{
			new PointF(0f, 0f),
			new PointF(0f, num2)
		}, DashStyle.Solid, Color.Blue, 1, Tag, LineCap.NoAnchor, LineCap.ArrowAnchor));
		workspace.Lines.Add(new Line(new PointF[2]
		{
			new PointF(0f, 0f),
			new PointF(0f, num)
		}, DashStyle.Solid, Color.Blue, 1, Tag));
		workspace.Lines.Add(new Line(new PointF[2]
		{
			new PointF(startAngle, 0f),
			new PointF(endAngle, 0f)
		}, DashStyle.Solid, Color.Blue, 1, Tag, LineCap.NoAnchor, LineCap.ArrowAnchor));
		workspace.Texts.Add(new Text(new PointF[1]
		{
			new PointF(endAngle, 0f)
		}, workspace.Font, "\nуглы крена, градусы", Color.Blue, ContentAlignment.TopCenter, Tag, drawAlways: true));
		workspace.Texts.Add(new Text(new PointF[1]
		{
			new PointF(0f, num2)
		}, workspace.Font, "плечи остойчивости, м", Color.Blue, ContentAlignment.TopCenter, Tag, drawAlways: true));
		for (float num5 = startAngle; num5 <= endAngle; num5 += stepAngle)
		{
			if (num5 > 0f)
			{
				workspace.Lines.Add(new Line(new PointF[2]
				{
					new PointF(num5, 0f),
					new PointF(num5, num2)
				}, DashStyle.Dash, Color.Green, 1, Tag));
			}
			else
			{
				workspace.Lines.Add(new Line(new PointF[2]
				{
					new PointF(num5, 0f),
					new PointF(num5, num)
				}, DashStyle.Dash, Color.Green, 1, Tag));
			}
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(num5, 0f)
			}, workspace.Font, num5.ToString(), Color.Blue, ContentAlignment.BottomCenter, Tag));
		}
		float inputValue = (float)Math.Round(num2, 0) / 10f;
		inputValue = Mathematics.GetStandartFloat(inputValue);
		if (inputValue == 0f)
		{
			inputValue = 0.01f;
		}
		int num6 = 100;
		for (float num7 = 0f; num7 <= num2 + inputValue; num7 += inputValue)
		{
			num6--;
			if (num6 < 0)
			{
				break;
			}
			workspace.Lines.Add(new Line(new PointF[2]
			{
				new PointF(0f, num7),
				new PointF(num3, num7)
			}, DashStyle.Dash, Color.Green, 1, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(0f, num7)
			}, workspace.Font, num7.ToString("0.000"), Color.Blue, ContentAlignment.MiddleLeft, Tag));
		}
		num6 = 100;
		for (float num8 = 0f; num8 >= num - inputValue; num8 -= inputValue)
		{
			num6--;
			if (num6 < 0)
			{
				break;
			}
			workspace.Lines.Add(new Line(new PointF[2]
			{
				new PointF(0f, num8),
				new PointF(num4, num8)
			}, DashStyle.Dash, Color.Green, 1, Tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(0f, num8)
			}, workspace.Font, num8.ToString("0.000"), Color.Blue, ContentAlignment.MiddleRight, Tag));
		}
		if (!DrawMetacentrH)
		{
			return;
		}
		float num9 = stabilityCurve.ReturnTrueAngleOnWater();
		for (int l = 0; l < stabilityCurve.Points.Count; l++)
		{
			if (stabilityCurve.Points[l].X > num9)
			{
				workspace.Lines.Add(new Line(new PointF[2]
				{
					new PointF(num9, 0f),
					new PointF(num9 + 57.3f, 0f)
				}, DashStyle.Solid, Color.Black, 1, Tag));
				p2d xPoint = Intersection.GetXPoint(new p2d(num9, 0f), new p2d(stabilityCurve.Points[l].X, stabilityCurve.Points[l].Y), new p2d(num9 + 57.3f, 0f), new p2d(num9 + 57.3f, 1f), isExtendFirst: true, isExtendSecond: true);
				workspace.Lines.Add(new Line(new PointF[2]
				{
					new PointF(num9, 0f),
					new PointF(xPoint.X, xPoint.Y)
				}, DashStyle.Solid, Color.Black, 1, Tag));
				workspace.Dimensions.Add(new Dimension(new PointF(xPoint.X, xPoint.Y), new PointF(num9 + 57.3f, 0f), DimOrientation.vert, 10f, Mathematics.GetVectorLength(new p2d[2]
				{
					new p2d(num9 + 57.3f, 0f),
					new p2d(xPoint.X, xPoint.Y)
				}).ToString(), workspace.Font, DashStyle.Solid, Color.Black, 1, Tag));
				workspace.Dimensions.Add(new Dimension(new PointF(num9, 0f), new PointF(num9 + 57.3f, 0f), DimOrientation.hor, 10f, Mathematics.GetVectorLength(new p2d[2]
				{
					new p2d(num9, 0f),
					new p2d(num9 + 57.3f, 0f)
				}).ToString(), workspace.Font, DashStyle.Solid, Color.Black, 1, Tag));
				break;
			}
		}
	}

	public static void DrawTransverseArmYZ(usgModelFrame workspace, PointF[] ArmPointForm, PointF[] ArmPointMass, string tag)
	{
		workspace.Polylines.Add(new Polyline(DashStyle.Solid, ArmPointForm, 5, Color.Aqua, tag));
		workspace.Polylines.Add(new Polyline(DashStyle.Solid, ArmPointMass, 5, Color.Red, tag));
	}

	public static void DrawLongArmXZ(usgModelFrame workspace, PointF[] ArmPointForm, PointF[] ArmPointMass, string tag)
	{
		workspace.Polylines.Add(new Polyline(DashStyle.Solid, ArmPointForm, 5, Color.Aqua, tag));
		workspace.Polylines.Add(new Polyline(DashStyle.Solid, ArmPointMass, 5, Color.Red, tag));
	}

	public static void DrawBroadSide(usgModelFrame workspace, List<Station> stations, bool drawStationsSled, string HeadName, string CenterName, bool DrawAxes, string tag, Color fillColor)
	{
		Contur broadProject = GetProjectElements.GetBroadProject(stations);
		p2d[] boundRect = Mathematics.GetBoundRect(broadProject.arrayPoints);
		if (fillColor == Color.Transparent)
		{
			workspace.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(broadProject.arrayPoints), 1, Color.Blue, tag));
		}
		else
		{
			workspace.Fillings.Add(new FillingContur(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(broadProject.arrayPoints), 1, Color.Black, fillColor, tag));
		}
		if (CenterName != "")
		{
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[0].X + (boundRect[2].X - boundRect[0].X) / 2f, boundRect[0].Y + (boundRect[1].Y - boundRect[0].Y) / 2f)
			}, workspace.Font, CenterName, Color.Black, ContentAlignment.MiddleCenter, tag));
		}
		if (HeadName != "")
		{
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[0].X + (boundRect[2].X - boundRect[0].X) / 2f, boundRect[1].Y)
			}, workspace.Font, HeadName, Color.Black, ContentAlignment.TopCenter, tag));
		}
		if (DrawAxes)
		{
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[0].X, boundRect[0].Y)
			}, workspace.Font, "ОЛ", Color.Black, ContentAlignment.BottomLeft, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[3].X, boundRect[3].Y)
			}, workspace.Font, "ОЛ", Color.Black, ContentAlignment.BottomLeft, tag));
			workspace.Lines.Add(new Line(new PointF[2]
			{
				new PointF(boundRect[0].X, boundRect[0].Y),
				new PointF(boundRect[3].X, boundRect[3].Y)
			}, DashStyle.Solid, Color.Blue, 2, tag, LineCap.NoAnchor, LineCap.ArrowAnchor));
		}
		if (!drawStationsSled)
		{
			return;
		}
		foreach (Station station in stations)
		{
			workspace.Polylines.Add(new Polyline(DashStyle.DashDot, STEB2.Converting.Converting.P2dToPointF(station.GetSledOnDP()), 1, Color.DarkGreen, tag));
			workspace.Texts.Add(new Text(STEB2.Converting.Converting.P2dToPointF(station.GetSledOnDP()), workspace.Font, station.X.ToString(), Color.Black, ContentAlignment.BottomCenter, tag));
		}
	}

	public static void DrawTopSide(usgModelFrame workspace, List<Station> stations, bool drawStationsSled, string HeadName, string CenterName, bool DrawAxes, string tag, Color fillColor)
	{
		Contur topProject = GetProjectElements.GetTopProject(stations);
		p2d[] boundRect = Mathematics.GetBoundRect(topProject.arrayPoints);
		workspace.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(topProject.arrayPoints), 1, Color.Blue, tag));
		workspace.FillPolylines(tag, fillColor);
		if (CenterName != "")
		{
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[0].X + (boundRect[2].X - boundRect[0].X) / 2f, boundRect[0].Y + (boundRect[1].Y - boundRect[0].Y) / 2f)
			}, workspace.Font, CenterName, Color.Black, ContentAlignment.MiddleCenter, tag));
		}
		if (HeadName != "")
		{
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[0].X + (boundRect[2].X - boundRect[0].X) / 2f, boundRect[1].Y)
			}, workspace.Font, HeadName, Color.Black, ContentAlignment.TopCenter, tag));
		}
		if (DrawAxes)
		{
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[0].X, 0f)
			}, workspace.Font, "ДП", Color.Black, ContentAlignment.BottomLeft, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[2].X, 0f)
			}, workspace.Font, "ДП", Color.Black, ContentAlignment.BottomRight, tag));
			workspace.Lines.Add(new Line(new PointF[2]
			{
				new PointF(boundRect[0].X, 0f),
				new PointF(boundRect[2].X, 0f)
			}, DashStyle.Solid, Color.Blue, 2, tag, LineCap.NoAnchor, LineCap.ArrowAnchor));
		}
		if (!drawStationsSled)
		{
			return;
		}
		foreach (Station station in stations)
		{
			workspace.Polylines.Add(new Polyline(DashStyle.DashDot, STEB2.Converting.Converting.P2dToPointF(station.GetSledOnTop()), 1, Color.DarkGreen, tag));
			workspace.Texts.Add(new Text(STEB2.Converting.Converting.P2dToPointF(station.GetSledOnTop()), workspace.Font, station.X.ToString(), Color.Black, ContentAlignment.BottomCenter, tag));
		}
	}

	public static void DrawHullSide(usgModelFrame workspace, List<Station> stations, bool drawAllStations, string HeadName, string CenterName, bool DrawAxes, string tag, Color fillColor)
	{
		if (drawAllStations)
		{
			foreach (Station station in stations)
			{
				Contur[] arrayConturs = station.StationContur.arrayConturs;
				foreach (Contur contur in arrayConturs)
				{
					workspace.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(contur.arrayPoints), 1, Color.Blue, tag));
				}
			}
		}
		else if (fillColor == Color.Transparent)
		{
			workspace.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(GetProjectElements.GetHullProject(stations).arrayPoints), 1, Color.Blue, tag));
		}
		else
		{
			workspace.Fillings.Add(new FillingContur(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(GetProjectElements.GetHullProject(stations).arrayPoints), 1, Color.Black, fillColor, tag));
		}
		p2d[] boundRect = Mathematics.GetBoundRect(GetProjectElements.GetHullProject(stations).arrayPoints);
		if (CenterName != "")
		{
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[0].X + (boundRect[2].X - boundRect[0].X) / 2f, boundRect[0].Y + (boundRect[1].Y - boundRect[0].Y) / 2f)
			}, workspace.Font, CenterName, Color.Black, ContentAlignment.MiddleCenter, tag));
		}
		if (HeadName != "")
		{
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[0].X + (boundRect[2].X - boundRect[0].X) / 2f, boundRect[1].Y)
			}, workspace.Font, HeadName, Color.Black, ContentAlignment.TopCenter, tag));
		}
		if (DrawAxes)
		{
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[0].X, boundRect[0].Y)
			}, workspace.Font, "ОЛ", Color.Black, ContentAlignment.BottomRight, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(boundRect[3].X, boundRect[3].Y)
			}, workspace.Font, "ОЛ", Color.Black, ContentAlignment.BottomLeft, tag));
			workspace.Lines.Add(new Line(new PointF[2]
			{
				new PointF(boundRect[0].X, boundRect[0].Y),
				new PointF(boundRect[3].X, boundRect[3].Y)
			}, DashStyle.Solid, Color.Blue, 2, tag, LineCap.NoAnchor, LineCap.ArrowAnchor));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(0f, 0f)
			}, workspace.Font, "ДП", Color.Black, ContentAlignment.BottomCenter, tag));
			workspace.Lines.Add(new Line(new PointF[2]
			{
				new PointF(0f, 0f),
				new PointF(0f, boundRect[2].Y)
			}, DashStyle.Solid, Color.Blue, 2, tag, LineCap.NoAnchor, LineCap.ArrowAnchor));
		}
		if (!drawAllStations)
		{
			return;
		}
		foreach (Station station2 in stations)
		{
			Contur[] arrayConturs2 = station2.StationContur.arrayConturs;
			foreach (Contur contur2 in arrayConturs2)
			{
				workspace.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(contur2.arrayPoints), 1, Color.Black, tag));
			}
		}
	}

	public static void DrawGPoint(usgModelFrame workspace, p3d Gpnt, string tag, Planes plane)
	{
		switch (plane)
		{
		case Planes.XZ:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Gpnt.X, Gpnt.Z)
			}, 10f, Color.Red, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Gpnt.X, Gpnt.Z)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "G", Color.Red, ContentAlignment.BottomRight, tag));
			break;
		case Planes.XY:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Gpnt.X, Gpnt.Y)
			}, 10f, Color.Red, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Gpnt.X, Gpnt.Y)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "G", Color.Red, ContentAlignment.BottomRight, tag));
			break;
		case Planes.YZ:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Gpnt.Y, Gpnt.Z)
			}, 10f, Color.Red, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Gpnt.Y, Gpnt.Z)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "G", Color.Red, ContentAlignment.BottomRight, tag));
			break;
		}
	}

	public static void DrawPoint(usgModelFrame workspace, p3d point, string tag, Planes plane, string PointText, float PointSize, Color color)
	{
		switch (plane)
		{
		case Planes.XZ:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(point.X, point.Z)
			}, PointSize, color, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(point.X, point.Z)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), PointText, color, ContentAlignment.BottomRight, tag));
			break;
		case Planes.XY:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(point.X, point.Y)
			}, PointSize, color, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(point.X, point.Y)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), PointText, color, ContentAlignment.BottomRight, tag));
			break;
		case Planes.YZ:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(point.Y, point.Z)
			}, PointSize, color, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(point.Y, point.Z)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), PointText, color, ContentAlignment.BottomRight, tag));
			break;
		}
	}

	public static void DrawСMPoint(usgModelFrame workspace, p3d Сpnt, p3d Mpnt, string tag, Planes plane)
	{
		switch (plane)
		{
		case Planes.XZ:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Сpnt.X, Сpnt.Z)
			}, 10f, Color.Blue, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Сpnt.X, Сpnt.Z)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "С", Color.Blue, ContentAlignment.BottomRight, tag));
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Mpnt.X, Mpnt.Z)
			}, 10f, Color.DarkGreen, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Mpnt.X, Mpnt.Z)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "M", Color.DarkGreen, ContentAlignment.BottomRight, tag));
			break;
		case Planes.XY:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Сpnt.X, Сpnt.Y)
			}, 10f, Color.Blue, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Сpnt.X, Сpnt.Y)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "С", Color.Blue, ContentAlignment.BottomRight, tag));
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Mpnt.X, Mpnt.Y)
			}, 10f, Color.DarkGreen, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Mpnt.X, Mpnt.Y)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "M", Color.DarkGreen, ContentAlignment.BottomRight, tag));
			break;
		case Planes.YZ:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Сpnt.Y, Сpnt.Z)
			}, 10f, Color.Blue, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Сpnt.Y, Сpnt.Z)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "С", Color.Blue, ContentAlignment.BottomRight, tag));
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Mpnt.Y, Mpnt.Z)
			}, 10f, Color.DarkGreen, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Mpnt.Y, Mpnt.Z)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "M", Color.DarkGreen, ContentAlignment.BottomRight, tag));
			break;
		}
	}

	public static void DrawСMPoint(usgModelFrame workspace, p3d Сpnt, string tag, Planes plane)
	{
		switch (plane)
		{
		case Planes.XZ:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Сpnt.X, Сpnt.Z)
			}, 10f, Color.Blue, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Сpnt.X, Сpnt.Z)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "С", Color.Blue, ContentAlignment.BottomRight, tag));
			break;
		case Planes.XY:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Сpnt.X, Сpnt.Y)
			}, 10f, Color.Blue, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Сpnt.X, Сpnt.Y)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "С", Color.Blue, ContentAlignment.BottomRight, tag));
			break;
		case Planes.YZ:
			workspace.Dots.Add(new Dot(new PointF[1]
			{
				new PointF(Сpnt.Y, Сpnt.Z)
			}, 10f, Color.Blue, tag));
			workspace.Texts.Add(new Text(new PointF[1]
			{
				new PointF(Сpnt.Y, Сpnt.Z)
			}, new Font(workspace.Font.FontFamily, workspace.Font.Size + 2f, FontStyle.Bold), "С", Color.Blue, ContentAlignment.BottomRight, tag));
			break;
		}
	}

	public static void DrawWL(usgModelFrame workspace, Waterlanding wtrl, string tag, Planes plane, float xORy, Color fillSubColor)
	{
		try
		{
			p3d wlPosition = wtrl.wlPosition;
			p3d p3d2 = Transform.OffsetPoint(wtrl.HeelAngle, wtrl.TrimAngle, 1f);
			float x = p3d2.X;
			float y = p3d2.Y;
			float z = p3d2.Z;
			float x2 = wlPosition.X;
			float y2 = wlPosition.Y;
			float z2 = wlPosition.Z;
			List<Station> list = new List<Station>();
			ImmersedStation[] immersedStations = wtrl.immersedStations;
			foreach (ImmersedStation immersedStation in immersedStations)
			{
				list.Add(new Station(immersedStation.StationContur, immersedStation.X));
			}
			STEB2.Stations.Tools.SortStations(list);
			p3d[] boundPointsOfVolume = STEB2.Volume.Tools.GetBoundPointsOfVolume(list);
			PointF pointF = new PointF(0f, 0f);
			PointF pointF2 = new PointF(0f, 0f);
			switch (plane)
			{
			case Planes.XZ:
			{
				Contur broadProject = GetProjectElements.GetBroadProject(list);
				pointF.X = boundPointsOfVolume[0].X;
				pointF.Y = x * x2 - x * pointF.X - y * xORy + y * y2 + z * z2 / z;
				pointF2.X = boundPointsOfVolume[1].X;
				pointF2.Y = x * x2 - x * pointF2.X - y * xORy + y * y2 + z * z2 / z;
				List<p2d> xPointConturLine2 = Intersection.GetXPointConturLine(new p2d(pointF.X, pointF.Y), new p2d(pointF2.X, pointF2.Y), isExtendLine: true, GetProjectElements.GetBroadProject(wtrl.stations));
				if (fillSubColor != Color.Transparent)
				{
					workspace.Fillings.Add(new FillingContur(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(broadProject.arrayPoints), 1, Color.DarkBlue, fillSubColor, tag));
				}
				workspace.Polylines.Add(new Polyline(DashStyle.DashDotDot, STEB2.Converting.Converting.P2dToPointF(xPointConturLine2.ToArray()), 2, Color.DarkBlue, tag));
				break;
			}
			case Planes.XY:
			{
				foreach (p3d item in wtrl.ipWithWaterplane)
				{
					workspace.Dots.Add(new Dot(new PointF[1]
					{
						new PointF(item.X, item.Y)
					}, 3f, Color.Blue, tag));
				}
				break;
			}
			case Planes.YZ:
			{
				Contur hullProject = GetProjectElements.GetHullProject(list);
				pointF.X = boundPointsOfVolume[0].Y;
				pointF.Y = x * x2 - x * xORy - y * pointF.X + y * y2 + z * z2 / z;
				pointF2.X = boundPointsOfVolume[1].Y;
				pointF2.Y = x * x2 - x * xORy - y * pointF2.X + y * y2 + z * z2 / z;
				List<p2d> xPointConturLine = Intersection.GetXPointConturLine(new p2d(pointF.X, pointF.Y), new p2d(pointF2.X, pointF2.Y), isExtendLine: true, GetProjectElements.GetHullProject(wtrl.stations));
				if (fillSubColor != Color.Transparent)
				{
					workspace.Fillings.Add(new FillingContur(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(hullProject.arrayPoints), 1, Color.DarkBlue, fillSubColor, tag));
				}
				workspace.Polylines.Add(new Polyline(DashStyle.DashDotDot, STEB2.Converting.Converting.P2dToPointF(xPointConturLine.ToArray()), 2, Color.DarkBlue, tag));
				break;
			}
			}
		}
		catch
		{
		}
	}
}
