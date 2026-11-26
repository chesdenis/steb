using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Drawing;
using System.Windows.Forms;
using STEB2;
using STEB2.Cisterns;
using STEB2.Compartments;
using STEB2.Containers;
using STEB2.Converting;
using STEB2.DataBase;
using STEB2.Geometry;
using STEB2.GraphicsElement;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB_LOAD;

public class frmDiagrammStability : Form
{
	private enum diagrammType
	{
		longDiagramm,
		transvDiagramm
	}

	public float aquaDensity;

	public float BaseMass;

	public p3d BaseGPos;

	public List<Station> stations;

	public List<Compartment> compartments;

	public List<Cistern> cisterns;

	public List<STEB2.Containers.Container> containers;

	public usgModelFrame usgModelFrame_transvStabDiagramm;

	public usgModelFrame usgModelFrame_longStabDiagramm;

	public CheckBox checkBox_isDrawDDOForTransvStab;

	public CheckBox checkBox_isUsingCompartmentsForTransvStab;

	public CheckBox checkBox_isUsingCisternsForTransvStab;

	public CheckBox checkBox_isUsingContainersForTransvStab;

	public CheckBox checkBox_isDrawDDOForLongStab;

	public CheckBox checkBox_isUsingCompartmentsForLongStab;

	public CheckBox checkBox_isUsingCisternsForLongStab;

	public CheckBox checkBox_isUsingContainersForLongStab;

	public usgNumberField usgNumberField_transvEndAngle;

	public Label label2;

	public usgNumberField usgNumberField_transvStartAngle;

	public Label label1;

	public usgNumberField usgNumberField_transvStepAngle;

	public Label label3;

	public usgNumberField usgNumberField_longStepAngle;

	public Label label4;

	public usgNumberField usgNumberField_longEndAngle;

	public Label label5;

	public usgNumberField usgNumberField_longStartAngle;

	public Label label6;

	public Button button_calc_transvStabDiagramm;

	public Button button_calc_longStabDiagramm;

	public Button button_report_transvStabDiagramm;

	public Button button_report_longStabDiagramm;

	public frmDiagrammStability(List<Station> stations, List<Compartment> compartments, List<Cistern> cisterns, List<STEB2.Containers.Container> containers, float BaseMass, p3d BaseGPos, float aquaDensity)
	{
		this.stations = stations;
		this.compartments = compartments;
		this.cisterns = cisterns;
		this.containers = containers;
		this.BaseMass = BaseMass;
		this.BaseGPos = BaseGPos;
		this.aquaDensity = aquaDensity;
		InitializeComponent();
		LoadStartParameter();
		usgNumberField_transvStartAngle.TextChanged += stabilityParameterChanged;
		usgNumberField_transvEndAngle.TextChanged += stabilityParameterChanged;
		usgNumberField_transvStepAngle.TextChanged += stabilityParameterChanged;
		usgNumberField_longStartAngle.TextChanged += stabilityParameterChanged;
		usgNumberField_longEndAngle.TextChanged += stabilityParameterChanged;
		usgNumberField_longStepAngle.TextChanged += stabilityParameterChanged;
		usgModelFrame_transvStabDiagramm.toolStripTextBox_ScaleX.TextChanged += stabilityParameterChanged;
		usgModelFrame_transvStabDiagramm.toolStripTextBox_ScaleY.TextChanged += stabilityParameterChanged;
		usgModelFrame_longStabDiagramm.toolStripTextBox_ScaleX.TextChanged += stabilityParameterChanged;
		usgModelFrame_longStabDiagramm.toolStripTextBox_ScaleY.TextChanged += stabilityParameterChanged;
		usgModelFrame_transvStabDiagramm.toolStripTextBox_FontSize.TextChanged += stabilityParameterChanged;
		usgModelFrame_longStabDiagramm.toolStripTextBox_FontSize.TextChanged += stabilityParameterChanged;
		button_calc_transvStabDiagramm.Click += button_calc_transvStabDiagramm_Click;
		button_calc_longStabDiagramm.Click += button_calc_longStabDiagramm_Click;
		button_report_transvStabDiagramm.Click += button_report_transvStabDiagramm_Click;
		button_report_longStabDiagramm.Click += button_report_longStabDiagramm_Click;
		Show();
	}

	public void button_report_longStabDiagramm_Click(object sender, EventArgs e)
	{
	}

	public void button_report_transvStabDiagramm_Click(object sender, EventArgs e)
	{
	}

	public void button_calc_longStabDiagramm_Click(object sender, EventArgs e)
	{
		RedrawStabilityDiagramms(diagrammType.longDiagramm);
	}

