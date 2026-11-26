using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace System.Drawing
{
    /// <summary>Image backed by the SVG recorded through <see cref="Graphics.FromImage"/>.</summary>
    public abstract class Image : IDisposable
    {
        private Graphics _graphics;
        protected Image(int width, int height) { Width = width; Height = height; }
        public int Width { get; }
        public int Height { get; }
        public Size Size => new Size(Width, Height);
        internal void Attach(Graphics g) { _graphics = g; }
        internal void Detach(Graphics g) { }

        /// <summary>The image content as an SVG document.</summary>
        public string ToSvg() => _graphics?.ToSvg(Width, Height) ?? new Graphics(Width, Height).ToSvg();

        /// <summary>
        /// Saves the picture. Only SVG output is available without GDI+, so the
        /// file is always written as SVG; a .bmp/.png extension is replaced with .svg.
        /// Returns the path actually written.
        /// </summary>
        public string Save(string filename, ImageFormat format)
        {
            string path = Path.ChangeExtension(filename, ".svg");
            File.WriteAllText(path, ToSvg(), new UTF8Encoding(false));
            return path;
        }
        public string Save(string filename) => Save(filename, ImageFormat.Bmp);
        public void Save(Stream stream, ImageFormat format)
        {
            byte[] b = new UTF8Encoding(false).GetBytes(ToSvg());
            stream.Write(b, 0, b.Length);
        }
        public virtual void Dispose() { }
    }

    public sealed class Bitmap : Image
    {
        public Bitmap(int width, int height) : base(width, height) { }
    }
}

namespace System.Drawing.Imaging
{
    public sealed class ImageFormat
    {
        private ImageFormat(string name) { Name = name; }
        public string Name { get; }
        public static ImageFormat Bmp { get; } = new ImageFormat("Bmp");
        public static ImageFormat Png { get; } = new ImageFormat("Png");
        public static ImageFormat Jpeg { get; } = new ImageFormat("Jpeg");
        public static ImageFormat Gif { get; } = new ImageFormat("Gif");
        public static ImageFormat Emf { get; } = new ImageFormat("Emf");
        public override string ToString() => Name;
    }
}
