namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class Carousel : ViewComponent
{
    private readonly IImageFileRepository _imageContext;
    public Carousel(IImageFileRepository imageContext)
    {
        _imageContext = imageContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(string searchFilter)
    {
        var pictures = await _imageContext.ImageFiles
            .Where(p => p.SearchFilter != null &&
                    p.SearchFilter.Contains(searchFilter ?? string.Empty))
            .ToArrayAsync();

        if (pictures.Length >= 30)
        {
            pictures = pictures.Skip(20).Take(10).ToArray();
        }
        else if (pictures.Length >= 20 & pictures.Length < 30)
        {
            pictures = pictures.Skip(10).Take(10).ToArray();
        }
        else
        {
            pictures = pictures.Take(10).ToArray();
        }

        return View(pictures);
    }
}