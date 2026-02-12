namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class Manifest
    (
    IPageInfoRepository pageInfoContext
    ) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        PageInfoModel pageInfo = await pageInfoContext.GetPageInfoByPathAsync(HttpContext);

        return View(pageInfo);
    }
}