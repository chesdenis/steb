using System;
using System.Collections.Generic;
using STEB2.Geometry;

namespace STEB2.Stations;

public class ImmersedStation
{
	public float X;

	public MultipleContur StationContur;

	public List<float> lstOfXWaterline;

	public List<p2d> lstWaterplanePoints;

	public float y;

	public ImmersedStation(MultipleContur StationContur, float X)
	{
		lstOfXWaterline = new List<float>();
		lstWaterplanePoints = new List<p2d>();
		this.StationContur = StationContur;
		this.X = X;
	}

	public ImmersedStation(Station sta, float TrimAngle, float HeelAngle, p3d wlPosition)
	{
		X = sta.X;
		lstOfXWaterline = new List<float>();
		lstWaterplanePoints = new List<p2d>();
		float num = (float)Math.Tan((double)TrimAngle * Math.PI / 180.0);
		float num2 = (y = num * (sta.X - wlPosition.X) + wlPosition.Z);
		List<Contur> list = new List<Contur>();
		for (int i = 0; i < sta.StationContur.arrayConturs.Length; i++)
		{
			p2d[] array = new p2d[1];
			p2d[] array2 = new p2d[sta.StationContur.arrayConturs[i].arrayPoints.Length];
			Array.Copy(sta.StationContur.arrayConturs[i].arrayPoints, array2, sta.StationContur.arrayConturs[i].arrayPoints.Length);
			List<float> list2 = new List<float>();
			for (int j = 0; j < sta.StationContur.arrayConturs[i].arrayPoints.Length; j++)
			{
				array2[j].X += 0f - wlPosition.Y;
				array2[j].Y += 0f - num2;
				float x = array2[j].X * (float)Math.Cos((0f - HeelAngle) * ((float)Math.PI / 180f)) - array2[j].Y * (float)Math.Sin((0f - HeelAngle) * ((float)Math.PI / 180f));
				float num3 = array2[j].X * (float)Math.Sin((0f - HeelAngle) * ((float)Math.PI / 180f)) + array2[j].Y * (float)Math.Cos((0f - HeelAngle) * ((float)Math.PI / 180f));
				array2[j].X = x;
				array2[j].Y = num3;
				if (j <= 0)
				{
					continue;
				}
				if (array2[j].Y <= 0f && array2[j - 1].Y > 0f)
				{
					p2d[] array3 = new p2d[1];
					array3[0].X = (0f - array2[j - 1].Y) / (array2[j].Y - array2[j - 1].Y);
					array3[0].X *= array2[j].X - array2[j - 1].X;
					array3[0].X += array2[j - 1].X;
					array3[0].Y = 0f;
					list2.Add(array3[0].X);
					array3 = Transform.RotateContur(new p2d(0f, 0f), array3, HeelAngle);
					array3 = Transform.MoveContur(new p2d(0f - wlPosition.Y, 0f - num2), new p2d(0f, 0f), array3);
					lstWaterplanePoints.Add(new p2d(array3[0].X, array3[0].Y));
					ref p2d reference = ref array[array.Length - 1];
					reference = array3[0];
					Array.Resize(ref array, array.Length + 1);
					ref p2d reference2 = ref array[array.Length - 1];
					reference2 = sta.StationContur.arrayConturs[i].arrayPoints[j];
					Array.Resize(ref array, array.Length + 1);
				}
				else if (array2[j].Y > 0f && array2[j - 1].Y <= 0f)
				{
					ref p2d reference3 = ref array[array.Length - 1];
					reference3 = sta.StationContur.arrayConturs[i].arrayPoints[j - 1];
					Array.Resize(ref array, array.Length + 1);
					p2d[] array4 = new p2d[1];
					array4[0].X = (0f - array2[j - 1].Y) / (array2[j].Y - array2[j - 1].Y);
					array4[0].X *= array2[j].X - array2[j - 1].X;
					array4[0].X += array2[j - 1].X;
					array4[0].Y = 0f;
					list2.Add(array4[0].X);
					array4 = Transform.RotateContur(new p2d(0f, 0f), array4, HeelAngle);
					array4 = Transform.MoveContur(new p2d(0f - wlPosition.Y, 0f - num2), new p2d(0f, 0f), array4);
					lstWaterplanePoints.Add(new p2d(array4[0].X, array4[0].Y));
					ref p2d reference4 = ref array[array.Length - 1];
					reference4 = array4[0];
					Array.Resize(ref array, array.Length + 1);
				}
				else if (array2[j].Y <= 0f && array2[j - 1].Y <= 0f)
				{
					if (j == 1)
					{
						ref p2d reference5 = ref array[array.Length - 1];
						reference5 = sta.StationContur.arrayConturs[i].arrayPoints[0];
						Array.Resize(ref array, array.Length + 1);
					}
					ref p2d reference6 = ref array[array.Length - 1];
					reference6 = sta.StationContur.arrayConturs[i].arrayPoints[j];
					Array.Resize(ref array, array.Length + 1);
				}
			}
			Array.Resize(ref array, array.Length - 1);
			list.Add(new Contur(array));
			lstOfXWaterline = new List<float>();
			for (int k = 0; k < list2.Count; k++)
			{
				lstOfXWaterline.Add(list2[k]);
			}
		}
		StationContur = new MultipleContur(list);
	}
}
