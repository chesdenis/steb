using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Drawing;
using System.Windows.Forms;
using STEB2.Converting;
using STEB2.DataBase;
using STEB2.Geometry;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB2.Controls;

public class Forms : UserControl
{

	public FormMainDimension formMainDimension;

	public FormDescription formDescription;

	public ToolStripButton toolStripButton_SaveForm;

	public Forms()
	{
		InitializeComponent();
		SetEvents();
		InitializeFormDescription();
	}

	private void SetEvents()
	{
		toolStripButton_SaveForm.Click += toolStripButton_SaveForm_Click;
		formMainDimension.UpdateTexts += formMainDimension_UpdateTexts;
		formDescription.FormDescriptionChanged += formDescription_FormDescriptionChanged;
	}

	public void formDescription_FormDescriptionChanged(object sender, EventArgs e)
	{
		Cursor = Cursors.WaitCursor;
		List<Station> inputStations = STEB2.Volume.Tools.TextToStations(formDescription.richTextBox_FormTextView.Text);
		float num = formMainDimension.usgNumberField_L.Value();
		float num2 = formMainDimension.usgNumberField_B.Value();
		float num3 = formMainDimension.usgNumberField_H.Value();
		formMainDimension.usgNumberField_d.Value();
		float kx = formMainDimension.usgNumberField_kX.Value();
		float ky = formMainDimension.usgNumberField_kY.Value();
		float kz = formMainDimension.usgNumberField_kZ.Value();
		if (num > 0f && num2 > 0f && num3 > 0f)
		{
			p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(inputStations);
			kx = num / gabariteOfVolume.X;
			ky = num2 / gabariteOfVolume.Y;
			kz = num3 / gabariteOfVolume.Z;
			formMainDimension.usgNumberField_kX.Text = kx.ToString();
			formMainDimension.usgNumberField_kY.Text = ky.ToString();
			formMainDimension.usgNumberField_kZ.Text = kz.ToString();
		}
		inputStations = STEB2.Volume.Tools.GetAffinnStations(inputStations, num, num2, num3, kx, ky, kz);
		formDescription.SetForm(inputStations);
		Cursor = Cursors.Default;
	}

	public void formMainDimension_UpdateTexts(object sender, EventArgs e)
	{
		Cursor = Cursors.WaitCursor;
		List<Station> inputStations = STEB2.Volume.Tools.TextToStations(formDescription.richTextBox_FormTextView.Text);
		float num = formMainDimension.usgNumberField_L.Value();
		float num2 = formMainDimension.usgNumberField_B.Value();
		float num3 = formMainDimension.usgNumberField_H.Value();
		formMainDimension.usgNumberField_d.Value();
		float kx = formMainDimension.usgNumberField_kX.Value();
		float ky = formMainDimension.usgNumberField_kY.Value();
		float kz = formMainDimension.usgNumberField_kZ.Value();
		if (num > 0f && num2 > 0f && num3 > 0f)
		{
			p3d gabariteOfVolume = STEB2.Volume.Tools.GetGabariteOfVolume(inputStations);
			kx = num / gabariteOfVolume.X;
			ky = num2 / gabariteOfVolume.Y;
			kz = num3 / gabariteOfVolume.Z;
			formMainDimension.usgNumberField_kX.Text = kx.ToString();
			formMainDimension.usgNumberField_kY.Text = ky.ToString();
			formMainDimension.usgNumberField_kZ.Text = kz.ToString();
		}
		inputStations = STEB2.Volume.Tools.GetAffinnStations(inputStations, num, num2, num3, kx, ky, kz);
		formDescription.SetForm(inputStations);
		formDescription.usgModelFrame_FormDrawing.ZoomAll();
		Cursor = Cursors.Default;
	}

