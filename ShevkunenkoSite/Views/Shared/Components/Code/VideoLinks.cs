namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class VideoLinks(
    IMovieFileRepository movieContext,
    IPageInfoRepository pageContext
    ) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(VideoLinksViewModel moviesList, bool? linkToInfoAboutMovie = null)
    {
        // Если постер или картинка для фильма
        if (moviesList.IsImage != null)
        {
            if (moviesList.MovieInMainList == true)
            {
                MoviesListViewModel movies = new()
                {
                    Movies = await movieContext.MovieFiles
                      .Where(mov => mov.MovieInMainList == moviesList.MovieInMainList)
                      .Where(mov => mov.SearchFilter.Contains(moviesList.SearchFilter ?? string.Empty))
                      .OrderBy(mov => mov.MovieDatePublished)
                      .ToArrayAsync(),

                    IsPartsMoreOne = moviesList.IsPartsMoreOne,
                    PageHeadTitle = moviesList.HeadTitleForVideoLinks,
                    IsImage = moviesList.IsImage,
                    IconType = moviesList.IconType,
                    LinkToInfoAboutMovie = linkToInfoAboutMovie
                };

                if (movies.Movies.Where(m => m.MoviePosterId == null).Any())
                {
                    movies.IsImage = true;
                }

                return View(movies);
            }
            else
            {
                MoviesListViewModel movies = new()
                {
                    Movies = await movieContext.MovieFiles
                     .Where(p => p.SearchFilter.Contains(moviesList.SearchFilter ?? string.Empty))
                     .OrderBy(p => p.MovieDatePublished)
                     .ToArrayAsync(),
                    IsPartsMoreOne = moviesList.IsPartsMoreOne,
                    PageHeadTitle = moviesList.HeadTitleForVideoLinks,
                    IsImage = moviesList.IsImage,
                    IconType = moviesList.IconType,
                    LinkToInfoAboutMovie = linkToInfoAboutMovie
                };

                return View(movies);
            }

        }
        // Если страница для фильма
        else
        {
            List<PageInfoModel> listOfPageLinks = [];

            List<List<PageInfoModel>> listsOfPageLinks = [];

            if (moviesList.MovieInMainList == true)
            {
                var movies = await movieContext.MovieFiles
                    .Where(p => p.MovieInMainList == moviesList.MovieInMainList)
                    .Where(p => p.SearchFilter.Contains(moviesList.SearchFilter ?? string.Empty))
                    .OrderBy(p => p.MovieDatePublished)
                    .ToListAsync();

                for (int i = 0; i < movies.Count; i++)
                {
                    foreach (var item in movies)
                    {
                        if (item.PageInfoModelId != null)
                        {
                            var pageModel = await pageContext.PagesInfoNoTracking.FirstAsync(p => p.PageInfoModelId == item.PageInfoModelId);

                            listOfPageLinks.Add(pageModel);
                        }
                    }
                }

                listsOfPageLinks.Add(listOfPageLinks);

                listOfPageLinks = [];
            }
            else
            {
                var movies = await movieContext.MovieFiles
                     .Where(p => p.SearchFilter.Contains(moviesList.SearchFilter ?? string.Empty))
                     .OrderBy(p => p.MovieDatePublished)
                     .ToListAsync();

                //for (int i = 0; i < movies.Count; i++)
                //{
                foreach (var item in movies)
                {
                    if (item.PageInfoModelId != null)
                    {
                        var pageModel = await pageContext.PagesInfoNoTracking.FirstAsync(p => p.PageInfoModelId == item.PageInfoModelId);

                        listOfPageLinks.Add(pageModel);
                    }
                }
                //}

                listsOfPageLinks.Add(listOfPageLinks);

                listOfPageLinks = [];
            }

            return View("PageLinks", listsOfPageLinks);
        }
    }
}