using Microsoft.IdentityModel.Tokens;

namespace ShevkunenkoSite.Controllers;

public class FilmsController
    (
    IFilmFileRepository filmContext,
    IImageFileRepository imageContext
    )
    : Controller
{
    #region Список фильмов

    [HttpGet]
    public async Task<IActionResult> Index
       (
       string? searchString,
       int pageNumber = 1
       )
    {
        var allFilmsSite = await filmContext.FilmFiles.Where(film => film.FilmInMainList == true).ToListAsync();

        if (!searchString.IsNullOrEmpty())
        {
            allFilmsSite = [.. allFilmsSite.FilmSiteSearch(searchString).OrderBy(filmSite => filmSite.FilmDatePublished)];
        }

        ItemsListViewModel itemList = new()
        {
            AllFilmFiles = [.. allFilmsSite
                 .Skip((pageNumber - 1) * DataConfig.NumberOfItemsPerPage)
                 .Take(DataConfig.NumberOfItemsPerPage)],

            #region Свойства PagingInfoViewModel

            TotalItems = allFilmsSite.Count,

            ItemsPerPage = DataConfig.NumberOfItemsPerPage,

            CurrentPage = pageNumber,

            SearchString = searchString ?? string.Empty,

            #endregion
        };

        return View(itemList);
    }

    #endregion

    #region Фильм

    [HttpGet]
    public async Task<IActionResult> Film(string? filmCaption, string? host)
    {
        if (string.IsNullOrEmpty(filmCaption) || !await filmContext.FilmFiles.Where(film => film.FilmCaption == filmCaption).AnyAsync())
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            #region Инициализация фильма

            FilmFileModel filmItem = await filmContext.FilmFiles
                .Include(img => img.FilmImage)
                .Include(img => img.FilmPoster)
                .Include(film => film.FullFilm)
                    .ThenInclude(image => image!.FilmImage)
                .AsNoTracking()
                .FirstAsync(film => film.FilmCaption == filmCaption);

            #endregion

            #region Определяем текущий видеохостинг

            if (host == "yt" && filmItem.FilmYouTube != null)
            {
                filmItem.CurrentVideoHost = filmItem.FilmYouTube;
            }
            else if (host == "vk" && filmItem.FilmVkVideo != null)
            {
                filmItem.CurrentVideoHost = filmItem.FilmVkVideo;
            }
            else if (host == "ok" && filmItem.FilmOkVideo != null)
            {
                filmItem.CurrentVideoHost = filmItem.FilmOkVideo;
            }
            else if (host == "ml" && filmItem.FilmMailRuVideo != null)
            {
                filmItem.CurrentVideoHost = filmItem.FilmMailRuVideo;
            }
            else
            {
                filmItem.CurrentVideoHost = filmItem.FilmContentUrl;
            }

            #endregion

            #region Работа с кадрами фильма

            #region Кадры фильма

            // Картинки с фильтром == название фильма + #film-album#
            var listOfPictures = from m in imageContext.ImageFiles
               .Where(p => p.SearchFilter != null &&
                           p.SearchFilter.Contains(filmItem.FilmCaption + "#film-album#"))
                                 select m;

            filmItem.ListOfPictures = [.. listOfPictures];

            #endregion

            #region Кадры другого фильма

            // Если задан GUID фильма для кадров
            if (filmItem.FilmForPictureId != null
                && await filmContext.FilmFiles
                    .Where(film => film.FilmFileModelId == filmItem.FilmForPictureId)
                    .AnyAsync())
            {
                #region Инициализация фильма для кадров

                filmItem.FilmForPictureAround = await filmContext.FilmFiles
                    .AsNoTracking()
                    .FirstAsync(film => film.FilmFileModelId == filmItem.FilmForPictureId);

                #endregion

                // Картинки с фильтром == название фильма + #film-album#
                if (await imageContext.ImageFiles
                    .Where(img => img.SearchFilter != null &&
                                  img.SearchFilter.Contains(filmItem.FilmForPictureAround.FilmCaption + "#film-album#"))
                    .AnyAsync())
                {
                    listOfPictures = from m in imageContext.ImageFiles
                       .Where(p => p.SearchFilter != null &&
                                   p.SearchFilter.Contains(filmItem.FilmForPictureAround.FilmCaption + "#film-album#"))
                                     select m;

                    filmItem.ListOfPictures = [.. listOfPictures];
                }
            }

            #endregion

            #region Кадры слева и справа

            if (listOfPictures.Any())
            {
                List<ImageFileModel> framesAroundFilm = [.. listOfPictures.AsEnumerable().Shuffle()];

                if (framesAroundFilm.Count > 1 && framesAroundFilm.Count < DataConfig.NumberOfPicturesAround * 2)
                {
                    filmItem.FramesOnTheLeft = [.. framesAroundFilm.Take(framesAroundFilm.Count / 2)];

                    filmItem.FramesOnTheRight = [.. framesAroundFilm.Skip(framesAroundFilm.Count / 2)];
                }
                else if (framesAroundFilm.Count >= DataConfig.NumberOfPicturesAround * 2)
                {
                    filmItem.FramesOnTheLeft = [.. framesAroundFilm.Take(DataConfig.NumberOfPicturesAround)];

                    filmItem.FramesOnTheRight = [.. framesAroundFilm.Skip(DataConfig.NumberOfPicturesAround).Take(DataConfig.NumberOfPicturesAround)];
                }
                else
                {
                    filmItem.FramesOnTheLeft = framesAroundFilm;

                    filmItem.FramesOnTheRight = framesAroundFilm;
                }
            }
            else
            {
                filmItem.FramesOnTheLeft = [];

                filmItem.FramesOnTheRight = [];
            }

            #endregion

            #endregion

            return View(filmItem);
        }
    }

    #endregion

    #region Альбом из кадров фильма

    /// <summary>
    /// Альбом из кадров фильма
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> FilmPhotoAlbum(
        Guid? imageId,
        string? filmCaption,
        int pageNumber = 1,
        bool showAI = true)
    {
        #region Инициализация PhotoAlbumViewModel

        PhotoAlbumViewModel photoAlbumView = new()
        {
            ShowAI = showAI
        };

        #endregion

        #region Разделители в фильтре картинки

        string album = "#film-album#";

        string note = "#film-note#";

        #endregion

        #region Если не задан filmCaption или картинок по filmCaption нет

        if (string.IsNullOrEmpty(filmCaption))
        {
            return RedirectToAction(nameof(Index));
        }

        bool hasPictures = await imageContext.ImageFiles
            .Where(img => img.SearchFilter != null &&
                          img.SearchFilter.Contains(filmCaption + album))
            .AnyAsync();

        if (!hasPictures)
        {
            return RedirectToAction(nameof(Film), new { filmCaption, host = "ok" });
        }

        photoAlbumView.CaptionOfAlbum = filmCaption;

        #endregion

        #region Если картинка не найдена

        if (imageId != null
                    && !await imageContext.ImageFiles.AnyAsync(img => img.ImageFileModelId == imageId))
        {
            return RedirectToAction(nameof(Film), new { filmCaption });
        }

        #endregion

        #region Просмотр картинки

        if (imageId != null)
        {
            #region Показываем страницу картинки

            photoAlbumView.AlbumOrPhoto = false;

            #endregion

            #region Экземпляр картинки

            var imageItem = await imageContext.ImageFiles.FirstAsync(img => img.ImageFileModelId == imageId);

            photoAlbumView.CurrentImageId = imageId;

            #endregion

            if (imageItem.SearchFilter is not null &&
                imageItem.SearchFilter.Contains(filmCaption + album, StringComparison.OrdinalIgnoreCase))
            {
                // ✅ Есть ли у текущей картинки AI-варианты
                photoAlbumView.CurrentImageHasAI =
                    !string.IsNullOrEmpty(imageItem.WebImageFileNameAI) ||
                    !string.IsNullOrEmpty(imageItem.WebImageHDFileNameAI) ||
                    !string.IsNullOrEmpty(imageItem.WebIconFileNameAI) ||
                    !string.IsNullOrEmpty(imageItem.WebIcon200FileNameAI) ||
                    !string.IsNullOrEmpty(imageItem.WebIcon100FileNameAI);

                #region Определение заголовка и подзаголовка альбома

                var (caption, noteText) = ParseAlbumFilter(imageItem.SearchFilter, album, note);

                if (caption is null)
                {
                    return RedirectToAction(nameof(Index));
                }

                photoAlbumView.CaptionOfAlbum = caption;

                photoAlbumView.NoteForCaptionOfAlbum = noteText;

                #endregion

                #region Массив картинок по определенному названию альбома

                var allItems = from m in imageContext.ImageFiles
                               .Where(p => p.SearchFilter != null &&
                                           p.SearchFilter.Contains(filmCaption + album))
                               .OrderBy(p => p.SortOfPicture)
                               select m;

                var arrayOfItems = await allItems.ToArrayAsync();

                if (arrayOfItems.Length == 0)
                {
                    return RedirectToAction(nameof(Film), new { filmCaption, host = "ok" });
                }

                photoAlbumView.AllImageFiles = [.. arrayOfItems];

                photoAlbumView.TotalItems = arrayOfItems.Length;

                #endregion

                #region Номер по порядку текущей картинки в массиве

                int indexZeroBased = Array.FindIndex(arrayOfItems, item => item.ImageFileModelId == imageId);

                if (indexZeroBased < 0)
                {
                    return RedirectToAction(nameof(Film), new { filmCaption, host = "ok" });
                }

                int indexOfItem = indexZeroBased + 1;

                #endregion

                #region Порядковый номер страницы альбома для текущей картинки

                if (indexOfItem < photoAlbumView.ItemsPerPage)
                {
                    pageNumber = 1;
                }
                else if (indexOfItem % photoAlbumView.ItemsPerPage == 0)
                {
                    pageNumber = indexOfItem / photoAlbumView.ItemsPerPage;
                }
                else
                {
                    pageNumber = indexOfItem / photoAlbumView.ItemsPerPage + 1;
                }

                photoAlbumView.CurrentPage = pageNumber;

                #endregion

                #region Массив картинок для текущей страницы

                var itemsOnPage = arrayOfItems
                   .Skip((pageNumber - 1) * photoAlbumView.ItemsPerPage)
                   .Take(photoAlbumView.ItemsPerPage)
                   .ToArray();

                photoAlbumView.ItemsOnPage = itemsOnPage;

                #endregion
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Просмотр страницы альбома

        else
        {
            #region Показываем страницу альбома

            photoAlbumView.AlbumOrPhoto = true;

            #endregion

            #region Массив картинок по определенному названию альбома

            var allItems = from m in imageContext.ImageFiles
                           .Where(p => p.SearchFilter != null &&
                                       p.SearchFilter.Contains(filmCaption + album))
                           .OrderBy(p => p.SortOfPicture)
                           select m;

            var arrayOfItems = await allItems.ToArrayAsync();

            if (arrayOfItems.Length == 0)
            {
                return RedirectToAction(nameof(Film), new { filmCaption, host = "ok" });
            }

            photoAlbumView.AllImageFiles = [.. arrayOfItems];

            photoAlbumView.TotalItems = arrayOfItems.Length;

            #endregion

            #region Проверка параметра pageNumber

            int maxPage = (arrayOfItems.Length + photoAlbumView.ItemsPerPage - 1) / photoAlbumView.ItemsPerPage;

            if (pageNumber < 1 || pageNumber > maxPage)
            {
                return RedirectToAction(nameof(FilmPhotoAlbum), new { filmCaption, pageNumber = 1, showAI });
            }

            #endregion

            #region Массив картинок для текущей страницы

            var itemsOnPage = arrayOfItems
               .Skip((pageNumber - 1) * photoAlbumView.ItemsPerPage)
               .Take(photoAlbumView.ItemsPerPage)
               .ToArray();

            photoAlbumView.ItemsOnPage = itemsOnPage;

            #endregion

            #region Определение заголовка и подзаголовка альбома

            if (itemsOnPage.Length == 0)
            {
                return RedirectToAction(nameof(Film), new { filmCaption, host = "ok" });
            }

            var (caption, noteText) = ParseAlbumFilter(itemsOnPage[0].SearchFilter, album, note);

            if (caption is null)
            {
                return RedirectToAction(nameof(Index));
            }

            photoAlbumView.CaptionOfAlbum = caption;

            photoAlbumView.NoteForCaptionOfAlbum = noteText;

            #endregion

            #region Номер текущей страницы

            photoAlbumView.CurrentPage = pageNumber;

            #endregion
        }

        #endregion

        return View(photoAlbumView);
    }

    #endregion

    #region Скачать файл

    public async Task<IActionResult> DownloadFilm(string? filmFileName)
    {
        if (string.IsNullOrEmpty(filmFileName))
        {
            return RedirectToAction(nameof(Index));
        }

        bool exists = await filmContext.FilmFiles.AnyAsync(film => film.FilmFileName == filmFileName);

        if (!exists)
        {
            return RedirectToAction(nameof(Index));
        }

        string filePath = Path.Combine(DataConfig.MovieFoldersPath, filmFileName);

        return PhysicalFile(filePath, "application/octet-stream", filmFileName);
    }

    #endregion

    #region Метод определения Заголовка и Подзаголовка альбома

    private static (string? Caption, string? Note) ParseAlbumFilter(
                    string? searchFilter, string album, string note)
    {
        if (string.IsNullOrEmpty(searchFilter))
            return (null, null);

        string[] filters = searchFilter.Split(',',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        string? filterForCaption = Array.Find(filters,
            p => p.Contains(album, StringComparison.OrdinalIgnoreCase));

        if (filterForCaption is null)
            return (null, null);

        filterForCaption = filterForCaption.ToLowerInvariant();

        int foundForCaption = filterForCaption.IndexOf(album);

        string caption = filterForCaption[..foundForCaption];

        string? noteText = null;

        if (filterForCaption.Contains(note))
        {
            int foundForNote = filterForCaption.IndexOf(note);
            noteText = filterForCaption[(foundForCaption + album.Length)..foundForNote];
        }

        return (caption, noteText);
    }

    #endregion
}