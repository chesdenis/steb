using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using STEB2;
using STEB2.Geometry;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB_LOAD;

public class frmHydroStaticReport : Form
{

	public usgNumberField usgNumberField_d;

	public usgNumberField usgNumberField_V;

	public usgNumberField usgNumberField_DSPL;

	public Label label1;

	public Label label2;

	public Label label3;

	public ReportControl reportControl;

	public Button buttonCalc;

	private List<Station> stations;

	private float aquaDensity;

	private void InitializeComponent()
	{
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.usgNumberField_DSPL = new STEB2.usgNumberField();
		this.usgNumberField_V = new STEB2.usgNumberField();
		this.usgNumberField_d = new STEB2.usgNumberField();
		this.reportControl = new STEB2.ReportControl();
		this.buttonCalc = new System.Windows.Forms.Button();
		this.label1.Name = "label1";
		this.label1.Text = "Осадка, м:";
		this.label2.Name = "label2";
		this.label2.Text = "Подводный объем, куб.м.:";
		this.label3.Name = "label3";
		this.label3.Text = "Водоизмещение, т:";
		this.usgNumberField_DSPL.Name = "usgNumberField_DSPL";
		this.usgNumberField_V.Name = "usgNumberField_V";
		this.usgNumberField_d.Name = "usgNumberField_d";
		this.reportControl.Name = "reportControl";
		this.buttonCalc.Name = "buttonCalc";
		this.buttonCalc.Text = "Показать таблицу значений";
		Controls.Add(this.buttonCalc);
		Controls.Add(this.reportControl);
		Controls.Add(this.label3);
		Controls.Add(this.label2);
		Controls.Add(this.label1);
		Controls.Add(this.usgNumberField_DSPL);
		Controls.Add(this.usgNumberField_V);
		Controls.Add(this.usgNumberField_d);
		Text = "Гидростатика - таблица значений";
	}

	public frmHydroStaticReport(List<Station> stations, float aquaDensity)
	{
		this.stations = stations;
		this.aquaDensity = aquaDensity;
		InitializeComponent();
		buttonCalc.Click += buttonCalc_Click;
		Show();
	}

