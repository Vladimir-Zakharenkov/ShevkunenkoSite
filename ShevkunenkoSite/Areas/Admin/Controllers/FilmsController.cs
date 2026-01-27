//Ignore Spelling: Org
namespace ShevkunenkoSite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class FilmsController(
    IFilmFileRepository filmContext,
    IImageFileRepository imageContext,
    IWebHostEnvironment hostEnvironment
    ) : Controller
{
    private readonly string rootPath = hostEnvironment.WebRootPath;

    readonly AddEditFilmViewModel filmItem = new();

    #region Добавить фильм в базу данных

    [HttpGet]
    public IActionResult AddFilm()
    {
        return View(filmItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(5_268_435_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
    public async Task<IActionResult?> AddFilm(
                [Bind(
                "FilmFileModelId," +
                "FileForFilmFormFile," +
                "FullFilmFormFile," +



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
        AddEditFilmViewModel filmItem)
    {
        return null;
        //    if (ModelState.IsValid)
        //    {
        //        #region Добавить картинку для фильма

        //        if (movieItem.ImageForMovieFormFile != null)
        //        {
        //            if (await imageContext.ImageFiles.Where(i => i.ImageFileName == movieItem.ImageForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == movieItem.ImageForMovieFormFile.FileName);

        //                movieItem.ImageFileModelId = imageFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.IconFileName == movieItem.ImageForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == movieItem.ImageForMovieFormFile.FileName);

        //                movieItem.ImageFileModelId = imageFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == movieItem.ImageForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == movieItem.ImageForMovieFormFile.FileName);

        //                movieItem.ImageFileModelId = imageFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == movieItem.ImageForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == movieItem.ImageForMovieFormFile.FileName);

        //                movieItem.ImageFileModelId = imageFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == movieItem.ImageForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == movieItem.ImageForMovieFormFile.FileName);

        //                movieItem.ImageFileModelId = imageFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == movieItem.ImageForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == movieItem.ImageForMovieFormFile.FileName);

        //                movieItem.ImageFileModelId = imageFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == movieItem.ImageForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == movieItem.ImageForMovieFormFile.FileName);

        //                movieItem.ImageFileModelId = imageFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == movieItem.ImageForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == movieItem.ImageForMovieFormFile.FileName);

        //                movieItem.ImageFileModelId = imageFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == movieItem.ImageForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == movieItem.ImageForMovieFormFile.FileName);

        //                movieItem.ImageFileModelId = imageFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == movieItem.ImageForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var imageFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == movieItem.ImageForMovieFormFile.FileName);

        //                movieItem.ImageFileModelId = imageFile.ImageFileModelId;
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("ImageForMovieFormFile", $"Добавьте картинку «{movieItem.ImageForMovieFormFile.FileName}» в базу данных");

        //                return View(movieItem);
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("ImageForMovieFormFile", $"Выберите картинку");

        //            return View(movieItem);
        //        }

        //        #endregion

        //        #region Добавить постер для фильма

        //        if (movieItem.PosterForMovieFormFile != null)
        //        {
        //            if (await imageContext.ImageFiles.Where(i => i.ImageFileName == movieItem.PosterForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var posterFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == movieItem.PosterForMovieFormFile.FileName);

        //                movieItem.MoviePosterString = posterFile.ImageFileName;
        //                movieItem.MoviePosterId = posterFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.IconFileName == movieItem.PosterForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var posterFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == movieItem.PosterForMovieFormFile.FileName);

        //                movieItem.MoviePosterString = posterFile.ImageFileName;
        //                movieItem.MoviePosterId = posterFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == movieItem.PosterForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var posterFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == movieItem.PosterForMovieFormFile.FileName);

        //                movieItem.MoviePosterString = posterFile.ImageFileName;
        //                movieItem.MoviePosterId = posterFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == movieItem.PosterForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var posterFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == movieItem.PosterForMovieFormFile.FileName);

        //                movieItem.MoviePosterString = posterFile.ImageFileName;
        //                movieItem.MoviePosterId = posterFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == movieItem.PosterForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var posterFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == movieItem.PosterForMovieFormFile.FileName);

        //                movieItem.MoviePosterString = posterFile.ImageFileName;
        //                movieItem.MoviePosterId = posterFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == movieItem.PosterForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var posterFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == movieItem.PosterForMovieFormFile.FileName);

        //                movieItem.MoviePosterString = posterFile.ImageFileName;
        //                movieItem.MoviePosterId = posterFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == movieItem.PosterForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var posterFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == movieItem.PosterForMovieFormFile.FileName);

        //                movieItem.MoviePosterString = posterFile.ImageFileName;
        //                movieItem.MoviePosterId = posterFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == movieItem.PosterForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var posterFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == movieItem.PosterForMovieFormFile.FileName);

        //                movieItem.MoviePosterString = posterFile.ImageFileName;
        //                movieItem.MoviePosterId = posterFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == movieItem.PosterForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var posterFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == movieItem.PosterForMovieFormFile.FileName);

        //                movieItem.MoviePosterString = posterFile.ImageFileName;
        //                movieItem.MoviePosterId = posterFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == movieItem.PosterForMovieFormFile.FileName).AnyAsync())
        //            {
        //                var posterFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == movieItem.PosterForMovieFormFile.FileName);

        //                movieItem.MoviePosterString = posterFile.ImageFileName;
        //                movieItem.MoviePosterId = posterFile.ImageFileModelId;
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("PosterForMovieFormFile", $"Добавьте картинку постера «{movieItem.PosterForMovieFormFile.FileName}» в базу данных");

        //                return View(movieItem);
        //            }
        //        }
        //        else
        //        {
        //            var posterFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileModelId == movieItem.ImageFileModelId);

        //            movieItem.MoviePosterString = posterFile.ImageFileName;
        //            movieItem.MoviePosterId = movieItem.ImageFileModelId;
        //        }

        //        #endregion

        //        #region Добавить информацию о сериях

        //        #region Добавить картинку заголовка серий

        //        if (movieItem.ImageHeadForSeriesFormFile != null && movieItem.MovieTotalParts > 1 && movieItem.MoviePart == 1)
        //        {
        //            if (await imageContext.ImageFiles.Where(i => i.ImageFileName == movieItem.ImageHeadForSeriesFormFile.FileName).AnyAsync())
        //            {
        //                var imageHeadSeriesFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileName == movieItem.ImageHeadForSeriesFormFile.FileName);

        //                movieItem.ImageForHeadSeriesId = imageHeadSeriesFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.IconFileName == movieItem.ImageHeadForSeriesFormFile.FileName).AnyAsync())
        //            {
        //                var imageHeadSeriesFile = await imageContext.ImageFiles.FirstAsync(i => i.IconFileName == movieItem.ImageHeadForSeriesFormFile.FileName);

        //                movieItem.ImageForHeadSeriesId = imageHeadSeriesFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.ImageHDFileName == movieItem.ImageHeadForSeriesFormFile.FileName).AnyAsync())
        //            {
        //                var imageHeadSeriesFile = await imageContext.ImageFiles.FirstAsync(i => i.ImageHDFileName == movieItem.ImageHeadForSeriesFormFile.FileName);

        //                movieItem.ImageForHeadSeriesId = imageHeadSeriesFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.Icon200FileName == movieItem.ImageHeadForSeriesFormFile.FileName).AnyAsync())
        //            {
        //                var imageHeadSeriesFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon200FileName == movieItem.ImageHeadForSeriesFormFile.FileName);

        //                movieItem.ImageForHeadSeriesId = imageHeadSeriesFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.Icon100FileName == movieItem.ImageHeadForSeriesFormFile.FileName).AnyAsync())
        //            {
        //                var imageHeadSeriesFile = await imageContext.ImageFiles.FirstAsync(i => i.Icon100FileName == movieItem.ImageHeadForSeriesFormFile.FileName);

        //                movieItem.ImageForHeadSeriesId = imageHeadSeriesFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebImageFileName == movieItem.ImageHeadForSeriesFormFile.FileName).AnyAsync())
        //            {
        //                var imageHeadSeriesFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageFileName == movieItem.ImageHeadForSeriesFormFile.FileName);

        //                movieItem.ImageForHeadSeriesId = imageHeadSeriesFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebIconFileName == movieItem.ImageHeadForSeriesFormFile.FileName).AnyAsync())
        //            {
        //                var imageHeadSeriesFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIconFileName == movieItem.ImageHeadForSeriesFormFile.FileName);

        //                movieItem.ImageForHeadSeriesId = imageHeadSeriesFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebImageHDFileName == movieItem.ImageHeadForSeriesFormFile.FileName).AnyAsync())
        //            {
        //                var imageHeadSeriesFile = await imageContext.ImageFiles.FirstAsync(i => i.WebImageHDFileName == movieItem.ImageHeadForSeriesFormFile.FileName);

        //                movieItem.ImageForHeadSeriesId = imageHeadSeriesFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebIcon200FileName == movieItem.ImageHeadForSeriesFormFile.FileName).AnyAsync())
        //            {
        //                var imageHeadSeriesFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon200FileName == movieItem.ImageHeadForSeriesFormFile.FileName);

        //                movieItem.ImageForHeadSeriesId = imageHeadSeriesFile.ImageFileModelId;
        //            }
        //            else if (await imageContext.ImageFiles.Where(i => i.WebIcon100FileName == movieItem.ImageHeadForSeriesFormFile.FileName).AnyAsync())
        //            {
        //                var imageHeadSeriesFile = await imageContext.ImageFiles.FirstAsync(i => i.WebIcon100FileName == movieItem.ImageHeadForSeriesFormFile.FileName);

        //                movieItem.ImageForHeadSeriesId = imageHeadSeriesFile.ImageFileModelId;
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("ImageHeadForSeriesFormFile", $"Добавьте картинку серий «{movieItem.ImageHeadForSeriesFormFile.FileName}» в базу данных");

        //                return View(movieItem);
        //            }
        //        }
        //        else
        //        {
        //            movieItem.ImageForHeadSeriesId = null;
        //        }

        //        #endregion

        //        #region Добавить ссылку на страницу серий фильма

        //        if (movieItem.MovieTotalParts > 1 && !string.IsNullOrEmpty(movieItem.PageForSeries))
        //        {
        //            movieItem.PageForSeries = "/" + movieItem.PageForSeries.Trim().Trim('/');

        //            if (!await pageInfoContext.PagesInfo.Where(p => p.PageFullPathWithData == movieItem.PageForSeries).AnyAsync())
        //            {
        //                ModelState.AddModelError("PageForSeries", "Указанной страницы нет в базе данных");

        //                return View(movieItem);
        //            }

        //            var pageForSeries = await pageInfoContext.PagesInfo.FirstAsync(p => p.PageFullPathWithData == movieItem.PageForSeries);

        //            movieItem.PageForMovieSeriesId = pageForSeries.PageInfoModelId;
        //        }
        //        else
        //        {
        //            movieItem.PageForMovieSeriesId = null;
        //        }

        //        #endregion

        //        movieItem.SeriesSearchFilter = movieItem.SeriesSearchFilter.Trim();

        //        #endregion

        //        #region Добавить файл фильма

        //        if (movieItem.FileForMovieFormFile != null)
        //        {
        //            if (!movieItem.FileForMovieFormFile.FileName.EndsWith(".mp4"))
        //            {
        //                ModelState.AddModelError("FileForMovieFormFile", "Формат фильмов на сайте «mp4»");

        //                return View(movieItem);
        //            }

        //            if (await movieInfoContext.MovieFiles.Where(c => c.MovieFileName == movieItem.FileForMovieFormFile.FileName).AnyAsync())
        //            {
        //                ModelState.AddModelError("FileForMovieFormFile", $"Файл {movieItem.FileForMovieFormFile.FileName} уже есть в базе данных"); ;

        //                return View(movieItem);
        //            }

        //            string path = Path.Combine(DataConfig.MovieFoldersPath, movieItem.FileForMovieFormFile.FileName);

        //            if (!System.IO.File.Exists(path))
        //            {
        //                using var stream = new FileStream(path, FileMode.Create);
        //                await movieItem.FileForMovieFormFile.CopyToAsync(stream);
        //            }

        //            IReadOnlyList<MetadataExtractor.Directory> movieDirectories = ImageMetadataReader.ReadMetadata(path);

        //            foreach (var movieDirectory in movieDirectories)
        //            {
        //                foreach (var tag in movieDirectory.Tags)
        //                {
        //                    if (movieDirectory.Name == "QuickTime Movie Header" && tag.Name == "Duration")
        //                    {
        //                        if (string.IsNullOrEmpty(tag.Description))
        //                        {
        //                            ModelState.AddModelError("movieItem.MovieDuration", "Продолжительность фильма равна 0");

        //                            return View(movieItem);
        //                        }
        //                        else
        //                        {
        //                            movieItem.MovieDuration = TimeSpan.Parse(tag.Description);
        //                        }
        //                    }

        //                    if (movieDirectory.Name == "QuickTime Track Header" && tag.Name == "Width" && Convert.ToUInt32(tag.Description) > 0)
        //                    {
        //                        if (string.IsNullOrEmpty(tag.Description) && !(Convert.ToUInt32(tag.Description) > 0))
        //                        {
        //                            ModelState.AddModelError("movieItem.MovieWidth", "Ширина кадра равна 0");

        //                            return View(movieItem);
        //                        }
        //                        else
        //                        {
        //                            movieItem.MovieWidth = Convert.ToUInt32(tag.Description);
        //                        }
        //                    }

        //                    if (movieDirectory.Name == "QuickTime Track Header" && tag.Name == "Height" && Convert.ToUInt32(tag.Description) > 0)
        //                    {
        //                        movieItem.MovieHeight = Convert.ToUInt32(tag.Description);

        //                        if (movieItem.MovieHeight == 0)
        //                        {
        //                            ModelState.AddModelError("movieItem.MovieHeight", "Высота кадра равна 0");

        //                            return View(movieItem);
        //                        }
        //                    }

        //                    if (movieDirectory.Name == "File" && tag.Name == "File Name")
        //                    {
        //                        if (string.IsNullOrEmpty(tag.Description))
        //                        {
        //                            ModelState.AddModelError("movieItem.MovieFileName", "Название файла не определено");

        //                            return View(movieItem);
        //                        }
        //                        else
        //                        {
        //                            movieItem.MovieFileName = tag.Description;
        //                        }
        //                    }

        //                    if (movieDirectory.Name == "File Type" && tag.Name == "Expected File Name Extension")
        //                    {
        //                        if (string.IsNullOrEmpty(tag.Description))
        //                        {
        //                            ModelState.AddModelError("movieItem.MovieFileExtension", "Расширение файла не определено");

        //                            return View(movieItem);
        //                        }
        //                        else
        //                        {
        //                            movieItem.MovieFileExtension = tag.Description;
        //                        }
        //                    }

        //                    if (movieDirectory.Name == "File Type" && tag.Name == "Detected MIME Type")
        //                    {
        //                        if (string.IsNullOrEmpty(tag.Description))
        //                        {
        //                            ModelState.AddModelError("movieItem.MovieMimeType", "MIME/TYPE файла не определен");

        //                            return View(movieItem);
        //                        }
        //                        else
        //                        {
        //                            movieItem.MovieMimeType = tag.Description;
        //                        }
        //                    }

        //                    if (movieDirectory.Name == "File" && tag.Name == "File Size")
        //                    {
        //                        if (string.IsNullOrEmpty(tag.Description))
        //                        {
        //                            ModelState.AddModelError("movieItem.MovieFileSize", "Размер файла равен 0");

        //                            return View(movieItem);
        //                        }
        //                        else
        //                        {
        //                            movieItem.MovieFileSize = Convert.ToUInt64(tag.Description[..tag.Description.IndexOf(' ')]);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            movieItem.MovieDuration = TimeSpan.Zero;
        //            movieItem.MovieWidth = 0;
        //            movieItem.MovieHeight = 0;
        //            movieItem.MovieFileName = string.Empty;
        //            movieItem.MovieFileExtension = string.Empty;
        //            movieItem.MovieMimeType = string.Empty;
        //            movieItem.MovieFileSize = 0;
        //            movieItem.MovieContentUrl = null;
        //        }

        //        #endregion

        //        #region Добавить ссылку на полный вариант фильма

        //        if (movieItem.FileForMovieFormFile != null & movieItem.FileForMovieFormFile?.FileName == movieItem.FullMovieFormFile?.FileName)
        //        {
        //            ModelState.AddModelError("FullMovieFormFile", $"Выбран {movieItem.FileForMovieFormFile?.FileName} один файл для отрывка и полного фильма."); ;

        //            return View(movieItem);

        //        }

        //        if (movieItem.FullMovieFormFile != null)
        //        {
        //            if (await movieInfoContext.MovieFiles.Where(mov => mov.MovieFileName == movieItem.FullMovieFormFile.FileName).AnyAsync())
        //            {
        //                fullMovie = await movieInfoContext.MovieFiles.FirstAsync(mov => mov.MovieFileName == movieItem.FullMovieFormFile.FileName);

        //                movieItem.FullMovieID = fullMovie.MovieFileModelId;
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("FullMovieFormFile", $"Файл фильма {movieItem.FullMovieFormFile.FileName} не найден в базе данных."); ;

        //                return View(movieItem);
        //            }
        //        }
        //        else
        //        {
        //            movieItem.FullMovieID = null;
        //            fullMovie = null;
        //        }

        //        #endregion

        //        #region Название (заголовок) фильма

        //        movieItem.MovieCaption = movieItem.MovieCaption.Trim();

        //        if (await movieInfoContext.MovieFiles.Where(c => c.MovieCaption == movieItem.MovieCaption & c.MovieTotalParts == movieItem.MovieTotalParts & c.MoviePart == movieItem.MoviePart).AnyAsync())
        //        {
        //            ModelState.AddModelError("movieItem.MovieCaption", $"Фильм «{movieItem.MovieCaption}» количество серий - {movieItem.MovieTotalParts} серия {movieItem.MoviePart} уже существует");

        //            return View(movieItem);
        //        }

        //        movieItem.MovieCaptionForOnline = movieItem.MovieCaptionForOnline.Trim();

        //        #endregion

        //        #region Добавить ссылки на видео хостинги

        //        movieItem.MovieContentUrl = new Uri("https://sergeyshef.ru/video/" + movieItem.MovieFileName);
        //        movieItem.MovieYouTube = movieItem.MovieYouTube;
        //        movieItem.MovieVkVideo = movieItem.MovieVkVideo;
        //        movieItem.MovieMailRuVideo = movieItem.MovieMailRuVideo;
        //        movieItem.MovieOkVideo = movieItem.MovieOkVideo;
        //        movieItem.MovieYandexDiskVideo = movieItem.MovieYandexDiskVideo;

        //        #endregion

        //        #region Добавить блок ссылок под фильмом

        //        #region Блок связанных видео 1

        //        movieItem.HeadTitleForVideoLinks1 = movieItem.HeadTitleForVideoLinks1.Trim();
        //        movieItem.SearchFilter1 = movieItem.SearchFilter1.Trim();
        //        movieItem.IconType1 = movieItem.IconType1.Trim();

        //        #region Тип картинки для ссылок (1)

        //        if (movieItem.ImageTypeForRef1 == "картинка")
        //        {
        //            movieItem.IsImage1 = true;
        //        }
        //        else if (movieItem.ImageTypeForRef1 == "постер")
        //        {
        //            movieItem.IsImage1 = false;
        //        }
        //        else
        //        {
        //            movieItem.IsImage1 = null;
        //        }

        //        #endregion

        //        #endregion

        //        #region Блок связанных видео 2

        //        movieItem.HeadTitleForVideoLinks2 = movieItem.HeadTitleForVideoLinks2.Trim();
        //        movieItem.SearchFilter2 = movieItem.SearchFilter2.Trim();
        //        movieItem.IconType2 = movieItem.IconType2.Trim();

        //        #region Тип картинки для ссылок (2)

        //        if (movieItem.ImageTypeForRef2 == "картинка")
        //        {
        //            movieItem.IsImage2 = true;
        //        }
        //        else if (movieItem.ImageTypeForRef2 == "постер")
        //        {
        //            movieItem.IsImage2 = false;
        //        }
        //        else
        //        {
        //            movieItem.IsImage2 = null;
        //        }

        //        #endregion

        //        #endregion

        //        #region Блок3

        //        movieItem.HeadTitleForVideoLinks3 = movieItem.HeadTitleForVideoLinks3.Trim();
        //        movieItem.SearchFilter3 = movieItem.SearchFilter3.Trim();
        //        movieItem.IconType3 = movieItem.IconType3.Trim();

        //        #region Тип картинки для ссылок (3)

        //        if (movieItem.ImageTypeForRef3 == "картинка")
        //        {
        //            movieItem.IsImage3 = true;
        //        }
        //        else if (movieItem.ImageTypeForRef3 == "постер")
        //        {
        //            movieItem.IsImage3 = false;
        //        }
        //        else
        //        {
        //            movieItem.IsImage3 = null;
        //        }

        //        #endregion

        //        #endregion

        //        #region Статья о видео 1

        //        movieItem.TextInfoModelId = movieItem.TextInfoModelId;

        //        if (!movieItem.TextInfoModelId.HasValue)
        //        {
        //            movieItem.TextInfoModelId = null;
        //        }

        //        #endregion

        //        #endregion

        //        #region Описание фильма

        //        movieItem.MovieDescriptionForSchemaOrg = movieItem.MovieDescriptionForSchemaOrg.Trim();
        //        movieItem.MovieDescriptionHtml = movieItem.MovieDescriptionHtml.Trim();

        //        #endregion

        //        #region Жанр фильма

        //        movieItem.MovieGenre = movieItem.MovieGenre.Trim();

        //        #endregion

        //        #region Примечания

        //        movieItem.MovieNote = movieItem.MovieNote.Trim();

        //        #endregion

        //        #region Съёмочная группа

        //        movieItem.MovieРroductionCompany = movieItem.MovieРroductionCompany.Trim();
        //        movieItem.MovieDirector1 = movieItem.MovieDirector1.Trim();
        //        movieItem.MovieDirector2 = movieItem.MovieDirector2.Trim();
        //        movieItem.MovieMusicBy = movieItem.MovieMusicBy.Trim();
        //        movieItem.MovieActor01 = movieItem.MovieActor01.Trim();
        //        movieItem.MovieActor02 = movieItem.MovieActor02.Trim();
        //        movieItem.MovieActor03 = movieItem.MovieActor03.Trim();
        //        movieItem.MovieActor04 = movieItem.MovieActor04.Trim();
        //        movieItem.MovieActor05 = movieItem.MovieActor05.Trim();
        //        movieItem.MovieActor06 = movieItem.MovieActor06.Trim();
        //        movieItem.MovieActor07 = movieItem.MovieActor07.Trim();
        //        movieItem.MovieActor08 = movieItem.MovieActor08.Trim();
        //        movieItem.MovieActor09 = movieItem.MovieActor09.Trim();
        //        movieItem.MovieActor10 = movieItem.MovieActor10.Trim();

        //        #endregion

        //        #region Фильтр поиска фильма

        //        if (movieItem.SearchFilter != null)
        //        {
        //            movieItem.SearchFilter = movieItem.SearchFilter.Trim();
        //        }
        //        else
        //        {
        //            movieItem.SearchFilter = string.Empty;
        //        }

        //        #endregion

        //        #region Темы видео

        //        var topicFilters = Request.Form["topicOptions"].ToString().Split(',');

        //        string topics = string.Empty;

        //        if (Request.Form["topicOptions"].Count > 0)
        //        {
        //            movieItem.TopicGuidList = Request.Form["topicOptions"].ToString().TrimEnd(',');
        //        }
        //        else
        //        {
        //            movieItem.TopicGuidList = string.Empty;
        //        }

        //        #endregion

        //        #region Ограничения по возрасту

        //        if (movieItem.MovieAdult)
        //        {
        //            movieItem.MovieAdult = true;
        //            movieItem.MovieIsFamilyFriendly = false;
        //        }
        //        else
        //        {
        //            movieItem.MovieAdult = false;
        //            movieItem.MovieIsFamilyFriendly = true;
        //        }

        //        #endregion

        //        #region Кадры справа и слева от фильма

        //        if (movieItem.FramesAroundMovie != string.Empty)
        //        {
        //            movieItem.FramesAroundMovie = movieItem.FramesAroundMovie.Trim();
        //        }
        //        else
        //        {
        //            movieItem.FramesAroundMovie = string.Empty;
        //        }

        //        #endregion

        //        #region Статья о фильме (1)

        //        if (movieItem.ArticleAboutMovie1FormFile != null)
        //        {
        //            if (await textInfoContext.Texts.Where(t => t.TxtFileName == movieItem.ArticleAboutMovie1FormFile.FileName).AnyAsync())
        //            {
        //                var textAboutMovie = await textInfoContext.Texts.FirstAsync(t => t.TxtFileName == movieItem.ArticleAboutMovie1FormFile.FileName);

        //                movieItem.TextInfoModelId = textAboutMovie.TextInfoModelId;
        //            }
        //            else if (await textInfoContext.Texts.Where(t => t.HtmlFileName == movieItem.ArticleAboutMovie1FormFile.FileName).AnyAsync())
        //            {
        //                var textAboutMovie = await textInfoContext.Texts.FirstAsync(t => t.HtmlFileName == movieItem.ArticleAboutMovie1FormFile.FileName);

        //                movieItem.TextInfoModelId = textAboutMovie.TextInfoModelId;
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("ArticleAboutMovie1FormFile", $"Файла «{movieItem.ArticleAboutMovie1FormFile.FileName}» нет в базе данных"); ;

        //                return View(movieItem);
        //            }

        //        }
        //        else
        //        {
        //            movieItem.TextInfoModelId = null;
        //        }

        //        #endregion

        //        #region Сохранить данные

        //        await movieInfoContext.AddNewMovieAsync(movieItem);

        //        var newMovie = await movieInfoContext.MovieFiles.FirstAsync(mov => mov.MovieCaption == movieItem.MovieCaption);

        //        return RedirectToAction("DetailsMovie", new { movieId = newMovie.MovieFileModelId, Area = "Admin" });

        //        #endregion
        //    }
        //    else
        //    {
        //        movieItem.TopicsForMovie = topicsForMovie;

        //        return View(movieItem);
        //    }
    }

    #endregion
}