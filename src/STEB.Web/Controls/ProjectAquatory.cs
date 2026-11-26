using System;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Drawing;
using System.Windows.Forms;
using STEB2.Converting;
using STEB2.DataBase;

namespace STEB2.Controls;

public class ProjectAquatory : UserControl
{

	public Label label1;

	public Button button_Add;

	public Button button_Delete;

	public ListView listView_Aquatories;

	public ColumnHeader columnHeader_AquaName;

	public ColumnHeader columnHeader_AquaDensity;

	public ProjectAquatory()
	{
		InitializeComponent();
		button_Add.Click += button_Add_Click;
		button_Delete.Click += button_Delete_Click;
		FillAquatoriesList();
	}

	public void button_Delete_Click(object sender, EventArgs e)
	{
		using (JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile))
		{
			oleDbConnection.Open();
			foreach (ListViewItem selectedItem in listView_Aquatories.SelectedItems)
			{
				Tools.ExecuteSQL("DELETE * FROM tblAquatories WHERE ID=" + selectedItem.Name, oleDbConnection);
			}
			oleDbConnection.Close();
		}
		FillAquatoriesList();
	}

	public void button_Add_Click(object sender, EventArgs e)
	{
		// Original: builds a modal Form "Введите название и плотность акватории" with a TextBox
		// (name) and a usgNumberField (density), calls ShowDialog(), then runs button_Add_DialogClosed.
		TextBox textBox = new TextBox();
		usgNumberField usgNumberField2 = new usgNumberField();
		Label label = new Label();
		label.Text = "Название акватории";
		AddRequested?.Invoke(textBox, usgNumberField2, label);
	}

	/// <summary>Raised instead of form.ShowDialog(): the view shows the two inputs, then calls button_Add_DialogClosed.</summary>
	public event Action<TextBox, usgNumberField, Label> AddRequested;

	/// <summary>The code that followed form.ShowDialog() in the original button_Add_Click.</summary>
	public void button_Add_DialogClosed(TextBox textBox, usgNumberField usgNumberField2, Label label)
	{
		using (JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile))
		{
			oleDbConnection.Open();
			if (label.Text.Length > 0 && label.Text.Length < 50)
			{
				Tools.ExecuteSQL("INSERT INTO tblAquatories (Name, AquaDensity) VALUES ('" + textBox.Text + "', " + STEB2.Converting.Converting.TextToFloat(usgNumberField2.Text).ToString().Replace(',', '.') + ")", oleDbConnection);
			}
			oleDbConnection.Close();
		}
		FillAquatoriesList();
	}

	public void FillAquatoriesList()
	{
		try
		{
			listView_Aquatories.Items.Clear();
			using JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile);
			oleDbConnection.Open();
			string[] array = Tools.ReturnScalarArray("SELECT * FROM tblAquatories ORDER BY ID;", "Name", oleDbConnection);
			string[] array2 = Tools.ReturnScalarArray("SELECT * FROM tblAquatories ORDER BY ID;", "AquaDensity", oleDbConnection);
			string[] array3 = Tools.ReturnScalarArray("SELECT * FROM tblAquatories ORDER BY ID;", "ID", oleDbConnection);
			oleDbConnection.Close();
			for (int i = 0; i < array.Length; i++)
			{
				ListViewItem listViewItem = new ListViewItem();
				listViewItem.Name = array3[i];
				listViewItem.Text = array[i];
				listViewItem.SubItems.Add(array2[i]);
				listView_Aquatories.Items.Add(listViewItem);
			}
		}
		catch
		{
		}
	}

	private void InitializeComponent()
	{
		this.label1 = new System.Windows.Forms.Label();
		this.button_Add = new System.Windows.Forms.Button();
		this.button_Delete = new System.Windows.Forms.Button();
		this.listView_Aquatories = new System.Windows.Forms.ListView();
		this.columnHeader_AquaName = new System.Windows.Forms.ColumnHeader();
		this.columnHeader_AquaDensity = new System.Windows.Forms.ColumnHeader();
		this.label1.Name = "label1";
		this.label1.Text = "Список акваторий:";
		this.button_Add.Name = "button_Add";
		this.button_Add.Text = "Добавить акваторию";
		this.button_Delete.Name = "button_Delete";
		this.button_Delete.Text = "Удалить акваторию";
		this.listView_Aquatories.Columns.AddRange(new [] { this.columnHeader_AquaName, this.columnHeader_AquaDensity });
		this.listView_Aquatories.Name = "listView_Aquatories";
		this.columnHeader_AquaName.Text = "Название акватории";
		this.columnHeader_AquaDensity.Text = "Плотность акватории";
	}
}
