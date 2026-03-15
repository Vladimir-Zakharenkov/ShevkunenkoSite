using System.Text;

namespace ShevkunenkoSite.Pages;

public class BrowserconfigModel(IPageInfoRepository pageinfoContext) : PageModel
{
    public readonly IPageInfoRepository pageContext = pageinfoContext;

    public async Task<IActionResult> OnGetAsync()
    {
        PageInfoModel pageInfo = await pageContext.GetPageInfoByPathAsync(HttpContext);

        StringBuilder sb = new();

        sb.Append($"<?xml version=\"1.0\" encoding=\"UTF-8\" ?><urlset><browserconfig><msapplication><tile><square70x70logo src=\"/images/pageicons/{pageInfo.IconType.PathToIcon}ms-tile-126.webp\"/><square150x150logo src=\"/images/pageicons/{pageInfo.IconType.PathToIcon}ms-tile-270.webp\"/><wide310x150logo src=\"/images/pageicons/{pageInfo.IconType.PathToIcon}ms-tile-558x270.webp\"/><square310x310logo src=\"/images/pageicons/{pageInfo.IconType.PathToIcon}ms-tile-558.webp\"/><TileColor>#ffffff</TileColor></tile></msapplication></browserconfig></urlset>");

        return new ContentResult
        {
            ContentType = "application/xml",
            Content = sb.ToString(),
            StatusCode = 200
        };
    }
}