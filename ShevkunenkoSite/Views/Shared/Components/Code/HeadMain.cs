namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class HeadMain(
    IPageInfoRepository pageInfoContext,
    IIconFileRepository iconFileContext,
    IBooksAndArticlesRepository bookContext) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        PageInfoModel pageInfoModel = await pageInfoContext.GetPageInfoByPathAsync(HttpContext);

        BooksAndArticlesModel? bookOrArticle = null;

        #region Список иконок в теге <head>

        List<IconFileModel> iconList = await iconFileContext.IconFiles
            .Where(icon => icon.IconPath == pageInfoModel.PageIconPath)
            .AsNoTracking()
            .ToListAsync();

        if (iconList.Count == 0)
        {
            iconList = await iconFileContext.IconFiles
                .Where(icon => icon.IconPath == "main/")
                .AsNoTracking()
                .ToListAsync();
        }

        #endregion

        if (HttpContext.Request.QueryString.ToString().Contains("articleid", StringComparison.CurrentCultureIgnoreCase))
        {
            string? articleGuid = HttpContext.Request.Query["articleid"];

            if (!string.IsNullOrEmpty(articleGuid)
                    && Guid.TryParse(articleGuid, out Guid newGuid)
                    && await bookContext.BooksAndArticles.Where(book => book.BooksAndArticlesModelId == newGuid).AnyAsync())
            {
                bookOrArticle = await bookContext.BooksAndArticles.FirstAsync(book => book.BooksAndArticlesModelId == newGuid);
            }
        }

        return View(new HeadViewModel
        {
            PageInfo = pageInfoModel,
            IconList = iconList,
            BookOrArticle = bookOrArticle
        });
    }
}