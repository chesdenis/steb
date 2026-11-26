using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEB2.DataBase.Json;
using System.Drawing;
using System.Windows.Forms;
using STEB2.DataBase;
using STEB2.GraphicsElement;

namespace STEB2.Controls;

public class ProjectSectionDefinition : UserControl
{
	public enum SectionType
	{
		Long,
		Transv,
		Top
	}

	private bool NeedUpdating = true;

	private SectionType _typeSection;

	public TextBox textBox;

	[Browsable(true)]
	[Description("Тип поперечного сечения")]
	public SectionType TypeSection
	{
		get
		{
			return _typeSection;
		}
		set
		{
			_typeSection = value;
		}
	}

	public event EventHandler ProjectSectionChanged;

	public ProjectSectionDefinition(SectionType TypeSection)
	{
		InitializeComponent();
		textBox.TextChanged += textBox_TextChanged;
		this.TypeSection = TypeSection;
		using JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile);
		oleDbConnection.Open();
		switch (TypeSection)
		{
		case SectionType.Long:
			textBox.Text = GetProjectElements.GetSectionDescription(SectionType.Long, oleDbConnection);
			break;
		case SectionType.Transv:
			textBox.Text = GetProjectElements.GetSectionDescription(SectionType.Transv, oleDbConnection);
			break;
		}
		oleDbConnection.Close();
	}

	public void textBox_TextChanged(object sender, EventArgs e)
	{
		SyncronizeDatabase();
	}

	private void SyncronizeDatabase()
	{
		if (NeedUpdating)
		{
			using JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile);
			oleDbConnection.Open();
			List<string> list = new List<string>();
			string text = Tools.ReturnScalar("SELECT * FROM tblShipForms", "ShipFormName", oleDbConnection).ToString();
			try
			{
				switch (_typeSection)
				{
				case SectionType.Long:
					list.Add("UPDATE tblShipForms SET LongSections =  \"" + textBox.Text + "\" WHERE ShipFormName = '" + text + "'");
					break;
				case SectionType.Transv:
					list.Add("UPDATE tblShipForms SET TransverseSections =  \"" + textBox.Text + "\" WHERE ShipFormName = '" + text + "'");
					break;
				case SectionType.Top:
					list.Add("UPDATE tblShipForms SET TopSections =  \"" + textBox.Text + "\" WHERE ShipFormName = '" + text + "'");
					break;
				}
			}
			catch
			{
			}
			Tools.ExecuteRangeSQL(list, oleDbConnection);
			oleDbConnection.Close();
		}
		if (ProjectSectionChanged != null)
		{
			ProjectSectionChanged(new object(), new EventArgs());
		}
	}

	private void InitializeComponent()
	{
		this.textBox = new System.Windows.Forms.TextBox();
		this.textBox.Multiline = true;
		this.textBox.Name = "textBox";
	}
}
