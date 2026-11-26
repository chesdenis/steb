using System;

namespace STEB2.Geometry;

public class Transform
{
	public static p3d OffsetPoint(float heel_angle, float trim_angle, float distance)
	{
		p2d[] array = new p2d[2]
		{
			new p2d(0f, 0f),
			new p2d(1f, 0f)
		};
		p2d[] array2 = new p2d[2]
		{
			new p2d(0f, 0f),
			new p2d(1f, 0f)
		};
		array = RotateContur(array[0], array, heel_angle);
		array2 = RotateContur(array2[0], array2, trim_angle);
		p3d[] array3 = new p3d[3]
		{
			new p3d(0f, 0f, 0f),
			new p3d(0f, array[1].X, array[1].Y),
			new p3d(array2[1].X, 0f, array2[1].Y)
		};
		p3d p3d2 = default;
		p3d p3d3 = array3[0];
		p3d p3d4 = array3[1];
		p3d p3d5 = array3[2];
		p3d2.X = (p3d5.Y - p3d3.Y) * (p3d4.Y - p3d3.Z) - (p3d5.Z - p3d3.Z) * (p3d4.Y - p3d3.Y);
		p3d2.Y = (p3d4.X - p3d3.X) * (p3d5.Z - p3d3.Z) - (p3d5.X - p3d3.X) * (p3d4.Z - p3d3.Z);
		p3d2.Z = (p3d5.X - p3d3.X) * (p3d4.Y - p3d3.Y) - (p3d4.X - p3d3.X) * (p3d5.Y - p3d3.Y);
		float num = (float)Math.Sqrt(p3d2.X * p3d2.X + p3d2.Y * p3d2.Y + p3d2.Z * p3d2.Z);
		float num2 = p3d2.X / num;
		float num3 = p3d2.Y / num;
		float num4 = p3d2.Z / num;
		return new p3d(distance * num2, distance * num3, distance * num4);
	}

	public static p3d OffsetPoint(float heel_angle, float trim_angle, float distance, p3d startPoint)
	{
		p2d[] array = new p2d[2]
		{
			new p2d(0f, 0f),
			new p2d(1f, 0f)
		};
		p2d[] array2 = new p2d[2]
		{
			new p2d(0f, 0f),
			new p2d(1f, 0f)
		};
		array = RotateContur(array[0], array, heel_angle);
		array2 = RotateContur(array2[0], array2, trim_angle);
		p3d[] array3 = new p3d[3]
		{
			new p3d(0f, 0f, 0f),
			new p3d(0f, array[1].X, array[1].Y),
			new p3d(array2[1].X, 0f, array2[1].Y)
		};
		p3d p3d2 = default;
		p3d p3d3 = array3[0];
		p3d p3d4 = array3[1];
		p3d p3d5 = array3[2];
		p3d2.X = (p3d5.Y - p3d3.Y) * (p3d4.Y - p3d3.Z) - (p3d5.Z - p3d3.Z) * (p3d4.Y - p3d3.Y);
		p3d2.Y = (p3d4.X - p3d3.X) * (p3d5.Z - p3d3.Z) - (p3d5.X - p3d3.X) * (p3d4.Z - p3d3.Z);
		p3d2.Z = (p3d5.X - p3d3.X) * (p3d4.Y - p3d3.Y) - (p3d4.X - p3d3.X) * (p3d5.Y - p3d3.Y);
		float num = (float)Math.Sqrt(p3d2.X * p3d2.X + p3d2.Y * p3d2.Y + p3d2.Z * p3d2.Z);
		float num2 = p3d2.X / num;
		float num3 = p3d2.Y / num;
		float num4 = p3d2.Z / num;
		p3d p3d6 = new p3d(distance * num2, distance * num3, distance * num4);
		return MoveContur(new p3d(0f, 0f, 0f), startPoint, new p3d[1] { p3d6 })[0];
	}

	public static p2d[] RotateContur(p2d bp, p2d[] InputPnts, float angle)
	{
		angle *= (float)Math.PI / 180f;
		p2d[] array = new p2d[InputPnts.Length];
		for (int i = 0; i < InputPnts.Length; i++)
		{
			array[i].X = (InputPnts[i].X - bp.X) * (float)Math.Cos(angle) - (InputPnts[i].Y - bp.Y) * (float)Math.Sin(angle) + bp.X;
			array[i].Y = (InputPnts[i].X - bp.X) * (float)Math.Sin(angle) + (InputPnts[i].Y - bp.Y) * (float)Math.Cos(angle) + bp.Y;
		}
		return array;
	}

	public static p2d[] MoveContur(p2d sp, p2d ep, p2d[] InputPnts)
	{
		float num = ep.X - sp.X;
		float num2 = ep.Y - sp.Y;
		p2d[] array = new p2d[InputPnts.Length];
		for (int i = 0; i <= InputPnts.Length - 1; i++)
		{
			array[i].X = InputPnts[i].X + num;
			array[i].Y = InputPnts[i].Y + num2;
		}
		return array;
	}

	public static p3d[] MoveContur(p3d sp, p3d ep, p3d[] InputPnts)
	{
		float num = ep.X - sp.X;
		float num2 = ep.Y - sp.Y;
		float num3 = ep.Z - sp.Z;
		p3d[] array = new p3d[InputPnts.Length];
		for (int i = 0; i <= InputPnts.Length - 1; i++)
		{
			array[i].X = InputPnts[i].X + num;
			array[i].Y = InputPnts[i].Y + num2;
			array[i].Z = InputPnts[i].Z + num3;
		}
		return array;
	}
}
