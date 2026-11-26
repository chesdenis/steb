using System;
using System.Collections.Generic;

namespace STEB2.DataBase;

public class Log
{
	public List<string> LogFile = new List<string>();

	public event EventHandler LogFires;

	public void writeLog(string text)
	{
		if (LogFile.Count > 50)
		{
			LogFile.Clear();
		}
		LogFile.Add(text);
		if (LogFires != null)
		{
			LogFires(new object(), new EventArgs());
		}
	}
}
