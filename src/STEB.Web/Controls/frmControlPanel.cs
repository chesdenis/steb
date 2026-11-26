using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using STEB2;
using STEB2.Cisterns;
using STEB2.Compartments;
using STEB2.Containers;
using STEB2.Controls;
using STEB2.Converting;
using STEB2.DataBase;
using STEB2.Geometry;
using STEB2.GraphicsElement;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB_LOAD;

public class frmControlPanel : Form
{

	public SplitContainer splitContainer;

	public usgModelFrame usgModelFrame_Sections;

	public usgModelFrame usgModelFrame_Waterlanding;

	public ToolStripMenuItem toolStripMenuItem_BaseData;

	public ToolStripMenuItem toolStripButton_DefineTransvSection;

	public ToolStripMenuItem toolStripButton_DefineLongSection;

	public ToolStripMenuItem toolStripButton_EditAquatory;

	public ToolStripMenuItem toolStripButton_BaseMassGpos;

	public ToolStripMenuItem toolStripButton_Compartment;

	public ToolStripMenuItem toolStripButton_Container;

	public ToolStripMenuItem toolStripMenuItem_Diagramms;

	public ToolStripMenuItem toolStripButton_EditShipForm;

	public ToolStripMenuItem toolStripButton_GryzRazmer;

	public ToolStripMenuItem toolStripButton_DiagrammXcXf;

	public ToolStripMenuItem toolStripButton_DiagrammrZc;

	public ToolStripMenuItem toolStripButton_DiagrammHydroCoeff;

	public ToolStripMenuItem toolStripButton_DSO;

	public ToolStripMenuItem toolStripButton_Pantokareni;

	public ToolStripButton toolStripButton_RecalcWaterlanding;

	public ToolStripMenuItem toolStripMenuItem_Help;

	public ToolStripMenuItem toolStripMenuItem_File;

	public ToolStripMenuItem toolStripMenuItem_CreateProject;

	public ToolStripMenuItem toolStripMenuItem_Open;

	public ToolStripMenuItem toolStripMenuItem_SaveProjectAs;

	public ToolStripMenuItem toolStripMenuItem_Exit;

	public ToolStripMenuItem toolStripMenuItem_CreateWithPrototype;

	public ToolStripMenuItem toolStripMenuItem7;

	public ToolStripMenuItem toolStripMenuItem_Reports_Docs;

	public ToolStripMenuItem toolStripButton_Report;

	public ToolStripMenuItem toolStripButton_About;

	public ToolStripMenuItem toolStripMenuItem_CircleStabilityDiagramm;

	public TextBox textBoxStatus;

	public ToolStripMenuItem toolStripMenuItem_Cisterns;

	public ToolStripMenuItem toolStripMenuItem_ReportHydrostatic;

	public ToolStripMenuItem toolStripMenuItem_Calcs;

	public ToolStripMenuItem toolStripMenuItem_SampleWaterlanding;

	/// <summary>Raised instead of Form.ShowDialog(): (editor key, window title).</summary>
	public event Action<string, string> OpenDialog;

	/// <summary>The code that followed ShowDialog() in every editor handler.</summary>
	public void DialogClosed()
	{
		ReloadAllVariables(needRecalc: true);
	}

	private float aquaDensity = 1f;

	private List<Station> stations = new List<Station>();

	private List<Compartment> compartments = new List<Compartment>();

	private List<Cistern> cisterns = new List<Cistern>();

	private List<STEB2.Containers.Container> containers = new List<STEB2.Containers.Container>();

	private Waterlanding trueWaterLanding = new Waterlanding();

	private float BaseMass = 0f;

	private p3d BaseGPos = default;

