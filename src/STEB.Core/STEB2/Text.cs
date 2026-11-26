using System.Drawing;

namespace STEB2;

public class Text
{
	private ContentAlignment _TextAlign;

	private bool _drawAlways;

	private int _index;

	private string _prefix;

	private Color _color;

	private Font _textFont;

	private string _text;

	private PointF[] _pnts;

	private PointF[] _drwpnts;

	private bool _isdraw;

	public ContentAlignment TextAlign
	{
		get
		{
			return _TextAlign;
		}
		set
		{
			_TextAlign = value;
		}
	}

	public bool drawAlways
	{
		get
		{
			return _drawAlways;
		}
		set
		{
			_drawAlways = value;
		}
	}

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

	public Font textFont
	{
		get
		{
			return _textFont;
		}
		set
		{
			_textFont = value;
		}
	}

	public string text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
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

	public bool isdraw
	{
		get
		{
			return _isdraw;
		}
		set
		{
			_isdraw = value;
		}
	}

	public Text(PointF[] pnts, Font textFont, string text, Color color, ContentAlignment textAlign, string prefix)
	{
		TextAlign = textAlign;
		_pnts = pnts;
		_text = text;
		_textFont = textFont;
		_color = color;
		_prefix = prefix;
		drawAlways = false;
	}

	public Text(PointF[] pnts, Font textFont, string text, Color color, ContentAlignment textAlign, string prefix, bool drawAlways)
	{
		TextAlign = textAlign;
		_pnts = pnts;
		_text = text;
		_textFont = textFont;
		_color = color;
		_prefix = prefix;
		this.drawAlways = drawAlways;
	}
}
