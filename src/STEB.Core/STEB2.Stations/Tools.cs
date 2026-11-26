using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using STEB2.Converting;
using STEB2.Geometry;

namespace STEB2.Stations;

public class Tools
{
	public static List<Station> SortStations(List<Station> inputStations)
	{
		for (int i = 1; i < inputStations.Count; i++)
		{
			for (int num = inputStations.Count - 1; num >= i; num--)
			{
				if (inputStations[num - 1].X > inputStations[num].X)
				{
					Station value = inputStations[num - 1];
					inputStations[num - 1] = inputStations[num];
					inputStations[num] = value;
				}
			}
		}
		return inputStations;
	}

	public static Station[] SortStations(Station[] inputStations)
	{
		for (int i = 1; i < inputStations.Length; i++)
		{
			for (int num = inputStations.Length - 1; num >= i; num--)
			{
				if (inputStations[num - 1].X > inputStations[num].X)
				{
					Station station = inputStations[num - 1];
					inputStations[num - 1] = inputStations[num];
					inputStations[num] = station;
				}
			}
		}
		return inputStations;
	}

	public static List<ImmersedStation> SortImmersedStations(List<ImmersedStation> inputStations)
	{
		for (int i = 1; i < inputStations.Count; i++)
		{
			for (int num = inputStations.Count - 1; num >= i; num--)
			{
				if (inputStations[num - 1].X > inputStations[num].X)
				{
					ImmersedStation value = inputStations[num - 1];
					inputStations[num - 1] = inputStations[num];
					inputStations[num] = value;
				}
			}
		}
		return inputStations;
	}

	public static ImmersedStation[] SortImmersedStations(ImmersedStation[] inputStations)
	{
		for (int i = 1; i < inputStations.Length; i++)
		{
			for (int num = inputStations.Length - 1; num >= i; num--)
			{
				if (inputStations[num - 1].X > inputStations[num].X)
				{
					ImmersedStation immersedStation = inputStations[num - 1];
					inputStations[num - 1] = inputStations[num];
					inputStations[num] = immersedStation;
				}
			}
		}
		return inputStations;
	}

	public static List<Station> TextToStations(string Text)
	{
		List<Station> list = new List<Station>();
		new List<Contur>();
		bool flag = false;
		int startIndex = 0;
		int num = 0;
		while (!flag)
		{
			startIndex = Text.IndexOf("new", startIndex);
			if (startIndex == -1)
			{
				break;
			}
			startIndex += "new".Length;
			num = ((Text.IndexOf("new", startIndex) == -1) ? Text.Length : Text.IndexOf("new", startIndex));
			string text = Text.Substring(startIndex, num - startIndex);
			bool flag2 = text.IndexOf("mirror") != -1;
			string text2 = "";
			for (int i = 0; i <= text.Length - 1; i++)
			{
				text2 = text.Substring(i, 1) switch
				{
					"0" => text2 + "0", 
					"1" => text2 + "1", 
					"2" => text2 + "2", 
					"3" => text2 + "3", 
					"4" => text2 + "4", 
					"5" => text2 + "5", 
					"6" => text2 + "6", 
					"7" => text2 + "7", 
					"8" => text2 + "8", 
					"9" => text2 + "9", 
					"." => text2 + ".", 
					"," => text2 + ",", 
					" " => text2 + " ", 
					"-" => text2 + "-", 
					_ => text2 + " ", 
				};
			}
			if (text2.Trim() != "")
			{
				string input = text2;
				string[] array = Regex.Split(input, " ");
				int num2 = -1;
				for (int j = 0; j <= array.Length - 1; j++)
				{
					if (array[j] != "")
					{
						num2++;
						array[num2] = array[j];
					}
				}
				Array.Resize(ref array, num2 + 1);
				try
				{
					p2d[] array2 = new p2d[1];
					for (int k = 1; k <= array.Length - 1; k += 2)
					{
						array2[array2.Length - 1].X = STEB2.Converting.Converting.TextToFloat(array[k]);
						array2[array2.Length - 1].Y = STEB2.Converting.Converting.TextToFloat(array[k + 1]);
						Array.Resize(ref array2, array2.Length + 1);
					}
					if (!flag2)
					{
						Array.Resize(ref array2, array2.Length - 1);
					}
					if (flag2)
					{
						for (int num3 = array.Length - 1; num3 >= 1; num3 -= 2)
						{
							array2[array2.Length - 1].X = -1f * STEB2.Converting.Converting.TextToFloat(array[num3 - 1]);
							array2[array2.Length - 1].Y = STEB2.Converting.Converting.TextToFloat(array[num3]);
							Array.Resize(ref array2, array2.Length + 1);
						}
						Array.Resize(ref array2, array2.Length - 1);
					}
					if (array2[array2.Length - 1].X != array2[0].X && array2[array2.Length - 1].Y != array2[0].Y)
					{
						Array.Resize(ref array2, array2.Length + 1);
						array2[array2.Length - 1].X = array2[0].X;
						array2[array2.Length - 1].Y = array2[0].Y;
					}
					Contur contur = new Contur(array2);
					bool flag3 = false;
					foreach (Station item in list)
					{
						if (item.X == STEB2.Converting.Converting.TextToFloat(array[0]))
						{
							item.StationContur.AddContur(contur);
							flag3 = true;
						}
					}
					if (!flag3)
					{
						list.Add(new Station(new MultipleContur(new Contur[1] { contur }), STEB2.Converting.Converting.TextToFloat(array[0])));
					}
				}
				catch
				{
				}
			}
			startIndex = num;
		}
		return list;
	}
}
