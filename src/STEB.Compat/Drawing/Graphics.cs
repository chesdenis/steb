using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;

namespace System.Drawing
{
    /// <summary>
    /// GDI+-compatible drawing surface that records calls as SVG elements.
    /// Only the members used by the STEB code are implemented.
    /// </summary>
    public sealed class Graphics : IDisposable
    {
        private readonly StringBuilder _body = new StringBuilder();
        private readonly Dictionary<string, string> _defs = new Dictionary<string, string>();
        private Color _background = Color.Transparent;
        private Image _target;

        public Graphics() { }
        public Graphics(float width, float height) { Width = width; Height = height; }

        public float Width { get; set; }
        public float Height { get; set; }
        public SmoothingMode SmoothingMode { get; set; }

        public static Graphics FromImage(Image image)
        {
            var g = new Graphics(image.Width, image.Height) { _target = image };
            image.Attach(g);
            return g;
        }

        // ---------------------------------------------------------------- output
        public string ToSvg() => ToSvg(Width, Height);

        public string ToSvg(float width, float height)
        {
            var sb = new StringBuilder();
            sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"").Append(F(width)).Append("\" height=\"").Append(F(height))
              .Append("\" viewBox=\"0 0 ").Append(F(width)).Append(' ').Append(F(height)).Append("\" font-family=\"'Microsoft Sans Serif',Arial,sans-serif\">");
            if (_defs.Count > 0 || _arrowUsed) { sb.Append("<defs>"); foreach (var d in _defs.Values) sb.Append(d); if (_arrowUsed) sb.Append(ArrowDefs); sb.Append("</defs>"); }
            if (_background.A > 0) sb.Append("<rect x=\"0\" y=\"0\" width=\"100%\" height=\"100%\" fill=\"").Append(Col(_background)).Append("\"/>");
            sb.Append(_body);
            sb.Append("</svg>");
            return sb.ToString();
        }

