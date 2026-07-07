namespace ShevkunenkoSite.Controllers;

public class Shevkunenko(
    IPageInfoRepository pageContext
        ) : Controller
{
    public IActionResult Index() => View();

    public async Task<IActionResult> Biography()
    {
        var pageInfoModel = await pageContext.GetPageInfoByPathAsync(HttpContext);

        return View(pageInfoModel);
    }

    public IActionResult Press() => Redirect("https://shevkunenko.ru/pressa/index.htm");

    public IActionResult Error404() => View();
}