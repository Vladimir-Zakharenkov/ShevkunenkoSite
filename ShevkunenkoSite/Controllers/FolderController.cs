namespace ShevkunenkoSite.Controllers;

[Authorize]
public class FolderController(SiteDbContext db, IFolderOpener opener) : Controller
{
    private readonly SiteDbContext _db = db;
    private readonly IFolderOpener _opener = opener;

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OpenText(Guid textInfoId)
    {
        var text = await _db.TextFile
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TextInfoModelId == textInfoId);

        if (text == null)
            return NotFound(new { error = "TextInfoModel не найден" });

        var (ok, error) = _opener.OpenTextFolder(text.FolderForText);

        return ok
            ? Ok(new { opened = text.FolderForText })
            : BadRequest(new { error });
    }
}