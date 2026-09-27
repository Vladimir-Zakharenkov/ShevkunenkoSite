namespace ShevkunenkoSite.Controllers;

[Authorize]
public class FolderController(SiteDbContext db, IFolderOpener opener) : Controller
{
    private readonly SiteDbContext _db = db;
    private readonly IFolderOpener _opener = opener;

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Open(string folderType, Guid entityId)
    {
        string baseFolderName;
        string? folderName;

        switch (folderType?.ToLowerInvariant())
        {
            case "text":
                var text = await _db.TextFile
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TextInfoModelId == entityId);

                if (text == null)
                    return NotFound(new { error = "TextInfoModel не найден" });

                baseFolderName = "texts";
                folderName = text.FolderForText;
                break;

            case "icon":
                var iconType = await _db.IconTypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.IconTypeModelId == entityId);

                if (iconType == null)
                    return NotFound(new { error = "IconTypeModel не найден" });

                baseFolderName = "images/pageicons";   // ← было "icons"
                folderName = iconType.PathToIcon
                    .Trim('/', '\\')
                    .Replace('\\', '/');
                break;

            default:
                return BadRequest(new { error = $"Неизвестный тип папки: {folderType}" });
        }

        var (ok, error) = _opener.OpenFolder(baseFolderName, folderName!);

        return ok
            ? Ok(new { opened = folderName })
            : BadRequest(new { error });
    }
}