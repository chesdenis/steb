using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace STEB_LOAD;

public class AboutBox : Form
{

	public Label labelProductName;

	public Label labelVersion;

	public Label labelCopyright;

	public Label labelCompanyName;

	public TextBox textBoxDescription;

	public Button okButton;

	public string AssemblyTitle
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), inherit: false);
			if (customAttributes.Length > 0)
			{
				AssemblyTitleAttribute assemblyTitleAttribute = (AssemblyTitleAttribute)customAttributes[0];
				if (assemblyTitleAttribute.Title != "")
				{
					return assemblyTitleAttribute.Title;
				}
			}
			return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
		}
	}

	public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

	public string AssemblyDescription
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyDescriptionAttribute)customAttributes[0]).Description;
		}
	}

	public string AssemblyProduct
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyProductAttribute)customAttributes[0]).Product;
		}
	}

	public string AssemblyCopyright
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
		}
	}

	public string AssemblyCompany
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyCompanyAttribute)customAttributes[0]).Company;
		}
	}

	private void InitializeComponent()
	{
		this.labelProductName = new System.Windows.Forms.Label();
		this.labelVersion = new System.Windows.Forms.Label();
		this.labelCopyright = new System.Windows.Forms.Label();
		this.labelCompanyName = new System.Windows.Forms.Label();
		this.textBoxDescription = new System.Windows.Forms.TextBox();
		this.okButton = new System.Windows.Forms.Button();
		Controls.Add(this.labelProductName);
		Controls.Add(this.labelVersion);
		Controls.Add(this.labelCopyright);
		Controls.Add(this.labelCompanyName);
		Controls.Add(this.textBoxDescription);
		Controls.Add(this.okButton);
		this.labelProductName.Name = "labelProductName";
		this.labelProductName.Text = "Название продукта";
		this.labelVersion.Name = "labelVersion";
		this.labelVersion.Text = "Версия";
		this.labelCopyright.Name = "labelCopyright";
		this.labelCopyright.Text = "Авторские права";
		this.labelCompanyName.Name = "labelCompanyName";
		this.labelCompanyName.Text = "Название организации";
		this.textBoxDescription.Multiline = true;
		this.textBoxDescription.Name = "textBoxDescription";
		this.textBoxDescription.ReadOnly = true;
		this.textBoxDescription.Text = "Описание";
		this.okButton.Name = "okButton";
		this.okButton.Text = "&ОК";
		this.okButton.Click += new System.EventHandler(this.okButton_Click);
		Text = "О программе:";
	}

	public AboutBox()
	{
		InitializeComponent();
		Text = $"О {AssemblyTitle}";
		labelProductName.Text = AssemblyProduct;
		labelVersion.Text = $"Версия {AssemblyVersion} ";
		labelCopyright.Text = AssemblyCopyright;
		labelCompanyName.Text = AssemblyCompany;
		textBoxDescription.Lines = AssemblyDescription.Split(';');
	}

	public void okButton_Click(object sender, EventArgs e)
	{
		Close();
	}
}
