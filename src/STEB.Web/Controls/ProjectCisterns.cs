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

namespace STEB2.Controls;

public class ProjectCisterns : UserControl
{

	public ToolStripButton toolStripButton_AddCistern;

	public ToolStripButton toolStripButton_DeleteCistern;

	public ToolStripButton toolStripButton_ModifyCistern;

	public ListView listView_Cisterns;

	public ColumnHeader Cistern_Name;

	public ColumnHeader Cistern_L;

	public ColumnHeader Cistern_B;

	public ColumnHeader Cistern_H;

	public ColumnHeader Cistern_percentOfVolume;

	public ColumnHeader Cistern_volumeDensity;

	public ColumnHeader Vmax;

	public ColumnHeader Mmax;

	public ColumnHeader Vcalc;

	public ColumnHeader Mcalc;

	public ColumnHeader waterLevel;

	private List<Cistern> Cisterns = new List<Cistern>();

	private void InitializeComponent()
	{
		this.toolStripButton_AddCistern = new System.Windows.Forms.ToolStripButton();
		this.toolStripButton_DeleteCistern = new System.Windows.Forms.ToolStripButton();
		this.toolStripButton_ModifyCistern = new System.Windows.Forms.ToolStripButton();
		this.listView_Cisterns = new System.Windows.Forms.ListView();
		this.Cistern_Name = new System.Windows.Forms.ColumnHeader();
		this.Cistern_L = new System.Windows.Forms.ColumnHeader();
		this.Cistern_B = new System.Windows.Forms.ColumnHeader();
		this.Cistern_H = new System.Windows.Forms.ColumnHeader();
		this.Cistern_percentOfVolume = new System.Windows.Forms.ColumnHeader();
		this.Cistern_volumeDensity = new System.Windows.Forms.ColumnHeader();
		this.Vmax = new System.Windows.Forms.ColumnHeader();
		this.Mmax = new System.Windows.Forms.ColumnHeader();
		this.Vcalc = new System.Windows.Forms.ColumnHeader();
		this.Mcalc = new System.Windows.Forms.ColumnHeader();
		this.waterLevel = new System.Windows.Forms.ColumnHeader();
		this.toolStripButton_AddCistern.Name = "toolStripButton_AddCistern";
		this.toolStripButton_AddCistern.Text = "Добавить новую цистерну";
		this.toolStripButton_DeleteCistern.Name = "toolStripButton_DeleteCistern";
		this.toolStripButton_DeleteCistern.Text = "Удалить цистерну";
		this.toolStripButton_ModifyCistern.Name = "toolStripButton_ModifyCistern";
		this.toolStripButton_ModifyCistern.Text = "Изменить параметры цистерны";
		this.listView_Cisterns.Columns.AddRange(new [] { 			this.Cistern_Name, this.Cistern_L, this.Cistern_B, this.Cistern_H, this.Cistern_percentOfVolume, this.Cistern_volumeDensity, this.Vmax, this.Mmax, this.Vcalc, this.Mcalc, 			this.waterLevel 		});
		this.listView_Cisterns.Name = "listView_Cisterns";
		this.Cistern_Name.Text = "Имя цистерны";
		this.Cistern_L.Text = "Длина, м";
		this.Cistern_B.Text = "Ширина, м";
		this.Cistern_H.Text = "Высота, м";
		this.Cistern_percentOfVolume.Text = "% заполн";
		this.Cistern_volumeDensity.Text = "Плотность жидкости";
		this.Vmax.Text = "Vmax, куб.м.";
		this.Mmax.Text = "Mmax, т";
		this.Vcalc.Text = "Vcalc, куб.м.";
		this.Mcalc.Text = "Mcalc, т";
		this.waterLevel.Text = "Уровень\nжидкости, м";
	}

	public ProjectCisterns()
	{
		InitializeComponent();
		toolStripButton_AddCistern.Click += toolStripButton_AddCistern_Click;
		toolStripButton_DeleteCistern.Click += toolStripButton_DeleteCistern_Click;
		toolStripButton_ModifyCistern.Click += toolStripButton_ModifyCistern_Click;
		LoadRows();
	}

	public List<Cistern> GetCisterns()
	{
		return Cisterns;
	}

