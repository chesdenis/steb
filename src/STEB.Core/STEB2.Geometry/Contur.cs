using System;
using System.Collections.Generic;

namespace STEB2.Geometry;

public class Contur
{
	public p2d CenterOfGravity = default;

	public p2d[] arrayPoints = new p2d[0];

	public List<p2d> listPoints = new List<p2d>();

	public float Area;

	public float Ix;

	public float Iy;

	public float Sx;

	public float Sy;

	public float trueArea;

	public Contur(p2d[] inpPnts)
	{
		if (inpPnts.Length != 0)
		{
			p2d[] array = new p2d[inpPnts.Length];
			inpPnts.CopyTo(array, 0);
			if ((array[array.Length - 1].X != array[0].X) | (array[array.Length - 1].Y != array[0].Y))
			{
				Array.Resize(ref array, array.Length + 1);
				ref p2d reference = ref array[array.Length - 1];
				reference = array[0];
			}
			arrayPoints = new p2d[array.Length];
			listPoints = new List<p2d>();
			for (int i = 0; i < array.Length; i++)
			{
				ref p2d reference2 = ref arrayPoints[i];
				reference2 = new p2d(array[i].X, array[i].Y);
				listPoints.Add(new p2d(array[i].X, array[i].Y));
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			for (int j = 1; j < array.Length; j++)
			{
				float num11 = ((float.IsNaN(array[j - 1].X) | float.IsInfinity(array[j - 1].X) | float.IsNegativeInfinity(array[j - 1].X) | float.IsPositiveInfinity(array[j - 1].X)) ? 0f : array[j - 1].X);
				float num12 = ((float.IsNaN(array[j].X) | float.IsInfinity(array[j].X) | float.IsNegativeInfinity(array[j].X) | float.IsPositiveInfinity(array[j].X)) ? 0f : array[j].X);
				float num13 = ((float.IsNaN(array[j - 1].Y) | float.IsInfinity(array[j - 1].Y) | float.IsNegativeInfinity(array[j - 1].Y) | float.IsPositiveInfinity(array[j - 1].Y)) ? 0f : array[j - 1].Y);
				float num14 = ((float.IsNaN(array[j].Y) | float.IsInfinity(array[j].Y) | float.IsNegativeInfinity(array[j].Y) | float.IsPositiveInfinity(array[j].Y)) ? 0f : array[j].Y);
				num = 0.5f * (num12 - num11) * (num14 + num13);
				num2 = 1f / 6f * (num12 - num11) * (num13 * num13 + num13 * num14 + num14 * num14);
				num3 = 1f / 6f * (num12 - num11) * (2f * num14 * num12 + num14 * num11 + num13 * num12 + 2f * num13 * num11);
				num4 = 1f / 12f * (num12 - num11) * (num14 * num14 * num14 + num14 * num14 * num13 + num14 * num13 * num13 + num13 * num13 * num13);
				num5 = 1f / 12f * (num12 - num11) * (num14 * (3f * num12 * num12 + 2f * num12 * num11 + num11 * num11) + num13 * (3f * num11 * num11 + 2f * num11 * num12 + num12 * num12));
				num6 += num;
				num7 += num2;
				num8 += num3;
				num9 += num4;
				num10 += num5;
			}
			Area = Math.Abs(num6);
			trueArea = num6;
			CenterOfGravity = new p2d(num8 / num6, num7 / num6);
			Sx = Math.Abs(num7);
			Sy = Math.Abs(num8);
			Iy = Math.Abs(num9);
			Ix = Math.Abs(num10);
		}
	}

	public Contur(List<p2d> inpPnts)
	{
		if (inpPnts.Count != 0)
		{
			p2d[] array = new p2d[inpPnts.Count];
			inpPnts.CopyTo(array, 0);
			if ((array[array.Length - 1].X != array[0].X) | (array[array.Length - 1].Y != array[0].Y))
			{
				Array.Resize(ref array, array.Length + 1);
				ref p2d reference = ref array[array.Length - 1];
				reference = array[0];
			}
			arrayPoints = new p2d[array.Length];
			listPoints = new List<p2d>();
			for (int i = 0; i < array.Length; i++)
			{
				ref p2d reference2 = ref arrayPoints[i];
				reference2 = new p2d(array[i].X, array[i].Y);
				listPoints.Add(new p2d(array[i].X, array[i].Y));
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			for (int j = 1; j < array.Length; j++)
			{
				float num11 = ((float.IsNaN(array[j - 1].X) | float.IsInfinity(array[j - 1].X) | float.IsNegativeInfinity(array[j - 1].X) | float.IsPositiveInfinity(array[j - 1].X)) ? 0f : array[j - 1].X);
				float num12 = ((float.IsNaN(array[j].X) | float.IsInfinity(array[j].X) | float.IsNegativeInfinity(array[j].X) | float.IsPositiveInfinity(array[j].X)) ? 0f : array[j].X);
				float num13 = ((float.IsNaN(array[j - 1].Y) | float.IsInfinity(array[j - 1].Y) | float.IsNegativeInfinity(array[j - 1].Y) | float.IsPositiveInfinity(array[j - 1].Y)) ? 0f : array[j - 1].Y);
				float num14 = ((float.IsNaN(array[j].Y) | float.IsInfinity(array[j].Y) | float.IsNegativeInfinity(array[j].Y) | float.IsPositiveInfinity(array[j].Y)) ? 0f : array[j].Y);
				num = 0.5f * (num12 - num11) * (num14 + num13);
				num2 = 1f / 6f * (num12 - num11) * (num13 * num13 + num13 * num14 + num14 * num14);
				num3 = 1f / 6f * (num12 - num11) * (2f * num14 * num12 + num14 * num11 + num13 * num12 + 2f * num13 * num11);
				num4 = 1f / 12f * (num12 - num11) * (num14 * num14 * num14 + num14 * num14 * num13 + num14 * num13 * num13 + num13 * num13 * num13);
				num5 = 1f / 12f * (num12 - num11) * (num14 * (3f * num12 * num12 + 2f * num12 * num11 + num11 * num11) + num13 * (3f * num11 * num11 + 2f * num11 * num12 + num12 * num12));
				num6 += num;
				num7 += num2;
				num8 += num3;
				num9 += num4;
				num10 += num5;
			}
			trueArea = num6;
			Area = Math.Abs(num6);
			CenterOfGravity = new p2d(num8 / num6, num7 / num6);
			Sx = Math.Abs(num7);
			Sy = Math.Abs(num8);
			Iy = Math.Abs(num9);
			Ix = Math.Abs(num10);
		}
	}
}
