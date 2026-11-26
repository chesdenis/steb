using System.Windows.Forms;

namespace STEB2;

public static class Progress
{
	public static string status = "";

	public static int _progressMaximum = 0;

	private static int _progressPosition = 0;

	public static int ProgressMaximum
	{
		get
		{
			return _progressMaximum;
		}
		set
		{
			_progressMaximum = value;
			_progressPosition = 0;
			try
			{
				Application.DoEvents();
			}
			catch
			{
			}
		}
	}

	public static int ProgressPosition
	{
		get
		{
			return _progressPosition;
		}
		set
		{
			_progressPosition = value;
			try
			{
				Application.DoEvents();
			}
			catch
			{
			}
		}
	}
}
