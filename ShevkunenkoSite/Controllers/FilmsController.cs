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

        #region Если не задан filmCaption

        if (string.IsNullOrEmpty(filmCaption))
        {
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Полный массив картинок по filmCaption из URL

        var allItemsQuery = from m in imageContext.ImageFiles
                            .Where(p => p.SearchFilter != null &&
                                        p.SearchFilter.ToLower().Contains((filmCaption + album).ToLower()))
                            .OrderBy(p => p.SortOfPicture)
                            .ThenBy(p => p.ImageFileModelId)
                            select m;

        var arrayOfItems = await allItemsQuery.ToArrayAsync();

        if (arrayOfItems.Length == 0)
        {
            return RedirectToAction(nameof(Film), new { filmCaption, host = "ok" });
        }

        photoAlbumView.AllImageFiles = [.. arrayOfItems];
        photoAlbumView.TotalItems = arrayOfItems.Length;

        #endregion

        #region Ключ выборки = filmCaption из URL (НЕ перезаписывается)

        // ✅ Ключевое: FilmCaption всегда равен filmCaption из URL.
        // Все ссылки (иконки, карусель, пагинация) строятся по нему.
        // CaptionOfAlbum — только «красивый заголовок» для отображения.
        photoAlbumView.FilmCaption = filmCaption;

        #endregion

        #region Красивый заголовок и подзаголовок — только для отображения

        // Берём из первой картинки полного массива — консистентно для всех страниц.
        var (displayCaption, displayNote) = ParseAlbumFilter(arrayOfItems[0].SearchFilter, filmCaption, album, note);

        photoAlbumView.CaptionOfAlbum = displayCaption ?? filmCaption;
        photoAlbumView.NoteForCaptionOfAlbum = displayNote;

        #endregion

        #region Если картинка не найдена в полном массиве

        if (imageId != null && !arrayOfItems.Any(item => item.ImageFileModelId == imageId))
        {
            return RedirectToAction(nameof(Film), new { filmCaption });
        }

        #endregion

        #region Просмотр картинки

        if (imageId != null)
        {
            photoAlbumView.AlbumOrPhoto = false;

            ImageFileModel imageItem = arrayOfItems.First(item => item.ImageFileModelId == imageId);

            photoAlbumView.CurrentImageId = imageId;

            photoAlbumView.CurrentImageHasAI =
                !string.IsNullOrEmpty(imageItem.WebImageFileNameAI) ||
                !string.IsNullOrEmpty(imageItem.WebImageHDFileNameAI) ||
                !string.IsNullOrEmpty(imageItem.WebIconFileNameAI) ||
                !string.IsNullOrEmpty(imageItem.WebIcon200FileNameAI) ||
                !string.IsNullOrEmpty(imageItem.WebIcon100FileNameAI);

            int indexZeroBased = Array.FindIndex(arrayOfItems, item => item.ImageFileModelId == imageId);

            if (indexZeroBased < 0)
            {
                return RedirectToAction(nameof(Film), new { filmCaption, host = "ok" });
            }

            int indexOfItem = indexZeroBased + 1;

            pageNumber = (indexOfItem - 1) / photoAlbumView.ItemsPerPage + 1;

            photoAlbumView.CurrentPage = pageNumber;

            photoAlbumView.ItemsOnPage = [.. arrayOfItems
                .Skip((pageNumber - 1) * photoAlbumView.ItemsPerPage)
                .Take(photoAlbumView.ItemsPerPage)];
        }

        #endregion

        #region Просмотр страницы альбома

        else
        {
            photoAlbumView.AlbumOrPhoto = true;

            int maxPage = (arrayOfItems.Length + photoAlbumView.ItemsPerPage - 1) / photoAlbumView.ItemsPerPage;

            if (pageNumber < 1 || pageNumber > maxPage)
            {
                return RedirectToAction(nameof(FilmPhotoAlbum), new { filmCaption, pageNumber = 1, showAI });
            }

            photoAlbumView.ItemsOnPage = [.. arrayOfItems
                .Skip((pageNumber - 1) * photoAlbumView.ItemsPerPage)
                .Take(photoAlbumView.ItemsPerPage)];

            photoAlbumView.CurrentPage = pageNumber;
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
        string? searchFilter,
        string filmCaption,
        string album,
        string note)
    {
        if (string.IsNullOrEmpty(searchFilter))
            return (null, null);

        string[] filters = searchFilter.Split(',',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        // ✅ Ищем фильтр ИМЕННО текущего фильма, а не первый попавшийся с album
        string? filterForCaption = Array.Find(filters,
            p => p.Contains(filmCaption + album, StringComparison.OrdinalIgnoreCase));

        if (filterForCaption is null)
            return (null, null);

        filterForCaption = filterForCaption.ToLowerInvariant();

        // caption — это часть фильтра ДО filmCaption + album (может быть пустой,
        // если фильтр начинается с filmCaption, как обычно и бывает)
        int foundForCaption = filterForCaption.IndexOf(
            filmCaption.ToLowerInvariant() + album,
            StringComparison.OrdinalIgnoreCase);

        if (foundForCaption < 0)
            return (null, null);

        string caption = foundForCaption > 0
            ? filterForCaption[..foundForCaption]
            : filmCaption;

        string? noteText = null;

        if (filterForCaption.Contains(note))
        {
            int foundForNote = filterForCaption.IndexOf(note);

            // note идёт сразу после filmCaption + album
            int noteStart = foundForCaption + filmCaption.Length + album.Length;

            if (foundForNote > noteStart)
                noteText = filterForCaption[noteStart..foundForNote];
        }

        return (caption, noteText);
    }
    #endregion
}