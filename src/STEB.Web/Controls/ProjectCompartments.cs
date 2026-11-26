using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Drawing;
using System.Windows.Forms;
using STEB2.Compartments;
using STEB2.Converting;
using STEB2.DataBase;
using STEB2.Geometry;

namespace STEB2.Controls;

public class ProjectCompartments : UserControl
{
	private bool NeedUpdating = true;

	public DataGridView dataGridView;

	public DataGridViewTextBoxColumn CompartmentName;

	public DataGridViewTextBoxColumn x0;

	public DataGridViewTextBoxColumn x1;

	public DataGridViewTextBoxColumn y0;

	public DataGridViewTextBoxColumn y1;

	public DataGridViewTextBoxColumn z0;

	public DataGridViewTextBoxColumn z1;

	public DataGridViewTextBoxColumn volumeDensity;

	public DataGridViewTextBoxColumn percentOfVolume;

	public DataGridViewTextBoxColumn coefficientPronicaem;

	public event EventHandler compartmentChanged;

	public ProjectCompartments()
	{
		InitializeComponent();
		dataGridView.CellEndEdit += dataGridView_CellEndEdit;
		dataGridView.RowsAdded += dataGridView_RowsAdded;
		dataGridView.RowsRemoved += dataGridView_RowsRemoved;
		try
		{
			LoadRows();
		}
		catch
		{
		}
	}

	public void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
	{
		SyncronizeDatabase();
	}

	public void dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
	{
		SyncronizeDatabase();
	}

	public void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
	{
		SyncronizeDatabase();
	}

	public List<Compartment> GetCompartment()
	{
		List<Compartment> list = new List<Compartment>();
		foreach (DataGridViewRow item in (IEnumerable)dataGridView.Rows)
		{
			try
			{
				float num = STEB2.Converting.Converting.TextToFloat(item.Cells["x0"].Value.ToString());
				float num2 = STEB2.Converting.Converting.TextToFloat(item.Cells["x1"].Value.ToString());
				float num3 = STEB2.Converting.Converting.TextToFloat(item.Cells["y0"].Value.ToString());
				float num4 = STEB2.Converting.Converting.TextToFloat(item.Cells["y1"].Value.ToString());
				float num5 = STEB2.Converting.Converting.TextToFloat(item.Cells["z0"].Value.ToString());
				float num6 = STEB2.Converting.Converting.TextToFloat(item.Cells["z1"].Value.ToString());
				float num7 = Math.Min(num, num2);
				float num8 = Math.Min(num3, num4) + Math.Abs(num4 - num3) / 2f;
				float z = Math.Min(num5, num6);
				float l = Math.Abs(num2 - num);
				float b = Math.Abs(num4 - num3);
				float h = Math.Abs(num6 - num5);
				list.Add(new Compartment(item.Cells["Name"].Value.ToString(), l, b, h, new p3d(num7, num8, z), STEB2.Converting.Converting.TextToFloat(item.Cells["volumeDensity"].Value.ToString()), STEB2.Converting.Converting.TextToFloat(item.Cells["percentOfVolume"].Value.ToString()), STEB2.Converting.Converting.TextToFloat(item.Cells["coefficientPronicaem"].Value.ToString())));
			}
			catch
			{
			}
		}
		return list;
	}

