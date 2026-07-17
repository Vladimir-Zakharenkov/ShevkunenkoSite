namespace ShevkunenkoSite.Controllers;

public class ArticlesController(
    IBooksAndArticlesRepository articleContext,
    ITextInfoRepository textContext,
    IPageInfoRepository pageContext,
    IImageFileRepository imageFileContext,
    IWebHostEnvironment hostEnvironment) : Controller
{
    private readonly string rootPath = hostEnvironment.WebRootPath;

    public IActionResult Index() => View();

    public async Task<IActionResult> Article(Guid? articleId, int? pageNumber, bool? scan)
    {
        if (articleId != null
            && await articleContext.BooksAndArticles.Where(article => article.BooksAndArticlesModelId == articleId).AnyAsync()
            && pageNumber != null
            && pageNumber > -1
            && pageNumber <= articleContext.BooksAndArticles.First(article => article.BooksAndArticlesModelId == articleId).NumberOfPages)
        {
            #region Если есть только скан

            if (!await textContext.Texts.Where(text => text.BooksAndArticlesModelId == articleId).AnyAsync() & scan == true)
            {
                ArticleViewModel scanForArticle = new()
                {
                    BookOrArticle = await articleContext.BooksAndArticles
                       .Include(logo => logo.LogoOfArticle)
                       .Include(scan => scan.ScanOfArticle)
                       .Include(movie => movie.VideoForBookOrArticle)
                       .FirstAsync(article => article.BooksAndArticlesModelId == articleId),

                    PageInfo = await pageContext.GetPageInfoByPathAsync(HttpContext),

                    HtmlText = null,

                    PageNumber = null,

                    Scan = true
                };

                return View("Article", scanForArticle);
            }

            #endregion

            #region Если для статьи или книги отсутствует текст

            if (!await textContext.Texts.Where(text => text.BooksAndArticlesModelId == articleId & text.SequenceNumber == pageNumber).AnyAsync())
            {
                return RedirectToAction(nameof(Index));
            }

            var textForBookOrArticle = await textContext.Texts.FirstAsync(text => text.BooksAndArticlesModelId == articleId & text.SequenceNumber == pageNumber);

            if (!System.IO.File.Exists(rootPath + DataConfig.TextsFolderPath + textForBookOrArticle.FolderForText + textForBookOrArticle.HtmlFileName))
            {
                return RedirectToAction(nameof(Index));
            }

            #endregion

            #region Экземпляр  книги или статьи

            var bookOrArticleItem = await articleContext.BooksAndArticles
                        .Include(logo => logo.LogoOfArticle)
                        .Include(scan => scan.ScanOfArticle)
                        .Include(movie => movie.VideoForBookOrArticle)
                        .Include(film => film.FilmForBookOrArticle)
                        .FirstAsync(article => article.BooksAndArticlesModelId == articleId);

            #endregion

            #region Html текст

            using StreamReader htmlText = new(rootPath + DataConfig.TextsFolderPath + textForBookOrArticle.FolderForText + textForBookOrArticle.HtmlFileName);

            var textWithMarkUp = htmlText.ReadToEnd();

            #endregion

            #region Кадры слева и справа

            FramesAroundMainContentModel framesAroundMainContent = new();

            var imageItems = await imageFileContext.ImageFiles
                .Where(img => img.SearchFilter.ToLower().Contains(bookOrArticleItem.CaptionOfText.ToLower()))
                .ToArrayAsync();

            imageItems = [.. imageItems.Shuffle()];

            if (imageItems.Length > 1)
            {
                framesAroundMainContent.FramesOnTheLeft = [.. imageItems.Take(imageItems.Length / 2)];
                framesAroundMainContent.FramesOnTheRight = [.. imageItems.Skip(imageItems.Length / 2)];
            }

            #endregion

            #region ViewModel

            ArticleViewModel bookOrArticle = new()
            {
                BookOrArticle = bookOrArticleItem,

                PageInfo = await pageContext.GetPageInfoByPathAsync(HttpContext),

                HtmlText = textWithMarkUp,

                PageNumber = pageNumber,

                Scan = scan,

                FramesAroundMainContent = framesAroundMainContent
            };

            #endregion

            return View("Article", bookOrArticle);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }
}