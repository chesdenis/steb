using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Headless base for the original controls. Keeps the state the recovered
    /// code reads and writes (Text, Enabled, Visible, Cursor, Font...) so the
    /// event handlers stay unchanged; Razor components render this state.
    /// </summary>
    public class Control : IDisposable
    {
        private string _text = "";
        public string Name { get; set; } = "";
        public virtual string Text
        {
            get => _text;
            set { value ??= ""; if (value == _text) return; _text = value; OnTextChanged(EventArgs.Empty); }
        }
        public bool Enabled { get; set; } = true;
        public bool Visible { get; set; } = true;
        public object Tag { get; set; }
        public Font Font { get; set; } = Font.DefaultControlFont;
        public Color BackColor { get; set; } = Color.Transparent;
        public Color ForeColor { get; set; } = Color.Black;
        public Cursor Cursor { get; set; } = Cursors.Default;
        // layout state (kept so the original code compiles; the Razor views do the layout)
        public DockStyle Dock { get; set; }
        public AnchorStyles Anchor { get; set; }
        public Padding Padding { get; set; }
        public Padding Margin { get; set; }
        public Size Size { get; set; } = new Size(800, 600);
        public int Width { get => Size.Width; set => Size = new Size(value, Size.Height); }
        public int Height { get => Size.Height; set => Size = new Size(Size.Width, value); }
        public Point Location { get; set; }
        public bool AutoScroll { get; set; }
        public int TabIndex { get; set; }
        public Control Parent { get; internal set; }
        public ControlCollection Controls => _controls ??= new ControlCollection(this);
        private ControlCollection _controls;
        public event EventHandler TextChanged;
        public event EventHandler Click;
        public event EventHandler LostFocus;
        public event PreviewKeyDownEventHandler PreviewKeyDown;
        public event KeyEventHandler KeyDown;
        /// <summary>The view calls this for keys the original handled in KeyDown (Enter).</summary>
        public void RaiseKeyDown(Keys key) => KeyDown?.Invoke(this, new KeyEventArgs(key));
        /// <summary>The view calls this when the user leaves the field (blur/change).</summary>
        public void RaiseLostFocus() => LostFocus?.Invoke(this, EventArgs.Empty);
        /// <summary>The view calls this for a key press the original handled in PreviewKeyDown.</summary>
        public void RaisePreviewKeyDown(Keys key) => PreviewKeyDown?.Invoke(this, new PreviewKeyDownEventArgs(key));
        /// <summary>Raised when the control's state changed and the view should re-render.</summary>
        public event EventHandler StateChanged;
        protected virtual void OnTextChanged(EventArgs e) { TextChanged?.Invoke(this, e); NotifyStateChanged(); }
        public void PerformClick() => Click?.Invoke(this, EventArgs.Empty);
        public void NotifyStateChanged() => StateChanged?.Invoke(this, EventArgs.Empty);
        public virtual void Invalidate() => NotifyStateChanged();
        public void Refresh() => NotifyStateChanged();
        public void Update() { }
        public void Focus() { }
        public void BringToFront() { }
        public void SendToBack() { }
        public void Show() => OnShow();
        public void Hide() => Visible = false;
        protected virtual void OnShow() => Visible = true;
        public void SuspendLayout() { }
        public void ResumeLayout() { }
        public void ResumeLayout(bool performLayout) { }
        public virtual void Dispose() { }
    }

    public class UserControl : Control { }
    public class Form : Control
    {
        private static readonly System.Threading.AsyncLocal<Action<Form>> _showHandler = new System.Threading.AsyncLocal<Action<Form>>();

        /// <summary>
        /// The web page that runs the original code sets this: Form.Show()/ShowDialog()
        /// hand the form to it, and it opens the form as a window.
        /// </summary>
        public static Action<Form> ShowHandler { get => _showHandler.Value; set => _showHandler.Value = value; }

        public DialogResult DialogResult { get; set; }
        public FormWindowState WindowState { get; set; }
        public FormStartPosition StartPosition { get; set; }
        public FormBorderStyle FormBorderStyle { get; set; }
        public Icon Icon { get; set; }
        public bool TopMost { get; set; }
        public bool MinimizeBox { get; set; } = true;
        public bool MaximizeBox { get; set; } = true;
        public bool KeyPreview { get; set; }
        public bool ShowInTaskbar { get; set; } = true;
        public Size ClientSize { get => Size; set => Size = value; }
        public bool IsOpen { get; private set; }

        protected override void OnShow()
        {
            Visible = true;
            if (IsOpen) return;
            IsOpen = true;
            ShowHandler?.Invoke(this);
        }

        /// <summary>
        /// Cannot block in a web host: the form is opened like Show() and this returns at once.
        /// Code that depended on values entered in the dialog was split at this call.
        /// </summary>
        public DialogResult ShowDialog() { Show(); return DialogResult.OK; }

        public void Close()
        {
            IsOpen = false;
            Visible = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler Closed;
    }
    public class Label : Control { }
    public class Button : Control { }
    public class TextBox : Control
    {
        public bool ReadOnly { get; set; }
        public bool Multiline { get; set; }
        public void Clear() => Text = "";
        public void AppendText(string text) => Text += text;
        public string[] Lines { get => Text.Replace("\r\n", "\n").Split('\n'); set => Text = string.Join("\r\n", value ?? new string[0]); }
    }
    public class RichTextBox : TextBox { }
    public class CheckBox : Control
    {
        private bool _checked;
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked == value) return;
                _checked = value;
                OnCheckedSet(value);
                CheckedChanged?.Invoke(this, EventArgs.Empty);
                NotifyStateChanged();
            }
        }
        protected virtual void OnCheckedSet(bool value) { }
        public event EventHandler CheckedChanged;
    }
    /// <summary>
    /// WinForms semantics: checking a radio button first unchecks the other radio buttons
    /// of the same parent (raising their CheckedChanged), then raises its own CheckedChanged.
    /// </summary>
    public class RadioButton : CheckBox
    {
        protected override void OnCheckedSet(bool value)
        {
            if (value && Parent != null)
                foreach (var sibling in Parent.Controls)
                    if (sibling is RadioButton rb && !ReferenceEquals(rb, this) && rb.Checked) rb.Checked = false;
        }
    }
    public enum ToolStripItemDisplayStyle { None, Text, Image, ImageAndText }
    public enum ToolStripLayoutStyle { StackWithOverflow, HorizontalStackWithOverflow, VerticalStackWithOverflow, Flow, Table }

    public class ToolStripItem : Control
    {
        public ToolStripItemDisplayStyle DisplayStyle { get; set; } = ToolStripItemDisplayStyle.ImageAndText;
        public Image Image { get; set; }
        public string ToolTipText { get; set; } = "";
    }
    public class ToolStripButton : ToolStripItem
    {
        public ToolStripButton() { }
        public ToolStripButton(string text) { Text = text; }
        public ToolStripButton(string text, Image image, EventHandler onClick) { Text = text; Image = image; if (onClick != null) Click += onClick; }
        public bool Checked { get; set; }
    }
    public class ToolStripMenuItem : ToolStripButton
    {
        public ToolStripMenuItem() { }
        public ToolStripMenuItem(string text) : base(text) { }
    }
    public class ToolStripLabel : ToolStripItem
    {
        public ToolStripLabel() { }
        public ToolStripLabel(string text) { Text = text; }
    }
    /// <summary>Headless toolbar text box: text plus a change event.</summary>
    public class ToolStripTextBox : ToolStripItem
    {
        public ToolStripTextBox() { }
        public ToolStripTextBox(string text) { Text = text; }
    }
    public class ToolStrip : Control
    {
        public ToolStrip() { }
        public ToolStrip(params ToolStripItem[] items) { Items.AddRange(items); }
        public System.Collections.Generic.List<ToolStripItem> Items { get; } = new System.Collections.Generic.List<ToolStripItem>();
        public ToolStripLayoutStyle LayoutStyle { get; set; }
    }
    public class MenuStrip : ToolStrip { }

    public class Panel : Control { }
    public class GroupBox : Control { }
    public class SplitterPanel : Panel { }
    public class SplitContainer : Control
    {
        public SplitterPanel Panel1 { get; } = new SplitterPanel();
        public SplitterPanel Panel2 { get; } = new SplitterPanel();
        public Orientation Orientation { get; set; } = Orientation.Vertical;
        public int SplitterDistance { get; set; }
    }
    public enum Orientation { Horizontal, Vertical }

    /// <summary>Headless web browser: remembers the address it was sent to; the Razor view shows that document.</summary>
    public class WebBrowser : Control
    {
        public string Url { get; private set; }
        public string DocumentText { get; set; }
        public void Navigate(string urlString) => Navigate(urlString, false);
        public void Navigate(string urlString, bool newWindow) { Url = urlString; DocumentText = null; NotifyStateChanged(); }
        public new void Refresh() => NotifyStateChanged();
        /// <summary>Printing is done by the browser (the view's "Печать" button).</summary>
        public void Print() => PrintRequested?.Invoke(this, EventArgs.Empty);
        public event EventHandler PrintRequested;
    }

    public class ComboBox : Control
    {
        private int _selectedIndex = -1;
        public System.Collections.Generic.List<object> Items { get; } = new System.Collections.Generic.List<object>();
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex) return;
                _selectedIndex = value;
                if (value >= 0 && value < Items.Count) base.Text = Items[value]?.ToString() ?? "";
                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
                NotifyStateChanged();
            }
        }
        public object SelectedItem
        {
            get => _selectedIndex >= 0 && _selectedIndex < Items.Count ? Items[_selectedIndex] : null;
            set => SelectedIndex = Items.IndexOf(value);
        }
        public event EventHandler SelectedIndexChanged;
    }

    public sealed class Cursor
    {
        private Cursor(string css) { Css = css; }
        public string Css { get; }
        internal static Cursor Make(string css) => new Cursor(css);
    }
    public static class Cursors
    {
        public static Cursor Default { get; } = Cursor.Make("default");
        public static Cursor WaitCursor { get; } = Cursor.Make("wait");
        public static Cursor Hand { get; } = Cursor.Make("pointer");
        public static Cursor Cross { get; } = Cursor.Make("crosshair");
        public static Cursor IBeam { get; } = Cursor.Make("text");
        public static Cursor SizeAll { get; } = Cursor.Make("move");
    }

    public enum DockStyle { None, Top, Bottom, Left, Right, Fill }
    [Flags] public enum AnchorStyles { None = 0, Top = 1, Bottom = 2, Left = 4, Right = 8 }
    public enum FormWindowState { Normal, Minimized, Maximized }
    public enum FormStartPosition { Manual, CenterScreen, WindowsDefaultLocation, WindowsDefaultBounds, CenterParent }
    public enum FormBorderStyle { None, FixedSingle, Fixed3D, FixedDialog, Sizable, FixedToolWindow, SizableToolWindow }

    public struct Padding
    {
        public Padding(int all) { Left = Top = Right = Bottom = all; }
        public Padding(int left, int top, int right, int bottom) { Left = left; Top = top; Right = right; Bottom = bottom; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int All { get => Left; set => Left = Top = Right = Bottom = value; }
    }

    public class ControlCollection : System.Collections.Generic.List<Control>
    {
        private readonly Control _owner;
        public ControlCollection(Control owner) { _owner = owner; }
        public new void Add(Control c) { if (c == null) return; c.Parent = _owner; base.Add(c); }
        public void AddRange(Control[] controls) { foreach (var c in controls) Add(c); }
    }

    [Flags]
    public enum MouseButtons { None = 0, Left = 0x100000, Right = 0x200000, Middle = 0x400000, XButton1 = 0x800000, XButton2 = 0x1000000 }

    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        { Button = button; Clicks = clicks; X = x; Y = y; Delta = delta; }
        public MouseButtons Button { get; }
        public int Clicks { get; }
        public int X { get; }
        public int Y { get; }
        public int Delta { get; }
        public Point Location => new Point(X, Y);
    }

    public class PaintEventArgs : EventArgs
    {
        public PaintEventArgs(Graphics graphics, Rectangle clipRect) { Graphics = graphics; ClipRectangle = clipRect; }
        public Graphics Graphics { get; }
        public Rectangle ClipRectangle { get; }
    }
}

namespace System.Windows.Forms
{
    public class KeyPressEventArgs : EventArgs
    {
        public KeyPressEventArgs(char keyChar) { KeyChar = keyChar; }
        public char KeyChar { get; set; }
        public bool Handled { get; set; }
    }
}
