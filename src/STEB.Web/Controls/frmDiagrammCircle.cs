using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using STEB2;
using STEB2.Cisterns;
using STEB2.Compartments;
using STEB2.Containers;
using STEB2.Converting;
using STEB2.Geometry;
using STEB2.GraphicsElement;
using STEB2.Mass;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB_LOAD;

public class frmDiagrammCircle : Form
{
	private ToolStripLabel StartCornerDirectionLabel = new ToolStripLabel("Начальный угол направления наклонения");

	private ToolStripTextBox StartCornerDirection = new ToolStripTextBox();

	private ToolStripLabel EndCornerDirectionLabel = new ToolStripLabel("Конечный угол направления наклонения");

	private ToolStripTextBox EndCornerDirection = new ToolStripTextBox();

	private ToolStripLabel StepCornerDirectionLabel = new ToolStripLabel("Шаг угла направления наклонения");

	private ToolStripTextBox StepCornerDirection = new ToolStripTextBox();

	private ToolStripLabel StartCornerSpaceLabel = new ToolStripLabel("Начальный угол пространственного наклонения");

	private ToolStripTextBox StartCornerSpace = new ToolStripTextBox();

	private ToolStripLabel EndCornerSpaceLabel = new ToolStripLabel("Конечный угол пространственного наклонения");

	private ToolStripTextBox EndCornerSpace = new ToolStripTextBox();

	private ToolStripLabel StepCornerSpaceLabel = new ToolStripLabel("Шаг угла пространственного наклонения");

	private ToolStripTextBox StepCornerSpace = new ToolStripTextBox();

	private ToolStripButton buttonCalc = new ToolStripButton("Расчет");

	private usgModelFrame modelFrame;

	private float aquaDensity;

	private List<Station> stations;

	private List<Compartment> compartments;

	private List<Cistern> cisterns;

	private List<STEB2.Containers.Container> containers;

	private float BaseMass = 0f;

	private p3d BaseGPos;

	public frmDiagrammCircle(float aquaDensity, List<Station> stations, List<STEB2.Containers.Container> containers, List<Compartment> compartments, List<Cistern> cisterns, float BaseMass, p3d BaseGPos)
	{
		InitializeComponent();
		this.aquaDensity = aquaDensity;
		this.stations = stations;
		this.containers = containers;
		this.compartments = compartments;
		this.cisterns = cisterns;
		this.BaseMass = BaseMass;
		this.BaseGPos = BaseGPos;
		StartCornerDirection.Text = "0";
		StartCornerSpace.Text = "0";
		EndCornerDirection.Text = "360";
		EndCornerSpace.Text = "25";
		StepCornerDirection.Text = "30";
		StepCornerSpace.Text = "5";
		Padding = new Padding(10);
		buttonCalc.DisplayStyle = ToolStripItemDisplayStyle.Text;
		buttonCalc.Font = new Font(buttonCalc.Font, FontStyle.Bold);
		ToolStrip toolStrip = new ToolStrip(StartCornerDirectionLabel, StartCornerDirection)
		{
			Padding = new Padding(5)
		};
		ToolStrip toolStrip2 = new ToolStrip(EndCornerDirectionLabel, EndCornerDirection)
		{
			Padding = new Padding(5)
		};
		ToolStrip toolStrip3 = new ToolStrip(StepCornerDirectionLabel, StepCornerDirection)
		{
			Padding = new Padding(5)
		};
		ToolStrip toolStrip4 = new ToolStrip(StartCornerSpaceLabel, StartCornerSpace)
		{
			Padding = new Padding(5)
		};
		ToolStrip toolStrip5 = new ToolStrip(EndCornerSpaceLabel, EndCornerSpace)
		{
			Padding = new Padding(5)
		};
		ToolStrip toolStrip6 = new ToolStrip(StepCornerSpaceLabel, StepCornerSpace)
		{
			Padding = new Padding(5)
		};
		ToolStrip toolStrip7 = new ToolStrip(buttonCalc);
		toolStrip.LayoutStyle = ToolStripLayoutStyle.Flow;
		toolStrip7.LayoutStyle = ToolStripLayoutStyle.Flow;
		modelFrame = new usgModelFrame();
		modelFrame.BackColor = Color.White;
		modelFrame.ModelFrameCaption = "Круговая диаграмма остойчивости";
		modelFrame.Dock = DockStyle.Fill;
		SplitContainer splitContainer = new SplitContainer();
		Controls.AddRange(new Control[8] { toolStrip, toolStrip2, toolStrip3, toolStrip4, toolStrip5, toolStrip6, toolStrip7, modelFrame });
		modelFrame.BringToFront();
		buttonCalc.Click += buttonCalc_Click;
		Show();
	}

