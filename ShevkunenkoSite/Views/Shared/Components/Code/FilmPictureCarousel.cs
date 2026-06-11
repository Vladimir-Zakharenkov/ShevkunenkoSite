namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class FilmPictureCarousel(
    IImageFileRepository imageContext,
    IFilmFileRepository filmContext
    ) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(FilmFileModel filmForPictureCarousel)
    {
        // Массив картинок
        ImageFileModel[] pictures = [];

        // Экземпляр полной версии фильма
        FilmFileModel? fullFilm = null;

        // Создаем экземпляр полной версии фильма если она существует
        if (await filmContext.FilmFiles.Where(film => film.FilmFileModelId == filmForPictureCarousel.FullFilmId).AnyAsync())
        {
            fullFilm = await filmContext.FilmFiles.FirstAsync(film => film.FilmFileModelId == filmForPictureCarousel.FullFilmId);
        }

        if (fullFilm != null)
        {
            if (await imageContext.ImageFiles.Where(pict => pict.SearchFilter.Contains($"{fullFilm.FilmCaption}#film-album#,")).AnyAsync())
            {
                pictures = await imageContext.ImageFiles
                    .Where(pict => pict.SearchFilter.Contains($"{fullFilm.FilmCaption}#film-album#,"))
                    .ToArrayAsync();
            }
            else
            {
                return View("Empty");
            }
        }
        else
        {
            if (await imageContext.ImageFiles.Where(pict => pict.SearchFilter.Contains($"{filmForPictureCarousel.FilmCaption}#film-album#,")).AnyAsync())
            {
                pictures = await imageContext.ImageFiles
                    .Where(pict => pict.SearchFilter.Contains($"{filmForPictureCarousel.FilmCaption}#film-album#,"))
                    .ToArrayAsync();
            }
            else
            {
                return View("Empty");
            }
        }

        int numbeOfImages = pictures.Length / 3;

        // Если картинок меньше 9 - карусель не показываем
        if (numbeOfImages < 9)
        {
            return View("Empty");
        }
        // В каждой карусели не более 12 картинок
        else if (numbeOfImages >= 12)
        {
            return View(new PictureCarouselForFilmViewModel
            {
                FirstCarousel = [.. pictures.Take(12)],
                SecondCarousel = [.. pictures.Skip(12).Take(12)],
                ThirdCarousel = [.. pictures.Skip(24)],
                FilmFile = filmForPictureCarousel
            });
        }
        else
        {
            return View(new PictureCarouselForFilmViewModel
            {
                FirstCarousel = [.. pictures.Take(numbeOfImages)],
                SecondCarousel = [.. pictures.Skip(numbeOfImages).Take(numbeOfImages)],
                ThirdCarousel = [.. pictures.Skip(numbeOfImages * 2)],
                FilmFile = filmForPictureCarousel
            });
        }
    }
}