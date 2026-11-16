using System.Linq;

namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class MovieFrames(IImageFileRepository imageFileContext) : ViewComponent
{
    private readonly int framesAroundMovie = 4;

    public async Task<IViewComponentResult> InvokeAsync(string? imageFilter, bool leftSide, bool oneFrame)
    {
        if (!string.IsNullOrEmpty(imageFilter)
            && await imageFileContext.ImageFiles
                .Where(img => img.SearchFilter != null &&
                      img.SearchFilter.ToLower().Contains(imageFilter.ToLower()))
                .AnyAsync())
        {
            var imageItems = await imageFileContext.ImageFiles
                .Where(img => img.SearchFilter != null &&
                      img.SearchFilter.ToLower().Contains(imageFilter.ToLower()))
                .ToArrayAsync();

            imageItems = imageItems.Shuffle().ToArray();

            if (imageItems.Length > 1)
            {
                if (leftSide)
                {
                    imageItems = imageItems.Take(imageItems.Length / 2).ToArray();
                }
                else
                {
                    imageItems = imageItems.Skip(imageItems.Length / 2).ToArray();
                }
            }

            // по одному кадру слева и справа (случайная выборка)
            if (oneFrame)
            {
                Random r = new();

                ImageFileModel imageItem = imageItems[r.Next(0, imageItems.Length)];

                return View("OneFrame", imageItem);
            }
            // кадры для страниц статей
            else if (HttpContext.Request.QueryString.ToString().Contains("articleid", StringComparison.CurrentCultureIgnoreCase)
                    || HttpContext.Request.QueryString.ToString().Contains("bookid", StringComparison.CurrentCultureIgnoreCase))
            {
                return View("PhotoArounArticle", imageItems);
            }
            // кадры для страниц видео
            else
            {
                if (imageItems.Length > framesAroundMovie)
                {
                    imageItems = [.. imageItems.Take(framesAroundMovie)];
                }

                return View(imageItems);
            }
        }
        else
        {
            ImageFileModel[] imageItems = [];

            return View(imageItems);
        }
    }
}