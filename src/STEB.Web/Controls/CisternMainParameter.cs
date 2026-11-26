using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Drawing;
using System.Windows.Forms;
using STEB2.Cisterns;
using STEB2.Converting;
using STEB2.DataBase;
using STEB2.Geometry;
using STEB2.GraphicsElement;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB2;

public class CisternMainParameter : UserControl
{
	public Cistern activeCst;

	public Label label10;

	public Label label11;

	public Label label4;

	public Label label5;

	public Label label9;

	public Label label12;

	public Label label13;

	public usgModelFrame usgModelFrame;

	public Label label1;

	public Label label2;

	public Label label3;

	public Label label6;

	public Label label7;

	public Label label8;

	public Label label14;

	public Label label15;

	public Label label16;

	public Label label17;

	public Label label18;

	public Label label19;

	public Label label20;

	public usgNumberField usgNumberField_L;

	public usgNumberField usgNumberField_B;

	public usgNumberField usgNumberField_H;

	public usgNumberField usgNumberField_kZ;

	public usgNumberField usgNumberField_kY;

	public usgNumberField usgNumberField_kX;

	public usgNumberField usgNumberField_z1;

	public usgNumberField usgNumberField_y1;

	public usgNumberField usgNumberField_x1;

	public usgNumberField usgNumberField_z0;

	public usgNumberField usgNumberField_y0;

	public usgNumberField usgNumberField_x0;

	public RichTextBox richTextBox;

	public usgNumberField usgNumberField_percOfVolume;

	public TextBox textBox_Name;

	public usgNumberField usgNumberField_CisternDensity;

	public usgNumberField usgNumberField_CalcVolume;

	public usgNumberField usgNumberField_CalcMass;

	public usgNumberField usgNumberField_CalcWaterLevel;

	public usgNumberField usgNumberField_MaxVolume;

	public usgNumberField usgNumberField_MaxMass;

	private void SetEvents()
	{
		textBox_Name.PreviewKeyDown += textBox_Name_PreviewKeyDown;
		usgNumberField_CisternDensity.PreviewKeyDown += usgNumberField_CisternDensity_PreviewKeyDown;
		usgNumberField_L.PreviewKeyDown += usgNumberField_L_PreviewKeyDown;
		usgNumberField_B.PreviewKeyDown += usgNumberField_B_PreviewKeyDown;
		usgNumberField_H.PreviewKeyDown += usgNumberField_H_PreviewKeyDown;
		usgNumberField_x0.PreviewKeyDown += usgNumberField_x0_PreviewKeyDown;
		usgNumberField_x1.PreviewKeyDown += usgNumberField_x1_PreviewKeyDown;
		usgNumberField_y0.PreviewKeyDown += usgNumberField_y0_PreviewKeyDown;
		usgNumberField_y1.PreviewKeyDown += usgNumberField_y1_PreviewKeyDown;
		usgNumberField_z0.PreviewKeyDown += usgNumberField_z0_PreviewKeyDown;
		usgNumberField_z1.PreviewKeyDown += usgNumberField_z1_PreviewKeyDown;
		usgNumberField_CalcVolume.PreviewKeyDown += usgNumberField_CalcVolume_PreviewKeyDown;
		usgNumberField_CalcMass.PreviewKeyDown += usgNumberField_CalcMass_PreviewKeyDown;
		usgNumberField_CalcWaterLevel.PreviewKeyDown += usgNumberField_CalcWaterLevel_PreviewKeyDown;
		usgNumberField_percOfVolume.PreviewKeyDown += usgNumberField_percOfVolume_PreviewKeyDown;
		richTextBox.PreviewKeyDown += richTextBox_PreviewKeyDown;
		textBox_Name.LostFocus += textBox_Name_LostFocus;
		usgNumberField_CisternDensity.LostFocus += usgNumberField_CisternDensity_LostFocus;
		usgNumberField_L.LostFocus += usgNumberField_L_LostFocus;
		usgNumberField_B.LostFocus += usgNumberField_B_LostFocus;
		usgNumberField_H.LostFocus += usgNumberField_H_LostFocus;
		usgNumberField_x0.LostFocus += usgNumberField_x0_LostFocus;
		usgNumberField_x1.LostFocus += usgNumberField_x1_LostFocus;
		usgNumberField_y0.LostFocus += usgNumberField_y0_LostFocus;
		usgNumberField_y1.LostFocus += usgNumberField_y1_LostFocus;
		usgNumberField_z0.LostFocus += usgNumberField_z0_LostFocus;
		usgNumberField_z1.LostFocus += usgNumberField_z1_LostFocus;
		usgNumberField_CalcVolume.LostFocus += usgNumberField_CalcVolume_LostFocus;
		usgNumberField_CalcMass.LostFocus += usgNumberField_CalcMass_LostFocus;
		usgNumberField_CalcWaterLevel.LostFocus += usgNumberField_CalcWaterLevel_LostFocus;
		usgNumberField_percOfVolume.LostFocus += usgNumberField_percOfVolume_LostFocus;
		richTextBox.LostFocus += richTextBox_LostFocus;
	}

