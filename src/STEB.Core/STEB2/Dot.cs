using System.Drawing;

namespace STEB2;

public class Dot
{
	private int _index;

	private string _prefix;

	private PointF[] _pnts;

	private PointF[] _drwpnts;

	private float _radius;

	private Color _color;

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

	public float radius
	{
		get
		{
			return _radius;
		}
		set
		{
			_radius = value;
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

	public Dot(PointF[] pnts, float radius, Color color, string prefix)
	{
		_color = color;
		_pnts = pnts;
		_radius = radius;
		_prefix = prefix;
	}
}