	public void LoadRows()
	{
		using (JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile))
		{
			oleDbConnection.Open();
			using (JsonDbCommand oleDbCommand = new JsonDbCommand("SELECT * FROM tblCompartments;", oleDbConnection))
			{
				using JsonDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
				NeedUpdating = false;
				while (oleDbDataReader.Read())
				{
					float num = STEB2.Converting.Converting.TextToFloat(oleDbDataReader["l"].ToString());
					float num2 = STEB2.Converting.Converting.TextToFloat(oleDbDataReader["b"].ToString());
					float num3 = STEB2.Converting.Converting.TextToFloat(oleDbDataReader["h"].ToString());
					float num4 = STEB2.Converting.Converting.TextToFloat(oleDbDataReader["x"].ToString());
					float num5 = STEB2.Converting.Converting.TextToFloat(oleDbDataReader["y"].ToString());
					float num6 = STEB2.Converting.Converting.TextToFloat(oleDbDataReader["z"].ToString());
					float num7 = num4;
					float num8 = num4 + num;
					float num9 = num5 - num2 / 2f;
					float num10 = num5 + num2 / 2f;
					float num11 = num6;
					float num12 = num6 + num3;
					dataGridView.Rows.Add(oleDbDataReader["Name"].ToString(), num7.ToString(), num8.ToString(), num9.ToString(), num10.ToString(), num11.ToString(), num12.ToString(), oleDbDataReader["volumeDensity"].ToString(), oleDbDataReader["percentOfVolume"].ToString(), oleDbDataReader["coefficientPronicaem"].ToString());
				}
				oleDbDataReader.Close();
			}
			oleDbConnection.Close();
		}
		NeedUpdating = true;
		if (compartmentChanged != null)
		{
			compartmentChanged(new object(), new EventArgs());
		}
	}

	private void SyncronizeDatabase()
	{
		if (NeedUpdating)
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile);
			oleDbConnection.Open();
			Tools.ExecuteSQL("DELETE * FROM tblCompartments;", oleDbConnection);
			List<string> list = new List<string>();
			foreach (DataGridViewRow item in (IEnumerable)dataGridView.Rows)
			{
				try
				{
					float num = STEB2.Converting.Converting.TextToFloat(item.Cells["x0"].Value.ToString());
					float num2 = STEB2.Converting.Converting.TextToFloat(item.Cells["x1"].Value.ToString());
					float num3 = STEB2.Converting.Converting.TextToFloat(item.Cells["y0"].Value.ToString());
					float num4 = STEB2.Converting.Converting.TextToFloat(item.Cells["y1"].Value.ToString());
					float num5 = STEB2.Converting.Converting.TextToFloat(item.Cells["z0"].Value.ToString());
					float num6 = STEB2.Converting.Converting.TextToFloat(item.Cells["z1"].Value.ToString());
					float num7 = Math.Min(num, num2);
					float num8 = Math.Min(num3, num4) + Math.Abs(num4 - num3) / 2f;
					float num9 = Math.Min(num5, num6);
					float num10 = Math.Abs(num2 - num);
					float num11 = Math.Abs(num4 - num3);
					float num12 = Math.Abs(num6 - num5);
					list.Add("INSERT INTO tblCompartments (Name,l,b,h,x,y,z,volumeDensity,percentOfVolume,coefficientPronicaem) VALUES('" + item.Cells["Name"].Value.ToString() + "'," + num10.ToString().Replace(',', '.') + "," + num11.ToString().Replace(',', '.') + "," + num12.ToString().Replace(',', '.') + "," + num7.ToString().Replace(',', '.') + "," + num8.ToString().Replace(',', '.') + "," + num9.ToString().Replace(',', '.') + "," + item.Cells["volumeDensity"].Value.ToString().Replace(',', '.') + "," + item.Cells["percentOfVolume"].Value.ToString().Replace(',', '.') + "," + item.Cells["coefficientPronicaem"].Value.ToString().Replace(',', '.') + ")");
				}
				catch
				{
				}
			}
			Tools.ExecuteRangeSQL(list, oleDbConnection);
			oleDbConnection.Close();
		}
		if (compartmentChanged != null)
		{
			compartmentChanged(new object(), new EventArgs());
		}
	}

	private void InitializeComponent()
	{
		this.dataGridView = new System.Windows.Forms.DataGridView();
		this.CompartmentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.x0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.x1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.y0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.y1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.z0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.z1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.volumeDensity = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.percentOfVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.coefficientPronicaem = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridView.Columns.AddRange(this.CompartmentName, this.x0, this.x1, this.y0, this.y1, this.z0, this.z1, this.volumeDensity, this.percentOfVolume, this.coefficientPronicaem);
		this.dataGridView.Name = "dataGridView";
		this.CompartmentName.HeaderText = "Имя отсека";
		this.CompartmentName.Name = "Name";
		this.x0.HeaderText = "x0, м";
		this.x0.Name = "x0";
		this.x0.ToolTipText = "Начало отсека по длине, м";
		this.x1.HeaderText = "x1, м";
		this.x1.Name = "x1";
		this.x1.ToolTipText = "Конец отсека по длине, м";
		this.y0.HeaderText = "y0, м";
		this.y0.Name = "y0";
		this.y0.ToolTipText = "Начало отсека по ширине, м";
		this.y1.HeaderText = "y1, м";
		this.y1.Name = "y1";
		this.y1.ToolTipText = "Конец отсека по ширине, м";
		this.z0.HeaderText = "z0, м";
		this.z0.Name = "z0";
		this.z0.ToolTipText = "Начало отсека по высоте, м";
		this.z1.HeaderText = "z1, м";
		this.z1.Name = "z1";
		this.z1.ToolTipText = "Конец отсека по высоте, м";
		this.volumeDensity.HeaderText = "Плотность, т/куб.м.";
		this.volumeDensity.Name = "volumeDensity";
		this.volumeDensity.ToolTipText = "Плотность жидкости, которая может быть в отсеке, т/куб.м.";
		this.percentOfVolume.HeaderText = "Заполненность, %";
		this.percentOfVolume.Name = "percentOfVolume";
		this.percentOfVolume.ToolTipText = "Заполненность отсека, %";
		this.coefficientPronicaem.HeaderText = "k";
		this.coefficientPronicaem.Name = "coefficientPronicaem";
		this.coefficientPronicaem.ToolTipText = "Коэффициент проницаемости от 0 до 1";
	}
}
