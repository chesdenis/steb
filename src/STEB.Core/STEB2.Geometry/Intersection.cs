using System;
using System.Collections.Generic;

namespace STEB2.Geometry;

public class Intersection
{
	public enum ClipOrientation
	{
		Up,
		Down,
		Left,
		Right
	}

	public static List<p2d> GetClippedRegion(List<p2d> srcRegion, p2d clipPoint, ClipOrientation orientation)
	{
		List<p2d> list = new List<p2d>();
		List<p2d> list2 = new List<p2d>();
		for (int i = 0; i < srcRegion.Count; i++)
		{
			p2d p2d2 = new p2d(float.NaN, float.NaN);
			try
			{
				switch (orientation)
				{
				case ClipOrientation.Up:
					p2d2 = GetXPoint(srcRegion[i - 1], srcRegion[i], clipPoint, new p2d(clipPoint.X + 1f, clipPoint.Y), isExtendFirst: false, isExtendSecond: true);
					break;
				case ClipOrientation.Down:
					p2d2 = GetXPoint(srcRegion[i - 1], srcRegion[i], clipPoint, new p2d(clipPoint.X + 1f, clipPoint.Y), isExtendFirst: false, isExtendSecond: true);
					break;
				case ClipOrientation.Left:
					p2d2 = GetXPoint(srcRegion[i - 1], srcRegion[i], clipPoint, new p2d(clipPoint.X, clipPoint.Y + 1f), isExtendFirst: false, isExtendSecond: true);
					break;
				case ClipOrientation.Right:
					p2d2 = GetXPoint(srcRegion[i - 1], srcRegion[i], clipPoint, new p2d(clipPoint.X, clipPoint.Y + 1f), isExtendFirst: false, isExtendSecond: true);
					break;
				}
			}
			catch
			{
			}
			if (float.IsInfinity(p2d2.X) | float.IsInfinity(p2d2.Y) | float.IsNaN(p2d2.X) | float.IsNaN(p2d2.Y))
			{
				try
				{
					list2.Add(new p2d(srcRegion[i - 1].X, srcRegion[i - 1].Y));
				}
				catch
				{
				}
			}
			else
			{
				list2.Add(new p2d(srcRegion[i - 1].X, srcRegion[i - 1].Y));
				list2.Add(new p2d(p2d2.X, p2d2.Y));
			}
			if (i == srcRegion.Count - 1)
			{
				list2.Add(new p2d(srcRegion[i].X, srcRegion[i].Y));
			}
		}
		foreach (p2d item in list2)
		{
			switch (orientation)
			{
			case ClipOrientation.Up:
				if (Math.Round(item.Y, 4) >= Math.Round(clipPoint.Y, 4))
				{
					list.Add(item);
				}
				break;
			case ClipOrientation.Down:
				if (Math.Round(item.Y, 4) <= Math.Round(clipPoint.Y, 4))
				{
					list.Add(item);
				}
				break;
			case ClipOrientation.Left:
				if (Math.Round(item.X, 4) >= Math.Round(clipPoint.X, 4))
				{
					list.Add(item);
				}
				break;
			case ClipOrientation.Right:
				if (Math.Round(item.X, 4) <= Math.Round(clipPoint.X, 4))
				{
					list.Add(item);
				}
				break;
			}
		}
		try
		{
			list.Add(new p2d(list[0].X, list[0].Y));
		}
		catch
		{
		}
		return list;
	}