	public void LoadRows()
	{
		try
		{
			Cisterns.Clear();
			using (JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile))
			{
				oleDbConnection.Open();
				using (JsonDbCommand oleDbCommand = new JsonDbCommand("SELECT * FROM tblCisterns;", oleDbConnection))
				{
					using JsonDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
					while (oleDbDataReader.Read())
					{
						Cistern cistern = new Cistern(oleDbDataReader["Name"].ToString().Trim(), oleDbDataReader["TextDecriptionOfStation"].ToString(), new p3d(STEB2.Converting.Converting.TextToFloat(oleDbDataReader["x0"].ToString()), STEB2.Converting.Converting.TextToFloat(oleDbDataReader["y0"].ToString()), STEB2.Converting.Converting.TextToFloat(oleDbDataReader["z0"].ToString())), new p3d(STEB2.Converting.Converting.TextToFloat(oleDbDataReader["x1"].ToString()), STEB2.Converting.Converting.TextToFloat(oleDbDataReader["y1"].ToString()), STEB2.Converting.Converting.TextToFloat(oleDbDataReader["z1"].ToString())), STEB2.Converting.Converting.TextToFloat(oleDbDataReader["volumeDensity"].ToString()));
						cistern.percentOfVolume = STEB2.Converting.Converting.TextToFloat(oleDbDataReader["percentOfVolume"].ToString());
						Cisterns.Add(cistern);
					}
					oleDbDataReader.Close();
				}
				oleDbConnection.Close();
			}
			FillCheckBoxList();
		}
		catch
		{
		}
	}

	private void FillCheckBoxList()
	{
		try
		{
			listView_Cisterns.Items.Clear();
			foreach (Cistern cistern in Cisterns)
			{
				ListViewItem listViewItem = new ListViewItem(cistern.Name.Trim());
				listViewItem.SubItems.Add(cistern.l.ToString());
				listViewItem.SubItems.Add(cistern.b.ToString());
				listViewItem.SubItems.Add(cistern.h.ToString());
				listViewItem.SubItems.Add(cistern.percentOfVolume.ToString());
				listViewItem.SubItems.Add(cistern.volumeDensity.ToString());
				listViewItem.SubItems.Add(cistern.maxVolume.ToString());
				listViewItem.SubItems.Add(cistern.maxMass.ToString());
				listViewItem.SubItems.Add(cistern.CalcVolume.ToString());
				listViewItem.SubItems.Add(cistern.CalcMass.ToString());
				listViewItem.SubItems.Add(cistern.waterLevel.ToString());
				listView_Cisterns.Items.Add(listViewItem);
			}
		}
		catch
		{
		}
	}

	private void SyncronizeDatabase()
	{
		try
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile);
			oleDbConnection.Open();
			Tools.ExecuteSQL("DELETE * FROM tblCisterns;", oleDbConnection);
			List<string> list = new List<string>();
			foreach (Cistern cistern in Cisterns)
			{
				list.Add("INSERT INTO tblCisterns (Name,x0,y0,z0,x1,y1,z1,percentOfVolume,volumeDensity,TextDecriptionOfStation) VALUES('" + cistern.Name + "'," + cistern.x0.ToString().Replace(',', '.') + "," + cistern.y0.ToString().Replace(',', '.') + "," + cistern.z0.ToString().Replace(',', '.') + "," + cistern.x1.ToString().Replace(',', '.') + "," + cistern.y1.ToString().Replace(',', '.') + "," + cistern.z1.ToString().Replace(',', '.') + "," + cistern.percentOfVolume.ToString().Replace(',', '.') + "," + cistern.volumeDensity.ToString().Replace(',', '.') + ",'" + cistern.TextDecriptionOfStation.ToString().Replace(',', '.') + "')");
			}
			Tools.ExecuteRangeSQL(list, oleDbConnection);
			oleDbConnection.Close();
		}
		catch
		{
		}
	}

	public void toolStripButton_ModifyCistern_Click(object sender, EventArgs e)
	{
		try
		{
			for (int i = 0; i < Cisterns.Count; i++)
			{
				if (!(Cisterns[i].Name == listView_Cisterns.SelectedItems[0].Text))
				{
					continue;
				}
				int index = i;
				CisternMainParameter cisternMainParameter = new CisternMainParameter(Cisterns[i], Cisterns);
				// Original: shows cisternMainParameter in a modal Form "Параметры цистерны ..." (ShowDialog), then:
				EditRequested?.Invoke("Параметры цистерны " + Cisterns[i].Name, cisternMainParameter, "Изменить цистерну?", delegate
				{
					try
					{
						if (MessageBox.Show("Изменить цистерну?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							Cisterns[index] = cisternMainParameter.activeCst;
						}
						FillCheckBoxList();
						SyncronizeDatabase();
					}
					catch
					{
					}
				});
				break;
			}
		}
		catch
		{
		}
	}

	/// <summary>Raised instead of form.ShowDialog(): (title, editor, closing question, code to run after closing).</summary>
	public event Action<string, CisternMainParameter, string, Action> EditRequested;

	public void toolStripButton_DeleteCistern_Click(object sender, EventArgs e)
	{
		try
		{
			if (MessageBox.Show("Удалить?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				foreach (Cistern cistern in Cisterns)
				{
					if (cistern.Name == listView_Cisterns.SelectedItems[0].Text)
					{
						Cisterns.Remove(cistern);
						break;
					}
				}
			}
			FillCheckBoxList();
			SyncronizeDatabase();
		}
		catch
		{
		}
	}

	public void toolStripButton_AddCistern_Click(object sender, EventArgs e)
	{
		try
		{
			CisternMainParameter cisternMainParameter = new CisternMainParameter(Cisterns);
			// Original: shows cisternMainParameter in a modal Form "Параметры новой цистерны" (ShowDialog), then:
			EditRequested?.Invoke("Параметры новой цистерны", cisternMainParameter, "Добавить новую цистерну?", delegate
			{
				try
				{
					if (MessageBox.Show("Добавить новую цистерну?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						Cisterns.Add(cisternMainParameter.activeCst);
					}
					FillCheckBoxList();
					SyncronizeDatabase();
				}
				catch
				{
				}
			});
		}
		catch
		{
		}
	}

}
