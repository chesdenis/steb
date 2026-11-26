using System;
using System.Collections.Generic;
using STEB2.Geometry;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB2.Compartments;

public class Compartment
{
	public string Name;

	public float l;

	public float b;

	public float h;

	public p3d pos;

	public float percentOfVolume;

	public float volumeDensity;

	public float coefficientPronicaem;

	public Waterlanding wtrl;

	public List<Station> CompartmentStations;

	public List<Station> Stations;

	public Compartment(string Name, float l, float b, float h, p3d pos, float volumeDensity, float percentOfVolume, float coefficientPronicaem)
	{
		this.Name = Name;
		this.l = l;
		this.b = b;
		this.h = h;
		this.pos = new p3d(pos.X, pos.Y, pos.Z);
		this.percentOfVolume = percentOfVolume;
		this.volumeDensity = volumeDensity;
		this.coefficientPronicaem = coefficientPronicaem;
	}

	public void CalcCompartment(List<Station> ShipFormStations)
	{
		if (percentOfVolume > 100f)
		{
			percentOfVolume = 100f;
		}
		if (percentOfVolume < 0f)
		{
			percentOfVolume = 0f;
		}
		if (coefficientPronicaem > 1f)
		{
			coefficientPronicaem = 1f;
		}
		if (coefficientPronicaem < 0f)
		{
			coefficientPronicaem = 0f;
		}
		List<Station> list = new List<Station>();
		ShipFormStations = STEB2.Stations.Tools.SortStations(ShipFormStations);
		int index = 0;
		float num = 0f;
		for (int i = 0; i < ShipFormStations.Count; i++)
		{
			if (i == 0)
			{
				num = Math.Abs(pos.X - ShipFormStations[i].X);
			}
			else if (num > Math.Abs(pos.X - ShipFormStations[i].X))
			{
				num = Math.Abs(pos.X - ShipFormStations[i].X);
				index = i;
			}
		}
		Station station = new Station(new MultipleContur(ShipFormStations[index].StationContur.arrayConturs), pos.X);
		int index2 = 0;
		float num2 = 0f;
		for (int j = 0; j < ShipFormStations.Count; j++)
		{
			if (j == 0)
			{
				num2 = Math.Abs(pos.X + this.l - ShipFormStations[j].X);
			}
			else if (num2 > Math.Abs(pos.X + this.l - ShipFormStations[j].X))
			{
				num2 = Math.Abs(pos.X + this.l - ShipFormStations[j].X);
				index2 = j;
			}
		}
		Station station2 = new Station(new MultipleContur(ShipFormStations[index2].StationContur.arrayConturs), pos.X + this.l);
		list.Add(station);
		for (int k = 0; k < ShipFormStations.Count; k++)
		{
			if (ShipFormStations[k].X > station.X && ShipFormStations[k].X < station2.X)
			{
				list.Add(new Station(new MultipleContur(ShipFormStations[k].StationContur.arrayConturs), ShipFormStations[k].X));
			}
		}
		list.Add(station2);
		Stations = list;
		CompartmentStations = new List<Station>();
		foreach (Station item2 in list)
		{
			CompartmentStations.Add(new Station(new MultipleContur(item2.StationContur.arrayConturs), item2.X));
		}
		List<Station> list2 = new List<Station>();
		Progress.ProgressMaximum += CompartmentStations.Count;
		for (int l = 0; l < CompartmentStations.Count; l++)
		{
			List<Contur> list3 = new List<Contur>();
			Contur[] arrayConturs = CompartmentStations[l].StationContur.arrayConturs;
			foreach (Contur contur in arrayConturs)
			{
				List<p2d> clippedRegion = Intersection.GetClippedRegion(contur.listPoints, new p2d(pos.Y - b / 2f, pos.Z), Intersection.ClipOrientation.Left);
				clippedRegion = Intersection.GetClippedRegion(clippedRegion, new p2d(pos.Y + b / 2f, pos.Z), Intersection.ClipOrientation.Right);
				clippedRegion = Intersection.GetClippedRegion(clippedRegion, new p2d(pos.Y, pos.Z + h), Intersection.ClipOrientation.Down);
				clippedRegion = Intersection.GetClippedRegion(clippedRegion, new p2d(pos.Y, pos.Z), Intersection.ClipOrientation.Up);
				list3.Add(new Contur(clippedRegion));
			}
			Station item = new Station(new MultipleContur(list3), CompartmentStations[l].X);
			list2.Add(item);
		}
		Progress.ProgressMaximum -= CompartmentStations.Count;
		CompartmentStations = list2;
	}

	public Compartment(string Name, float l, float b, float h, p3d pos, float percentOfVolume, float volumeDensity, List<Station> ShipFormStations, float coefficientPronicaem)
	{
		this.Name = Name;
		this.l = l;
		this.b = b;
		this.h = h;
		this.pos = new p3d(pos.X, pos.Y, pos.Z);
		this.percentOfVolume = percentOfVolume;
		this.volumeDensity = volumeDensity;
		this.coefficientPronicaem = coefficientPronicaem;
		CalcCompartment(ShipFormStations);
	}
}
