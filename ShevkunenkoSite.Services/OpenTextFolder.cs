// Ignore Spelling: env

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ShevkunenkoSite.Services;

// Services/FolderOpener.cs
public interface IFolderOpener
{
    (bool Ok, string? Error) OpenTextFolder(string folderName);
}

public class FolderOpener(IWebHostEnvironment env, ILogger<FolderOpener> logger) : IFolderOpener
{
    private readonly IWebHostEnvironment _env = env;
    private readonly ILogger<FolderOpener> _logger = logger;

    public (bool Ok, string? Error) OpenTextFolder(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            return (false, "Имя папки не указано");

        if (!OperatingSystem.IsWindows())
            return (false, "Открытие проводника поддерживается только в Windows");

        var webRoot = _env.WebRootPath
            ?? Path.Combine(_env.ContentRootPath, "wwwroot");

        var baseFolder = Path.GetFullPath(Path.Combine(webRoot, "texts"));

        string full;
        try
        {
            full = Path.GetFullPath(Path.Combine(baseFolder, folderName));
        }
        catch (Exception ex)
        {
            return (false, $"Некорректное имя папки: {ex.Message}");
        }

        var baseWithSep = baseFolder.EndsWith(Path.DirectorySeparatorChar)
            ? baseFolder
            : baseFolder + Path.DirectorySeparatorChar;

        if (!full.StartsWith(baseWithSep, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Попытка выйти за пределы texts: {Name}", folderName);
            return (false, "Недопустимый путь");
        }

        if (!Directory.Exists(full))
            return (false, $"Папка не найдена: {folderName}");

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"\"{full}\"",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось открыть папку {Path}", full);
            return (false, $"Ошибка запуска: {ex.Message}");
        }

        _logger.LogInformation("Открыта папка: {Path}", full);
        return (true, null);
    }
}