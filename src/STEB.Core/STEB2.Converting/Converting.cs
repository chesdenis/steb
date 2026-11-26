using System;
using System.Collections.Generic;
using System.Drawing;
using STEB2.Geometry;

namespace STEB2.Converting;

public class Converting
{
	public static float TextToFloat(string strText)
	{
		if (strText.IndexOf('E') > 0)
		{
			return 0f;
		}
		strText = strText.Trim();
		if (strText.Trim() == "null")
		{
			return float.NaN;
		}
		try
		{
			if (strText == "")
			{
				return 0f;
			}
			float num = 0f;
			float num2 = 1f;
			long num3 = 0L;
			for (int num4 = strText.Length - 1; num4 > -1; num4--)
			{
				switch (strText.Substring(num4, 1))
				{
				case ".":
				case ",":
					num /= num2;
					num2 = 0.1f;
					break;
				case "-":
					num *= -1f;
					break;
				default:
					num3 = Convert.ToInt64(strText.Substring(num4, 1));
					num += (float)num3 * num2;
					break;
				case " ":
					break;
				}
				num2 *= 10f;
			}
			return num;
		}
		catch
		{
			return 0f;
		}
	}

	public static List<p2d> ArrayToList(p2d[] Array)
	{
		List<p2d> list = new List<p2d>();
		for (int i = 0; i < Array.Length; i++)
		{
			list.Add(new p2d(Array[i].X, Array[i].Y));
		}
		return list;
	}

	public static p2d[] ListToArray(List<p2d> List)
	{
		p2d[] array = new p2d[List.Count];
		for (int i = 0; i < List.Count; i++)
		{
			ref p2d reference = ref array[i];
			reference = new p2d(List[i].X, List[i].Y);
		}
		return array;
	}

	public static PointF[] P2dToPointF(p2d[] List)
	{
		PointF[] array = new PointF[List.Length];
		for (int i = 0; i < List.Length; i++)
		{
			ref PointF reference = ref array[i];
			reference = new PointF(List[i].X, List[i].Y);
		}
		return array;
	}
}