	private void InitializeComponent()
	{
		this.splitContainer = new System.Windows.Forms.SplitContainer();
		this.usgModelFrame_Sections = new STEB2.usgModelFrame();
		this.usgModelFrame_Waterlanding = new STEB2.usgModelFrame();
		this.toolStripButton_RecalcWaterlanding = new System.Windows.Forms.ToolStripButton();
		this.toolStripMenuItem_File = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Open = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_CreateProject = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_CreateWithPrototype = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_SaveProjectAs = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_BaseData = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_EditShipForm = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_DefineTransvSection = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_DefineLongSection = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_EditAquatory = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_BaseMassGpos = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_Compartment = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Cisterns = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_Container = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Calcs = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_SampleWaterlanding = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Diagramms = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_GryzRazmer = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_DiagrammXcXf = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_DiagrammrZc = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_DiagrammHydroCoeff = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_DSO = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_Pantokareni = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_CircleStabilityDiagramm = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Reports_Docs = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_ReportHydrostatic = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_Report = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Help = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripButton_About = new System.Windows.Forms.ToolStripMenuItem();
		this.textBoxStatus = new System.Windows.Forms.TextBox();
		this.splitContainer.Name = "splitContainer";
		Controls.Add(this.usgModelFrame_Sections);
		Controls.Add(this.usgModelFrame_Waterlanding);
		this.usgModelFrame_Sections.ModelFrameCaption = "Схема загрузки отсеков";
		this.usgModelFrame_Sections.Name = "usgModelFrame_Sections";
		this.usgModelFrame_Waterlanding.ModelFrameCaption = "Схема посадки корпуса";
		this.usgModelFrame_Waterlanding.Name = "usgModelFrame_Waterlanding";
		this.toolStripButton_RecalcWaterlanding.Name = "toolStripButton_RecalcWaterlanding";
		this.toolStripButton_RecalcWaterlanding.Text = "Пересчитать посадку";
		this.toolStripMenuItem_File.Name = "toolStripMenuItem_File";
		this.toolStripMenuItem_File.Text = "Файл";
		this.toolStripMenuItem_Open.Name = "toolStripMenuItem_Open";
		this.toolStripMenuItem_Open.Text = "Открыть...";
		this.toolStripMenuItem_CreateProject.Name = "toolStripMenuItem_CreateProject";
		this.toolStripMenuItem_CreateProject.Text = "Создать...";
		this.toolStripMenuItem_CreateWithPrototype.Name = "toolStripMenuItem_CreateWithPrototype";
		this.toolStripMenuItem_CreateWithPrototype.Text = "Создать на основе прототипа...";
		this.toolStripMenuItem_SaveProjectAs.Name = "toolStripMenuItem_SaveProjectAs";
		this.toolStripMenuItem_SaveProjectAs.Text = "Сохранить как...";
		this.toolStripMenuItem_Exit.Name = "toolStripMenuItem_Exit";
		this.toolStripMenuItem_Exit.Text = "Выход";
		this.toolStripMenuItem_BaseData.Name = "toolStripMenuItem_BaseData";
		this.toolStripMenuItem_BaseData.Text = "Исходные данные";
		this.toolStripButton_EditShipForm.Name = "toolStripButton_EditShipForm";
		this.toolStripButton_EditShipForm.Text = "Редактировать форму корпуса";
		this.toolStripButton_DefineTransvSection.Name = "toolStripButton_DefineTransvSection";
		this.toolStripButton_DefineTransvSection.Text = "Поперечные сечения";
		this.toolStripButton_DefineLongSection.Name = "toolStripButton_DefineLongSection";
		this.toolStripButton_DefineLongSection.Text = "Продольные сечения";
		this.toolStripButton_EditAquatory.Name = "toolStripButton_EditAquatory";
		this.toolStripButton_EditAquatory.Text = "Редактировать акваторию";
		this.toolStripButton_BaseMassGpos.Name = "toolStripButton_BaseMassGpos";
		this.toolStripButton_BaseMassGpos.Text = "Начальные Масса и ЦТ";
		this.toolStripButton_Compartment.Name = "toolStripButton_Compartment";
		this.toolStripButton_Compartment.Text = "Отсеки";
		this.toolStripMenuItem_Cisterns.Name = "toolStripMenuItem_Cisterns";
		this.toolStripMenuItem_Cisterns.Text = "Цистерны";
		this.toolStripButton_Container.Name = "toolStripButton_Container";
		this.toolStripButton_Container.Text = "Контейнеры";
		this.toolStripMenuItem_Calcs.Name = "toolStripMenuItem_Calcs";
		this.toolStripMenuItem_Calcs.Text = "Расчеты";
		this.toolStripMenuItem_SampleWaterlanding.Name = "toolStripMenuItem_SampleWaterlanding";
		this.toolStripMenuItem_SampleWaterlanding.Text = "Произвольная посадка судна";
		this.toolStripMenuItem_Diagramms.Name = "toolStripMenuItem_Diagramms";
		this.toolStripMenuItem_Diagramms.Text = "Диаграммы и графики";
		this.toolStripButton_GryzRazmer.Name = "toolStripButton_GryzRazmer";
		this.toolStripButton_GryzRazmer.Text = "Грузовой размер";
		this.toolStripButton_DiagrammXcXf.Name = "toolStripButton_DiagrammXcXf";
		this.toolStripButton_DiagrammXcXf.Text = "Диаграмма Xc и Xf";
		this.toolStripButton_DiagrammrZc.Name = "toolStripButton_DiagrammrZc";
		this.toolStripButton_DiagrammrZc.Text = "Диаграмма r и Zc";
		this.toolStripButton_DiagrammHydroCoeff.Name = "toolStripButton_DiagrammHydroCoeff";
		this.toolStripButton_DiagrammHydroCoeff.Text = "Диаграммы коэффициентов полноты";
		this.toolStripButton_DSO.Name = "toolStripButton_DSO";
		this.toolStripButton_DSO.Text = "Диаграммы остойчивости";
		this.toolStripButton_Pantokareni.Name = "toolStripButton_Pantokareni";
		this.toolStripButton_Pantokareni.Text = "Диаграмма плеч остойчивости формы";
		this.toolStripMenuItem_CircleStabilityDiagramm.Name = "toolStripMenuItem_CircleStabilityDiagramm";
		this.toolStripMenuItem_CircleStabilityDiagramm.Text = "Круговая диаграмма остойчивости";
		this.toolStripMenuItem_Reports_Docs.Name = "toolStripMenuItem_Reports_Docs";
		this.toolStripMenuItem_Reports_Docs.Text = "Отчеты и документы";
		this.toolStripMenuItem_ReportHydrostatic.Name = "toolStripMenuItem_ReportHydrostatic";
		this.toolStripMenuItem_ReportHydrostatic.Text = "Отчет по гидростатике";
		this.toolStripButton_Report.Name = "toolStripButton_Report";
		this.toolStripButton_Report.Text = "Отчет о посадке";
		this.toolStripMenuItem_Help.Name = "toolStripMenuItem_Help";
		this.toolStripMenuItem_Help.Text = "Справка";
		this.toolStripMenuItem7.Name = "toolStripMenuItem7";
		this.toolStripMenuItem7.Text = "Руководство пользователя...";
		this.toolStripButton_About.Name = "toolStripButton_About";
		this.toolStripButton_About.Text = "О программе...";
		this.textBoxStatus.Multiline = true;
		this.textBoxStatus.Name = "textBoxStatus";
		Controls.Add(this.splitContainer);
		Controls.Add(this.textBoxStatus);
		Text = "STEB - Load - панель управления загрузкой судна";
	}

