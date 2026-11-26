using System.Drawing;
using System.Drawing.Drawing2D;

namespace STEB2;

public class FillingContur
{
	private int _index;

	private string _prefix;

	private DashStyle _dashStyle;

	private PointF[] _pnts;

	private PointF[] _drwpnts;

	private int _penWidth;

	private Color _color;

	private Color _FillColor;

	public int index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	public string prefix
	{
		get
		{
			return _prefix;
		}
		set
		{
			_prefix = value;
		}
	}

	public DashStyle dashStyle
	{
		get
		{
			return _dashStyle;
		}
		set
		{
			_dashStyle = value;
		}
	}

	public PointF[] pnts
	{
		get
		{
			return _pnts;
		}
		set
		{
			_pnts = value;
		}
	}

	public PointF[] drwpnts
	{
		get
		{
			return _drwpnts;
		}
		set
		{
			_drwpnts = value;
		}
	}

	public int penWidth
	{
		get
		{
			return _penWidth;
		}
		set
		{
			_penWidth = value;
		}
	}

	public Color color
	{
		get
		{
			return _color;
		}
		set
		{
			_color = value;
		}
	}

	public Color FillColor
	{
		get
		{
			return _FillColor;
		}
		set
		{
			_FillColor = value;
		}
	}

	public FillingContur(DashStyle dashStyle, PointF[] pnts, int penWidth, Color color, Color FillColor, string prefix)
	{
		_color = color;
		_dashStyle = dashStyle;
		_penWidth = penWidth;
		_pnts = pnts;
		_FillColor = FillColor;
		_prefix = prefix;
	}
}
