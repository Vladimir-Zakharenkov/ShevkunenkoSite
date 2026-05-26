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
        #region Проверка наличия книги с таким названием

        if (bookCaption == null
            || bookCaption is not string
            || !await articleContext.BooksAndArticles
                        .Where(article => article.CaptionOfText == bookCaption.Replace('-', ' '))
                        .AnyAsync())
        {
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Инициализация ViewModel

        ArticleViewModel articleViewModel = new();

        #endregion

        #region Экземпляр книги (статьи)

        articleViewModel.BookOrArticle = await articleContext.BooksAndArticles
                .Include(logo => logo.LogoOfArticle)
                .Include(scan => scan.ScanOfArticle)
                .Include(movie => movie.VideoForBookOrArticle)
                .Include(film => film.FilmForBookOrArticle)
                .FirstAsync(article => article.CaptionOfText == bookCaption.Replace('-', ' '));

        #endregion

        #region Если страница указана не верно

        if (pageNumber == null
            || pageNumber is not int
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

        if (await textContext.Texts
            .Where(text => text.BooksAndArticlesModelId == articleViewModel.BookOrArticle.BooksAndArticlesModelId & text.SequenceNumber == pageNumber)
            .AnyAsync())
        {
            articleViewModel.TextForBookOrArticle = await textContext.Texts
                .Include(audio => audio.AudioInfoModel)
                .FirstAsync(text => text.BooksAndArticlesModelId == articleViewModel.BookOrArticle.BooksAndArticlesModelId & text.SequenceNumber == pageNumber);
        }
        else
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
            if (await audioFileContext.AudioFiles
                .Where(audio => audio.AudioInfoModelId == articleViewModel.TextForBookOrArticle.AudioInfoModelId)
                .AnyAsync())
            {
                articleViewModel.AudioFileForText = await audioFileContext.AudioFiles
                    .Include(abook => abook.AudioBookModel)
                    .FirstAsync(audio => audio.AudioInfoModelId == articleViewModel.TextForBookOrArticle.AudioInfoModelId);
            }
        }

        #endregion

        #region Информация о странице сайта

        articleViewModel.PageInfo = await pageContext.GetPageInfoByPathAsync(HttpContext);

        #endregion

        #region Фото из книги - Фото слева и справа от текста

        // Если есть картинки с фильтром == заглавию книги с заменой пробелов на тире + #album#
        if (await imageFileContext.ImageFiles
            .Where(img => img.SearchFilter.Contains(articleViewModel.BookOrArticle.CaptionOfText.Replace(' ', '-') + "#album#"))
            .AnyAsync())
        {
            var listOfPictures = from m in imageFileContext.ImageFiles
               .Where(p => p.SearchFilter.Contains(articleViewModel.BookOrArticle.CaptionOfText.Replace(' ', '-') + "#album#"))
               .OrderBy(p => p.SortOfPicture)
                                 select m;

            articleViewModel.ListOfPictures = [.. listOfPictures.AsEnumerable().Shuffle()];

            if (articleViewModel.ListOfPictures.Count > 1 && articleViewModel.ListOfPictures.Count < DataConfig.NumberOfPicturesAround * 2)
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
        else
        {
            articleViewModel.FramesAroundMainContent = new FramesAroundMainContentModel
            {
                FramesOnTheLeft = [],
                FramesOnTheRight = []
            };
        }

        #endregion

        return View("Book", articleViewModel);
    }

    public async Task<IActionResult> AudioBook(string? audioBookCaption, string? audioActor, int? audioBookPart)
    {
        #region Проверка наличия аудиокниги с таким названием

        if (audioBookCaption == null
            || audioBookCaption is not string
            || !await audioBookContext.AudioBooks
                        .Where(abook => abook.CaptionOfAudioBook == audioBookCaption)
                        .AnyAsync())
        {
            return RedirectToAction(nameof(AudioBookIndex));
        }

        #endregion

        #region Проверка наличия актера в базе аудиокниг

        if (audioActor == null
           || audioActor is not string
           || !await audioBookContext.AudioBooks
                       .Where(abook => abook.ActorOfAudioBook == audioActor)
                       .AnyAsync())
        {
            return RedirectToAction(nameof(AudioBookIndex));
        }

        #endregion

        #region Инициализация ViewModel

        ArticleViewModel audioBookViewModel = new();

        #endregion

        #region Экземпляр аудиокниги по названию и актеру

        if (await audioBookContext.AudioBooks
            .Where(abook => abook.CaptionOfAudioBook == audioBookCaption & abook.ActorOfAudioBook == audioActor)
            .AnyAsync())
        {
            audioBookViewModel.AudioBook = await audioBookContext.AudioBooks
                .Include(abook => abook.BookForAudioBook)
                .FirstAsync(abook => abook.CaptionOfAudioBook == audioBookCaption & abook.ActorOfAudioBook == audioActor);
        }
        else
        {
            return RedirectToAction(nameof(AudioBookIndex));
        }

        #endregion

        #region Экемпляр связанной книги

        if (audioBookViewModel.AudioBook.BookForAudioBook != null)
        {
            audioBookViewModel.BookOrArticle = audioBookViewModel.AudioBook.BookForAudioBook;
        }

        #endregion

        #region Экземпляр аудиофайла по аудиокниге и номеру части

        if (audioBookPart is not int
            || audioBookPart < 1
            || audioBookPart > audioBookViewModel.AudioBook.NumberOfFiles)
        {
            RedirectToAction(nameof(AudioBook), new { audioBookCaption, audioActor, audioBookPart = 1 });
        }
        else
        {
            audioBookViewModel.AudioNumber = audioBookPart;

            if (await audioFileContext.AudioFiles
                .Include(audioBook => audioBook.AudioBookModel)
                //.Include(pageForAudio => pageForAudio.PageInfoModel)
                .Where(audioFile => audioFile.AudioBookModelId == audioBookViewModel.AudioBook.AudioBookModelId
                                                & audioFile.SequenceNumber == audioBookPart)
                .AnyAsync())
            {
                audioBookViewModel.AudioFileForText = await audioFileContext.AudioFiles
                    .Include(audioBook => audioBook.AudioBookModel)
                    //.Include(pageForAudio => pageForAudio.PageInfoModel)
                    .FirstAsync(audioFile =>
                                                    audioFile.AudioBookModelId == audioBookViewModel.AudioBook.AudioBookModelId
                                                    & audioFile.SequenceNumber == audioBookPart);
            }
            else
            {
                return RedirectToAction(nameof(AudioBookIndex));
            }
        }

        #endregion

        #region Информация о старнице сайта

        audioBookViewModel.PageInfo = await pageContext.GetPageInfoByPathAsync(HttpContext);

        #endregion

        #region Фото из книги - Фото слева и справа от текста

        if (audioBookViewModel.AudioBook.BookForAudioBook != null && await imageFileContext.ImageFiles
            .Where(img => img.SearchFilter.Contains(audioBookViewModel.AudioBook.BookForAudioBook.CaptionOfText.Replace(' ', '-') + "#album#"))
            .AnyAsync())
        {
            var listOfPictures = from m in imageFileContext.ImageFiles
               .Where(p => p.SearchFilter.Contains(audioBookViewModel.AudioBook.BookForAudioBook.CaptionOfText.Replace(' ', '-') + "#album#"))
               .OrderBy(p => p.SortOfPicture)
                                 select m;

            audioBookViewModel.ListOfPictures = [.. listOfPictures.AsEnumerable().Shuffle()];

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
        else
        {
            audioBookViewModel.FramesAroundMainContent = new FramesAroundMainContentModel
            {
                FramesOnTheLeft = [],
                FramesOnTheRight = []
            };
        }

        #endregion

        #region Ссылка на текст для аудиофайла (страница текста и транскрипт)

        if (audioBookViewModel.AudioFileForText != null)
        {
            // Список текстов ссылающихся на текущий аудиофайл
            List<TextInfoModel> listOfTexts = [];

            if (await textContext.Texts
                .Where(text => text.AudioInfoModelId == audioBookViewModel.AudioFileForText.AudioInfoModelId)
                .AnyAsync())
            {
                listOfTexts = await textContext.Texts
                    .Where(text => text.AudioInfoModelId == audioBookViewModel.AudioFileForText.AudioInfoModelId)
                    .ToListAsync();

                var firstNumberInTextList = listOfTexts.Any() ? listOfTexts.Min(text => text.SequenceNumber) : 0;

                var transcriptTextSize = listOfTexts.Max(text => text.TxtFileSize);

                audioBookViewModel.TextForBookOrArticle = await textContext.Texts
                    .Include(text => text.BooksAndArticlesModel)
                    .FirstAsync(text =>
                        text.AudioInfoModelId == audioBookViewModel.AudioFileForText.AudioInfoModelId
                        & text.SequenceNumber == firstNumberInTextList);

                var textInfoForTranscript = await textContext.Texts
                   .Include(text => text.BooksAndArticlesModel)
                   .FirstAsync(text =>
                       text.AudioInfoModelId == audioBookViewModel.AudioFileForText.AudioInfoModelId
                       & text.TxtFileSize == transcriptTextSize);

                using StreamReader transcriptText = new(rootPath + DataConfig.TextsFolderPath + textInfoForTranscript.FolderForText + textInfoForTranscript.TxtFileName);

                audioBookViewModel.Transcript = transcriptText.ReadToEnd();
            }
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

        if (imageId == null & string.IsNullOrEmpty(albumCaption))
        {
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Если картинка не найдена по введенному Id

        if (imageId != null & await imageFileContext.ImageFiles
                    .Where(img => img.ImageFileModelId == imageId).AnyAsync() == false)
        {
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Если нельзя найти картинки по указанному названию альбома

        if (albumCaption != null & await imageFileContext.ImageFiles
                    .Where(img => img.SearchFilter.Contains(albumCaption + album)).AnyAsync() == false)
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

            var itemsOnPage = await allItems
               .Skip((pageNumber - 1) * photoAlbumView.ItemsPerPage)
               .Take(photoAlbumView.ItemsPerPage)
               .ToArrayAsync();

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
}