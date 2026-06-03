using Microsoft.IdentityModel.Tokens;

namespace ShevkunenkoSite.Controllers
{
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

                FilmFileModel film = await filmContext.FilmFiles
                    .AsNoTracking()
                    .Include(image => image.FilmImage)
                    .FirstAsync(film => film.FilmCaption == filmCaption);

                #endregion

                #region Определяем текущий видеохостинг

                if (host == "yt" && film.FilmYouTube != null)
                {
                    film.CurrentVideoHost = film.FilmYouTube;
                }
                else if (host == "vk" && film.FilmVkVideo != null)
                {
                    film.CurrentVideoHost = film.FilmVkVideo;
                }
                else if (host == "ok" && film.FilmOkVideo != null)
                {
                    film.CurrentVideoHost = film.FilmOkVideo;
                }
                else if (host == "ml" && film.FilmMailRuVideo != null)
                {
                    film.CurrentVideoHost = film.FilmMailRuVideo;
                }
                else
                {
                    film.CurrentVideoHost = film.FilmContentUrl;
                }

                #endregion

                #region Кадры слева и справа от фильма

                // Если есть картинки с фильтром == название фильма + #album#
                if (await imageContext.ImageFiles
                    .Where(img => img.SearchFilter.Contains(film.FilmCaption + "#film-album#"))
                    .AnyAsync())
                {
                    var listOfPictures = from m in imageContext.ImageFiles
                       .Where(p => p.SearchFilter.Contains(film.FilmCaption + "#film-album#"))
                                         select m;

                    List<ImageFileModel> framesAroundFilm = [.. listOfPictures.AsEnumerable().Shuffle()];

                    film.ListOfPictures = [.. listOfPictures.AsEnumerable()];

                    if (framesAroundFilm.Count > 1 && framesAroundFilm.Count < DataConfig.NumberOfPicturesAround * 2)
                    {
                        film.FramesOnTheLeft = [.. framesAroundFilm.Take(framesAroundFilm.Count / 2)];

                        film.FramesOnTheRight = [.. framesAroundFilm.Skip(framesAroundFilm.Count / 2)];
                    }
                    else if (framesAroundFilm.Count >= DataConfig.NumberOfPicturesAround * 2)
                    {
                        film.FramesOnTheLeft = [.. framesAroundFilm.Take(DataConfig.NumberOfPicturesAround)];

                        film.FramesOnTheRight = [.. framesAroundFilm.Skip(DataConfig.NumberOfPicturesAround).Take(DataConfig.NumberOfPicturesAround)];
                    }
                    else
                    {
                        film.FramesOnTheLeft = framesAroundFilm;

                        film.FramesOnTheRight = framesAroundFilm;
                    }
                }
                else
                {
                    film.FramesOnTheLeft = [];

                    film.FramesOnTheRight = [];
                }

                #endregion

