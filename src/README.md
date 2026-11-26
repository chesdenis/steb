# STEB — recovered source (2010 → .NET 10 / Blazor Server)

Recovered from the original binaries `STEB_LOAD.exe` and `STEB_LOAD_DLL.dll`
(build of 8–10 May 2010, .NET 2.0, Windows Forms) with ILSpy.

**Rule of the port:** calculation and data logic stay exactly as decompiled.
Only parts tied to the old framework were replaced.

## Layout

| Project | What it is |
|---|---|
| `STEB.Core` | The engine from STEB_LOAD_DLL (namespaces `STEB2.*`): geometry, stations, volumes and waterlines, stability, masses, tanks, compartments, containers, drawing primitives. |
| `STEB.Compat` | A small cross-platform re-implementation of the parts of GDI+ (`System.Drawing`) and WinForms the original code uses. Same namespaces, so the old code compiles unchanged. Drawing is rendered to SVG. |
| `STEB.Web` | ASP.NET Core Blazor Server app. `Controls/` holds the original WinForms controls as headless models (their code kept as decompiled); `Components/` holds the Razor views that draw them. |

## What was replaced (framework only)

| Original (2010) | Now |
|---|---|
| Access `.xsteb` via `System.Data.OleDb` (Jet 4) | JSON project file `.xsteb.json`, read and written by `STEB2.DataBase.Json` (`JsonDbConnection`, `JsonDbCommand`, `JsonDbDataReader`). It understands the same SQL statements the program sends, so no query was changed. |
| GDI+ painting in `usgModelFrame` | Same paint code, drawn onto an SVG `Graphics` (`STEB.Compat`). Zoom (wheel) and pan (drag) work as before. |
| "Export to AutoCAD" (AutoCAD COM automation) | Removed (not supported) |
| "Save image" (`.bmp`) | Downloadable `.svg` |
| Explorer context-menu hooks (`ShellContextMenu`, `LocalWindowsHook`) | Removed (not possible in a browser) |
| `MessageBox` questions | `STEB.Compat` `MessageBox` with a per-request answer handler. With no handler the default button's answer is used. |
| Russian-locale Windows | App culture fixed to `ru-RU` |

## What is ported

All menus of the 2010 control panel (STEB_LOAD.exe):

* **File**: open, create, create from a prototype (copy of an existing project), save as, download.
* **🌐 Language**: English, Русский, Español, Français, Türkçe (global setting, see below).
* **Input data**: hull form (paste or load a `.psteb` prototype), transverse and longitudinal sections, aquatory, lightship mass and CG, compartments, tanks, containers, 3D hull view (new in the port).
* **Calculations**: floating position on the control panel; arbitrary floating position (16 methods).
* **Diagrams and charts**: deadweight scale, Xc/Xf, r/Zc, form coefficients, stability diagrams, cross curves (pantokarens), circular stability diagram.
* **Reports and documents**: hydrostatics table, floating-position report (HTML, printable, downloadable).
* **Help → About the author** (new in the port). The original "About" box and the unconnected "User guide" item are not shown.

`frmIcingCalc` exists in the binary but was never opened by the program, so it is not ported.

### How the old UI code is hosted

* Each decompiled Form/UserControl was turned into a headless model (`Controls/`) by a conversion script:
  layout-only controls are dropped, `InitializeComponent` keeps only control creation and texts, and handlers stay as decompiled.
* `Form.Show()`/`ShowDialog()` hand the form to the page (`FormHost`), which opens it as a window over the control panel.
  Where the original code read values from a modal dialog *after* `ShowDialog()` returned (adding an aquatory, tank editing),
  the handler is split at that point and the rest runs when the web dialog is closed.
* Yes/No questions that need an answer before the code continues ("Delete?", "Apply the tank changes?") become browser confirmations.
  Others ("Recalculate the floating position?", "Redraw?", "Overlay the diagrams?") take the original default button's answer.
* Web additions that do not touch calculations: drawings are fitted to their window after each calculation,
  and long calculations show a progress box with the engine's own status text.

## Build, run, test

See the [main README](../README.md#build-run-and-test). License: [MIT](../LICENSE); educational use only — see [DISCLAIMER](../DISCLAIMER.md).

## Interface language

`Services/Localization.cs` (`L.T`) translates the program's texts only where they are displayed:
Razor markup, window titles, grid/list headers, report HTML, the log, and text drawn by the original
painting code (through the `Graphics.TextTransform` hook in `STEB.Compat`). Keys in `Resources/i18n.json`
are the original Russian strings, so the decompiled code is unchanged. Whole texts are looked up first;
texts the original code builds by concatenation are translated phrase by phrase. The chosen language is
stored in `settings.json` next to the app.

## Known framework differences to watch

* Modern .NET formats `float`/`double` with `ToString()` as shortest round-trip
  (e.g. `3.3000002`), where .NET 2.0 used 7/15 significant digits (`3.3`).
  The engine round-trips some numbers through text, so the last digits may
  differ.
* The engine keeps the open project in a static field (`STEB2.DataBase.Tools.workFile`), as the desktop app did.
  One running server = one user/project. Each user runs their own copy.
* Text size in drawings is approximated (no GDI+ font metrics).
