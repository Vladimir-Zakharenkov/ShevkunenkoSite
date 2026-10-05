namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class ViewImage(
    IImageFileRepository imageFileContext
    ) : ViewComponent
{
    readonly ViewImageViewModel viewImageViewModel = new();

    public async Task<IViewComponentResult> InvokeAsync(string? imageId, string? cssClass, string? iconType)
    {
        #region Поиск картинки по GUID

        if (Guid.TryParse(imageId, out Guid imageIdGuid)
            & await imageFileContext.ImageFiles
                .Where(img => img.ImageFileModelId == imageIdGuid)
                .AnyAsync())
        {
            viewImageViewModel.ImageItem = await imageFileContext.ImageFiles.FirstAsync(img => img.ImageFileModelId == imageIdGuid);
        }

        #endregion

        #region Поиск картинки по названию файла

        else if (await imageFileContext.ImageFiles.Where(img => img.WebImageFileName == imageId || img.ImageFileName == imageId).AnyAsync())
        {
            viewImageViewModel.ImageItem = await imageFileContext.ImageFiles.FirstAsync(img => img.WebImageFileName == imageId || img.ImageFileName == imageId);
        }

        #endregion

        #region Если ничего не найдено, выводим картинку NoImage

        else
        {
            viewImageViewModel.ImageItem = await imageFileContext.ImageFiles.FirstAsync(img => img.ImageFileModelId == Guid.Parse(DataConfig.NoImage));
        }

        #endregion

        #region Размер и тип файла картинки

        if (iconType == "webhd"
                || iconType == "webimage"
                || iconType == "webicon300"
                || iconType == "webicon200"
                || iconType == "webicon100"
                || iconType == "hd"
                || iconType == "image"
                || iconType == "icon300"
                || iconType == "icon200"
                || iconType == "icon100")
        {
            viewImageViewModel.IconType = iconType;
        }
        else
        {
            viewImageViewModel.IconType = "webimage";
        }

        #endregion

        #region CSS для картинки

        viewImageViewModel.CssClass = string.IsNullOrEmpty(cssClass)  ? "img-fluid w-100" : ($"img-fluid w-100 {cssClass}");

        #endregion

        return View(viewImageViewModel);
    }
}