	public void richTextBox_LostFocus(object sender, EventArgs e)
	{
		activeCst.TextDecriptionOfStation = richTextBox.Text;
		RedrawCistern();
	}

	public void usgNumberField_percOfVolume_LostFocus(object sender, EventArgs e)
	{
		activeCst.percentOfVolume = STEB2.Converting.Converting.TextToFloat(usgNumberField_percOfVolume.Text);
		RedrawCistern();
	}

	public void usgNumberField_CalcWaterLevel_LostFocus(object sender, EventArgs e)
	{
		activeCst.waterLevel = STEB2.Converting.Converting.TextToFloat(usgNumberField_CalcWaterLevel.Text);
		RedrawCistern();
	}

	public void usgNumberField_CalcMass_LostFocus(object sender, EventArgs e)
	{
		activeCst.CalcMass = STEB2.Converting.Converting.TextToFloat(usgNumberField_CalcMass.Text);
		RedrawCistern();
	}

	public void usgNumberField_CalcVolume_LostFocus(object sender, EventArgs e)
	{
		activeCst.CalcVolume = STEB2.Converting.Converting.TextToFloat(usgNumberField_CalcVolume.Text);
		RedrawCistern();
	}

	public void usgNumberField_z1_LostFocus(object sender, EventArgs e)
	{
		activeCst.z1 = STEB2.Converting.Converting.TextToFloat(usgNumberField_z1.Text);
		RedrawCistern();
	}

	public void usgNumberField_z0_LostFocus(object sender, EventArgs e)
	{
		activeCst.z0 = STEB2.Converting.Converting.TextToFloat(usgNumberField_z0.Text);
		RedrawCistern();
	}

	public void usgNumberField_y1_LostFocus(object sender, EventArgs e)
	{
		activeCst.y1 = STEB2.Converting.Converting.TextToFloat(usgNumberField_y1.Text);
		RedrawCistern();
	}

	public void usgNumberField_y0_LostFocus(object sender, EventArgs e)
	{
		activeCst.y0 = STEB2.Converting.Converting.TextToFloat(usgNumberField_y0.Text);
		RedrawCistern();
	}

	public void usgNumberField_x1_LostFocus(object sender, EventArgs e)
	{
		activeCst.x1 = STEB2.Converting.Converting.TextToFloat(usgNumberField_x1.Text);
		RedrawCistern();
	}

	public void usgNumberField_x0_LostFocus(object sender, EventArgs e)
	{
		activeCst.x0 = STEB2.Converting.Converting.TextToFloat(usgNumberField_x0.Text);
		RedrawCistern();
	}

	public void usgNumberField_H_LostFocus(object sender, EventArgs e)
	{
		activeCst.h = STEB2.Converting.Converting.TextToFloat(usgNumberField_H.Text);
		RedrawCistern();
	}

	public void usgNumberField_B_LostFocus(object sender, EventArgs e)
	{
		activeCst.b = STEB2.Converting.Converting.TextToFloat(usgNumberField_B.Text);
		RedrawCistern();
	}

	public void usgNumberField_L_LostFocus(object sender, EventArgs e)
	{
		activeCst.l = STEB2.Converting.Converting.TextToFloat(usgNumberField_L.Text);
		RedrawCistern();
	}

	public void usgNumberField_CisternDensity_LostFocus(object sender, EventArgs e)
	{
		activeCst.volumeDensity = STEB2.Converting.Converting.TextToFloat(usgNumberField_CisternDensity.Text);
		RedrawCistern();
	}

