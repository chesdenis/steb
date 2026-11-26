using System.Drawing.Drawing2D;

namespace System.Drawing
{
    public sealed class FontFamily
    {
        public FontFamily(string name) { Name = name; }
        public string Name { get; }
        public static FontFamily GenericSansSerif => new FontFamily("Microsoft Sans Serif");
        public override string ToString() => "[FontFamily: Name=" + Name + "]";
    }

    /// <summary>Font description only; metrics are approximated (no GDI+ available).</summary>
    public sealed class Font : IDisposable
    {
        public Font(FontFamily family, float emSize) : this(family, emSize, FontStyle.Regular, GraphicsUnit.Point) { }
        public Font(FontFamily family, float emSize, FontStyle style) : this(family, emSize, style, GraphicsUnit.Point) { }
        public Font(FontFamily family, float emSize, GraphicsUnit unit) : this(family, emSize, FontStyle.Regular, unit) { }
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit)
        {
            FontFamily = family ?? FontFamily.GenericSansSerif; Size = emSize; Style = style; Unit = unit;
        }
        public Font(string familyName, float emSize) : this(new FontFamily(familyName), emSize) { }
        public Font(string familyName, float emSize, FontStyle style) : this(new FontFamily(familyName), emSize, style) { }
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit) : this(new FontFamily(familyName), emSize, style, unit) { }
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet) : this(new FontFamily(familyName), emSize, style, unit) { }
        public Font(Font prototype, FontStyle newStyle) : this(prototype.FontFamily, prototype.Size, newStyle, prototype.Unit) { }

        /// <summary>WinForms default control font (Microsoft Sans Serif 8.25pt).</summary>
        public static Font DefaultControlFont => new Font("Microsoft Sans Serif", 8.25f);

        public FontFamily FontFamily { get; }
        public string Name => FontFamily.Name;
        public float Size { get; }
        public FontStyle Style { get; }
        public GraphicsUnit Unit { get; }
        public bool Bold => (Style & FontStyle.Bold) != 0;
        public bool Italic => (Style & FontStyle.Italic) != 0;
        public float SizeInPoints => Unit == GraphicsUnit.Pixel ? Size * 72f / 96f : Size;
        /// <summary>Size in device pixels at 96 DPI.</summary>
        public float SizeInPixels => Unit == GraphicsUnit.Pixel ? Size : Size * 96f / 72f;
        /// <summary>Line spacing in pixels, approximating GDI+ for sans-serif fonts.</summary>
        public int Height => (int)Math.Ceiling(SizeInPixels * 1.15f);
        public float GetHeight() => SizeInPixels * 1.15f;
        public void Dispose() { }
    }

    /// <summary>Window icon placeholder (icons come from resources the web app does not use).</summary>
    public sealed class Icon : IDisposable
    {
        public void Dispose() { }
    }

    public abstract class Brush : IDisposable
    {
        public virtual void Dispose() { }
    }

    public sealed class SolidBrush : Brush
    {
        public SolidBrush(Color color) { Color = color; }
        public Color Color { get; set; }
    }

    public static class Brushes
    {
        public static Brush Transparent => new SolidBrush(Color.Transparent);
        public static Brush White => new SolidBrush(Color.White);
        public static Brush Black => new SolidBrush(Color.Black);
        public static Brush Blue => new SolidBrush(Color.Blue);
        public static Brush Red => new SolidBrush(Color.Red);
        public static Brush Green => new SolidBrush(Color.Green);
        public static Brush DarkGreen => new SolidBrush(Color.DarkGreen);
        public static Brush DarkBlue => new SolidBrush(Color.DarkBlue);
        public static Brush Gray => new SolidBrush(Color.Gray);
        public static Brush LightGray => new SolidBrush(Color.LightGray);
        public static Brush Yellow => new SolidBrush(Color.Yellow);
        public static Brush Aqua => new SolidBrush(Color.Aqua);
        public static Brush Orange => new SolidBrush(Color.Orange);
    }

    public static class Pens
    {
        public static Pen Black => new Pen(Color.Black);
        public static Pen Blue => new Pen(Color.Blue);
        public static Pen Red => new Pen(Color.Red);
        public static Pen Green => new Pen(Color.Green);
        public static Pen Gray => new Pen(Color.Gray);
        public static Pen White => new Pen(Color.White);
    }

    public sealed class Pen : IDisposable
    {
        public Pen(Color color) : this(color, 1f) { }
        public Pen(Color color, float width) { Color = color; Width = width; }
        public Pen(Brush brush) : this(brush, 1f) { }
        public Pen(Brush brush, float width)
        {
            Color = brush is SolidBrush sb ? sb.Color : brush is HatchBrush hb ? hb.ForegroundColor : Color.Black;
            Width = width;
        }
        public Color Color { get; set; }
        public float Width { get; set; }
        public DashStyle DashStyle { get; set; } = DashStyle.Solid;
        public LineCap StartCap { get; set; } = LineCap.Flat;
        public LineCap EndCap { get; set; } = LineCap.Flat;
        public float MiterLimit { get; set; } = 10f;
        public void Dispose() { }
    }
}

namespace System.Drawing.Drawing2D
{
    public sealed class HatchBrush : Brush
    {
        public HatchBrush(HatchStyle hatchStyle, Color foreColor) : this(hatchStyle, foreColor, Color.Transparent) { }
        public HatchBrush(HatchStyle hatchStyle, Color foreColor, Color backColor)
        {
            HatchStyle = hatchStyle; ForegroundColor = foreColor; BackgroundColor = backColor;
        }
        public HatchStyle HatchStyle { get; }
        public Color ForegroundColor { get; }
        public Color BackgroundColor { get; }
    }
}
