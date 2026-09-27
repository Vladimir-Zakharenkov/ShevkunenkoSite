// Ignore Spelling: env

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ShevkunenkoSite.Services;

public interface IFolderOpener
{
    // baseFolderName — папка внутри wwwroot (например, "texts" или "icons")
    (bool Ok, string? Error) OpenFolder(string baseFolderName, string folderName);
}

public class FolderOpener(IWebHostEnvironment env, ILogger<FolderOpener> logger) : IFolderOpener
{
    private readonly IWebHostEnvironment _env = env;
    private readonly ILogger<FolderOpener> _logger = logger;

    public (bool Ok, string? Error) OpenFolder(string baseFolderName, string folderName)
    {
        if (string.IsNullOrWhiteSpace(baseFolderName))
            return (false, "Базовая папка не указана");

        if (string.IsNullOrWhiteSpace(folderName))
            return (false, "Имя папки не указано");

        if (!OperatingSystem.IsWindows())
            return (false, "Открытие проводника поддерживается только в Windows");

        var webRoot = _env.WebRootPath
            ?? Path.Combine(_env.ContentRootPath, "wwwroot");

        var baseFolder = Path.GetFullPath(Path.Combine(webRoot, baseFolderName));

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

        // Защита от выхода за пределы базовой папки (path traversal)
        if (!full.StartsWith(baseWithSep, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Попытка выйти за пределы {Base}: {Name}", baseFolderName, folderName);
            return (false, "Недопустимый путь");
        }

        if (!Directory.Exists(full))
            return (false, $"Папка не найдена: {full}");

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