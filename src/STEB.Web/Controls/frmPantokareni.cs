using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Drawing;
using System.Windows.Forms;
using STEB2;
using STEB2.Converting;
using STEB2.DataBase;
using STEB2.GraphicsElement;
using STEB2.Stability;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB_LOAD;

public class frmPantokareni : Form
{

	private ToolStripLabel Pantokareni_transvStartAngleLabel;

	private ToolStripTextBox Pantokareni_transvStartAngle;

	private ToolStripLabel Pantokareni_transvEndAngleLabel;

	private ToolStripTextBox Pantokareni_transvEndAngle;

	private ToolStripLabel Pantokareni_transvStepAngleLabel;

	private ToolStripTextBox Pantokareni_transvStepAngle;

	private ToolStripLabel Pantokareni_transvStartVolumeLabel;

	private ToolStripTextBox Pantokareni_transvStartVolume;

	private ToolStripLabel Pantokareni_transvEndVolumeLabel;

	private ToolStripTextBox Pantokareni_transvEndVolume;

	private ToolStripLabel Pantokareni_transvStepVolumeLabel;

	private ToolStripTextBox Pantokareni_transvStepVolume;

	private ToolStripButton Pantokareni_button;

	private usgModelFrame Pantokareni_modelFrame;

	private float aquaDensity;

	private List<Station> stations;

	private void InitializeComponent()
	{
		Text = "frmPantokareni";
	}

