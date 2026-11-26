namespace STEB2;

/// <summary>
/// The original was a small top-most "Пожалуйста, подождите" window showing
/// <see cref="Progress.status"/> while long calculations ran inside
/// <c>using (new frmPleaseWait()) { ... }</c>. In the web app the page shows a busy
/// indicator with the same status text instead.
/// </summary>
public class frmPleaseWait : System.Windows.Forms.Form
{
}
