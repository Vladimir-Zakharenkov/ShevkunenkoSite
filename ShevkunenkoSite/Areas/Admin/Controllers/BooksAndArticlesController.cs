using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace ShevkunenkoSite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class BooksAndArticlesController
    (
    IBooksAndArticlesRepository bookContext,
    ITextInfoRepository textContext,
    IImageFileRepository imageContext,
    IMovieFileRepository movieContext,
    IFilmFileRepository filmContext
    ) : Controller
{
    #region Список книг и статей

    public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
    {
        var allBooksAndArticles = await bookContext.BooksAndArticles.ToListAsync();

        if (!searchString.IsNullOrEmpty())
        {
            allBooksAndArticles = [.. allBooksAndArticles.BookSearch(searchString).OrderBy(book => book.CaptionOfText)];
        }

        return View(new ItemsListViewModel
        {
            AllBooksAndArticlesFiles = [..  allBooksAndArticles
                     .Skip((pageNumber - 1) * DataConfig.NumberOfItemsPerPage)
                     .Take(DataConfig.NumberOfItemsPerPage)],

            #region Свойства PagingInfoViewModel

            TotalItems = allBooksAndArticles.Count,

            ItemsPerPage = DataConfig.NumberOfItemsPerPage,

            CurrentPage = pageNumber,

            SearchString = searchString ?? string.Empty

            #endregion
        });
    }

    #endregion

    #region Информация о книге или статье

    public async Task<IActionResult> DetailsBook(Guid? bookId)
    {
        if (bookId.HasValue && await bookContext.BooksAndArticles
                                                                .Where(b => b.BooksAndArticlesModelId == bookId)
                                                                .AnyAsync())
        {
            var bookItem = await bookContext.BooksAndArticles
                .Include(bookCover => bookCover.ImageFileModel)
                .Include(articleLogo => articleLogo.LogoOfArticle)
                .Include(articleScan => articleScan.ScanOfArticle)
                .Include(videoForArticle => videoForArticle.VideoForBookOrArticle)
                .Include(filmForArticle => filmForArticle.FilmForBookOrArticle)
                .FirstAsync(b => b.BooksAndArticlesModelId == bookId);

            return View(bookItem);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

    #region Добавить книгу или статью

    [HttpGet]
    public ViewResult AddBook()
    {
        BooksAndArticlesModel newBook = new();

        // Список картинок сайта (для выбора обложки)
        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

        // Список видео (movie) на сайте
        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

        // Список видео (film) на сайте
        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

        return View(newBook);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(5_268_435_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
    public async Task<IActionResult> AddBook(
                [Bind(
                "BooksAndArticlesModelId," +
                "ImageFileModelId," +
                "LogoOfArticleId," +
                "ScanOfArticleId," +
                "VideoForBookOrArticleId," +
                "TypeOfText," +
                "TypesOfText," +
                "Publisher," +
                "AuthorOfText," +
                "CaptionOfText," +
                "TheSubtitle," +
                "BookDescription," +
                "NumberOfPages," +
                "DateOfPublication," +
                "UrlOfArticle," +
                "TagsForBook," +
                "RefToWordDoc," +
                "RefToPdf," +
                "RefToAudio," +
                "PageForBookOrArticle," +
                "CoverForBookFormFile," +
                "LogoOfArticleFormFile," +
                "ScanOfArticleFormFile," +
                "VideoForBookOrArticleFormFile," +
                "FilmForBookOrArticleFormFile"
        )]
        BooksAndArticlesModel addBook)
    {
        if (ModelState.IsValid)
        {
            #region Тип текста

            if (addBook.TypeOfText != null)
            {
                _ = addBook.TypeOfText.Trim();
            }
            else
            {
                addBook.TypeOfText = string.Empty;
            }

            #endregion

            #region Добавить обложку для книги

            if (addBook.ImageFileModelId != Guid.Empty & addBook.CoverForBookFormFile == null)
            {
                _ = addBook.ImageFileModelId;
            }
            else
            {
                if (addBook.CoverForBookFormFile != null)
                {
                    if (!(addBook.CoverForBookFormFile.FileName.EndsWith(".webp") || addBook.CoverForBookFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("CoverForBookFormFile", $"Выбран некорректный файл «{addBook.CoverForBookFormFile.FileName}»");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список видео (film) на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

                        return View(addBook);
                    }

                    if (addBook.TypeOfText == "article")
                    {
                        addBook.ImageFileModelId = null;
                    }
                    else
                    {
                        if (await imageContext.ImageFiles.Where(image => image.WebImageFileName == addBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(image => image.WebImageFileName == addBook.CoverForBookFormFile.FileName);

                            addBook.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(image => image.WebImageHDFileName == addBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(image => image.WebImageHDFileName == addBook.CoverForBookFormFile.FileName);

                            addBook.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(image => image.WebIconFileName == addBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(image => image.WebIconFileName == addBook.CoverForBookFormFile.FileName);

                            addBook.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(image => image.WebIcon200FileName == addBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(image => image.WebIcon200FileName == addBook.CoverForBookFormFile.FileName);

                            addBook.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(image => image.WebIcon100FileName == addBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(image => image.WebIcon100FileName == addBook.CoverForBookFormFile.FileName);

                            addBook.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(image => image.ImageFileName == addBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(image => image.ImageFileName == addBook.CoverForBookFormFile.FileName);

                            addBook.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(image => image.ImageHDFileName == addBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(image => image.ImageHDFileName == addBook.CoverForBookFormFile.FileName);

                            addBook.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(image => image.IconFileName == addBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(image => image.IconFileName == addBook.CoverForBookFormFile.FileName);

                            addBook.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(image => image.Icon200FileName == addBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(image => image.Icon200FileName == addBook.CoverForBookFormFile.FileName);

                            addBook.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(image => image.Icon100FileName == addBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(image => image.Icon100FileName == addBook.CoverForBookFormFile.FileName);

                            addBook.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else
                        {
                            ModelState.AddModelError("CoverForBookFormFile", $"Добавьте картинку «{addBook.CoverForBookFormFile.FileName}» в базу данных");

                            // Список картинок сайта для обложки
                            ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                            // Список видео на сайте
                            ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                            // Список видео (film) на сайте
                            ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

                            return View(addBook);
                        }
                    }
                }
                else
                {
                    addBook.ImageFileModelId = null;
                }
            }

            #endregion

            #region Издатель книги или статьи

            _ = addBook.Publisher.Trim();

            #endregion

            #region Логотип статьи

            if (addBook.LogoOfArticleId != Guid.Empty & addBook.LogoOfArticleFormFile == null)
            {
                _ = addBook.LogoOfArticleId;
            }
            else
            {
                if (addBook.LogoOfArticleFormFile != null)
                {
                    if (!(addBook.LogoOfArticleFormFile.FileName.EndsWith(".webp") || addBook.LogoOfArticleFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("LogoOfArticleFormFile", $"Выбран некорректный файл «{addBook.LogoOfArticleFormFile.FileName}»");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список видео (film) на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

                        return View(addBook);
                    }

                    if (addBook.TypeOfText == "book")
                    {
                        addBook.LogoOfArticleId = null;
                    }
                    else
                    {
                        if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == addBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == addBook.LogoOfArticleFormFile.FileName);

                            addBook.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == addBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == addBook.LogoOfArticleFormFile.FileName);

                            addBook.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == addBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == addBook.LogoOfArticleFormFile.FileName);

                            addBook.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == addBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == addBook.LogoOfArticleFormFile.FileName);

                            addBook.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == addBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == addBook.LogoOfArticleFormFile.FileName);

                            addBook.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageFileName == addBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == addBook.LogoOfArticleFormFile.FileName);

                            addBook.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == addBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == addBook.LogoOfArticleFormFile.FileName);

                            addBook.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.IconFileName == addBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == addBook.LogoOfArticleFormFile.FileName);

                            addBook.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == addBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == addBook.LogoOfArticleFormFile.FileName);

                            addBook.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == addBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == addBook.LogoOfArticleFormFile.FileName);

                            addBook.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else
                        {
                            ModelState.AddModelError("LogoOfArticleFormFile", $"Добавьте картинку «{addBook.LogoOfArticleFormFile.FileName}» в базу данных");

                            // Список картинок сайта для обложки
                            ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                            // Список видео на сайте
                            ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                            // Список видео (film) на сайте
                            ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

                            return View(addBook);
                        }
                    }
                }
                else
                {
                    addBook.LogoOfArticleId = null;
                }
            }

            #endregion

            #region Скан статьи

            if (addBook.ScanOfArticleId != Guid.Empty & addBook.ScanOfArticleFormFile == null)
            {
                _ = addBook.ScanOfArticleId;
            }
            else
            {
                if (addBook.ScanOfArticleFormFile != null)
                {
                    if (!(addBook.ScanOfArticleFormFile.FileName.EndsWith(".webp") || addBook.ScanOfArticleFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("ScanOfArticleFormFile", $"Выбран некорректный файл «{addBook.ScanOfArticleFormFile.FileName}»");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список видео (film) на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

                        return View(addBook);
                    }

                    if (addBook.TypeOfText == "book")
                    {
                        addBook.ScanOfArticleId = null;
                    }
                    else
                    {
                        if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == addBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == addBook.ScanOfArticleFormFile.FileName);

                            addBook.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == addBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == addBook.ScanOfArticleFormFile.FileName);

                            addBook.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == addBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == addBook.ScanOfArticleFormFile.FileName);

                            addBook.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == addBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == addBook.ScanOfArticleFormFile.FileName);

                            addBook.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == addBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == addBook.ScanOfArticleFormFile.FileName);

                            addBook.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageFileName == addBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == addBook.ScanOfArticleFormFile.FileName);

                            addBook.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == addBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == addBook.ScanOfArticleFormFile.FileName);

                            addBook.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.IconFileName == addBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == addBook.ScanOfArticleFormFile.FileName);

                            addBook.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == addBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == addBook.ScanOfArticleFormFile.FileName);

                            addBook.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == addBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == addBook.ScanOfArticleFormFile.FileName);

                            addBook.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else
                        {
                            ModelState.AddModelError("ScanOfArticleFormFile", $"Добавьте картинку «{addBook.ScanOfArticleFormFile.FileName}» в базу данных");

                            // Список картинок сайта для обложки
                            ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                            // Список видео на сайте
                            ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                            // Список видео (film) на сайте
                            ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

                            return View(addBook);
                        }
                    }
                }
                else
                {
                    addBook.ScanOfArticleId = null;
                }
            }

            #endregion

            #region Видео (movie) связанное с книгой (статьёй)

            if (addBook.VideoForBookOrArticleId != Guid.Empty & addBook.VideoForBookOrArticleFormFile == null)
            {
                _ = addBook.VideoForBookOrArticleId;
            }
            else
            {
                if (addBook.VideoForBookOrArticleFormFile != null)
                {
                    if (!addBook.VideoForBookOrArticleFormFile.FileName.EndsWith(".mp4"))
                    {
                        ModelState.AddModelError("VideoForBookOrArticleFormFile", $"Выбран некорректный файл «{addBook.VideoForBookOrArticleFormFile.FileName}»");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список видео (film) на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

                        return View(addBook);
                    }

                    if (await movieContext.MovieFiles
                            .Where(movie => movie.MovieFileName == addBook.VideoForBookOrArticleFormFile.FileName)
                            .AnyAsync())
                    {
                        var movieForItem = await movieContext.MovieFiles
                                .FirstAsync(movie => movie.MovieFileName == addBook.VideoForBookOrArticleFormFile.FileName);

                        addBook.VideoForBookOrArticleId = movieForItem.MovieFileModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("VideoForBookOrArticleFormFile", $"Файл видео «{addBook.VideoForBookOrArticleFormFile.FileName}» не найден в базе данных");

                        // Список картинок сайта
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок сайта
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список видео (film) на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

                        return View(addBook);
                    }
                }
                else
                {
                    addBook.VideoForBookOrArticleId = null;
                }
            }

            #endregion

            #region Фильм (film) связанный с книгой (статьёй)

            if (addBook.FilmForBookOrArticleId != Guid.Empty & addBook.FilmForBookOrArticleFormFile == null)
            {
                addBook.VideoForBookOrArticleId = null;

                _ = addBook.FilmForBookOrArticleId;
            }
            else
            {
                if (addBook.FilmForBookOrArticleFormFile != null)
                {
                    if (!addBook.FilmForBookOrArticleFormFile.FileName.EndsWith(".mp4"))
                    {
                        ModelState.AddModelError("FilmForBookOrArticleFormFile", $"Выбран некорректный файл «{addBook.FilmForBookOrArticleFormFile.FileName}»");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список видео (film) на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

                        return View(addBook);
                    }

                    if (await filmContext.FilmFiles
                            .Where(film => film.FilmFileName == addBook.FilmForBookOrArticleFormFile.FileName)
                            .AnyAsync())
                    {
                        var filmForItem = await filmContext.FilmFiles
                                .FirstAsync(film => film.FilmFileName == addBook.FilmForBookOrArticleFormFile.FileName);

                        addBook.FilmForBookOrArticleId = filmForItem.FilmFileModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("FilmForBookOrArticleFormFile", $"Файл видео «{addBook.FilmForBookOrArticleFormFile.FileName}» не найден в базе данных");

                        // Список картинок сайта
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок сайта
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список видео (film) на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

                        return View(addBook);
                    }
                }
                else
                {
                    addBook.FilmForBookOrArticleId = null;
                }
            }

            #endregion

            #region Ссылка на статью или издателя

            _ = addBook.UrlOfArticle;

            #endregion

            #region Автор книги или статьи

            if (addBook.AuthorOfText != null)
            {
                _ = addBook.AuthorOfText.Trim();
            }
            else
            {
                addBook.AuthorOfText = string.Empty;
            }

            #endregion

            #region Название книги или статьи

            _ = addBook.BookDescription.Trim();

            #endregion

            #region Подзаголовок книги или статьи

            _ = addBook.TheSubtitle.Trim();

            #endregion

            #region Описание книги или статьи

            _ = addBook.CaptionOfText.Trim();

            #endregion

            #region Количество страниц

            _ = addBook.NumberOfPages;

            #endregion

            #region Дата публикации книги (статьи)

            _ = addBook.DateOfPublication;

            #endregion

            #region Теги по содержанию книги (статьи)

            _ = addBook.TagsForBook.Trim();

            #endregion

            #region Ссылки на скачивание текста и аудиокниги

            _ = addBook.RefToWordDoc;

            _ = addBook.RefToPdf;

            _ = addBook.RefToAudio;

            #endregion

            #region Добавить в БД

            await bookContext.AddBookOrArticleAsync(addBook);

            #endregion

            #region Открытие параметров добавленной книги (статьи)

            var newBook = await bookContext.BooksAndArticles
                .FirstAsync(book => book.CaptionOfText == addBook.CaptionOfText);

            return RedirectToAction(nameof(DetailsBook), new { bookId = newBook.BooksAndArticlesModelId });

            #endregion
        }
        else
        {
            // Список картинок сайта для обложки
            ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

            // Список видео на сайте
            ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

            // Список видео (film) на сайте
            ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

            return View(addBook);
        }
    }

    #endregion

    #region Изменить книгу или статью

    [HttpGet]
    public async Task<IActionResult> EditBook(Guid? bookId)
    {
        if (bookId.HasValue & await bookContext.BooksAndArticles.Where(book => book.BooksAndArticlesModelId == bookId).AnyAsync())
        {
            var editBook = await bookContext.BooksAndArticles
                .Include(cover => cover.ImageFileModel)
                .Include(articleLogo => articleLogo.LogoOfArticle)
                .Include(articleScan => articleScan.ScanOfArticle)
                .Include(videoForArticle => videoForArticle.VideoForBookOrArticle)
                .Include(filmForArticle => filmForArticle.FilmForBookOrArticle)
                .FirstAsync(b => b.BooksAndArticlesModelId == bookId);

            // Список картинок сайта для обложки
            ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

            // Список картинок сайта для логотипа
            ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

            // Список видео на сайте
            ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

            // Список фильмов на сайте
            ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaption");

            return View(editBook);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(5_268_435_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]

    public async Task<IActionResult> EditBook(
                [Bind(
                "BooksAndArticlesModelId," +
                "ImageFileModelId," +
                "LogoOfArticleId," +
                "ScanOfArticleId," +
                "VideoForBookOrArticleId," +
                "FilmForBookOrArticleId," +
                "TypeOfText," +
                "TypesOfText," +
                "Publisher," +
                "AuthorOfText," +
                "CaptionOfText," +
                "TheSubtitle," +
                "BookDescription," +
                "NumberOfPages," +
                "DateOfPublication," +
                "UrlOfArticle," +
                "TagsForBook," +
                "RefToWordDoc," +
                "RefToPdf," +
                "RefToAudio," +
                "PageForBookOrArticle," +
                "CoverForBookFormFile," +
                "LogoOfArticleFormFile," +
                "ScanOfArticleFormFile," +
                "VideoForBookOrArticleFormFile," +
                "FilmForBookOrArticleFormFile"
        )]
        BooksAndArticlesModel editBook)
    {
        if (ModelState.IsValid)
        {
            #region Инициализация экземпляра страницы

            BooksAndArticlesModel bookUpdate = await bookContext.BooksAndArticles
                .FirstAsync(b => b.BooksAndArticlesModelId == editBook.BooksAndArticlesModelId);

            #endregion

            #region Тип текста

            bookUpdate.TypeOfText = editBook.TypeOfText.Trim();

            #endregion

            #region Изменить обложку для книги

            if (editBook.ImageFileModelId != Guid.Empty & editBook.CoverForBookFormFile == null)
            {
                bookUpdate.ImageFileModelId = editBook.ImageFileModelId;
            }
            else
            {
                if (editBook.CoverForBookFormFile != null)
                {
                    if (!(editBook.CoverForBookFormFile.FileName.EndsWith(".webp") || editBook.CoverForBookFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("CoverForBookFormFile", $"Выбран некорректный файл «{editBook.CoverForBookFormFile.FileName}»");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок сайта для логотипа
                        ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список фильмов на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaptionCaption");

                        return View(editBook);
                    }

                    if (bookUpdate.TypeOfText == "article")
                    {
                        bookUpdate.ImageFileModelId = null;
                    }
                    else
                    {
                        if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == editBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == editBook.CoverForBookFormFile.FileName);

                            bookUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == editBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == editBook.CoverForBookFormFile.FileName);

                            bookUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == editBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == editBook.CoverForBookFormFile.FileName);

                            bookUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == editBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == editBook.CoverForBookFormFile.FileName);

                            bookUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == editBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == editBook.CoverForBookFormFile.FileName);

                            bookUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageFileName == editBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == editBook.CoverForBookFormFile.FileName);

                            bookUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == editBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == editBook.CoverForBookFormFile.FileName);

                            bookUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.IconFileName == editBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == editBook.CoverForBookFormFile.FileName);

                            bookUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == editBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == editBook.CoverForBookFormFile.FileName);

                            bookUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == editBook.CoverForBookFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == editBook.CoverForBookFormFile.FileName);

                            bookUpdate.ImageFileModelId = imageFile.ImageFileModelId;
                        }
                        else
                        {
                            ModelState.AddModelError("CoverForBookFormFile", $"Добавьте картинку «{editBook.CoverForBookFormFile.FileName}» в базу данных");

                            // Список картинок сайта для обложки
                            ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                            // Список картинок сайта для логотипа
                            ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                            // Список видео на сайте
                            ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                            // Список фильмов на сайте
                            ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaptionCaption");

                            return View(editBook);
                        }
                    }
                }
                else
                {
                    bookUpdate.ImageFileModelId = null;
                }
            }

            #endregion

            #region Издатель книги или статьи

            bookUpdate.Publisher = editBook.Publisher.Trim();

            #endregion

            #region Логотип статьи

            if (editBook.LogoOfArticleId != Guid.Empty & editBook.LogoOfArticleFormFile == null)
            {
                bookUpdate.LogoOfArticleId = editBook.LogoOfArticleId;
            }
            else
            {
                if (editBook.LogoOfArticleFormFile != null)
                {
                    if (!(editBook.LogoOfArticleFormFile.FileName.EndsWith(".webp") || editBook.LogoOfArticleFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("LogoOfArticleFormFile", $"Выбран некорректный файл «{editBook.LogoOfArticleFormFile.FileName}»");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок сайта для логотипа
                        ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список фильмов на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaptionCaption");

                        return View(editBook);
                    }

                    if (bookUpdate.TypeOfText == "book")
                    {
                        bookUpdate.LogoOfArticleId = null;
                    }
                    else
                    {
                        if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == editBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == editBook.LogoOfArticleFormFile.FileName);

                            bookUpdate.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == editBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == editBook.LogoOfArticleFormFile.FileName);

                            bookUpdate.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == editBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == editBook.LogoOfArticleFormFile.FileName);

                            bookUpdate.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == editBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == editBook.LogoOfArticleFormFile.FileName);

                            bookUpdate.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == editBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == editBook.LogoOfArticleFormFile.FileName);

                            bookUpdate.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageFileName == editBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == editBook.LogoOfArticleFormFile.FileName);

                            bookUpdate.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == editBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == editBook.LogoOfArticleFormFile.FileName);

                            bookUpdate.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.IconFileName == editBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == editBook.LogoOfArticleFormFile.FileName);

                            bookUpdate.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == editBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == editBook.LogoOfArticleFormFile.FileName);

                            bookUpdate.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == editBook.LogoOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == editBook.LogoOfArticleFormFile.FileName);

                            bookUpdate.LogoOfArticleId = imageFile.ImageFileModelId;
                        }
                        else
                        {
                            ModelState.AddModelError("LogoOfArticleFormFile", $"Добавьте картинку «{editBook.LogoOfArticleFormFile.FileName}» в базу данных");

                            // Список картинок сайта для обложки
                            ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                            // Список картинок сайта для логотипа
                            ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                            // Список видео на сайте
                            ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                            // Список фильмов на сайте
                            ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaptionCaption");

                            return View(editBook);
                        }
                    }
                }
                else
                {
                    bookUpdate.LogoOfArticleId = null;
                }
            }

            #endregion

            #region Скан статьи

            if (editBook.ScanOfArticleId != Guid.Empty & editBook.ScanOfArticleFormFile == null)
            {
                bookUpdate.ScanOfArticleId = editBook.ScanOfArticleId;
            }
            else
            {
                if (editBook.ScanOfArticleFormFile != null)
                {
                    if (!(editBook.ScanOfArticleFormFile.FileName.EndsWith(".webp") || editBook.ScanOfArticleFormFile.FileName.EndsWith(".png")))
                    {
                        ModelState.AddModelError("ScanOfArticleFormFile", $"Выбран некорректный файл «{editBook.ScanOfArticleFormFile.FileName}»");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок сайта для логотипа
                        ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список фильмов на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaptionCaption");

                        return View(editBook);
                    }

                    if (bookUpdate.TypeOfText == "book")
                    {
                        bookUpdate.ScanOfArticleId = null;
                    }
                    else
                    {
                        if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == editBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == editBook.ScanOfArticleFormFile.FileName);

                            bookUpdate.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == editBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == editBook.ScanOfArticleFormFile.FileName);

                            bookUpdate.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == editBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == editBook.ScanOfArticleFormFile.FileName);

                            bookUpdate.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == editBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == editBook.ScanOfArticleFormFile.FileName);

                            bookUpdate.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == editBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == editBook.ScanOfArticleFormFile.FileName);

                            bookUpdate.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageFileName == editBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == editBook.ScanOfArticleFormFile.FileName);

                            bookUpdate.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == editBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == editBook.ScanOfArticleFormFile.FileName);

                            bookUpdate.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.IconFileName == editBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == editBook.ScanOfArticleFormFile.FileName);

                            bookUpdate.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == editBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == editBook.ScanOfArticleFormFile.FileName);

                            bookUpdate.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == editBook.ScanOfArticleFormFile.FileName).AnyAsync())
                        {
                            var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == editBook.ScanOfArticleFormFile.FileName);

                            bookUpdate.ScanOfArticleId = imageFile.ImageFileModelId;
                        }
                        else
                        {
                            ModelState.AddModelError("ScanOfArticleFormFile", $"Добавьте картинку «{editBook.ScanOfArticleFormFile.FileName}» в базу данных");

                            // Список картинок сайта для обложки
                            ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                            // Список картинок сайта для логотипа
                            ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                            // Список видео на сайте
                            ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                            // Список фильмов на сайте
                            ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaptionCaption");

                            return View(editBook);
                        }
                    }
                }
                else
                {
                    bookUpdate.ScanOfArticleId = null;
                }
            }

            #endregion

            #region Видео связанное с книгой (статьёй)

            if (editBook.VideoForBookOrArticleId != Guid.Empty & editBook.VideoForBookOrArticleFormFile == null)
            {
                bookUpdate.VideoForBookOrArticleId = editBook.VideoForBookOrArticleId;
            }
            else
            {
                if (editBook.VideoForBookOrArticleFormFile != null)
                {
                    if (!editBook.VideoForBookOrArticleFormFile.FileName.EndsWith(".mp4"))
                    {
                        ModelState.AddModelError("VideoForBookOrArticleFormFile", $"Выбран некорректный файл «{editBook.VideoForBookOrArticleFormFile.FileName}»");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок сайта для логотипа
                        ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список фильмов на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaptionCaption");

                        return View(editBook);
                    }

                    if (await movieContext.MovieFiles
                            .Where(movie => movie.MovieFileName == editBook.VideoForBookOrArticleFormFile.FileName)
                            .AnyAsync())
                    {
                        var movieForItem = await movieContext.MovieFiles
                                .FirstAsync(movie => movie.MovieFileName == editBook.VideoForBookOrArticleFormFile.FileName);

                        bookUpdate.VideoForBookOrArticleId = movieForItem.MovieFileModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("VideoForBookOrArticleFormFile", $"Файл видео «{editBook.VideoForBookOrArticleFormFile.FileName}» не найден в базе данных");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок сайта для логотипа
                        ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список фильмов на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaptionCaption");

                        return View(editBook);
                    }
                }
                else
                {
                    bookUpdate.VideoForBookOrArticleId = null;
                }
            }

            #endregion

            #region Фильм по книге (статье)

            if (editBook.FilmForBookOrArticleId != Guid.Empty & editBook.FilmForBookOrArticleFormFile == null)
            {
                bookUpdate.FilmForBookOrArticleId = editBook.FilmForBookOrArticleId;

                if (bookUpdate.FilmForBookOrArticleId != null)
                {
                    bookUpdate.VideoForBookOrArticleId = null;
                }
            }
            else
            {
                if (editBook.FilmForBookOrArticleFormFile != null)
                {
                    if (!editBook.FilmForBookOrArticleFormFile.FileName.EndsWith(".mp4"))
                    {
                        ModelState.AddModelError("FilmForBookOrArticleFormFile", $"Выбран некорректный файл «{editBook.FilmForBookOrArticleFormFile.FileName}»");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок сайта для логотипа
                        ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список фильмов на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaptionCaption");

                        return View(editBook);
                    }

                    if (await filmContext.FilmFiles
                            .Where(film => film.FilmFileName == editBook.FilmForBookOrArticleFormFile.FileName)
                            .AnyAsync())
                    {
                        var filmForItem = await filmContext.FilmFiles
                                .FirstAsync(film => film.FilmFileName == editBook.FilmForBookOrArticleFormFile.FileName);

                        bookUpdate.FilmForBookOrArticleId = filmForItem.FilmFileModelId;

                        bookUpdate.VideoForBookOrArticleId = null;
                    }
                    else
                    {
                        ModelState.AddModelError("FIlmForBookOrArticleFormFile", $"Файл видео «{editBook.FilmForBookOrArticleFormFile.FileName}» не найден в базе данных");

                        // Список картинок сайта для обложки
                        ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список картинок сайта для логотипа
                        ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

                        // Список видео на сайте
                        ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

                        // Список фильмов на сайте
                        ViewData["FilmFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaptionCaption");

                        return View(editBook);
                    }
                }
                else
                {
                    bookUpdate.FilmForBookOrArticleId = null;
                }
            }

            #endregion

            #region Ссылка на статью или издателя

            bookUpdate.UrlOfArticle = editBook.UrlOfArticle;

            #endregion

            #region Автор книги или статьи

            bookUpdate.AuthorOfText = editBook.AuthorOfText.Trim();

            #endregion

            #region Заголовок книги или статьи

            bookUpdate.BookDescription = editBook.BookDescription.Trim();

            #endregion

            #region Подзаголовок книги или статьи

            bookUpdate.TheSubtitle = editBook.TheSubtitle.Trim();

            #endregion

            #region Описание книги или статьи

            bookUpdate.CaptionOfText = editBook.CaptionOfText.Trim();

            #endregion

            #region Количество страниц

            bookUpdate.NumberOfPages = editBook.NumberOfPages;

            #endregion

            #region Дата публикации книги (статьи)

            bookUpdate.DateOfPublication = editBook.DateOfPublication;

            #endregion

            #region Теги по содержанию книги (статьи)

            bookUpdate.TagsForBook = editBook.TagsForBook.Trim();

            #endregion

            #region Ссылки на загрузку

            bookUpdate.RefToWordDoc = editBook.RefToWordDoc;

            bookUpdate.RefToPdf = editBook.RefToPdf;

            bookUpdate.RefToAudio = editBook.RefToAudio;

            #endregion

            #region Сохранить и перейти к DetailsBook

            await bookContext.SaveChangesInBookOrArticleAsync();

            return RedirectToAction(nameof(DetailsBook), new { bookId = bookUpdate.BooksAndArticlesModelId });

            #endregion
        }
        else
        {
            // Список картинок сайта для обложки
            ViewData["ImageFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

            // Список картинок сайта для логотипа
            ViewData["LogoFiles"] = new SelectList(imageContext.ImageFiles.OrderBy(orderImage => orderImage.ImageCaption), "ImageFileModelId", "ImageCaption");

            // Список видео на сайте
            ViewData["VideoFiles"] = new SelectList(movieContext.MovieFiles.OrderBy(orderVideo => orderVideo.MovieCaption), "MovieFileModelId", "MovieCaption");

            // Список фильмов на сайте
            ViewData["VideoFiles"] = new SelectList(filmContext.FilmFiles.OrderBy(orderFilm => orderFilm.FilmCaption), "FilmFileModelId", "FilmCaptionCaption");

            return View(editBook);
        }
    }

    #endregion

    #region Удалить книгу или статью

    [HttpGet]
    public async Task<IActionResult> DeleteBook(Guid? bookId)
    {
        if (bookId.HasValue & await bookContext.BooksAndArticles.Where(book => book.BooksAndArticlesModelId == bookId).AnyAsync())
        {
            var deleteBook = await bookContext.BooksAndArticles.FirstAsync(book => book.BooksAndArticlesModelId == bookId);

            return View(deleteBook);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBook(BooksAndArticlesModel? deleteBook)
    {
        if (deleteBook != null)
        {
            if (await textContext.Texts.Where(text => text.BooksAndArticlesModelId == deleteBook.BooksAndArticlesModelId).AnyAsync())
            {
                ModelState.AddModelError("SequenceNumber", "Ссылки на книгу в базе текстов сайта");

                return View(deleteBook);
            }

            if (await bookContext.BooksAndArticles.Where(book => book.BooksAndArticlesModelId == deleteBook.BooksAndArticlesModelId).AnyAsync())
            {
                await bookContext.DeleteBookOrArticleAsync(deleteBook.BooksAndArticlesModelId);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion
}