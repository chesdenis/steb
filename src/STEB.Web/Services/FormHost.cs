using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STEB.Web.Services;

/// <summary>
/// Runs the original (synchronous) WinForms-era code for one page:
///  * off the UI thread, exposing the engine's progress text (STEB2.Progress.status);
///  * collecting every Form the code opens with Show()/ShowDialog(), which the page
///    then displays as windows on top of the control panel.
/// </summary>
public sealed class FormHost : IDisposable
{
    private readonly object _lock = new object();
    private Timer _timer;

    public List<Form> OpenForms { get; } = new List<Form>();
    public bool IsBusy { get; private set; }
    public string Status { get; private set; } = "";

    /// <summary>Raised (from any thread) when windows, busy state or status change.</summary>
    public event Action Changed;

    public async Task Run(Action work)
    {
        IsBusy = true;
        Status = "";
        STEB2.Progress.status = "";
        Changed?.Invoke();
        _timer = new Timer(_ => { Status = STEB2.Progress.status; Changed?.Invoke(); }, null, 300, 300);
        // AsyncLocal: flows into Task.Run below, so forms opened by the work come to this host.
        Form.ShowHandler = f => { lock (_lock) { if (!OpenForms.Contains(f)) OpenForms.Add(f); } };
        try
        {
            await Task.Run(work);
        }
        finally
        {
            Form.ShowHandler = null;
            _timer?.Dispose();
            _timer = null;
            IsBusy = false;
            Changed?.Invoke();
        }
    }

    public void Close(Form form)
    {
        lock (_lock) OpenForms.Remove(form);
        try { form.Close(); } catch { }
        Changed?.Invoke();
    }

    public void Dispose()
    {
        _timer?.Dispose();
        lock (_lock)
        {
            foreach (var f in OpenForms) { try { f.Dispose(); } catch { } }
            OpenForms.Clear();
        }
    }
}
