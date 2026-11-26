using System.Collections.Generic;
using STEB2.Geometry;

namespace STEB2.Stability;

public class StabilityCurve
{
	public float DSPL;

	public string Name;

	public List<p2d> NullPoints;

	public List<p2d> MaxPoints;

	public List<p2d> MinPoints;

	public List<p2d> Points;

	public StabilityCurve(float DSPL, string Name, List<p2d> Points)
	{
		this.DSPL = DSPL;
		this.Name = Name;
		this.Points = new List<p2d>();
		for (int i = 0; i < Points.Count; i++)
		{
			this.Points.Add(new p2d(Points[i].X, Points[i].Y));
		}
		NullPoints = new List<p2d>();
		MaxPoints = new List<p2d>();
		MinPoints = new List<p2d>();
		if (this.Points.Count <= 2)
		{
			return;
		}
		for (int j = 1; j < this.Points.Count - 1; j++)
		{
			if (this.Points[j].Y > this.Points[j + 1].Y && this.Points[j].Y > this.Points[j - 1].Y)
			{
				MaxPoints.Add(new p2d(this.Points[j].X, this.Points[j].Y));
			}
			if (this.Points[j].Y < this.Points[j + 1].Y && this.Points[j].Y < this.Points[j - 1].Y)
			{
				MinPoints.Add(new p2d(this.Points[j].X, this.Points[j].Y));
			}
			if (this.Points[j - 1].Y <= 0f && this.Points[j].Y >= 0f)
			{
				NullPoints.Add(Mathematics.LineInterpolation(this.Points[j - 1], this.Points[j]));
			}
			if (this.Points[j - 1].Y >= 0f && this.Points[j].Y <= 0f)
			{
				NullPoints.Add(Mathematics.LineInterpolation(this.Points[j - 1], this.Points[j]));
			}
		}
	}

	public float ReturnTrueAngleOnWater()
	{
		List<float> list = new List<float>();
		for (int i = 0; i < NullPoints.Count; i++)
		{
			for (int j = 0; j < Points.Count; j++)
			{
				if (Points[j].X > NullPoints[i].X)
				{
					if (Points[j].Y > NullPoints[i].Y)
					{
						list.Add(NullPoints[i].X);
					}
					break;
				}
			}
		}
		return Mathematics.GetMinimum(list);
	}

	public List<float> ReturnDeadriseAnglesOnWater()
	{
		List<float> list = new List<float>();
		for (int i = 0; i < NullPoints.Count; i++)
		{
			for (int j = 0; j < Points.Count; j++)
			{
				if (Points[j].X > NullPoints[i].X)
				{
					if (Points[j].Y < NullPoints[i].Y)
					{
						list.Add(NullPoints[i].X);
					}
					break;
				}
			}
		}
		return list;
	}
}
