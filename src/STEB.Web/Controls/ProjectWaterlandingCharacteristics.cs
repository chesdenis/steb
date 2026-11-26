using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace STEB2.Controls;

public class ProjectWaterlandingCharacteristics : UserControl
{
	public enum tpDefWaterlanding
	{
		dn_dk_dlb_dpb,
		dn_dk_heel,
		dlb_dpb_trim,
		d_heel_trim,
		DSPL_heel_trim,
		Volume_heel_trim,
		delta,
		rSmall,
		Rbig,
		MetHTransv,
		MetHLong,
		Cpos,
		Gwl,
		Iwl,
		MassGpos,
		alpha
	}

	private tpDefWaterlanding _defaultDefWaterlanding = tpDefWaterlanding.MassGpos;

	public Label label3;

	public Label label2;

	public Label label1;

	public Label label7;

	public usgNumberField usgNumberField_delta;

	public Label label6;

	public Label label5;

	public Label label8;

	public usgNumberField usgNumberField_IxWl;

	public Label label9;

	public usgNumberField usgNumberField_YgWl;

	public Label label10;

	public usgNumberField usgNumberField_XgWl;

	public Label label14;

	public usgNumberField usgNumberField_IyWL;

	public Label label11;

	public usgNumberField usgNumberField_alpha;

	public Label label13;

	public usgNumberField usgNumberField_Rbig;

	public Label label12;

	public usgNumberField usgNumberField_rSmall;

	public Label label20;

	public usgNumberField usgNumberField_MetHTransv;

	public usgNumberField usgNumberField_Zc;

	public usgNumberField usgNumberField_Yc;

	public usgNumberField usgNumberField_Xc;

	public usgNumberField usgNumberField_V;

	public usgNumberField usgNumberField_DSPL;

	public Label label16;

	public usgNumberField usgNumberField_Zg;

	public Label label15;

	public Label label18;

	public Label label17;

	public usgNumberField usgNumberField_Mass;

	public usgNumberField usgNumberField_Xg;

	public usgNumberField usgNumberField_Yg;

	public Button button_Update;

	public Label label21;

	public usgNumberField usgNumberField_trimAngle;

	public usgNumberField usgNumberField_dn;

	public Label label28;

	public Label label27;

	public usgNumberField usgNumberField_dpb;

	public usgNumberField usgNumberField_dk;

	public Label label30;

	public usgNumberField usgNumberField_dlb;

	public Label label29;

	public Label label31;

	public usgNumberField usgNumberField_heelAngle;

	public RadioButton radioButton_MassGpos;

	public RadioButton radioButton_dn_dk_dlb_dpb;

	public RadioButton radioButton_alpha;

	public RadioButton radioButton_Iwl;

	public RadioButton radioButton_Gwl;

	public RadioButton radioButton_Cpos;

	public RadioButton radioButton_MetHTransv;

	public RadioButton radioButton_Rbig;

	public RadioButton radioButton_rSmall;

	public RadioButton radioButton_delta;

	public RadioButton radioButton_Volume_heel_trim;

	public RadioButton radioButton_DSPL_heel_trim;

	public RadioButton radioButton_d_heel_trim;

	public RadioButton radioButton_dlb_dpb_trim;

	public RadioButton radioButton_dn_dk_heel;

	public Label label33;

	public usgNumberField usgNumberField_MetHLong;

	public RadioButton radioButton_MetHLong;