	public void textBox_Name_LostFocus(object sender, EventArgs e)
	{
		activeCst.Name = textBox_Name.Text;
		RedrawCistern();
	}

	private void FillTextBoxes()
	{
		textBox_Name.Text = activeCst.Name;
		usgNumberField_CisternDensity.Text = activeCst.volumeDensity.ToString();
		usgNumberField_L.Text = activeCst.l.ToString();
		usgNumberField_B.Text = activeCst.b.ToString();
		usgNumberField_H.Text = activeCst.h.ToString();
		usgNumberField_x0.Text = activeCst.x0.ToString();
		usgNumberField_x1.Text = activeCst.x1.ToString();
		usgNumberField_y0.Text = activeCst.y0.ToString();
		usgNumberField_y1.Text = activeCst.y1.ToString();
		usgNumberField_z0.Text = activeCst.z0.ToString();
		usgNumberField_z1.Text = activeCst.z1.ToString();
		usgNumberField_CalcVolume.Text = activeCst.CalcVolume.ToString();
		usgNumberField_CalcMass.Text = activeCst.CalcMass.ToString();
		usgNumberField_CalcWaterLevel.Text = activeCst.waterLevel.ToString();
		usgNumberField_percOfVolume.Text = activeCst.percentOfVolume.ToString();
		usgNumberField_MaxMass.Text = activeCst.maxMass.ToString();
		usgNumberField_MaxVolume.Text = activeCst.maxVolume.ToString();
		richTextBox.Text = activeCst.TextDecriptionOfStation.ToString();
	}

