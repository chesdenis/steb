using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using STEB2;
using STEB2.Converting;
using STEB2.DataBase;
using STEB2.GraphicsElement;
using STEB2.Stability;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB_LOAD;

public class frmDiagrammXcXf : Form
{
	private ToolStripLabel StartDraftLabel;

	private ToolStripTextBox StartDraft;

	private ToolStripLabel EndDraftLabel;

	private ToolStripTextBox EndDraft;

	private ToolStripLabel StepDraftLabel;

	private ToolStripTextBox StepDraft;

	private ToolStripButton buttonCalc;

	private ToolStripButton buttonReport;

	private usgModelFrame modelFrame;

	private float aquaDensity;

	private List<Station> stations;

	public frmDiagrammXcXf(float aquaDensity, List<Station> stations)
	{
		InitializeComponent();
		this.aquaDensity = aquaDensity;
		this.stations = stations;
		Padding = new Padding(10);
		StartDraftLabel = new ToolStripLabel("Начальная осадка, м:");
		StartDraft = new ToolStripTextBox();
		EndDraftLabel = new ToolStripLabel("Конечная осадка, м:");
		EndDraft = new ToolStripTextBox();
		StepDraftLabel = new ToolStripLabel("Шаг осадок, м:");
		StepDraft = new ToolStripTextBox();
		buttonCalc = new ToolStripButton("Расчет");
		buttonCalc.DisplayStyle = ToolStripItemDisplayStyle.Text;
		buttonReport = new ToolStripButton("Отчет");
		buttonReport.DisplayStyle = ToolStripItemDisplayStyle.Text;
		buttonCalc.Font = new Font(buttonCalc.Font, FontStyle.Bold);
		buttonReport.Font = new Font(buttonReport.Font, FontStyle.Bold);
		ToolStrip toolStrip = new ToolStrip(StartDraftLabel, StartDraft, EndDraftLabel, EndDraft, StepDraftLabel, StepDraft);
		ToolStrip toolStrip2 = new ToolStrip(buttonCalc, buttonReport);
		toolStrip.LayoutStyle = ToolStripLayoutStyle.Flow;
		toolStrip2.LayoutStyle = ToolStripLayoutStyle.Flow;
		modelFrame = new usgModelFrame();
		modelFrame.BackColor = Color.White;
		modelFrame.ModelFrameCaption = "Абсцисса центра величины и абсцисса центра тяжести ватерлинии";
		modelFrame.Dock = DockStyle.Fill;
		Controls.AddRange(new Control[3] { toolStrip, toolStrip2, modelFrame });
		modelFrame.BringToFront();
		using (JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile))
		{
			oleDbConnection.Open();
			ToolStripTextBox toolStripTextBox_FontSize = modelFrame.toolStripTextBox_FontSize;
			string text = (modelFrame.toolStripTextBox_FontSize.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "textHeight", oleDbConnection).ToString()).ToString());
			toolStripTextBox_FontSize.Text = text;
			modelFrame.toolStripTextBox_ScaleX.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "XcXf_DiagrammXScale", oleDbConnection).ToString()).ToString();
			modelFrame.toolStripTextBox_ScaleY.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "XcXf_DiagrammYScale", oleDbConnection).ToString()).ToString();
			StartDraft.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "XcXf_StartDraft", oleDbConnection).ToString()).ToString();
			EndDraft.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "XcXf_EndDraft", oleDbConnection).ToString()).ToString();
			StepDraft.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "XcXf_StepDraft", oleDbConnection).ToString()).ToString();
			oleDbConnection.Close();
		}
		StartDraft.TextChanged += ParameterChanged;
		EndDraft.TextChanged += ParameterChanged;
		StepDraft.TextChanged += ParameterChanged;
		modelFrame.toolStripTextBox_ScaleX.TextChanged += ParameterChanged;
		modelFrame.toolStripTextBox_ScaleY.TextChanged += ParameterChanged;
		modelFrame.toolStripTextBox_FontSize.TextChanged += ParameterChanged;
		modelFrame.Click += ParameterChanged;
		buttonCalc.Click += buttonCalc_Click;
		buttonReport.Click += buttonReport_Click;
		Show();
	}

	public void buttonReport_Click(object sender, EventArgs e)
	{
		try
		{
			Form form = new Form();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmControlPanel));
			form.Padding = new Padding(10);
			ReportControl reportControl = new ReportControl();
			form.Controls.Add(reportControl);
			form.Icon = (Icon)null;
			reportControl.Dock = DockStyle.Fill;
			form.Text = "Отчет " + Text;
			form.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
			form.Show();
			List<StabilityCurve> hydroCurves = STEB2.Volume.Tools.GetHydroCurves(Converting.TextToFloat(StartDraft.Text), Converting.TextToFloat(EndDraft.Text), Converting.TextToFloat(StepDraft.Text), aquaDensity, 0.001f, stations, STEB2.Volume.Tools.HydroCurvesType.XcXf);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<html>");
			stringBuilder.AppendLine("<body>");
			stringBuilder.AppendLine("<p>Таблица - " + Text);
			stringBuilder.AppendLine("<table border=1 width=100%>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Осадка, м");
			stringBuilder.AppendLine("<td>Xc, м");
			stringBuilder.AppendLine("<td>Xf, м");
			stringBuilder.AppendLine("</tr>");
			for (int i = 0; i < hydroCurves[0].Points.Count; i++)
			{
				stringBuilder.AppendLine("<tr>");
				stringBuilder.AppendLine("<td>" + Math.Round(hydroCurves[0].Points[i].Y, 3));
				stringBuilder.AppendLine("<td>" + Math.Round(hydroCurves[0].Points[i].X, 3));
				stringBuilder.AppendLine("<td>" + Math.Round(hydroCurves[1].Points[i].X, 3));
				stringBuilder.AppendLine("</tr>");
			}
			stringBuilder.AppendLine("</table>");
			if (modelFrame.IsEmpty())
			{
				buttonCalc_Click(new object(), new EventArgs());
				modelFrame.Redraw();
				modelFrame.ZoomAll();
				modelFrame.SaveImage(Application.StartupPath + "\\xcxf_img1.bmp");
			}
			else
			{
				modelFrame.SaveImage(Application.StartupPath + "\\xcxf_img1.bmp");
			}
			stringBuilder.AppendLine("<br><img src=\"" + Application.StartupPath + "\\xcxf_img1.bmp\" align=center style=\"margin:1em 1em 1em 1em\">");
			stringBuilder.AppendLine("<p align = \"center\">Рис.1 Диаграмма Xc и Xf");
			stringBuilder.AppendLine("</body>");
			stringBuilder.AppendLine("</html>");
			using (StreamWriter streamWriter = new StreamWriter(Application.StartupPath + "\\xcxf_table.html", append: false, Encoding.Default))
			{
				streamWriter.Write(stringBuilder.ToString());
				streamWriter.Close();
			}
			reportControl.Navigate(Application.StartupPath + "\\xcxf_table.html");
		}
		catch
		{
		}
	}

	public void buttonCalc_Click(object sender, EventArgs e)
	{
		using (new frmPleaseWait())
		{
			try
			{
				List<StabilityCurve> hydroCurves = STEB2.Volume.Tools.GetHydroCurves(Converting.TextToFloat(StartDraft.Text), Converting.TextToFloat(EndDraft.Text), Converting.TextToFloat(StepDraft.Text), aquaDensity, 0.001f, stations, STEB2.Volume.Tools.HydroCurvesType.XcXf);
				DrawProjectElements.DrawCurves(modelFrame, hydroCurves, "hydrocurves");
			}
			catch
			{
			}
		}
	}

	public void ParameterChanged(object sender, EventArgs e)
	{
		try
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "XcXf_DiagrammXScale", Converting.TextToFloat(modelFrame.toolStripTextBox_ScaleX.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "XcXf_DiagrammYScale", Converting.TextToFloat(modelFrame.toolStripTextBox_ScaleY.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "XcXf_StartDraft", Converting.TextToFloat(StartDraft.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "XcXf_EndDraft", Converting.TextToFloat(EndDraft.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "XcXf_StepDraft", Converting.TextToFloat(StepDraft.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "textHeight", Converting.TextToFloat(modelFrame.toolStripTextBox_FontSize.Text), oleDbConnection);
			oleDbConnection.Close();
		}
		catch
		{
		}
	}

	private void InitializeComponent()
	{
		Text = "Диаграмма Xc и Xf";
	}
}
