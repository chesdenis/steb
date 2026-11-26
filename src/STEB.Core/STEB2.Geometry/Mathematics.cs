using System;
using System.Collections.Generic;

namespace STEB2.Geometry;

public class Mathematics
{
	public static float GetStandartFloat(float inputValue)
	{
		if (inputValue == 0f)
		{
			return inputValue;
		}
		if (float.IsInfinity(inputValue) | float.IsNaN(inputValue))
		{
			return inputValue;
		}
		bool flag = inputValue < 0f;
		inputValue = Math.Abs(inputValue);
		float num = (float)Math.Truncate(inputValue);
		int num2 = 0;
		if (num > 10f)
		{
			while (num > 10f)
			{
				inputValue /= 10f;
				num = (float)Math.Truncate(inputValue);
				num2++;
				if (num == 0f)
				{
					break;
				}
			}
		}
		else if (num < 1f)
		{
			while (num < 1f)
			{
				inputValue *= 10f;
				num = (float)Math.Truncate(inputValue);
				num2--;
				if (num == 0f)
				{
					break;
				}
			}
		}
		num = (int)num switch
		{
			1 => 1f, 
			2 => 2f, 
			3 => 2.5f, 
			4 => 2.5f, 
			5 => 5f, 
			6 => 5f, 
			7 => 5f, 
			8 => 10f, 
			9 => 10f, 
			_ => 5f, 
		};
		if (!flag)
		{
			return (float)((double)num * Math.Pow(10.0, num2));
		}
		return 0f - (float)((double)num * Math.Pow(10.0, num2));
	}

	public static float GetVectorLength(p2d[] twoPoints)
	{
		twoPoints = Transform.MoveContur(twoPoints[0], new p2d(0f, 0f), twoPoints);
		return (float)Math.Sqrt(twoPoints[1].X * twoPoints[1].X + twoPoints[1].Y * twoPoints[1].Y);
	}

	public static float GetVectorAngle(p2d[] twoPoints)
	{
		twoPoints = Transform.MoveContur(twoPoints[0], new p2d(0f, 0f), twoPoints);
		float x = twoPoints[1].X;
		float y = twoPoints[1].Y;
		if (x == 0f && y == 0f)
		{
			return float.NaN;
		}
		if (x == 0f)
		{
			return (y > 0f) ? 90 : 270;
		}
		float num = (float)Math.Atan(y / x);
		num = num * 180f / (float)Math.PI;
		if (x > 0f)
		{
			if (!(y >= 0f))
			{
				return 360f + num;
			}
			return num;
		}
		return 180f + num;
	}

	public static float Get3dVectorLength(p3d vectorPoint)
	{
		return (float)Math.Sqrt(vectorPoint.X * vectorPoint.X + vectorPoint.Y * vectorPoint.Y + vectorPoint.Z * vectorPoint.Z);
	}

	public static float Get3dVectorLength(p3d StartPoint, p3d EndPoint)
	{
		return (float)Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2.0) + Math.Pow(EndPoint.Y - StartPoint.Y, 2.0) + Math.Pow(EndPoint.Z - StartPoint.Z, 2.0));
	}

	public static p2d[] GetBoundRect(p2d[] InputPoints)
	{
		List<p2d> list = new List<p2d>();
		for (int i = 0; i < InputPoints.Length; i++)
		{
			if (!(float.IsInfinity(InputPoints[i].X) | float.IsInfinity(InputPoints[i].Y) | float.IsNaN(InputPoints[i].X) | float.IsNaN(InputPoints[i].Y)))
			{
				list.Add(new p2d(InputPoints[i].X, InputPoints[i].Y));
			}
		}
		p2d[] array = new p2d[5];
		try
		{
			float num2;
			float num = (num2 = list[0].X);
			float num4;
			float num3 = (num4 = list[0].Y);
			for (int j = 0; j < list.Count; j++)
			{
				num = ((num > list[j].X) ? list[j].X : num);
				num2 = ((num2 < list[j].X) ? list[j].X : num2);
				num3 = ((num3 > list[j].Y) ? list[j].Y : num3);
				num4 = ((num4 < list[j].Y) ? list[j].Y : num4);
			}
			array[0].X = num;
			array[0].Y = num3;
			array[1].X = num;
			array[1].Y = num4;
			array[2].X = num2;
			array[2].Y = num4;
			array[3].X = num2;
			array[3].Y = num3;
			array[4].X = num;
			array[4].Y = num3;
		}
		catch
		{
		}
		return array;
	}

	public static p2d[] GetBoundRect(List<p2d> InputPoints)
	{
		List<p2d> list = new List<p2d>();
		for (int i = 0; i < InputPoints.Count; i++)
		{
			if (!(float.IsInfinity(InputPoints[i].X) | float.IsInfinity(InputPoints[i].Y) | float.IsNaN(InputPoints[i].X) | float.IsNaN(InputPoints[i].Y)))
			{
				list.Add(new p2d(InputPoints[i].X, InputPoints[i].Y));
			}
		}
		p2d[] array = new p2d[5];
		try
		{
			float num2;
			float num = (num2 = list[0].X);
			float num4;
			float num3 = (num4 = list[0].Y);
			for (int j = 0; j < list.Count; j++)
			{
				num = ((num > list[j].X) ? list[j].X : num);
				num2 = ((num2 < list[j].X) ? list[j].X : num2);
				num3 = ((num3 > list[j].Y) ? list[j].Y : num3);
				num4 = ((num4 < list[j].Y) ? list[j].Y : num4);
			}
			array[0].X = num;
			array[0].Y = num3;
			array[1].X = num;
			array[1].Y = num4;
			array[2].X = num2;
			array[2].Y = num4;
			array[3].X = num2;
			array[3].Y = num3;
			array[4].X = num;
			array[4].Y = num3;
		}
		catch
		{
		}
		return array;
	}

	public static p2d LineInterpolation(p2d first, p2d second)
	{
		float x = first.X;
		float x2 = second.X;
		float y = first.Y;
		float y2 = second.Y;
		float x3 = (x * y2 - y * x2) / (y2 - y);
		return new p2d(x3, 0f);
	}

	public static float GetMaximum(List<float> input)
	{
		if (input.Count == 0)
		{
			return 0f;
		}
		float num = Math.Abs(input[0]);
		int index = 0;
		for (int i = 0; i < input.Count; i++)
		{
			if (num < Math.Abs(input[i]))
			{
				num = Math.Abs(input[i]);
				index = i;
			}
		}
		return input[index];
	}

	public static float GetMinimum(List<float> input)
	{
		if (input.Count == 0)
		{
			return 0f;
		}
		float num = Math.Abs(input[0]);
		int index = 0;
		for (int i = 0; i < input.Count; i++)
		{
			if (num > Math.Abs(input[i]))
			{
				num = Math.Abs(input[i]);
				index = i;
			}
		}
		return input[index];
	}

	public static float GetMinimum(List<p2d> input)
	{
		if (input.Count == 0)
		{
			return 0f;
		}
		float num = Math.Abs(input[0].X);
		int index = 0;
		for (int i = 0; i < input.Count; i++)
		{
			if (num > Math.Abs(input[i].X))
			{
				num = Math.Abs(input[i].X);
				index = i;
			}
		}
		return input[index].X;
	}
}
