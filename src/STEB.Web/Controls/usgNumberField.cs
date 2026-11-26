using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace STEB2;

public class usgNumberField : TextBox
{

	private float TextToFloat(string strText)
	{
		if (strText.IndexOf('E') > 0)
		{
			return 0f;
		}
		strText = strText.Trim();
		if (strText.Trim() == "null")
		{
			return float.NaN;
		}
		try
		{
			if (strText == "")
			{
				return 0f;
			}
			float num = 0f;
			float num2 = 1f;
			long num3 = 0L;
			for (int num4 = strText.Length - 1; num4 > -1; num4--)
			{
				switch (strText.Substring(num4, 1))
				{
				case ".":
				case ",":
					num /= num2;
					num2 = 0.1f;
					break;
				case "-":
					num *= -1f;
					break;
				default:
					num3 = Convert.ToInt64(strText.Substring(num4, 1));
					num += (float)num3 * num2;
					break;
				case " ":
					break;
				}
				num2 *= 10f;
			}
			return num;
		}
		catch
		{
			return 0f;
		}
	}

	public float Value()
	{
		return TextToFloat(Text);
	}

	public float[] ListOfValue()
	{
		if (Text.Trim().Length == 0)
		{
			return new float[0];
		}
		string text = Text;
		if (text.IndexOf("...") > 0)
		{
			string[] array = text.Split(new string[1] { "..." }, StringSplitOptions.RemoveEmptyEntries);
			float num = TextToFloat(array[0].Split(',')[0]);
			float num2 = TextToFloat(array[0].Split(',')[1]);
			float num3 = num2 - num;
			float num4 = TextToFloat(array[1]);
			List<float> list = new List<float>();
			if (num3 != 0f)
			{
				for (float num5 = num; num5 < num4; num5 += num3)
				{
					list.Add(num5);
				}
			}
			return list.ToArray();
		}
		string[] array2 = text.Split(',');
		List<float> list2 = new List<float>();
		for (int i = 0; i < array2.Length; i++)
		{
			list2.Add(TextToFloat(array2[i]));
		}
		return list2.ToArray();
	}

	public string SQLText()
	{
		return Text.Replace(',', '.');
	}

	public usgNumberField()
	{
		InitializeComponent();
	}

	public void usgNumberField_KeyPress(object sender, KeyPressEventArgs e)
	{
		switch (e.KeyChar)
		{
		case '\b':
		case ',':
		case '-':
		case '.':
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			e.Handled = false;
			break;
		default:
			e.Handled = true;
			break;
		}
	}

	private void InitializeComponent()
	{
	}
}
