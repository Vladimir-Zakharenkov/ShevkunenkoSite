namespace ShevkunenkoSite.Areas.Movies.Pages;

public class Pestraya_lentaModel : PageModel
{
    private readonly IPageInfoRepository _pageInfoContext;

    public Pestraya_lentaModel(IPageInfoRepository pageInfoContext)
    {
        _pageInfoContext = pageInfoContext;
    }

    [BindProperty]
    public IEnumerable<PageInfoModel> PageLinks { get; set; } = Enumerable.Empty<PageInfoModel>();

    public async Task<IActionResult> OnGetAsync()
    {
        PageLinks =  await _pageInfoContext.PagesInfoNoTracking.Where(p => p.PageFilter.Contains("Пёстрая лента,")).ToArrayAsync();

        return Page();
    }
}