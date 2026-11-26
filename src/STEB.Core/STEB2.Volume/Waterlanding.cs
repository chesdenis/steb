using System;
using System.Collections.Generic;
using STEB2.Geometry;
using STEB2.Stability;
using STEB2.Stations;

namespace STEB2.Volume;

public class Waterlanding
{
	public float aquaDensity = 1f;

	public List<Station> stations = new List<Station>();

	public List<p2d> diagrammLongPoints = new List<p2d>();

	public List<p2d> diagrammTransvPoints = new List<p2d>();

	public List<p2d> diagrammDLongPoints = new List<p2d>();

	public List<p2d> diagrammDTransvPoints = new List<p2d>();

	public p2d[] RightningLongArm;

	public p2d[] RightningTransArm;

	public p2d[] RightningLongArmForm;

	public p2d[] RightningTransArmForm;

	public float lfLongitudinal;

	public float lfTransverse;

	public float lLongitudinal;

	public float lTransverse;

	public float DSPL;

	public p3d GPos;

	public p3d MPos;

	public p3d wlPosition;

	public float TrimAngle;

	public float HeelAngle;

	public ImmersedStation[] immersedStations;

	public float xf;

	public float yf;

	public p3d CPos;

	public float r;

	public float R;

	public float mTransverseHeight;

	public float mLongHeight;

	public float S;

	public p2d[] Sw;

	public float V;

	public float beta;

	public float alpha;

	public float delta;

	public float Lwl;

	public float Bwl;

	public float d_n;

	public float d_k;

	public float d_lb;

	public float d_pb;

	public float Ix;

	public float Iy;

	public float Sx;

	public float Sy;

	public List<p3d> ipWithWaterplane;

	public void GetWLGabarite_DNDKDLBDPB()
	{
		try
		{
			float x = ipWithWaterplane[0].X;
			float x2 = ipWithWaterplane[0].X;
			int index = 0;
			int index2 = 0;
			float y = ipWithWaterplane[0].Y;
			float y2 = ipWithWaterplane[0].Y;
			int index3 = 0;
			int index4 = 0;
			d_n = ipWithWaterplane[index].Z;
			d_k = ipWithWaterplane[index2].Z;
			d_pb = ipWithWaterplane[index3].Z;
			d_lb = ipWithWaterplane[index4].Z;
			for (int i = 0; i < ipWithWaterplane.Count; i++)
			{
				if (x < ipWithWaterplane[i].X)
				{
					x = ipWithWaterplane[i].X;
					index = i;
					d_n = ipWithWaterplane[index].Z;
				}
				if (x2 > ipWithWaterplane[i].X)
				{
					x2 = ipWithWaterplane[i].X;
					index2 = i;
					d_k = ipWithWaterplane[index2].Z;
				}
				if (y < ipWithWaterplane[i].Y)
				{
					y = ipWithWaterplane[i].Y;
					index3 = i;
					d_pb = ipWithWaterplane[index3].Z;
				}
				if (y2 > ipWithWaterplane[i].Y)
				{
					y2 = ipWithWaterplane[i].Y;
					index4 = i;
					d_lb = ipWithWaterplane[index4].Z;
				}
			}
			Lwl = (float)Math.Sqrt(Math.Pow(ipWithWaterplane[index].X - ipWithWaterplane[index2].X, 2.0) + Math.Pow(ipWithWaterplane[index].Z - ipWithWaterplane[index2].Z, 2.0));
			Bwl = (float)Math.Sqrt(Math.Pow(ipWithWaterplane[index3].Y - ipWithWaterplane[index4].Y, 2.0) + Math.Pow(ipWithWaterplane[index3].Z - ipWithWaterplane[index4].Z, 2.0));
		}
		catch
		{
		}
	}

