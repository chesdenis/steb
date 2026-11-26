using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using STEB2.Converting;
using STEB2.Geometry;

namespace STEB2;

/// <summary>
/// Display list + view transform of a drawing (originally a WinForms UserControl).
/// Ported headless: painting goes to an SVG <see cref="Graphics"/> and the
/// Blazor viewer forwards mouse events. Drawing logic is unchanged.
/// </summary>
public partial class usgModelFrame : UserControl
{
	private string _modelFrameCaption = "";

	private bool isDrag;

	private bool drawApperture;

	private Point oldLocation = default;

	private bool NeedRecalcPoints;

	private bool _DrawOrigin;

	private List<Text> textToPublish = new List<Text>();

	private PointF _screenCenter = new PointF(0f, 0f);

	private float _screenScale = 25f;

	public List<Line> Lines = new List<Line>();

	public List<Dot> Dots = new List<Dot>();

	public List<Text> Texts = new List<Text>();

	public List<Polyline> Polylines = new List<Polyline>();

	public List<Hatch> Hatchs = new List<Hatch>();

	public List<Spline> Splines = new List<Spline>();

	public List<Dimension> Dimensions = new List<Dimension>();

	public List<FillingContur> Fillings = new List<FillingContur>();

	public List<Circle> Circles = new List<Circle>();

	private PointF _origin = default;

	// ---- headless replacements for the former WinForms child controls ----
	public ToolStripTextBox toolStripTextBox_ScaleX = new ToolStripTextBox("1");

	public ToolStripTextBox toolStripTextBox_ScaleY = new ToolStripTextBox("1");

	public ToolStripTextBox toolStripTextBox_FontSize = new ToolStripTextBox("10");

	private readonly Surface pictureBox_ModelFrame = new Surface(892, 353);

	private readonly Surface toolStrip1 = new Surface(892, 0);

	public string ModelFrameCaption
	{
		get
		{
			return _modelFrameCaption;
		}
		set
		{
			_modelFrameCaption = value;
		}
	}

	[Description("Рисовать ли начало координат")]
	[Browsable(true)]
	[DefaultValue(false)]
	public bool DrawOrigin
	{
		get
		{
			return _DrawOrigin;
		}
		set
		{
			_DrawOrigin = value;
		}
	}

	public PointF screenCenter
	{
		get
		{
			return _screenCenter;
		}
		set
		{
			_screenCenter = value;
			NeedRecalcPoints = true;
			Invalidate();
		}
	}

	public float screenScale
	{
		get
		{
			return _screenScale;
		}
		set
		{
			_screenScale = Math.Abs(value);
			NeedRecalcPoints = true;
			Invalidate();
		}
	}

	public PointF Origin
	{
		get
		{
			return _origin;
		}
		set
		{
			_origin = value;
			Invalidate();
		}
	}

	private float TextSize
	{
		get
		{
			return STEB2.Converting.Converting.TextToFloat(toolStripTextBox_FontSize.Text);
		}
		set
		{
			toolStripTextBox_FontSize.Text = value.ToString();
			NeedRecalcPoints = true;
		}
	}

	public float ScaleX
	{
		get
		{
			return STEB2.Converting.Converting.TextToFloat(toolStripTextBox_ScaleX.Text);
		}
		set
		{
			toolStripTextBox_ScaleX.Text = value.ToString();
			NeedRecalcPoints = true;
		}
	}

	public float ScaleY
	{
		get
		{
			return STEB2.Converting.Converting.TextToFloat(toolStripTextBox_ScaleY.Text);
		}
		set
		{
			toolStripTextBox_ScaleY.Text = value.ToString();
			NeedRecalcPoints = true;
		}
	}

	public usgModelFrame()
	{
		pictureBox_ModelFrame.Redrawn += Invalidate;
		toolStripTextBox_ScaleX.TextChanged += toolStripTextBox_ScaleX_TextChanged;
		toolStripTextBox_ScaleY.TextChanged += toolStripTextBox_ScaleY_TextChanged;
		toolStripTextBox_FontSize.TextChanged += toolStripTextBox_FontSize_TextChanged;
	}

