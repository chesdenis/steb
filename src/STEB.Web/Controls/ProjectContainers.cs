using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Drawing;
using System.Windows.Forms;
using STEB2.Containers;
using STEB2.Converting;
using STEB2.DataBase;
using STEB2.Geometry;

namespace STEB2.Controls;

public class ProjectContainers : UserControl
{
	private bool NeedUpdating = true;

	public DataGridView dataGridView;

	public DataGridViewTextBoxColumn ContainerName;

	public DataGridViewTextBoxColumn l;

	public DataGridViewTextBoxColumn b;

	public DataGridViewTextBoxColumn h;

	public DataGridViewTextBoxColumn x;

	public DataGridViewTextBoxColumn y;

	public DataGridViewTextBoxColumn z;

	public DataGridViewTextBoxColumn Mass;

	public event EventHandler containerChanged;

	public ProjectContainers()
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

	public List<STEB2.Containers.Container> GetContainers()
	{
		List<STEB2.Containers.Container> list = new List<STEB2.Containers.Container>();
		foreach (DataGridViewRow item in (IEnumerable)dataGridView.Rows)
		{
			try
			{
				list.Add(new STEB2.Containers.Container(item.Cells["Name"].Value.ToString(), STEB2.Converting.Converting.TextToFloat(item.Cells["l"].Value.ToString()), STEB2.Converting.Converting.TextToFloat(item.Cells["b"].Value.ToString()), STEB2.Converting.Converting.TextToFloat(item.Cells["h"].Value.ToString()), new p3d(STEB2.Converting.Converting.TextToFloat(item.Cells["x"].Value.ToString()), STEB2.Converting.Converting.TextToFloat(item.Cells["y"].Value.ToString()), STEB2.Converting.Converting.TextToFloat(item.Cells["z"].Value.ToString())), STEB2.Converting.Converting.TextToFloat(item.Cells["Mass"].Value.ToString())));
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
			using (JsonDbCommand oleDbCommand = new JsonDbCommand("SELECT * FROM tblContainers;", oleDbConnection))
			{
				using JsonDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
				NeedUpdating = false;
				while (oleDbDataReader.Read())
				{
					dataGridView.Rows.Add(oleDbDataReader["Name"].ToString(), oleDbDataReader["l"].ToString(), oleDbDataReader["b"].ToString(), oleDbDataReader["h"].ToString(), oleDbDataReader["x"].ToString(), oleDbDataReader["y"].ToString(), oleDbDataReader["z"].ToString(), oleDbDataReader["Mass"].ToString());
				}
				oleDbDataReader.Close();
			}
			oleDbConnection.Close();
		}
		NeedUpdating = true;
		if (containerChanged != null)
		{
			containerChanged(new object(), new EventArgs());
		}
	}

	private void SyncronizeDatabase()
	{
		if (NeedUpdating)
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile);
			oleDbConnection.Open();
			Tools.ExecuteSQL("DELETE * FROM tblContainers;", oleDbConnection);
			List<string> list = new List<string>();
			foreach (DataGridViewRow item in (IEnumerable)dataGridView.Rows)
			{
				try
				{
					list.Add("INSERT INTO tblContainers (Name,l,b,h,x,y,z,Mass) VALUES('" + item.Cells["Name"].Value.ToString() + "'," + item.Cells["l"].Value.ToString().Replace(',', '.') + "," + item.Cells["b"].Value.ToString().Replace(',', '.') + "," + item.Cells["h"].Value.ToString().Replace(',', '.') + "," + item.Cells["x"].Value.ToString().Replace(',', '.') + "," + item.Cells["y"].Value.ToString().Replace(',', '.') + "," + item.Cells["z"].Value.ToString().Replace(',', '.') + "," + item.Cells["Mass"].Value.ToString().Replace(',', '.') + ")");
				}
				catch
				{
				}
			}
			Tools.ExecuteRangeSQL(list, oleDbConnection);
			oleDbConnection.Close();
		}
		if (containerChanged != null)
		{
			containerChanged(new object(), new EventArgs());
		}
	}

	private void InitializeComponent()
	{
		this.dataGridView = new System.Windows.Forms.DataGridView();
		this.ContainerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.l = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.b = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.h = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.x = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.y = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.z = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Mass = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridView.Columns.AddRange(this.ContainerName, this.l, this.b, this.h, this.x, this.y, this.z, this.Mass);
		this.dataGridView.Name = "dataGridView";
		this.ContainerName.HeaderText = "Имя контейнера";
		this.ContainerName.Name = "Name";
		this.l.HeaderText = "l, м";
		this.l.Name = "l";
		this.l.ToolTipText = "Длина отсека, м";
		this.b.HeaderText = "b, м";
		this.b.Name = "b";
		this.b.ToolTipText = "Ширина отсека, м";
		this.h.HeaderText = "h, м";
		this.h.Name = "h";
		this.h.ToolTipText = "Высота отсека, м";
		this.x.HeaderText = "x, м";
		this.x.Name = "x";
		this.x.ToolTipText = "Отстояние по длине от начала координат, м";
		this.y.HeaderText = "y, м";
		this.y.Name = "y";
		this.y.ToolTipText = "Отстояние по ширине от начала координат, м";
		this.z.HeaderText = "z, м";
		this.z.Name = "z";
		this.z.ToolTipText = "Отстояние по высоте от начала координат, м";
		this.Mass.HeaderText = "Масса контейнера, т";
		this.Mass.Name = "Mass";
		this.Mass.ToolTipText = "Масса контейнера, т";
	}
}
