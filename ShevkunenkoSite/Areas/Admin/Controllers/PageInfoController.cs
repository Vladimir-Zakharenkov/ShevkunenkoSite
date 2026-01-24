using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace ShevkunenkoSite.Areas.Admin.Controllers;

// Имена методов не начинать со слова Page
[Area("Admin")]
[Authorize]
public class PageInfoController(
    IPageInfoRepository pageInfoContext,
    IMovieFileRepository movieContext,
    IIconFileRepository iconContext,
    IImageFileRepository imageContext,
    IBackgroundFotoRepository backgroundContext,
    IBooksAndArticlesRepository bookAndArticleContext,
    IAudioInfoRepository audioFileContext,
    ITextInfoRepository textFileContext,
    IWebHostEnvironment hostEnvironment
    ) : Controller
{
    private readonly string rootPath = hostEnvironment.WebRootPath;

    #region Список страниц сайта

    private static readonly char[] separator = [','];

    public async Task<ViewResult> Index
        (
        string? searchString,
        int pageNumber = 1,
        bool pageCard = false
        )
    {
        var allSitePages = await pageInfoContext.PagesInfo.ToListAsync();

        if (!searchString.IsNullOrEmpty())
        {
            allSitePages = [.. allSitePages.PageSearch(searchString).OrderBy(sitePage => sitePage.PageTitle)];
        }

        ItemsListViewModel itemList = new()
        {
            AllSitePages = [.. allSitePages
                     .Skip((pageNumber - 1) * DataConfig.NumberOfItemsPerPage)
                     .Take(DataConfig.NumberOfItemsPerPage)],

            #region Свойства PagingInfoViewModel

            TotalItems = allSitePages.Count,

            ItemsPerPage = DataConfig.NumberOfItemsPerPage,

            CurrentPage = pageNumber,

            SearchString = searchString ?? string.Empty,

            PageCard = pageCard

            #endregion
        };

        if (pageCard == false)
        {
            return View(itemList);
        }
        else
        {
            return View("PageCardList", itemList);
        }
    }

    #endregion

    #region Информация о странице сайта

    public async Task<IActionResult> DetailsPage(Guid? pageId)
    {
        if (pageId.HasValue
                && await pageInfoContext.PagesInfo
                    .Where(page => page.PageInfoModelId == pageId)
                    .AnyAsync())
        {
            #region Инициализация экземпляра страницы

            var pageItem = await pageInfoContext.PagesInfo
                .Include(image => image.ImageFileModel)
                .Include(text => text.TextInfo)
                .Include(background => background.BackgroundFileModel)
                .Include(audioFile => audioFile.AudioInfo)
                // TODO: убрать nullable для картинки фильма 
                .Include(movie => movie.MovieFile)
                .Include(movie => movie.MovieFile)
                .AsNoTracking()
                .FirstAsync(p => p.PageInfoModelId == pageId);

            #endregion

            #region Текстовый файл для страницы

            if (pageItem.TextInfo != null)
            {
                var textItem = await textFileContext.Texts
                    .AsNoTracking()
                    .FirstAsync(p => p.TextInfoModelId == pageItem.TextInfoId);

                using StreamReader clearText = new(rootPath + DataConfig.TextsFolderPath + textItem.FolderForText + textItem.TxtFileName);

                pageItem.ClearText = clearText.ReadToEnd();

                using StreamReader htmlText = new(rootPath + DataConfig.TextsFolderPath + textItem.FolderForText + textItem.HtmlFileName);

                pageItem.HtmlText = htmlText.ReadToEnd();
            }

            #endregion

            #region Инициализация экземпляра иконки для страницы

            if (await iconContext.IconFiles
                .Where(icon => icon.IconPath == pageItem.PageIconPath && icon.IconFileName == DataConfig.IconItem).AnyAsync())
            {
                pageItem.IconItem = await iconContext.IconFiles
                    .FirstAsync(icon => icon.IconPath == pageItem.PageIconPath && icon.IconFileName == DataConfig.IconItem);
            }
            else
            {
                pageItem.IconItem = await iconContext.IconFiles
                    .FirstAsync(icon => icon.IconPath == "main/" && icon.IconFileName == DataConfig.IconItem);
            }

            #endregion

            #region Ссылки на текущую страницу по GUID (1)

            pageItem.LinksFromPagesByGuid = [];

#pragma warning disable CA1862
            pageItem.LinksFromPagesByGuid
                .AddRange(await pageInfoContext.PagesInfo
                    .Where(p => p.RefPages != null && p.RefPages.ToLower()
                    .Contains(pageItem.PageInfoModelId.ToString().ToLower()))
                    .ToArrayAsync());
#pragma warning restore CA1862

            _ = pageItem.LinksFromPagesByGuid.Distinct().OrderBy(p => p.SortOfPage);

            #endregion

            #region Ссылки на текущую страницу по GUID (2)

            pageItem.LinksFromPagesByGuid2 = [];

#pragma warning disable CA1862
            pageItem.LinksFromPagesByGuid2
                 .AddRange(await pageInfoContext.PagesInfo
                    .Where(p => p.RefPages2 != null && p.RefPages2.ToLower().Contains(pageItem.PageInfoModelId.ToString().ToLower()))
                .ToArrayAsync());
#pragma warning restore CA1862

            _ = pageItem.LinksFromPagesByGuid2.Distinct().OrderBy(p => p.SortOfPage);

            #endregion

            #region Ссылки на текущую страницу по фильтру PageFilter

            pageItem.DictionaryOfOutPages = [];

            if (!string.IsNullOrEmpty(pageItem.PageFilter))
            {
                string[] pageFilters = pageItem.PageFilter.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pageFilters.Length > 0)
                {
                    for (int i = 0; i < pageFilters.Length; i++)
                    {
                        if (await pageInfoContext.PagesInfo.Where(p => p.PageFilter.Contains(pageFilters[i].Trim() + ',')).AnyAsync())
                        {
                            var listOfFilterOut = await pageInfoContext.PagesInfo
                                .Where(p => p.PageFilterOut != null && p.PageFilterOut.Contains(pageFilters[i].Trim() + ','))
                                .ToListAsync();

                            _ = listOfFilterOut.Distinct().OrderBy(p => p.SortOfPage);

                            pageItem.DictionaryOfOutPages[pageFilters[i].Trim()] = listOfFilterOut;
                        }
                    }
                }
            }

            #endregion

            #region Ссылки на страницы сайта по GUID1

            pageItem.LinksToPagesByGuid = [];

            if (pageItem.RefPages != null && pageItem.RefPages != string.Empty)
            {
                string[] pageIdOut = pageItem.RefPages.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pageIdOut.Length > 0)
                {
                    foreach (string refPageId in pageIdOut)
                    {
                        if (Guid.TryParse(refPageId, out Guid pageGuid))
                        {
                            if (await pageInfoContext.PagesInfo.Where(p => p.PageInfoModelId == pageGuid).AnyAsync())
                            {
                                pageItem.LinksToPagesByGuid.Add(await pageInfoContext.PagesInfo.FirstAsync(p => p.PageInfoModelId == pageGuid));

                                _ = pageItem.LinksToPagesByGuid.Distinct().OrderBy(p => p.PageCardText);
                            }
                        }
                    }
                }
            }

            #endregion

            #region Ссылки на страницы сайта по GUID2

            pageItem.LinksToPagesByGuid2 = [];

            if (pageItem.RefPages2 != null && pageItem.RefPages2 != string.Empty)
            {
                string[] pageIdOut2 = pageItem.RefPages2.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pageIdOut2.Length > 0)
                {
                    foreach (string refPageId2 in pageIdOut2)
                    {
                        if (Guid.TryParse(refPageId2, out Guid pageGuid2))
                        {
                            if (await pageInfoContext.PagesInfo.Where(p => p.PageInfoModelId == pageGuid2).AnyAsync())
                            {
                                pageItem.LinksToPagesByGuid2.Add(await pageInfoContext.PagesInfo.FirstAsync(p => p.PageInfoModelId == pageGuid2));

                                _ = pageItem.LinksToPagesByGuid2.Distinct().OrderBy(p => p.PageCardText);
                            }
                        }
                    }
                }
            }

            #endregion

            #region Ссылки на страницы сайта по фильтрам (PageFilterOut)

            pageItem.DictionaryOfLinksByPageFilterOut = [];

            if (pageItem.PageFilterOut != null && pageItem.PageFilterOut != string.Empty)
            {
                string[] pageFiltersOut = pageItem.PageFilterOut.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pageFiltersOut.Length > 0)
                {
                    for (int i = 0; i < pageFiltersOut.Length; i++)
                    {
                        if (await pageInfoContext.PagesInfo
                                .Where(p => p.PageFilter.Contains((pageFiltersOut[i] + ',').Trim()))
                                .AnyAsync())
                        {
                            var pages = await pageInfoContext.PagesInfo
                                .Where(p => p.PageFilter.Contains((pageFiltersOut[i] + ',').Trim()))
                                .ToListAsync();

                            _ = pages.OrderBy(p => p.SortOfPage);

                            pageItem.DictionaryOfLinksByPageFilterOut[pageFiltersOut[i].Trim()] = pages;
                        }
                    }
                }
            }

            #endregion

            #region  Ссылки на видео сайта по текстовому фильтру (VideoFilterOut)

            pageItem.DictionaryOfLinksByVideoFilterOut = [];

            if (pageItem.VideoFilterOut != null && pageItem.VideoFilterOut != string.Empty)
            {
                string[] videoFilterOut = pageItem.VideoFilterOut.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (videoFilterOut.Length > 0)
                {
                    for (int i = 0; i < videoFilterOut.Length; i++)
                    {
                        if (await movieContext.MovieFiles.Where(p => p.SearchFilter.Contains(videoFilterOut[i].Trim())).AnyAsync())
                        {
                            var movies = await movieContext.MovieFiles.Where(p => p.SearchFilter.Contains(videoFilterOut[i].Trim()) & p.MovieInMainList == true).ToListAsync();

                            movies.Sort((movies1, movies2) => movies1.MovieDatePublished.CompareTo(movies2.MovieDatePublished));

                            VideoLinksViewModel videoLinksViewModel = new()
                            {
                                HeadTitleForVideoLinks = videoFilterOut[i],
                                IsImage = false,
                                IconType = "webicon300",
                                SearchFilter = videoFilterOut[i],
                                MovieInMainList = true,
                                IsPartsMoreOne = true
                            };

                            pageItem.DictionaryOfLinksByVideoFilterOut[videoFilterOut[i].Trim()] = videoLinksViewModel;
                        }
                    }
                }
            }

            #endregion

            #region Группы картинок по фильтрам (PhotoFilterOut)

            pageItem.DictionaryOfLinksByFotoFilterOut = [];

            if (pageItem.PhotoFilterOut != null && pageItem.PhotoFilterOut != string.Empty)
            {
                string[] pictureFiltersOut = pageItem.PhotoFilterOut.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pictureFiltersOut.Length > 0)
                {
                    for (int i = 0; i < pictureFiltersOut.Length; i++)
                    {
                        if (await imageContext.ImageFiles.Where(img => img.SearchFilter.Contains(pictureFiltersOut[i].Trim())).AnyAsync())
                        {
                            var pictures = await imageContext.ImageFiles.Where(img => img.SearchFilter.Contains(pictureFiltersOut[i].Trim())).ToListAsync();

                            _ = pictures.OrderBy(p => p.ImageCaption);

                            pageItem.DictionaryOfLinksByFotoFilterOut[pictureFiltersOut[i].Trim()] = new();

                            // TODO Сформировать ImageListViewModel для передачи в DetailsPage
                        }
                    }
                }
            }

            #endregion

            #region Кадры слева и справа от текста

            if (pageItem.AudioInfoId != null && await audioFileContext.AudioFiles.Where(audio => audio.AudioInfoModelId == pageItem.AudioInfoId).AnyAsync())
            {
                var audioForPage = await audioFileContext.AudioFiles
                    .Include(aBook => aBook.AudioBookModel)
                    .FirstAsync(audio => audio.AudioInfoModelId == pageItem.AudioInfoId);

                if (audioForPage.AudioBookModel != null
                        && audioForPage.AudioBookModel.BookForAudioBook != null
                        && await imageContext.ImageFiles
                                       .Where(img => img.SearchFilter.Contains(audioForPage.AudioBookModel.BookForAudioBook.CaptionOfText.Replace(' ', '-') + "#album#"))
                                       .AnyAsync())
                {
                    var listOfPictures = await imageContext.ImageFiles
                       .Where(p => p.SearchFilter.Contains(audioForPage.AudioBookModel.BookForAudioBook.CaptionOfText.Replace(' ', '-') + "#album#"))
                       .ToArrayAsync(); ;

                    listOfPictures = [.. listOfPictures.Shuffle()];

                    if (listOfPictures.Length > 1)
                    {
                        pageItem.FramesAroundMainContent = new FramesAroundMainContentModel
                        {
                            FramesOnTheLeft = [.. listOfPictures.Take(listOfPictures.Length / 2)],
                            FramesOnTheRight = [.. listOfPictures.Skip(listOfPictures.Length / 2)]
                        };
                    }
                    else
                    {
                        pageItem.FramesAroundMainContent = new FramesAroundMainContentModel
                        {
                            FramesOnTheLeft = [.. listOfPictures],
                            FramesOnTheRight = [.. listOfPictures]
                        };
                    }
                }
                else
                {
                    pageItem.FramesAroundMainContent = new FramesAroundMainContentModel
                    {
                        FramesOnTheLeft = [],
                        FramesOnTheRight = []
                    };
                }
            }

            if (pageItem.OgType == "book")
            {
                if (pageItem.RoutData.Contains("bookid", StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] routData = pageItem.RoutData[1..].Split('&');

                    var dictionaryOfRoutdata = routData.Select(part => part.Split('=')).ToDictionary(split => split[0], split => split[1]);

                    if (Guid.TryParse(dictionaryOfRoutdata["bookid"], out var bookGuid))
                    {
                        if (await bookAndArticleContext.BooksAndArticles.Where(book => book.BooksAndArticlesModelId == bookGuid).AnyAsync())
                        {
                            var bookForPage = await bookAndArticleContext.BooksAndArticles.FirstAsync(book => book.BooksAndArticlesModelId == bookGuid);

#pragma warning disable CA1862
                            var imageItems = await imageContext.ImageFiles
                                                            .Where(img => img.SearchFilter.ToLower().Contains(bookForPage.CaptionOfText.ToLower()))
                                                            .ToArrayAsync();
#pragma warning restore CA1862

                            imageItems = [.. imageItems.Shuffle()];

                            if (imageItems.Length > 1)
                            {
                                pageItem.FramesAroundMainContent = new FramesAroundMainContentModel
                                {
                                    FramesOnTheLeft = [.. imageItems.Take(imageItems.Length / 2)],
                                    FramesOnTheRight = [.. imageItems.Skip(imageItems.Length / 2)]
                                };
                            }
                            else
                            {
                                pageItem.FramesAroundMainContent = new FramesAroundMainContentModel
                                {
                                    FramesOnTheLeft = [.. imageItems],
                                    FramesOnTheRight = [.. imageItems]
                                };
                            }
                        }
                    }
                }

                if (pageItem.RoutData.Contains("bookcaption", StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] routData = pageItem.RoutData[1..].Split('&');

                    var dictionaryOfRoutdata = routData.Select(part => part.Split('=')).ToDictionary(split => split[0], split => split[1]);

#pragma warning restore CA1862
                    if (await imageContext.ImageFiles
                                .Where(img => img.SearchFilter.ToLower().Contains(dictionaryOfRoutdata["bookcaption"].ToLower())).AnyAsync())
                    {
                        var imageItems = await imageContext.ImageFiles
                                    .Where(img => img.SearchFilter.ToLower().Contains(dictionaryOfRoutdata["bookcaption"].ToLower()))
                                    .ToArrayAsync();

                        imageItems = [.. imageItems.Shuffle()];

                        if (imageItems.Length > 1)
                        {
                            pageItem.FramesAroundMainContent = new FramesAroundMainContentModel
                            {
                                FramesOnTheLeft = [.. imageItems.Take(imageItems.Length / 2)],
                                FramesOnTheRight = [.. imageItems.Skip(imageItems.Length / 2)]
                            };
                        }
                        else
                        {
                            pageItem.FramesAroundMainContent = new FramesAroundMainContentModel
                            {
                                FramesOnTheLeft = [.. imageItems],
                                FramesOnTheRight = [.. imageItems]
                            };
                        }
                    }
                    else
                    {
                        pageItem.FramesAroundMainContent = new FramesAroundMainContentModel
                        {
                            FramesOnTheLeft = [],
                            FramesOnTheRight = []
                        };
                    }
#pragma warning disable CA1862
                }
            }

            #endregion

            return View(pageItem);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

    #region Добавить страницу сайта в базу данных

    [HttpGet]
    public ViewResult AddPage()
    {
        PageInfoModel newPage = new();

        // Список картинок сайта
        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

        // Список картинок для фона (фотопленка)
        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

        // Список текстовых файлов
        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

        // Список аудиофайлов
        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

        return View(newPage);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(5_268_435_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
    public async Task<IActionResult> AddPage(
        [Bind(
                "PageInfoModelId," +
                "PageIconPath," +
                "BrowserConfig," +
                "BrowserConfigFolder," +
                "Manifest," +
                "PageAsRazorPage," +
                "ImageFileModelId," +
                "ImageFileFormFile," +
                "BackgroundFileModelId," +
                "BackgroundFormFile," +
                "AudioInfoId," +
                "AudioInfoFormFile," +
                "TextInfoId," +
                "TextFileFormFile," +
                "PageCardText," +
                "SortOfPage," +
                "PageArea," +
                "Controller," +
                "Action," +
                "RoutData," +
                "PageLoc," +
                "PagePathNickName," +
                "PagePathNickName2," +
                "PageTitle," +
                "PageDescription," +
                "PageKeyWords," +
                "OgType," +
                "PageIconPath," +
                "BrowserConfig," +
                "BrowserConfigFolder," +
                "Manifest," +
                "PageLastmod," +
                "Changefreq," +
                "Priority," +
                "PageHeading," +
                "ImagePageHeadingFormFile," +
                "ImagePageHeadingId," +
                "PageFilter," +
                "TextOfPage," +
                "PageFilterOut," +
                "PageLinksByFilters," +
                "VideoFilterOut," +
                "VideoLinks," +
                "RefPages," +
                "PageLinks," +
                "RefPages2," +
                "PageLinks2," +
                "PhotoLinks," +
                "PhotoFilterOut"
        )]
        PageInfoModel addPage)
    {
        if (ModelState.IsValid)
        {
            #region Добавить ссылку на текстовый файл

            if (addPage.TextInfoId != Guid.Empty & addPage.TextFileFormFile == null)
            {
                _ = addPage.TextInfoId;
            }
            else if (addPage.TextInfoId == Guid.Empty & addPage.TextFileFormFile == null)
            {
                addPage.TextInfoId = null;
            }
            else
            {
                if (addPage.TextFileFormFile != null)
                {
                    if (!(addPage.TextFileFormFile.FileName.EndsWith(".html") || addPage.TextFileFormFile.FileName.EndsWith(".txt")))
                    {
                        ModelState.AddModelError("TextFileFormFile", $"Выбран некорректный файл «{addPage.TextFileFormFile.FileName}»");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(addPage);
                    }

                    if (await textFileContext.Texts.Where(textFile => textFile.HtmlFileName == addPage.TextFileFormFile.FileName
                                                                        || textFile.TxtFileName == addPage.TextFileFormFile.FileName).AnyAsync())
                    {
                        var newTextFile = await textFileContext.Texts.FirstAsync(textFile => textFile.HtmlFileName == addPage.TextFileFormFile.FileName
                                                                                                        || textFile.TxtFileName == addPage.TextFileFormFile.FileName);

                        addPage.TextInfoId = newTextFile.TextInfoModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("TextFileFormFile", $"Добавьте текстовый файл «{addPage.TextFileFormFile.FileName}» в базу данных");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(addPage);
                    }
                }
                else
                {
                    addPage.TextInfoId = null;
                }
            }

            #endregion
            
            #region Индекс сортировки страницы

            _ = addPage.SortOfPage;

            #endregion

            #region Title-Description-KeyWords

            if (addPage.TextFileFormFile != null)
            {
                if (addPage.TextFileFormFile.FileName.Contains("sledstvie-prodoljaetsya-", StringComparison.InvariantCultureIgnoreCase))
                {
                    addPage.PageTitle = "Николай Модестов «Следствие продолжается» страница " + (addPage.SortOfPage - 1).ToString();

                    addPage.PageDescription = addPage.PageTitle + ".";

                    addPage.PageKeyWords = "Следствие продолжается, Николай Модестов, криминал, 90-е годы,";
                }
            }
            else
            {
                _ = addPage.PageTitle.Trim();
                _ = addPage.PageDescription.Trim();
                _ = addPage.PageKeyWords.Trim();
            }

            #endregion

            #region MVC или RazorPage

            _ = addPage.PageAsRazorPage;

            #endregion

            #region Добавить картинку для страницы

            if (addPage.TextFileFormFile != null)
            {
                if (addPage.TextFileFormFile.FileName.Contains("sledstvie-prodoljaetsya-", StringComparison.InvariantCultureIgnoreCase))
                {
                    addPage.ImageFileModelId = new("c131c3fa-5d0b-4424-7e33-08de5b2ee0af");
                }
            }
            else if (addPage.ImageFileModelId != Guid.Empty)
            {
                _ = addPage.ImageFileModelId;
            }
            else
            {
                if (addPage.ImageFileFormFile != null)
                {
                    if (!(addPage.ImageFileFormFile.FileName.EndsWith(".webp") || addPage.ImageFileFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("ImageFileFormFile", $"Выбран некорректный файл «{addPage.ImageFileFormFile.FileName}»");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(addPage);
                    }

                    if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == addPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == addPage.ImageFileFormFile.FileName);

                        addPage.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == addPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == addPage.ImageFileFormFile.FileName);

                        addPage.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == addPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == addPage.ImageFileFormFile.FileName);

                        addPage.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == addPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == addPage.ImageFileFormFile.FileName);

                        addPage.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == addPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == addPage.ImageFileFormFile.FileName);

                        addPage.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.ImageFileName == addPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == addPage.ImageFileFormFile.FileName);

                        addPage.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == addPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == addPage.ImageFileFormFile.FileName);

                        addPage.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.IconFileName == addPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == addPage.ImageFileFormFile.FileName);

                        addPage.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == addPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == addPage.ImageFileFormFile.FileName);

                        addPage.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == addPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == addPage.ImageFileFormFile.FileName);

                        addPage.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("ImageFileFormFile", $"Добавьте картинку «{addPage.ImageFileFormFile.FileName}» в базу данных");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(addPage);
                    }
                }
                else
                {
                    ModelState.AddModelError("ImageFileFormFile", $"Выберите файл картинки");

                    ModelState.AddModelError("ImageFileModelId", $"Выберите картинку");

                    // Список картинок сайта
                    ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                    // Список картинок для фона (фотопленка)
                    ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                    // Список текстовых файлов
                    ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                    // Список аудиофайлов
                    ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                    return View(addPage);
                }
            }

            #endregion

            #region Добавить фон для страницы

            if (addPage.TextFileFormFile != null)
            {
                if (addPage.TextFileFormFile.FileName.Contains("sledstvie-prodoljaetsya-", StringComparison.InvariantCultureIgnoreCase))
                {
                    addPage.BackgroundFileModelId = new("9DCC0265-DF8B-4678-A66B-7006EA116E5A");
                }
            }
            else if (addPage.BackgroundFileModelId != Guid.Empty)
            {
                _ = addPage.BackgroundFileModelId;
            }
            else
            {
                if (addPage.BackgroundFormFile != null)
                {
                    if (!(addPage.BackgroundFormFile.FileName.EndsWith(".webp") || addPage.BackgroundFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("BackgroundFormFile", $"Выбран некорректный файл «{addPage.BackgroundFormFile.FileName}»");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(addPage);
                    }

                    if (await backgroundContext.BackgroundFiles.Where(bk => bk.WebLeftBackground == addPage.BackgroundFormFile.FileName).AnyAsync())
                    {
                        var newBackground = await backgroundContext.BackgroundFiles.FirstAsync(bk => bk.WebLeftBackground == addPage.BackgroundFormFile.FileName);

                        addPage.BackgroundFileModelId = newBackground.BackgroundFileModelId;
                    }
                    else if (await backgroundContext.BackgroundFiles.Where(bk => bk.WebRightBackground == addPage.BackgroundFormFile.FileName).AnyAsync())
                    {
                        var newBackground = await backgroundContext.BackgroundFiles.FirstAsync(bk => bk.WebRightBackground == addPage.BackgroundFormFile.FileName);

                        addPage.BackgroundFileModelId = newBackground.BackgroundFileModelId;
                    }
                    else if (await backgroundContext.BackgroundFiles.Where(bk => bk.LeftBackground == addPage.BackgroundFormFile.FileName).AnyAsync())
                    {
                        var newBackground = await backgroundContext.BackgroundFiles.FirstAsync(bk => bk.LeftBackground == addPage.BackgroundFormFile.FileName);

                        addPage.BackgroundFileModelId = newBackground.BackgroundFileModelId;
                    }
                    else if (await backgroundContext.BackgroundFiles.Where(bk => bk.RightBackground == addPage.BackgroundFormFile.FileName).AnyAsync())
                    {
                        var newBackground = await backgroundContext.BackgroundFiles.FirstAsync(bk => bk.RightBackground == addPage.BackgroundFormFile.FileName);

                        addPage.BackgroundFileModelId = newBackground.BackgroundFileModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("BackgroundFormFile", $"Добавьте фон «{addPage.BackgroundFormFile.FileName}» в базу данных");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(addPage);
                    }
                }
                else
                {
                    ModelState.AddModelError("BackgroundFormFile", $"Выберите файл фона");

                    ModelState.AddModelError("BackgroundFileModelId", $"Выберите фон");

                    // Список картинок сайта
                    ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                    // Список картинок для фона (фотопленка)
                    ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                    // Список текстовых файлов
                    ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                    // Список аудиофайлов
                    ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                    return View(addPage);
                }
            }

            #endregion

            #region Добавить аудиофайл

            if (addPage.AudioInfoId != Guid.Empty)
            {
                _ = addPage.AudioInfoId;
            }
            else
            {
                if (addPage.AudioInfoFormFile != null)
                {
                    if (!addPage.AudioInfoFormFile.FileName.EndsWith(".mp3"))
                    {
                        ModelState.AddModelError("AudioInfoFormFile", $"Выбран некорректный файл «{addPage.AudioInfoFormFile.FileName}»");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(addPage);
                    }

                    if (await audioFileContext.AudioFiles.Where(audioFile => audioFile.AudioFileName == addPage.AudioInfoFormFile.FileName).AnyAsync())
                    {
                        var newAudioFile = await audioFileContext.AudioFiles.FirstAsync(audioFile => audioFile.AudioFileName == addPage.AudioInfoFormFile.FileName);

                        addPage.AudioInfoId = newAudioFile.AudioInfoModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("AudioInfoFormFile", $"Добавьте аудиофайл «{addPage.AudioInfoFormFile.FileName}» в базу данных");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(addPage);
                    }
                }
                else
                {
                    addPage.AudioInfoId = null;
                }
            }

            #endregion

            #region Текст карточки страницы

            if (addPage.TextFileFormFile != null)
            {
                if (addPage.TextFileFormFile.FileName.Contains("sledstvie-prodoljaetsya-", StringComparison.InvariantCultureIgnoreCase))
                {
                    addPage.PageCardText = "СТРАНИЦА " + (addPage.SortOfPage - 1).ToString();
                }
            }
            else
            {
                addPage.PageCardText = addPage.PageCardText.Trim().ToUpper();
            }

            #endregion

            #region Адрес

            #region Область

            if (string.IsNullOrEmpty(addPage.PageArea.Trim()) || addPage.PageArea == "Root")
            {
                addPage.PageArea = string.Empty;
            }
            else
            {
                addPage.PageArea = "/" + addPage.PageArea.Trim().Trim('/').ToLower();
            }

            #endregion

            #region Контроллер

            if (addPage.PageAsRazorPage)
            {
                addPage.Controller = string.Empty;
            }
            else
            {
                if (addPage.PageTitle.Contains("Следствие продолжается", StringComparison.InvariantCultureIgnoreCase))
                {
                    addPage.Controller = "/books";
                }
                else
                {
                    addPage.Controller = "/" + addPage.Controller.Trim().Trim('/').ToLower();
                }
            }

            #endregion

            #region Действие

            if (addPage.PageAsRazorPage)
            {
                addPage.Action = string.Empty;
            }
            else
            {
                if (addPage.PageTitle.Contains("Следствие продолжается", StringComparison.InvariantCultureIgnoreCase))
                {
                    addPage.Action = "/book";
                }
                else
                {
                    addPage.Action = "/" + addPage.Action.Trim().Trim('/').ToLower();
                }
            }

            #endregion

            #region Адрес без Области (для RazorPage)

            if (addPage.PageAsRazorPage)
            {
                if (string.IsNullOrWhiteSpace(addPage.PageLoc) || string.IsNullOrEmpty(addPage.PageLoc))
                {
                    ModelState.AddModelError("PageItem.PageLoc", "Введите адрес страницы без области");

                    // Список картинок сайта
                    ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                    // Список картинок для фона (фотопленка)
                    ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                    // Список текстовых файлов
                    ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                    // Список аудиофайлов
                    ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                    return View(addPage);
                }
                else if (addPage.PageLoc == "/")
                {
                    addPage.PageLoc = "/";
                }
                else
                {
                    addPage.PageLoc = "/" + addPage.PageLoc.Trim().Trim('/').TrimStart('?').ToLower();
                }
            }
            else
            {
                addPage.PageLoc = addPage.Action;
            }

            #endregion

            #region Данные (RoutData)

            if (addPage.PageTitle.Contains("Следствие продолжается", StringComparison.InvariantCultureIgnoreCase))
            {
                addPage.RoutData = "?bookcaption=следствие-продолжается&pagenumber=" + (addPage.SortOfPage - 1).ToString();
            }
            else if (string.IsNullOrWhiteSpace(addPage.RoutData) || string.IsNullOrEmpty(addPage.RoutData))
            {
                addPage.RoutData = string.Empty;
            }
            else
            {
                addPage.RoutData = "?" + addPage.RoutData.Trim().Trim('/').TrimStart('?').ToLower();
            }

            #endregion

            #region Псевдоним страницы (1)

            if (addPage.PageTitle.Contains("Следствие продолжается", StringComparison.InvariantCultureIgnoreCase))
            {
                addPage.PagePathNickName = "/%D0%BA%D0%BD%D0%B8%D0%B3%D0%B0/%D1%81%D0%BB%D0%B5%D0%B4%D1%81%D1%82%D0%B2%D0%B8%D0%B5-%D0%BF%D1%80%D0%BE%D0%B4%D0%BE%D0%BB%D0%B6%D0%B0%D0%B5%D1%82%D1%81%D1%8F/%D1%81%D1%82%D1%80%D0%B0%D0%BD%D0%B8%D1%86%D0%B0-" + (addPage.SortOfPage - 1).ToString();
            }
            else if (string.IsNullOrWhiteSpace(addPage.PagePathNickName) || string.IsNullOrEmpty(addPage.PagePathNickName))
            {
                addPage.PagePathNickName = string.Empty;
            }
            else if (addPage.PagePathNickName == "/")
            {
                addPage.PagePathNickName = "/";
            }
            else
            {
                addPage.PagePathNickName = "/" + addPage.PagePathNickName.Trim().Trim('/').ToLower();
            }

            #endregion

            #region Псевдоним страницы (2)

            if (string.IsNullOrWhiteSpace(addPage.PagePathNickName2) || string.IsNullOrEmpty(addPage.PagePathNickName2))
            {
                addPage.PagePathNickName2 = string.Empty;
            }
            else if (addPage.PagePathNickName2 == "/")
            {
                addPage.PagePathNickName2 = "/";
            }
            else
            {
                addPage.PagePathNickName2 = "/" + addPage.PagePathNickName2.Trim().Trim('/').ToLower();
            }

            #endregion

            #region Проверка существующих страниц

            string checkPageFullPathWithData = string.Empty;

            if (addPage.PageAsRazorPage)
            {
                checkPageFullPathWithData = addPage.PageArea + addPage.PageLoc + addPage.RoutData;

                if (await pageInfoContext.PagesInfo.Where(p => p.PageFullPathWithData == checkPageFullPathWithData).AnyAsync())
                {
                    ModelState.AddModelError("pageItem.PageLoc", $"Страница «{addPage.PageArea + addPage.PageLoc + addPage.RoutData}» уже существует");

                    // Список картинок сайта
                    ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                    // Список картинок для фона (фотопленка)
                    ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                    // Список текстовых файлов
                    ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                    // Список аудиофайлов
                    ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                    return View(addPage);
                }
            }
            else
            {
                checkPageFullPathWithData = addPage.PageArea + addPage.Controller + addPage.Action + addPage.RoutData;

                if (await pageInfoContext.PagesInfo.Where(p => p.PageFullPathWithData == checkPageFullPathWithData).AnyAsync())
                {
                    ModelState.AddModelError("pageItem.PageLoc", $"Страница «{addPage.PageArea + addPage.Controller + addPage.Action + addPage.RoutData}» уже существует");

                    return View(addPage);
                }
            }

            if (!string.IsNullOrEmpty(addPage.PagePathNickName) && await pageInfoContext.PagesInfo.Where(p => p.PagePathNickName == addPage.PagePathNickName).AnyAsync())
            {
                ModelState.AddModelError("pageItem.PagePathNickName", $"Страница с псевдонимом «{addPage.PagePathNickName}» уже существует");

                return View(addPage);
            }

            #endregion

            #endregion

            #region OgType - PageIconPath - BrowserConfig - BrowserConfigFolder - Manifest

            if (addPage.PageTitle.Contains("Следствие продолжается", StringComparison.InvariantCultureIgnoreCase))
            {
                addPage.OgType = "book";
            }

            _ = addPage.OgType.Trim();

            if (addPage.OgType == "website")
            {
                addPage.PageIconPath = "main/";
                addPage.BrowserConfig = "main.xml";
                addPage.BrowserConfigFolder = "/main";
                addPage.Manifest = "main.json";
            }
            else if (addPage.OgType == "movie")
            {
                addPage.PageIconPath = "movie/";
                addPage.BrowserConfig = "movie.xml";
                addPage.BrowserConfigFolder = "/movie";
                addPage.Manifest = "movie.json";
            }
            else
            {
                addPage.PageIconPath = "main/";
                addPage.BrowserConfig = "main.xml";
                addPage.BrowserConfigFolder = "/main";
                addPage.Manifest = "main.json";
            }

            if (addPage.PageArea == "admin")
            {
                addPage.PageIconPath = "admin/";
                addPage.BrowserConfig = "admin.xml";
                addPage.BrowserConfigFolder = "/admin";
                addPage.Manifest = "admin.json";
            }

            #endregion

            #region Данные для Sitemap

            addPage.PageLastmod = DateTime.Now;

            _ = addPage.Changefreq.Trim();

            _ = addPage.Priority.Trim();

            #endregion

            #region Содержание страницы (заголовок, картинка, текст)

            #region Оформление заголовка страницы

            _ = addPage.PageHeading.Trim();

            #endregion

            #region Добавить картинку для  заголовка страницы

            if (addPage.ImagePageHeadingId != Guid.Empty)
            {
                _ = addPage.ImagePageHeadingId;
            }
            else
            {
                if (addPage.ImagePageHeadingFormFile != null)
                {
                    if (!(addPage.ImagePageHeadingFormFile.FileName.EndsWith(".webp") || addPage.ImagePageHeadingFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("ImagePageHeadingFormFile", $"Выбран некорректный файл «{addPage.ImagePageHeadingFormFile.FileName}»");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(addPage);
                    }

                    if (addPage.ImagePageHeadingFormFile != null)
                    {
                        if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == addPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == addPage.ImagePageHeadingFormFile.FileName);

                            addPage.ImagePageHeadingId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == addPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == addPage.ImagePageHeadingFormFile.FileName);

                            addPage.ImagePageHeadingId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == addPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == addPage.ImagePageHeadingFormFile.FileName);

                            addPage.ImagePageHeadingId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == addPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == addPage.ImagePageHeadingFormFile.FileName);

                            addPage.ImagePageHeadingId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == addPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == addPage.ImagePageHeadingFormFile.FileName);

                            addPage.ImagePageHeadingId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageFileName == addPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == addPage.ImagePageHeadingFormFile.FileName);

                            addPage.ImagePageHeadingId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.IconFileName == addPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == addPage.ImagePageHeadingFormFile.FileName);

                            addPage.ImagePageHeadingId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == addPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == addPage.ImagePageHeadingFormFile.FileName);

                            addPage.ImagePageHeadingId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == addPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == addPage.ImagePageHeadingFormFile.FileName);

                            addPage.ImagePageHeadingId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == addPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == addPage.ImagePageHeadingFormFile.FileName);

                            addPage.ImagePageHeadingId = imageFile.ImageFileModelId;
                        }
                        else
                        {
                            ModelState.AddModelError("ImagePageHeadingFormFile", $"Добавьте картинку «{addPage.ImagePageHeadingFormFile.FileName}» в базу данных");

                            // Список картинок сайта
                            ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                            // Список картинок для фона (фотопленка)
                            ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                            // Список текстовых файлов
                            ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                            // Список аудиофайлов
                            ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                            return View(addPage);
                        }
                    }
                }
                else
                {
                    addPage.ImagePageHeadingId = null;
                }
            }

            #endregion

            #region Текст страницы

            _ = addPage.TextOfPage;

            #endregion

            #endregion

            #region Фильтр поиска текущей страницы

            _ = addPage.PageFilter.Trim();

            #endregion

            #region Поиск связанных страниц

            _ = addPage.PageLinksByFilters;

            if (addPage.PageTitle.Contains("Следствие продолжается", StringComparison.InvariantCultureIgnoreCase))
            {
                addPage.PageFilterOut = "Книги Николая Модестова, Книги о Сергее Шевкуненко,";

                addPage.PageLinksByFilters = true;
            }

            if (addPage.PageFilterOut != null)
            {
                addPage.PageFilterOut = addPage.PageFilterOut.Trim();
            }
            else
            {
                addPage.PageFilterOut = null;
            }

            #endregion

            #region Поиск связанных видео

            _ = addPage.VideoLinks;

            if (addPage.VideoFilterOut != null)
            {
                _ = addPage.VideoFilterOut.Trim();
            }
            else
            {
                addPage.VideoFilterOut = null;
            }

            #endregion

            #region Поиск связанных страниц по GUID(1)

            _ = addPage.PageLinks;

            if (addPage.RefPages != null)
            {
                _ = addPage.RefPages.Trim();
            }
            else
            {
                addPage.RefPages = null;
            }

            #endregion

            #region Поиск связанных страниц по GUID(2)

            _ = addPage.PageLinks2;

            if (addPage.RefPages2 != null)
            {
                _ = addPage.RefPages2.Trim();
            }
            else
            {
                addPage.RefPages2 = null;
            }

            #endregion

            #region Альбом связанных картинок

            _ = addPage.PhotoLinks;

            if (addPage.PhotoFilterOut != null)
            {
                _ = addPage.PhotoFilterOut.Trim();
            }
            else
            {
                addPage.PhotoFilterOut = null;
            }

            #endregion

            #region Сохранить в базе данных

            await pageInfoContext.AddNewPageAsync(addPage);

            #endregion

            #region Открытие страницы DetailsPage

            PageInfoModel newPage = await pageInfoContext.PagesInfo.FirstAsync(p => p.PageFullPathWithData == checkPageFullPathWithData);

            return RedirectToAction("DetailsPage", new { pageId = newPage.PageInfoModelId, Area = "Admin" });

            #endregion
        }
        else
        {
            // Список картинок сайта
            ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

            // Список картинок для фона (фотопленка)
            ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

            // Список текстовых файлов
            ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

            // Список аудиофайлов
            ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

            return View(addPage);
        }
    }

    #endregion

    #region Изменить страницу в базе данных

    [HttpGet]
    public async Task<IActionResult> EditPage(Guid? pageId)
    {
        PageInfoModel editPage = new();

        if (pageId.HasValue
            && await pageInfoContext.PagesInfo
                .Where(page => page.PageInfoModelId == pageId)
                .AnyAsync())
        {
            #region Инициализация экземпляра страницы

            editPage = await pageInfoContext.PagesInfo
                .Include(image => image.ImageFileModel)
                .Include(text => text.TextInfo).ThenInclude(book => book != null ? book.BooksAndArticlesModel : null)
                .Include(background => background.BackgroundFileModel)
                .Include(audioFile => audioFile.AudioInfo)
                .Include(movie => movie.MovieFile).ThenInclude(movieImage => movieImage != null ? movieImage.ImageFileModel : null)
                .Include(movie => movie.MovieFile).ThenInclude(moviePoster => moviePoster != null ? moviePoster.MoviePoster : null)
                .FirstAsync(i => i.PageInfoModelId == pageId);

            #endregion

            #region Инициализация иконки страницы

            if (await iconContext.IconFiles
                .Where(icon => icon.IconPath == editPage.PageIconPath && icon.IconFileName == DataConfig.IconItem)
                .AnyAsync())
            {
                editPage.IconItem = await iconContext.IconFiles
                    .FirstAsync(icon => icon.IconPath == editPage.PageIconPath && icon.IconFileName == DataConfig.IconItem);
            }
            else
            {
                editPage.IconItem = await iconContext.IconFiles
                    .FirstAsync(icon => icon.IconPath == "main/" && icon.IconFileName == DataConfig.IconItem);
            }

            #endregion

            #region Ссылки на текущую страницу по GUID (1)

            List<PageInfoModel> linksFromPagesByGuid = [];

#pragma warning disable CA1862
            linksFromPagesByGuid
                .AddRange(await pageInfoContext.PagesInfo
                    .Where(p => p.RefPages != null && p.RefPages.ToLower().Contains(editPage.PageInfoModelId.ToString().ToLower()))
                    .ToArrayAsync());
#pragma warning restore CA1862

            _ = linksFromPagesByGuid.Distinct().OrderBy(p => p.SortOfPage);

            #endregion

            #region Ссылки на текущую страницу по GUID (2)

            List<PageInfoModel> linksFromPagesByGuid2 = [];

#pragma warning disable CA1862
            linksFromPagesByGuid2
                .AddRange(await pageInfoContext.PagesInfo
                    .Where(p => p.RefPages2 != null && p.RefPages2.ToLower().Contains(editPage.PageInfoModelId.ToString().ToLower()))
                .ToArrayAsync());
#pragma warning restore CA1862

            _ = linksFromPagesByGuid2.Distinct().OrderBy(p => p.SortOfPage);

            #endregion

            #region Ссылки на текущую страницу по фильтру PageFilter

            string[] pageFilters = editPage.PageFilter.Split(',', StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, List<PageInfoModel>> dictionaryOfOutPages = [];

            if (pageFilters.Length > 0)
            {
                for (int i = 0; i < pageFilters.Length; i++)
                {
                    if (await pageInfoContext.PagesInfo.Where(p => p.PageFilter.Contains(pageFilters[i] + ',')).AnyAsync())
                    {
                        var listOfFilterOut = await pageInfoContext.PagesInfo.Where(p => p.PageFilterOut != null && p.PageFilterOut.Contains(pageFilters[i] + ',')).ToListAsync();

                        _ = listOfFilterOut.Distinct().OrderBy(p => p.SortOfPage);

                        dictionaryOfOutPages[pageFilters[i]] = listOfFilterOut;

                        editPage.DictionaryOfOutPages = dictionaryOfOutPages;
                    }
                }
            }

            #endregion

            #region Ссылки на страницы сайта по фильтрам (PageFilterOut)

            Dictionary<string, List<PageInfoModel>> dictionaryOfLinksByPageFilterOut = [];

            if (editPage.PageFilterOut != null && editPage.PageFilterOut != string.Empty)
            {
                string[] pageFiltersOut = editPage.PageFilterOut.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pageFiltersOut.Length > 0)
                {
                    for (int i = 0; i < pageFiltersOut.Length; i++)
                    {
                        if (await pageInfoContext.PagesInfo.Where(p => p.PageFilter.Contains(pageFiltersOut[i].Trim())).AnyAsync())
                        {
                            var pages = await pageInfoContext.PagesInfo.Where(p => p.PageFilter.Contains(pageFiltersOut[i].Trim())).ToListAsync();

                            _ = pages.OrderBy(p => p.SortOfPage);

                            dictionaryOfLinksByPageFilterOut[pageFiltersOut[i].Trim()] = pages;

                            editPage.DictionaryOfLinksByPageFilterOut = dictionaryOfLinksByPageFilterOut;
                        }
                    }
                }
            }

            #endregion

            #region Ссылки на страницы сайта по GUID1

            List<PageInfoModel> linksToPagesByGuid = [];

            if (editPage.RefPages != null && editPage.RefPages != string.Empty)
            {
                string[] pageIdOut = editPage.RefPages.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pageIdOut.Length > 0)
                {
                    foreach (string refPageId in pageIdOut)
                    {
                        if (Guid.TryParse(refPageId, out Guid pageGuid))
                        {
                            if (await pageInfoContext.PagesInfo.Where(p => p.PageInfoModelId == pageGuid).AnyAsync())
                            {
                                linksToPagesByGuid.Add(await pageInfoContext.PagesInfo.FirstAsync(p => p.PageInfoModelId == pageGuid));

                                _ = linksToPagesByGuid.Distinct().OrderBy(p => p.PageCardText);

                                editPage.LinksToPagesByGuid = linksToPagesByGuid;
                            }
                        }
                    }
                }
            }

            #endregion

            #region Ссылки на страницы сайта по GUID2

            List<PageInfoModel> linksToPagesByGuid2 = [];

            if (editPage.RefPages2 != null && editPage.RefPages2 != string.Empty)
            {
                string[] pageIdOut2 = editPage.RefPages2.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pageIdOut2.Length > 0)
                {
                    foreach (string refPageId2 in pageIdOut2)
                    {
                        if (Guid.TryParse(refPageId2, out Guid pageGuid2))
                        {
                            if (await pageInfoContext.PagesInfo.Where(p => p.PageInfoModelId == pageGuid2).AnyAsync())
                            {
                                linksToPagesByGuid2.Add(await pageInfoContext.PagesInfo.FirstAsync(p => p.PageInfoModelId == pageGuid2));

                                _ = linksToPagesByGuid2.Distinct().OrderBy(p => p.PageCardText);

                                editPage.LinksToPagesByGuid2 = linksToPagesByGuid2;
                            }
                        }
                    }
                }
            }

            #endregion

            #region  Ссылки на видео сайта по текстовому фильтру (VideoFilterOut)

            Dictionary<string, VideoLinksViewModel> dictionaryOfLinksByVideoFilterOut = [];

            if (editPage.VideoFilterOut != null && editPage.VideoFilterOut != string.Empty)
            {
                string[] videoFilterOut = editPage.VideoFilterOut.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (videoFilterOut.Length > 0)
                {
                    for (int i = 0; i < videoFilterOut.Length; i++)
                    {
                        if (await movieContext.MovieFiles.Where(p => p.SearchFilter.Contains(videoFilterOut[i])).AnyAsync())
                        {
                            var movies = await movieContext.MovieFiles.Where(p => p.SearchFilter.Contains(videoFilterOut[i]) & p.MovieInMainList == true).ToListAsync();

                            movies.Sort((movies1, movies2) => movies1.MovieDatePublished.CompareTo(movies2.MovieDatePublished));

                            VideoLinksViewModel videoLinksViewModel = new()
                            {
                                HeadTitleForVideoLinks = videoFilterOut[i],
                                IsImage = false,
                                IconType = "webicon300",
                                SearchFilter = videoFilterOut[i],
                                MovieInMainList = true,
                                IsPartsMoreOne = true
                            };

                            dictionaryOfLinksByVideoFilterOut[videoFilterOut[i]] = videoLinksViewModel;

                            editPage.DictionaryOfLinksByVideoFilterOut = dictionaryOfLinksByVideoFilterOut;
                        }
                    }
                }
            }

            #endregion

            #region Группы картинок по фильтрам (PhotoFilterOut)

            Dictionary<string, ImageListViewModel> dictionaryOfLinksByFotoFilterOut = [];

            if (editPage.PhotoFilterOut != null && editPage.PhotoFilterOut != string.Empty)
            {
                string[] pictureFiltersOut = editPage.PhotoFilterOut.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pictureFiltersOut.Length > 0)
                {
                    for (int i = 0; i < pictureFiltersOut.Length; i++)
                    {
                        if (await imageContext.ImageFiles.Where(img => img.SearchFilter.Contains(pictureFiltersOut[i])).AnyAsync())
                        {
                            var pictures = await imageContext.ImageFiles.Where(img => img.SearchFilter.Contains(pictureFiltersOut[i])).ToListAsync();

                            _ = pictures.OrderBy(p => p.ImageCaption);

                            dictionaryOfLinksByFotoFilterOut[pictureFiltersOut[i]] = new();

                            // TODO Сформировать ImageListViewModel для передачи в EditPage
                        }
                    }
                }
            }

            #endregion

            #region Кадры слева и справа от текста

            if (editPage.AudioInfoId != null && await audioFileContext.AudioFiles.Where(audio => audio.AudioInfoModelId == editPage.AudioInfoId).AnyAsync())
            {
                var audioForPage = await audioFileContext.AudioFiles
                    .Include(aBook => aBook.AudioBookModel)
                    .FirstAsync(audio => audio.AudioInfoModelId == editPage.AudioInfoId);

                if (audioForPage.AudioBookModel != null
                        && audioForPage.AudioBookModel.BookForAudioBook != null
                        && await imageContext.ImageFiles
                                       .Where(img => img.SearchFilter.Contains(audioForPage.AudioBookModel.BookForAudioBook.CaptionOfText.Replace(' ', '-') + "#album#"))
                                       .AnyAsync())
                {
                    var listOfPictures = await imageContext.ImageFiles
                       .Where(p => p.SearchFilter.Contains(audioForPage.AudioBookModel.BookForAudioBook.CaptionOfText.Replace(' ', '-') + "#album#"))
                       .ToArrayAsync(); ;

                    listOfPictures = [.. listOfPictures.Shuffle()];

                    if (listOfPictures.Length > 1)
                    {
                        editPage.FramesAroundMainContent = new FramesAroundMainContentModel
                        {
                            FramesOnTheLeft = [.. listOfPictures.Take(listOfPictures.Length / 2)],
                            FramesOnTheRight = [.. listOfPictures.Skip(listOfPictures.Length / 2)]
                        };
                    }
                    else
                    {
                        editPage.FramesAroundMainContent = new FramesAroundMainContentModel
                        {
                            FramesOnTheLeft = [.. listOfPictures],
                            FramesOnTheRight = [.. listOfPictures]
                        };
                    }
                }
                else
                {
                    editPage.FramesAroundMainContent = new FramesAroundMainContentModel
                    {
                        FramesOnTheLeft = [],
                        FramesOnTheRight = []
                    };
                }
            }

            if (editPage.OgType == "book")
            {
                if (editPage.RoutData.Contains("bookid", StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] routData = editPage.RoutData[1..].Split('&');

                    var dictionaryOfRoutdata = routData.Select(part => part.Split('=')).ToDictionary(split => split[0], split => split[1]);

                    if (Guid.TryParse(dictionaryOfRoutdata["bookid"], out var bookGuid))
                    {
                        if (await bookAndArticleContext.BooksAndArticles.Where(book => book.BooksAndArticlesModelId == bookGuid).AnyAsync())
                        {
                            var bookForPage = await bookAndArticleContext.BooksAndArticles.FirstAsync(book => book.BooksAndArticlesModelId == bookGuid);

#pragma warning disable CA1862
                            var imageItems = await imageContext.ImageFiles
                                                            .Where(img => img.SearchFilter.ToLower().Contains(bookForPage.CaptionOfText.ToLower()))
                                                            .ToArrayAsync();
#pragma warning restore CA1862

                            imageItems = [.. imageItems.Shuffle()];

                            if (imageItems.Length > 1)
                            {
                                editPage.FramesAroundMainContent = new FramesAroundMainContentModel
                                {
                                    FramesOnTheLeft = [.. imageItems.Take(imageItems.Length / 2)],
                                    FramesOnTheRight = [.. imageItems.Skip(imageItems.Length / 2)]
                                };
                            }
                            else
                            {
                                editPage.FramesAroundMainContent = new FramesAroundMainContentModel
                                {
                                    FramesOnTheLeft = [.. imageItems],
                                    FramesOnTheRight = [.. imageItems]
                                };
                            }
                        }
                    }
                }

                if (editPage.RoutData.Contains("bookcaption", StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] routData = editPage.RoutData[1..].Split('&');

                    var dictionaryOfRoutdata = routData.Select(part => part.Split('=')).ToDictionary(split => split[0], split => split[1]);

#pragma warning disable CA1862
                    var imageItems = await imageContext.ImageFiles
                                .Where(img => img.SearchFilter.ToLower().Contains(dictionaryOfRoutdata["bookcaption"].ToLower()))
                                .ToArrayAsync();
#pragma warning restore CA1862

                    imageItems = [.. imageItems.Shuffle()];

                    if (imageItems.Length > 1)
                    {
                        editPage.FramesAroundMainContent = new FramesAroundMainContentModel
                        {
                            FramesOnTheLeft = [.. imageItems.Take(imageItems.Length / 2)],
                            FramesOnTheRight = [.. imageItems.Skip(imageItems.Length / 2)]
                        };
                    }
                    else
                    {
                        editPage.FramesAroundMainContent = new FramesAroundMainContentModel
                        {
                            FramesOnTheLeft = [.. imageItems],
                            FramesOnTheRight = [.. imageItems]
                        };
                    }
                }
            }

            #endregion

            // Список картинок сайта
            ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

            // Список картинок для фона (фотопленка)
            ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

            // Список текстовых файлов
            ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

            // Список аудиофайлов
            ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

            return View(editPage);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(5_268_435_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPage(
        [Bind(
                "PageInfoModelId," +
                "PageIconPath," +
                "BrowserConfig," +
                "BrowserConfigFolder," +
                "Manifest," +
                "PageAsRazorPage," +
                "ImageFileModelId," +
                "ImageFileFormFile," +
                "BackgroundFileModelId," +
                "BackgroundFormFile," +
                "AudioInfoId," +
                "AudioInfoFormFile," +
                "TextInfoId," +
                "TextFileFormFile," +
                "PageCardText," +
                "SortOfPage," +
                "PageArea," +
                "Controller," +
                "Action," +
                "RoutData," +
                "PageLoc," +
                "PagePathNickName," +
                "PagePathNickName2," +
                "PageTitle," +
                "PageDescription," +
                "PageKeyWords," +
                "OgType," +
                "PageIconPath," +
                "BrowserConfig," +
                "BrowserConfigFolder," +
                "Manifest," +
                "PageLastmod," +
                "Changefreq," +
                "Priority," +
                "PageHeading," +
                "ImagePageHeadingFormFile," +
                "ImagePageHeadingId," +
                "PageFilter," +
                "TextOfPage," +
                "PageFilterOut," +
                "PageLinksByFilters," +
                "VideoFilterOut," +
                "VideoLinks," +
                "RefPages," +
                "PageLinks," +
                "RefPages2," +
                "PageLinks2," +
                "PhotoLinks," +
                "PhotoFilterOut"
        )]
        PageInfoModel editPage)
    {
        if (ModelState.IsValid)
        {
            #region Инициализация экземпляра страницы

            var pageUpdate = await pageInfoContext.PagesInfo
                .FirstAsync(page => page.PageInfoModelId == editPage.PageInfoModelId);

            #endregion

            #region Инициализация иконки страницы

            IconFileModel editPageiconItem;

            if (await iconContext.IconFiles
                .Where(icon => icon.IconPath == pageUpdate.PageIconPath && icon.IconFileName == DataConfig.IconItem).AnyAsync())
            {
                editPageiconItem = await iconContext.IconFiles
                    .FirstAsync(icon => icon.IconPath == pageUpdate.PageIconPath && icon.IconFileName == DataConfig.IconItem);
            }
            else
            {
                editPageiconItem = await iconContext.IconFiles
                    .FirstAsync(icon => icon.IconPath == "main/" && icon.IconFileName == DataConfig.IconItem);
            }

            #endregion

            #region MVC или RazorPage

            pageUpdate.PageAsRazorPage = editPage.PageAsRazorPage;

            #endregion

            #region Изменить картинку для страницы

            if (editPage.ImageFileModelId != Guid.Empty & editPage.ImageFileFormFile == null)
            {
                pageUpdate.ImageFileModelId = editPage.ImageFileModelId;
            }
            else
            {
                if (editPage.ImageFileFormFile != null)
                {
                    if (!(editPage.ImageFileFormFile.FileName.EndsWith(".webp") || editPage.ImageFileFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("ImageFileFormFile", $"Выбран некорректный файл «{editPage.ImageFileFormFile.FileName}»");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(editPage);
                    }

                    if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == editPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == editPage.ImageFileFormFile.FileName);

                        pageUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == editPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == editPage.ImageFileFormFile.FileName);

                        pageUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == editPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == editPage.ImageFileFormFile.FileName);

                        pageUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == editPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == editPage.ImageFileFormFile.FileName);

                        pageUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == editPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == editPage.ImageFileFormFile.FileName);

                        pageUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.ImageFileName == editPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == editPage.ImageFileFormFile.FileName);

                        pageUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == editPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == editPage.ImageFileFormFile.FileName);

                        pageUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.IconFileName == editPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == editPage.ImageFileFormFile.FileName);

                        pageUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == editPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == editPage.ImageFileFormFile.FileName);

                        pageUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == editPage.ImageFileFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == editPage.ImageFileFormFile.FileName);

                        pageUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("ImageFileFormFile", $"Добавьте картинку «{editPage.ImageFileFormFile.FileName}» в базу данных");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(editPage);
                    }
                }
                else
                {
                    pageUpdate.ImageFileModelId = editPage.ImageFileModelId;
                }
            }

            #endregion

            #region Изменить ссылку на текстовый файл

            if (editPage.TextInfoId != Guid.Empty & editPage.TextFileFormFile == null)
            {
                pageUpdate.TextInfoId = editPage.TextInfoId;
            }
            else if (editPage.TextInfoId == Guid.Empty & editPage.TextFileFormFile == null)
            {
                pageUpdate.TextInfoId = null;
            }
            else
            {
                if (editPage.TextFileFormFile != null)
                {
                    if (!(editPage.TextFileFormFile.FileName.EndsWith(".html") || editPage.TextFileFormFile.FileName.EndsWith(".txt")))
                    {
                        ModelState.AddModelError("TextFileFormFile", $"Выбран некорректный файл «{editPage.TextFileFormFile.FileName}»");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(editPage);
                    }

                    if (await textFileContext.Texts.Where(textFile => textFile.HtmlFileName == editPage.TextFileFormFile.FileName
                                                                        || textFile.TxtFileName == editPage.TextFileFormFile.FileName).AnyAsync())
                    {
                        var newTextFile = await textFileContext.Texts.FirstAsync(textFile => textFile.HtmlFileName == editPage.TextFileFormFile.FileName
                                                                                                        || textFile.TxtFileName == editPage.TextFileFormFile.FileName);

                        pageUpdate.TextInfoId = newTextFile.TextInfoModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("TextFileFormFile", $"Добавьте текстовый файл «{editPage.TextFileFormFile.FileName}» в базу данных");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(editPage);
                    }
                }
                else
                {
                    pageUpdate.TextInfoId = editPage.TextInfoId;
                }
            }

            #endregion

            #region Изменить фон для страницы

            if (editPage.BackgroundFileModelId != Guid.Empty & editPage.BackgroundFormFile == null)
            {
                pageUpdate.BackgroundFileModelId = editPage.BackgroundFileModelId;
            }
            else
            {
                if (editPage.BackgroundFormFile != null)
                {
                    if (!(editPage.BackgroundFormFile.FileName.EndsWith(".webp") | editPage.BackgroundFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("BackgroundFormFile", $"Выбран некорректный файл «{editPage.BackgroundFormFile.FileName}»");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(editPage);
                    }

                    if (await backgroundContext.BackgroundFiles.Where(bk => bk.WebLeftBackground == editPage.BackgroundFormFile.FileName).AnyAsync())
                    {
                        var newBackground = await backgroundContext.BackgroundFiles.FirstAsync(bk => bk.WebLeftBackground == editPage.BackgroundFormFile.FileName);

                        pageUpdate.BackgroundFileModelId = newBackground.BackgroundFileModelId;
                    }
                    else if (await backgroundContext.BackgroundFiles.Where(bk => bk.WebRightBackground == editPage.BackgroundFormFile.FileName).AnyAsync())
                    {
                        var newBackground = await backgroundContext.BackgroundFiles.FirstAsync(bk => bk.WebRightBackground == editPage.BackgroundFormFile.FileName);

                        pageUpdate.BackgroundFileModelId = newBackground.BackgroundFileModelId;
                    }
                    else if (await backgroundContext.BackgroundFiles.Where(bk => bk.LeftBackground == editPage.BackgroundFormFile.FileName).AnyAsync())
                    {
                        var newBackground = await backgroundContext.BackgroundFiles.FirstAsync(bk => bk.LeftBackground == editPage.BackgroundFormFile.FileName);

                        pageUpdate.BackgroundFileModelId = newBackground.BackgroundFileModelId;
                    }
                    else if (await backgroundContext.BackgroundFiles.Where(bk => bk.RightBackground == editPage.BackgroundFormFile.FileName).AnyAsync())
                    {
                        var newBackground = await backgroundContext.BackgroundFiles.FirstAsync(bk => bk.RightBackground == editPage.BackgroundFormFile.FileName);

                        pageUpdate.BackgroundFileModelId = newBackground.BackgroundFileModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("BackgroundFormFile", $"Добавьте фон «{editPage.BackgroundFormFile.FileName}» в базу данных");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(editPage);
                    }
                }
                else
                {
                    pageUpdate.BackgroundFileModelId = editPage.BackgroundFileModelId;
                }
            }

            #endregion

            #region Изменить аудиофайл

            if (editPage.AudioInfoId != Guid.Empty & editPage.AudioInfoFormFile == null)
            {
                pageUpdate.AudioInfoId = editPage.AudioInfoId;
            }
            else if (editPage.AudioInfoId == Guid.Empty & editPage.AudioInfoFormFile == null)
            {
                pageUpdate.AudioInfoId = null;
            }
            else
            {
                if (editPage.AudioInfoFormFile != null)
                {
                    if (!editPage.AudioInfoFormFile.FileName.EndsWith(".mp3"))
                    {
                        ModelState.AddModelError("AudioInfoFormFile", $"Выбран некорректный файл «{editPage.AudioInfoFormFile.FileName}»");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(editPage);
                    }

                    if (await audioFileContext.AudioFiles.Where(audioFile => audioFile.AudioFileName == editPage.AudioInfoFormFile.FileName).AnyAsync())
                    {
                        var newAudioFile = await audioFileContext.AudioFiles.FirstAsync(audioFile => audioFile.AudioFileName == editPage.AudioInfoFormFile.FileName);

                        pageUpdate.AudioInfoId = newAudioFile.AudioInfoModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("AudioInfoFormFile", $"Добавьте аудиофайл «{editPage.AudioInfoFormFile.FileName}» в базу данных");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(editPage);
                    }
                }
                else
                {
                    pageUpdate.AudioInfoId = editPage.AudioInfoId;
                }
            }

            #endregion

            #region Текст карточки страницы

            pageUpdate.PageCardText = editPage.PageCardText.Trim().ToUpper();

            #endregion

            #region Изменить индекс сортировки

            pageUpdate.SortOfPage = editPage.SortOfPage;

            #endregion

            #region Изменить адрес страницы

            #region Area

            if (string.IsNullOrEmpty(editPage.PageArea.Trim()) || editPage.PageArea == "Root")
            {
                pageUpdate.PageArea = string.Empty;
            }
            else
            {
                pageUpdate.PageArea = "/" + editPage.PageArea.Trim().Trim('/').ToLower();
            }

            #endregion

            #region Controller

            if (string.IsNullOrWhiteSpace(editPage.Controller) || string.IsNullOrEmpty(editPage.Controller) || editPage.PageAsRazorPage)
            {
                pageUpdate.Controller = string.Empty;
            }
            else
            {
                pageUpdate.Controller = "/" + editPage.Controller.Trim().Trim('/').ToLower();
            }

            #endregion

            #region Action

            if (string.IsNullOrWhiteSpace(editPage.Action) || string.IsNullOrEmpty(editPage.Action) || editPage.PageAsRazorPage)
            {
                pageUpdate.Action = string.Empty;
            }
            else
            {
                pageUpdate.Action = "/" + editPage.Action.Trim().Trim('/').ToLower();
            }

            #endregion

            #region QueryString

            if (string.IsNullOrWhiteSpace(editPage.RoutData) || string.IsNullOrEmpty(editPage.RoutData))
            {
                pageUpdate.RoutData = string.Empty;
            }
            else
            {
                pageUpdate.RoutData = "?" + editPage.RoutData.Trim().Trim('/').TrimStart('?').ToLower();
            }

            #endregion

            #region Адрес (для RazorPage) или Действие (для MVC)

            if (editPage.PageAsRazorPage)
            {
                if (string.IsNullOrWhiteSpace(editPage.PageLoc) || string.IsNullOrEmpty(editPage.PageLoc))
                {
                    ModelState.AddModelError("PageItem.PageLoc", "Введите адрес страницы без области");

                    // Список картинок сайта
                    ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                    // Список картинок для фона (фотопленка)
                    ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                    // Список текстовых файлов
                    ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                    // Список аудиофайлов
                    ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                    return View(editPage);
                }
                else if (editPage.PageLoc == "/")
                {
                    pageUpdate.PageLoc = "/";
                }
                else
                {
                    pageUpdate.PageLoc = "/" + editPage.PageLoc.Trim().Trim('/').TrimStart('?').ToLower();
                }
            }
            else
            {
                pageUpdate.PageLoc = editPage.Action;
            }

            #endregion

            #region Псевдоним адреса (1)

            if (string.IsNullOrWhiteSpace(editPage.PagePathNickName) || string.IsNullOrEmpty(editPage.PagePathNickName))
            {
                pageUpdate.PagePathNickName = string.Empty;
            }
            else if (editPage.PagePathNickName == "/")
            {
                pageUpdate.PagePathNickName = "/";
            }
            else
            {
                pageUpdate.PagePathNickName = "/" + editPage.PagePathNickName.Trim().Trim('/').ToLower();
            }

            #endregion

            #region Псевдоним адреса (2)

            if (string.IsNullOrWhiteSpace(editPage.PagePathNickName2) || string.IsNullOrEmpty(editPage.PagePathNickName2))
            {
                pageUpdate.PagePathNickName2 = string.Empty;
            }
            else if (editPage.PagePathNickName2 == "/")
            {
                pageUpdate.PagePathNickName2 = "/";
            }
            else
            {
                pageUpdate.PagePathNickName2 = "/" + editPage.PagePathNickName2.Trim().Trim('/').ToLower();
            }

            #endregion

            #endregion

            #region Изменить заголовок страницы теги title, description, keywords

            pageUpdate.PageTitle = editPage.PageTitle.Trim();
            pageUpdate.PageDescription = editPage.PageDescription.Trim();
            pageUpdate.PageKeyWords = editPage.PageKeyWords.Trim();

            #endregion

            #region OgType - PageIconPath - BrowserConfig - BrowserConfigFolder - Manifest

            if (editPage.OgType == "website")
            {
                pageUpdate.OgType = "website";
                pageUpdate.PageIconPath = "main/";
                pageUpdate.BrowserConfig = "main.xml";
                pageUpdate.BrowserConfigFolder = "/main";
                pageUpdate.Manifest = "main.json";
            }
            else if (editPage.OgType == "movie")
            {
                pageUpdate.OgType = "movie";
                pageUpdate.PageIconPath = "movie/";
                pageUpdate.BrowserConfig = "movie.xml";
                pageUpdate.BrowserConfigFolder = "/movie";
                pageUpdate.Manifest = "movie.json";
            }
            else
            {
                pageUpdate.PageIconPath = "main/";
                pageUpdate.BrowserConfig = "main.xml";
                pageUpdate.BrowserConfigFolder = "/main";
                pageUpdate.Manifest = "main.json";
            }

            if (pageUpdate.PageArea == "/admin")
            {
                pageUpdate.PageIconPath = "admin/";
                pageUpdate.BrowserConfig = "admin.xml";
                pageUpdate.BrowserConfigFolder = "/admin";
                pageUpdate.Manifest = "admin.json";
            }

            #endregion

            #region Изменить данные для Sitemap

            pageUpdate.PageLastmod = DateTime.Now;
            pageUpdate.Changefreq = editPage.Changefreq.Trim();
            pageUpdate.Priority = editPage.Priority.Trim();

            #endregion

            #region Содержание страницы

            #region Изменить оформление заголовка страницы

            pageUpdate.PageHeading = editPage.PageHeading.Trim();

            #endregion

            #region Изменить картинку для  заголовка страницы

            if (editPage.ImagePageHeadingId != Guid.Empty & editPage.ImagePageHeadingFormFile == null)
            {
                pageUpdate.ImagePageHeadingId = editPage.ImagePageHeadingId;
            }
            else
            {
                if (editPage.ImagePageHeadingFormFile != null)
                {
                    if (!(editPage.ImagePageHeadingFormFile.FileName.EndsWith(".webp") || editPage.ImagePageHeadingFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("ImagePageHeadingId", $"Выбран некорректный файл «{editPage.ImagePageHeadingFormFile.FileName}»");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(editPage);
                    }

                    if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == editPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == editPage.ImagePageHeadingFormFile.FileName);

                        pageUpdate.ImagePageHeadingId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == editPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == editPage.ImagePageHeadingFormFile.FileName);

                        pageUpdate.ImagePageHeadingId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == editPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == editPage.ImagePageHeadingFormFile.FileName);

                        pageUpdate.ImagePageHeadingId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == editPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == editPage.ImagePageHeadingFormFile.FileName);

                        pageUpdate.ImagePageHeadingId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == editPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == editPage.ImagePageHeadingFormFile.FileName);

                        pageUpdate.ImagePageHeadingId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.ImageFileName == editPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == editPage.ImagePageHeadingFormFile.FileName);

                        pageUpdate.ImagePageHeadingId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == editPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == editPage.ImagePageHeadingFormFile.FileName);

                        pageUpdate.ImagePageHeadingId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.IconFileName == editPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == editPage.ImagePageHeadingFormFile.FileName);

                        pageUpdate.ImagePageHeadingId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == editPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == editPage.ImagePageHeadingFormFile.FileName);

                        pageUpdate.ImagePageHeadingId = imageFile.ImageFileModelId;
                    }
                    else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == editPage.ImagePageHeadingFormFile.FileName).AnyAsync())
                    {
                        var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == editPage.ImagePageHeadingFormFile.FileName);

                        pageUpdate.ImagePageHeadingId = imageFile.ImageFileModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("ImagePageHeadingFormFile", $"Добавьте картинку «{editPage.ImagePageHeadingFormFile.FileName}» в базу данных");

                        // Список картинок сайта
                        ViewData["ImageFIles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок для фона (фотопленка)
                        ViewData["BackgroundImages"] = new SelectList(backgroundContext.BackgroundFiles.OrderBy(orderBackgroundImage => orderBackgroundImage.WebLeftBackground), "BackgroundFileModelId", "WebLeftBackground");

                        // Список текстовых файлов
                        ViewData["Texts"] = new SelectList(textFileContext.Texts.OrderBy(orderText => orderText.TxtFileName), "TextInfoModelId", "TxtFileName");

                        // Список аудиофайлов
                        ViewData["AudioFiles"] = new SelectList(audioFileContext.AudioFiles.OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile), "AudioInfoModelId", "CaptionOfTextInAudioFile");

                        return View(editPage);
                    }
                }
                else
                {
                    pageUpdate.ImagePageHeadingId = null;
                }
            }

            #endregion

            #region Изменить текст страницы

            pageUpdate.TextOfPage = editPage.TextOfPage.Trim();

            #endregion

            #endregion

            #region Изменить фильтры поиска текущей страницы

            pageUpdate.PageFilter = editPage.PageFilter.Trim();

            #endregion

            #region Изменить группы связанных ссылок

            #region Поиск связанных страниц по фильтрам

            pageUpdate.PageLinksByFilters = editPage.PageLinksByFilters;

            if (!string.IsNullOrEmpty(editPage.PageFilterOut))
            {
                pageUpdate.PageFilterOut = editPage.PageFilterOut.Trim();
            }
            else
            {
                editPage.PageFilterOut = null;
            }

            #endregion

            #region Поиск связанных страниц по GUID (1)

            pageUpdate.PageLinks = editPage.PageLinks;

            if (editPage.RefPages != null)
            {
                pageUpdate.RefPages = editPage.RefPages.Trim().ToLower();
            }
            else
            {
                editPage.RefPages = null;
            }

            #endregion

            #region Поиск связанных страниц по GUID (2)

            pageUpdate.PageLinks2 = editPage.PageLinks2;

            if (editPage.RefPages2 != null)
            {
                pageUpdate.RefPages2 = editPage.RefPages2.Trim().ToLower();
            }
            else
            {
                editPage.RefPages2 = null;
            }

            #endregion

            #region Поиск связанных видео

            pageUpdate.VideoLinks = editPage.VideoLinks;

            if (editPage.VideoFilterOut != null)
            {
                pageUpdate.VideoFilterOut = editPage.VideoFilterOut.Trim();
            }
            else
            {
                editPage.VideoFilterOut = null;
            }

            #endregion

            #region Альбом картинок

            pageUpdate.PhotoLinks = editPage.PhotoLinks;

            if (editPage.PhotoFilterOut != null)
            {
                pageUpdate.PhotoFilterOut = editPage.PhotoFilterOut.Trim();
            }
            else
            {
                editPage.PhotoFilterOut = null;
            }

            #endregion

            #endregion

            #region Сохранить изменения

            await pageInfoContext.SaveChangesInPageAsync();

            #endregion

            #region Переадресация на страницу информации о странице

            return RedirectToAction("DetailsPage", new { pageId = pageUpdate.PageInfoModelId, Area = "Admin" });

            #endregion
        }
        else
        {
            return View(editPage);
        }
    }

    #endregion

    #region Удалить страницу из базы данных

    public async Task<IActionResult> DeletePage(Guid? pageId)
    {
        PageInfoModel deletePage = new();

        if (pageId.HasValue)
        {
            if (await pageInfoContext.PagesInfo.Where(i => i.PageInfoModelId == pageId).AnyAsync())
            {
                deletePage = await pageInfoContext.PagesInfo.FirstAsync(i => i.PageInfoModelId == pageId);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

            return View(deletePage);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePage(PageInfoModel deletePage)
    {
        if (deletePage != null)
        {
            await pageInfoContext.DeletePageAsync(deletePage.PageInfoModelId);

            return RedirectToAction(nameof(Index));
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion
}