	public static bool IsPointOnLine(p2d pntStart, p2d pntEnd, p2d verifyPoint)
	{
		try
		{
			p2d[] array = new p2d[3] { pntStart, pntEnd, verifyPoint };
			float vectorAngle = Mathematics.GetVectorAngle(array);
			array = Transform.RotateContur(new p2d(0f, 0f), array, 0f - vectorAngle);
			if (Math.Round(array[2].X, 4) > Math.Round(array[1].X, 4))
			{
				return false;
			}
			if (Math.Round(array[2].X, 4) < Math.Round(array[0].X, 4))
			{
				return false;
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static p2d GetIpxIpy(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = (x1 - x2) * (y3 - y4) - (x3 - x4) * (y1 - y2);
		num = (x2 * (y1 - y3) - x1 * (y2 - y3)) * x4 + x3 * (x1 * (y2 - y4) - x2 * (y1 - y4));
		num /= num3;
		num2 = (y1 * (x2 - x3) - y2 * (x1 - x3)) * y4 - y3 * (y1 * (x2 - x4) - y2 * (x1 - x4));
		num2 /= num3;
		return new p2d(num, num2);
	}

	public static List<p2d> GetXPointConturLine(p2d LineStartPoint, p2d LineEndPoint, bool isExtendLine, Contur contur)
	{
		List<p2d> list = new List<p2d>();
		for (int i = 1; i < contur.arrayPoints.Length; i++)
		{
			p2d xPoint = GetXPoint(LineStartPoint, LineEndPoint, contur.arrayPoints[i - 1], contur.arrayPoints[i], isExtendLine, isExtendSecond: false);
			if (!float.IsNaN(xPoint.X) && !float.IsNaN(xPoint.Y))
			{
				list.Add(xPoint);
			}
		}
		return list;
	}

	public static p2d GetXPoint(p2d firstLineStartPoint, p2d firstLineEndPoint, p2d secondLineStartPoint, p2d secondLineEndPoint, bool isExtendFirst, bool isExtendSecond)
	{
		try
		{
			float x = firstLineStartPoint.X;
			float y = firstLineStartPoint.Y;
			float x2 = firstLineEndPoint.X;
			float y2 = firstLineEndPoint.Y;
			float x3 = secondLineStartPoint.X;
			float y3 = secondLineStartPoint.Y;
			float x4 = secondLineEndPoint.X;
			float y4 = secondLineEndPoint.Y;
			p2d ipxIpy = GetIpxIpy(x, y, x2, y2, x3, y3, x4, y4);
			if (isExtendFirst && !isExtendSecond && IsPointOnLine(secondLineStartPoint, secondLineEndPoint, ipxIpy))
			{
				return ipxIpy;
			}
			if (!isExtendFirst && isExtendSecond && IsPointOnLine(firstLineStartPoint, firstLineEndPoint, ipxIpy))
			{
				return ipxIpy;
			}
			if (!isExtendFirst && !isExtendSecond && IsPointOnLine(secondLineStartPoint, secondLineEndPoint, ipxIpy) && IsPointOnLine(firstLineStartPoint, firstLineEndPoint, ipxIpy))
			{
				return ipxIpy;
			}
			if (isExtendFirst && isExtendSecond)
			{
				return ipxIpy;
			}
			return new p2d(float.NaN, float.NaN);
		}
		catch
		{
			return new p2d(float.NaN, float.NaN);
		}
	}

	public static p2d GetXPoint(p2d firstLineBasePoint, double firstLineAngle, p2d secondLineBasePoint, double secondLineAngle)
	{
		try
		{
			p2d result = default;
			if (firstLineAngle == secondLineAngle)
			{
				return new p2d(float.NaN, float.NaN);
			}
			if (firstLineAngle % 90.0 == 0.0 && firstLineAngle != 0.0)
			{
				firstLineAngle -= 1E-08;
			}
			if (secondLineAngle % 90.0 == 0.0 && secondLineAngle != 0.0)
			{
				secondLineAngle -= 1E-08;
			}
			firstLineAngle *= Math.PI / 180.0;
			secondLineAngle *= Math.PI / 180.0;
			result.X = (float)(0.0 - ((double)(0f - firstLineBasePoint.Y) + Math.Tan(firstLineAngle) * (double)firstLineBasePoint.X + (double)secondLineBasePoint.Y - Math.Tan(secondLineAngle) * (double)secondLineBasePoint.X));
			result.X /= (float)(0.0 - Math.Tan(firstLineAngle) + Math.Tan(secondLineAngle));
			result.Y = (float)(0.0 - ((double)(0f - firstLineBasePoint.Y) * Math.Tan(secondLineAngle) + Math.Tan(firstLineAngle) * (double)secondLineBasePoint.Y - Math.Tan(firstLineAngle) * Math.Tan(secondLineAngle) * (double)secondLineBasePoint.X + Math.Tan(firstLineAngle) * (double)firstLineBasePoint.X * Math.Tan(secondLineAngle)));
			result.Y /= (float)(0.0 - Math.Tan(firstLineAngle) + Math.Tan(secondLineAngle));
			return result;
		}
		catch
		{
			return new p2d(float.NaN, float.NaN);
		}
	}
}
