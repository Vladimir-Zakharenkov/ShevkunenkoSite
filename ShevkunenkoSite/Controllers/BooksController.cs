namespace ShevkunenkoSite.Controllers;

public class BooksController(
    IBooksAndArticlesRepository articleContext,
    ITextInfoRepository textContext,
    IPageInfoRepository pageContext,
    IImageFileRepository imageFileContext,
    IAudioBookRepository audioBookContext,
    IAudioInfoRepository audioFileContext,
    IWebHostEnvironment hostEnvironment
    ) : Controller
{
    private readonly string rootPath = hostEnvironment.WebRootPath;

    public IActionResult Index() => View();

    public IActionResult AudioBookIndex() => View(); // TODO: Написать страницу списка аудиокниг

    public async Task<IActionResult> Book(string? bookCaption, int? pageNumber)
    {
        #region ArticleViewModel + Экземпляр книги (статьи)

        if (bookCaption == null)
        {
            return RedirectToAction(nameof(Index));
        }

        string normalizedCaption = bookCaption.Replace('-', ' ');

        ArticleViewModel articleViewModel = new();

        var bookOrArticle = await articleContext.BooksAndArticles
                                .Include(logo => logo.LogoOfArticle)
                                .Include(scan => scan.ScanOfArticle)
                                .Include(movie => movie.VideoForBookOrArticle)
                                .Include(film => film.FilmForBookOrArticle)
                                .FirstOrDefaultAsync(article => article.CaptionOfText == normalizedCaption);

        if (bookOrArticle == null)
        {
            return RedirectToAction(nameof(Index));
        }

        articleViewModel.BookOrArticle = bookOrArticle;

        #endregion

        #region Если страница указана не верно

        if (pageNumber == null
            || pageNumber < 0
            || pageNumber > articleViewModel.BookOrArticle.NumberOfPages)
        {
            return RedirectToAction(nameof(Book), new { bookCaption, pageNumber = 0 });
        }

        #endregion

        #region Номер текущей страницы

        articleViewModel.PageNumber = pageNumber;

        #endregion

        #region Экземпляр текста для текущей страницы

        articleViewModel.TextForBookOrArticle = await textContext.Texts
            .Include(audio => audio.AudioInfoModel)
            .FirstOrDefaultAsync(text => text.BooksAndArticlesModelId == articleViewModel.BookOrArticle.BooksAndArticlesModelId
                                      && text.SequenceNumber == pageNumber);

        if (articleViewModel.TextForBookOrArticle == null)
        {
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region HTML текст

        if (System.IO.File.Exists(rootPath + DataConfig.TextsFolderPath + articleViewModel.TextForBookOrArticle.FolderForText + articleViewModel.TextForBookOrArticle.HtmlFileName))
        {
            using StreamReader htmlText = new(rootPath + DataConfig.TextsFolderPath + articleViewModel.TextForBookOrArticle.FolderForText + articleViewModel.TextForBookOrArticle.HtmlFileName);

            articleViewModel.HtmlText = htmlText.ReadToEnd();
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Аудиофайл (аудиокнига) связанная с экземпляром текста

        if (articleViewModel.TextForBookOrArticle.AudioInfoModelId != null)
        {
            articleViewModel.AudioFileForText = await audioFileContext.AudioFiles
                .Include(abook => abook.AudioBookModel)
                .FirstOrDefaultAsync(audio => audio.AudioInfoModelId == articleViewModel.TextForBookOrArticle.AudioInfoModelId);
        }

        #endregion

        #region Информация о странице сайта

        articleViewModel.PageInfo = await pageContext.GetPageInfoByPathAsync(HttpContext);

        #endregion

        #region Фото из книги - Фото слева и справа от текста

        string albumFilter = articleViewModel.BookOrArticle.CaptionOfText.Replace(' ', '-') + "#album#";

        var pictures = await imageFileContext.ImageFiles
            .Where(img => img.SearchFilter.Contains(albumFilter))
            .OrderBy(img => img.SortOfPicture)
            .ToListAsync();

        if (pictures.Count == 0)
        {
            articleViewModel.FramesAroundMainContent = new FramesAroundMainContentModel
            {
                FramesOnTheLeft = [],
                FramesOnTheRight = []
            };
        }
        else
        {
            articleViewModel.ListOfPictures = [.. pictures.Shuffle()];

            if (articleViewModel.ListOfPictures.Count > 1
                && articleViewModel.ListOfPictures.Count < DataConfig.NumberOfPicturesAround * 2)
            {
                articleViewModel.FramesAroundMainContent = new FramesAroundMainContentModel
                {
                    FramesOnTheLeft = [.. articleViewModel.ListOfPictures.Take(articleViewModel.ListOfPictures.Count / 2)],
                    FramesOnTheRight = [.. articleViewModel.ListOfPictures.Skip(articleViewModel.ListOfPictures.Count / 2)]
                };

                articleViewModel.ShowPicturesAround = true;
            }
            else if (articleViewModel.ListOfPictures.Count >= DataConfig.NumberOfPicturesAround * 2)
            {
                articleViewModel.FramesAroundMainContent = new FramesAroundMainContentModel
                {
                    FramesOnTheLeft = [.. articleViewModel.ListOfPictures.Take(DataConfig.NumberOfPicturesAround)],
                    FramesOnTheRight = [.. articleViewModel.ListOfPictures.Skip(DataConfig.NumberOfPicturesAround).Take(DataConfig.NumberOfPicturesAround)]
                };

                articleViewModel.ShowPicturesAround = true;
            }
            else
            {
                articleViewModel.FramesAroundMainContent = new FramesAroundMainContentModel
                {
                    FramesOnTheLeft = articleViewModel.ListOfPictures,
                    FramesOnTheRight = articleViewModel.ListOfPictures
                };
            }
        }

        #endregion

        return View("Book", articleViewModel);
    }

    public async Task<IActionResult> AudioBook(string? audioBookCaption, string? audioActor, int? audioBookPart)
    {
        #region Инициализация ViewModel

        ArticleViewModel audioBookViewModel = new();

        #endregion

        #region Экземпляр аудиокниги по названию и актеру

        audioBookViewModel.AudioBook = await audioBookContext.AudioBooks
                                        .Include(abook => abook.BookForAudioBook)
                                            .ThenInclude(film => film!.FilmForBookOrArticle)
                                        .FirstOrDefaultAsync(abook => abook.CaptionOfAudioBook == audioBookCaption
                                                                    && abook.ActorOfAudioBook == audioActor);

        if (audioBookViewModel.AudioBook == null)
        {
            return RedirectToAction(nameof(AudioBookIndex));
        }

        #endregion

        #region Экземпляр связанной книги

        if (audioBookViewModel.AudioBook.BookForAudioBook != null)
        {
            audioBookViewModel.BookOrArticle = audioBookViewModel.AudioBook.BookForAudioBook;
        }

        #endregion

        #region Проверка и установка номера части аудиокниги

        if (audioBookPart == null
            || audioBookPart < 1
            || audioBookPart > audioBookViewModel.AudioBook.NumberOfFiles)
        {
            return RedirectToAction(nameof(AudioBook), new { audioBookCaption, audioActor, audioBookPart = 1 });
        }

        audioBookViewModel.AudioNumber = audioBookPart;

        #endregion

        #region Экземпляр аудиофайла по аудиокниге и номеру части

        audioBookViewModel.AudioFileForText = await audioFileContext.AudioFiles
                                                    .Include(audioBook => audioBook.AudioBookModel)
                                                    .FirstOrDefaultAsync(audioFile => audioFile.AudioBookModelId == audioBookViewModel.AudioBook.AudioBookModelId
                                                                                   && audioFile.SequenceNumber == audioBookPart);

        if (audioBookViewModel.AudioFileForText == null)
        {
            return RedirectToAction(nameof(AudioBookIndex));
        }

        #endregion

        #region Информация о странице сайта

        audioBookViewModel.PageInfo = await pageContext.GetPageInfoByPathAsync(HttpContext);

        #endregion

        #region Фото из книги - Фото слева и справа от текста

        audioBookViewModel.FramesAroundMainContent = new FramesAroundMainContentModel
        {
            FramesOnTheLeft = [],
            FramesOnTheRight = []
        };

        if (audioBookViewModel.AudioBook.BookForAudioBook != null)
        {
            string albumFilter = audioBookViewModel.AudioBook.BookForAudioBook.CaptionOfText.Replace(' ', '-') + "#album#";

            var pictures = await imageFileContext.ImageFiles
                .Where(img => img.SearchFilter.Contains(albumFilter))
                .OrderBy(img => img.SortOfPicture)
                .ToListAsync();

            if (pictures.Count > 0)
            {
                audioBookViewModel.ListOfPictures = [.. pictures.Shuffle()];

                if (audioBookViewModel.ListOfPictures.Count > 1)
                {
                    audioBookViewModel.FramesAroundMainContent = new FramesAroundMainContentModel
                    {
                        FramesOnTheLeft = [.. audioBookViewModel.ListOfPictures.Take(audioBookViewModel.ListOfPictures.Count / 2)],
                        FramesOnTheRight = [.. audioBookViewModel.ListOfPictures.Skip(audioBookViewModel.ListOfPictures.Count / 2)]
                    };
                }
                else
                {
                    audioBookViewModel.FramesAroundMainContent = new FramesAroundMainContentModel
                    {
                        FramesOnTheLeft = [.. audioBookViewModel.ListOfPictures],
                        FramesOnTheRight = [.. audioBookViewModel.ListOfPictures]
                    };
                }
            }
        }

        #endregion

        #region Ссылка на текст для аудиофайла (страница текста и транскрипт)

        // Список текстов ссылающихся на текущий аудиофайл
        List<TextInfoModel> listOfTexts = await textContext.Texts
                                            .Include(text => text.BooksAndArticlesModel)
                                            .Where(text => text.AudioInfoModelId == audioBookViewModel.AudioFileForText.AudioInfoModelId)
                                            .ToListAsync();

        if (listOfTexts.Count > 0)
        {
            var firstNumberInTextList = listOfTexts.Min(text => text.SequenceNumber);

            var transcriptTextSize = listOfTexts.Max(text => text.TxtFileSize);

            audioBookViewModel.TextForBookOrArticle = listOfTexts
                .First(text => text.SequenceNumber == firstNumberInTextList);

            var textInfoForTranscript = listOfTexts
                .First(text => text.TxtFileSize == transcriptTextSize);

            using StreamReader transcriptText = new(rootPath + DataConfig.TextsFolderPath
                + textInfoForTranscript.FolderForText + textInfoForTranscript.TxtFileName);

            audioBookViewModel.Transcript = transcriptText.ReadToEnd();
        }

        #endregion

        return View(audioBookViewModel);
    }

    public async Task<IActionResult> PhotoAlbum(Guid? imageId, string? albumCaption, int pageNumber = 1)
    {
        #region Разделители в фильтре картинки

        string album = "#album#";

        string note = "#note#";

        #endregion

        #region Если не указан imageId картинки и заголовок альбома

        if (imageId == null && string.IsNullOrEmpty(albumCaption))
        {
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Если картинка не найдена по введенному Id

        if (imageId != null
            && !await imageFileContext.ImageFiles
                .AnyAsync(img => img.ImageFileModelId == imageId))
        {
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Если нельзя найти картинки по указанному названию альбома

        if (albumCaption != null
                && !await imageFileContext.ImageFiles
                    .AnyAsync(img => img.SearchFilter.Contains(albumCaption + album)))
        {
            return RedirectToAction(nameof(Index));
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

            var imageItem = await imageFileContext.ImageFiles.FirstAsync(img => img.ImageFileModelId == imageId);

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

                var allItems = from m in imageFileContext.ImageFiles
                   .Where(p => p.SearchFilter.Contains(photoAlbumView.CaptionOfAlbum + album))
                   .OrderBy(p => p.SortOfPicture)
                               select m;

                var arrayOfItems = await allItems.ToArrayAsync();

                photoAlbumView.AllImageFiles = arrayOfItems;

                photoAlbumView.TotalItems = arrayOfItems.Length;

                #endregion

                #region Номер по порядку в массиве текущей картинки

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

            var allItems = from m in imageFileContext.ImageFiles
               .Where(p => p.SearchFilter.Contains(albumCaption + album))
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
                return RedirectToAction(nameof(PhotoAlbum), new { pageNumber = 1 });
            }

            #endregion

            #region Массив картинок для текущей страницы

            var itemsOnPage = arrayOfItems
                                .Skip((pageNumber - 1) * photoAlbumView.ItemsPerPage)
                                .Take(photoAlbumView.ItemsPerPage)
                                .ToArray();

            if (itemsOnPage.Length == 0)
            {
                return RedirectToAction(nameof(PhotoAlbum), new { pageNumber = 1 });
            }

            photoAlbumView.ItemsOnPage = itemsOnPage;

            #endregion

            #region Определение заголовка и подзаголовка альбома

            string[] filters = itemsOnPage[0].SearchFilter.Split(',', StringSplitOptions.RemoveEmptyEntries);

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

    #region Скачать файл

    //public async Task<IActionResult> DownloadBook(string? filmFileName)
    //{
    //    if (!string.IsNullOrEmpty(filmFileName)
    //        && await filmContext.FilmFiles.Where(film => film.FilmFileName == filmFileName).AnyAsync())
    //    {
    //        string filePath = Path.Combine(DataConfig.MovieFoldersPath, filmFileName!);

    //        if (!System.IO.File.Exists(filePath))
    //            return NotFound("Файл не найден");

    //        var stream = System.IO.File.OpenRead(filePath);

    //        var mimeType = "application/octet-stream";

    //        return File(stream, mimeType, filmFileName);
    //    }
    //    else
    //    {
    //        return RedirectToAction(nameof(Index));
    //    }
    //}

    #endregion

}