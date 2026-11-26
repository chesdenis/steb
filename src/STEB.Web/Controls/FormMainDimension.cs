using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace STEB2.Controls;

public class FormMainDimension : UserControl
{

	public Label label1;

	public Label label2;

	public Label label3;

	public Label label4;

	public Label label6;

	public Label label7;

	public Label label8;

	public usgNumberField usgNumberField_L;

	public usgNumberField usgNumberField_d;

	public usgNumberField usgNumberField_H;

	public usgNumberField usgNumberField_B;

	public usgNumberField usgNumberField_kZ;

	public usgNumberField usgNumberField_kY;

	public usgNumberField usgNumberField_kX;

	public event EventHandler UpdateTexts;

	public FormMainDimension()
	{
		InitializeComponent();
		usgNumberField_kX.TextChanged += usgNumberField_kX_TextChanged;
		usgNumberField_kZ.TextChanged += usgNumberField_kZ_TextChanged;
		usgNumberField_kY.TextChanged += usgNumberField_kY_TextChanged;
		usgNumberField_L.TextChanged += usgNumberField_L_TextChanged;
		usgNumberField_B.TextChanged += usgNumberField_B_TextChanged;
		usgNumberField_H.TextChanged += usgNumberField_H_TextChanged;
		usgNumberField_d.TextChanged += usgNumberField_d_TextChanged;
	}

	public void usgNumberField_d_TextChanged(object sender, EventArgs e)
	{
		if (UpdateTexts != null)
		{
			UpdateTexts(new object(), new EventArgs());
		}
	}

	public void usgNumberField_H_TextChanged(object sender, EventArgs e)
	{
		if (UpdateTexts != null)
		{
			UpdateTexts(new object(), new EventArgs());
		}
	}

	public void usgNumberField_B_TextChanged(object sender, EventArgs e)
	{
		if (UpdateTexts != null)
		{
			UpdateTexts(new object(), new EventArgs());
		}
	}

	public void usgNumberField_L_TextChanged(object sender, EventArgs e)
	{
		if (UpdateTexts != null)
		{
			UpdateTexts(new object(), new EventArgs());
		}
	}

	public void usgNumberField_kY_TextChanged(object sender, EventArgs e)
	{
		if (UpdateTexts != null)
		{
			UpdateTexts(new object(), new EventArgs());
		}
	}

	public void usgNumberField_kZ_TextChanged(object sender, EventArgs e)
	{
		if (UpdateTexts != null)
		{
			UpdateTexts(new object(), new EventArgs());
		}
	}

	public void usgNumberField_kX_TextChanged(object sender, EventArgs e)
	{
		if (UpdateTexts != null)
		{
			UpdateTexts(new object(), new EventArgs());
		}
	}

	private void InitializeComponent()
	{
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.usgNumberField_d = new STEB2.usgNumberField();
		this.usgNumberField_H = new STEB2.usgNumberField();
		this.usgNumberField_B = new STEB2.usgNumberField();
		this.usgNumberField_L = new STEB2.usgNumberField();
		this.usgNumberField_kZ = new STEB2.usgNumberField();
		this.label6 = new System.Windows.Forms.Label();
		this.usgNumberField_kY = new STEB2.usgNumberField();
		this.label7 = new System.Windows.Forms.Label();
		this.usgNumberField_kX = new STEB2.usgNumberField();
		this.label8 = new System.Windows.Forms.Label();
		this.label1.Name = "label1";
		this.label1.Text = "Длина формы корпуса, м:";
		this.label2.Name = "label2";
		this.label2.Text = "Ширина формы корпуса, м:";
		this.label3.Name = "label3";
		this.label3.Text = "Высота формы корпуса, м:";
		this.label4.Name = "label4";
		this.label4.Text = "Проектная осадка корпуса, м:";
		this.usgNumberField_d.Name = "usgNumberField_d";
		this.usgNumberField_H.Name = "usgNumberField_H";
		this.usgNumberField_B.Name = "usgNumberField_B";
		this.usgNumberField_L.Name = "usgNumberField_L";
		this.usgNumberField_kZ.Name = "usgNumberField_kZ";
		this.label6.Name = "label6";
		this.label6.Text = "kZ:";
		this.usgNumberField_kY.Name = "usgNumberField_kY";
		this.label7.Name = "label7";
		this.label7.Text = "kY:";
		this.usgNumberField_kX.Name = "usgNumberField_kX";
		this.label8.Name = "label8";
		this.label8.Text = "kX:";
	}
}
