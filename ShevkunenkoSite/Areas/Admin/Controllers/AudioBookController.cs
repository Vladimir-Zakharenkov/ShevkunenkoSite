using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShevkunenkoSite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class AudioBookController(
    IAudioBookRepository audioBookContext,
    IBooksAndArticlesRepository bookContext
    ) : Controller
{
    #region Список аудиокниг

    public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
    {
        var allAudioBooks = await audioBookContext.AudioBooks.ToListAsync();

        if (!string.IsNullOrEmpty(searchString))
        {
            allAudioBooks = [.. allAudioBooks.AudioBookSearch(searchString).OrderBy(audioBook => audioBook.CaptionOfAudioBook)];
        }

        return View(new ItemsListViewModel
        {
            AllAudioBooks = [.. allAudioBooks
                     .Skip((pageNumber - 1) * DataConfig.NumberOfItemsPerPage)
                     .Take(DataConfig.NumberOfItemsPerPage)],

            CurrentPage = pageNumber,
            ItemsPerPage = DataConfig.NumberOfItemsPerPage,
            TotalItems = allAudioBooks.Count,

            SearchString = searchString ?? string.Empty
        });
    }

    #endregion

    #region Информация об аудиокниге

    public async Task<IActionResult> DetailsAudioBook(Guid? audioBookId)
    {
        if (audioBookId.HasValue
                & await audioBookContext.AudioBooks
                    .Where(audioBook => audioBook.AudioBookModelId == audioBookId)
                    .AnyAsync())
        {
            var audioBook = await audioBookContext.AudioBooks
                .Include(inc => inc.BookForAudioBook)
                .FirstAsync(audioBook => audioBook.AudioBookModelId == audioBookId);

            return View(audioBook);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

    #region Добавить аудиокнигу

    [HttpGet]
    public ViewResult AddAudioBook()
    {
        AudioBookModel newBook = new();

        #region SelectList из названий книг на сайте

        // Список картинок сайта для обложки
        ViewData["Books"] = new SelectList(bookContext.BooksAndArticles
                                                                            .Where(book => book.TypeOfText == "book")
                                                                            .OrderBy(orderBook => orderBook.CaptionOfText),
                                                                            "BooksAndArticlesModelId", "CaptionOfText");

        #endregion

        return View(newBook);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(5_268_435_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
    public async Task<IActionResult> AddAudioBook(
                [Bind(
                "AudioBookModelId," +
                "CaptionOfAudioBook," +
                "AudioBookDescription," +
                "ActorOfAudioBook," +
                "NumberOfFiles," +
                "BookForAudioBookId," +
                "BookForAudioBook"
                )]
        AudioBookModel addAudioBook)
    {
        if (ModelState.IsValid)
        {
            #region Название аудиокниги

            _ = addAudioBook.CaptionOfAudioBook.Trim();

            #endregion

            #region Описание аудиокниги

            _ = addAudioBook.AudioBookDescription.Trim();

            #endregion

            #region Исполнитель

            _ = addAudioBook.ActorOfAudioBook.Trim();

            #endregion

            #region Количество файлов

            _ = addAudioBook.NumberOfFiles;

            #endregion

            #region Книга для аудиокниги

            if (addAudioBook.BookForAudioBookId == Guid.Empty)
            {
                addAudioBook.BookForAudioBookId = null;
            }
            else
            {
                _ = addAudioBook.BookForAudioBookId;
            }

            #endregion

            #region Добавить в БД

            await audioBookContext.AddAudioBookAsync(addAudioBook);

            #endregion

            #region Открытие параметров добавленной аудиокниги

            var newAudioBook = await audioBookContext.AudioBooks
                .FirstAsync(audioBook => audioBook.CaptionOfAudioBook == addAudioBook.CaptionOfAudioBook & audioBook.ActorOfAudioBook == addAudioBook.ActorOfAudioBook);

            return RedirectToAction(nameof(DetailsAudioBook), new { audioBookId = newAudioBook.AudioBookModelId });

            #endregion
        }
        else
        {
            #region SelectList из названий книг на сайте

            // Список картинок сайта для обложки
            ViewData["Books"] = new SelectList(bookContext.BooksAndArticles
                                                                                .Where(book => book.TypeOfText == "book")
                                                                                .OrderBy(orderBook => orderBook.CaptionOfText),
                                                                                "BooksAndArticlesModelId", "CaptionOfText");

            #endregion

            return View(nameof(AddAudioBook), addAudioBook);
        }
    }

    #endregion

    #region Изменить аудиокнигу

    [HttpGet]
    public async Task<IActionResult> EditAudioBook(Guid? audioBookId)
    {
        if (audioBookId.HasValue
            & await audioBookContext.AudioBooks
                .Where(audioBook => audioBook.AudioBookModelId == audioBookId)
                .AnyAsync())
        {
            #region Экземпляр редактируемой аудиокниги

            var editAudioBook = await audioBookContext.AudioBooks
                .Include(inc => inc.BookForAudioBook)
                .FirstAsync(audioBook => audioBook.AudioBookModelId == audioBookId);

            #endregion

            #region SelectList из названий книг на сайте

            // Список картинок сайта для обложки
            ViewData["Books"] = new SelectList(bookContext.BooksAndArticles
                                                                                .Where(book => book.TypeOfText == "book")
                                                                                .OrderBy(orderBook => orderBook.CaptionOfText),
                                                                                "BooksAndArticlesModelId", "CaptionOfText");

            #endregion

            return View(editAudioBook);
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
    public async Task<IActionResult> EditAudioBook(
                [Bind(
                "AudioBookModelId," +
                "CaptionOfAudioBook," +
                "AudioBookDescription," +
                "ActorOfAudioBook," +
                "NumberOfFiles," +
                "BookForAudioBookId," +
                "BookForAudioBook"
                )]
        AudioBookModel audioBookItem)
    {
        AudioBookModel audioBookUpdate = await audioBookContext.AudioBooks
            .FirstAsync(audioBook => audioBook.AudioBookModelId == audioBookItem.AudioBookModelId);

        if (ModelState.IsValid)
        {
            #region Название аудиокниги

            audioBookUpdate.CaptionOfAudioBook = audioBookItem.CaptionOfAudioBook.Trim();

            #endregion

            #region Описание аудиокниги

            audioBookUpdate.AudioBookDescription = audioBookItem.AudioBookDescription.Trim();

            #endregion

            #region Исполнитель

            audioBookUpdate.ActorOfAudioBook = audioBookItem.ActorOfAudioBook.Trim();

            #endregion

            #region Количество файлов

            audioBookUpdate.NumberOfFiles = audioBookItem.NumberOfFiles;

            #endregion

            #region Книга для аудиокниги

            if (audioBookItem.BookForAudioBookId == Guid.Empty)
            {
                audioBookUpdate.BookForAudioBookId = null;
            }
            else
            {
                audioBookUpdate.BookForAudioBookId = audioBookItem.BookForAudioBookId;
            }

            #endregion

            #region Сохранить и перейти к DetailsAudioBook

            await audioBookContext.SaveChangesInAudioBookAsync();

            return RedirectToAction(nameof(DetailsAudioBook), new { audioBookId = audioBookUpdate.AudioBookModelId });

            #endregion
        }
        else
        {
            #region SelectList из названий книг на сайте

            // Список картинок сайта для обложки
            ViewData["Books"] = new SelectList(bookContext.BooksAndArticles
                                                                                .Where(book => book.TypeOfText == "book")
                                                                                .OrderBy(orderBook => orderBook.CaptionOfText),
                                                                                "BooksAndArticlesModelId", "CaptionOfText");

            #endregion

            return View(nameof(EditAudioBook), audioBookItem);
        }
    }

    #endregion
}