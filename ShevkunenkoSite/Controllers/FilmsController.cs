using Microsoft.IdentityModel.Tokens;

namespace ShevkunenkoSite.Controllers
{
    public class FilmsController
        (
        IFilmFileRepository filmContext
        )
        : Controller
    {
        #region Список фильмов

        [HttpGet]
        public async Task<IActionResult> Index
           (
           string? searchString,
           int pageNumber = 1
           )
        {
            var allFilmsSite = await filmContext.FilmFiles.Where(film => film.FilmInMainList == true).ToListAsync();

            if (!searchString.IsNullOrEmpty())
            {
                allFilmsSite = [.. allFilmsSite.FilmSiteSearch(searchString).OrderBy(filmSite => filmSite.FilmDatePublished)];
            }

            ItemsListViewModel itemList = new()
            {
                AllFilmFiles = [.. allFilmsSite
                     .Skip((pageNumber - 1) * DataConfig.NumberOfItemsPerPage)
                     .Take(DataConfig.NumberOfItemsPerPage)],

                #region Свойства PagingInfoViewModel

                TotalItems = allFilmsSite.Count,

                ItemsPerPage = DataConfig.NumberOfItemsPerPage,

                CurrentPage = pageNumber,

                SearchString = searchString ?? string.Empty,

                #endregion
            };

            return View(itemList);
        }

        #endregion

        public IActionResult Film(string? filmCaption, string? videoHosting)
        {
            return View();
        }
    }
}