	public tpDefWaterlanding defaultDefWaterlanding
	{
		get
		{
			return _defaultDefWaterlanding;
		}
		set
		{
			SetControlBackColor(new Control[25]
			{
				usgNumberField_Mass, usgNumberField_Xg, usgNumberField_Yg, usgNumberField_Zg, usgNumberField_alpha, usgNumberField_delta, usgNumberField_dk, usgNumberField_dlb, usgNumberField_dn, usgNumberField_dpb,
				usgNumberField_DSPL, usgNumberField_heelAngle, usgNumberField_IxWl, usgNumberField_IyWL, usgNumberField_MetHTransv, usgNumberField_MetHLong, usgNumberField_Rbig, usgNumberField_rSmall, usgNumberField_trimAngle, usgNumberField_V,
				usgNumberField_Xc, usgNumberField_XgWl, usgNumberField_Yc, usgNumberField_YgWl, usgNumberField_Zc
			}, Color.LightGray);
			switch (value)
			{
			case tpDefWaterlanding.dn_dk_dlb_dpb:
				SetControlBackColor(new Control[4] { usgNumberField_dk, usgNumberField_dlb, usgNumberField_dn, usgNumberField_dpb }, Color.White);
				break;
			case tpDefWaterlanding.dn_dk_heel:
				SetControlBackColor(new Control[3] { usgNumberField_dk, usgNumberField_dn, usgNumberField_heelAngle }, Color.White);
				break;
			case tpDefWaterlanding.dlb_dpb_trim:
				SetControlBackColor(new Control[3] { usgNumberField_dlb, usgNumberField_dpb, usgNumberField_trimAngle }, Color.White);
				break;
			case tpDefWaterlanding.d_heel_trim:
				SetControlBackColor(new Control[6] { usgNumberField_dlb, usgNumberField_dpb, usgNumberField_dk, usgNumberField_dn, usgNumberField_heelAngle, usgNumberField_trimAngle }, Color.White);
				break;
			case tpDefWaterlanding.DSPL_heel_trim:
				SetControlBackColor(new Control[3] { usgNumberField_DSPL, usgNumberField_heelAngle, usgNumberField_trimAngle }, Color.White);
				break;
			case tpDefWaterlanding.Volume_heel_trim:
				SetControlBackColor(new Control[3] { usgNumberField_V, usgNumberField_heelAngle, usgNumberField_trimAngle }, Color.White);
				break;
			case tpDefWaterlanding.delta:
				SetControlBackColor(new Control[1] { usgNumberField_delta }, Color.White);
				break;
			case tpDefWaterlanding.rSmall:
				SetControlBackColor(new Control[1] { usgNumberField_rSmall }, Color.White);
				break;
			case tpDefWaterlanding.Rbig:
				SetControlBackColor(new Control[1] { usgNumberField_Rbig }, Color.White);
				break;
			case tpDefWaterlanding.MetHTransv:
				SetControlBackColor(new Control[1] { usgNumberField_MetHTransv }, Color.White);
				break;
			case tpDefWaterlanding.MetHLong:
				SetControlBackColor(new Control[1] { usgNumberField_MetHLong }, Color.White);
				break;
			case tpDefWaterlanding.Cpos:
				SetControlBackColor(new Control[3] { usgNumberField_Xc, usgNumberField_Yc, usgNumberField_Zc }, Color.White);
				break;
			case tpDefWaterlanding.Gwl:
				SetControlBackColor(new Control[2] { usgNumberField_XgWl, usgNumberField_YgWl }, Color.White);
				break;
			case tpDefWaterlanding.Iwl:
				SetControlBackColor(new Control[2] { usgNumberField_IxWl, usgNumberField_IyWL }, Color.White);
				break;
			case tpDefWaterlanding.MassGpos:
				SetControlBackColor(new Control[4] { usgNumberField_Mass, usgNumberField_Xg, usgNumberField_Yg, usgNumberField_Zg }, Color.White);
				break;
			case tpDefWaterlanding.alpha:
				SetControlBackColor(new Control[1] { usgNumberField_alpha }, Color.White);
				break;
			}
			_defaultDefWaterlanding = value;
		}
	}

	public event EventHandler NeedUpdate_dn_dk_dlb_dpb;

	public event EventHandler NeedUpdate_dn_dk_heel;

	public event EventHandler NeedUpdate_d_lb_dpb_trim;

	public event EventHandler NeedUpdate_d_heel_trim;

	public event EventHandler NeedUpdate_DSPL_heel_trim;

	public event EventHandler NeedUpdate_Volume_heel_trim;

	public event EventHandler NeedUpdate_delta;

	public event EventHandler NeedUpdate_rSmall;

	public event EventHandler NeedUpdate_Rbig;

	public event EventHandler NeedUpdate_MetHTransv;

	public event EventHandler NeedUpdate_MetHLong;

	public event EventHandler NeedUpdate_Cpos;

	public event EventHandler NeedUpdate_Gwl;

	public event EventHandler NeedUpdate_Iwl;

	public event EventHandler NeedUpdate_MassGpos;

	public event EventHandler NeedUpdate_alpha;

	private void SetControlBackColor(Control[] ctrls, Color color)
	{
		foreach (Control control in ctrls)
		{
			control.BackColor = color;
		}
	}

