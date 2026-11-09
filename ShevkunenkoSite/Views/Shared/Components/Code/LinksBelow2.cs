using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class LinksBelow2(
    IMovieFileRepository movieContext,
    IPageInfoRepository pageContext
    ) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(MovieFileModel movie)
    {
        string[] movieFilters = movie.SearchFilter2.Split(',', StringSplitOptions.RemoveEmptyEntries);

        IEnumerable<MovieFileModel> refMovies = [];

        List<MovieFileModel> onList = [];

        if (movie.IsImage2 != null)
        {
            if (!movie.AllMoviesFromDB2)
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
                IsImage = movie.IsImage2,
                IconType = movie.IconType2,
                IsPartsMoreOne = movie.IsPartsMoreOne2,
                PageHeadTitle = movie.HeadTitleForVideoLinks2
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

                    _ = refMovies.OrderBy(s => s.MoviePart);

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