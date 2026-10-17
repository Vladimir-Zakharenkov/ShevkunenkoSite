namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class FilmPictureCarousel(
    IImageFileRepository imageContext
    ) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string captionForPictureCarousel)
    {
        // Разделители в фильтре картинки
        string album = "#film-album#";

        // Приводим в нижний регистр
        string pattern = (captionForPictureCarousel + album).ToLower();

        // Массив картинок
        ImageFileModel[] pictures = await imageContext.ImageFiles
                            .Where(pict => pict.SearchFilter.ToLower().Contains(pattern))
                            .ToArrayAsync();

        if (pictures.Length == 0)
        {
            return View("Empty");
        }

        int countPerCarousel = pictures.Length / 3;
        int remainder = pictures.Length % 3;

        // Если картинок меньше 9 - карусель не показываем
        if (countPerCarousel < 3)
        {
            return View("Empty");
        }

        // В каждой карусели не более 12 картинок
        else if (countPerCarousel >= 12)
        {
            return View(new PictureCarouselForFilmViewModel
            {
                FirstCarousel = [.. pictures.Take(countPerCarousel).Shuffle2()],
                SecondCarousel = [.. pictures.Skip(countPerCarousel).Take(countPerCarousel).Shuffle2()],
                ThirdCarousel = [.. pictures.Skip(countPerCarousel * 2).Take(countPerCarousel + remainder).Shuffle2()],
                FilmCaption = captionForPictureCarousel
            });
        }
        else
        {
            return View(new PictureCarouselForFilmViewModel
            {
                FirstCarousel = [.. pictures.Take(countPerCarousel).Shuffle2()],
                SecondCarousel = [.. pictures.Skip(countPerCarousel).Take(countPerCarousel).Shuffle2()],
                ThirdCarousel = [.. pictures.Skip(countPerCarousel * 2).Take(countPerCarousel + remainder).Shuffle2()],
                FilmCaption = captionForPictureCarousel
            });
        }
    }
}