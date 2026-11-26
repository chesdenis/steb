using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Drawing;
using System.Windows.Forms;
using STEB2;
using STEB2.Controls;
using STEB2.Converting;
using STEB2.DataBase;
using STEB2.Geometry;
using STEB2.GraphicsElement;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB_LOAD;

public class frmSampleWaterlanding : Form
{

	public ProjectWaterlandingCharacteristics projectWaterlandingCharacteristics;

	public SplitContainer splitContainer;

	public usgModelFrame usgModelFrame_Sections;

	public usgModelFrame usgModelFrame_Waterlanding;

	private float aquaDensity = 1f;

	private List<Station> stations = new List<Station>();

	private Waterlanding trueWaterLanding = new Waterlanding();

	private float BaseMass = 0f;

	private p3d BaseGPos = default;

	private void InitializeComponent()
	{
		this.projectWaterlandingCharacteristics = new STEB2.Controls.ProjectWaterlandingCharacteristics();
		this.splitContainer = new System.Windows.Forms.SplitContainer();
		this.usgModelFrame_Sections = new STEB2.usgModelFrame();
		this.usgModelFrame_Waterlanding = new STEB2.usgModelFrame();
		this.projectWaterlandingCharacteristics.Name = "projectWaterlandingCharacteristics";
		this.splitContainer.Name = "splitContainer";
		Controls.Add(this.usgModelFrame_Sections);
		Controls.Add(this.usgModelFrame_Waterlanding);
		this.usgModelFrame_Sections.ModelFrameCaption = "Схема загрузки отсеков";
		this.usgModelFrame_Sections.Name = "usgModelFrame_Sections";
		this.usgModelFrame_Waterlanding.ModelFrameCaption = "Схема посадки корпуса";
		this.usgModelFrame_Waterlanding.Name = "usgModelFrame_Waterlanding";
		Controls.Add(this.splitContainer);
		Controls.Add(this.projectWaterlandingCharacteristics);
		Text = "Произвольная посадка";
	}