	private void RedrawCistern()
	{
		usgModelFrame.ClearDB();
		List<Station> list = new List<Station>();
		using (JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile))
		{
			oleDbConnection.Open();
			string shipFormName = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
			list = GetProjectElements.GetProjectStation(shipFormName, oleDbConnection);
			oleDbConnection.Close();
		}
		DrawProjectElements.DrawBroadSide(usgModelFrame, list, drawStationsSled: false, "Проекция бок", "", DrawAxes: false, "broadSide", Color.White);
		DrawProjectElements.DrawBroadSide(usgModelFrame, activeCst.Stations, drawStationsSled: false, activeCst.Name, "", DrawAxes: false, "cisternBroadSide", Color.Aqua);
		DrawProjectElements.DrawHullSide(usgModelFrame, list, drawAllStations: false, "Проекция корпус", "", DrawAxes: false, "hullSide", Color.White);
		DrawProjectElements.DrawHullSide(usgModelFrame, activeCst.Stations, drawAllStations: false, activeCst.Name, "", DrawAxes: false, "hullSide", Color.Aqua);
		try
		{
			List<Station> list2 = new List<Station>();
			ImmersedStation[] immersedStations = activeCst.wtrl.immersedStations;
			foreach (ImmersedStation immersedStation in immersedStations)
			{
				list2.Add(new Station(immersedStation.StationContur, immersedStation.X));
			}
			DrawProjectElements.DrawHullSide(usgModelFrame, list2, drawAllStations: false, "", "V = " + activeCst.wtrl.V + " куб.м.", DrawAxes: false, "hullSide", Color.Blue);
			DrawProjectElements.DrawBroadSide(usgModelFrame, list2, drawStationsSled: false, "", "V = " + activeCst.wtrl.V + " куб.м.", DrawAxes: false, "cisternBroadSide", Color.Blue);
		}
		catch
		{
		}
		usgModelFrame.MoveGroup(new p2d(0f, 0f), new p2d(STEB2.Volume.Tools.GetGabariteOfVolume(list).X + STEB2.Volume.Tools.GetGabariteOfVolume(list).Y, 0f), new string[1] { "hullSide" });
		usgModelFrame.Redraw();
		FillTextBoxes();
	}

	public CisternMainParameter(List<Cistern> listOfCisterns)
	{
		activeCst = new Cistern("Новая цистерна", 1f, 1f, 1f, "new 0 \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 new 1 \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 new 2 \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 new 3 \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 new 4 \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 new 5 \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 new 6 \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 new 7 \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 new 8 \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 new 9 \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 new 10 \n 0 0 \n 1 0 \n 1 1 \n 0 1\n 0 0 ", 1f);
		InitializeComponent();
		SetEvents();
		RedrawCistern();
	}

	public CisternMainParameter(Cistern activeCst, List<Cistern> listOfCisterns)
	{
		this.activeCst = activeCst;
		InitializeComponent();
		SetEvents();
		RedrawCistern();
	}

	public void textBox_Name_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.Name = textBox_Name.Text;
			RedrawCistern();
		}
	}

	public void richTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.TextDecriptionOfStation = richTextBox.Text;
			RedrawCistern();
		}
	}

	public void usgNumberField_percOfVolume_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.percentOfVolume = STEB2.Converting.Converting.TextToFloat(usgNumberField_percOfVolume.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_CalcWaterLevel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.waterLevel = STEB2.Converting.Converting.TextToFloat(usgNumberField_CalcWaterLevel.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_CalcMass_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.CalcMass = STEB2.Converting.Converting.TextToFloat(usgNumberField_CalcMass.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_CalcVolume_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.CalcVolume = STEB2.Converting.Converting.TextToFloat(usgNumberField_CalcVolume.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_z1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.z1 = STEB2.Converting.Converting.TextToFloat(usgNumberField_z1.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_z0_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.z0 = STEB2.Converting.Converting.TextToFloat(usgNumberField_z0.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_y1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.y1 = STEB2.Converting.Converting.TextToFloat(usgNumberField_y1.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_y0_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.y0 = STEB2.Converting.Converting.TextToFloat(usgNumberField_y0.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_x1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.x1 = STEB2.Converting.Converting.TextToFloat(usgNumberField_x1.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_x0_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.x0 = STEB2.Converting.Converting.TextToFloat(usgNumberField_x0.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_H_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.h = STEB2.Converting.Converting.TextToFloat(usgNumberField_H.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_B_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.b = STEB2.Converting.Converting.TextToFloat(usgNumberField_B.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_L_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.l = STEB2.Converting.Converting.TextToFloat(usgNumberField_L.Text);
			RedrawCistern();
		}
	}

	public void usgNumberField_CisternDensity_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			activeCst.volumeDensity = STEB2.Converting.Converting.TextToFloat(usgNumberField_CisternDensity.Text);
			RedrawCistern();
		}
	}

	private void InitializeComponent()
	{
		this.label10 = new System.Windows.Forms.Label();
		this.textBox_Name = new System.Windows.Forms.TextBox();
		this.label11 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.usgNumberField_CalcWaterLevel = new STEB2.usgNumberField();
		this.usgNumberField_CalcMass = new STEB2.usgNumberField();
		this.usgNumberField_CalcVolume = new STEB2.usgNumberField();
		this.usgNumberField_CisternDensity = new STEB2.usgNumberField();
		this.label12 = new System.Windows.Forms.Label();
		this.usgNumberField_MaxVolume = new STEB2.usgNumberField();
		this.label13 = new System.Windows.Forms.Label();
		this.usgNumberField_MaxMass = new STEB2.usgNumberField();
		this.usgModelFrame = new STEB2.usgModelFrame();
		this.usgNumberField_L = new STEB2.usgNumberField();
		this.label1 = new System.Windows.Forms.Label();
		this.usgNumberField_B = new STEB2.usgNumberField();
		this.label2 = new System.Windows.Forms.Label();
		this.usgNumberField_H = new STEB2.usgNumberField();
		this.label3 = new System.Windows.Forms.Label();
		this.usgNumberField_kZ = new STEB2.usgNumberField();
		this.label6 = new System.Windows.Forms.Label();
		this.usgNumberField_kY = new STEB2.usgNumberField();
		this.label7 = new System.Windows.Forms.Label();
		this.usgNumberField_kX = new STEB2.usgNumberField();
		this.label8 = new System.Windows.Forms.Label();
		this.usgNumberField_z1 = new STEB2.usgNumberField();
		this.label14 = new System.Windows.Forms.Label();
		this.usgNumberField_y1 = new STEB2.usgNumberField();
		this.label15 = new System.Windows.Forms.Label();
		this.usgNumberField_x1 = new STEB2.usgNumberField();
		this.label16 = new System.Windows.Forms.Label();
		this.usgNumberField_z0 = new STEB2.usgNumberField();
		this.label17 = new System.Windows.Forms.Label();
		this.usgNumberField_y0 = new STEB2.usgNumberField();
		this.label18 = new System.Windows.Forms.Label();
		this.usgNumberField_x0 = new STEB2.usgNumberField();
		this.label19 = new System.Windows.Forms.Label();
		this.label20 = new System.Windows.Forms.Label();
		this.usgNumberField_percOfVolume = new STEB2.usgNumberField();
		this.richTextBox = new System.Windows.Forms.RichTextBox();
		this.label10.Name = "label10";
		this.label10.Text = "Название цистерны:";
		this.textBox_Name.Name = "textBox_Name";
		this.label11.Name = "label11";
		this.label11.Text = "Плотность жидкости в цистерне, т/куб.м.:";
		this.label4.Name = "label4";
		this.label4.Text = "Расчетный объем в цистерне, куб.м.";
		this.label5.Name = "label5";
		this.label5.Text = "Расчетный вес жидкости в цистерне, т";
		this.label9.Name = "label9";
		this.label9.Text = "Расчетный уровень жидкости в цистерне, м";
		this.usgNumberField_CalcWaterLevel.Name = "usgNumberField_CalcWaterLevel";
		this.usgNumberField_CalcMass.Name = "usgNumberField_CalcMass";
		this.usgNumberField_CalcVolume.Name = "usgNumberField_CalcVolume";
		this.usgNumberField_CisternDensity.Name = "usgNumberField_CisternDensity";
		this.label12.Name = "label12";
		this.label12.Text = "Максимальный объем в цистерне, куб.м.";
		this.usgNumberField_MaxVolume.Name = "usgNumberField_MaxVolume";
		this.usgNumberField_MaxVolume.ReadOnly = true;
		this.label13.Name = "label13";
		this.label13.Text = "Максимальный вес жидкости в цистерне, т";
		this.usgNumberField_MaxMass.Name = "usgNumberField_MaxMass";
		this.usgNumberField_MaxMass.ReadOnly = true;
		this.usgModelFrame.ModelFrameCaption = "";
		this.usgModelFrame.Name = "usgModelFrame";
		this.usgNumberField_L.Name = "usgNumberField_L";
		this.usgNumberField_L.ReadOnly = true;
		this.label1.Name = "label1";
		this.label1.Text = "Длина, м:";
		this.usgNumberField_B.Name = "usgNumberField_B";
		this.usgNumberField_B.ReadOnly = true;
		this.label2.Name = "label2";
		this.label2.Text = "Ширина, м:";
		this.usgNumberField_H.Name = "usgNumberField_H";
		this.usgNumberField_H.ReadOnly = true;
		this.label3.Name = "label3";
		this.label3.Text = "Высота, м:";
		this.usgNumberField_kZ.Name = "usgNumberField_kZ";
		this.usgNumberField_kZ.ReadOnly = true;
		this.label6.Name = "label6";
		this.label6.Text = "kZ:";
		this.usgNumberField_kY.Name = "usgNumberField_kY";
		this.usgNumberField_kY.ReadOnly = true;
		this.label7.Name = "label7";
		this.label7.Text = "kY:";
		this.usgNumberField_kX.Name = "usgNumberField_kX";
		this.usgNumberField_kX.ReadOnly = true;
		this.label8.Name = "label8";
		this.label8.Text = "kX:";
		this.usgNumberField_z1.Name = "usgNumberField_z1";
		this.label14.Name = "label14";
		this.label14.Text = "z1, м:";
		this.usgNumberField_y1.Name = "usgNumberField_y1";
		this.label15.Name = "label15";
		this.label15.Text = "y1, м:";
		this.usgNumberField_x1.Name = "usgNumberField_x1";
		this.label16.Name = "label16";
		this.label16.Text = "x1, м:";
		this.usgNumberField_z0.Name = "usgNumberField_z0";
		this.label17.Name = "label17";
		this.label17.Text = "z0, м:";
		this.usgNumberField_y0.Name = "usgNumberField_y0";
		this.label18.Name = "label18";
		this.label18.Text = "y0, м:";
		this.usgNumberField_x0.Name = "usgNumberField_x0";
		this.label19.Name = "label19";
		this.label19.Text = "x0, м:";
		this.label20.Name = "label20";
		this.label20.Text = "% заполнения";
		this.usgNumberField_percOfVolume.Name = "usgNumberField_percOfVolume";
		this.richTextBox.Name = "richTextBox";
		this.richTextBox.Text = "";
	}
}
