namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class FilmPictureCarousel(
    IImageFileRepository imageContext
    ) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string captionForPictureCarousel)
    {
        // Массив картинок
        ImageFileModel[] pictures = [];

        #region Разделители в фильтре картинки

        string album = "#film-album#";

        #endregion

        if (await imageContext.ImageFiles.Where(pict => pict.SearchFilter.ToLower().Contains(captionForPictureCarousel + album)).AnyAsync())
        {
            pictures = await imageContext.ImageFiles
                .Where(pict => pict.SearchFilter.ToLower().Contains(captionForPictureCarousel + album))
                .ToArrayAsync();
        }
        else
        {
            return View("Empty");
        }

        int numbeOfImages = pictures.Length / 3;

        // Если картинок меньше 9 - карусель не показываем
        if (numbeOfImages < 3)
        {
            return View("Empty");
        }
        // В каждой карусели не более 12 картинок
        else if (numbeOfImages >= 12)
        {
            return View(new PictureCarouselForFilmViewModel
            {
                FirstCarousel = [.. pictures.Take(12).Shuffle2()],
                SecondCarousel = [.. pictures.Skip(12).Take(12).Shuffle2()],
                ThirdCarousel = [.. pictures.Skip(24).Take(12).Shuffle2()],
                FilmCaption = captionForPictureCarousel
            });
        }
        else
        {
            return View(new PictureCarouselForFilmViewModel
            {
                FirstCarousel = [.. pictures.Take(numbeOfImages).Shuffle2()],
                SecondCarousel = [.. pictures.Skip(numbeOfImages).Take(numbeOfImages).Shuffle2()],
                ThirdCarousel = [.. pictures.Skip(numbeOfImages * 2).Take(numbeOfImages).Shuffle2()],
                FilmCaption = captionForPictureCarousel
            });
        }
    }
}