        // ---------------------------------------------------------------- API
        public void Clear(Color color) { _body.Clear(); _background = color; }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            if (!Finite(x1, y1, x2, y2)) return;
            _body.Append("<line x1=\"").Append(F(x1)).Append("\" y1=\"").Append(F(y1)).Append("\" x2=\"").Append(F(x2)).Append("\" y2=\"").Append(F(y2)).Append('"');
            Stroke(pen); _body.Append("/>");
        }
        public void DrawLine(Pen pen, PointF p1, PointF p2) => DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);
        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2) => DrawLine(pen, (float)x1, y1, x2, y2);
        public void DrawLine(Pen pen, Point p1, Point p2) => DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);

        public void DrawLines(Pen pen, PointF[] points)
        {
            if (points == null || points.Length < 2) return;
            _body.Append("<polyline points=\"").Append(Pts(points)).Append("\" fill=\"none\"");
            Stroke(pen); _body.Append("/>");
        }
        public void DrawLines(Pen pen, Point[] points) => DrawLines(pen, ToF(points));

        public void DrawPolygon(Pen pen, PointF[] points)
        {
            if (points == null || points.Length < 2) return;
            _body.Append("<polygon points=\"").Append(Pts(points)).Append("\" fill=\"none\"");
            Stroke(pen); _body.Append("/>");
        }

        public void FillPolygon(Brush brush, PointF[] points)
        {
            if (points == null || points.Length < 3) return;
            _body.Append("<polygon points=\"").Append(Pts(points)).Append('"');
            Fill(brush); _body.Append("/>");
        }
        public void FillPolygon(Brush brush, Point[] points) => FillPolygon(brush, ToF(points));

        /// <summary>Cardinal spline with GDI+ default tension 0.5, emitted as cubic Béziers.</summary>
        public void DrawCurve(Pen pen, PointF[] points) => DrawCurve(pen, points, 0.5f);
        public void DrawCurve(Pen pen, PointF[] points, float tension)
        {
            if (points == null || points.Length < 2) return;
            float k = tension / 3f;
            var sb = new StringBuilder();
            sb.Append('M').Append(F(points[0].X)).Append(',').Append(F(points[0].Y));
            for (int i = 0; i < points.Length - 1; i++)
            {
                PointF p0 = points[Math.Max(i - 1, 0)], p1 = points[i], p2 = points[i + 1], p3 = points[Math.Min(i + 2, points.Length - 1)];
                var c1 = new PointF(p1.X + k * (p2.X - p0.X), p1.Y + k * (p2.Y - p0.Y));
                var c2 = new PointF(p2.X - k * (p3.X - p1.X), p2.Y - k * (p3.Y - p1.Y));
                sb.Append(" C").Append(F(c1.X)).Append(',').Append(F(c1.Y)).Append(' ').Append(F(c2.X)).Append(',').Append(F(c2.Y))
                  .Append(' ').Append(F(p2.X)).Append(',').Append(F(p2.Y));
            }
            _body.Append("<path d=\"").Append(sb).Append("\" fill=\"none\"");
            Stroke(pen); _body.Append("/>");
        }

        public void DrawEllipse(Pen pen, RectangleF r)
        {
            if (!Finite(r.X, r.Y, r.Width, r.Height)) return;
            _body.Append("<ellipse cx=\"").Append(F(r.X + r.Width / 2)).Append("\" cy=\"").Append(F(r.Y + r.Height / 2))
                 .Append("\" rx=\"").Append(F(Math.Abs(r.Width / 2))).Append("\" ry=\"").Append(F(Math.Abs(r.Height / 2))).Append("\" fill=\"none\"");
            Stroke(pen); _body.Append("/>");
        }
        public void DrawEllipse(Pen pen, float x, float y, float w, float h) => DrawEllipse(pen, new RectangleF(x, y, w, h));

        public void FillEllipse(Brush brush, RectangleF r)
        {
            if (!Finite(r.X, r.Y, r.Width, r.Height)) return;
            _body.Append("<ellipse cx=\"").Append(F(r.X + r.Width / 2)).Append("\" cy=\"").Append(F(r.Y + r.Height / 2))
                 .Append("\" rx=\"").Append(F(Math.Abs(r.Width / 2))).Append("\" ry=\"").Append(F(Math.Abs(r.Height / 2))).Append('"');
            Fill(brush); _body.Append("/>");
        }
        public void FillEllipse(Brush brush, float x, float y, float w, float h) => FillEllipse(brush, new RectangleF(x, y, w, h));

        public void FillRectangle(Brush brush, RectangleF r)
        {
            if (!Finite(r.X, r.Y, r.Width, r.Height)) return;
            _body.Append("<rect x=\"").Append(F(r.X)).Append("\" y=\"").Append(F(r.Y)).Append("\" width=\"").Append(F(Math.Abs(r.Width)))
                 .Append("\" height=\"").Append(F(Math.Abs(r.Height))).Append('"');
            Fill(brush); _body.Append("/>");
        }
        public void FillRectangle(Brush brush, Rectangle r) => FillRectangle(brush, new RectangleF(r.X, r.Y, r.Width, r.Height));
        public void FillRectangle(Brush brush, float x, float y, float w, float h) => FillRectangle(brush, new RectangleF(x, y, w, h));

        public void DrawRectangle(Pen pen, float x, float y, float w, float h)
        {
            _body.Append("<rect x=\"").Append(F(x)).Append("\" y=\"").Append(F(y)).Append("\" width=\"").Append(F(Math.Abs(w)))
                 .Append("\" height=\"").Append(F(Math.Abs(h))).Append("\" fill=\"none\"");
            Stroke(pen); _body.Append("/>");
        }
        public void DrawRectangle(Pen pen, Rectangle r) => DrawRectangle(pen, r.X, r.Y, r.Width, r.Height);

        /// <summary>Draws text with its layout box's top-left corner at <paramref name="point"/> (GDI+ semantics).</summary>
        public void DrawString(string s, Font font, Brush brush, PointF point)
        {
            if (string.IsNullOrEmpty(s) || !Finite(point.X, point.Y)) return;
            s = Translate(s);
            float px = font.SizeInPixels;
            string[] lines = s.Replace("\r\n", "\n").Split('\n');
            float pad = px / 6f;
            for (int i = 0; i < lines.Length; i++)
            {
                _body.Append("<text x=\"").Append(F(point.X + pad)).Append("\" y=\"").Append(F(point.Y + i * font.GetHeight() + px * 0.95f))
                     .Append("\" font-size=\"").Append(F(px)).Append('"');
                if (font.Bold) _body.Append(" font-weight=\"bold\"");
                if (font.Italic) _body.Append(" font-style=\"italic\"");
                Fill(brush);
                _body.Append(" xml:space=\"preserve\">").Append(Esc(lines[i])).Append("</text>");
            }
        }
        public void DrawString(string s, Font font, Brush brush, float x, float y) => DrawString(s, font, brush, new PointF(x, y));
        public void DrawString(string s, Font font, Brush brush, RectangleF layout) => DrawString(s, font, brush, layout.Location);

        /// <summary>Approximate GDI+ MeasureString for a proportional sans-serif font.</summary>
        public SizeF MeasureString(string s, Font font)
        {
            if (string.IsNullOrEmpty(s)) return new SizeF(0, font.GetHeight());
            s = Translate(s);
            string[] lines = s.Replace("\r\n", "\n").Split('\n');
            float px = font.SizeInPixels, w = 0;
            foreach (var l in lines) w = Math.Max(w, TextWidth(l, px, font.Bold));
            return new SizeF(w + px / 3f, lines.Length * font.GetHeight());
        }

        public void Dispose() { _target?.Detach(this); }

        /// <summary>
        /// Display-language hook (web port): texts drawn by the original code are passed through it
        /// just before rendering. Null = draw the text unchanged.
        /// </summary>
        public static Func<string, string> TextTransform { get; set; }

        public static string Translate(string s) => TextTransform == null || string.IsNullOrEmpty(s) ? s : TextTransform(s);

        // ---------------------------------------------------------------- helpers
        private static float TextWidth(string s, float px, bool bold)
        {
            float w = 0;
            foreach (char c in s)
            {
                float f = c == ' ' ? 0.28f : c == '.' || c == ',' || c == ':' || c == ';' || c == 'i' || c == 'l' || c == '|' ? 0.26f
                        : char.IsDigit(c) ? 0.56f : char.IsUpper(c) ? 0.68f : 0.55f;
                w += f * px;
            }
            return bold ? w * 1.07f : w;
        }

        private bool _arrowUsed;
        private const string ArrowDefs =
            "<marker id=\"arrowEnd\" viewBox=\"0 0 10 10\" refX=\"10\" refY=\"5\" markerWidth=\"6\" markerHeight=\"6\" orient=\"auto\" markerUnits=\"userSpaceOnUse\"><path d=\"M0,0 L10,5 L0,10 z\" fill=\"context-stroke\"/></marker>" +
            "<marker id=\"arrowStart\" viewBox=\"0 0 10 10\" refX=\"0\" refY=\"5\" markerWidth=\"6\" markerHeight=\"6\" orient=\"auto\" markerUnits=\"userSpaceOnUse\"><path d=\"M10,0 L0,5 L10,10 z\" fill=\"context-stroke\"/></marker>";

        private void Stroke(Pen pen)
        {
            _body.Append(" stroke=\"").Append(Col(pen.Color)).Append('"');
            if (pen.Color.A < 255) _body.Append(" stroke-opacity=\"").Append(F(pen.Color.A / 255f)).Append('"');
            _body.Append(" stroke-width=\"").Append(F(pen.Width <= 0 ? 1 : pen.Width)).Append('"');
            float w = Math.Max(pen.Width, 1);
            switch (pen.DashStyle)
            {
                case DashStyle.Dash: _body.Append(" stroke-dasharray=\"").Append(F(3 * w)).Append(',').Append(F(w)).Append('"'); break;
                case DashStyle.Dot: _body.Append(" stroke-dasharray=\"").Append(F(w)).Append(',').Append(F(w)).Append('"'); break;
                case DashStyle.DashDot: _body.Append(" stroke-dasharray=\"").Append(F(3 * w)).Append(',').Append(F(w)).Append(',').Append(F(w)).Append(',').Append(F(w)).Append('"'); break;
                case DashStyle.DashDotDot: _body.Append(" stroke-dasharray=\"").Append(F(3 * w)).Append(',').Append(F(w)).Append(',').Append(F(w)).Append(',').Append(F(w)).Append(',').Append(F(w)).Append(',').Append(F(w)).Append('"'); break;
            }
            if (pen.EndCap == LineCap.ArrowAnchor) { _arrowUsed = true; _body.Append(" marker-end=\"url(#arrowEnd)\""); }
            if (pen.StartCap == LineCap.ArrowAnchor) { _arrowUsed = true; _body.Append(" marker-start=\"url(#arrowStart)\""); }
        }

        private void Fill(Brush brush)
        {
            if (brush is SolidBrush sb)
            {
                _body.Append(" fill=\"").Append(Col(sb.Color)).Append('"');
                if (sb.Color.A < 255) _body.Append(" fill-opacity=\"").Append(F(sb.Color.A / 255f)).Append('"');
            }
            else if (brush is HatchBrush hb)
            {
                string id = "h" + (int)hb.HatchStyle + "_" + hb.ForegroundColor.ToArgb().ToString("x8") + "_" + hb.BackgroundColor.ToArgb().ToString("x8");
                if (!_defs.ContainsKey(id)) _defs[id] = HatchPattern(id, hb);
                _body.Append(" fill=\"url(#").Append(id).Append(")\"");
            }
            else _body.Append(" fill=\"none\"");
        }

        private static string HatchPattern(string id, HatchBrush hb)
        {
            string fg = Col(hb.ForegroundColor), bg = hb.BackgroundColor.A == 0 ? "none" : Col(hb.BackgroundColor);
            string inner;
            switch (hb.HatchStyle)
            {
                case HatchStyle.Horizontal: case HatchStyle.LightHorizontal: case HatchStyle.DarkHorizontal: case HatchStyle.NarrowHorizontal:
                    inner = "<path d=\"M0,0.5 H8\" stroke=\"" + fg + "\" stroke-width=\"1\"/>"; break;
                case HatchStyle.Vertical: case HatchStyle.LightVertical: case HatchStyle.DarkVertical: case HatchStyle.NarrowVertical:
                    inner = "<path d=\"M0.5,0 V8\" stroke=\"" + fg + "\" stroke-width=\"1\"/>"; break;
                case HatchStyle.ForwardDiagonal: case HatchStyle.LightDownwardDiagonal: case HatchStyle.DarkDownwardDiagonal: case HatchStyle.WideDownwardDiagonal:
                    inner = "<path d=\"M0,0 L8,8\" stroke=\"" + fg + "\" stroke-width=\"1\"/>"; break;
                case HatchStyle.BackwardDiagonal: case HatchStyle.LightUpwardDiagonal: case HatchStyle.DarkUpwardDiagonal: case HatchStyle.WideUpwardDiagonal:
                    inner = "<path d=\"M8,0 L0,8\" stroke=\"" + fg + "\" stroke-width=\"1\"/>"; break;
                case HatchStyle.Cross: case HatchStyle.SmallGrid:
                    inner = "<path d=\"M0,0.5 H8 M0.5,0 V8\" stroke=\"" + fg + "\" stroke-width=\"1\"/>"; break;
                case HatchStyle.DiagonalCross:
                    inner = "<path d=\"M0,0 L8,8 M8,0 L0,8\" stroke=\"" + fg + "\" stroke-width=\"1\"/>"; break;
                default: // PercentNN and the rest: dotted fill
                    inner = "<rect x=\"0\" y=\"0\" width=\"1\" height=\"1\" fill=\"" + fg + "\"/><rect x=\"2\" y=\"2\" width=\"1\" height=\"1\" fill=\"" + fg + "\"/>"; break;
            }
            int size = hb.HatchStyle >= HatchStyle.Percent05 && hb.HatchStyle <= HatchStyle.Percent90 ? 4 : 8;
            return "<pattern id=\"" + id + "\" width=\"" + size + "\" height=\"" + size + "\" patternUnits=\"userSpaceOnUse\">"
                 + (bg == "none" ? "" : "<rect width=\"" + size + "\" height=\"" + size + "\" fill=\"" + bg + "\"/>") + inner + "</pattern>";
        }

        private static PointF[] ToF(Point[] p) { var r = new PointF[p.Length]; for (int i = 0; i < p.Length; i++) r[i] = new PointF(p[i].X, p[i].Y); return r; }

        private static string Pts(PointF[] pts)
        {
            var sb = new StringBuilder();
            foreach (var p in pts)
            {
                if (!Finite(p.X, p.Y)) continue;
                sb.Append(F(p.X)).Append(',').Append(F(p.Y)).Append(' ');
            }
            return sb.ToString().TrimEnd();
        }

        private static bool Finite(params float[] v) { foreach (var f in v) if (float.IsNaN(f) || float.IsInfinity(f) || Math.Abs(f) > 1e7f) return false; return true; }
        internal static string F(float v) => v.ToString("0.###", CultureInfo.InvariantCulture);
        internal static string Col(Color c) => c.A == 0 ? "none" : "#" + c.R.ToString("x2") + c.G.ToString("x2") + c.B.ToString("x2");
        private static string Esc(string s) => s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }
}