	public void CalcWaterlineGeometricCharacteristic()
	{
		try
		{
			p2d[] array = new p2d[1];
			for (int i = 0; i < immersedStations.Length; i++)
			{
				float num = 0f;
				if (immersedStations[i].lstOfXWaterline.Count != 0)
				{
					num = immersedStations[i].lstOfXWaterline[0];
					for (int j = 0; j < immersedStations[i].lstOfXWaterline.Count; j++)
					{
						num = ((num > immersedStations[i].lstOfXWaterline[j]) ? immersedStations[i].lstOfXWaterline[j] : num);
					}
					array[array.Length - 1].X = immersedStations[i].X;
					array[array.Length - 1].Y = num;
					Array.Resize(ref array, array.Length + 1);
				}
			}
			for (int num2 = immersedStations.Length - 1; num2 >= 0; num2--)
			{
				float num3 = 0f;
				if (immersedStations[num2].lstOfXWaterline.Count != 0)
				{
					num3 = immersedStations[num2].lstOfXWaterline[0];
					for (int k = 0; k < immersedStations[num2].lstOfXWaterline.Count; k++)
					{
						num3 = ((num3 < immersedStations[num2].lstOfXWaterline[k]) ? immersedStations[num2].lstOfXWaterline[k] : num3);
					}
					array[array.Length - 1].X = immersedStations[num2].X;
					array[array.Length - 1].Y = num3;
					Array.Resize(ref array, array.Length + 1);
				}
			}
			Array.Resize(ref array, array.Length - 1);
			Contur contur = new Contur(array);
			S = Math.Abs(contur.Area);
			Sx = contur.Sx;
			Sy = contur.Sy;
			xf = contur.CenterOfGravity.X;
			yf = contur.CenterOfGravity.Y;
			array = Transform.MoveContur(new p2d(xf, yf), new p2d(0f, 0f), array);
			new Contur(array);
			GetMetacentricH(stations, 0.001f, aquaDensity, CPos);
			r = mTransverseHeight;
			R = mLongHeight;
			Ix = r * V;
			Iy = R * V;
			GetMetacentricH(stations, 0.001f, aquaDensity, GPos);
			MPos = new p3d(GPos.X, GPos.Y, GPos.Z + mTransverseHeight);
			p2d[] inputPnts = new p2d[3]
			{
				new p2d(CPos.Y, CPos.Z),
				new p2d(GPos.Y, GPos.Z),
				new p2d(GPos.Y, GPos.Z)
			};
			inputPnts = Transform.RotateContur(new p2d(GPos.Y, GPos.Z), inputPnts, 0f - HeelAngle);
			ref p2d reference = ref inputPnts[2];
			reference = new p2d(inputPnts[1].X, inputPnts[1].Y + mTransverseHeight);
			inputPnts = Transform.RotateContur(inputPnts[1], inputPnts, HeelAngle);
			MPos = new p3d(CPos.X, inputPnts[2].X, inputPnts[2].Y);
		}
		catch
		{
		}
	}

	public void CalcCoefficient()
	{
		alpha = S / Math.Abs(Lwl * Bwl);
		if (Math.Abs(alpha) > 1f)
		{
			alpha = 0.01f;
		}
		delta = V / Math.Abs(Lwl * Bwl * d_lb);
		if (Math.Abs(delta) > 1f)
		{
			delta = 0.01f;
		}
	}

