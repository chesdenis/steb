using System.Drawing;
using System.Drawing.Drawing2D;

namespace STEB2;

public class Dimension
{
	public Font TextFont;

	public float distance;

	public string Text;

	public PointF sp;

	public PointF ep;

	public DimOrientation orient;

	public string prefix;

	public DashStyle dashStyle;

	public int penWidth;

	public Color color;

	public Dimension(PointF sp, PointF ep, DimOrientation orient, float distance, string Text, Font TextFont, DashStyle dashStyle, Color color, int penWidth, string prefix)
	{
		this.sp = sp;
		this.ep = ep;
		this.orient = orient;
		this.distance = ((orient == DimOrientation.vert) ? (0f - distance) : distance);
		this.Text = Text;
		this.TextFont = TextFont;
		this.color = color;
		this.dashStyle = dashStyle;
		this.penWidth = penWidth;
		this.prefix = prefix;
	}
}