	public void toolStripButton_SaveForm_Click(object sender, EventArgs e)
	{
		Cursor = Cursors.WaitCursor;
		toolStripButton_SaveForm.Enabled = false;
		using (JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile))
		{
			oleDbConnection.Open();
			List<Station> list = STEB2.Volume.Tools.TextToStations(formDescription.richTextBox_FormTextView.Text);
			string text = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
			try
			{
				if (text != "")
				{
					STEB2.DataBase.Tools.ExecuteSQL("UPDATE tblShipForms SET L = " + formMainDimension.usgNumberField_L.SQLText() + " WHERE ShipFormName = '" + text + "'", oleDbConnection);
					STEB2.DataBase.Tools.ExecuteSQL("UPDATE tblShipForms SET B = " + formMainDimension.usgNumberField_B.SQLText() + " WHERE ShipFormName = '" + text + "'", oleDbConnection);
					STEB2.DataBase.Tools.ExecuteSQL("UPDATE tblShipForms SET H = " + formMainDimension.usgNumberField_H.SQLText() + " WHERE ShipFormName = '" + text + "'", oleDbConnection);
					STEB2.DataBase.Tools.ExecuteSQL("UPDATE tblShipForms SET d = " + formMainDimension.usgNumberField_d.SQLText() + " WHERE ShipFormName = '" + text + "'", oleDbConnection);
					STEB2.DataBase.Tools.ExecuteSQL("UPDATE tblShipForms SET kX = " + formMainDimension.usgNumberField_kX.SQLText() + " WHERE ShipFormName = '" + text + "'", oleDbConnection);
					STEB2.DataBase.Tools.ExecuteSQL("UPDATE tblShipForms SET kY = " + formMainDimension.usgNumberField_kY.SQLText() + " WHERE ShipFormName = '" + text + "'", oleDbConnection);
					STEB2.DataBase.Tools.ExecuteSQL("UPDATE tblShipForms SET kZ = " + formMainDimension.usgNumberField_kZ.SQLText() + " WHERE ShipFormName = '" + text + "'", oleDbConnection);
					STEB2.DataBase.Tools.ExecuteSQL("DELETE * FROM tblStations WHERE ShipFormName = '" + text + "'", oleDbConnection);
					foreach (Station item in list)
					{
						Application.DoEvents();
						STEB2.DataBase.Tools.ExecuteSQL("INSERT INTO tblStations (ShipFormName, X) VALUES ('" + text + "','" + item.X + "')", oleDbConnection);
						int num = (int)STEB2.DataBase.Tools.ReturnScalar("SELECT MAX(StationID) AS maxID FROM tblStations", "maxID", oleDbConnection);
						foreach (Contur listContur in item.StationContur.listConturs)
						{
							Application.DoEvents();
							STEB2.DataBase.Tools.ExecuteSQL("INSERT INTO tblStationsConturs(StationID) VALUES (" + num + ")", oleDbConnection);
							int num2 = (int)STEB2.DataBase.Tools.ReturnScalar("SELECT MAX(ConturID) AS maxID FROM tblStationsConturs", "maxID", oleDbConnection);
							List<string> list2 = new List<string>();
							foreach (p2d listPoint in listContur.listPoints)
							{
								string[] array = new string[7]
								{
									"INSERT INTO tblStationsContursPoints (ConturID, PointX, PointY) VALUES ('",
									num2.ToString(),
									"','",
									null,
									null,
									null,
									null
								};
								float num3 = listPoint.X;
								array[3] = num3.ToString();
								array[4] = "','";
								float num4 = listPoint.Y;
								array[5] = num4.ToString();
								array[6] = "')";
								list2.Add(string.Concat(array));
							}
							STEB2.DataBase.Tools.ExecuteRangeSQL(list2, oleDbConnection);
						}
					}
				}
			}
			catch
			{
			}
			oleDbConnection.Close();
		}
		toolStripButton_SaveForm.Enabled = true;
		Cursor = Cursors.Default;
	}

	private void InitializeFormDescription()
	{
		Cursor = Cursors.WaitCursor;
		using (JsonDbConnection oleDbConnection = new JsonDbConnection(STEB2.DataBase.Tools.cnnString + STEB2.DataBase.Tools.workFile))
		{
			oleDbConnection.Open();
			string text = STEB2.DataBase.Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
			formDescription.richTextBox_FormTextView.Text = STEB2.DataBase.Tools.ReturnShipFormText(text, oleDbConnection);
			string[] array = STEB2.DataBase.Tools.ReturnArrayOfResultByArrayOfFields("SELECT * FROM tblShipForms WHERE ShipFormName='" + text + "';", new string[7] { "L", "B", "H", "d", "kX", "kY", "kZ" }, oleDbConnection);
			formMainDimension.usgNumberField_L.Text = array[0].ToString();
			formMainDimension.usgNumberField_B.Text = array[1].ToString();
			formMainDimension.usgNumberField_H.Text = array[2].ToString();
			formMainDimension.usgNumberField_d.Text = array[3].ToString();
			formMainDimension.usgNumberField_kX.Text = array[4].ToString();
			formMainDimension.usgNumberField_kY.Text = array[5].ToString();
			formMainDimension.usgNumberField_kZ.Text = array[6].ToString();
			formDescription.SetForm(STEB2.Volume.Tools.GetAffinnStations(STEB2.Volume.Tools.TextToStations(formDescription.richTextBox_FormTextView.Text), STEB2.Converting.Converting.TextToFloat(formMainDimension.usgNumberField_L.Text), STEB2.Converting.Converting.TextToFloat(formMainDimension.usgNumberField_B.Text), STEB2.Converting.Converting.TextToFloat(formMainDimension.usgNumberField_H.Text), STEB2.Converting.Converting.TextToFloat(formMainDimension.usgNumberField_kX.Text), STEB2.Converting.Converting.TextToFloat(formMainDimension.usgNumberField_kY.Text), STEB2.Converting.Converting.TextToFloat(formMainDimension.usgNumberField_kZ.Text)));
			oleDbConnection.Close();
		}
		Cursor = Cursors.Default;
	}

	private void InitializeComponent()
	{
		this.toolStripButton_SaveForm = new System.Windows.Forms.ToolStripButton();
		this.formDescription = new STEB2.FormDescription();
		this.formMainDimension = new STEB2.Controls.FormMainDimension();
		this.toolStripButton_SaveForm.Name = "toolStripButton_SaveForm";
		this.toolStripButton_SaveForm.Text = "Сохранить\nформу корпуса...";
		this.formDescription.Name = "formDescription";
		this.formMainDimension.Name = "formMainDimension";
	}
}
