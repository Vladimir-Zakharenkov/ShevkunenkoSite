namespace ShevkunenkoSite.Controllers;

public class ManifestController(
    IPageInfoRepository pageContext,
    IIconRepository iconContext
    ) : Controller
{
    async public void Manifest(Guid? pageId)
    {
        PageInfoModel pageInfo = new();

        //PageInfoModel pageInfo = await pageContext.GetPageInfoByPathAsync(HttpContext);

        ManifestModel manifest = new();

        if (pageContext.PagesInfo.Where(page => page.PageInfoModelId == pageId).Any())
        {
            pageInfo = pageContext.PagesInfo.First(page => page.PageInfoModelId == pageId);
        }

        manifest.Name = pageInfo.PageTitle;

        manifest.Description = pageInfo.PageDescription.StartOfDescription();

        if (HttpContext.Request.IsHttps)
        {
            manifest.Start_url = "https://" + HttpContext.Request.Host.ToString() + pageInfo.PageFullPathWithData;
        }
        else
        {
            manifest.Start_url = "http://" + HttpContext.Request.Host.ToString() + pageInfo.PageFullPathWithData;
        }

        manifest.Id = manifest.Start_url;

        manifest.Icons = iconContext.Icons
                .Where(icon => icon.PathToIcon == pageInfo.PageIconPath && icon.IconMimeType != "image/svg+xml")
                .Select(p => new IconForManifest
                {
                    src = DataConfig.IconsFolder + p.PathToIcon + p.IconFileName,
                    sizes = p.IconSize,
                    type = p.IconMimeType,
                    purpose = p.IconPurpose
                });

        await HttpContext.Response.WriteAsJsonAsync(manifest);
    }
}
