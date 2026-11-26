using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace STEB2;

public class ReportControl : UserControl
{
	private string PathTofReport = "";

	public ToolStrip toolStrip;

	public ToolStripButton toolStripButtonPrint;

	public ToolStripLabel toolStripLabel1;

	public ToolStripTextBox toolStripTextBoxPathToHTML;

	public WebBrowser webBrowser;

	public ToolStripButton toolStripButton_ContextMenu;

	public ReportControl()
	{
		InitializeComponent();
		toolStripButtonPrint.Click += toolStripButtonPrint_Click;
	}

	public void toolStripButtonPrint_Click(object sender, EventArgs e)
	{
		try
		{
			webBrowser.Print();
		}
		catch
		{
		}
	}

	public void Navigate(string strPath)
	{
		PathTofReport = strPath;
		try
		{
			toolStripTextBoxPathToHTML.Text = strPath;
			webBrowser.Navigate(toolStripTextBoxPathToHTML.Text, newWindow: false);
			webBrowser.Refresh();
		}
		catch
		{
			Exception ex = new Exception();
			MessageBox.Show(ex.Message, ex.Source);
		}
	}

	public void toolStripButton_ContextMenu_Click(object sender, EventArgs e)
	{
		// Original: Windows Explorer context menu for the report file (ShellContextMenu).
		// Not available in a browser; the report view offers download and print instead.
	}

	private void InitializeComponent()
	{
		this.toolStrip = new System.Windows.Forms.ToolStrip();
		this.toolStripButtonPrint = new System.Windows.Forms.ToolStripButton();
		this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
		this.toolStripTextBoxPathToHTML = new System.Windows.Forms.ToolStripTextBox();
		this.toolStripButton_ContextMenu = new System.Windows.Forms.ToolStripButton();
		this.webBrowser = new System.Windows.Forms.WebBrowser();
		this.toolStrip.Name = "toolStrip";
		this.toolStrip.Text = "toolStrip1";
		this.toolStripButtonPrint.Name = "toolStripButtonPrint";
		this.toolStripButtonPrint.Text = "Печать";
		this.toolStripLabel1.Name = "toolStripLabel1";
		this.toolStripLabel1.Text = "Адрес:";
		this.toolStripTextBoxPathToHTML.Name = "toolStripTextBoxPathToHTML";
		this.toolStripButton_ContextMenu.Name = "toolStripButton_ContextMenu";
		this.toolStripButton_ContextMenu.Text = "Действия с отчетом...";
		this.toolStripButton_ContextMenu.Click += new System.EventHandler(this.toolStripButton_ContextMenu_Click);
		this.webBrowser.Name = "webBrowser";
		Controls.Add(this.webBrowser);
		Controls.Add(this.toolStrip);
	}
}
