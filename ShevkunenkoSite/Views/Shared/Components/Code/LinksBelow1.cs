namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class LinksBelow1(
    IMovieFileRepository movieContext,
    IPageInfoRepository pageContext
    ) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(MovieFileModel movie)
    {
        string[] movieFilters = movie.SearchFilter1.Split(',', StringSplitOptions.RemoveEmptyEntries);

        IEnumerable<MovieFileModel> refMovies = [];

        List<MovieFileModel> onList = [];

        if (movie.IsImage1 != null)
        {
            if (!movie.AllMoviesFromDB1)
            {
                for (int i = 0; i < movieFilters.Length; i++)
                {
                    refMovies = await movieContext.MovieFiles.AsNoTracking().Where(mov => mov.SearchFilter.Contains(movieFilters[i].Trim() + ",") & mov.MovieInMainList == true).ToArrayAsync();

                    onList.AddRange(refMovies);
                }
            }
            else
            {
                for (int i = 0; i < movieFilters.Length; i++)
                {
                    refMovies = await movieContext.MovieFiles.AsNoTracking().Where(mov => mov.SearchFilter.Contains(movieFilters[i].Trim() + ",")).ToArrayAsync();

                    onList.AddRange(refMovies);
                }
            }

            refMovies = onList.Distinct();

            MoviesListViewModel movies = new()
            {
                Movies = refMovies,
                IsImage = movie.IsImage1,
                IconType = movie.IconType1,
                IsPartsMoreOne = movie.IsPartsMoreOne1,
                PageHeadTitle = movie.HeadTitleForVideoLinks1
            };

            return View(movies);
        }
        else
        {
            List<PageInfoModel> listOfPageLinks = [];

            List<List<PageInfoModel>> listsOfPageLinks = [];

            if (movieFilters.Length > 0)
            {
                for (int i = 0; i < movieFilters.Length; i++)
                {
                    refMovies = await movieContext.MovieFiles.AsNoTracking().Where(mov => mov.SearchFilter.Contains(movieFilters[i].Trim() + ",")).ToArrayAsync();

                    _= refMovies.OrderBy(s => s.MoviePart);

                    foreach (var item in refMovies)
                    {
                        if (item.PageInfoModelId != null)
                        {
                            var pageModel = await pageContext.PagesInfoNoTracking.FirstAsync(p => p.PageInfoModelId == item.PageInfoModelId);

                            listOfPageLinks.Add(pageModel);
                        }
                    }

                    listsOfPageLinks.Add(listOfPageLinks);

                    listOfPageLinks = [];
                }
            }

            return View("PageLinks", listsOfPageLinks);
        }
    }
}