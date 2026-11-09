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
                            .Where(p => p.SearchFilter != null &&
                                        p.SearchFilter.ToLower().Contains(pattern))
                            .OrderBy(p => p.SortOfPicture)
                            .ThenBy(p => p.ImageFileModelId)
                            .ToArrayAsync();

        if (pictures.Length == 0)
        {
            return View("Empty");
        }

        // В каждой карусели не более 12 картинок
        int countPerCarousel = pictures.Length / 3 >= 12 ? 12 : pictures.Length / 3;

        int remainder = pictures.Length % 3;

        // Если картинок меньше 9 - карусель не показываем
        if (countPerCarousel < 3)
        {
            return View("Empty");
        }
        else
        {
            return View(new PictureCarouselForFilmViewModel
            {
                FirstCarousel = [.. pictures.Take(countPerCarousel)],
                SecondCarousel = [.. pictures.Skip(countPerCarousel).Take(countPerCarousel)],
                ThirdCarousel = [.. pictures.Skip(countPerCarousel * 2).Take(countPerCarousel + remainder)],
                FilmCaption = captionForPictureCarousel
            });
        }
    }
}