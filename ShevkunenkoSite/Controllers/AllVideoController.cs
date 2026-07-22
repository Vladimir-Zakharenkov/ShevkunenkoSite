// Ignore Spelling: Programmy

namespace ShevkunenkoSite.Controllers;

//[Route("/All-Video/[action]")]
public class AllVideoController(
    IMovieFileRepository movieContext,
    IImageFileRepository imageContext,
    ITopicMovieRepository topicMovieContext,
    IPageInfoRepository pageContext,
    IWebHostEnvironment hostEnvironment
    ) : Controller
{
    private readonly string rootPath = hostEnvironment.WebRootPath;

    #region Фильмы на сайте

    public async Task<ViewResult> Index(Guid? topicId, int pageNumber = 1)
    {
        MoviesListViewModel moviesListViewModel = new();

        List<PageInfoModel> pagesForMovies = [];

        // выборка фильмов по теме (topicId)
        if (topicId.HasValue
                && await topicMovieContext.TopicMovies.Where(t => t.TopicMovieModelId == topicId).AnyAsync()
                && await movieContext.MovieFiles.Where(p => p.TopicGuidList.Contains(topicId.ToString()!)).AnyAsync())
        {
            TopicMovieModel topicMovie = await topicMovieContext.TopicMovies.FirstAsync(mt => mt.TopicMovieModelId == topicId);

            moviesListViewModel.Movies = await movieContext.MovieFiles
                .Where(p => p.TopicGuidList.Contains(topicId.ToString()!) == true /*& p.MovieInMainList == true*/)
                .OrderBy(p => p.MovieDatePublished)
                .Skip((pageNumber - 1) * DataConfig.NumberOfItemsPerPage)
                .Take(DataConfig.NumberOfItemsPerPage)
                .ToArrayAsync();

            moviesListViewModel.PageHeadTitle = topicMovie.TopicHeadPage;

            moviesListViewModel.IsImage = topicMovie.ImageForRef;

            moviesListViewModel.IconType = topicMovie.IconTypeForRef;

            moviesListViewModel.IsPartsMoreOne = topicMovie.GeneralPageForMovieEpisodes;

            moviesListViewModel.PagingInfo = new PagingInfoViewModel
            {
                CurrentPage = pageNumber,
                ItemsPerPage = DataConfig.NumberOfItemsPerPage,
                TotalItems = movieContext.MovieFiles
                    .Where(p => p.TopicGuidList.Contains(topicId.ToString()!) == true).Count()
            };
        }
        // выборка всех фильмов
        else
        {
            moviesListViewModel.Movies = await movieContext.MovieFiles
                .Where(p => p.MovieInMainList == true)
                .OrderBy(p => p.MovieDatePublished)
                .Skip((pageNumber - 1) * DataConfig.NumberOfItemsPerPage)
                .Take(DataConfig.NumberOfItemsPerPage)
                .ToArrayAsync();

            moviesListViewModel.PageHeadTitle = "ВИДЕО НА САЙТЕ";

            moviesListViewModel.IsImage = false;

            moviesListViewModel.IconType = "webicon300";

            moviesListViewModel.IsPartsMoreOne = true;

            moviesListViewModel.PagingInfo = new PagingInfoViewModel
            {
                CurrentPage = pageNumber,
                ItemsPerPage = DataConfig.NumberOfItemsPerPage,
                TotalItems = movieContext.MovieFiles
                    .Where(p => p.MovieInMainList == true)
                    .Count()
            };
        }

        // если выбрана картинка для страницы видео
        if (moviesListViewModel.IsImage == null)
        {
            var pagesIdOfMovies = from p in moviesListViewModel.Movies
                                  select p.PageInfoModelId;

            foreach (var p in pagesIdOfMovies)
            {
                if (await pageContext.PagesInfo.Where(i => i.PageInfoModelId == p).AnyAsync())
                {
                    var pg = await pageContext.PagesInfo.FirstAsync(i => i.PageInfoModelId == p);

                    pagesForMovies.Add(pg);
                }
            }

            moviesListViewModel.MoviePages = pagesForMovies;
        }
        else
        {
            moviesListViewModel.MoviePages = [];
        }

        return View(moviesListViewModel);
    }

    [Route("video-na-saite/shevkunenko-roli-v-kino")]
    [ActionName("Shevkunenko-roli-v-kino")]
    public async Task<ViewResult> Shevkunenko() =>
        View("Index", new MoviesListViewModel
        {
            Movies = await movieContext.MovieFiles
                .AsNoTracking()
                .Where(p => p.MovieInMainList == true & p.SearchFilter.Contains("Сергей Шевкуненко - роли в кино,"))
                .OrderBy(p => p.MovieDatePublished)
                .ToArrayAsync(),

            IsImage = false,

            IsPartsMoreOne = true,

            PageHeadTitle = "СЕРГЕЙ ШЕВКУНЕНКО - РОЛИ В КИНО"
        });

    [Route("video-na-saite/programmy-o-sergee-shevkunenko")]
    [ActionName("Programmy-o-Sergee-Shevkunenko")]
    public async Task<ViewResult> Programmy() => View();

    [Route("video-na-saite/lihie-90-e")]
    [ActionName("lihie-90-e")]
    public async Task<ViewResult> Lihie90() =>
        View("Index", new MoviesListViewModel
        {
            Movies = await movieContext.MovieFiles
                .AsNoTracking()
                .Where(p => p.SearchFilter.Contains("Программа «Лихие 90-е»,"))
                .OrderBy(p => p.MovieDatePublished)
                .ToArrayAsync(),

            IsImage = true,

            IconType = "webicon300",

            IsPartsMoreOne = false,

            AllMoviesFromDB = true,

            PageHeadTitle = "<h1 class=\"fs-2 text_shadow\">ПРОГРАММА «ЛИХИЕ 90-е»</h1>"
        });

    [Route("video-na-saite/bbc-top-100-us-movies")]
    [ActionName("bbc-top-100-us-movies")]
    public async Task<ViewResult> BbcTop100() =>
        View("Index", new MoviesListViewModel
        {
            Movies = await movieContext.MovieFiles
                .AsNoTracking()
                .Where(p => p.SearchFilter.Contains("BBC Top 100 US,"))
                .OrderBy(p => p.MovieUploadDate)
                .ToArrayAsync(),
            IsImage = false,
            IconType = "webicon300",
            IsPartsMoreOne = true,
            AllMoviesFromDB = false,
            PageHeadTitle = "<h1 class=\"fs-2 text_shadow\">The&nbsp;100&nbsp;greatest American&nbsp;films&nbsp;by&nbsp;BBC</h1>"
        });

    public async Task<ActionResult> Movie(Guid? movieId, string? videoHosting, Guid? imageID, bool? pageOfSeries = false, string? topic = null)
    {
        if ((movieId == null & topic == null) || (topic != null & !await movieContext.MovieFiles.Where(p => p.TopicGuidList.Contains(topic + ',')).AnyAsync()))
        {
            return RedirectToAction(nameof(Index));
        }

        if (topic != null)
        {
            return View("Topic", new MoviesListViewModel
            {
                Movies = await movieContext.MovieFiles
                .AsNoTracking()
                .Where(p => p.SearchFilter.Contains(topic + ","))
                .OrderBy(p => p.MovieUploadDate)
                .ToArrayAsync(),
                IsImage = null,
                IconType = "webicon300",
                IsPartsMoreOne = true,
                AllMoviesFromDB = false,
                PageHeadTitle = topic
            });
        }
        else if (movieId.HasValue && await movieContext.MovieFiles.Where(m => m.MovieFileModelId == movieId).AnyAsync())
        {
            MovieFileModel movieItem = await movieContext.MovieFiles.Include(t => t.TextInfoModel).FirstAsync(m => m.MovieFileModelId == movieId);

            if (pageOfSeries == true && movieItem.MovieTotalParts > 1 && !string.IsNullOrEmpty(movieItem.SeriesSearchFilter))
            {
                MovieFileModel[] allSeries = await movieContext.MovieFiles.Where(m => m.SeriesSearchFilter == movieItem.SeriesSearchFilter).ToArrayAsync();

                if (allSeries.Length > 0)
                {
                    allSeries = [.. allSeries.OrderBy(allSeries => allSeries.MoviePart)];
                }

                return View("Series", new SeriesViewModel
                {
                    AllSeriesMovies = allSeries,
                    HeadImageSeries = allSeries[0].ImageForHeadSeriesId,
                    IsImage = true,
                    IconType = "webicon300",
                    IsPartsMoreOne = false,
                    MovieInMainList = false,
                    HeadTitleForVideoLinks = string.Empty
                });
            }
            else if (imageID == null)
            {
                MovieFileModel? fullMovie = null;

                if (movieItem.FullMovieID != null && await movieContext.MovieFiles.Where(m => m.MovieFileModelId == movieItem.FullMovieID).AnyAsync())
                {
                    fullMovie = await movieContext.MovieFiles.AsNoTracking().FirstAsync(m => m.MovieFileModelId == movieItem.FullMovieID);
                }

                bool sergeyshefRu = false;

                Uri? videoRef;

                #region Статья о фильме (ArticleAboutMovie1)

                string articleAboutMovie1 = string.Empty;

                if (movieItem.TextInfoModel != null)
                {
                    StreamReader htmlText = new(rootPath + DataConfig.TextsFolderPath + movieItem.TextInfoModel.HtmlFileName);

                    articleAboutMovie1 = htmlText.ReadToEnd();
                }

                #endregion

                if (!string.IsNullOrEmpty(videoHosting))
                {
                    if (videoHosting.Contains("vk.com") || videoHosting.Contains("vkvideo.ru"))
                    {
                        videoRef = movieItem.MovieVkVideo;
                    }
                    else if (videoHosting.Contains("mail.ru"))
                    {
                        videoRef = movieItem.MovieMailRuVideo;
                    }
                    else if (videoHosting.Contains("ok.ru"))
                    {
                        videoRef = movieItem.MovieOkVideo;
                    }
                    else if (videoHosting.Contains("youtube.com"))
                    {
                        videoRef = movieItem.MovieYouTube;
                    }
                    else
                    {
                        videoRef = movieItem.MovieContentUrl;
                        sergeyshefRu = true;
                    }
                }
                else
                {
                    videoRef = movieItem.MovieYouTube;
                }

                return View("Movie", new MoviePageViewModel
                {
                    MovieFile = movieItem,
                    FullMovie = fullMovie,
                    SergeyshefRu = sergeyshefRu,
                    VideoRef = videoRef!,
                    ArticleAboutMovie1 = articleAboutMovie1
                });
            }
            else if (imageID != null)
            {
                ImageFileModel imageItem = await imageContext.ImageFiles.FirstAsync(p => p.ImageFileModelId == imageID);

                ImageFileModel[] imageArray = [];

                if (movieItem.MovieCaption == "Пропавшая экспедиция - 1" || movieItem.MovieCaption == "Пропавшая экспедиция - 2")
                {
                    imageArray = await imageContext.ImageFiles.Where(p => p.SearchFilter.Contains("Пропавшая экспедиция - альбом,")).ToArrayAsync();
                }
                else if (movieItem.MovieCaption.Contains("Интервью"))
                {
                    imageArray = await imageContext.ImageFiles.Where(p => p.SearchFilter.Contains("Криминальная звезда,")).ToArrayAsync();
                }
                else
                {
                    imageArray = await imageContext.ImageFiles.Where(p => p.SearchFilter.Contains(movieItem.MovieCaption ?? string.Empty)).ToArrayAsync();
                }

                return View("Frames", new FramesViewModel
                {
                    MovieFile = movieItem,
                    ImageArray = imageArray,
                    ImageFile = imageItem
                });
            }
            else
            {
                return View("NoMovie");
            }
        }
        else
        {
            return View("NoMovie");
        }
    }

    // страница для многосерийных фильмов
    public ViewResult MoviePage()
    {
        return View();
    }

    #endregion
}