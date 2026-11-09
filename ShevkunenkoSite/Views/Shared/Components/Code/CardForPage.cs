// Ignore Spelling: Loc

namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class CardForPage(IPageInfoRepository pageInfoContext) : ViewComponent
{
    private Guid pageIdGuid;

    public async Task<IViewComponentResult> InvokeAsync(string? pageLoc)
    {
        PageInfoModel page;

        PageInfoModel errorPage = await pageInfoContext.PagesInfoNoTracking.AsNoTracking().FirstAsync(p => p.PageInfoModelId == Guid.Parse("A195DA69-34CC-4215-9DBA-38D949685EEE"));

        if (!string.IsNullOrEmpty(pageLoc))
        {
            if (Guid.TryParse(pageLoc, out pageIdGuid))
            {
                if (await pageInfoContext.PagesInfoNoTracking.Where(g => g.PageInfoModelId == pageIdGuid).AnyAsync())
                {
                    page = await pageInfoContext.PagesInfoNoTracking.AsNoTracking().FirstAsync(p => p.PageInfoModelId == pageIdGuid);

                    return View(page);
                }
                else
                {
                    return View(errorPage);
                }
            }
            else
            {
                if (await pageInfoContext.PagesInfoNoTracking.Where(p => p.PageFullPathWithData == pageLoc).AnyAsync())
                {
                    page = await pageInfoContext.PagesInfoNoTracking.FirstAsync(p => p.PageFullPathWithData == pageLoc);

                    return View(page);
                }
                else if (await pageInfoContext.PagesInfoNoTracking.Where(p => p.PagePathNickNameWithData == pageLoc).AnyAsync())
                {
                    page = await pageInfoContext.PagesInfoNoTracking.FirstAsync(p => p.PagePathNickNameWithData == pageLoc);

                    return View(page);
                }
                else
                {
                    return View(errorPage);
                }
            }
        }
        else
        {
            return View(errorPage);
        }
    }
}