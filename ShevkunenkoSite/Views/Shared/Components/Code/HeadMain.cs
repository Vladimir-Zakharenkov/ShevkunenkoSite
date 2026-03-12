namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class HeadMain(
    IPageInfoRepository pageInfoContext,
    IIconRepository iconTypeContext,
    IBooksAndArticlesRepository bookContext) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        HeadViewModel headModel = new();

        #region Определяем страницу по HttpContext

        headModel.PageInfo = await pageInfoContext.GetPageInfoByPathAsync(HttpContext);

        #endregion

        #region Список иконок в теге <head>

        if (await iconTypeContext.Icons
                .Where(icon => icon.IconTypeModelId == headModel.PageInfo.IconTypeModelId).AnyAsync())
        {
            headModel.IconsForHead = await iconTypeContext.Icons
                    .Where(icon => icon.IconTypeModelId == headModel.PageInfo.IconTypeModelId).ToListAsync();
        }

        #endregion

        #region Если запрос к книге или статье

        if (HttpContext.Request.QueryString.ToString().Contains("articleid", StringComparison.CurrentCultureIgnoreCase))
        {
            string? articleGuid = HttpContext.Request.Query["articleid"];

            if (!string.IsNullOrEmpty(articleGuid)
                    && Guid.TryParse(articleGuid, out Guid newGuid)
                    && await bookContext.BooksAndArticles.Where(book => book.BooksAndArticlesModelId == newGuid).AnyAsync())
            {
                headModel.BookOrArticle = await bookContext.BooksAndArticles.FirstAsync(book => book.BooksAndArticlesModelId == newGuid);
            }
        }

        #endregion

        return View(headModel);
    }
}