	public ProjectWaterlandingCharacteristics()
	{
		InitializeComponent();
		usgNumberField_Mass.KeyDown += KeysDown;
		usgNumberField_Xg.KeyDown += KeysDown;
		usgNumberField_Yg.KeyDown += KeysDown;
		usgNumberField_Zg.KeyDown += KeysDown;
		usgNumberField_alpha.KeyDown += KeysDown;
		usgNumberField_delta.KeyDown += KeysDown;
		usgNumberField_dk.KeyDown += KeysDown;
		usgNumberField_dlb.KeyDown += KeysDown;
		usgNumberField_dn.KeyDown += KeysDown;
		usgNumberField_dpb.KeyDown += KeysDown;
		usgNumberField_DSPL.KeyDown += KeysDown;
		usgNumberField_heelAngle.KeyDown += KeysDown;
		usgNumberField_IxWl.KeyDown += KeysDown;
		usgNumberField_IyWL.KeyDown += KeysDown;
		usgNumberField_MetHTransv.KeyDown += KeysDown;
		usgNumberField_MetHLong.KeyDown += KeysDown;
		usgNumberField_Rbig.KeyDown += KeysDown;
		usgNumberField_rSmall.KeyDown += KeysDown;
		usgNumberField_trimAngle.KeyDown += KeysDown;
		usgNumberField_V.KeyDown += KeysDown;
		usgNumberField_Xc.KeyDown += KeysDown;
		usgNumberField_XgWl.KeyDown += KeysDown;
		usgNumberField_Yc.KeyDown += KeysDown;
		usgNumberField_YgWl.KeyDown += KeysDown;
		usgNumberField_Zc.KeyDown += KeysDown;
		button_Update.Click += button_Update_Click;
		radioButton_alpha.CheckedChanged += radioButton_alpha_CheckedChanged;
		radioButton_Cpos.CheckedChanged += radioButton_Cpos_CheckedChanged;
		radioButton_d_heel_trim.CheckedChanged += radioButton_d_heel_trim_CheckedChanged;
		radioButton_delta.CheckedChanged += radioButton_delta_CheckedChanged;
		radioButton_dlb_dpb_trim.CheckedChanged += radioButton_dlb_dpb_trim_CheckedChanged;
		radioButton_dn_dk_dlb_dpb.CheckedChanged += radioButton_dn_dk_dlb_dpb_CheckedChanged;
		radioButton_dn_dk_heel.CheckedChanged += radioButton_dn_dk_heel_CheckedChanged;
		radioButton_DSPL_heel_trim.CheckedChanged += radioButton_DSPL_heel_trim_CheckedChanged;
		radioButton_Gwl.CheckedChanged += radioButton_Gwl_CheckedChanged;
		radioButton_Iwl.CheckedChanged += radioButton_Iwl_CheckedChanged;
		radioButton_MassGpos.CheckedChanged += radioButton_MassGpos_CheckedChanged;
		radioButton_MetHTransv.CheckedChanged += radioButton_MetHTransv_CheckedChanged;
		radioButton_MetHLong.CheckedChanged += radioButton_MetHLong_CheckedChanged;
		radioButton_Rbig.CheckedChanged += radioButton_Rbig_CheckedChanged;
		radioButton_rSmall.CheckedChanged += radioButton_rSmall_CheckedChanged;
		radioButton_Volume_heel_trim.CheckedChanged += radioButton_Volume_heel_trim_CheckedChanged;
		defaultDefWaterlanding = tpDefWaterlanding.MassGpos;
	}