	public void usgModelFrame_Sections_toolStripTextBox_FontSize_TextChanged(object sender, EventArgs e)
	{
		try
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "textHeight", Converting.TextToFloat(usgModelFrame_Sections.toolStripTextBox_FontSize.Text), oleDbConnection);
			oleDbConnection.Close();
		}
		catch
		{
		}
	}

	public void usgModelFrame_Waterlanding_toolStripTextBox_FontSize_TextChanged(object sender, EventArgs e)
	{
		try
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			STEB2.DataBase.Tools.ExecuteUpdateSQL("tblShipForms", "textHeight", Converting.TextToFloat(usgModelFrame_Waterlanding.toolStripTextBox_FontSize.Text), oleDbConnection);
			oleDbConnection.Close();
		}
		catch
		{
		}
	}

	private void ReenableControls()
	{
		try
		{
			STEB2.DataBase.Tools.workFile = (string.IsNullOrEmpty(STEB2.DataBase.Tools.workFile) ? "" : STEB2.DataBase.Tools.workFile);
			Text = Text + " " + STEB2.DataBase.Tools.workFile;
			if (STEB2.DataBase.Tools.workFile.Trim() == "")
			{
				toolStripMenuItem_BaseData.Enabled = false;
				toolStripMenuItem_Diagramms.Enabled = false;
				toolStripMenuItem_SaveProjectAs.Enabled = false;
				toolStripMenuItem_Reports_Docs.Enabled = false;
				toolStripButton_RecalcWaterlanding.Enabled = false;
			}
			else
			{
				toolStripMenuItem_BaseData.Enabled = true;
				toolStripMenuItem_Diagramms.Enabled = true;
				toolStripMenuItem_SaveProjectAs.Enabled = true;
				toolStripMenuItem_Reports_Docs.Enabled = true;
				toolStripButton_RecalcWaterlanding.Enabled = true;
			}
		}
		catch
		{
		}
	}

	private void ReloadAllVariables(bool needRecalc)
	{
		try
		{
			LoadAquatory();
		}
		catch
		{
		}
		try
		{
			LoadBaseMassGPos();
		}
		catch
		{
		}
		try
		{
			LoadCisterns();
		}
		catch
		{
		}
		try
		{
			LoadStations();
		}
		catch
		{
		}
		try
		{
			LoadСompartments();
		}
		catch
		{
		}
		try
		{
			LoadСontainers();
		}
		catch
		{
		}
		SyncronizeFormsValue();
		if (needRecalc)
		{
			RecalcTrueWaterLanding();
		}
	}

	private void SyncronizeFormsValue()
	{
		foreach (Form openForm in Application.OpenForms)
		{
			if (openForm.Name == "frmDiagrammStability")
			{
				try
				{
					frmDiagrammStability frmDiagrammStability2 = (frmDiagrammStability)openForm;
					frmDiagrammStability2.BaseGPos = BaseGPos;
					frmDiagrammStability2.BaseMass = BaseMass;
					frmDiagrammStability2.stations = stations;
					frmDiagrammStability2.compartments = compartments;
					frmDiagrammStability2.containers = containers;
					frmDiagrammStability2.cisterns = cisterns;
					frmDiagrammStability2.aquaDensity = aquaDensity;
				}
				catch
				{
				}
			}
		}
	}

	private void RedrawAll()
	{
		if (MessageBox.Show("Перерисовать?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
		{
			usgModelFrame_Sections.ClearDB();
			usgModelFrame_Waterlanding.ClearDB();
			DrawHullAndBroadSideRotated();
			DrawTransverseSections();
			DrawLongitudinalSections();
		}
	}

	private void DrawTransverseSections()
	{
		using (new frmPleaseWait())
		{
			Progress.status = "Вывод поперечных сечений формы корпуса...";
			Progress.ProgressMaximum = 0;
			usgModelFrame_Sections.ClearDB("transvSectionForm");
			usgModelFrame_Sections.ClearDB("transvSectionFormBack");
			string textLong = "";
			string text = "";
			using (JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile))
			{
				oleDbConnection.Open();
				string text2 = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
				textLong = GetProjectElements.GetSectionDescription(ProjectSectionDefinition.SectionType.Long, oleDbConnection);
				text = GetProjectElements.GetSectionDescription(ProjectSectionDefinition.SectionType.Transv, oleDbConnection);
				oleDbConnection.Close();
			}
			p3d gabariteOfProject = STEB2.Volume.Tools.GetGabariteOfProject(stations, cisterns, containers);
			int num = 1;
			int num2 = 1;
			int num3 = 1;
			try
			{
				string[] array = text.Split(';');
				string[] array2 = array;
				foreach (string text3 in array2)
				{
					Progress.ProgressMaximum++;
				}
				array2 = array;
				foreach (string text3 in array2)
				{
					if (text3.Trim() != "")
					{
						Progress.ProgressPosition++;
						DrawProjectElements.DrawHullSection(usgModelFrame_Sections, stations, containers, cisterns, compartments, Converting.TextToFloat(text3.Split(':')[0].Trim()), text3.Split(':')[1].Trim(), "", "transvSectionForm" + num3, textLong, trueWaterLanding.HeelAngle, trueWaterLanding.TrimAngle);
						DrawProjectElements.DrawWL(usgModelFrame_Sections, trueWaterLanding, "transvSectionForm" + num3, DrawProjectElements.Planes.YZ, Converting.TextToFloat(text3.Split(':')[0].Trim()), Color.Transparent);
						usgModelFrame_Sections.MoveGroup(new p2d(0f, 0f), new p2d(gabariteOfProject.X + 1.5f * gabariteOfProject.Y * (float)num, -2f * gabariteOfProject.Z * (float)num2), new string[1] { "transvSectionForm" + num3 });
						num3++;
						if (num > 2)
						{
							num = 1;
							num2++;
						}
						else
						{
							num++;
						}
					}
				}
			}
			catch
			{
			}
		}
	}

	private void DrawLongitudinalSections()
	{
		using (new frmPleaseWait())
		{
			Progress.status = "Вывод продольных сечений формы корпуса...";
			Progress.ProgressMaximum = 0;
			usgModelFrame_Sections.ClearDB("broadSectionForm");
			usgModelFrame_Sections.ClearDB("broadSectionBackForm");
			string text = "";
			string textTransv = "";
			using (JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile))
			{
				oleDbConnection.Open();
				string text2 = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
				text = GetProjectElements.GetSectionDescription(ProjectSectionDefinition.SectionType.Long, oleDbConnection);
				textTransv = GetProjectElements.GetSectionDescription(ProjectSectionDefinition.SectionType.Transv, oleDbConnection);
				oleDbConnection.Close();
			}
			p3d gabariteOfProject = STEB2.Volume.Tools.GetGabariteOfProject(stations, cisterns, containers);
			int num = 1;
			int num2 = 1;
			try
			{
				string[] array = text.Split(';');
				string[] array2 = array;
				foreach (string text3 in array2)
				{
					Progress.ProgressMaximum++;
				}
				array2 = array;
				foreach (string text3 in array2)
				{
					if (text3.Trim() != "")
					{
						Progress.ProgressPosition++;
						DrawProjectElements.DrawBroadSection(usgModelFrame_Sections, stations, containers, cisterns, compartments, Converting.TextToFloat(text3.Split(':')[0].Trim()), text3.Split(':')[1].Trim(), "", "broadSectionForm" + num2, textTransv, trueWaterLanding.HeelAngle, trueWaterLanding.TrimAngle);
						DrawProjectElements.DrawWL(usgModelFrame_Sections, trueWaterLanding, "broadSectionForm" + num2, DrawProjectElements.Planes.XZ, Converting.TextToFloat(text3.Split(':')[0].Trim()), Color.Transparent);
						usgModelFrame_Sections.MoveGroup(new p2d(0f, 0f), new p2d(0f, -2f * gabariteOfProject.Z * (float)num), new string[1] { "broadSectionForm" + num2 });
						num2++;
						num++;
					}
				}
			}
			catch
			{
			}
		}
	}

	public void DrawHullAndBroadSideRotated_ToolBtnClick(object sender, EventArgs e)
	{
		DrawHullAndBroadSideRotated();
	}

	public void DrawHullSectionsCistern_ToolBtnClick(object sender, EventArgs e)
	{
	}

	public void DrawHullSectionsCompartment_ToolBtnClick(object sender, EventArgs e)
	{
	}

	public void DrawHullSectionsContainer_ToolBtnClick(object sender, EventArgs e)
	{
	}

	public void DrawHullSectionsForm_ToolBtnClick(object sender, EventArgs e)
	{
	}

	public void DrawHullSectionsWL_ToolBtnClick(object sender, EventArgs e)
	{
	}

	public void DrawLongSectionsCistern_ToolBtnClick(object sender, EventArgs e)
	{
	}

	public void DrawLongSectionsCompartment_ToolBtnClick(object sender, EventArgs e)
	{
	}

	public void DrawLongSectionsContainer_ToolBtnClick(object sender, EventArgs e)
	{
	}

	public void DrawLongSectionsForm_ToolBtnClick(object sender, EventArgs e)
	{
	}

	public void DrawLongSectionsWL_ToolBtnClick(object sender, EventArgs e)
	{
	}

	public void log_LogFires(object sender, EventArgs e)
	{
		List<string> list = new List<string>();
		list.AddRange(STEB2.DataBase.Tools.log.LogFile);
		list.Reverse();
		textBoxStatus.Lines = list.ToArray();
	}

	public frmControlPanel()
	{
		InitializeComponent();
		ReenableControls();
		toolStripButton_About.Click += toolStripButton_About_Click;
		STEB2.DataBase.Tools.log.LogFires += log_LogFires;
		try
		{
			using (JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile))
			{
				oleDbConnection.Open();
				ToolStripTextBox toolStripTextBox_FontSize = usgModelFrame_Waterlanding.toolStripTextBox_FontSize;
				string text = (usgModelFrame_Sections.toolStripTextBox_FontSize.Text = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "textHeight", oleDbConnection).ToString()).ToString());
				toolStripTextBox_FontSize.Text = text;
				oleDbConnection.Close();
			}
			usgModelFrame_Sections.toolStripTextBox_FontSize.TextChanged += usgModelFrame_Sections_toolStripTextBox_FontSize_TextChanged;
			usgModelFrame_Waterlanding.toolStripTextBox_FontSize.TextChanged += usgModelFrame_Waterlanding_toolStripTextBox_FontSize_TextChanged;
			toolStripButton_Compartment.Click += toolStripButton_Compartment_Click;
			toolStripButton_Container.Click += toolStripButton_Container_Click;
			toolStripMenuItem_Cisterns.Click += toolStripMenuItem_Cisterns_Click;
			toolStripButton_EditAquatory.Click += toolStripButton_EditAquatory_Click;
			toolStripButton_EditShipForm.Click += toolStripButton_EditShipForm_Click;
			toolStripButton_DefineLongSection.Click += toolStripButton_DefineLongSection_Click;
			toolStripButton_DefineTransvSection.Click += toolStripButton_DefineTransvSection_Click;
			toolStripButton_DSO.Click += toolStripButton_DSO_Click;
			toolStripButton_BaseMassGpos.Click += toolStripButton_BaseMassGpos_Click;
			toolStripButton_Report.Click += toolStripButton_Report_Click;
			toolStripButton_RecalcWaterlanding.Click += toolStripButton_RecalcWaterlanding_Click;
			toolStripButton_Pantokareni.Click += toolStripButton_Pantokareni_Click;
			toolStripButton_GryzRazmer.Click += toolStripButton_GryzRazmer_Click;
			toolStripButton_DiagrammHydroCoeff.Click += toolStripButton_DiagrammHydroCoeff_Click;
			toolStripButton_DiagrammrZc.Click += toolStripButton_DiagrammrZc_Click;
			toolStripButton_DiagrammXcXf.Click += toolStripButton_DiagrammXcXf_Click;
			toolStripMenuItem_CircleStabilityDiagramm.Click += toolStripMenuItem_CircleStabilityDiagramm_Click;
			toolStripMenuItem_ReportHydrostatic.Click += toolStripMenuItem_ReportGryzRazmer_Click;
			toolStripMenuItem_SampleWaterlanding.Click += toolStripMenuItem_SampleWaterlanding_Click;
		}
		catch
		{
		}
		ReloadAllVariables(needRecalc: true);
	}

	public void toolStripButton_RecalcWaterlanding_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: true);
	}

	public void toolStripButton_About_Click(object sender, EventArgs e)
	{
		try
		{
			new AboutBox().ShowDialog();
		}
		catch
		{
		}
	}

	public void toolStripButton_Report_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		try
		{
			frmReport frmReport2 = new frmReport(trueWaterLanding);
			frmReport2.Padding = new Padding(10);
			frmReport2.Text = "Отчет о посадке";
			frmReport2.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
			frmReport2.Show();
		}
		catch
		{
		}
	}

	public void toolStripMenuItem_SampleWaterlanding_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		try
		{
			frmSampleWaterlanding frmSampleWaterlanding2 = new frmSampleWaterlanding(stations, aquaDensity);
			frmSampleWaterlanding2.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
		}
		catch
		{
		}
	}

	public void toolStripMenuItem_ReportGryzRazmer_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		try
		{
			frmHydroStaticReport frmHydroStaticReport2 = new frmHydroStaticReport(stations, aquaDensity);
			frmHydroStaticReport2.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
		}
		catch
		{
		}
	}

	public void toolStripButton_BaseMassGpos_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		// Original: shows the editor in a modal Form (ShowDialog), then continues in DialogClosed().
		OpenDialog?.Invoke("gposition", "Отредактируйте начальное положение ЦТ и вес судна");
	}

	public void toolStripButton_DSO_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		try
		{
			frmDiagrammStability frmDiagrammStability2 = new frmDiagrammStability(stations, compartments, cisterns, containers, BaseMass, BaseGPos, aquaDensity);
			frmDiagrammStability2.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
		}
		catch
		{
		}
	}

	public void toolStripMenuItem_CircleStabilityDiagramm_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		try
		{
			frmDiagrammCircle frmDiagrammCircle2 = new frmDiagrammCircle(aquaDensity, stations, containers, compartments, cisterns, BaseMass, BaseGPos);
			frmDiagrammCircle2.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
		}
		catch
		{
		}
	}

	public void toolStripButton_DiagrammXcXf_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		try
		{
			frmDiagrammXcXf frmDiagrammXcXf2 = new frmDiagrammXcXf(aquaDensity, stations);
			frmDiagrammXcXf2.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
		}
		catch
		{
		}
	}

	public void toolStripButton_DiagrammrZc_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		try
		{
			frmDiagrammrZc frmDiagrammrZc2 = new frmDiagrammrZc(aquaDensity, stations);
			frmDiagrammrZc2.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
		}
		catch
		{
		}
	}

	public void toolStripButton_DiagrammHydroCoeff_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		try
		{
			frmDiagrammHydroCoeff frmDiagrammHydroCoeff2 = new frmDiagrammHydroCoeff(aquaDensity, stations);
			frmDiagrammHydroCoeff2.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
		}
		catch
		{
		}
	}

	public void toolStripButton_GryzRazmer_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		try
		{
			frmGryzRazmer frmGryzRazmer2 = new frmGryzRazmer(aquaDensity, stations);
			frmGryzRazmer2.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
		}
		catch
		{
		}
	}

	public void toolStripButton_Pantokareni_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		try
		{
			frmPantokareni frmPantokareni2 = new frmPantokareni(aquaDensity, stations);
			frmPantokareni2.Size = ((WindowState == FormWindowState.Maximized) ? new Size(Width / 2, Height / 2) : new Size(Width, Height));
		}
		catch
		{
		}
	}

	public void toolStripButton_DefineTransvSection_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		// Original: shows the editor in a modal Form (ShowDialog), then continues in DialogClosed().
		OpenDialog?.Invoke("transv", "Отредактируйте список поперечных сечений");
	}

	public void toolStripButton_DefineLongSection_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		// Original: shows the editor in a modal Form (ShowDialog), then continues in DialogClosed().
		OpenDialog?.Invoke("long", "Отредактируйте список продольных сечений");
	}

	public void toolStripButton_EditShipForm_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		// Original: shows the editor in a modal Form (ShowDialog), then continues in DialogClosed().
		OpenDialog?.Invoke("hull", "Отредактируйте список форм проекта");
	}

	public void toolStripButton_EditAquatory_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		// Original: shows the editor in a modal Form (ShowDialog), then continues in DialogClosed().
		OpenDialog?.Invoke("aquatory", "Отредактируйте список акваторий");
	}

	public void toolStripButton_Container_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		// Original: shows the editor in a modal Form (ShowDialog), then continues in DialogClosed().
		OpenDialog?.Invoke("containers", "Отредактируйте список контейнеров и закройте окно для принятия изменений");
	}

	public void toolStripButton_Compartment_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		// Original: shows the editor in a modal Form (ShowDialog), then continues in DialogClosed().
		OpenDialog?.Invoke("compartments", "Отредактируйте список отсеков и закройте окно для принятия изменений");
	}

	public void toolStripMenuItem_Cisterns_Click(object sender, EventArgs e)
	{
		ReloadAllVariables(needRecalc: false);
		// Original: shows the editor in a modal Form (ShowDialog), then continues in DialogClosed().
		OpenDialog?.Invoke("cisterns", "Редактирование цистерн");
	}

	private void RecalcTrueWaterLanding()
	{
		try
		{
			if (MessageBox.Show("Пересчитать посадку?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				using (new frmPleaseWait())
				{
					trueWaterLanding = new Waterlanding_MassGpos_Compartments_Containers_Cisterns(BaseMass, new p3d(BaseGPos.X, BaseGPos.Y, BaseGPos.Z), compartments, containers, cisterns, stations, 0.001f, aquaDensity);
				}
				RedrawAll();
			}
		}
		catch
		{
		}
	}

	private void LoadBaseMassGPos()
	{
		try
		{
			using (new frmPleaseWait())
			{
				Progress.status = "Чтение начальной массы и координат ЦТ...";
				Progress.ProgressMaximum = 1;
				Progress.ProgressPosition = 1;
				using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
				oleDbConnection.Open();
				string text = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
				BaseMass = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms WHERE ShipFormName = '" + text + "'", "ShipFormMass", oleDbConnection).ToString());
				BaseGPos.X = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms WHERE ShipFormName = '" + text + "'", "Xg", oleDbConnection).ToString());
				BaseGPos.Y = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms WHERE ShipFormName = '" + text + "'", "Yg", oleDbConnection).ToString());
				BaseGPos.Z = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms WHERE ShipFormName = '" + text + "'", "Zg", oleDbConnection).ToString());
				oleDbConnection.Close();
			}
		}
		catch
		{
		}
	}

	private void LoadСontainers()
	{
		try
		{
			using (new frmPleaseWait())
			{
				Progress.status = "Чтение контейнеров...";
				Progress.ProgressMaximum = 1;
				Progress.ProgressPosition = 1;
				ProjectContainers projectContainers = new ProjectContainers();
				containers = projectContainers.GetContainers();
			}
		}
		catch
		{
		}
	}

	private void LoadCisterns()
	{
		try
		{
			using (new frmPleaseWait())
			{
				Progress.status = "Чтение цистерн...";
				Progress.ProgressMaximum = 0;
				Progress.ProgressPosition = 1;
				ProjectCisterns projectCisterns = new ProjectCisterns();
				cisterns = projectCisterns.GetCisterns();
			}
		}
		catch
		{
		}
	}

	private void LoadСompartments()
	{
		try
		{
			using (new frmPleaseWait())
			{
				Progress.status = "Чтение отсеков...";
				Progress.ProgressMaximum = 0;
				Progress.ProgressPosition = 1;
				ProjectCompartments projectCompartments = new ProjectCompartments();
				compartments = projectCompartments.GetCompartment();
				foreach (Compartment compartment in compartments)
				{
					Progress.ProgressMaximum++;
				}
				foreach (Compartment compartment2 in compartments)
				{
					Progress.status = "Расчет формы отсека: " + compartment2.Name;
					Progress.ProgressPosition++;
					compartment2.CalcCompartment(stations);
				}
			}
		}
		catch
		{
		}
	}

	private void LoadStations()
	{
		try
		{
			using (new frmPleaseWait())
			{
				Progress.status = "Чтение формы корпуса...";
				Progress.ProgressMaximum = 0;
				Progress.ProgressPosition = 1;
				using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
				oleDbConnection.Open();
				string shipFormName = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
				stations = GetProjectElements.GetProjectStation(shipFormName, oleDbConnection);
				oleDbConnection.Close();
			}
		}
		catch
		{
		}
	}

	private void LoadAquatory()
	{
		try
		{
			using (new frmPleaseWait())
			{
				Progress.status = "Чтение акватории...";
				Progress.ProgressMaximum = 0;
				Progress.ProgressPosition = 1;
				using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
				oleDbConnection.Open();
				try
				{
					aquaDensity = Converting.TextToFloat(STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblAquatories", "AquaDensity", oleDbConnection).ToString());
					aquaDensity = ((aquaDensity <= 0f) ? 1f : aquaDensity);
				}
				catch
				{
				}
				oleDbConnection.Close();
			}
		}
		catch
		{
		}
	}

	private void DrawHullAndBroadSideRotated()
	{
		try
		{
			using (new frmPleaseWait())
			{
				Progress.status = "Вывод посадки...";
				Progress.ProgressMaximum = 0;
				Progress.ProgressPosition = 1;
				usgModelFrame_Waterlanding.ClearDB();
				DrawProjectElements.DrawHullSide(usgModelFrame_Waterlanding, stations, drawAllStations: false, "КОРПУС", "", DrawAxes: true, "hull_side", Color.White);
				DrawProjectElements.DrawBroadSide(usgModelFrame_Waterlanding, stations, drawStationsSled: false, "БОК", "", DrawAxes: true, "broad_side", Color.White);
				if ((trueWaterLanding != null) | (BaseMass > 0f))
				{
					try
					{
						DrawProjectElements.DrawGPoint(usgModelFrame_Waterlanding, trueWaterLanding.GPos, "hull_side", DrawProjectElements.Planes.YZ);
						DrawProjectElements.DrawGPoint(usgModelFrame_Waterlanding, trueWaterLanding.GPos, "broad_side", DrawProjectElements.Planes.XZ);
						DrawProjectElements.DrawСMPoint(usgModelFrame_Waterlanding, new p3d(trueWaterLanding.CPos.X, trueWaterLanding.CPos.Y, trueWaterLanding.CPos.Z), new p3d(trueWaterLanding.MPos.X, trueWaterLanding.MPos.Y, trueWaterLanding.MPos.Z), "hull_side", DrawProjectElements.Planes.YZ);
						DrawProjectElements.DrawСMPoint(usgModelFrame_Waterlanding, new p3d(trueWaterLanding.CPos.X, trueWaterLanding.CPos.Y, trueWaterLanding.CPos.Z), new p3d(trueWaterLanding.MPos.X, trueWaterLanding.MPos.Y, trueWaterLanding.MPos.Z), "broad_side", DrawProjectElements.Planes.XZ);
						DrawProjectElements.DrawWL(usgModelFrame_Waterlanding, trueWaterLanding, "broad_side", DrawProjectElements.Planes.XZ, 0f, Color.Aqua);
						DrawProjectElements.DrawWL(usgModelFrame_Waterlanding, trueWaterLanding, "hull_side", DrawProjectElements.Planes.YZ, trueWaterLanding.CPos.X, Color.Aqua);
						List<Station> list = new List<Station>();
						ImmersedStation[] immersedStations = trueWaterLanding.immersedStations;
						foreach (ImmersedStation immersedStation in immersedStations)
						{
							list.Add(new Station(immersedStation.StationContur, immersedStation.X));
						}
						DrawProjectElements.DrawHullSide(usgModelFrame_Waterlanding, list, drawAllStations: false, "", "", DrawAxes: false, "hull_side_immersed", Color.Aqua);
						DrawProjectElements.DrawBroadSide(usgModelFrame_Waterlanding, list, drawStationsSled: false, "", "", DrawAxes: false, "broad_side_immersed", Color.Aqua);
					}
					catch
					{
					}
				}
				p3d gabariteOfProject = STEB2.Volume.Tools.GetGabariteOfProject(stations, cisterns, containers);
				try
				{
					usgModelFrame_Waterlanding.RotateGroup(new p2d(trueWaterLanding.GPos.Y, trueWaterLanding.GPos.Z), 0f - trueWaterLanding.HeelAngle, new string[2] { "hull_side", "hull_side_immersed" });
					usgModelFrame_Waterlanding.RotateGroup(new p2d(trueWaterLanding.GPos.X, trueWaterLanding.GPos.Z), 0f - trueWaterLanding.TrimAngle, new string[2] { "broad_side", "broad_side_immersed" });
				}
				catch
				{
				}
				usgModelFrame_Waterlanding.MoveGroup(new p2d(0f, 0f), new p2d(gabariteOfProject.X + 1.5f * gabariteOfProject.Y, 0f), new string[2] { "hull_side", "hull_side_immersed" });
				usgModelFrame_Waterlanding.Redraw();
			}
		}
		catch
		{
		}
	}

	/// <summary>Web host: detach from the static log when the page closes.</summary>
	public override void Dispose()
	{
		STEB2.DataBase.Tools.log.LogFires -= log_LogFires;
	}
}
