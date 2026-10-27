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

    public async Task<IViewComponentResult> InvokeAsync(string? imageId, string? cssClass, string? iconType)
    {
        ImageFileModel? image = null;

        #region Поиск картинки по GUID

        if (Guid.TryParse(imageId, out var guid))
        {
            image = await imageFileContext.ImageFiles
                .FirstOrDefaultAsync(img => img.ImageFileModelId == guid);
        }

        #endregion

        #region Если по GUID не нашли, ищем по названию файла

        image ??= await imageFileContext.ImageFiles
            .FirstOrDefaultAsync(img => img.WebImageFileName == imageId
                                     || img.ImageFileName == imageId 
                                     || img.WebImageFileNameAI == imageId);

        #endregion

        #region Если ничего не найдено, выводим картинку NoImage

        image ??= await imageFileContext.ImageFiles
            .FirstOrDefaultAsync(img => img.ImageFileModelId == Guid.Parse(DataConfig.NoImage));

        #endregion

        #region Если не нашли NoImage

        if (image is null)
        {
            var noImageGuid = Guid.Parse(DataConfig.NoImage);

            image = await imageFileContext.ImageFiles
                .FirstOrDefaultAsync(img => img.ImageFileModelId == noImageGuid)
                ?? throw new InvalidOperationException(
                    $"Не найдено изображение NoImage с Id = {DataConfig.NoImage}");
        }

        #endregion

        var vm = new ViewImageViewModel
        {
            ImageItem = image,
            IconType = iconType is not null && AllowedIconTypes.Contains(iconType) 
                        ? iconType 
                        : "webimage",
            CssClass = string.IsNullOrEmpty(cssClass)
                        ? "img-fluid w-100"
                        : $"img-fluid w-100 {cssClass}"
        };

        return View(vm);
    }
}