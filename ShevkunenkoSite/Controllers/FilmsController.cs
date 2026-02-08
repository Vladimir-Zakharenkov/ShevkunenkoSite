namespace ShevkunenkoSite.Controllers
{
    public class FilmsController : Controller
    {
        public IActionResult Index(string? filmFilter)
        {
            return View();
        }

        public IActionResult Film(string? filmCaption, string? videoHosting)
        {
            return View();
        }
    }
}