	public void buttonCalc_Click(object sender, EventArgs e)
	{
		Cursor = Cursors.WaitCursor;
		try
		{
			float[] array = usgNumberField_d.ListOfValue();
			float[] array2 = usgNumberField_DSPL.ListOfValue();
			float[] array3 = usgNumberField_V.ListOfValue();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<html>");
			stringBuilder.AppendLine("<body>");
			if (array.Length > 0)
			{
				stringBuilder.AppendLine("<p>Плотность акватории: " + aquaDensity.ToString("0.000") + " т/куб.м.");
				stringBuilder.AppendLine("<p>Таблица - по осадкам");
				stringBuilder.AppendLine("<table border=1 width=100%>");
				stringBuilder.AppendLine("<tr>");
				stringBuilder.AppendLine("<td>Осадка, м");
				stringBuilder.AppendLine("<td>Подводный объем, куб.м");
				stringBuilder.AppendLine("<td>Водоизмещение, т");
				stringBuilder.AppendLine("<td>Коэфф-т полноты ватерлинии");
				stringBuilder.AppendLine("<td>Коэфф-т общей полноты");
				stringBuilder.AppendLine("<td>Длина по ватерлинии, м");
				stringBuilder.AppendLine("<td>Ширина по ватерлинии, м");
				stringBuilder.AppendLine("<td>Абсцисса ЦВ, м");
				stringBuilder.AppendLine("<td>Ордината ЦВ, м");
				stringBuilder.AppendLine("<td>Аппликата ЦВ, м");
				stringBuilder.AppendLine("<td>Абсцисса ЦТ, м");
				stringBuilder.AppendLine("<td>Ордината ЦТ, м");
				stringBuilder.AppendLine("<td>Аппликата ЦТ, м");
				stringBuilder.AppendLine("<td>Поперечный метрадиус, м");
				stringBuilder.AppendLine("<td>Продольный метрадиус, м");
				stringBuilder.AppendLine("</tr>");
				for (int i = 0; i < array.Length; i++)
				{
					Waterlanding_WaterlinePosition waterlanding_WaterlinePosition = new Waterlanding_WaterlinePosition(stations, 0f, 0f, new p3d(0f, 0f, array[i]), aquaDensity);
					waterlanding_WaterlinePosition.GetWLGabarite_DNDKDLBDPB();
					waterlanding_WaterlinePosition.CalcWaterlineGeometricCharacteristic();
					waterlanding_WaterlinePosition.CalcCoefficient();
					stringBuilder.AppendLine("<tr>");
					stringBuilder.AppendLine("<td>" + Math.Round(array[i], 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.V, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.DSPL, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.alpha, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.delta, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.Lwl, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.Bwl, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.CPos.X, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.CPos.Y, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.CPos.Z, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.GPos.X, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.GPos.Y, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.GPos.Z, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.r, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_WaterlinePosition.R, 3));
					stringBuilder.AppendLine("</tr>");
				}
				stringBuilder.AppendLine("</table>");
			}
			if (array3.Length > 0)
			{
				stringBuilder.AppendLine("<p>Таблица - по подводным объемам");
				stringBuilder.AppendLine("<table border=1 width=100%>");
				stringBuilder.AppendLine("<tr>");
				stringBuilder.AppendLine("<td>Осадка, м");
				stringBuilder.AppendLine("<td>Подводный объем, куб.м");
				stringBuilder.AppendLine("<td>Водоизмещение, т");
				stringBuilder.AppendLine("<td>Коэфф-т полноты ватерлинии");
				stringBuilder.AppendLine("<td>Коэфф-т общей полноты");
				stringBuilder.AppendLine("<td>Длина по ватерлинии, м");
				stringBuilder.AppendLine("<td>Ширина по ватерлинии, м");
				stringBuilder.AppendLine("<td>Абсцисса ЦВ, м");
				stringBuilder.AppendLine("<td>Ордината ЦВ, м");
				stringBuilder.AppendLine("<td>Аппликата ЦВ, м");
				stringBuilder.AppendLine("<td>Абсцисса ЦТ, м");
				stringBuilder.AppendLine("<td>Ордината ЦТ, м");
				stringBuilder.AppendLine("<td>Аппликата ЦТ, м");
				stringBuilder.AppendLine("<td>Поперечный метрадиус, м");
				stringBuilder.AppendLine("<td>Продольный метрадиус, м");
				stringBuilder.AppendLine("</tr>");
				for (int i = 0; i < array3.Length; i++)
				{
					Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(array3[i], 0f, 0f, stations, 0.001f, aquaDensity);
					waterlanding_Volume_Heel_Trim.GetWLGabarite_DNDKDLBDPB();
					waterlanding_Volume_Heel_Trim.CalcWaterlineGeometricCharacteristic();
					waterlanding_Volume_Heel_Trim.CalcCoefficient();
					stringBuilder.AppendLine("<tr>");
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.wlPosition.Z, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.V, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.DSPL, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.alpha, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.delta, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.Lwl, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.Bwl, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.CPos.X, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.CPos.Y, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.CPos.Z, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.GPos.X, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.GPos.Y, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.GPos.Z, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.r, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.R, 3));
					stringBuilder.AppendLine("</tr>");
				}
				stringBuilder.AppendLine("</table>");
			}
			if (array2.Length > 0)
			{
				stringBuilder.AppendLine("<p>Таблица  - по водоизмещениям");
				stringBuilder.AppendLine("<table border=1 width=100%>");
				stringBuilder.AppendLine("<tr>");
				stringBuilder.AppendLine("<td>Осадка, м");
				stringBuilder.AppendLine("<td>Подводный объем, куб.м");
				stringBuilder.AppendLine("<td>Водоизмещение, т");
				stringBuilder.AppendLine("<td>Коэфф-т полноты ватерлинии");
				stringBuilder.AppendLine("<td>Коэфф-т общей полноты");
				stringBuilder.AppendLine("<td>Длина по ватерлинии, м");
				stringBuilder.AppendLine("<td>Ширина по ватерлинии, м");
				stringBuilder.AppendLine("<td>Абсцисса ЦВ, м");
				stringBuilder.AppendLine("<td>Ордината ЦВ, м");
				stringBuilder.AppendLine("<td>Аппликата ЦВ, м");
				stringBuilder.AppendLine("<td>Абсцисса ЦТ, м");
				stringBuilder.AppendLine("<td>Ордината ЦТ, м");
				stringBuilder.AppendLine("<td>Аппликата ЦТ, м");
				stringBuilder.AppendLine("<td>Поперечный метрадиус, м");
				stringBuilder.AppendLine("<td>Продольный метрадиус, м");
				stringBuilder.AppendLine("</tr>");
				for (int i = 0; i < array2.Length; i++)
				{
					Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(array2[i] / aquaDensity, 0f, 0f, stations, 0.001f, aquaDensity);
					waterlanding_Volume_Heel_Trim.GetWLGabarite_DNDKDLBDPB();
					waterlanding_Volume_Heel_Trim.CalcWaterlineGeometricCharacteristic();
					waterlanding_Volume_Heel_Trim.CalcCoefficient();
					stringBuilder.AppendLine("<tr>");
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.wlPosition.Z, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.V, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.DSPL, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.alpha, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.delta, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.Lwl, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.Bwl, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.CPos.X, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.CPos.Y, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.CPos.Z, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.GPos.X, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.GPos.Y, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.GPos.Z, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.r, 3));
					stringBuilder.AppendLine("<td>" + Math.Round(waterlanding_Volume_Heel_Trim.R, 3));
					stringBuilder.AppendLine("</tr>");
				}
				stringBuilder.AppendLine("</table>");
			}
			stringBuilder.AppendLine("</body>");
			stringBuilder.AppendLine("</html>");
			using (StreamWriter streamWriter = new StreamWriter(Application.StartupPath + "\\gryz_razmer.html", append: false, Encoding.Default))
			{
				streamWriter.Write(stringBuilder.ToString());
				streamWriter.Close();
			}
			reportControl.Navigate(Application.StartupPath + "\\gryz_razmer.html");
		}
		catch
		{
		}
		Cursor = Cursors.Default;
	}
}
