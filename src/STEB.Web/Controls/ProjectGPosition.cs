using System;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Drawing;
using System.Windows.Forms;
using STEB2.DataBase;

namespace STEB2.Controls;

public class ProjectGPosition : UserControl
{
	private bool NeedUpdating = true;

	public usgNumberField usgNumberField_Zg;

	public Label label16;

	public usgNumberField usgNumberField_Mass;

	public Label label18;

	public Label label15;

	public usgNumberField usgNumberField_Xg;

	public Label label17;

	public usgNumberField usgNumberField_Yg;

	public event EventHandler GChanged;

	public ProjectGPosition()
	{
		InitializeComponent();
		usgNumberField_Mass.TextChanged += usgNumberField_Mass_TextChanged;
		usgNumberField_Xg.TextChanged += usgNumberField_Xg_TextChanged;
		usgNumberField_Yg.TextChanged += usgNumberField_Yg_TextChanged;
		usgNumberField_Zg.TextChanged += usgNumberField_Zg_TextChanged;
		try
		{
			LoadRows();
		}
		catch
		{
		}
	}

	public void usgNumberField_Zg_TextChanged(object sender, EventArgs e)
	{
		if (GChanged != null)
		{
			GChanged(new object(), new EventArgs());
		}
		SyncronizeDatabase();
	}

	public void usgNumberField_Yg_TextChanged(object sender, EventArgs e)
	{
		if (GChanged != null)
		{
			GChanged(new object(), new EventArgs());
		}
		SyncronizeDatabase();
	}

	public void usgNumberField_Xg_TextChanged(object sender, EventArgs e)
	{
		if (GChanged != null)
		{
			GChanged(new object(), new EventArgs());
		}
		SyncronizeDatabase();
	}

	public void usgNumberField_Mass_TextChanged(object sender, EventArgs e)
	{
		if (GChanged != null)
		{
			GChanged(new object(), new EventArgs());
		}
		SyncronizeDatabase();
	}

	public void LoadRows()
	{
		// Fix (bug present in the 2010 build): filling the fields raised TextChanged -> SyncronizeDatabase(),
		// which saved the still-empty Xg/Yg/Zg as 0 before they were read. Suspend saving while loading,
		// as ProjectCompartments/ProjectContainers already do.
		NeedUpdating = false;
		using (JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile))
		{
			oleDbConnection.Open();
			string text = Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
			usgNumberField_Mass.Text = Tools.ReturnScalar("SELECT * FROM tblShipForms WHERE ShipFormName = '" + text + "'", "ShipFormMass", oleDbConnection).ToString();
			usgNumberField_Xg.Text = Tools.ReturnScalar("SELECT * FROM tblShipForms WHERE ShipFormName = '" + text + "'", "Xg", oleDbConnection).ToString();
			usgNumberField_Yg.Text = Tools.ReturnScalar("SELECT * FROM tblShipForms WHERE ShipFormName = '" + text + "'", "Yg", oleDbConnection).ToString();
			usgNumberField_Zg.Text = Tools.ReturnScalar("SELECT * FROM tblShipForms WHERE ShipFormName = '" + text + "'", "Zg", oleDbConnection).ToString();
			oleDbConnection.Close();
		}
		NeedUpdating = true;
		if (GChanged != null)
		{
			GChanged(new object(), new EventArgs());
		}
	}

	private void SyncronizeDatabase()
	{
		if (NeedUpdating)
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile);
			oleDbConnection.Open();
			string text = Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
			Tools.ExecuteSQL("UPDATE tblShipForms SET ShipFormMass =  '" + usgNumberField_Mass.Value() + "' WHERE ShipFormName = '" + text + "'", oleDbConnection);
			Tools.ExecuteSQL("UPDATE tblShipForms SET Xg =  '" + usgNumberField_Xg.Value() + "' WHERE ShipFormName = '" + text + "'", oleDbConnection);
			Tools.ExecuteSQL("UPDATE tblShipForms SET Yg =  '" + usgNumberField_Yg.Value() + "' WHERE ShipFormName = '" + text + "'", oleDbConnection);
			Tools.ExecuteSQL("UPDATE tblShipForms SET Zg =  '" + usgNumberField_Zg.Value() + "' WHERE ShipFormName = '" + text + "'", oleDbConnection);
			oleDbConnection.Close();
		}
		if (GChanged != null)
		{
			GChanged(new object(), new EventArgs());
		}
	}

	private void InitializeComponent()
	{
		this.usgNumberField_Zg = new STEB2.usgNumberField();
		this.label16 = new System.Windows.Forms.Label();
		this.usgNumberField_Mass = new STEB2.usgNumberField();
		this.label18 = new System.Windows.Forms.Label();
		this.label15 = new System.Windows.Forms.Label();
		this.usgNumberField_Xg = new STEB2.usgNumberField();
		this.label17 = new System.Windows.Forms.Label();
		this.usgNumberField_Yg = new STEB2.usgNumberField();
		this.usgNumberField_Zg.Name = "usgNumberField_Zg";
		this.label16.Name = "label16";
		this.label16.Text = "Вес судна, т:";
		this.usgNumberField_Mass.Name = "usgNumberField_Mass";
		this.label18.Name = "label18";
		this.label18.Text = "Xg, м";
		this.label15.Name = "label15";
		this.label15.Text = "Zg, м";
		this.usgNumberField_Xg.Name = "usgNumberField_Xg";
		this.label17.Name = "label17";
		this.label17.Text = "Yg, м";
		this.usgNumberField_Yg.Name = "usgNumberField_Yg";
	}
}
