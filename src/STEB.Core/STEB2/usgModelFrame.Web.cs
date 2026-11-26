using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace STEB2;

/// <summary>
/// Web-host side of <see cref="usgModelFrame"/>: replaces the WinForms PictureBox,
/// toolbar buttons and mouse plumbing. (The AutoCAD COM export of the original is not supported.)
/// </summary>
public partial class usgModelFrame
{
    /// <summary>Minimal stand-in for the PictureBox / ToolStrip the original code measured.</summary>
    private sealed class Surface
    {
        public Surface(int width, int height) { Width = width; Height = height; }
        public int Width { get; set; }
        public int Height { get; set; }
        public void Invalidate() => Redrawn?.Invoke();
        public void Focus() { }
        public event Action Redrawn;
    }


    /// <summary>Mouse position over the drawing in pixels (set by the viewer).</summary>
    public Point CursorPosition { get; set; }

    /// <summary>Raised whenever the original code asked the control to repaint.</summary>
    public event EventHandler Invalidated;

    public override void Invalidate()
    {
        Invalidated?.Invoke(this, EventArgs.Empty);
        base.Invalidate();
    }

    private Point PointToClient(Point p) => p;

    /// <summary>Size of the drawing area in pixels (the former PictureBox size).</summary>
    public int ViewWidth => pictureBox_ModelFrame.Width;
    public int ViewHeight => pictureBox_ModelFrame.Height;

    public void SetViewSize(int width, int height)
    {
        if (width > 0) pictureBox_ModelFrame.Width = width;
        if (height > 0) pictureBox_ModelFrame.Height = height;
        NeedRecalcPoints = true;
    }

    // ------------------------------------------------------------------ rendering
    /// <summary>Renders exactly what the original Paint handler drew, as SVG.</summary>
    public string RenderSvg()
    {
        using var g = new Graphics(pictureBox_ModelFrame.Width, pictureBox_ModelFrame.Height);
        pictureBox_ModelFrame_Paint(this, new PaintEventArgs(g, new Rectangle(0, 0, pictureBox_ModelFrame.Width, pictureBox_ModelFrame.Height)));
        return g.ToSvg();
    }

    /// <summary>
    /// Replaces the "Сохранить изображение" button: same content as the original
    /// bitmap export (white background, coordinates, caption), returned as SVG.
    /// </summary>
    public string ExportImageSvg()
    {
        Image image = new Bitmap(pictureBox_ModelFrame.Width, pictureBox_ModelFrame.Height);
        using (Graphics graphics = Graphics.FromImage(image))
        {
            graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, pictureBox_ModelFrame.Width, pictureBox_ModelFrame.Height));
            graphics.DrawString(screenCenter.X / ScaleX + ";" + screenCenter.Y / ScaleY + "(" + screenScale + ")", Font, Brushes.Blue, new PointF(0f, pictureBox_ModelFrame.Height - Font.Height));
            drawFilling(graphics);
            drawHatch(graphics);
            drawLines(graphics);
            drawPolylines(graphics);
            drawPolylinesSplines(graphics);
            drawCircles(graphics);
            drawTexts(graphics);
            drawDots(graphics);
            drawDimensions(graphics);
            if (DrawOrigin)
            {
                DrawCenterOrigin(graphics);
            }
            graphics.DrawString(_modelFrameCaption, Font, Brushes.Black, new PointF(0f, 0f));
        }
        return image.ToSvg();
    }

    // ------------------------------------------------------------------ mouse (forwarded by the viewer)
    public void MouseDown(int x, int y) { CursorPosition = new Point(x, y); pictureBox_ModelFrame_MouseDown(this, new MouseEventArgs(MouseButtons.Left, 1, x, y, 0)); }
    public void MouseMove(int x, int y) { CursorPosition = new Point(x, y); pictureBox_ModelFrame_MouseMove(this, new MouseEventArgs(MouseButtons.None, 0, x, y, 0)); }
    public void MouseUp(int x, int y) { CursorPosition = new Point(x, y); pictureBox_ModelFrame_MouseUp(this, new MouseEventArgs(MouseButtons.Left, 1, x, y, 0)); }
    /// <summary>delta: WinForms wheel units (120 per notch, positive = away from user).</summary>
    public void MouseWheel(int x, int y, int delta) { CursorPosition = new Point(x, y); pictureBox_ModelFrame_MouseWheel(this, new MouseEventArgs(MouseButtons.None, 0, x, y, delta)); }
    public void MouseEnter() => pictureBox_ModelFrame_MouseEnter(this, EventArgs.Empty);
    public void MouseLeave() => pictureBox_ModelFrame_MouseLeave(this, EventArgs.Empty);
}
