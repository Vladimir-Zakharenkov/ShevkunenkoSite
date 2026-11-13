
namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class ViewImage(
    IImageFileRepository imageFileContext
    ) : ViewComponent
{
    private static readonly HashSet<string> AllowedIconTypes =
    [
        "webhd", "webimage", "webicon300", "webicon200", "webicon100",
        "webhdAI", "webimageAI", "webicon300AI", "webicon200AI", "webicon100AI",
        "hd", "image", "icon300", "icon200", "icon100"
    ];


    public async Task<IViewComponentResult> InvokeAsync(
        string? imageId = null,
        string? cssClass = null,
        string? iconType = null,
        ImageFileModel? imageItem = null)
    {
        ImageFileModel? image = imageItem;

        if (image is null)
        {
            var query = imageFileContext.ImageFiles
                .AsNoTracking();

            if (Guid.TryParse(imageId, out Guid imageGuid))
            {
                image = await query.FirstOrDefaultAsync(
                    img => img.ImageFileModelId == imageGuid);
            }
            else if (!string.IsNullOrWhiteSpace(imageId))
            {
                image = await query.FirstOrDefaultAsync(img =>
                    img.WebImageFileName == imageId
                    || img.ImageFileName == imageId
                    || img.WebImageFileNameAI == imageId);
            }

            image ??= await query.FirstOrDefaultAsync(
                img => img.ImageFileModelId == DataConfig.NoImage);
        }

        if (image is null)
        {
            throw new InvalidOperationException(
                $"Не найдено изображение NoImage с Id = {DataConfig.NoImage}");
        }

        ViewImageViewModel vm = new()
        {
            ImageItem = image,

            IconType = iconType is not null
                && AllowedIconTypes.Contains(iconType)
                    ? iconType
                    : "webimage",

            CssClass = string.IsNullOrWhiteSpace(cssClass)
                ? "img-fluid w-100"
                : $"img-fluid w-100 {cssClass}"
        };

        return View(vm);
    }
}