	public void button_calc_transvStabDiagramm_Click(object sender, EventArgs e)
	{
		RedrawStabilityDiagramms(diagrammType.transvDiagramm);
	}

	public void stabilityParameterChanged(object sender, EventArgs e)
	{
		try
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "transvStaticDiagrammStartAngle", Converting.TextToFloat(usgNumberField_transvStartAngle.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "transvStaticDiagrammEndAngle", Converting.TextToFloat(usgNumberField_transvEndAngle.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "transvStaticDiagrammStepAngle", Converting.TextToFloat(usgNumberField_transvStepAngle.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "transvStaticDiagrammXScale", Converting.TextToFloat(usgModelFrame_transvStabDiagramm.toolStripTextBox_ScaleX.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "transvStaticDiagrammYScale", Converting.TextToFloat(usgModelFrame_transvStabDiagramm.toolStripTextBox_ScaleY.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "longStaticDiagrammStartAngle", Converting.TextToFloat(usgNumberField_longStartAngle.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "longStaticDiagrammEndAngle", Converting.TextToFloat(usgNumberField_longEndAngle.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "longStaticDiagrammStepAngle", Converting.TextToFloat(usgNumberField_longStepAngle.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "longStaticDiagrammXScale", Converting.TextToFloat(usgModelFrame_longStabDiagramm.toolStripTextBox_ScaleX.Text), oleDbConnection);
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "longStaticDiagrammYScale", Converting.TextToFloat(usgModelFrame_longStabDiagramm.toolStripTextBox_ScaleY.Text), oleDbConnection);
			oleDbConnection.Close();
		}
		catch
		{
		}
	}

	private void ReportStabilityDiagramms(diagrammType type)
	{
		using (new frmPleaseWait())
		{
			try
			{
				switch (type)
				{
				case diagrammType.longDiagramm:
				{
					List<Compartment> list = (checkBox_isUsingCompartmentsForTransvStab.Checked ? compartments : new List<Compartment>());
					List<Cistern> list2 = (checkBox_isUsingCisternsForTransvStab.Checked ? cisterns : new List<Cistern>());
					List<STEB2.Containers.Container> list3 = (checkBox_isUsingContainersForTransvStab.Checked ? containers : new List<STEB2.Containers.Container>());
					Waterlanding_MassGpos_Compartments_Containers_Cisterns waterlanding_MassGpos_Compartments_Containers_Cisterns = new Waterlanding_MassGpos_Compartments_Containers_Cisterns(BaseMass, BaseGPos, list, list3, list2, stations, 0.001f, aquaDensity);
					waterlanding_MassGpos_Compartments_Containers_Cisterns.GetDStability();
					try
					{
						Form form = new Form();
						ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmControlPanel));
						form.Padding = new Padding(10);
						ReportControl reportControl = new ReportControl();
						form.Controls.Add(reportControl);
						form.Icon = (Icon)null;
						reportControl.Dock = DockStyle.Fill;
						form.Text = "Отчет Диаграмма поперечной статической остойчивости";
						form.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
						form.Show();
						break;
					}
					catch
					{
						break;
					}
				}
				case diagrammType.transvDiagramm:
				{
					List<Compartment> list = (checkBox_isUsingCompartmentsForTransvStab.Checked ? compartments : new List<Compartment>());
					List<Cistern> list2 = (checkBox_isUsingCisternsForTransvStab.Checked ? cisterns : new List<Cistern>());
					List<STEB2.Containers.Container> list3 = (checkBox_isUsingContainersForTransvStab.Checked ? containers : new List<STEB2.Containers.Container>());
					Waterlanding_MassGpos_Compartments_Containers_Cisterns waterlanding_MassGpos_Compartments_Containers_Cisterns = new Waterlanding_MassGpos_Compartments_Containers_Cisterns(BaseMass, BaseGPos, list, list3, list2, stations, 0.001f, aquaDensity);
					waterlanding_MassGpos_Compartments_Containers_Cisterns.GetDStability();
					try
					{
						Form form = new Form();
						ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmControlPanel));
						form.Padding = new Padding(10);
						ReportControl reportControl = new ReportControl();
						form.Controls.Add(reportControl);
						form.Icon = (Icon)null;
						reportControl.Dock = DockStyle.Fill;
						form.Text = "Отчет Диаграмма продольной статической остойчивости";
						form.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
						form.Show();
						break;
					}
					catch
					{
						break;
					}
				}
				}
			}
			catch
			{
			}
		}
	}

	private void RedrawStabilityDiagramms(diagrammType type)
	{
		using (new frmPleaseWait())
		{
			try
			{
				switch (type)
				{
				case diagrammType.longDiagramm:
				{
					List<Compartment> list = (checkBox_isUsingCompartmentsForTransvStab.Checked ? compartments : new List<Compartment>());
					List<Cistern> list2 = (checkBox_isUsingCisternsForTransvStab.Checked ? cisterns : new List<Cistern>());
					List<STEB2.Containers.Container> list3 = (checkBox_isUsingContainersForTransvStab.Checked ? containers : new List<STEB2.Containers.Container>());
					Waterlanding_MassGpos_Compartments_Containers_Cisterns waterlanding_MassGpos_Compartments_Containers_Cisterns = new Waterlanding_MassGpos_Compartments_Containers_Cisterns(BaseMass, BaseGPos, list, list3, list2, stations, 0.001f, aquaDensity);
					waterlanding_MassGpos_Compartments_Containers_Cisterns.GetDStability();
					DrawProjectElements.DrawStabilityDiagramm(usgModelFrame_longStabDiagramm, waterlanding_MassGpos_Compartments_Containers_Cisterns.diagrammLongPoints, waterlanding_MassGpos_Compartments_Containers_Cisterns.diagrammDLongPoints, waterlanding_MassGpos_Compartments_Containers_Cisterns.DSPL, "0", usgNumberField_longStartAngle.Value(), usgNumberField_longEndAngle.Value(), usgNumberField_longStepAngle.Value(), DrawMetacentrH: true, checkBox_isDrawDDOForLongStab.Checked);
					break;
				}
				case diagrammType.transvDiagramm:
				{
					List<Compartment> list = (checkBox_isUsingCompartmentsForTransvStab.Checked ? compartments : new List<Compartment>());
					List<Cistern> list2 = (checkBox_isUsingCisternsForTransvStab.Checked ? cisterns : new List<Cistern>());
					List<STEB2.Containers.Container> list3 = (checkBox_isUsingContainersForTransvStab.Checked ? containers : new List<STEB2.Containers.Container>());
					Waterlanding_MassGpos_Compartments_Containers_Cisterns waterlanding_MassGpos_Compartments_Containers_Cisterns = new Waterlanding_MassGpos_Compartments_Containers_Cisterns(BaseMass, BaseGPos, list, list3, list2, stations, 0.001f, aquaDensity);
					waterlanding_MassGpos_Compartments_Containers_Cisterns.GetDStability();
					DrawProjectElements.DrawStabilityDiagramm(usgModelFrame_transvStabDiagramm, waterlanding_MassGpos_Compartments_Containers_Cisterns.diagrammTransvPoints, waterlanding_MassGpos_Compartments_Containers_Cisterns.diagrammDTransvPoints, waterlanding_MassGpos_Compartments_Containers_Cisterns.DSPL, "0", usgNumberField_transvStartAngle.Value(), usgNumberField_transvEndAngle.Value(), usgNumberField_transvStepAngle.Value(), DrawMetacentrH: true, checkBox_isDrawDDOForTransvStab.Checked);
					break;
				}
				}
				usgModelFrame_longStabDiagramm.ZoomAll();
				usgModelFrame_transvStabDiagramm.ZoomAll();
			}
			catch
			{
			}
		}
	}

	private void LoadStartParameter()
	{
		using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
		oleDbConnection.Open();
		ToolStripTextBox toolStripTextBox_FontSize = usgModelFrame_transvStabDiagramm.toolStripTextBox_FontSize;
		string text = (usgModelFrame_longStabDiagramm.toolStripTextBox_FontSize.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "textHeight", oleDbConnection).ToString()).ToString());
		toolStripTextBox_FontSize.Text = text;
		usgModelFrame_transvStabDiagramm.toolStripTextBox_ScaleX.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "transvStaticDiagrammXScale", oleDbConnection).ToString()).ToString();
		usgModelFrame_transvStabDiagramm.toolStripTextBox_ScaleY.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "transvStaticDiagrammYScale", oleDbConnection).ToString()).ToString();
		usgModelFrame_longStabDiagramm.toolStripTextBox_ScaleX.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "longStaticDiagrammXScale", oleDbConnection).ToString()).ToString();
		usgModelFrame_longStabDiagramm.toolStripTextBox_ScaleY.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "longStaticDiagrammYScale", oleDbConnection).ToString()).ToString();
		usgNumberField_transvStartAngle.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "transvStaticDiagrammStartAngle", oleDbConnection).ToString()).ToString();
		usgNumberField_transvEndAngle.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "transvStaticDiagrammEndAngle", oleDbConnection).ToString()).ToString();
		usgNumberField_transvStepAngle.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "transvStaticDiagrammStepAngle", oleDbConnection).ToString()).ToString();
		usgNumberField_longStartAngle.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "longStaticDiagrammStartAngle", oleDbConnection).ToString()).ToString();
		usgNumberField_longEndAngle.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "longStaticDiagrammEndAngle", oleDbConnection).ToString()).ToString();
		usgNumberField_longStepAngle.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "longStaticDiagrammStepAngle", oleDbConnection).ToString()).ToString();
		oleDbConnection.Close();
	}

	private void InitializeComponent()
	{
		this.usgModelFrame_transvStabDiagramm = new STEB2.usgModelFrame();
		this.button_report_transvStabDiagramm = new System.Windows.Forms.Button();
		this.button_calc_transvStabDiagramm = new System.Windows.Forms.Button();
		this.usgNumberField_transvStepAngle = new STEB2.usgNumberField();
		this.label3 = new System.Windows.Forms.Label();
		this.usgNumberField_transvEndAngle = new STEB2.usgNumberField();
		this.label2 = new System.Windows.Forms.Label();
		this.usgNumberField_transvStartAngle = new STEB2.usgNumberField();
		this.label1 = new System.Windows.Forms.Label();
		this.checkBox_isDrawDDOForTransvStab = new System.Windows.Forms.CheckBox();
		this.checkBox_isUsingCompartmentsForTransvStab = new System.Windows.Forms.CheckBox();
		this.checkBox_isUsingCisternsForTransvStab = new System.Windows.Forms.CheckBox();
		this.checkBox_isUsingContainersForTransvStab = new System.Windows.Forms.CheckBox();
		this.usgModelFrame_longStabDiagramm = new STEB2.usgModelFrame();
		this.button_report_longStabDiagramm = new System.Windows.Forms.Button();
		this.button_calc_longStabDiagramm = new System.Windows.Forms.Button();
		this.usgNumberField_longStepAngle = new STEB2.usgNumberField();
		this.label4 = new System.Windows.Forms.Label();
		this.usgNumberField_longEndAngle = new STEB2.usgNumberField();
		this.label5 = new System.Windows.Forms.Label();
		this.usgNumberField_longStartAngle = new STEB2.usgNumberField();
		this.label6 = new System.Windows.Forms.Label();
		this.checkBox_isDrawDDOForLongStab = new System.Windows.Forms.CheckBox();
		this.checkBox_isUsingCompartmentsForLongStab = new System.Windows.Forms.CheckBox();
		this.checkBox_isUsingCisternsForLongStab = new System.Windows.Forms.CheckBox();
		this.checkBox_isUsingContainersForLongStab = new System.Windows.Forms.CheckBox();
		Controls.Add(this.usgModelFrame_transvStabDiagramm);
		this.usgModelFrame_transvStabDiagramm.ModelFrameCaption = "Диаграмма поперечной остойчивости";
		this.usgModelFrame_transvStabDiagramm.Name = "usgModelFrame_transvStabDiagramm";
		Controls.Add(this.button_report_transvStabDiagramm);
		Controls.Add(this.button_calc_transvStabDiagramm);
		Controls.Add(this.usgNumberField_transvStepAngle);
		Controls.Add(this.label3);
		Controls.Add(this.usgNumberField_transvEndAngle);
		Controls.Add(this.label2);
		Controls.Add(this.usgNumberField_transvStartAngle);
		Controls.Add(this.label1);
		Controls.Add(this.checkBox_isDrawDDOForTransvStab);
		Controls.Add(this.checkBox_isUsingCompartmentsForTransvStab);
		Controls.Add(this.checkBox_isUsingCisternsForTransvStab);
		Controls.Add(this.checkBox_isUsingContainersForTransvStab);
		this.button_report_transvStabDiagramm.Name = "button_report_transvStabDiagramm";
		this.button_report_transvStabDiagramm.Text = "ОТЧЕТ";
		this.button_calc_transvStabDiagramm.Name = "button_calc_transvStabDiagramm";
		this.button_calc_transvStabDiagramm.Text = "РАСЧЕТ ДИАГРАММЫ";
		this.usgNumberField_transvStepAngle.Name = "usgNumberField_transvStepAngle";
		this.label3.Name = "label3";
		this.label3.Text = "Шаг углов крена, градусы:";
		this.usgNumberField_transvEndAngle.Name = "usgNumberField_transvEndAngle";
		this.label2.Name = "label2";
		this.label2.Text = "Конечный угол крена, градусы:";
		this.usgNumberField_transvStartAngle.Name = "usgNumberField_transvStartAngle";
		this.label1.Name = "label1";
		this.label1.Text = "Начальный угол крена, градусы:";
		this.checkBox_isDrawDDOForTransvStab.Checked = true;
		this.checkBox_isDrawDDOForTransvStab.Name = "checkBox_isDrawDDOForTransvStab";
		this.checkBox_isDrawDDOForTransvStab.Text = "Рисовать диаграмму динамической остойчивости";
		this.checkBox_isUsingCompartmentsForTransvStab.Checked = true;
		this.checkBox_isUsingCompartmentsForTransvStab.Name = "checkBox_isUsingCompartmentsForTransvStab";
		this.checkBox_isUsingCompartmentsForTransvStab.Text = "Учитывать заполненность отсеков";
		this.checkBox_isUsingCisternsForTransvStab.Checked = true;
		this.checkBox_isUsingCisternsForTransvStab.Name = "checkBox_isUsingCisternsForTransvStab";
		this.checkBox_isUsingCisternsForTransvStab.Text = "Учитывать цистерны";
		this.checkBox_isUsingContainersForTransvStab.Checked = true;
		this.checkBox_isUsingContainersForTransvStab.Name = "checkBox_isUsingContainersForTransvStab";
		this.checkBox_isUsingContainersForTransvStab.Text = "Учитывать контейнеры";
		Controls.Add(this.usgModelFrame_longStabDiagramm);
		this.usgModelFrame_longStabDiagramm.ModelFrameCaption = "Диаграмма продольной остойчивости";
		this.usgModelFrame_longStabDiagramm.Name = "usgModelFrame_longStabDiagramm";
		Controls.Add(this.button_report_longStabDiagramm);
		Controls.Add(this.button_calc_longStabDiagramm);
		Controls.Add(this.usgNumberField_longStepAngle);
		Controls.Add(this.label4);
		Controls.Add(this.usgNumberField_longEndAngle);
		Controls.Add(this.label5);
		Controls.Add(this.usgNumberField_longStartAngle);
		Controls.Add(this.label6);
		Controls.Add(this.checkBox_isDrawDDOForLongStab);
		Controls.Add(this.checkBox_isUsingCompartmentsForLongStab);
		Controls.Add(this.checkBox_isUsingCisternsForLongStab);
		Controls.Add(this.checkBox_isUsingContainersForLongStab);
		this.button_report_longStabDiagramm.Name = "button_report_longStabDiagramm";
		this.button_report_longStabDiagramm.Text = "ОТЧЕТ";
		this.button_calc_longStabDiagramm.Name = "button_calc_longStabDiagramm";
		this.button_calc_longStabDiagramm.Text = "РАСЧЕТ ДИАГРАММЫ";
		this.usgNumberField_longStepAngle.Name = "usgNumberField_longStepAngle";
		this.label4.Name = "label4";
		this.label4.Text = "Шаг углов крена, градусы:";
		this.usgNumberField_longEndAngle.Name = "usgNumberField_longEndAngle";
		this.label5.Name = "label5";
		this.label5.Text = "Конечный угол крена, градусы:";
		this.usgNumberField_longStartAngle.Name = "usgNumberField_longStartAngle";
		this.label6.Name = "label6";
		this.label6.Text = "Начальный угол крена, градусы:";
		this.checkBox_isDrawDDOForLongStab.Checked = true;
		this.checkBox_isDrawDDOForLongStab.Name = "checkBox_isDrawDDOForLongStab";
		this.checkBox_isDrawDDOForLongStab.Text = "Рисовать диаграмму динамической остойчивости";
		this.checkBox_isUsingCompartmentsForLongStab.Checked = true;
		this.checkBox_isUsingCompartmentsForLongStab.Name = "checkBox_isUsingCompartmentsForLongStab";
		this.checkBox_isUsingCompartmentsForLongStab.Text = "Учитывать заполненность отсеков";
		this.checkBox_isUsingCisternsForLongStab.Checked = true;
		this.checkBox_isUsingCisternsForLongStab.Name = "checkBox_isUsingCisternsForLongStab";
		this.checkBox_isUsingCisternsForLongStab.Text = "Учитывать цистерны";
		this.checkBox_isUsingContainersForLongStab.Checked = true;
		this.checkBox_isUsingContainersForLongStab.Name = "checkBox_isUsingContainersForLongStab";
		this.checkBox_isUsingContainersForLongStab.Text = "Учитывать контейнеры";
		Text = "Диаграммы остойчивости";
	}
}