	public frmPantokareni(float aquaDensity, List<Station> stations)
	{
		InitializeComponent();
		this.aquaDensity = aquaDensity;
		this.stations = stations;
		Padding = new Padding(10);
		Text = "Плечи остойчивости формы:";
		Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
		Pantokareni_transvStartAngleLabel = new ToolStripLabel("Начальный угол крена, градусы:");
		Pantokareni_transvStartAngle = new ToolStripTextBox();
		Pantokareni_transvEndAngleLabel = new ToolStripLabel("Конечный угол крена, градусы:");
		Pantokareni_transvEndAngle = new ToolStripTextBox();
		Pantokareni_transvStepAngleLabel = new ToolStripLabel("Шаг углов крена, градусы:");
		Pantokareni_transvStepAngle = new ToolStripTextBox();
		Pantokareni_transvStartVolumeLabel = new ToolStripLabel("Начальное водоизмещение, т:");
		Pantokareni_transvStartVolume = new ToolStripTextBox();
		Pantokareni_transvEndVolumeLabel = new ToolStripLabel("Конечное водоизмешение, т:");
		Pantokareni_transvEndVolume = new ToolStripTextBox();
		Pantokareni_transvStepVolumeLabel = new ToolStripLabel("Шаг водоизмещений, т:");
		Pantokareni_transvStepVolume = new ToolStripTextBox();
		Pantokareni_button = new ToolStripButton("Расчет");
		Pantokareni_button.DisplayStyle = ToolStripItemDisplayStyle.Text;
		Pantokareni_button.Font = new Font(Pantokareni_button.Font, FontStyle.Bold);
		ToolStrip toolStrip = new ToolStrip(Pantokareni_transvStartAngleLabel, Pantokareni_transvStartAngle, Pantokareni_transvEndAngleLabel, Pantokareni_transvEndAngle, Pantokareni_transvStepAngleLabel, Pantokareni_transvStepAngle);
		ToolStrip toolStrip2 = new ToolStrip(Pantokareni_transvStartVolumeLabel, Pantokareni_transvStartVolume, Pantokareni_transvEndVolumeLabel, Pantokareni_transvEndVolume, Pantokareni_transvStepVolumeLabel, Pantokareni_transvStepVolume);
		ToolStrip toolStrip3 = new ToolStrip(Pantokareni_button);
		toolStrip.LayoutStyle = ToolStripLayoutStyle.Flow;
		toolStrip2.LayoutStyle = ToolStripLayoutStyle.Flow;
		toolStrip3.LayoutStyle = ToolStripLayoutStyle.Flow;
		Pantokareni_modelFrame = new usgModelFrame();
		Pantokareni_modelFrame.BackColor = Color.White;
		Pantokareni_modelFrame.ModelFrameCaption = "Пантокарены";
		Pantokareni_modelFrame.Dock = DockStyle.Fill;
		Controls.AddRange(new Control[4] { toolStrip, toolStrip2, toolStrip3, Pantokareni_modelFrame });
		Pantokareni_modelFrame.BringToFront();
		using (JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile))
		{
			oleDbConnection.Open();
			ToolStripTextBox toolStripTextBox_FontSize = Pantokareni_modelFrame.toolStripTextBox_FontSize;
			string text = (Pantokareni_modelFrame.toolStripTextBox_FontSize.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "textHeight", oleDbConnection).ToString()).ToString());
			toolStripTextBox_FontSize.Text = text;
			Pantokareni_modelFrame.toolStripTextBox_ScaleX.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "PantokareniTransvStaticDiagrammXScale", oleDbConnection).ToString()).ToString();
			Pantokareni_modelFrame.toolStripTextBox_ScaleY.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "PantokareniTransvStaticDiagrammYScale", oleDbConnection).ToString()).ToString();
			Pantokareni_transvStartAngle.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "Pantokareni_transvStartAngle", oleDbConnection).ToString()).ToString();
			Pantokareni_transvEndAngle.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "Pantokareni_transvEndAngle", oleDbConnection).ToString()).ToString();
			Pantokareni_transvStepAngle.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "Pantokareni_transvStepAngle", oleDbConnection).ToString()).ToString();
			Pantokareni_transvStartVolume.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "Pantokareni_transvStartVolume", oleDbConnection).ToString()).ToString();
			Pantokareni_transvEndVolume.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "Pantokareni_transvEndVolume", oleDbConnection).ToString()).ToString();
			Pantokareni_transvStepVolume.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "Pantokareni_transvStepVolume", oleDbConnection).ToString()).ToString();
			oleDbConnection.Close();
		}
		Pantokareni_transvEndAngle.TextChanged += PantokareniParameterChanged;
		Pantokareni_transvEndVolume.TextChanged += PantokareniParameterChanged;
		Pantokareni_transvStartAngle.TextChanged += PantokareniParameterChanged;
		Pantokareni_transvStartVolume.TextChanged += PantokareniParameterChanged;
		Pantokareni_transvStepAngle.TextChanged += PantokareniParameterChanged;
		Pantokareni_transvStepVolume.TextChanged += PantokareniParameterChanged;
		Pantokareni_modelFrame.toolStripTextBox_ScaleX.TextChanged += PantokareniParameterChanged;
		Pantokareni_modelFrame.toolStripTextBox_ScaleY.TextChanged += PantokareniParameterChanged;
		Pantokareni_modelFrame.toolStripTextBox_FontSize.TextChanged += PantokareniParameterChanged;
		Pantokareni_button.Click += Pantokareni_button_Click;
		Show();
	}

	public void Pantokareni_button_Click(object sender, EventArgs e)
	{
		using (new frmPleaseWait())
		{
			try
			{
				List<StabilityCurve> curvesArmsTransvStabilityForms = STEB2.Volume.Tools.GetCurvesArmsTransvStabilityForms(Converting.TextToFloat(Pantokareni_transvStartVolume.Text), Converting.TextToFloat(Pantokareni_transvEndVolume.Text), Converting.TextToFloat(Pantokareni_transvStepVolume.Text), Converting.TextToFloat(Pantokareni_transvStartAngle.Text), Converting.TextToFloat(Pantokareni_transvEndAngle.Text), Converting.TextToFloat(Pantokareni_transvStepAngle.Text), aquaDensity, 0.001f, stations);
				DrawProjectElements.DrawPantokarens(Pantokareni_modelFrame, curvesArmsTransvStabilityForms, "pantokareni");
			}
			catch
			{
			}
		}
	}

	public void PantokareniParameterChanged(object sender, EventArgs e)
	{
		try
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "PantokareniTransvStaticDiagrammXScale", Converting.TextToFloat(Pantokareni_modelFrame.toolStripTextBox_ScaleX.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "PantokareniTransvStaticDiagrammYScale", Converting.TextToFloat(Pantokareni_modelFrame.toolStripTextBox_ScaleY.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "Pantokareni_transvStartAngle", Converting.TextToFloat(Pantokareni_transvStartAngle.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "Pantokareni_transvEndAngle", Converting.TextToFloat(Pantokareni_transvEndAngle.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "Pantokareni_transvStepAngle", Converting.TextToFloat(Pantokareni_transvStepAngle.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "Pantokareni_transvStartVolume", Converting.TextToFloat(Pantokareni_transvStartVolume.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "Pantokareni_transvEndVolume", Converting.TextToFloat(Pantokareni_transvEndVolume.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "Pantokareni_transvStepVolume", Converting.TextToFloat(Pantokareni_transvStepVolume.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "textHeight", Converting.TextToFloat(Pantokareni_modelFrame.toolStripTextBox_FontSize.Text), oleDbConnection);
			oleDbConnection.Close();
		}
		catch
		{
		}
	}
}
