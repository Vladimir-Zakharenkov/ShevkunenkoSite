using Azure;

namespace ShevkunenkoSite.Controllers;

// Тестовый контроллер
public class TestController(IPageInfoRepository pageContext) : Controller
{
    public async void Test()
    {
        var pageInfo = pageContext.GetPageInfoByPathAsync(HttpContext);

        await HttpContext.Response.WriteAsJsonAsync(pageInfo);

        //await HttpContext.Response.WriteAsJsonAsync("{\"name\":\"Tom\",\"start_url\":https://shevkunenko.site/}");
    }
}
