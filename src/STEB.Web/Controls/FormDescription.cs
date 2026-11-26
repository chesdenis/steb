using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using STEB2.Converting;
using STEB2.Geometry;
using STEB2.Stations;

namespace STEB2;

public class FormDescription : UserControl
{

	public RichTextBox richTextBox_FormTextView;

	public usgModelFrame usgModelFrame_FormDrawing;

	public event EventHandler FormDescriptionChanged;

	public FormDescription()
	{
		InitializeComponent();
		richTextBox_FormTextView.TextChanged += richTextBox_FormTextView_TextChanged;
	}

	public void richTextBox_FormTextView_TextChanged(object sender, EventArgs e)
	{
		if (FormDescriptionChanged != null)
		{
			FormDescriptionChanged(new object(), new EventArgs());
		}
	}

	public void ClearForm()
	{
		usgModelFrame_FormDrawing.ClearDB();
	}

	public void SetForm(List<Station> stations)
	{
		if (stations.Count == 0)
		{
			return;
		}
		Random random = new Random();
		usgModelFrame_FormDrawing.ClearDB();
		List<p2d> list = new List<p2d>();
		foreach (Station station in stations)
		{
			Contur[] arrayConturs = station.StationContur.arrayConturs;
			foreach (Contur contur in arrayConturs)
			{
				list.AddRange(STEB2.Converting.Converting.ArrayToList(contur.arrayPoints));
			}
		}
		p2d[] boundRect = Mathematics.GetBoundRect(list);
		usgModelFrame_FormDrawing.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(boundRect), 1, Color.DarkGreen, ""));
		usgModelFrame_FormDrawing.Texts.Add(new Text(new PointF[1]
		{
			new PointF(boundRect[0].X, boundRect[0].Y)
		}, Font, "ОЛ", Color.Black, ContentAlignment.BottomRight, ""));
		usgModelFrame_FormDrawing.Texts.Add(new Text(new PointF[1]
		{
			new PointF(boundRect[3].X, boundRect[3].Y)
		}, Font, "ОЛ", Color.Black, ContentAlignment.BottomLeft, ""));
		usgModelFrame_FormDrawing.Texts.Add(new Text(new PointF[1]
		{
			new PointF(0f, 0f)
		}, Font, "ДП", Color.Black, ContentAlignment.BottomCenter, ""));
		usgModelFrame_FormDrawing.Texts.Add(new Text(new PointF[1]
		{
			new PointF(0f, boundRect[2].Y)
		}, Font, "ДП", Color.Black, ContentAlignment.TopCenter, ""));
		usgModelFrame_FormDrawing.Dimensions.Add(new Dimension(new PointF(boundRect[0].X, boundRect[0].Y), new PointF(boundRect[3].X, boundRect[3].Y), DimOrientation.hor, 50f, (boundRect[3].X - boundRect[0].X).ToString("0.000"), Font, DashStyle.Solid, Color.Red, 1, ""));
		usgModelFrame_FormDrawing.Dimensions.Add(new Dimension(new PointF(boundRect[3].X, boundRect[3].Y), new PointF(boundRect[2].X, boundRect[2].Y), DimOrientation.vert, 50f, (boundRect[2].Y - boundRect[3].Y).ToString("0.000"), Font, DashStyle.Solid, Color.Red, 1, ""));
		foreach (Station station2 in stations)
		{
			Contur[] arrayConturs2 = station2.StationContur.arrayConturs;
			foreach (Contur contur2 in arrayConturs2)
			{
				usgModelFrame_FormDrawing.Polylines.Add(new Polyline(DashStyle.Solid, STEB2.Converting.Converting.P2dToPointF(contur2.arrayPoints), 1, Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)), ""));
			}
		}
		usgModelFrame_FormDrawing.Redraw();
	}

	public void SetText(List<Station> stations)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Station station in stations)
		{
			foreach (Contur listContur in station.StationContur.listConturs)
			{
				stringBuilder.AppendLine("new " + station.X);
				foreach (p2d listPoint in listContur.listPoints)
				{
					float num = listPoint.X;
					string text = num.ToString("0.000");
					float num2 = listPoint.Y;
					stringBuilder.AppendLine("Y = " + text + " Z = " + num2.ToString("0.000"));
				}
			}
		}
		richTextBox_FormTextView.Text = stringBuilder.ToString();
	}

	private void InitializeComponent()
	{
		this.usgModelFrame_FormDrawing = new STEB2.usgModelFrame();
		this.richTextBox_FormTextView = new System.Windows.Forms.RichTextBox();
		this.usgModelFrame_FormDrawing.Name = "usgModelFrame_FormDrawing";
		this.richTextBox_FormTextView.Name = "richTextBox_FormTextView";
		this.richTextBox_FormTextView.Text = "";
	}
}
