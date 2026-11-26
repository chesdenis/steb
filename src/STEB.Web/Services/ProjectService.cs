using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using STEB2.DataBase.Json;

namespace STEB.Web.Services;

/// <summary>
/// Opens/creates project files. The recovered engine keeps the current file in the
/// static <see cref="STEB2.DataBase.Tools.workFile"/> exactly as the desktop program
/// did, so one server instance works on one project at a time (single user).
/// </summary>
public sealed class ProjectService
{
    public const string Extension = ".xsteb.json";

    public ProjectService(IWebHostEnvironment env)
    {
        ProjectsFolder = Path.Combine(env.ContentRootPath, "Projects");
        TemplateFile = Path.Combine(env.ContentRootPath, "Templates", "_blank.xsteb.json");
        Directory.CreateDirectory(ProjectsFolder);
    }

    public string ProjectsFolder { get; }
    public string TemplateFile { get; }

    public event Action Changed;

    public string CurrentFile => string.IsNullOrEmpty(STEB2.DataBase.Tools.workFile) ? null : STEB2.DataBase.Tools.workFile;
    public string CurrentName => CurrentFile == null ? null : Path.GetFileName(CurrentFile);

    public IReadOnlyList<string> ListProjects() =>
        Directory.GetFiles(ProjectsFolder, "*" + Extension).Select(Path.GetFileName).OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase).ToList();

    /// <summary>Same steps as Program.Main of STEB_LOAD: set the work file, then upgrade it.</summary>
    public void Open(string fileName)
    {
        string path = Path.Combine(ProjectsFolder, Path.GetFileName(fileName));
        if (!File.Exists(path)) throw new FileNotFoundException("Файл проекта не найден", path);
        STEB2.DataBase.Tools.workFile = path;
        STEB2.FileVersionChecker.FileVersionChecker.CheckAndUpdateFile();
        Changed?.Invoke();
    }

    /// <summary>"Файл → Создать...": a new project from the blank template.</summary>
    public string Create(string name)
    {
        string file = SafeName(name) + Extension;
        string path = Path.Combine(ProjectsFolder, file);
        if (File.Exists(path)) throw new IOException("Проект с таким именем уже существует: " + file);
        File.Copy(TemplateFile, path);
        JsonDatabase.Forget(path);
        Open(file);
        return file;
    }

    /// <summary>
    /// "Файл → Создать на основе прототипа...": the original copied a chosen project file
    /// (the prototype) to a new file and opened it.
    /// </summary>
    public string CreateFromPrototype(string prototypeFile, string name)
    {
        string src = Path.Combine(ProjectsFolder, Path.GetFileName(prototypeFile ?? ""));
        if (!File.Exists(src)) throw new FileNotFoundException("Выберите прототип.", src);
        string file = SafeName(name) + Extension;
        string path = Path.Combine(ProjectsFolder, file);
        if (File.Exists(path)) throw new IOException("Проект с таким именем уже существует: " + file);
        File.Copy(src, path);
        JsonDatabase.Forget(path);
        Open(file);
        return file;
    }

    /// <summary>"Файл → Сохранить как...": copy the current project under a new name and continue in the copy.</summary>
    public string SaveAs(string name)
    {
        if (CurrentFile == null) throw new InvalidOperationException("Проект не открыт.");
        string file = SafeName(name) + Extension;
        string path = Path.Combine(ProjectsFolder, file);
        if (File.Exists(path)) throw new IOException("Проект с таким именем уже существует: " + file);
        File.Copy(CurrentFile, path);
        JsonDatabase.Forget(path);
        Open(file);
        return file;
    }

    public byte[] ReadCurrent() => File.ReadAllBytes(CurrentFile);

    private static string SafeName(string name)
    {
        name = (name ?? "").Trim();
        foreach (char c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
        if (name.Length == 0) throw new ArgumentException("Укажите имя проекта.");
        return name;
    }
}
