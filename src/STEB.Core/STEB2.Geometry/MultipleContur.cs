using System;
using System.Collections.Generic;

namespace STEB2.Geometry;

public class MultipleContur
{
	public Contur[] arrayConturs = new Contur[0];

	public List<Contur> listConturs = new List<Contur>();

	public p2d CenterOfGravity = default;

	public float Area;

	public MultipleContur(Contur[] inpConturs)
	{
		if (inpConturs.Length != 0)
		{
			arrayConturs = new Contur[inpConturs.Length];
			listConturs = new List<Contur>();
			for (int i = 0; i < inpConturs.Length; i++)
			{
				arrayConturs[i] = new Contur(inpConturs[i].arrayPoints);
				listConturs.Add(new Contur(inpConturs[i].arrayPoints));
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			for (int j = 0; j < inpConturs.Length; j++)
			{
				inpConturs[j] = new Contur(inpConturs[j].arrayPoints);
				num += inpConturs[j].Area;
				num2 += inpConturs[j].Area * inpConturs[j].CenterOfGravity.X;
				num3 += inpConturs[j].Area * inpConturs[j].CenterOfGravity.Y;
			}
			Area = num;
			CenterOfGravity = new p2d(num2 / num, num3 / num);
		}
	}

	public MultipleContur(List<Contur> inpConturs)
	{
		if (inpConturs.Count != 0)
		{
			arrayConturs = new Contur[inpConturs.Count];
			listConturs = new List<Contur>();
			for (int i = 0; i < inpConturs.Count; i++)
			{
				arrayConturs[i] = new Contur(inpConturs[i].arrayPoints);
				listConturs.Add(new Contur(inpConturs[i].arrayPoints));
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			for (int j = 0; j < inpConturs.Count; j++)
			{
				num += inpConturs[j].Area;
				num2 += inpConturs[j].Area * inpConturs[j].CenterOfGravity.X;
				num3 += inpConturs[j].Area * inpConturs[j].CenterOfGravity.Y;
			}
			Area = num;
			CenterOfGravity = new p2d(num2 / num, num3 / num);
		}
	}

	public void AddContur(Contur cntr)
	{
		Array.Resize(ref arrayConturs, arrayConturs.Length + 1);
		arrayConturs[arrayConturs.Length - 1] = cntr;
		listConturs.Add(cntr);
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < listConturs.Count; i++)
		{
			num += listConturs[i].Area;
			num2 += listConturs[i].Area * listConturs[i].CenterOfGravity.X;
			num3 += listConturs[i].Area * listConturs[i].CenterOfGravity.Y;
		}
		Area = num;
		CenterOfGravity = new p2d(num3 / num, num2 / num);
	}
}
