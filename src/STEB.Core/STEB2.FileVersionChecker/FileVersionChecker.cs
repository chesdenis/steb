using STEB2.DataBase.Json;
using STEB2.DataBase;

namespace STEB2.FileVersionChecker;

public class FileVersionChecker
{
	public static void CheckAndUpdateFile()
	{
		using JsonDbConnection oleDbConnection = new JsonDbConnection(Tools.cnnString + Tools.workFile);
		oleDbConnection.Open();
		if (!VerifyTableExists(oleDbConnection, "tblCisterns"))
		{
			CreateCisternTable(oleDbConnection);
		}
		oleDbConnection.Close();
	}

	private static void CreateCisternTable(JsonDbConnection cnn)
	{
		Tools.ExecuteSQL("CREATE TABLE tblCisterns  (Name CHAR CONSTRAINT key1 PRIMARY KEY, x0 FLOAT, y0 FLOAT, z0 FLOAT, x1 FLOAT, y1 FLOAT, z1 FLOAT, percentOfVolume FLOAT, volumeDensity FLOAT, TextDecriptionOfStation TEXT);", cnn);
	}

	private static bool VerifyTableExists(JsonDbConnection cnn, string TableName)
	{
		try
		{
			using (JsonDbCommand oleDbCommand = new JsonDbCommand("SELECT * FROM " + TableName, cnn))
			{
				using (JsonDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
				{
					oleDbDataReader.Read();
					oleDbDataReader.Close();
				}
				oleDbCommand.Dispose();
			}
			return true;
		}
		catch
		{
			return false;
		}
	}
}
