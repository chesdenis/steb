using System.Threading;

namespace System.Windows.Forms
{
    public enum DialogResult { None, OK, Cancel, Abort, Retry, Ignore, Yes, No }
    public enum MessageBoxButtons { OK, OKCancel, AbortRetryIgnore, YesNoCancel, YesNo, RetryCancel }
    public enum MessageBoxIcon { None = 0, Hand = 16, Stop = 16, Error = 16, Question = 32, Exclamation = 48, Warning = 48, Asterisk = 64, Information = 64 }
    public enum MessageBoxDefaultButton { Button1 = 0, Button2 = 256, Button3 = 512 }

    public static class Application
    {
        /// <summary>No message loop in a web host: kept as a no-op so the original calls still compile.</summary>
        public static void DoEvents() { }
        public static string StartupPath => AppContext.BaseDirectory.TrimEnd('\\', '/');
        /// <summary>No desktop windows exist in the web host; kept so loops over open forms still compile.</summary>
        public static System.Collections.Generic.List<Form> OpenForms { get; } = new System.Collections.Generic.List<Form>();
    }

    public sealed class MessageBoxRequest
    {
        public string Text { get; init; } = "";
        public string Caption { get; init; } = "";
        public MessageBoxButtons Buttons { get; init; }
        public MessageBoxIcon Icon { get; init; }
        public MessageBoxDefaultButton DefaultButton { get; init; }
    }

    /// <summary>
    /// Synchronous MessageBox replacement. A web UI cannot block for an answer,
    /// so the page answers questions in advance through <see cref="Handler"/>
    /// (per async flow); messages without an answer are recorded in <see cref="Messages"/>.
    /// With no handler the default button's result is returned.
    /// </summary>
    public static class MessageBox
    {
        private static readonly AsyncLocal<Func<MessageBoxRequest, DialogResult>> _handler = new AsyncLocal<Func<MessageBoxRequest, DialogResult>>();
        private static readonly AsyncLocal<Action<MessageBoxRequest>> _sink = new AsyncLocal<Action<MessageBoxRequest>>();

        public static Func<MessageBoxRequest, DialogResult> Handler { get => _handler.Value; set => _handler.Value = value; }
        public static Action<MessageBoxRequest> Messages { get => _sink.Value; set => _sink.Value = value; }

        public static DialogResult Show(string text) => Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        public static DialogResult Show(string text, string caption) => Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons) => Show(text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) => Show(text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            var req = new MessageBoxRequest { Text = text ?? "", Caption = caption ?? "", Buttons = buttons, Icon = icon, DefaultButton = defaultButton };
            var h = Handler;
            if (h != null) return h(req);
            Messages?.Invoke(req);
            return DefaultResult(buttons, defaultButton);
        }

        public static DialogResult DefaultResult(MessageBoxButtons buttons, MessageBoxDefaultButton def)
        {
            int i = def == MessageBoxDefaultButton.Button1 ? 0 : def == MessageBoxDefaultButton.Button2 ? 1 : 2;
            DialogResult[] r = buttons switch
            {
                MessageBoxButtons.OK => new[] { DialogResult.OK },
                MessageBoxButtons.OKCancel => new[] { DialogResult.OK, DialogResult.Cancel },
                MessageBoxButtons.AbortRetryIgnore => new[] { DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore },
                MessageBoxButtons.YesNoCancel => new[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel },
                MessageBoxButtons.YesNo => new[] { DialogResult.Yes, DialogResult.No },
                _ => new[] { DialogResult.Retry, DialogResult.Cancel },
            };
            return r[Math.Min(i, r.Length - 1)];
        }
    }
}