	public void CalcStabilityArms(p3d GPosition)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		p2d xPoint = Intersection.GetXPoint(new p2d(CPos.Y, CPos.Z), HeelAngle - 90f, new p2d(wlPosition.Y, wlPosition.Z), HeelAngle);
		p2d xPoint2 = Intersection.GetXPoint(new p2d(GPosition.Y, GPosition.Z), HeelAngle - 90f, new p2d(wlPosition.Y, wlPosition.Z), HeelAngle);
		p2d xPoint3 = Intersection.GetXPoint(new p2d(0f, 0f), HeelAngle - 90f, new p2d(wlPosition.Y, wlPosition.Z), HeelAngle);
		RightningTransArm = new p2d[4]
		{
			new p2d(CPos.Y, CPos.Z),
			xPoint,
			xPoint2,
			new p2d(GPosition.Y, GPosition.Z)
		};
		RightningTransArmForm = new p2d[4]
		{
			new p2d(CPos.Y, CPos.Z),
			xPoint,
			xPoint3,
			new p2d(0f, 0f)
		};
		num = (float)Math.Sqrt(Math.Pow(xPoint.X - xPoint2.X, 2.0) + Math.Pow(xPoint.Y - xPoint2.Y, 2.0));
		num3 = (float)Math.Sqrt(Math.Pow(xPoint.X - xPoint3.X, 2.0) + Math.Pow(xPoint.Y - xPoint3.Y, 2.0));
		p2d[] inputPnts = Transform.MoveContur(xPoint2, new p2d(0f, 0f), new p2d[2] { xPoint2, xPoint });
		inputPnts = Transform.RotateContur(new p2d(0f, 0f), inputPnts, 0f - HeelAngle);
		if (inputPnts[1].X < 0f)
		{
			lTransverse = 0f - num;
		}
		else
		{
			lTransverse = num;
		}
		inputPnts = Transform.MoveContur(xPoint3, new p2d(0f, 0f), new p2d[2] { xPoint3, xPoint });
		inputPnts = Transform.RotateContur(new p2d(0f, 0f), inputPnts, 0f - HeelAngle);
		if (inputPnts[1].X < 0f)
		{
			lfTransverse = 0f - num3;
		}
		else
		{
			lfTransverse = num3;
		}
		p2d xPoint4 = Intersection.GetXPoint(new p2d(CPos.X, CPos.Z), TrimAngle - 90f, new p2d(wlPosition.X, wlPosition.Z), TrimAngle);
		p2d xPoint5 = Intersection.GetXPoint(new p2d(GPosition.X, GPosition.Z), TrimAngle - 90f, new p2d(wlPosition.X, wlPosition.Z), TrimAngle);
		p2d xPoint6 = Intersection.GetXPoint(new p2d(0f, 0f), TrimAngle - 90f, new p2d(wlPosition.X, wlPosition.Z), TrimAngle);
		RightningLongArm = new p2d[4]
		{
			new p2d(CPos.X, CPos.Z),
			xPoint4,
			xPoint5,
			new p2d(GPosition.X, GPosition.Z)
		};
		RightningLongArmForm = new p2d[4]
		{
			new p2d(CPos.X, CPos.Z),
			xPoint4,
			xPoint6,
			new p2d(0f, 0f)
		};
		num2 = (float)Math.Sqrt(Math.Pow(xPoint4.X - xPoint5.X, 2.0) + Math.Pow(xPoint4.Y - xPoint5.Y, 2.0));
		num4 = (float)Math.Sqrt(Math.Pow(xPoint4.X - xPoint6.X, 2.0) + Math.Pow(xPoint4.Y - xPoint6.Y, 2.0));
		p2d[] inputPnts2 = Transform.MoveContur(xPoint5, new p2d(0f, 0f), new p2d[2] { xPoint5, xPoint4 });
		inputPnts2 = Transform.RotateContur(new p2d(0f, 0f), inputPnts2, 0f - TrimAngle);
		if (inputPnts2[1].X < 0f)
		{
			lLongitudinal = 0f - num2;
		}
		else
		{
			lLongitudinal = num2;
		}
		inputPnts2 = Transform.MoveContur(xPoint6, new p2d(0f, 0f), new p2d[2] { xPoint6, xPoint4 });
		inputPnts2 = Transform.RotateContur(new p2d(0f, 0f), inputPnts2, 0f - TrimAngle);
		if (inputPnts2[1].X < 0f)
		{
			lfLongitudinal = 0f - num4;
		}
		else
		{
			lfLongitudinal = num4;
		}
	}

	public void SetGPosition(p3d GPosition)
	{
		GPos = GPosition;
	}

	public void GetMetacentricH()
	{
		StabilityCurve stabilityCurve = new StabilityCurve(DSPL, "", diagrammTransvPoints);
		float num = stabilityCurve.ReturnTrueAngleOnWater();
		for (int i = 0; i < stabilityCurve.Points.Count; i++)
		{
			if (stabilityCurve.Points[i].X > num)
			{
				p2d xPoint = Intersection.GetXPoint(new p2d(num, 0f), new p2d(stabilityCurve.Points[i].X, stabilityCurve.Points[i].Y), new p2d(num + 57.3f, 0f), new p2d(num + 57.3f, 1f), isExtendFirst: true, isExtendSecond: true);
				mTransverseHeight = Mathematics.GetVectorLength(new p2d[2]
				{
					new p2d(num + 57.3f, 0f),
					new p2d(xPoint.X, xPoint.Y)
				});
				break;
			}
		}
		StabilityCurve stabilityCurve2 = new StabilityCurve(DSPL, "", diagrammLongPoints);
		float num2 = stabilityCurve2.ReturnTrueAngleOnWater();
		for (int j = 0; j < stabilityCurve2.Points.Count; j++)
		{
			if (stabilityCurve2.Points[j].X > num2)
			{
				p2d xPoint2 = Intersection.GetXPoint(new p2d(num2, 0f), new p2d(stabilityCurve2.Points[j].X, stabilityCurve2.Points[j].Y), new p2d(num2 + 57.3f, 0f), new p2d(num2 + 57.3f, 1f), isExtendFirst: true, isExtendSecond: true);
				mLongHeight = Mathematics.GetVectorLength(new p2d[2]
				{
					new p2d(num2 + 57.3f, 0f),
					new p2d(xPoint2.X, xPoint2.Y)
				});
				break;
			}
		}
	}

	public void GetMetacentricH(List<Station> lstStation, float precisy, float aquaDensity, p3d Gpos)
	{
		Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(V, HeelAngle - 1f, TrimAngle, lstStation, precisy, aquaDensity);
		waterlanding_Volume_Heel_Trim.CalcStabilityArms(Gpos);
		waterlanding_Volume_Heel_Trim.SetGPosition(Gpos);
		CalcStabilityArms(waterlanding_Volume_Heel_Trim.GPos);
		float num = lTransverse - waterlanding_Volume_Heel_Trim.lTransverse;
		float num2 = HeelAngle * ((float)Math.PI / 180f) - waterlanding_Volume_Heel_Trim.HeelAngle * ((float)Math.PI / 180f);
		float num3 = num / num2;
		mTransverseHeight = num3;
		Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim2 = new Waterlanding_Volume_Heel_Trim(V, HeelAngle, TrimAngle - 0.1f, lstStation, precisy, aquaDensity);
		waterlanding_Volume_Heel_Trim2.CalcStabilityArms(Gpos);
		waterlanding_Volume_Heel_Trim2.SetGPosition(Gpos);
		CalcStabilityArms(waterlanding_Volume_Heel_Trim2.GPos);
		float num4 = lLongitudinal - waterlanding_Volume_Heel_Trim2.lLongitudinal;
		float num5 = TrimAngle * ((float)Math.PI / 180f) - waterlanding_Volume_Heel_Trim2.TrimAngle * ((float)Math.PI / 180f);
		float num6 = num4 / num5;
		mLongHeight = num6;
	}

	public void GetDStability()
	{
		try
		{
			float num = 0f;
			diagrammDLongPoints.Clear();
			diagrammDTransvPoints.Clear();
			diagrammDTransvPoints.Add(new p2d(0f, 0f));
			diagrammDLongPoints.Add(new p2d(0f, 0f));
			for (int i = 1; i < diagrammTransvPoints.Count; i++)
			{
				if (diagrammTransvPoints[i - 1].X >= 0f)
				{
					num += 0.5f * (diagrammTransvPoints[i].X * ((float)Math.PI / 180f) - diagrammTransvPoints[i - 1].X * ((float)Math.PI / 180f)) * (diagrammTransvPoints[i].Y + diagrammTransvPoints[i - 1].Y);
					diagrammDTransvPoints.Add(new p2d(diagrammTransvPoints[i].X, num));
				}
			}
			diagrammDLongPoints.Add(new p2d(diagrammLongPoints[0].X, num));
			for (int j = 1; j < diagrammLongPoints.Count; j++)
			{
				if (diagrammLongPoints[j - 1].X >= 0f)
				{
					num += 0.5f * (diagrammLongPoints[j].X * ((float)Math.PI / 180f) - diagrammLongPoints[j - 1].X * ((float)Math.PI / 180f)) * (diagrammLongPoints[j].Y + diagrammLongPoints[j - 1].Y);
					diagrammDLongPoints.Add(new p2d(diagrammLongPoints[j].X, num));
				}
			}
		}
		catch
		{
		}
	}
}
