using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using STEB.Web.Components;
using STEB.Web.Services;

// The original program ran on Russian-locale Windows: keep the same number/text formatting.
// Windows-1251 for old text files (.psteb) written by the desktop programs.
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var ru = CultureInfo.GetCultureInfo("ru-RU");
CultureInfo.DefaultThreadCurrentCulture = ru;
CultureInfo.DefaultThreadCurrentUICulture = ru;

var builder = WebApplication.CreateBuilder(args);
// Display language (global setting, Resources/i18n.json + settings.json).
L.Init(builder.Environment.ContentRootPath);
System.Drawing.Graphics.TextTransform = L.T;
// Serve framework assets (_framework/blazor.web.js) from the build manifest in every
// environment, not only "Development": IDE run configurations often start the app
// without ASPNETCORE_ENVIRONMENT. For a published app this is a harmless no-op.
builder.WebHost.UseStaticWebAssets();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(o => o.DetailedErrors = builder.Environment.IsDevelopment())
    // Hull descriptions (.psteb text) are often 100+ KB; the SignalR default limit (32 KB)
    // silently drops the circuit when such text is pasted into a text box.
    .AddHubOptions(o => o.MaximumReceiveMessageSize = 16 * 1024 * 1024);
builder.Services.AddSingleton<ProjectService>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Download the currently open project file.
app.MapGet("/download/project", (ProjectService projects) =>
{
    if (projects.CurrentFile == null) return Results.NotFound();
    return Results.File(projects.ReadCurrent(), "application/json", projects.CurrentName);
});

app.Run();