	private void RedrawAll()
	{
		FillWtrlParameter();
		if (MessageBox.Show("Перерисовать?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
		{
			usgModelFrame_Sections.ClearDB();
			usgModelFrame_Waterlanding.ClearDB();
			DrawHullAndBroadSideRotated();
			DrawHullSectionsForm();
			if (!projectWaterlandingCharacteristics.radioButton_dlb_dpb_trim.Checked && !projectWaterlandingCharacteristics.radioButton_dn_dk_dlb_dpb.Checked && !projectWaterlandingCharacteristics.radioButton_dn_dk_heel.Checked && !projectWaterlandingCharacteristics.radioButton_d_heel_trim.Checked)
			{
				DrawHullSectionsWL();
			}
			DrawLongSectionsForm();
			if (!projectWaterlandingCharacteristics.radioButton_dlb_dpb_trim.Checked && !projectWaterlandingCharacteristics.radioButton_dn_dk_dlb_dpb.Checked && !projectWaterlandingCharacteristics.radioButton_dn_dk_heel.Checked && !projectWaterlandingCharacteristics.radioButton_d_heel_trim.Checked)
			{
				DrawLongSectionsWL();
			}
		}
	}

	private void FillWtrlParameter()
	{
		try
		{
			trueWaterLanding.CalcWaterlineGeometricCharacteristic();
			trueWaterLanding.CalcCoefficient();
			trueWaterLanding.GetWLGabarite_DNDKDLBDPB();
			projectWaterlandingCharacteristics.usgNumberField_alpha.Text = Math.Round(trueWaterLanding.alpha, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_delta.Text = Math.Round(trueWaterLanding.delta, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_dk.Text = Math.Round(trueWaterLanding.d_k, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_dlb.Text = Math.Round(trueWaterLanding.d_lb, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_dn.Text = Math.Round(trueWaterLanding.d_n, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_dpb.Text = Math.Round(trueWaterLanding.d_pb, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_DSPL.Text = Math.Round(trueWaterLanding.DSPL, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_heelAngle.Text = Math.Round(trueWaterLanding.HeelAngle, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_IxWl.Text = Math.Round(trueWaterLanding.Ix, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_IyWL.Text = Math.Round(trueWaterLanding.Iy, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_MetHLong.Text = Math.Round(trueWaterLanding.mLongHeight, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_MetHTransv.Text = Math.Round(trueWaterLanding.mTransverseHeight, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_Rbig.Text = Math.Round(trueWaterLanding.R, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_rSmall.Text = Math.Round(trueWaterLanding.r, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_trimAngle.Text = Math.Round(trueWaterLanding.TrimAngle, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_V.Text = Math.Round(trueWaterLanding.V, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_Xc.Text = Math.Round(trueWaterLanding.CPos.X, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_XgWl.Text = Math.Round(trueWaterLanding.xf, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_Yc.Text = Math.Round(trueWaterLanding.CPos.Y, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_YgWl.Text = Math.Round(trueWaterLanding.yf, 3).ToString();
			projectWaterlandingCharacteristics.usgNumberField_Zc.Text = Math.Round(trueWaterLanding.CPos.Z, 3).ToString();
		}
		catch
		{
		}
	}

	public frmSampleWaterlanding(List<Station> stations, float aquaDensity)
	{
		this.stations = stations;
		this.aquaDensity = aquaDensity;
		InitializeComponent();
		projectWaterlandingCharacteristics.NeedUpdate_MassGpos += projectWaterlandingCharacteristics_NeedUpdate_MassGpos;
		projectWaterlandingCharacteristics.NeedUpdate_Volume_heel_trim += projectWaterlandingCharacteristics_NeedUpdate_Volume_heel_trim;
		projectWaterlandingCharacteristics.NeedUpdate_DSPL_heel_trim += projectWaterlandingCharacteristics_NeedUpdate_DSPL_heel_trim;
		projectWaterlandingCharacteristics.NeedUpdate_d_heel_trim += projectWaterlandingCharacteristics_NeedUpdate_d_heel_trim;
		projectWaterlandingCharacteristics.NeedUpdate_d_lb_dpb_trim += projectWaterlandingCharacteristics_NeedUpdate_d_lb_dpb_trim;
		projectWaterlandingCharacteristics.NeedUpdate_dn_dk_dlb_dpb += projectWaterlandingCharacteristics_NeedUpdate_dn_dk_dlb_dpb;
		projectWaterlandingCharacteristics.NeedUpdate_dn_dk_heel += projectWaterlandingCharacteristics_NeedUpdate_dn_dk_heel;
		Show();
	}

	public void projectWaterlandingCharacteristics_NeedUpdate_dn_dk_heel(object sender, EventArgs e)
	{
		using (new frmPleaseWait())
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			try
			{
				float num = projectWaterlandingCharacteristics.usgNumberField_dn.Value();
				float z = projectWaterlandingCharacteristics.usgNumberField_dk.Value();
				float heelAngle = projectWaterlandingCharacteristics.usgNumberField_heelAngle.Value();
				p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(stations);
				float num2 = 0f;
				num2 = Mathematics.GetVectorAngle(new p2d[2]
				{
					new p2d(0f, z),
					new p2d(gabariteOfVolume.X, num)
				});
				trueWaterLanding = new Waterlanding_WaterlinePosition(stations, num2, heelAngle, new p3d(0f, 0f, z), aquaDensity);
				trueWaterLanding.SetGPosition(new p3d(projectWaterlandingCharacteristics.usgNumberField_Xg.Value(), projectWaterlandingCharacteristics.usgNumberField_Yg.Value(), projectWaterlandingCharacteristics.usgNumberField_Zg.Value()));
				trueWaterLanding.CalcStabilityArms(trueWaterLanding.GPos);
			}
			catch
			{
			}
			oleDbConnection.Close();
		}
		RedrawAll();
	}

	public void projectWaterlandingCharacteristics_NeedUpdate_dn_dk_dlb_dpb(object sender, EventArgs e)
	{
		using (new frmPleaseWait())
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			try
			{
				p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(stations);
				float num = projectWaterlandingCharacteristics.usgNumberField_dn.Value();
				float z = projectWaterlandingCharacteristics.usgNumberField_dk.Value();
				float num2 = projectWaterlandingCharacteristics.usgNumberField_dlb.Value();
				float num3 = projectWaterlandingCharacteristics.usgNumberField_dpb.Value();
				float num4 = 0f;
				num4 = Mathematics.GetVectorAngle(new p2d[2]
				{
					new p2d((0f - gabariteOfVolume.Y) / 2f, num2),
					new p2d(gabariteOfVolume.Y / 2f, num3)
				});
				float num5 = 0f;
				num5 = Mathematics.GetVectorAngle(new p2d[2]
				{
					new p2d(0f, z),
					new p2d(gabariteOfVolume.X, num)
				});
				trueWaterLanding = new Waterlanding_WaterlinePosition(stations, num5, num4, new p3d(0f, 0f, z), aquaDensity);
				trueWaterLanding.SetGPosition(new p3d(projectWaterlandingCharacteristics.usgNumberField_Xg.Value(), projectWaterlandingCharacteristics.usgNumberField_Yg.Value(), projectWaterlandingCharacteristics.usgNumberField_Zg.Value()));
				trueWaterLanding.CalcStabilityArms(trueWaterLanding.GPos);
			}
			catch
			{
			}
			oleDbConnection.Close();
		}
		RedrawAll();
	}

	public void projectWaterlandingCharacteristics_NeedUpdate_d_lb_dpb_trim(object sender, EventArgs e)
	{
		using (new frmPleaseWait())
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			try
			{
				p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(stations);
				float z = projectWaterlandingCharacteristics.usgNumberField_dlb.Value();
				float num = projectWaterlandingCharacteristics.usgNumberField_dpb.Value();
				float num2 = 0f;
				num2 = Mathematics.GetVectorAngle(new p2d[2]
				{
					new p2d((0f - gabariteOfVolume.Y) / 2f, z),
					new p2d(gabariteOfVolume.Y / 2f, num)
				});
				float num3 = 0f;
				num3 = projectWaterlandingCharacteristics.usgNumberField_trimAngle.Value();
				trueWaterLanding = new Waterlanding_WaterlinePosition(stations, num3, num2, new p3d(gabariteOfVolume.X / 2f, (0f - gabariteOfVolume.Y) / 2f, z), aquaDensity);
				trueWaterLanding.SetGPosition(new p3d(projectWaterlandingCharacteristics.usgNumberField_Xg.Value(), projectWaterlandingCharacteristics.usgNumberField_Yg.Value(), projectWaterlandingCharacteristics.usgNumberField_Zg.Value()));
				trueWaterLanding.CalcStabilityArms(trueWaterLanding.GPos);
			}
			catch
			{
			}
			oleDbConnection.Close();
		}
		RedrawAll();
	}

	public void projectWaterlandingCharacteristics_NeedUpdate_d_heel_trim(object sender, EventArgs e)
	{
		using (new frmPleaseWait())
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			try
			{
				List<float> list = new List<float>();
				list.Add(projectWaterlandingCharacteristics.usgNumberField_dk.Value());
				list.Add(projectWaterlandingCharacteristics.usgNumberField_dn.Value());
				list.Add(projectWaterlandingCharacteristics.usgNumberField_dlb.Value());
				list.Add(projectWaterlandingCharacteristics.usgNumberField_dpb.Value());
				float maximum = Mathematics.GetMaximum(list);
				p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(stations);
				trueWaterLanding = new Waterlanding_WaterlinePosition(stations, projectWaterlandingCharacteristics.usgNumberField_trimAngle.Value(), projectWaterlandingCharacteristics.usgNumberField_heelAngle.Value(), new p3d(gabariteOfVolume.X / 2f, 0f, maximum), aquaDensity);
				trueWaterLanding.SetGPosition(new p3d(projectWaterlandingCharacteristics.usgNumberField_Xg.Value(), projectWaterlandingCharacteristics.usgNumberField_Yg.Value(), projectWaterlandingCharacteristics.usgNumberField_Zg.Value()));
				trueWaterLanding.CalcStabilityArms(trueWaterLanding.GPos);
			}
			catch
			{
			}
			oleDbConnection.Close();
		}
		RedrawAll();
	}

	public void projectWaterlandingCharacteristics_NeedUpdate_DSPL_heel_trim(object sender, EventArgs e)
	{
		using (new frmPleaseWait())
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			try
			{
				trueWaterLanding = new Waterlanding_Volume_Heel_Trim(projectWaterlandingCharacteristics.usgNumberField_DSPL.Value() / aquaDensity, projectWaterlandingCharacteristics.usgNumberField_heelAngle.Value(), projectWaterlandingCharacteristics.usgNumberField_trimAngle.Value(), stations, 0.001f, aquaDensity);
				trueWaterLanding.SetGPosition(new p3d(projectWaterlandingCharacteristics.usgNumberField_Xg.Value(), projectWaterlandingCharacteristics.usgNumberField_Yg.Value(), projectWaterlandingCharacteristics.usgNumberField_Zg.Value()));
				trueWaterLanding.CalcStabilityArms(trueWaterLanding.GPos);
			}
			catch
			{
			}
			oleDbConnection.Close();
		}
		RedrawAll();
	}

	public void projectWaterlandingCharacteristics_NeedUpdate_Volume_heel_trim(object sender, EventArgs e)
	{
		using (new frmPleaseWait())
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			try
			{
				trueWaterLanding = new Waterlanding_Volume_Heel_Trim(projectWaterlandingCharacteristics.usgNumberField_V.Value(), projectWaterlandingCharacteristics.usgNumberField_heelAngle.Value(), projectWaterlandingCharacteristics.usgNumberField_trimAngle.Value(), stations, 0.001f, aquaDensity);
				trueWaterLanding.SetGPosition(new p3d(projectWaterlandingCharacteristics.usgNumberField_Xg.Value(), projectWaterlandingCharacteristics.usgNumberField_Yg.Value(), projectWaterlandingCharacteristics.usgNumberField_Zg.Value()));
				trueWaterLanding.CalcStabilityArms(trueWaterLanding.GPos);
			}
			catch
			{
			}
			oleDbConnection.Close();
		}
		RedrawAll();
	}

	public void projectWaterlandingCharacteristics_NeedUpdate_MassGpos(object sender, EventArgs e)
	{
		using (new frmPleaseWait())
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile);
			oleDbConnection.Open();
			try
			{
				trueWaterLanding = new Waterlanding_MassGpos(projectWaterlandingCharacteristics.usgNumberField_Mass.Value(), new p3d(projectWaterlandingCharacteristics.usgNumberField_Xg.Value(), projectWaterlandingCharacteristics.usgNumberField_Yg.Value(), projectWaterlandingCharacteristics.usgNumberField_Zg.Value()), stations, 0.001f, aquaDensity);
				trueWaterLanding.SetGPosition(new p3d(projectWaterlandingCharacteristics.usgNumberField_Xg.Value(), projectWaterlandingCharacteristics.usgNumberField_Yg.Value(), projectWaterlandingCharacteristics.usgNumberField_Zg.Value()));
				trueWaterLanding.CalcStabilityArms(trueWaterLanding.GPos);
			}
			catch
			{
			}
			oleDbConnection.Close();
		}
		RedrawAll();
	}

	private void DrawLongSectionsWL()
	{
		try
		{
			using (new frmPleaseWait())
			{
				Progress.status = "Вывод продольных сечений формы корпуса...";
				Progress.ProgressMaximum = 0;
				usgModelFrame_Sections.ClearDB("broadSectionWL");
				string text = "";
				string text2 = "";
				using (JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile))
				{
					oleDbConnection.Open();
					string text3 = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
					text = GetProjectElements.GetSectionDescription(ProjectSectionDefinition.SectionType.Long, oleDbConnection);
					text2 = GetProjectElements.GetSectionDescription(ProjectSectionDefinition.SectionType.Transv, oleDbConnection);
					oleDbConnection.Close();
				}
				p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(stations);
				int num = 1;
				try
				{
					string[] array = text.Split(';');
					string[] array2 = array;
					foreach (string text4 in array2)
					{
						Progress.ProgressMaximum++;
					}
					array2 = array;
					foreach (string text4 in array2)
					{
						if (text4.Trim() != "")
						{
							Progress.ProgressPosition++;
							try
							{
								DrawProjectElements.DrawWL(usgModelFrame_Sections, trueWaterLanding, "broadSectionWL" + text4.Split(':')[1] + "(" + num + ")", DrawProjectElements.Planes.XZ, Converting.TextToFloat(text4.Split(':')[0]), Color.Transparent);
							}
							catch
							{
							}
							usgModelFrame_Sections.MoveGroup(new p2d(0f, 0f), new p2d(0f, -2f * gabariteOfVolume.Z * (float)num), new string[1] { "broadSectionWL" + text4.Split(':')[1] + "(" + num + ")" });
							num++;
						}
					}
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
	}

	private void DrawLongSectionsForm()
	{
		try
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
				p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(stations);
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
							DrawProjectElements.DrawBroadSide(usgModelFrame_Sections, stations, drawStationsSled: false, "", "", DrawAxes: true, "broadSectionBackForm" + num2, Color.LightGray);
							DrawProjectElements.DrawBroadSection(usgModelFrame_Sections, stations, Converting.TextToFloat(text3.Split(':')[0].Trim()), text3.Split(':')[1].Trim(), "", "broadSectionForm" + num2, textTransv);
							usgModelFrame_Sections.MoveGroup(new p2d(0f, 0f), new p2d(0f, -2f * gabariteOfVolume.Z * (float)num), new string[2]
							{
								"broadSectionForm" + num2,
								"broadSectionBackForm" + num2
							});
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
		catch
		{
		}
	}

	private void DrawHullSectionsWL()
	{
		try
		{
			using (new frmPleaseWait())
			{
				Progress.status = "Вывод поперечных сечений формы корпуса...";
				Progress.ProgressMaximum = 0;
				usgModelFrame_Sections.ClearDB("transvSectionWL");
				string text = "";
				string text2 = "";
				using (JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile))
				{
					oleDbConnection.Open();
					string text3 = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
					text = GetProjectElements.GetSectionDescription(ProjectSectionDefinition.SectionType.Long, oleDbConnection);
					text2 = GetProjectElements.GetSectionDescription(ProjectSectionDefinition.SectionType.Transv, oleDbConnection);
					oleDbConnection.Close();
				}
				p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(stations);
				int num = 1;
				int num2 = 1;
				try
				{
					string[] array = text2.Split(';');
					string[] array2 = array;
					foreach (string text4 in array2)
					{
						Progress.ProgressMaximum++;
					}
					array2 = array;
					foreach (string text4 in array2)
					{
						if (text4.Trim() != "")
						{
							Progress.ProgressPosition++;
							try
							{
								DrawProjectElements.DrawWL(usgModelFrame_Sections, trueWaterLanding, "transvSectionWL" + text4.Split(':')[1] + "(" + num + ":" + num2 + ")", DrawProjectElements.Planes.YZ, Converting.TextToFloat(text4.Split(':')[0]), Color.Transparent);
							}
							catch
							{
							}
							usgModelFrame_Sections.MoveGroup(new p2d(0f, 0f), new p2d(gabariteOfVolume.X + 1.5f * gabariteOfVolume.Y * (float)num, -2f * gabariteOfVolume.Z * (float)num2), new string[1] { "transvSectionWL" + text4.Split(':')[1] + "(" + num + ":" + num2 + ")" });
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
		catch
		{
		}
	}

	private void DrawHullSectionsForm()
	{
		try
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
				p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(stations);
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
							DrawProjectElements.DrawHullSide(usgModelFrame_Sections, stations, drawAllStations: false, text3.Split(':')[1].Trim(), "", DrawAxes: true, "transvSectionFormBack" + num3, Color.LightGray);
							DrawProjectElements.DrawHullSection(usgModelFrame_Sections, stations, Converting.TextToFloat(text3.Split(':')[0].Trim()), "", "", "transvSectionForm" + num3, textLong);
							usgModelFrame_Sections.MoveGroup(new p2d(0f, 0f), new p2d(gabariteOfVolume.X + 1.5f * gabariteOfVolume.Y * (float)num, -2f * gabariteOfVolume.Z * (float)num2), new string[2]
							{
								"transvSectionForm" + num3,
								"transvSectionFormBack" + num3
							});
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
						DrawProjectElements.DrawСMPoint(usgModelFrame_Waterlanding, new p3d(trueWaterLanding.CPos.X, trueWaterLanding.CPos.Y, trueWaterLanding.CPos.Z), "hull_side", DrawProjectElements.Planes.YZ);
						if (!projectWaterlandingCharacteristics.radioButton_dlb_dpb_trim.Checked && !projectWaterlandingCharacteristics.radioButton_dn_dk_dlb_dpb.Checked && !projectWaterlandingCharacteristics.radioButton_dn_dk_heel.Checked && !projectWaterlandingCharacteristics.radioButton_d_heel_trim.Checked)
						{
							DrawProjectElements.DrawWL(usgModelFrame_Waterlanding, trueWaterLanding, "broad_side", DrawProjectElements.Planes.XZ, 0f, Color.Transparent);
						}
						if (!projectWaterlandingCharacteristics.radioButton_dlb_dpb_trim.Checked && !projectWaterlandingCharacteristics.radioButton_dn_dk_dlb_dpb.Checked && !projectWaterlandingCharacteristics.radioButton_dn_dk_heel.Checked && !projectWaterlandingCharacteristics.radioButton_d_heel_trim.Checked)
						{
							DrawProjectElements.DrawWL(usgModelFrame_Waterlanding, trueWaterLanding, "hull_side", DrawProjectElements.Planes.YZ, trueWaterLanding.CPos.X, Color.Aqua);
						}
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
				p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(stations);
				try
				{
					usgModelFrame_Waterlanding.RotateGroup(new p2d(trueWaterLanding.GPos.Y, trueWaterLanding.GPos.Z), 0f - trueWaterLanding.HeelAngle, new string[2] { "hull_side", "hull_side_immersed" });
					usgModelFrame_Waterlanding.RotateGroup(new p2d(trueWaterLanding.GPos.X, trueWaterLanding.GPos.Z), 0f - trueWaterLanding.TrimAngle, new string[2] { "broad_side", "broad_side_immersed" });
				}
				catch
				{
				}
				usgModelFrame_Waterlanding.MoveGroup(new p2d(0f, 0f), new p2d(gabariteOfVolume.X + 1.5f * gabariteOfVolume.Y, 0f), new string[2] { "hull_side", "hull_side_immersed" });
				usgModelFrame_Waterlanding.Redraw();
			}
		}
		catch
		{
		}
	}
}
