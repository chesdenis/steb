using System;
using System.Collections.Generic;
using STEB2.DataBase.Json;
using System.Text;
using System.Windows.Forms;
using STEB2.Converting;

namespace STEB2.DataBase;

public class Tools
{
	public static string cnnString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=";

	public static string workFile = "";

	public static Log log = new Log();

	public static void ExecuteUpdateSQL(string TableName, string FieldName, float NewValue, JsonDbConnection cnn)
	{
		ExecuteSQL("UPDATE " + TableName + " SET " + FieldName + " = " + NewValue.ToString().Replace(',', '.'), cnn);
	}

	public static void ExecuteUpdateSQL(string TableName, string FieldName, string NewValue, JsonDbConnection cnn)
	{
		ExecuteSQL("UPDATE " + TableName + " SET " + FieldName + " = '" + NewValue + "'", cnn);
	}

	public static void ExecuteSQL(string SQLText, JsonDbConnection cnn)
	{
		try
		{
			new JsonDbCommand(SQLText, cnn).ExecuteNonQuery();
		}
		catch (Exception ex)
		{
			log.writeLog(" ВНИМАНИЕ! " + ex.Message);
		}
	}

	public static void ExecuteRangeSQL(List<string> listSQLText, JsonDbConnection cnn)
	{
		for (int i = 0; i < listSQLText.Count; i++)
		{
			try
			{
				new JsonDbCommand(listSQLText[i], cnn).ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				log.writeLog(" ВНИМАНИЕ! " + ex.Message);
			}
		}
	}

	public static string[] ReturnScalarArray(string SQLText, string fieldName, JsonDbConnection cnn)
	{
		string[] array = new string[1];
		try
		{
			using (JsonDbCommand oleDbCommand = new JsonDbCommand(SQLText, cnn))
			{
				using (JsonDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
				{
					while (oleDbDataReader.Read())
					{
						array[array.Length - 1] = oleDbDataReader[fieldName].ToString();
						Array.Resize(ref array, array.Length + 1);
					}
					oleDbDataReader.Close();
				}
				oleDbCommand.Dispose();
			}
			Array.Resize(ref array, array.Length - 1);
		}
		catch (Exception ex)
		{
			log.writeLog(" ВНИМАНИЕ! " + ex.Message);
		}
		return array;
	}

	public static object ReturnScalar(string SQLText, string fieldName, JsonDbConnection cnn)
	{
		object result = new object();
		try
		{
			using JsonDbCommand oleDbCommand = new JsonDbCommand(SQLText, cnn);
			using (JsonDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
			{
				if (oleDbDataReader.Read())
				{
					result = oleDbDataReader[fieldName];
					oleDbDataReader.Close();
					oleDbCommand.Dispose();
					return result;
				}
				oleDbDataReader.Close();
			}
			oleDbCommand.Dispose();
		}
		catch (Exception ex)
		{
			log.writeLog(" ВНИМАНИЕ! " + ex.Message);
		}
		return result;
	}

	public static string ReturnShipFormText(string ShipFormName, JsonDbConnection cnn)
	{
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			float num = float.NaN;
			using JsonDbCommand oleDbCommand = new JsonDbCommand("SELECT tblStations.ShipFormName, tblStations.X, tblStationsContursPoints.PointX, tblStationsContursPoints.PointY, tblStationsContursPoints.PointID FROM (tblShipForms INNER JOIN tblStations ON tblShipForms.ShipFormName = tblStations.ShipFormName) INNER JOIN (tblStationsConturs INNER JOIN tblStationsContursPoints ON tblStationsConturs.ConturID = tblStationsContursPoints.ConturID) ON tblStations.StationID = tblStationsConturs.StationID WHERE (((tblStations.ShipFormName)='" + ShipFormName + "')) ORDER BY tblStations.X, tblStationsContursPoints.PointID;", cnn);
			using (JsonDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
			{
				while (oleDbDataReader.Read())
				{
					Application.DoEvents();
					if (float.IsNaN(num))
					{
						stringBuilder.AppendLine("new " + oleDbDataReader["X"].ToString());
						stringBuilder.AppendLine("Y = " + oleDbDataReader["PointX"].ToString() + " Z = " + oleDbDataReader["PointY"].ToString());
						num = STEB2.Converting.Converting.TextToFloat(oleDbDataReader["X"].ToString());
					}
					else if (num != STEB2.Converting.Converting.TextToFloat(oleDbDataReader["X"].ToString()))
					{
						stringBuilder.AppendLine("new " + oleDbDataReader["X"].ToString());
						stringBuilder.AppendLine("Y = " + oleDbDataReader["PointX"].ToString() + " Z = " + oleDbDataReader["PointY"].ToString());
						num = STEB2.Converting.Converting.TextToFloat(oleDbDataReader["X"].ToString());
					}
					else
					{
						stringBuilder.AppendLine("Y = " + oleDbDataReader["PointX"].ToString() + " Z = " + oleDbDataReader["PointY"].ToString());
					}
				}
				oleDbDataReader.Close();
			}
			oleDbCommand.Dispose();
		}
		catch (Exception ex)
		{
			log.writeLog(" ВНИМАНИЕ! " + ex.Message);
		}
		return stringBuilder.ToString();
	}

	public static string[] ReturnArrayOfResultByArrayOfFields(string SQLText, string[] ArrayOfFields, JsonDbConnection cnn)
	{
		string[] array = new string[1];
		try
		{
			for (int i = 0; i < ArrayOfFields.Length; i++)
			{
				array[array.Length - 1] = ReturnScalar(SQLText, ArrayOfFields[i], cnn).ToString();
				Array.Resize(ref array, array.Length + 1);
			}
			Array.Resize(ref array, array.Length - 1);
		}
		catch (Exception ex)
		{
			log.writeLog(" ВНИМАНИЕ! " + ex.Message);
		}
		return array;
	}
}