	private void Calc(float baseMass, p3d baseGPos, List<Compartment> compartments, List<STEB2.Containers.Container> containers, List<Cistern> cisterns, List<Station> stations, float Precisy, float aquaDensity)
	{
		modelFrame.ClearDB();
		float num = Converting.TextToFloat(StartCornerDirection.Text);
		float num2 = Converting.TextToFloat(EndCornerDirection.Text);
		float num3 = Converting.TextToFloat(StepCornerDirection.Text);
		float num4 = Converting.TextToFloat(StartCornerSpace.Text);
		float num5 = Converting.TextToFloat(EndCornerSpace.Text);
		float num6 = Converting.TextToFloat(StepCornerSpace.Text);
		Progress.ProgressMaximum = 0;
		for (float num7 = num4; num7 <= num5; num7 += num6)
		{
			for (float num8 = num; num8 <= num2; num8 += num3)
			{
				Progress.ProgressMaximum++;
			}
		}
		using (new frmPleaseWait())
		{
			for (float num7 = num4; num7 <= num5; num7 += num6)
			{
				List<p2d> list = new List<p2d>();
				List<p2d> list2 = new List<p2d>();
				List<Text> list3 = new List<Text>();
				for (float num8 = num; num8 <= num2; num8 += num3)
				{
					if (num7 == 0f || num7 % 90f == 0f || num8 == 0f || num8 % 90f == 0f)
					{
						continue;
					}
					Progress.ProgressPosition++;
					Progress.status = "Угол направления наклонения " + num8 + "\nУгол пространственного наклонения " + num7;
					float num9 = 0f;
					num9 = (float)Math.Atan(Math.Tan((double)num7 * Math.PI / 180.0) / Math.Cos((double)num8 * Math.PI / 180.0));
					num9 = (float)((double)(num9 * 180f) / Math.PI);
					float num10 = 0f;
					num10 = (float)Math.Atan(Math.Tan((double)num7 * Math.PI / 180.0) / Math.Sin((double)num8 * Math.PI / 180.0));
					num10 = (float)((double)(num10 * 180f) / Math.PI);
					if (!float.IsNaN(num9) && !float.IsNaN(num10) && !float.IsInfinity(num9) && !float.IsInfinity(num10))
					{
						List<MassTable> list4 = STEB2.Mass.Tools.CalcMassGposByCompartmentsContainersBaseMassCisterns(baseMass, baseGPos, compartments, containers, cisterns, num10, num9, Precisy);
						Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(list4[0].Mass / aquaDensity, num10, num9, stations, Precisy, aquaDensity);
						waterlanding_Volume_Heel_Trim.GPos = list4[0].GPosition;
						waterlanding_Volume_Heel_Trim.CalcStabilityArms(waterlanding_Volume_Heel_Trim.GPos);
						if (!float.IsNaN(waterlanding_Volume_Heel_Trim.lTransverse) && !float.IsInfinity(waterlanding_Volume_Heel_Trim.lTransverse) && waterlanding_Volume_Heel_Trim.lTransverse != 0f)
						{
							p2d[] inputPnts = new p2d[2]
							{
								new p2d(0f, 0f),
								new p2d(Math.Abs(waterlanding_Volume_Heel_Trim.lTransverse), 0f)
							};
							inputPnts = Transform.RotateContur(new p2d(0f, 0f), inputPnts, num8);
							list.Add(new p2d(inputPnts[1].X, inputPnts[1].Y));
						}
					}
				}
				foreach (p2d item in list2)
				{
					modelFrame.Dots.Add(new Dot(new PointF[1]
					{
						new PointF(item.X, item.Y)
					}, 4f, Color.Blue, "calcPoints"));
				}
				foreach (Text item2 in list3)
				{
					modelFrame.Texts.Add(item2);
				}
				modelFrame.Polylines.Add(new Polyline(DashStyle.Solid, Converting.P2dToPointF(Converting.ListToArray(list)), 2, Color.Red, "graphSpline"));
				try
				{
					modelFrame.Texts.Add(new Text(new PointF[1]
					{
						new PointF(list[0].X, list[0].Y)
					}, modelFrame.Font, num7.ToString(), Color.Black, ContentAlignment.MiddleCenter, "graphSpline"));
				}
				catch
				{
				}
				modelFrame.Redraw();
			}
		}
		List<p2d> list5 = new List<p2d>();
		foreach (Dot dot in modelFrame.Dots)
		{
			try
			{
				for (int i = 0; i < dot.pnts.Length; i++)
				{
					list5.Add(new p2d(dot.pnts[i].X * modelFrame.ScaleX, dot.pnts[i].Y * modelFrame.ScaleY));
				}
			}
			catch
			{
			}
		}
		foreach (Polyline polyline in modelFrame.Polylines)
		{
			try
			{
				for (int i = 0; i < polyline.pnts.Length; i++)
				{
					list5.Add(new p2d(polyline.pnts[i].X * modelFrame.ScaleX, polyline.pnts[i].Y * modelFrame.ScaleY));
				}
			}
			catch
			{
			}
		}
		if (list5.Count > 0)
		{
			Contur contur = new Contur(list5);
			p2d p2d2 = Mathematics.GetBoundRect(Converting.ListToArray(list5))[0];
			p2d p2d3 = Mathematics.GetBoundRect(Converting.ListToArray(list5))[2];
			float num11 = p2d3.X - p2d2.X;
			float num12 = p2d3.Y - p2d2.Y;
			PointF pointF = new PointF(num11 / 2f, num12 / 2f);
			float num13 = ((num11 >= num12) ? num11 : num12);
			for (float num14 = 0f; num14 <= num13; num14 += num13 / 10f)
			{
				modelFrame.Circles.Add(new Circle(DashStyle.Solid, new PointF[4]
				{
					new PointF((0f - num14) / 2f, (0f - num14) / 2f),
					new PointF(num14 / 2f, (0f - num14) / 2f),
					new PointF(num14 / 2f, num14 / 2f),
					new PointF((0f - num14) / 2f, num14 / 2f)
				}, 1, Color.DarkGreen, "graphCircle"));
				p2d[] array = new p2d[3]
				{
					new p2d(0f, 0f),
					new p2d(num14 / 2f, 0f),
					new p2d((0f - num14) / 2f, 0f)
				};
				modelFrame.Texts.Add(new Text(new PointF[1]
				{
					new PointF(array[1].X, array[1].Y)
				}, modelFrame.Font, (num14 / 2f).ToString("0.00"), Color.Black, ContentAlignment.MiddleCenter, "graphAxesText"));
				modelFrame.Texts.Add(new Text(new PointF[1]
				{
					new PointF(array[2].X, array[2].Y)
				}, modelFrame.Font, (num14 / 2f).ToString("0.00"), Color.Black, ContentAlignment.MiddleCenter, "graphAxesText"));
			}
			for (float num14 = 0f; num14 <= 360f; num14 += 15f)
			{
				p2d[] inputPnts2 = new p2d[2]
				{
					new p2d(0f, 0f),
					new p2d(num13 / 2f, 0f)
				};
				inputPnts2 = Transform.RotateContur(new p2d(0f, 0f), inputPnts2, num14);
				modelFrame.Lines.Add(new Line(Converting.P2dToPointF(inputPnts2), DashStyle.Solid, Color.DarkGreen, 1, "graphAxes"));
				p2d[] array = new p2d[2]
				{
					new p2d(0f, 0f),
					new p2d(num13 / 2f + num13 / 50f, 0f)
				};
				array = Transform.RotateContur(new p2d(0f, 0f), array, num14);
				modelFrame.Texts.Add(new Text(new PointF[1]
				{
					new PointF(array[1].X, array[1].Y)
				}, modelFrame.Font, num14.ToString(), Color.Black, ContentAlignment.MiddleCenter, "graphAxesText"));
			}
		}
		DrawProjectElements.DrawTopSide(modelFrame, this.stations, drawStationsSled: false, "Проекция полуширота", "", DrawAxes: false, "topSide", Color.White);
		modelFrame.MoveGroup(new p2d(0f, 0f), new p2d(0f - baseGPos.X, 0f - baseGPos.Y), new string[1] { "topSide" });
	}

	public void buttonCalc_Click(object sender, EventArgs e)
	{
		Calc(BaseMass, BaseGPos, compartments, containers, cisterns, stations, 0.001f, aquaDensity);
	}

	private void InitializeComponent()
	{
		Text = "Круговая диаграмма остойчивости";
	}
}