	public void radioButton_Volume_heel_trim_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.Volume_heel_trim;
	}

	public void radioButton_rSmall_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.rSmall;
	}

	public void radioButton_Rbig_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.Rbig;
	}

	public void radioButton_MetHLong_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.MetHLong;
	}

	public void radioButton_MetHTransv_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.MetHTransv;
	}

	public void radioButton_MassGpos_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.MassGpos;
	}

	public void radioButton_Iwl_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.Iwl;
	}

	public void radioButton_Gwl_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.Gwl;
	}

	public void radioButton_DSPL_heel_trim_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.DSPL_heel_trim;
	}

	public void radioButton_dn_dk_heel_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.dn_dk_heel;
	}

	public void radioButton_dn_dk_dlb_dpb_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.dn_dk_dlb_dpb;
	}

	public void radioButton_dlb_dpb_trim_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.dlb_dpb_trim;
	}

	public void radioButton_delta_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.delta;
	}

	public void radioButton_d_heel_trim_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.d_heel_trim;
	}

	public void radioButton_Cpos_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.Cpos;
	}

	public void radioButton_alpha_CheckedChanged(object sender, EventArgs e)
	{
		defaultDefWaterlanding = tpDefWaterlanding.alpha;
	}

	public void comboBox_AquatoryChanger_TextChanged(object sender, EventArgs e)
	{
		NeedRecalc();
	}

	public void button_Update_Click(object sender, EventArgs e)
	{
		NeedRecalc();
	}

	private void NeedRecalc()
	{
		switch (defaultDefWaterlanding)
		{
		case tpDefWaterlanding.dn_dk_dlb_dpb:
			if (NeedUpdate_dn_dk_dlb_dpb != null)
			{
				NeedUpdate_dn_dk_dlb_dpb(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.dn_dk_heel:
			if (NeedUpdate_dn_dk_heel != null)
			{
				NeedUpdate_dn_dk_heel(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.dlb_dpb_trim:
			if (NeedUpdate_d_lb_dpb_trim != null)
			{
				NeedUpdate_d_lb_dpb_trim(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.d_heel_trim:
			if (NeedUpdate_d_heel_trim != null)
			{
				NeedUpdate_d_heel_trim(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.DSPL_heel_trim:
			if (NeedUpdate_DSPL_heel_trim != null)
			{
				NeedUpdate_DSPL_heel_trim(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.Volume_heel_trim:
			if (NeedUpdate_Volume_heel_trim != null)
			{
				NeedUpdate_Volume_heel_trim(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.delta:
			if (NeedUpdate_delta != null)
			{
				NeedUpdate_delta(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.rSmall:
			if (NeedUpdate_rSmall != null)
			{
				NeedUpdate_rSmall(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.Rbig:
			if (NeedUpdate_Rbig != null)
			{
				NeedUpdate_Rbig(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.MetHTransv:
			if (NeedUpdate_MetHTransv != null)
			{
				NeedUpdate_MetHTransv(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.MetHLong:
			if (NeedUpdate_MetHLong != null)
			{
				NeedUpdate_MetHLong(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.Cpos:
			if (NeedUpdate_Cpos != null)
			{
				NeedUpdate_Cpos(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.Gwl:
			if (NeedUpdate_Gwl != null)
			{
				NeedUpdate_Gwl(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.Iwl:
			if (NeedUpdate_Iwl != null)
			{
				NeedUpdate_Iwl(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.MassGpos:
			if (NeedUpdate_MassGpos != null)
			{
				NeedUpdate_MassGpos(new object(), new EventArgs());
			}
			break;
		case tpDefWaterlanding.alpha:
			if (NeedUpdate_alpha != null)
			{
				NeedUpdate_alpha(new object(), new EventArgs());
			}
			break;
		}
	}

	public void KeysDown(object sender, KeyEventArgs e)
	{
		if ((e.KeyCode == Keys.Return) | (e.KeyCode == Keys.Return))
		{
			NeedRecalc();
		}
	}

	private void InitializeComponent()
	{
		this.button_Update = new System.Windows.Forms.Button();
		this.usgNumberField_Zg = new STEB2.usgNumberField();
		this.label16 = new System.Windows.Forms.Label();
		this.usgNumberField_Mass = new STEB2.usgNumberField();
		this.label18 = new System.Windows.Forms.Label();
		this.label15 = new System.Windows.Forms.Label();
		this.usgNumberField_Xg = new STEB2.usgNumberField();
		this.label17 = new System.Windows.Forms.Label();
		this.usgNumberField_Yg = new STEB2.usgNumberField();
		this.label11 = new System.Windows.Forms.Label();
		this.usgNumberField_alpha = new STEB2.usgNumberField();
		this.usgNumberField_XgWl = new STEB2.usgNumberField();
		this.label14 = new System.Windows.Forms.Label();
		this.label10 = new System.Windows.Forms.Label();
		this.usgNumberField_IyWL = new STEB2.usgNumberField();
		this.usgNumberField_YgWl = new STEB2.usgNumberField();
		this.label8 = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.usgNumberField_IxWl = new STEB2.usgNumberField();
		this.label5 = new System.Windows.Forms.Label();
		this.label33 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.usgNumberField_MetHLong = new STEB2.usgNumberField();
		this.label20 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label13 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.usgNumberField_Zc = new STEB2.usgNumberField();
		this.usgNumberField_Yc = new STEB2.usgNumberField();
		this.usgNumberField_MetHTransv = new STEB2.usgNumberField();
		this.usgNumberField_Rbig = new STEB2.usgNumberField();
		this.usgNumberField_Xc = new STEB2.usgNumberField();
		this.usgNumberField_V = new STEB2.usgNumberField();
		this.label12 = new System.Windows.Forms.Label();
		this.usgNumberField_delta = new STEB2.usgNumberField();
		this.usgNumberField_rSmall = new STEB2.usgNumberField();
		this.usgNumberField_DSPL = new STEB2.usgNumberField();
		this.label7 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.usgNumberField_heelAngle = new STEB2.usgNumberField();
		this.label31 = new System.Windows.Forms.Label();
		this.label21 = new System.Windows.Forms.Label();
		this.usgNumberField_trimAngle = new STEB2.usgNumberField();
		this.usgNumberField_dn = new STEB2.usgNumberField();
		this.label28 = new System.Windows.Forms.Label();
		this.label27 = new System.Windows.Forms.Label();
		this.usgNumberField_dpb = new STEB2.usgNumberField();
		this.usgNumberField_dk = new STEB2.usgNumberField();
		this.label30 = new System.Windows.Forms.Label();
		this.usgNumberField_dlb = new STEB2.usgNumberField();
		this.label29 = new System.Windows.Forms.Label();
		this.radioButton_MetHTransv = new System.Windows.Forms.RadioButton();
		this.radioButton_MetHLong = new System.Windows.Forms.RadioButton();
		this.radioButton_alpha = new System.Windows.Forms.RadioButton();
		this.radioButton_Iwl = new System.Windows.Forms.RadioButton();
		this.radioButton_Gwl = new System.Windows.Forms.RadioButton();
		this.radioButton_Cpos = new System.Windows.Forms.RadioButton();
		this.radioButton_Rbig = new System.Windows.Forms.RadioButton();
		this.radioButton_rSmall = new System.Windows.Forms.RadioButton();
		this.radioButton_delta = new System.Windows.Forms.RadioButton();
		this.radioButton_Volume_heel_trim = new System.Windows.Forms.RadioButton();
		this.radioButton_DSPL_heel_trim = new System.Windows.Forms.RadioButton();
		this.radioButton_d_heel_trim = new System.Windows.Forms.RadioButton();
		this.radioButton_dlb_dpb_trim = new System.Windows.Forms.RadioButton();
		this.radioButton_dn_dk_heel = new System.Windows.Forms.RadioButton();
		this.radioButton_dn_dk_dlb_dpb = new System.Windows.Forms.RadioButton();
		this.radioButton_MassGpos = new System.Windows.Forms.RadioButton();
		this.button_Update.Name = "button_Update";
		this.button_Update.Text = "Определить(Enter)";
		Controls.Add(this.usgNumberField_Zg);
		Controls.Add(this.label16);
		Controls.Add(this.usgNumberField_Mass);
		Controls.Add(this.label18);
		Controls.Add(this.label15);
		Controls.Add(this.usgNumberField_Xg);
		Controls.Add(this.label17);
		Controls.Add(this.usgNumberField_Yg);
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
		Controls.Add(this.label11);
		Controls.Add(this.usgNumberField_alpha);
		Controls.Add(this.usgNumberField_XgWl);
		Controls.Add(this.label14);
		Controls.Add(this.label10);
		Controls.Add(this.usgNumberField_IyWL);
		Controls.Add(this.usgNumberField_YgWl);
		Controls.Add(this.label8);
		Controls.Add(this.label9);
		Controls.Add(this.usgNumberField_IxWl);
		this.label11.Name = "label11";
		this.label11.Text = "Коэффициент полноты \r\nватерлинии";
		this.usgNumberField_alpha.Name = "usgNumberField_alpha";
		this.usgNumberField_XgWl.Name = "usgNumberField_XgWl";
		this.label14.Name = "label14";
		this.label14.Text = "Осевой момент инерции \r\nватерлинии относительно \r\nоси ординат, м^4";
		this.label10.Name = "label10";
		this.label10.Text = "Абсцисса ЦТ, м";
		this.usgNumberField_IyWL.Name = "usgNumberField_IyWL";
		this.usgNumberField_YgWl.Name = "usgNumberField_YgWl";
		this.label8.Name = "label8";
		this.label8.Text = "Осевой момент инерции \r\nотносительно \r\nоси абсцисс, м^4";
		this.label9.Name = "label9";
		this.label9.Text = "Ордината ЦТ, м";
		this.usgNumberField_IxWl.Name = "usgNumberField_IxWl";
		Controls.Add(this.label5);
		Controls.Add(this.label33);
		Controls.Add(this.label3);
		Controls.Add(this.usgNumberField_MetHLong);
		Controls.Add(this.label20);
		Controls.Add(this.label2);
		Controls.Add(this.label13);
		Controls.Add(this.label1);
		Controls.Add(this.usgNumberField_Zc);
		Controls.Add(this.usgNumberField_Yc);
		Controls.Add(this.usgNumberField_MetHTransv);
		Controls.Add(this.usgNumberField_Rbig);
		Controls.Add(this.usgNumberField_Xc);
		Controls.Add(this.usgNumberField_V);
		Controls.Add(this.label12);
		Controls.Add(this.usgNumberField_delta);
		Controls.Add(this.usgNumberField_rSmall);
		Controls.Add(this.usgNumberField_DSPL);
		Controls.Add(this.label7);
		Controls.Add(this.label6);
		this.label5.Name = "label5";
		this.label5.Text = "V, куб.м.";
		this.label33.Name = "label33";
		this.label33.Text = "Поперечная \r\nметацентрическая высота, м";
		this.label3.Name = "label3";
		this.label3.Text = "Zc, м";
		this.usgNumberField_MetHLong.Name = "usgNumberField_MetHLong";
		this.label20.Name = "label20";
		this.label20.Text = "Продольная \r\nметацентрическая высота, м";
		this.label2.Name = "label2";
		this.label2.Text = "Yc, м";
		this.label13.Name = "label13";
		this.label13.Text = "Продольный \r\nметацентрический \r\nрадиус, м";
		this.label1.Name = "label1";
		this.label1.Text = "Xc, м";
		this.usgNumberField_Zc.Name = "usgNumberField_Zc";
		this.usgNumberField_Yc.Name = "usgNumberField_Yc";
		this.usgNumberField_MetHTransv.Name = "usgNumberField_MetHTransv";
		this.usgNumberField_Rbig.Name = "usgNumberField_Rbig";
		this.usgNumberField_Xc.Name = "usgNumberField_Xc";
		this.usgNumberField_V.Name = "usgNumberField_V";
		this.label12.Name = "label12";
		this.label12.Text = "Поперечный\r\nметацентрический \r\nрадиус, м";
		this.usgNumberField_delta.Name = "usgNumberField_delta";
		this.usgNumberField_rSmall.Name = "usgNumberField_rSmall";
		this.usgNumberField_DSPL.Name = "usgNumberField_DSPL";
		this.label7.Name = "label7";
		this.label7.Text = "Коэффициент \r\nобщей полноты";
		this.label6.Name = "label6";
		this.label6.Text = "DSPL, т.";
		Controls.Add(this.usgNumberField_heelAngle);
		Controls.Add(this.label31);
		Controls.Add(this.label21);
		Controls.Add(this.usgNumberField_trimAngle);
		Controls.Add(this.usgNumberField_dn);
		Controls.Add(this.label28);
		Controls.Add(this.label27);
		Controls.Add(this.usgNumberField_dpb);
		Controls.Add(this.usgNumberField_dk);
		Controls.Add(this.label30);
		Controls.Add(this.usgNumberField_dlb);
		Controls.Add(this.label29);
		this.usgNumberField_heelAngle.Name = "usgNumberField_heelAngle";
		this.label31.Name = "label31";
		this.label31.Text = "Угол \r\nкрена, градусы:";
		this.label21.Name = "label21";
		this.label21.Text = "Угол \r\nдифферента, градусы:";
		this.usgNumberField_trimAngle.Name = "usgNumberField_trimAngle";
		this.usgNumberField_dn.Name = "usgNumberField_dn";
		this.label28.Name = "label28";
		this.label28.Text = "Осадка носом, м";
		this.label27.Name = "label27";
		this.label27.Text = "Осадка \r\nправого борта, м";
		this.usgNumberField_dpb.Name = "usgNumberField_dpb";
		this.usgNumberField_dk.Name = "usgNumberField_dk";
		this.label30.Name = "label30";
		this.label30.Text = "Осадка кормой, м";
		this.usgNumberField_dlb.Name = "usgNumberField_dlb";
		this.label29.Name = "label29";
		this.label29.Text = "Осадка \r\nлевого борта, м";
		Controls.Add(this.radioButton_MetHTransv);
		Controls.Add(this.radioButton_MetHLong);
		Controls.Add(this.radioButton_alpha);
		Controls.Add(this.radioButton_Iwl);
		Controls.Add(this.radioButton_Gwl);
		Controls.Add(this.radioButton_Cpos);
		Controls.Add(this.radioButton_Rbig);
		Controls.Add(this.radioButton_rSmall);
		Controls.Add(this.radioButton_delta);
		Controls.Add(this.radioButton_Volume_heel_trim);
		Controls.Add(this.radioButton_DSPL_heel_trim);
		Controls.Add(this.radioButton_d_heel_trim);
		Controls.Add(this.radioButton_dlb_dpb_trim);
		Controls.Add(this.radioButton_dn_dk_heel);
		Controls.Add(this.radioButton_dn_dk_dlb_dpb);
		Controls.Add(this.radioButton_MassGpos);
		this.radioButton_MetHTransv.Name = "radioButton_MetHTransv";
		this.radioButton_MetHTransv.Text = "Поперечной метацентрической высоте";
		this.radioButton_MetHLong.Name = "radioButton_MetHLong";
		this.radioButton_MetHLong.Text = "Продольной метацентрической высоте";
		this.radioButton_alpha.Name = "radioButton_alpha";
		this.radioButton_alpha.Text = "Коэффициенту полноты ватерлинии";
		this.radioButton_Iwl.Name = "radioButton_Iwl";
		this.radioButton_Iwl.Text = "Моментам инерции ватерлинии";
		this.radioButton_Gwl.Name = "radioButton_Gwl";
		this.radioButton_Gwl.Text = "Координатам ЦТ ватерлинии";
		this.radioButton_Cpos.Name = "radioButton_Cpos";
		this.radioButton_Cpos.Text = "Координатам центра величины";
		this.radioButton_Rbig.Name = "radioButton_Rbig";
		this.radioButton_Rbig.Text = "Продольному метацентрическому радиусу";
		this.radioButton_rSmall.Name = "radioButton_rSmall";
		this.radioButton_rSmall.Text = "Поперечному метацентрическому радиусу";
		this.radioButton_delta.Name = "radioButton_delta";
		this.radioButton_delta.Text = "Коэффициенту общей полноты";
		this.radioButton_Volume_heel_trim.Name = "radioButton_Volume_heel_trim";
		this.radioButton_Volume_heel_trim.Text = "Объемному водоизмещению, углу крена, углу дифферента";
		this.radioButton_DSPL_heel_trim.Name = "radioButton_DSPL_heel_trim";
		this.radioButton_DSPL_heel_trim.Text = "Весовому водоизмещению, углу крена, углу дифферента";
		this.radioButton_d_heel_trim.Name = "radioButton_d_heel_trim";
		this.radioButton_d_heel_trim.Text = "Осадке на миделе, углу крена и углу дифферента";
		this.radioButton_dlb_dpb_trim.Name = "radioButton_dlb_dpb_trim";
		this.radioButton_dlb_dpb_trim.Text = "Осадке левого, правого борта и угла дифферента";
		this.radioButton_dn_dk_heel.Name = "radioButton_dn_dk_heel";
		this.radioButton_dn_dk_heel.Text = "Осадке носом, кормой и углом крена";
		this.radioButton_dn_dk_dlb_dpb.Name = "radioButton_dn_dk_dlb_dpb";
		this.radioButton_dn_dk_dlb_dpb.Text = "Осадке носом, кормой, левого и правого борта";
		this.radioButton_MassGpos.Checked = true;
		this.radioButton_MassGpos.Name = "radioButton_MassGpos";
		this.radioButton_MassGpos.Text = "Весу судна и координатам ЦТ";
		Controls.Add(this.button_Update);
	}
}
