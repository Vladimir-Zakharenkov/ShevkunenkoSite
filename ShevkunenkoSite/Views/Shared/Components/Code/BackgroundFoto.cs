namespace ShevkunenkoSite.Views.Shared.Components.Code;

[ViewComponent]
public class BackgroundFoto(
    IPageInfoRepository pageInfoContext,
    IMovieFileRepository movieFileContext) : ViewComponent
{
    private Guid pageIdGuid;
    private Guid movieIdGuid;

    public async Task<IViewComponentResult> InvokeAsync(int background)
    {
        PageInfoModel pageInfoModel = await pageInfoContext.GetPageInfoByPathAsync(HttpContext);

        if (HttpContext.Request.Path.ToString().Contains("details")
            || HttpContext.Request.Path.ToString().Contains("update")
            || HttpContext.Request.Path.ToString().Contains("editpage")
            || HttpContext.Request.Path.ToString().Contains("detailspage")
            || HttpContext.Request.Path.ToString().Contains("editmovie")
            || HttpContext.Request.Path.ToString().Contains("detailsmovie"))
        {
            if (Guid.TryParse(HttpContext.Request.Query["pageId"].ToString(), out pageIdGuid) 
                & await pageInfoContext.PagesInfo.Where(g => g.PageInfoModelId == pageIdGuid).AnyAsync())
            {
                pageInfoModel = await pageInfoContext.PagesInfo.AsNoTracking().FirstAsync(p => p.PageInfoModelId == pageIdGuid);
            }

            if (Guid.TryParse(HttpContext.Request.Query["movieId"].ToString(), out movieIdGuid) 
                & await movieFileContext.MovieFiles.Where(m => m.MovieFileModelId == movieIdGuid).AnyAsync())
            {
                var movie = await movieFileContext.MovieFiles.AsNoTracking().FirstAsync(m => m.MovieFileModelId == movieIdGuid);

                if (movie.PageInfoModelId != null)
                {
                    pageInfoModel = await pageInfoContext.PagesInfo.AsNoTracking().FirstAsync(p => p.PageInfoModelId == movie.PageInfoModelId);
                }
            }
        }

        if (pageInfoModel.BackgroundFileModel != null)
        {
            if (!string.IsNullOrEmpty(pageInfoModel.BackgroundFileModel.WebLeftBackground) & !string.IsNullOrEmpty(pageInfoModel.BackgroundFileModel.WebRightBackground))
            {
                return background % 2 == 0
                    ? View("BackgroundFoto", pageInfoModel.BackgroundFileModel.WebLeftBackground)
                    : View("BackgroundFoto", pageInfoModel.BackgroundFileModel.WebRightBackground);
            }
            else
            {
                return background % 2 == 0
                   ? View("BackgroundFoto", pageInfoModel.BackgroundFileModel.LeftBackground)
                    : View("BackgroundFoto", pageInfoModel.BackgroundFileModel.RightBackground);
            }
        }
        else
        {
            return View("BackgroundFoto", string.Empty);
        }
    }
}