	public void SaveImage(string PathToImage)
	{
		try
		{
			Image image = new Bitmap(pictureBox_ModelFrame.Width, pictureBox_ModelFrame.Height);
			using (Graphics graphics = Graphics.FromImage(image))
			{
				graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, pictureBox_ModelFrame.Width, pictureBox_ModelFrame.Height));
				drawFilling(graphics);
				drawHatch(graphics);
				drawLines(graphics);
				drawPolylines(graphics);
				drawPolylinesSplines(graphics);
				drawCircles(graphics);
				drawTexts(graphics);
				drawDots(graphics);
				drawDimensions(graphics);
				if (DrawOrigin)
				{
					DrawCenterOrigin(graphics);
				}
			}
			image.Save(PathToImage, ImageFormat.Bmp);
		}
		catch
		{
		}
	}

	private void toolStripTextBox_FontSize_TextChanged(object sender, EventArgs e)
	{
		ZoomAll();
	}

	private void toolStripTextBox_ScaleY_TextChanged(object sender, EventArgs e)
	{
		ZoomAll();
	}

	private void toolStripTextBox_ScaleX_TextChanged(object sender, EventArgs e)
	{
		ZoomAll();
	}

	private void toolStripButton_ZoomAll_Click(object sender, EventArgs e)
	{
		ZoomAll();
	}

	private void pictureBox_ModelFrame_MouseEnter(object sender, EventArgs e)
	{
		drawApperture = true;
	}

	private void pictureBox_ModelFrame_MouseLeave(object sender, EventArgs e)
	{
		drawApperture = false;
		pictureBox_ModelFrame.Invalidate();
	}

	private void pictureBox_ModelFrame_MouseWheel(object sender, MouseEventArgs e)
	{
		PointF curPosInModel = getCurPosInModel();
		float num = _screenCenter.X * screenScale;
		float num2 = _screenCenter.Y * screenScale;
		float num3 = curPosInModel.X * screenScale;
		float num4 = curPosInModel.Y * screenScale;
		float num5 = screenScale + screenScale * ((float)e.Delta / 1000f);
		float num6 = _screenCenter.X * num5;
		float num7 = _screenCenter.Y * num5;
		float num8 = curPosInModel.X * num5;
		float num9 = curPosInModel.Y * num5;
		screenScale = num5;
		_screenCenter.X -= (num6 - num) / screenScale + (num8 - num3) / screenScale;
		_screenCenter.Y -= (num7 - num2) / screenScale - (num9 - num4) / screenScale;
	}

	private void pictureBox_ModelFrame_MouseMove(object sender, MouseEventArgs e)
	{
		Point location = e.Location;
		if (isDrag)
		{
			float num = location.X - oldLocation.X;
			float num2 = location.Y - oldLocation.Y;
			_screenCenter.X += num / screenScale;
			_screenCenter.Y += num2 / screenScale;
			oldLocation = location;
			NeedRecalcPoints = true;
			pictureBox_ModelFrame.Invalidate();
		}
		oldLocation = location;
		pictureBox_ModelFrame.Invalidate();
	}

	private void pictureBox_ModelFrame_MouseUp(object sender, MouseEventArgs e)
	{
		isDrag = false;
	}

	private void pictureBox_ModelFrame_MouseDown(object sender, MouseEventArgs e)
	{
		pictureBox_ModelFrame.Focus();
		isDrag = true;
		oldLocation = e.Location;
		pictureBox_ModelFrame_MouseMove(sender, e);
	}

	private void pictureBox_ModelFrame_Paint(object sender, PaintEventArgs e)
	{
		e.Graphics.DrawString(screenCenter.X / ScaleX + ";" + screenCenter.Y / ScaleY + "(" + screenScale + ")", Font, Brushes.Blue, new PointF(0f, pictureBox_ModelFrame.Height - Font.Height));
		drawFilling(e.Graphics);
		drawHatch(e.Graphics);
		drawLines(e.Graphics);
		drawPolylines(e.Graphics);
		drawPolylinesSplines(e.Graphics);
		drawCircles(e.Graphics);
		if (!isDrag)
		{
			drawTexts(e.Graphics);
		}
		drawDots(e.Graphics);
		drawDimensions(e.Graphics);
		if (DrawOrigin)
		{
			DrawCenterOrigin(e.Graphics);
		}
		if (drawApperture)
		{
			DrawApperture(e.Graphics);
		}
		e.Graphics.DrawString(_modelFrameCaption, Font, Brushes.Black, new PointF(0f, 0f));
		if (!isDrag)
		{
			NeedRecalcPoints = false;
		}
	}

	private void DrawApperture(Graphics g)
	{
		using Pen pen = new Pen(Brushes.Green, 1f);
		Point point = PointToClient(CursorPosition);
		point.Y -= toolStrip1.Height;
		PointF curPosInModel = getCurPosInModel();
		g.DrawLine(pen, point.X, point.Y, point.X, pictureBox_ModelFrame.Height);
		g.DrawLine(pen, point.X, point.Y, point.X, -pictureBox_ModelFrame.Height);
		g.DrawLine(pen, point.X, point.Y, pictureBox_ModelFrame.Width, point.Y);
		g.DrawLine(pen, point.X, point.Y, -pictureBox_ModelFrame.Width, point.Y);
		g.FillRectangle(new SolidBrush(BackColor), new RectangleF(new PointF((float)point.X + 5f, (float)point.Y + 5f), g.MeasureString(curPosInModel.X / ScaleX + ";" + curPosInModel.Y / ScaleY, Font)));
		g.DrawString(curPosInModel.X / ScaleX + ";" + curPosInModel.Y / ScaleY, Font, Brushes.Blue, (float)point.X + 5f, (float)point.Y + 5f);
	}

	private void drawFilling(Graphics g)
	{
		foreach (FillingContur filling in Fillings)
		{
			using Pen pen = new Pen(filling.color, filling.penWidth);
			pen.DashStyle = filling.dashStyle;
			SolidBrush brush = new SolidBrush(filling.FillColor);
			try
			{
				if (NeedRecalcPoints)
				{
					filling.drwpnts = PrepareToPrintInWorkArea(ScalePoints(filling.pnts));
				}
				g.FillPolygon(brush, filling.drwpnts);
				g.DrawLines(pen, filling.drwpnts);
			}
			catch
			{
			}
		}
	}

	private void drawHatch(Graphics g)
	{
		foreach (Hatch hatch in Hatchs)
		{
			using Pen pen = new Pen(hatch.color, hatch.penWidth);
			pen.DashStyle = hatch.dashStyle;
			HatchBrush brush = new HatchBrush(hatch.hatchStyle, hatch.FillColor);
			try
			{
				if (NeedRecalcPoints)
				{
					hatch.drwpnts = PrepareToPrintInWorkArea(ScalePoints(hatch.pnts));
				}
				g.FillPolygon(brush, hatch.drwpnts);
				g.DrawLines(pen, hatch.drwpnts);
			}
			catch
			{
			}
		}
	}

	private void drawLines(Graphics g)
	{
		foreach (Line line in Lines)
		{
			using Pen pen = new Pen(line.color, line.penWidth);
			pen.DashStyle = line.dashStyle;
			if (line.lineStartCap != LineCap.NoAnchor)
			{
				pen.StartCap = line.lineStartCap;
			}
			if (line.lineEndCap != LineCap.NoAnchor)
			{
				pen.EndCap = line.lineEndCap;
			}
			try
			{
				if (NeedRecalcPoints)
				{
					line.drwpnts = PrepareToPrintInWorkArea(ScalePoints(line.pnts));
				}
				g.DrawLines(pen, line.drwpnts);
			}
			catch
			{
			}
		}
	}

	private void drawCircles(Graphics g)
	{
		foreach (Circle circle in Circles)
		{
			using Pen pen = new Pen(circle.color, circle.penWidth);
			pen.DashStyle = circle.dashStyle;
			try
			{
				if (NeedRecalcPoints)
				{
					circle.drwpnts = PrepareToPrintInWorkArea(ScalePoints(circle.pnts));
				}
				g.DrawEllipse(pen, new RectangleF(circle.drwpnts[0], new SizeF(circle.drwpnts[1].X - circle.drwpnts[0].X, circle.drwpnts[2].Y - circle.drwpnts[1].Y)));
			}
			catch
			{
			}
		}
	}

	private void drawDots(Graphics g)
	{
		foreach (Dot dot in Dots)
		{
			using Pen pen = new Pen(dot.color);
			try
			{
				if (NeedRecalcPoints)
				{
					dot.drwpnts = PrepareToPrintInWorkArea(ScalePoints(dot.pnts));
				}
				g.DrawEllipse(pen, new RectangleF(dot.drwpnts[0].X - dot.radius / 2f, dot.drwpnts[0].Y - dot.radius / 2f, dot.radius, dot.radius));
				g.FillEllipse(new SolidBrush(dot.color), new RectangleF(dot.drwpnts[0].X - dot.radius / 2f, dot.drwpnts[0].Y - dot.radius / 2f, dot.radius, dot.radius));
			}
			catch
			{
			}
		}
	}

	private void drawPolylines(Graphics g)
	{
		foreach (Polyline polyline in Polylines)
		{
			using Pen pen = new Pen(polyline.color, polyline.penWidth);
			pen.DashStyle = polyline.dashStyle;
			try
			{
				if (NeedRecalcPoints)
				{
					polyline.drwpnts = PrepareToPrintInWorkArea(ScalePoints(polyline.pnts));
				}
				g.DrawLines(pen, polyline.drwpnts);
			}
			catch
			{
			}
		}
	}

	private void drawPolylinesSplines(Graphics g)
	{
		foreach (Spline spline in Splines)
		{
			using Pen pen = new Pen(spline.color, spline.penWidth);
			pen.DashStyle = spline.dashStyle;
			try
			{
				if (NeedRecalcPoints)
				{
					spline.drwpnts = PrepareToPrintInWorkArea(ScalePoints(spline.pnts));
				}
				g.DrawCurve(pen, spline.drwpnts);
			}
			catch
			{
			}
		}
	}

	private RectangleF GetRectangleOfText(Graphics g, Text tx)
	{
		PointF[] array = PrepareToPrintInWorkArea(ScalePoints(tx.pnts));
		RectangleF result = new RectangleF(new PointF(0f, 0f), g.MeasureString(tx.text, tx.textFont));
		switch (tx.TextAlign)
		{
		case ContentAlignment.BottomCenter:
			result.Location = new PointF(array[0].X - result.Width / 2f, array[0].Y);
			break;
		case ContentAlignment.BottomLeft:
			result.Location = new PointF(array[0].X - result.Width, array[0].Y);
			break;
		case ContentAlignment.BottomRight:
			result.Location = new PointF(array[0].X, array[0].Y);
			break;
		case ContentAlignment.MiddleCenter:
			result.Location = new PointF(array[0].X - result.Width / 2f, array[0].Y - result.Height / 2f);
			break;
		case ContentAlignment.MiddleLeft:
			result.Location = new PointF(array[0].X - result.Width, array[0].Y - result.Height / 2f);
			break;
		case ContentAlignment.MiddleRight:
			result.Location = new PointF(array[0].X, array[0].Y - result.Height / 2f);
			break;
		case ContentAlignment.TopCenter:
			result.Location = new PointF(array[0].X - result.Width / 2f, array[0].Y - result.Height);
			break;
		case ContentAlignment.TopLeft:
			result.Location = new PointF(array[0].X - result.Width, array[0].Y - result.Height);
			break;
		case ContentAlignment.TopRight:
			result.Location = new PointF(array[0].X, array[0].Y - result.Height);
			break;
		}
		return result;
	}

	private void drawTexts(Graphics g)
	{
		if (NeedRecalcPoints)
		{
			textToPublish.Clear();
			foreach (Text text in Texts)
			{
				RectangleF rectangleOfText = GetRectangleOfText(g, text);
				bool flag = true;
				foreach (Text item in textToPublish)
				{
					if (GetRectangleOfText(g, item).IntersectsWith(rectangleOfText))
					{
						flag = false;
					}
				}
				if (flag)
				{
					textToPublish.Add(text);
				}
				else if (text.drawAlways)
				{
					textToPublish.Add(text);
				}
			}
		}
		foreach (Text item2 in textToPublish)
		{
			using SolidBrush brush = new SolidBrush(item2.color);
			try
			{
				new HatchBrush(HatchStyle.Percent50, Color.Black);
				if (NeedRecalcPoints)
				{
					item2.drwpnts = PrepareToPrintInWorkArea(ScalePoints(item2.pnts));
				}
				SizeF sizeF = g.MeasureString(item2.text, item2.textFont);
				switch (item2.TextAlign)
				{
				case ContentAlignment.BottomCenter:
					g.DrawString(item2.text, new Font(item2.textFont.FontFamily, TextSize), brush, new PointF(item2.drwpnts[0].X - sizeF.Width / 2f, item2.drwpnts[0].Y));
					break;
				case ContentAlignment.BottomLeft:
					g.DrawString(item2.text, new Font(item2.textFont.FontFamily, TextSize), brush, new PointF(item2.drwpnts[0].X - sizeF.Width, item2.drwpnts[0].Y));
					break;
				case ContentAlignment.BottomRight:
					g.DrawString(item2.text, new Font(item2.textFont.FontFamily, TextSize), brush, new PointF(item2.drwpnts[0].X, item2.drwpnts[0].Y));
					break;
				case ContentAlignment.MiddleCenter:
					g.DrawString(item2.text, new Font(item2.textFont.FontFamily, TextSize), brush, new PointF(item2.drwpnts[0].X - sizeF.Width / 2f, item2.drwpnts[0].Y - sizeF.Height / 2f));
					break;
				case ContentAlignment.MiddleLeft:
					g.DrawString(item2.text, new Font(item2.textFont.FontFamily, TextSize), brush, new PointF(item2.drwpnts[0].X - sizeF.Width, item2.drwpnts[0].Y - sizeF.Height / 2f));
					break;
				case ContentAlignment.MiddleRight:
					g.DrawString(item2.text, new Font(item2.textFont.FontFamily, TextSize), brush, new PointF(item2.drwpnts[0].X, item2.drwpnts[0].Y - sizeF.Height / 2f));
					break;
				case ContentAlignment.TopCenter:
					g.DrawString(item2.text, new Font(item2.textFont.FontFamily, TextSize), brush, new PointF(item2.drwpnts[0].X - sizeF.Width / 2f, item2.drwpnts[0].Y - sizeF.Height));
					break;
				case ContentAlignment.TopLeft:
					g.DrawString(item2.text, new Font(item2.textFont.FontFamily, TextSize), brush, new PointF(item2.drwpnts[0].X - sizeF.Width, item2.drwpnts[0].Y - sizeF.Height));
					break;
				case ContentAlignment.TopRight:
					g.DrawString(item2.text, new Font(item2.textFont.FontFamily, TextSize), brush, new PointF(item2.drwpnts[0].X, item2.drwpnts[0].Y - sizeF.Height));
					break;
				}
			}
			catch
			{
			}
		}
	}

	private void drawDimensions(Graphics g)
	{
		foreach (Dimension dimension in Dimensions)
		{
			try
			{
				using Pen pen = new Pen(dimension.color, dimension.penWidth);
				switch (dimension.orient)
				{
				case DimOrientation.vert:
				{
					PointF[] array2 = PrepareToPrintInWorkArea(ScalePoints(new PointF[6]
					{
						dimension.sp,
						new PointF(dimension.sp.X - dimension.distance / screenScale, dimension.sp.Y),
						new PointF(dimension.sp.X - dimension.distance / screenScale + dimension.distance / screenScale / 7f, dimension.sp.Y),
						new PointF(dimension.ep.X - dimension.distance / screenScale + dimension.distance / screenScale / 7f, dimension.ep.Y),
						new PointF(dimension.ep.X - dimension.distance / screenScale, dimension.ep.Y),
						dimension.ep
					}));
					g.DrawLine(pen, array2[0], array2[1]);
					g.DrawLine(pen, array2[5], array2[4]);
					using (Pen pen3 = new Pen(new SolidBrush(dimension.color), dimension.penWidth))
					{
						pen3.StartCap = LineCap.ArrowAnchor;
						pen3.EndCap = LineCap.ArrowAnchor;
						pen3.MiterLimit = 10f;
						g.DrawLine(pen3, array2[2], array2[3]);
					}
					SizeF sizeF2 = g.MeasureString(dimension.Text, dimension.TextFont);
					g.DrawString(dimension.Text, new Font(dimension.TextFont.FontFamily, TextSize), new SolidBrush(dimension.color), new PointF(array2[2].X, array2[2].Y - sizeF2.Height / 2f + (array2[3].Y - array2[2].Y) / 2f));
					break;
				}
				case DimOrientation.hor:
				{
					PointF[] array = PrepareToPrintInWorkArea(ScalePoints(new PointF[6]
					{
						dimension.sp,
						new PointF(dimension.sp.X, dimension.sp.Y - dimension.distance / screenScale),
						new PointF(dimension.sp.X, dimension.sp.Y - dimension.distance / screenScale + dimension.distance / screenScale / 7f),
						new PointF(dimension.ep.X, dimension.ep.Y - dimension.distance / screenScale + dimension.distance / screenScale / 7f),
						new PointF(dimension.ep.X, dimension.ep.Y - dimension.distance / screenScale),
						dimension.ep
					}));
					g.DrawLine(pen, array[0], array[1]);
					g.DrawLine(pen, array[5], array[4]);
					using (Pen pen2 = new Pen(new SolidBrush(dimension.color), dimension.penWidth))
					{
						pen2.StartCap = LineCap.ArrowAnchor;
						pen2.EndCap = LineCap.ArrowAnchor;
						pen2.MiterLimit = 10f;
						g.DrawLine(pen2, array[2], array[3]);
					}
					SizeF sizeF = g.MeasureString(dimension.Text, dimension.TextFont);
					g.DrawString(dimension.Text, new Font(dimension.TextFont.FontFamily, TextSize), new SolidBrush(dimension.color), new PointF(array[2].X + (array[3].X - array[2].X) / 2f - sizeF.Width / 2f, array[2].Y - sizeF.Height));
					break;
				}
				}
			}
			catch
			{
			}
		}
	}

	private PointF[] PrepareToPrintInWorkArea(PointF[] InputPnts)
	{
		PointF[] array = new PointF[InputPnts.Length];
		for (int i = 0; i < InputPnts.Length; i++)
		{
			array[i].X = InputPnts[i].X * screenScale + screenCenter.X * screenScale;
			array[i].Y = (0f - InputPnts[i].Y) * screenScale + screenCenter.Y * screenScale;
		}
		return array;
	}

	private List<PointF> PrepareToPrintInWorkArea(List<PointF> InputPnts)
	{
		List<PointF> list = new List<PointF>();
		for (int i = 0; i < InputPnts.Count; i++)
		{
			list.Add(new PointF((InputPnts[i].X + screenCenter.X) * screenScale, (0f - InputPnts[i].Y + screenCenter.Y) * screenScale));
		}
		return list;
	}

	public void MoveGroup(p2d sp, p2d ep, string[] GroupTags)
	{
		float num = ep.X - sp.X;
		float num2 = ep.Y - sp.Y;
		foreach (Line line in Lines)
		{
			foreach (string text in GroupTags)
			{
				if (line.prefix == text)
				{
					for (int j = 0; j < line.pnts.Length; j++)
					{
						line.pnts[j].X = line.pnts[j].X + num;
						line.pnts[j].Y = line.pnts[j].Y + num2;
					}
				}
			}
		}
		foreach (Dot dot in Dots)
		{
			foreach (string text2 in GroupTags)
			{
				if (dot.prefix == text2)
				{
					for (int l = 0; l < dot.pnts.Length; l++)
					{
						dot.pnts[l].X = dot.pnts[l].X + num;
						dot.pnts[l].Y = dot.pnts[l].Y + num2;
					}
				}
			}
		}
		foreach (Polyline polyline in Polylines)
		{
			foreach (string text3 in GroupTags)
			{
				if (polyline.prefix == text3)
				{
					for (int n = 0; n < polyline.pnts.Length; n++)
					{
						polyline.pnts[n].X = polyline.pnts[n].X + num;
						polyline.pnts[n].Y = polyline.pnts[n].Y + num2;
					}
				}
			}
		}
		foreach (Spline spline in Splines)
		{
			foreach (string text4 in GroupTags)
			{
				if (spline.prefix == text4)
				{
					for (int num4 = 0; num4 < spline.pnts.Length; num4++)
					{
						spline.pnts[num4].X = spline.pnts[num4].X + num;
						spline.pnts[num4].Y = spline.pnts[num4].Y + num2;
					}
				}
			}
		}
		foreach (Hatch hatch in Hatchs)
		{
			foreach (string text5 in GroupTags)
			{
				if (hatch.prefix == text5)
				{
					for (int num6 = 0; num6 < hatch.pnts.Length; num6++)
					{
						hatch.pnts[num6].X = hatch.pnts[num6].X + num;
						hatch.pnts[num6].Y = hatch.pnts[num6].Y + num2;
					}
				}
			}
		}
		foreach (FillingContur filling in Fillings)
		{
			foreach (string text6 in GroupTags)
			{
				if (filling.prefix == text6)
				{
					for (int num8 = 0; num8 < filling.pnts.Length; num8++)
					{
						filling.pnts[num8].X = filling.pnts[num8].X + num;
						filling.pnts[num8].Y = filling.pnts[num8].Y + num2;
					}
				}
			}
		}
		foreach (Text text9 in Texts)
		{
			foreach (string text7 in GroupTags)
			{
				if (text9.prefix == text7)
				{
					for (int num10 = 0; num10 < text9.pnts.Length; num10++)
					{
						text9.pnts[num10].X = text9.pnts[num10].X + num;
						text9.pnts[num10].Y = text9.pnts[num10].Y + num2;
					}
				}
			}
		}
		foreach (Dimension dimension in Dimensions)
		{
			foreach (string text8 in GroupTags)
			{
				if (dimension.prefix == text8)
				{
					dimension.ep = new PointF(dimension.ep.X + num, dimension.ep.Y + num2);
					dimension.sp = new PointF(dimension.sp.X + num, dimension.sp.Y + num2);
				}
			}
		}
		NeedRecalcPoints = true;
	}

	public void ReColorGroup(Color newColor, string[] GroupTags)
	{
		foreach (Line line in Lines)
		{
			foreach (string text in GroupTags)
			{
				if (line.prefix == text)
				{
					line.color = newColor;
				}
			}
		}
		foreach (Circle circle in Circles)
		{
			foreach (string text2 in GroupTags)
			{
				if (circle.prefix == text2)
				{
					circle.color = newColor;
				}
			}
		}
		foreach (Dot dot in Dots)
		{
			foreach (string text3 in GroupTags)
			{
				if (dot.prefix == text3)
				{
					dot.color = newColor;
				}
			}
		}
		foreach (Polyline polyline in Polylines)
		{
			foreach (string text4 in GroupTags)
			{
				if (polyline.prefix == text4)
				{
					polyline.color = newColor;
				}
			}
		}
		foreach (Spline spline in Splines)
		{
			foreach (string text5 in GroupTags)
			{
				if (spline.prefix == text5)
				{
					spline.color = newColor;
				}
			}
		}
		foreach (Hatch hatch in Hatchs)
		{
			foreach (string text6 in GroupTags)
			{
				if (hatch.prefix == text6)
				{
					hatch.color = newColor;
				}
			}
		}
		foreach (FillingContur filling in Fillings)
		{
			foreach (string text7 in GroupTags)
			{
				if (filling.prefix == text7)
				{
					filling.color = newColor;
				}
			}
		}
		foreach (Text text10 in Texts)
		{
			foreach (string text8 in GroupTags)
			{
				if (text10.prefix == text8)
				{
					text10.color = newColor;
				}
			}
		}
		foreach (Dimension dimension in Dimensions)
		{
			foreach (string text9 in GroupTags)
			{
				if (dimension.prefix == text9)
				{
					dimension.color = newColor;
				}
			}
		}
	}

	public void RotateGroup(p2d bp, float angle, string[] GroupTags)
	{
		angle *= (float)Math.PI / 180f;
		foreach (Line line in Lines)
		{
			foreach (string text in GroupTags)
			{
				if (line.prefix == text)
				{
					for (int j = 0; j < line.pnts.Length; j++)
					{
						PointF pointF = new PointF
						{
							X = (line.pnts[j].X - bp.X) * (float)Math.Cos(angle) - (line.pnts[j].Y - bp.Y) * (float)Math.Sin(angle) + bp.X,
							Y = (line.pnts[j].X - bp.X) * (float)Math.Sin(angle) + (line.pnts[j].Y - bp.Y) * (float)Math.Cos(angle) + bp.Y
						};
						ref PointF reference = ref line.pnts[j];
						reference = new PointF(pointF.X, pointF.Y);
					}
				}
			}
		}
		foreach (Circle circle in Circles)
		{
			foreach (string text2 in GroupTags)
			{
				if (circle.prefix == text2)
				{
					for (int l = 0; l < circle.pnts.Length; l++)
					{
						PointF pointF2 = new PointF
						{
							X = (circle.pnts[l].X - bp.X) * (float)Math.Cos(angle) - (circle.pnts[l].Y - bp.Y) * (float)Math.Sin(angle) + bp.X,
							Y = (circle.pnts[l].X - bp.X) * (float)Math.Sin(angle) + (circle.pnts[l].Y - bp.Y) * (float)Math.Cos(angle) + bp.Y
						};
						ref PointF reference2 = ref circle.pnts[l];
						reference2 = new PointF(pointF2.X, pointF2.Y);
					}
				}
			}
		}
		foreach (Dot dot in Dots)
		{
			foreach (string text3 in GroupTags)
			{
				if (dot.prefix == text3)
				{
					for (int n = 0; n < dot.pnts.Length; n++)
					{
						PointF pointF3 = new PointF
						{
							X = (dot.pnts[n].X - bp.X) * (float)Math.Cos(angle) - (dot.pnts[n].Y - bp.Y) * (float)Math.Sin(angle) + bp.X,
							Y = (dot.pnts[n].X - bp.X) * (float)Math.Sin(angle) + (dot.pnts[n].Y - bp.Y) * (float)Math.Cos(angle) + bp.Y
						};
						ref PointF reference3 = ref dot.pnts[n];
						reference3 = new PointF(pointF3.X, pointF3.Y);
					}
				}
			}
		}
		foreach (Polyline polyline in Polylines)
		{
			foreach (string text4 in GroupTags)
			{
				if (polyline.prefix == text4)
				{
					for (int num2 = 0; num2 < polyline.pnts.Length; num2++)
					{
						PointF pointF4 = new PointF
						{
							X = (polyline.pnts[num2].X - bp.X) * (float)Math.Cos(angle) - (polyline.pnts[num2].Y - bp.Y) * (float)Math.Sin(angle) + bp.X,
							Y = (polyline.pnts[num2].X - bp.X) * (float)Math.Sin(angle) + (polyline.pnts[num2].Y - bp.Y) * (float)Math.Cos(angle) + bp.Y
						};
						ref PointF reference4 = ref polyline.pnts[num2];
						reference4 = new PointF(pointF4.X, pointF4.Y);
					}
				}
			}
		}
		foreach (Spline spline in Splines)
		{
			foreach (string text5 in GroupTags)
			{
				if (spline.prefix == text5)
				{
					for (int num4 = 0; num4 < spline.pnts.Length; num4++)
					{
						PointF pointF5 = new PointF
						{
							X = (spline.pnts[num4].X - bp.X) * (float)Math.Cos(angle) - (spline.pnts[num4].Y - bp.Y) * (float)Math.Sin(angle) + bp.X,
							Y = (spline.pnts[num4].X - bp.X) * (float)Math.Sin(angle) + (spline.pnts[num4].Y - bp.Y) * (float)Math.Cos(angle) + bp.Y
						};
						ref PointF reference5 = ref spline.pnts[num4];
						reference5 = new PointF(pointF5.X, pointF5.Y);
					}
				}
			}
		}
		foreach (Hatch hatch in Hatchs)
		{
			foreach (string text6 in GroupTags)
			{
				if (hatch.prefix == text6)
				{
					for (int num6 = 0; num6 < hatch.pnts.Length; num6++)
					{
						PointF pointF6 = new PointF
						{
							X = (hatch.pnts[num6].X - bp.X) * (float)Math.Cos(angle) - (hatch.pnts[num6].Y - bp.Y) * (float)Math.Sin(angle) + bp.X,
							Y = (hatch.pnts[num6].X - bp.X) * (float)Math.Sin(angle) + (hatch.pnts[num6].Y - bp.Y) * (float)Math.Cos(angle) + bp.Y
						};
						ref PointF reference6 = ref hatch.pnts[num6];
						reference6 = new PointF(pointF6.X, pointF6.Y);
					}
				}
			}
		}
		foreach (FillingContur filling in Fillings)
		{
			foreach (string text7 in GroupTags)
			{
				if (filling.prefix == text7)
				{
					for (int num8 = 0; num8 < filling.pnts.Length; num8++)
					{
						PointF pointF7 = new PointF
						{
							X = (filling.pnts[num8].X - bp.X) * (float)Math.Cos(angle) - (filling.pnts[num8].Y - bp.Y) * (float)Math.Sin(angle) + bp.X,
							Y = (filling.pnts[num8].X - bp.X) * (float)Math.Sin(angle) + (filling.pnts[num8].Y - bp.Y) * (float)Math.Cos(angle) + bp.Y
						};
						ref PointF reference7 = ref filling.pnts[num8];
						reference7 = new PointF(pointF7.X, pointF7.Y);
					}
				}
			}
		}
		foreach (Text text10 in Texts)
		{
			foreach (string text8 in GroupTags)
			{
				if (text10.prefix == text8)
				{
					for (int num10 = 0; num10 < text10.pnts.Length; num10++)
					{
						PointF pointF8 = new PointF
						{
							X = (text10.pnts[num10].X - bp.X) * (float)Math.Cos(angle) - (text10.pnts[num10].Y - bp.Y) * (float)Math.Sin(angle) + bp.X,
							Y = (text10.pnts[num10].X - bp.X) * (float)Math.Sin(angle) + (text10.pnts[num10].Y - bp.Y) * (float)Math.Cos(angle) + bp.Y
						};
						ref PointF reference8 = ref text10.pnts[num10];
						reference8 = new PointF(pointF8.X, pointF8.Y);
					}
				}
			}
		}
		foreach (Dimension dimension in Dimensions)
		{
			foreach (string text9 in GroupTags)
			{
				if (dimension.prefix == text9)
				{
					PointF pointF9 = new PointF
					{
						X = (dimension.sp.X - bp.X) * (float)Math.Cos(angle) - (dimension.sp.Y - bp.Y) * (float)Math.Sin(angle) + bp.X,
						Y = (dimension.sp.X - bp.X) * (float)Math.Sin(angle) + (dimension.sp.Y - bp.Y) * (float)Math.Cos(angle) + bp.Y
					};
					dimension.sp = new PointF(pointF9.X, pointF9.Y);
					pointF9.X = (dimension.ep.X - bp.X) * (float)Math.Cos(angle) - (dimension.ep.Y - bp.Y) * (float)Math.Sin(angle) + bp.X;
					pointF9.Y = (dimension.ep.X - bp.X) * (float)Math.Sin(angle) + (dimension.ep.Y - bp.Y) * (float)Math.Cos(angle) + bp.Y;
					dimension.ep = new PointF(pointF9.X, pointF9.Y);
				}
			}
		}
		NeedRecalcPoints = true;
	}

	public void FillPolylines(string GroupTag, Color fillColor, HatchStyle style)
	{
		foreach (Polyline polyline in Polylines)
		{
			if (polyline.prefix == GroupTag)
			{
				PointF[] array = new PointF[polyline.pnts.Length];
				Array.Copy(polyline.pnts, array, polyline.pnts.Length);
				Hatchs.Add(new Hatch(polyline.dashStyle, array, polyline.penWidth, polyline.color, fillColor, style, GroupTag));
			}
		}
		NeedRecalcPoints = true;
	}

	public void FillPolylines(string GroupTag, Color fillColor)
	{
		foreach (Polyline polyline in Polylines)
		{
			if (polyline.prefix == GroupTag)
			{
				PointF[] array = new PointF[polyline.pnts.Length];
				Array.Copy(polyline.pnts, array, polyline.pnts.Length);
				Fillings.Add(new FillingContur(DashStyle.Solid, array, polyline.penWidth, polyline.color, fillColor, GroupTag));
			}
		}
		NeedRecalcPoints = true;
	}

	public void Redraw()
	{
		clearTrash();
		NeedRecalcPoints = true;
		Invalidate();
	}

	public void clearTrash()
	{
		for (int i = 0; i < Fillings.Count; i++)
		{
			if (Fillings[i].pnts.Length == 0)
			{
				Fillings.RemoveAt(i);
				i = 0;
			}
		}
		for (int j = 0; j < Splines.Count; j++)
		{
			if (Splines[j].pnts.Length == 0)
			{
				Splines.RemoveAt(j);
				j = 0;
			}
		}
		for (int k = 0; k < Hatchs.Count; k++)
		{
			if (Hatchs[k].pnts.Length == 0)
			{
				Hatchs.RemoveAt(k);
				k = 0;
			}
		}
		for (int l = 0; l < Polylines.Count; l++)
		{
			if (Polylines[l].pnts.Length == 0)
			{
				Polylines.RemoveAt(l);
				l = 0;
			}
		}
		for (int m = 0; m < Texts.Count; m++)
		{
			if (Texts[m].pnts.Length == 0)
			{
				Texts.RemoveAt(m);
				m = 0;
			}
		}
		for (int n = 0; n < Dots.Count; n++)
		{
			if (Dots[n].pnts.Length == 0)
			{
				Dots.RemoveAt(n);
				n = 0;
			}
		}
		for (int num = 0; num < Lines.Count; num++)
		{
			if (Lines[num].pnts.Length == 0)
			{
				Lines.RemoveAt(num);
				num = 0;
			}
		}
		for (int num2 = 0; num2 < Circles.Count; num2++)
		{
			if (Circles[num2].pnts.Length == 0)
			{
				Circles.RemoveAt(num2);
				num2 = 0;
			}
		}
	}

	public void ClearDB()
	{
		Lines.Clear();
		Polylines.Clear();
		Hatchs.Clear();
		Texts.Clear();
		Dots.Clear();
		Splines.Clear();
		Dimensions.Clear();
		Fillings.Clear();
		Circles.Clear();
		Redraw();
	}

	public void ClearDB(string TagSubstring)
	{
		for (int i = 0; i < Lines.Count; i++)
		{
			if (Lines[i].prefix.IndexOf(TagSubstring) >= 0)
			{
				Lines.RemoveAt(i);
				i = -1;
			}
		}
		for (int j = 0; j < Polylines.Count; j++)
		{
			if (Polylines[j].prefix.IndexOf(TagSubstring) >= 0)
			{
				Polylines.RemoveAt(j);
				j = -1;
			}
		}
		for (int k = 0; k < Hatchs.Count; k++)
		{
			if (Hatchs[k].prefix.IndexOf(TagSubstring) >= 0)
			{
				Hatchs.RemoveAt(k);
				k = -1;
			}
		}
		for (int l = 0; l < Texts.Count; l++)
		{
			if (Texts[l].prefix.IndexOf(TagSubstring) >= 0)
			{
				Texts.RemoveAt(l);
				l = -1;
			}
		}
		for (int m = 0; m < Dots.Count; m++)
		{
			if (Dots[m].prefix.IndexOf(TagSubstring) >= 0)
			{
				Dots.RemoveAt(m);
				m = -1;
			}
		}
		for (int n = 0; n < Splines.Count; n++)
		{
			if (Splines[n].prefix.IndexOf(TagSubstring) >= 0)
			{
				Splines.RemoveAt(n);
				n = -1;
			}
		}
		for (int num = 0; num < Dimensions.Count; num++)
		{
			if (Dimensions[num].prefix.IndexOf(TagSubstring) >= 0)
			{
				Dimensions.RemoveAt(num);
				num = -1;
			}
		}
		for (int num2 = 0; num2 < Fillings.Count; num2++)
		{
			if (Fillings[num2].prefix.IndexOf(TagSubstring) >= 0)
			{
				Fillings.RemoveAt(num2);
				num2 = -1;
			}
		}
		for (int num3 = 0; num3 < Circles.Count; num3++)
		{
			if (Circles[num3].prefix.IndexOf(TagSubstring) >= 0)
			{
				Circles.RemoveAt(num3);
				num3 = -1;
			}
		}
		Redraw();
	}

	public void ClearDBExactly(string Tag)
	{
		for (int i = 0; i < Lines.Count; i++)
		{
			if (Lines[i].prefix == Tag)
			{
				Lines.RemoveAt(i);
				i = -1;
			}
		}
		for (int j = 0; j < Polylines.Count; j++)
		{
			if (Polylines[j].prefix == Tag)
			{
				Polylines.RemoveAt(j);
				j = -1;
			}
		}
		for (int k = 0; k < Hatchs.Count; k++)
		{
			if (Hatchs[k].prefix == Tag)
			{
				Hatchs.RemoveAt(k);
				k = -1;
			}
		}
		for (int l = 0; l < Texts.Count; l++)
		{
			if (Texts[l].prefix == Tag)
			{
				Texts.RemoveAt(l);
				l = -1;
			}
		}
		for (int m = 0; m < Dots.Count; m++)
		{
			if (Dots[m].prefix == Tag)
			{
				Dots.RemoveAt(m);
				m = -1;
			}
		}
		for (int n = 0; n < Splines.Count; n++)
		{
			if (Splines[n].prefix == Tag)
			{
				Splines.RemoveAt(n);
				n = -1;
			}
		}
		for (int num = 0; num < Dimensions.Count; num++)
		{
			if (Dimensions[num].prefix == Tag)
			{
				Dimensions.RemoveAt(num);
				num = -1;
			}
		}
		for (int num2 = 0; num2 < Fillings.Count; num2++)
		{
			if (Fillings[num2].prefix == Tag)
			{
				Fillings.RemoveAt(num2);
				num2 = -1;
			}
		}
		for (int num3 = 0; num3 < Circles.Count; num3++)
		{
			if (Circles[num3].prefix == Tag)
			{
				Circles.RemoveAt(num3);
				num3 = -1;
			}
		}
		Redraw();
	}

	public bool IsEmpty()
	{
		bool result = true;
		if (Lines.Count > 0)
		{
			result = false;
		}
		if (Texts.Count > 0)
		{
			result = false;
		}
		if (Polylines.Count > 0)
		{
			result = false;
		}
		if (Hatchs.Count > 0)
		{
			result = false;
		}
		if (Splines.Count > 0)
		{
			result = false;
		}
		if (Dimensions.Count > 0)
		{
			result = false;
		}
		if (Fillings.Count > 0)
		{
			result = false;
		}
		if (Circles.Count > 0)
		{
			result = false;
		}
		return result;
	}

	private void RescaleToFit()
	{
		try
		{
			List<p2d> list = new List<p2d>();
			foreach (Line line in Lines)
			{
				try
				{
					for (int i = 0; i < line.pnts.Length; i++)
					{
						list.Add(new p2d(line.pnts[i].X * ScaleX, line.pnts[i].Y * ScaleY));
					}
				}
				catch
				{
				}
			}
			foreach (Circle circle in Circles)
			{
				try
				{
					for (int j = 0; j < circle.pnts.Length; j++)
					{
						list.Add(new p2d(circle.pnts[j].X * ScaleX, circle.pnts[j].Y * ScaleY));
					}
				}
				catch
				{
				}
			}
			foreach (Dot dot in Dots)
			{
				try
				{
					for (int k = 0; k < dot.pnts.Length; k++)
					{
						list.Add(new p2d(dot.pnts[k].X * ScaleX, dot.pnts[k].Y * ScaleY));
					}
				}
				catch
				{
				}
			}
			foreach (Polyline polyline in Polylines)
			{
				try
				{
					for (int l = 0; l < polyline.pnts.Length; l++)
					{
						list.Add(new p2d(polyline.pnts[l].X * ScaleX, polyline.pnts[l].Y * ScaleY));
					}
				}
				catch
				{
				}
			}
			foreach (FillingContur filling in Fillings)
			{
				try
				{
					for (int m = 0; m < filling.pnts.Length; m++)
					{
						list.Add(new p2d(filling.pnts[m].X * ScaleX, filling.pnts[m].Y * ScaleY));
					}
				}
				catch
				{
				}
			}
			foreach (Spline spline in Splines)
			{
				try
				{
					for (int n = 0; n < spline.pnts.Length; n++)
					{
						list.Add(new p2d(spline.pnts[n].X * ScaleX, spline.pnts[n].Y * ScaleY));
					}
				}
				catch
				{
				}
			}
			foreach (Hatch hatch in Hatchs)
			{
				try
				{
					for (int num = 0; num < hatch.pnts.Length; num++)
					{
						list.Add(new p2d(hatch.pnts[num].X * ScaleX, hatch.pnts[num].Y * ScaleY));
					}
				}
				catch
				{
				}
			}
			if (list.Count > 0)
			{
				new Contur(list);
				p2d p2d2 = Mathematics.GetBoundRect(STEB2.Converting.Converting.ListToArray(list))[0];
				p2d p2d3 = Mathematics.GetBoundRect(STEB2.Converting.Converting.ListToArray(list))[2];
				float num2 = p2d3.X - p2d2.X;
				float num3 = p2d3.Y - p2d2.Y;
				new PointF(num2 / 2f, num3 / 2f);
				float num4 = (float)pictureBox_ModelFrame.Width / num2;
				float num5 = (float)pictureBox_ModelFrame.Height / num3;
				if (num4 <= num5)
				{
					_screenScale = (float)pictureBox_ModelFrame.Width / num2;
					_screenCenter.X = (0f - num2) / 2f + (num2 / 2f - p2d2.X);
					_screenCenter.Y = num3 / 2f + p2d2.Y + (float)(pictureBox_ModelFrame.Height / 2) / screenScale;
				}
				if (num4 > num5)
				{
					_screenScale = (float)pictureBox_ModelFrame.Height / num3;
					_screenCenter.X = (0f - num2) / 2f + (num2 / 2f - p2d2.X) + (float)(pictureBox_ModelFrame.Width / 2) / screenScale - num2 / 2f;
					_screenCenter.Y = num3 / 2f + p2d2.Y + (float)(pictureBox_ModelFrame.Height / 2) / screenScale;
				}
			}
		}
		catch
		{
		}
	}

	public void ZoomAll()
	{
		RescaleToFit();
		Redraw();
	}

	private void DrawCenterOrigin(Graphics g)
	{
		using Pen pen = new Pen(Brushes.Blue, 1f);
		pen.DashStyle = DashStyle.DashDot;
		g.DrawLine(pen, screenCenter.X * screenScale, screenCenter.Y * screenScale, screenCenter.X * screenScale, pictureBox_ModelFrame.Height);
		g.DrawLine(pen, screenCenter.X * screenScale, screenCenter.Y * screenScale, screenCenter.X * screenScale, -pictureBox_ModelFrame.Height);
		g.DrawLine(pen, screenCenter.X * screenScale, screenCenter.Y * screenScale, pictureBox_ModelFrame.Width, screenCenter.Y * screenScale);
		g.DrawLine(pen, screenCenter.X * screenScale, screenCenter.Y * screenScale, -pictureBox_ModelFrame.Width, screenCenter.Y * screenScale);
	}

	private PointF getCurPosInModel()
	{
		Point point = PointToClient(CursorPosition);
		PointF result = new PointF((float)point.X / screenScale - screenCenter.X, (float)(point.Y - toolStrip1.Height) / screenScale - screenCenter.Y);
		result.Y *= -1f;
		return result;
	}

	private PointF[] ScalePoints(PointF[] inputPoints)
	{
		PointF[] array = new PointF[inputPoints.Length];
		for (int i = 0; i < inputPoints.Length; i++)
		{
			ref PointF reference = ref array[i];
			reference = new PointF(inputPoints[i].X * ScaleX, inputPoints[i].Y * ScaleY);
		}
		return array;
	}
}
