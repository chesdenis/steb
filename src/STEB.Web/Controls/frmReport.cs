using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using STEB2.Volume;

namespace STEB2;

public class frmReport : Form
{

	public ReportControl reportControl;

	private Waterlanding trueWaterLanding;

	private void InitializeComponent()
	{
		this.reportControl = new STEB2.ReportControl();
		this.reportControl.Name = "reportControl";
		Controls.Add(this.reportControl);
		Text = "Отчет о текущей посадке";
	}

	public frmReport(Waterlanding trueWaterLanding)
	{
		this.trueWaterLanding = trueWaterLanding;
		InitializeComponent();
		GetReport();
	}

	private void GetReport()
	{
		Cursor = Cursors.WaitCursor;
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<html>");
			stringBuilder.AppendLine("<body>");
			stringBuilder.AppendLine("<p>ОТЧЕТ о посадке судна");
			stringBuilder.AppendLine("<table border=1 width=100%>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Параметр");
			stringBuilder.AppendLine("<td>Значение");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Угол крена:");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.HeelAngle.ToString("0.0") + " градусов;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Угол дифферента");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.TrimAngle.ToString("0.0") + " градусов;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Осадка левого борта");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.d_lb.ToString("0.0") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Осадка правого борта");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.d_pb.ToString("0.0") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Осадка носом");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.d_n.ToString("0.0") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Осадка кормой");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.d_k.ToString("0.0") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Поперечная метацентрическая высота");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.mTransverseHeight.ToString("0.0") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Продольная метацентрическая высота");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.mLongHeight.ToString("0.0") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td colspan = 2>Характеристики ватерлинии");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Коэффициент полноты ватерлинии");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.alpha.ToString("0.000"));
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Длина по ватерлинии");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.Lwl.ToString("0.000") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Ширина по ватерлинии");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.Bwl.ToString("0.000") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Площадь ватерлинии");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.S.ToString("0.000") + " кв.м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Момент инерции, относительно оси X");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.Iy.ToString("0.000") + " м^3;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Момент инерции, относительно оси Y");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.Ix.ToString("0.000") + " м^3;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Абсцисса ЦТ ватерлинии");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.xf.ToString("0.000") + " м;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Ордината ЦТ ватерлинии");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.yf.ToString("0.000") + " м;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td colspan=2>Характеристики объема");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Весовое водоизмещение");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.DSPL.ToString("0.000") + " т;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Объемное водоизмещение");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.V.ToString("0.000") + " м^3;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Коэффициент полноты водоизмещения");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.delta.ToString("0.000"));
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Положение центра величины");
			stringBuilder.AppendLine("<td>Xc:" + trueWaterLanding.CPos.X.ToString("0.000") + " м.;<br>Yc:" + trueWaterLanding.CPos.Y.ToString("0.000") + " м.;<br>Zc:" + trueWaterLanding.CPos.Z.ToString("0.000") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Положение центра тяжести");
			stringBuilder.AppendLine("<td>Xg:" + trueWaterLanding.GPos.X.ToString("0.000") + " м.;<br>Yg:" + trueWaterLanding.GPos.Y.ToString("0.000") + " м.;<br>Zg:" + trueWaterLanding.GPos.Z.ToString("0.000") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Поперечный метацентрический радиус");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.r.ToString("0.000") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td>Продольный метацентрический радиус");
			stringBuilder.AppendLine("<td>" + trueWaterLanding.R.ToString("0.000") + " м.;");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<body>");
			stringBuilder.AppendLine("</html>");
			using (StreamWriter streamWriter = new StreamWriter(Application.StartupPath + "\\currentWtrl.html", append: false, Encoding.Default))
			{
				streamWriter.Write(stringBuilder.ToString());
				streamWriter.Close();
			}
			reportControl.Navigate(Application.StartupPath + "\\currentWtrl.html");
		}
		catch
		{
		}
		Cursor = Cursors.Default;
	}
}
