using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace STEB2.Geometry;

public class Curve
{
	public List<p2d> listPoints = new List<p2d>();

	public p2d[] arrayPoints = new p2d[0];

	public Color curveColor = Color.Black;

	public float curveWidth = 0.1f;

	public DashStyle curveStyle;

	public Curve(List<p2d> inpPnts)
	{
		listPoints = new List<p2d>();
		arrayPoints = new p2d[inpPnts.Count];
		for (int i = 0; i < inpPnts.Count; i++)
		{
			listPoints.Add(new p2d(inpPnts[i].X, inpPnts[i].Y));
			ref p2d reference = ref arrayPoints[i];
			reference = new p2d(inpPnts[i].X, inpPnts[i].Y);
		}
	}

	public Curve(p2d[] inpPnts)
	{
		listPoints = new List<p2d>();
		arrayPoints = new p2d[inpPnts.Length];
		for (int i = 0; i < inpPnts.Length; i++)
		{
			listPoints.Add(new p2d(inpPnts[i].X, inpPnts[i].Y));
			ref p2d reference = ref arrayPoints[i];
			reference = new p2d(inpPnts[i].X, inpPnts[i].Y);
		}
	}
}