                return View(film);
            }
        }

        #endregion

        #region Кадры фильма

        [HttpGet]
        public async Task<IActionResult> FilmPhotoAlbum(Guid? imageId, string? filmCaption, int pageNumber = 1)
        {
            #region Если не задан или не найден фильм по filmCaption

            if (string.IsNullOrEmpty(filmCaption) || await filmContext.FilmFiles.Where(film => film.FilmCaption == filmCaption.Trim()).AnyAsync() == false)
            {
                return RedirectToAction(nameof(Index));
            }

            #endregion

            #region Если картинка не найдена

            if (imageId != null && await imageContext.ImageFiles.Where(img => img.ImageFileModelId == imageId).AnyAsync() == false)
            {
                return RedirectToAction(nameof(Film), new { filmCaption });
            }

            #endregion

            #region Разделители в фильтре картинки

            string album = "#film-album#";

            string note = "#film-note#";

            #endregion

            #region Если нельзя найти картинки по filmCaption

            if (await imageContext.ImageFiles.Where(img => img.SearchFilter.Contains(filmCaption + album)).AnyAsync() == false)
            {
                return RedirectToAction(nameof(Film), new { filmCaption });
            }

            #endregion

            #region Инициализация PhotoAlbumViewModel

            PhotoAlbumViewModel photoAlbumView = new();

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

                if (imageItem.SearchFilter.Contains(album))
                {
                    #region Определение заголовка и подзаголовка альбома

                    string[] filters = imageItem.SearchFilter.Split(',', StringSplitOptions.TrimEntries);

                    string? filterForCaption = Array.Find(filters, p => p.Contains(album));

                    if (filterForCaption != null)
                    {
                        int foundForCaption = filterForCaption.IndexOf(album);

                        photoAlbumView.CaptionOfAlbum = filterForCaption[..foundForCaption];

                        if (filterForCaption.Contains(note))
                        {
                            int foundForNote = filterForCaption.IndexOf(note);

                            photoAlbumView.NoteForCaptionOfAlbum = filterForCaption[(foundForCaption + album.Length)..foundForNote];
                        }
                    }

                    #endregion

                    #region Массив картинок по определенному названию альбома

                    var allItems = from m in imageContext.ImageFiles
                       .Where(p => p.SearchFilter.Contains(photoAlbumView.CaptionOfAlbum + album))
                       .OrderBy(p => p.SortOfPicture)
                                   select m;

                    var arrayOfItems = await allItems.ToArrayAsync();

                    photoAlbumView.AllImageFiles = arrayOfItems;

                    photoAlbumView.TotalItems = arrayOfItems.Length;

                    #endregion

                    #region Номер по порядку текущей картинки в массиве

                    var indexOfItem = Array.FindIndex(arrayOfItems, item => item.ImageFileModelId == imageId) + 1;

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

                    var itemsOnPage = await allItems
                       .Skip((pageNumber - 1) * photoAlbumView.ItemsPerPage)
                       .Take(photoAlbumView.ItemsPerPage)
                       .ToArrayAsync();

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
                   .Where(p => p.SearchFilter.Contains(filmCaption + album))
                   .OrderBy(p => p.SortOfPicture)
                               select m;

                var arrayOfItems = await allItems.ToArrayAsync();

                photoAlbumView.AllImageFiles = arrayOfItems;

                photoAlbumView.TotalItems = arrayOfItems.Length;

                #endregion

                #region Проверка параметра pageNumber

                if (pageNumber < 1
                    || pageNumber > (arrayOfItems.Length % photoAlbumView.ItemsPerPage == 0 ? (arrayOfItems.Length / photoAlbumView.ItemsPerPage) : (arrayOfItems.Length / photoAlbumView.ItemsPerPage + 1)))
                {
                    return RedirectToAction(nameof(FilmPhotoAlbum), new { pageNumber = 1 });
                }

                #endregion

                #region Массив картинок для текущей страницы

                var itemsOnPage = await allItems
                   .Skip((pageNumber - 1) * photoAlbumView.ItemsPerPage)
                   .Take(photoAlbumView.ItemsPerPage)
                   .ToArrayAsync();

                photoAlbumView.ItemsOnPage = itemsOnPage;

                #endregion

                #region Определение заголовка и подзаголовка альбома

                string[] filters = itemsOnPage[0].SearchFilter.Split(',', StringSplitOptions.TrimEntries);

                string? filterForCaption = Array.Find(filters, p => p.Contains(album));

                if (filterForCaption != null)
                {
                    int foundForCaption = filterForCaption.IndexOf(album);

                    photoAlbumView.CaptionOfAlbum = filterForCaption[..foundForCaption];

                    if (filterForCaption.Contains(note))
                    {
                        int foundForNote = filterForCaption.IndexOf(note);

                        photoAlbumView.NoteForCaptionOfAlbum = filterForCaption[(foundForCaption + album.Length)..foundForNote];
                    }
                }

                #endregion

                #region Устанавливаем Id для CurrentImageId

                photoAlbumView.CurrentImageId = itemsOnPage[0].ImageFileModelId;

                #endregion

                #region Номер текущей страницы

                photoAlbumView.CurrentPage = pageNumber;

                #endregion
            }

            #endregion

            return View(photoAlbumView);
        }

        #endregion
    }
}