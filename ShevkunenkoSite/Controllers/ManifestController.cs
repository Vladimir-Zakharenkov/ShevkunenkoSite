namespace ShevkunenkoSite.Controllers;

public class ManifestController(IPageInfoRepository pageContext) : Controller
{
    async public void Manifest(Guid? pageId)
    {
        PageInfoModel pageInfo = new();

        ManifestModel manifest = new();

        if (pageContext.PagesInfo.Where(page => page.PageInfoModelId == pageId).Any())
        {
            pageInfo = pageContext.PagesInfo.First(page => page.PageInfoModelId == pageId);
        }

        manifest.Name = pageInfo.PageTitle;

        manifest.Description = pageInfo.PageDescription;

        if (HttpContext.Request.IsHttps)
        {
            manifest.Start_url = "https://" + HttpContext.Request.Host.ToString() + pageInfo.PageFullPathWithData;
        }
        else
        {
            manifest.Start_url = "http://" + HttpContext.Request.Host.ToString() + pageInfo.PageFullPathWithData;
        }

        manifest.Id = manifest.Start_url;

        await HttpContext.Response.WriteAsJsonAsync(manifest);
    }
}
