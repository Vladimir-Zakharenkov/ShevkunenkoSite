using Microsoft.IdentityModel.Tokens;
using System.Runtime.Intrinsics.X86;

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

        #region Фильм

        [HttpGet]
        public async Task<IActionResult> Film(string? filmCaption, string? host)
        {
            if (string.IsNullOrEmpty(filmCaption) || !await filmContext.FilmFiles.Where(film => film.FilmCaption == filmCaption).AnyAsync())
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                FilmFileModel film = await filmContext.FilmFiles
                    .AsNoTracking()
                    .Include(image => image.FilmImage)
                    .FirstAsync(film => film.FilmCaption == filmCaption);

                if (host == "yt" && film.FilmYouTube != null)
                {
                    film.CurrentVideoHost = film.FilmYouTube;
                }
                else if (host == "vk" && film.FilmVkVideo != null)
                {
                    film.CurrentVideoHost = film.FilmVkVideo;
                }
                else if (host == "ok" && film.FilmOkVideo != null)
                {
                    film.CurrentVideoHost = film.FilmOkVideo;
                }
                else if (host == "ml" && film.FilmMailRuVideo != null)
                {
                    film.CurrentVideoHost = film.FilmMailRuVideo;
                }
                else
                {
                    film.CurrentVideoHost = film.FilmContentUrl;
                }

                return View(film);
            }
        }

        #endregion
    }
}
