//Ignore Spelling: Org
using Microsoft.IdentityModel.Tokens;

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

    readonly FilmFileAddDTOModel filmItem = new();

    #region Список фильмов

    public async Task<ViewResult> Index
       (
       string? searchString,
       int pageNumber = 1,
       bool pageCard = false
       )
    {
        var allFilmsSite = await filmContext.FilmFiles.ToListAsync();

        if (!searchString.IsNullOrEmpty())
        {
            allFilmsSite = [.. allFilmsSite.FilmSearch(searchString).OrderBy(filmSite => filmSite.FilmDatePublished)];
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

            PageCard = pageCard

            #endregion
        };

        if (pageCard == false)
        {
            return View(itemList);
        }
        else
        {
            return View("FilmCards", itemList);
        }
    }

    #endregion

    #region Информация о фильме

    public async Task<IActionResult> DetailsFilm(Guid? filmId)
    {
        if (filmId.HasValue && await filmContext.FilmFiles.Where(film => film.FilmFileModelId == filmId).AnyAsync())
        {
            FilmFileModel filmItem = await filmContext.FilmFiles
                .Include(img => img.FilmImage)
                .Include(img => img.FilmPoster)
                .Include(film => film.FullFilm)
                .AsNoTracking()
                .FirstAsync(film => film.FilmFileModelId == filmId);

            return View(filmItem);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

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
                "FileForFilmFormFile," +
                "FullFilmFormFile," +
                "FilmCaption," +
                "FilmCaptionOriginal," +
                "FilmDescriptionForSchemaOrg," +
                "FilmDescriptionHtml," +
                "FilmNote," +
                "FilmInMainList," +
                "SearchFilterForFilm," +
                "FilmGenre," +
                "FilmDateCreated," +
                "FilmDatePublished," +
                "FilmUploadDate," +
                "FilmInLanguage1," +
                "FilmInLanguage2," +
                "FilmSubtitles1," +
                "FilmSubtitles2," +
                "FilmРroductionCompany," +
                "FilmDirector1," +
                "FilmDirector2," +
                "FilmMusicBy," +
                "FilmActor01," +
                "FilmActor02," +
                "FilmActor03," +
                "FilmActor04," +
                "FilmActor05," +
                "FilmActor06," +
                "FilmActor07," +
                "FilmActor08," +
                "FilmActor09," +
                "FilmActor10," +
                "FilmYouTube," +
                "FilmVkVideo," +
                "FilmMailRuVideo," +
                "FilmOkVideo," +
                "FilmYandexDiskVideo," +
                "FilmKinoTeatrRu," +
                "FilmKinoPoisk," +
                "FilmImbd," +
                "SeriesSearchFilter," +
                "FilmTotalParts," +
                "FilmPart," +
                "PosterForFilmFormFile," +
                "ImageForFilmFormFile"
        )]
        FilmFileAddDTOModel filmItem)
    {
        if (ModelState.IsValid)
        {
            FilmFileModel addFilm = new();

            #region Добавить файл фильма

            #region Проверка расширения выбранного файла

            if (!filmItem.FileForFilmFormFile.FileName.EndsWith(".mp4"))
            {
                ModelState.AddModelError("FileForFilmFormFile", $"Вы выбрали файл {filmItem.FileForFilmFormFile.FileName}" + Environment.NewLine + "Формат фильмов на сайте  должен быть «mp4»");

                return View(filmItem);
            }

            #endregion

            #region Поиск имени файла в базе данных

            if (await filmContext.FilmFiles.Where(film => film.FilmFileName == filmItem.FileForFilmFormFile.FileName).AnyAsync())
            {
                ModelState.AddModelError("FileForFilmFormFile", $"Вы выбрали файл «{filmItem.FileForFilmFormFile.FileName}»" + Environment.NewLine + "Файл с таким именем уже есть в базе данных");

                return View(filmItem);
            }

            #endregion

            #region Копируем выбранный файл в папку DataConfig.MovieFoldersPath

            string path = Path.Combine(DataConfig.MovieFoldersPath, filmItem.FileForFilmFormFile.FileName);

            if (!System.IO.File.Exists(path))
            {
                using var stream = new FileStream(path, FileMode.Create);
                await filmItem.FileForFilmFormFile.CopyToAsync(stream);
            }

            #endregion

            #region Определение параметров файла

            IReadOnlyList<MetadataExtractor.Directory> filmDirectories = ImageMetadataReader.ReadMetadata(path);

            foreach (var movieDirectory in filmDirectories)
            {
                foreach (var tag in movieDirectory.Tags)
                {
                    #region Продолжительность фильма FilmDuration

                    if (movieDirectory.Name == "QuickTime Movie Header" && tag.Name == "Duration")
                    {
                        if (string.IsNullOrEmpty(tag.Description))
                        {
                            ModelState.AddModelError("filmItem.FilmDuration", "Продолжительность фильма равна 0");

                            return View(filmItem);
                        }
                        else
                        {
                            addFilm.FilmDuration = TimeSpan.Parse(tag.Description);
                        }
                    }

                    #endregion

                    #region Ширина кадра FilmWidth

                    if (movieDirectory.Name == "QuickTime Track Header" && tag.Name == "Width" && Convert.ToInt32(tag.Description) > 0)
                    {
                        addFilm.FilmWidth = Convert.ToInt32(tag.Description);
                    }

                    #endregion

                    #region Высота кадра FilmHeight

                    if (movieDirectory.Name == "QuickTime Track Header" && tag.Name == "Height" && Convert.ToInt32(tag.Description) > 0)
                    {
                        addFilm.FilmHeight = Convert.ToInt32(tag.Description);
                    }

                    #endregion

                    #region Имя файла

                    if (movieDirectory.Name == "File" && tag.Name == "File Name")
                    {
                        if (string.IsNullOrEmpty(tag.Description))
                        {
                            ModelState.AddModelError("filmItem.FilmFileName", "Название файла не определено");

                            return View(filmItem);
                        }
                        else
                        {
                            addFilm.FilmFileName = tag.Description;
                        }
                    }

                    #endregion

                    #region Расширение файла

                    if (movieDirectory.Name == "File Type" && tag.Name == "Expected File Name Extension")
                    {
                        if (string.IsNullOrEmpty(tag.Description))
                        {
                            ModelState.AddModelError("filmItem.FilmFileExtension", "Расширение файла не определено");

                            return View(filmItem);
                        }
                        else
                        {
                            addFilm.FilmFileExtension = tag.Description;
                        }
                    }

                    #endregion

                    #region Определение MIME Type

                    if (movieDirectory.Name == "File Type" && tag.Name == "Detected MIME Type")
                    {
                        if (string.IsNullOrEmpty(tag.Description))
                        {
                            ModelState.AddModelError("filmItem.FilmMimeType", "MIME/TYPE файла не определен");

                            return View(filmItem);
                        }
                        else
                        {
                            addFilm.FilmMimeType = tag.Description;
                        }
                    }

                    #endregion

                    #region Размер файла

                    if (movieDirectory.Name == "File" && tag.Name == "File Size")
                    {
                        if (string.IsNullOrEmpty(tag.Description))
                        {
                            ModelState.AddModelError("filmItem.FilmFileSize", "Размер файла равен 0");

                            return View(filmItem);
                        }
                        else
                        {
                            addFilm.FilmFileSize = Convert.ToUInt64(tag.Description[..tag.Description.IndexOf(' ')]);
                        }
                    }

                    #endregion
                }
            }

            #region Проверка ширины и высоты кадра

            if (addFilm.FilmWidth < 1)
            {
                ModelState.AddModelError("filmItem.FilmWidth", "Ширина кадра равна 0");

                return View(filmItem);
            }

            if (addFilm.FilmHeight < 1)
            {
                ModelState.AddModelError("filmItem.FilmHeight", "Высота кадра равна 0");

                return View(filmItem);
            }

            #endregion

            #endregion

            #endregion

            #region Ссылка на полную версию фильма

            if (filmItem.FileForFilmFormFile != null)
            {
                if (filmItem.FullFilmFormFile != null)
                {
                    if (filmItem.FileForFilmFormFile.FileName == filmItem.FullFilmFormFile.FileName)
                    {
                        ModelState.AddModelError("FullFilmFormFile", $"Выбран файл «{filmItem.FileForFilmFormFile.FileName}» для полной и неполной версии."); ;

                        return View(filmItem);
                    }

                    if (await filmContext.FilmFiles.Where(film => film.FilmFileName == filmItem.FullFilmFormFile.FileName).AnyAsync())
                    {
                        var fullFilm = await filmContext.FilmFiles.FirstAsync(film => film.FilmFileName == filmItem.FullFilmFormFile.FileName);

                        addFilm.FullFilmId = fullFilm.FilmFileModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("FullFilmFormFile", $"Файл «{filmItem.FullFilmFormFile.FileName}» не найден в базе данных."); ;

                        return View(filmItem);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("FileForFilmFormFile", "Выберите файл фильма."); ;

                return View(filmItem);
            }

            #endregion

            #region Название фильма в базе данных

            if (await filmContext.FilmFiles.Where(film => film.FilmCaption == filmItem.FilmCaption).AnyAsync())
            {
                ModelState.AddModelError("filmItem.FilmCaption", $"Фильм «{filmItem.FilmCaption}» уже существует.");

                return View(filmItem);
            }
            else
            {
                addFilm.FilmCaption = filmItem.FilmCaption.Trim();
            }

            #endregion

            #region Оригинальное название фильма

            if (!string.IsNullOrEmpty(filmItem.FilmCaptionOriginal.Trim()))
            {
                if (await filmContext.FilmFiles.Where(film => film.FilmCaptionOriginal == filmItem.FilmCaptionOriginal).AnyAsync())
                {
                    ModelState.AddModelError("filmItem.FilmCaptionOriginal", $"Фильм «{filmItem.FilmCaptionOriginal}» уже существует.");

                    return View(filmItem);
                }
                else
                {
                    addFilm.FilmCaptionOriginal = filmItem.FilmCaptionOriginal.Trim();
                }
            }

            #endregion

            #region Краткое содержание, примечания админа

            addFilm.FilmDescriptionForSchemaOrg = filmItem.FilmDescriptionForSchemaOrg.Trim();

            addFilm.FilmDescriptionHtml = filmItem.FilmDescriptionHtml.Trim();

            if (!string.IsNullOrEmpty(filmItem.FilmNote))
            {
                addFilm.FilmNote = filmItem.FilmNote.Trim();
            }

            #endregion

            #region Критерии поиска

            addFilm.FilmInMainList = filmItem.FilmInMainList;

            if (filmItem.SearchFilterForFilm != null)
            {
                addFilm.SearchFilterForFilm = filmItem.SearchFilterForFilm.Trim();
            }

            addFilm.FilmGenre = filmItem.FilmGenre.Trim();

            addFilm.FilmAdult = filmItem.FilmAdult;

            #endregion

            #region Язык и субтитры

            addFilm.FilmInLanguage1 = filmItem.FilmInLanguage1.Trim();

            if (filmItem.FilmInLanguage2 != null)
            {
                addFilm.FilmInLanguage2 = filmItem.FilmInLanguage2.Trim();
            }

            if (filmItem.FilmSubtitles1 != null)
            {
                addFilm.FilmSubtitles1 = filmItem.FilmSubtitles1.Trim();
            }

            if (filmItem.FilmSubtitles2 != null)
            {
                addFilm.FilmSubtitles2 = filmItem.FilmSubtitles2.Trim();
            }

            #endregion

            #region Съёмочная группа

            addFilm.FilmРroductionCompany = filmItem.FilmРroductionCompany.Trim();

            addFilm.FilmDirector1 = filmItem.FilmDirector1.Trim();

            if (filmItem.FilmDirector2 != null)
            {
                addFilm.FilmDirector2 = filmItem.FilmDirector2.Trim();
            }

            if (filmItem.FilmMusicBy != null)
            {
                addFilm.FilmMusicBy = filmItem.FilmMusicBy.Trim();
            }

            if (filmItem.FilmActor01 != null)
            {
                addFilm.FilmActor01 = filmItem.FilmActor01.Trim();
            }

            if (filmItem.FilmActor02 != null)
            {
                addFilm.FilmActor02 = filmItem.FilmActor02.Trim();
            }

            if (filmItem.FilmActor03 != null)
            {
                addFilm.FilmActor03 = filmItem.FilmActor03.Trim();
            }

            if (filmItem.FilmActor04 != null)
            {
                addFilm.FilmActor04 = filmItem.FilmActor04.Trim();
            }

            if (filmItem.FilmActor05 != null)
            {
                addFilm.FilmActor05 = filmItem.FilmActor05.Trim();
            }

            if (filmItem.FilmActor06 != null)
            {
                addFilm.FilmActor06 = filmItem.FilmActor06.Trim();
            }

            if (filmItem.FilmActor07 != null)
            {
                addFilm.FilmActor07 = filmItem.FilmActor07.Trim();
            }

            if (filmItem.FilmActor08 != null)
            {
                addFilm.FilmActor08 = filmItem.FilmActor08.Trim();
            }

            if (filmItem.FilmActor09 != null)
            {
                addFilm.FilmActor09 = filmItem.FilmActor09.Trim();
            }

            if (filmItem.FilmActor10 != null)
            {
                addFilm.FilmActor10 = filmItem.FilmActor10.Trim();
            }

            #endregion

            #region Ссылки на видеохостинги

            if (!string.IsNullOrEmpty(addFilm.FilmFileName))
            {
                addFilm.FilmContentUrl = new Uri("https://sergeyshef.ru/video/" + addFilm.FilmFileName);
            }

            if (filmItem.FilmYouTube != null)
            {
                addFilm.FilmYouTube = filmItem.FilmYouTube;
            }

            if (filmItem.FilmVkVideo != null)
            {
                addFilm.FilmVkVideo = filmItem.FilmVkVideo;
            }

            if (filmItem.FilmMailRuVideo != null)
            {
                addFilm.FilmMailRuVideo = filmItem.FilmMailRuVideo;
            }

            if (filmItem.FilmOkVideo != null)
            {
                addFilm.FilmOkVideo = filmItem.FilmOkVideo;
            }

            if (filmItem.FilmYandexDiskVideo != null)
            {
                addFilm.FilmYandexDiskVideo = filmItem.FilmYandexDiskVideo;
            }

            #endregion

            #region Ссылки на информацию о фильме

            if (filmItem.FilmKinoTeatrRu != null)
            {
                addFilm.FilmKinoTeatrRu = filmItem.FilmKinoTeatrRu;
            }

            if (filmItem.FilmKinoPoisk != null)
            {
                addFilm.FilmKinoPoisk = filmItem.FilmKinoPoisk;
            }

            if (filmItem.FilmImbd != null)
            {
                addFilm.FilmImbd = filmItem.FilmMailRuVideo;
            }

            #endregion

            #region Многосерийный фильм

            if (filmItem.SeriesSearchFilter != null)
            {
                addFilm.SeriesSearchFilter = filmItem.SeriesSearchFilter.Trim();
            }

            addFilm.FilmTotalParts = filmItem.SeriesSearchFilter != null && filmItem.FilmTotalParts != null ? filmItem.FilmTotalParts : null;

            addFilm.FilmPart = filmItem.SeriesSearchFilter != null && filmItem.FilmTotalParts != null && filmItem.FilmPart != null ? filmItem.FilmPart : null;

            #endregion

            #region Постер и картинка фильма

            if (filmItem.PosterForFilmFormFile != null)
            {
                var posterGuid = await imageContext.GetImageGuidByFileNameAsync(filmItem.PosterForFilmFormFile.FileName);

                if (posterGuid != Guid.Empty)
                {
                    addFilm.FilmPosterId = posterGuid;
                }
                else
                {
                    ModelState.AddModelError("PosterForFilmFormFile", $"Вы выбрали файл «{filmItem.PosterForFilmFormFile.FileName}»" + Environment.NewLine + "Файла с таким именем нет в базе данных");

                    return View(filmItem);

                }
            }
            else
            {
                ModelState.AddModelError("PosterForFilmFormFile", "Выберите файл постера");
            }

            if (filmItem.ImageForFilmFormFile != null)
            {
                var imageGuid = await imageContext.GetImageGuidByFileNameAsync(filmItem.ImageForFilmFormFile.FileName);

                if (imageGuid != Guid.Empty)
                {
                    addFilm.FilmImageId = imageGuid;
                }
                else
                {
                    ModelState.AddModelError("ImageForFilmFormFile", $"Вы выбрали файл «{filmItem.ImageForFilmFormFile.FileName}»" + Environment.NewLine + "Файла с таким именем нет в базе данных");

                    return View(filmItem);
                }
            }
            else
            {
                ModelState.AddModelError("ImageForFilmFormFile", "Выберите файл картинки");
            }

            #endregion

            #region Сохранить данные

            await filmContext.AddNewFilmAsync(addFilm);

            var newFilm = await filmContext.FilmFiles.FirstAsync(film => film.FilmCaption == addFilm.FilmCaption);

            return RedirectToAction("DetailsFilm", new { filmId = newFilm.FilmFileModelId, Area = "Admin" });

            #endregion
        }
        else
        {
            return View(filmItem);
        }
    }

    #endregion

    #region Изменить информацию о фильме

    [HttpGet]
    public async Task<IActionResult> EditFilm(Guid? filmId)
    {
        if (filmId.HasValue && await filmContext.FilmFiles.Where(film => film.FilmFileModelId == filmId).AnyAsync())
        {
            FilmFileModel editFilm = await filmContext.FilmFiles
                .Include(film => film.FilmImage)
                .Include(film => film.FilmPoster)
                .Include(film => film.PageInfoModel)
                .Include(film => film.FullFilm)
                .AsNoTracking()
                .FirstAsync(film => film.FilmFileModelId == filmId);

            return View(editFilm);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion
}