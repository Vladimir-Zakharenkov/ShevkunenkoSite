//Ignore Spelling: Org
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace ShevkunenkoSite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public partial class FilmsController(
    IFilmFileRepository filmContext,
    IImageFileRepository imageContext,
    IWebHostEnvironment hostEnvironment
    ) : Controller
{
    private readonly string rootPath = hostEnvironment.WebRootPath;

    #region Regex

    /// <summary>
    /// Один или более пробельных символов — для нормализации FilmCaption
    /// </summary>
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    #endregion

    #region FillAllFilmsAsync

    /// <summary>
    /// Заполняет AllFilms для FilmFileAddDTOModel (AddFilm).
    /// Список — все фильмы.
    /// </summary>
    private async Task FillAllFilmsAsync(FilmFileAddDTOModel filmItem)
    {
        filmItem.AllFilms = await filmContext.FilmFiles
            .AsNoTracking()
            .OrderBy(f => f.FilmCaption)
            .Select(f => new SelectListItem
            {
                Value = f.FilmFileModelId.ToString(),
                Text = f.FilmCaption
            })
            .ToListAsync();
    }

    /// <summary>
    /// Заполняет AllFilms для FilmFileModel (EditFilm).
    /// Список — все фильмы, кроме текущего.
    /// </summary>
    private async Task FillAllFilmsAsync(FilmFileModel editFilm, Guid? excludeFilmId)
    {
        editFilm.AllFilms = await filmContext.FilmFiles
            .AsNoTracking()
            .Where(f => f.FilmFileModelId != excludeFilmId)
            .OrderBy(f => f.FilmCaption)
            .Select(f => new SelectListItem
            {
                Value = f.FilmFileModelId.ToString(),
                Text = f.FilmCaption
            })
            .ToListAsync();
    }

    #endregion

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
        if (!filmId.HasValue)
        {
            return RedirectToAction(nameof(Index));
        }

        #region Инициализация фильма

        FilmFileModel? filmItem = await filmContext.FilmFiles
            .Include(img => img.FilmImage)
            .Include(img => img.FilmPoster)
            .Include(film => film.FullFilm)
            .AsNoTracking()
            .FirstOrDefaultAsync(film => film.FilmFileModelId == filmId);

        if (filmItem is null)
        {
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Кадры слева и справа от фильма

        var listOfPictures = from m in imageContext.ImageFiles
                             .Where(p => p.SearchFilter != null &&
                                         p.SearchFilter.Contains(filmItem.FilmCaption + "#film-album#"))
                             select m;

        if (filmItem.FilmForPictureId != null)
        {
            FilmFileModel? filmForPictureAround = await filmContext.FilmFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(film => film.FilmFileModelId == filmItem.FilmForPictureId);

            if (filmForPictureAround is not null)
            {
                filmItem.FilmForPictureAround = filmForPictureAround;

                if (await imageContext.ImageFiles
                    .Where(img => img.SearchFilter != null &&
                                  img.SearchFilter.Contains(filmForPictureAround.FilmCaption + "#film-album#"))
                    .AnyAsync())
                {
                    listOfPictures = from m in imageContext.ImageFiles
                                     .Where(p => p.SearchFilter != null &&
                                                 p.SearchFilter.Contains(filmForPictureAround.FilmCaption + "#film-album#"))
                                     select m;
                }
            }
        }

        List<ImageFileModel> picturesList = await listOfPictures.ToListAsync();

        if (picturesList.Count > 0)
        {
            List<ImageFileModel> framesAroundFilm = [.. picturesList.Shuffle()];

            if (framesAroundFilm.Count > 1 && framesAroundFilm.Count < DataConfig.NumberOfPicturesAround * 2)
            {
                filmItem.FramesOnTheLeft = [.. framesAroundFilm.Take(framesAroundFilm.Count / 2)];
                filmItem.FramesOnTheRight = [.. framesAroundFilm.Skip(framesAroundFilm.Count / 2)];
            }
            else if (framesAroundFilm.Count >= DataConfig.NumberOfPicturesAround * 2)
            {
                filmItem.FramesOnTheLeft = [.. framesAroundFilm.Take(DataConfig.NumberOfPicturesAround)];
                filmItem.FramesOnTheRight = [.. framesAroundFilm.Skip(DataConfig.NumberOfPicturesAround).Take(DataConfig.NumberOfPicturesAround)];
            }
            else
            {
                filmItem.FramesOnTheLeft = framesAroundFilm;
                filmItem.FramesOnTheRight = framesAroundFilm;
            }
        }
        else
        {
            filmItem.FramesOnTheLeft = [];
            filmItem.FramesOnTheRight = [];
        }

        #endregion

        return View(filmItem);
    }

    #endregion

    #region Добавить фильм в базу данных

    [HttpGet]
    public async Task<IActionResult> AddFilm()
    {
        FilmFileAddDTOModel filmItem = new();

        await FillAllFilmsAsync(filmItem);

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
                "FilmProductionCompany," +
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
                "FilmForPictureId," +
                "PosterForFilmFormFile," +
                "ImageForFilmFormFile," +
                "FilmIsFamilyFriendly"
        )]
        FilmFileAddDTOModel filmItem)
    {
        if (ModelState.IsValid)
        {
            #region Инициализация addFilm

            FilmFileModel addFilm = new();

            #endregion

            #region Проверка выбран ли файл фильма

            if (filmItem.FileForFilmFormFile == null)
            {
                ModelState.AddModelError("FileForFilmFormFile", "Выберите файл фильма");
            }
            else
            {
                #region Проверка расширения выбранного файла

                if (!filmItem.FileForFilmFormFile.FileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("FileForFilmFormFile", $"Вы выбрали файл {filmItem.FileForFilmFormFile.FileName}" + Environment.NewLine + "Формат фильмов на сайте должен быть «mp4»");

                    await FillAllFilmsAsync(filmItem);

                    return View(filmItem);
                }

                #endregion

                #region Поиск имени файла в базе данных

                if (await filmContext.FilmFiles.Where(film => film.FilmFileName == filmItem.FileForFilmFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("FileForFilmFormFile", $"Вы выбрали файл «{filmItem.FileForFilmFormFile.FileName}»" + Environment.NewLine + "Файл с таким именем уже есть в базе данных");

                    await FillAllFilmsAsync(filmItem);

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
                                ModelState.AddModelError("", "Продолжительность фильма равна 0");

                                await FillAllFilmsAsync(filmItem);

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
                                ModelState.AddModelError("", "Название файла не определено");

                                await FillAllFilmsAsync(filmItem);

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
                                ModelState.AddModelError("", "Расширение файла не определено");

                                await FillAllFilmsAsync(filmItem);

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
                                ModelState.AddModelError("", "MIME/TYPE файла не определен");

                                await FillAllFilmsAsync(filmItem);

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
                                ModelState.AddModelError("", "Размер файла равен 0");

                                await FillAllFilmsAsync(filmItem);

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
                    ModelState.AddModelError("", "Ширина кадра равна 0");

                    await FillAllFilmsAsync(filmItem);

                    return View(filmItem);
                }

                if (addFilm.FilmHeight < 1)
                {
                    ModelState.AddModelError("", "Высота кадра равна 0");

                    await FillAllFilmsAsync(filmItem);

                    return View(filmItem);
                }

                #endregion

                #endregion
            }

            #endregion

            #region Ссылка на полную версию фильма

            if (filmItem.FileForFilmFormFile != null)
            {
                if (filmItem.FullFilmFormFile != null)
                {
                    if (filmItem.FileForFilmFormFile.FileName == filmItem.FullFilmFormFile.FileName)
                    {
                        ModelState.AddModelError("FullFilmFormFile", $"Выбран файл «{filmItem.FileForFilmFormFile.FileName}» для полной и неполной версии.");

                        await FillAllFilmsAsync(filmItem);

                        return View(filmItem);
                    }

                    var fullFilm = await filmContext.FilmFiles
                        .FirstOrDefaultAsync(film => film.FilmFileName == filmItem.FullFilmFormFile.FileName);

                    if (fullFilm != null)
                    {
                        addFilm.FullFilmId = fullFilm.FilmFileModelId;
                    }
                    else
                    {
                        ModelState.AddModelError("FullFilmFormFile", $"Файл «{filmItem.FullFilmFormFile.FileName}» не найден в базе данных.");

                        await FillAllFilmsAsync(filmItem);

                        return View(filmItem);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("FileForFilmFormFile", "Выберите файл фильма.");

                await FillAllFilmsAsync(filmItem);

                return View(filmItem);
            }

            #endregion

            #region Название фильма в базе данных

            filmItem.FilmCaption = WhitespaceRegex().Replace(
                (filmItem.FilmCaption ?? string.Empty).Trim(),
                "--");

            if (string.IsNullOrEmpty(filmItem.FilmCaption))
            {
                ModelState.AddModelError(nameof(filmItem.FilmCaption), "Введите название фильма");

                await FillAllFilmsAsync(filmItem);

                return View(filmItem);
            }

            if (await filmContext.FilmFiles.Where(film => film.FilmCaption == filmItem.FilmCaption).AnyAsync())
            {
                ModelState.AddModelError(nameof(filmItem.FilmCaption), $"Фильм «{filmItem.FilmCaption}» уже существует.");

                await FillAllFilmsAsync(filmItem);

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
                    ModelState.AddModelError(nameof(filmItem.FilmCaptionOriginal), $"Фильм «{filmItem.FilmCaptionOriginal}» уже существует.");

                    await FillAllFilmsAsync(filmItem);

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

            addFilm.FilmIsFamilyFriendly = filmItem.FilmIsFamilyFriendly;

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

            addFilm.FilmProductionCompany = filmItem.FilmProductionCompany.Trim();

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
                addFilm.FilmImbd = filmItem.FilmImbd;
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

            #region Фильм для альбома кадров

            if (filmItem.FilmForPictureId != null &&
                !await filmContext.FilmFiles.AnyAsync(f => f.FilmFileModelId == filmItem.FilmForPictureId))
            {
                ModelState.AddModelError(nameof(filmItem.FilmForPictureId), "Выбранный фильм не найден в базе данных.");

                await FillAllFilmsAsync(filmItem);

                return View(filmItem);
            }

            addFilm.FilmForPictureId = filmItem.FilmForPictureId;

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

                    await FillAllFilmsAsync(filmItem);

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

                    await FillAllFilmsAsync(filmItem);

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
            await FillAllFilmsAsync(filmItem);

            return View(filmItem);
        }
    }

    #endregion

    #region Изменить информацию о фильме

    [HttpGet]
    public async Task<IActionResult> EditFilm(Guid? filmId)
    {
        if (!filmId.HasValue)
        {
            return RedirectToAction(nameof(Index));
        }

        #region Инициализация editFilm

        FilmFileModel? editFilm = await filmContext.FilmFiles
            .Include(film => film.FilmImage)
            .Include(film => film.FilmPoster)
            .Include(film => film.FullFilm)
            .AsNoTracking()
            .FirstOrDefaultAsync(film => film.FilmFileModelId == filmId);

        if (editFilm is null)
        {
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Список фильмов сайта

        await FillAllFilmsAsync(editFilm, filmId);

        #endregion

        #region Кадры слева и справа от фильма

        var listOfPictures = from m in imageContext.ImageFiles
                             .Where(p => p.SearchFilter != null &&
                                         p.SearchFilter.Contains(editFilm.FilmCaption + "#film-album#"))
                             select m;

        if (editFilm.FilmForPictureId != null)
        {
            FilmFileModel? filmForPictureAround = await filmContext.FilmFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(film => film.FilmFileModelId == editFilm.FilmForPictureId);

            if (filmForPictureAround is not null)
            {
                editFilm.FilmForPictureAround = filmForPictureAround;

                if (await imageContext.ImageFiles
                    .Where(img => img.SearchFilter != null &&
                                  img.SearchFilter.Contains(filmForPictureAround.FilmCaption + "#film-album#"))
                    .AnyAsync())
                {
                    listOfPictures = from m in imageContext.ImageFiles
                                     .Where(p => p.SearchFilter != null &&
                                                 p.SearchFilter.Contains(filmForPictureAround.FilmCaption + "#film-album#"))
                                     select m;
                }
            }
        }

        List<ImageFileModel> picturesList = await listOfPictures.ToListAsync();

        if (picturesList.Count > 0)
        {
            List<ImageFileModel> framesAroundFilm = [.. picturesList.Shuffle()];

            if (framesAroundFilm.Count > 1 && framesAroundFilm.Count < DataConfig.NumberOfPicturesAround * 2)
            {
                editFilm.FramesOnTheLeft = [.. framesAroundFilm.Take(framesAroundFilm.Count / 2)];
                editFilm.FramesOnTheRight = [.. framesAroundFilm.Skip(framesAroundFilm.Count / 2)];
            }
            else if (framesAroundFilm.Count >= DataConfig.NumberOfPicturesAround * 2)
            {
                editFilm.FramesOnTheLeft = [.. framesAroundFilm.Take(DataConfig.NumberOfPicturesAround)];
                editFilm.FramesOnTheRight = [.. framesAroundFilm.Skip(DataConfig.NumberOfPicturesAround).Take(DataConfig.NumberOfPicturesAround)];
            }
            else
            {
                editFilm.FramesOnTheLeft = framesAroundFilm;
                editFilm.FramesOnTheRight = framesAroundFilm;
            }
        }
        else
        {
            editFilm.FramesOnTheLeft = [];
            editFilm.FramesOnTheRight = [];
        }

        #endregion

        return View(editFilm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(5_268_435_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
    public async Task<IActionResult?> EditFilm(
            [Bind(
                "FileForFilmFormFile," +
                "FilmFileModelId," +
                "FullFilmFormFile," +
                "FullFilmId," +
                "FullFilm," +
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
                "FilmProductionCompany," +
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
                "FilmPosterId," +
                "FilmPoster," +
                "FilmImageId," +
                "FilmForPictureId," +
                "FilmImage," +
                "FilmIsFamilyFriendly"
        )]
        FilmFileModel editFilm)
    {
        if (ModelState.IsValid)
        {
            #region Инициализация filmUpdate

            FilmFileModel filmUpdate = await filmContext.FilmFiles.FirstAsync(film => film.FilmFileModelId == editFilm.FilmFileModelId);

            #endregion

            #region Если выбран новый файл фильма

            if (editFilm.FileForFilmFormFile != null)
            {
                #region Проверка расширения выбранного файла

                if (!editFilm.FileForFilmFormFile.FileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("FileForFilmFormFile", $"Вы выбрали файл «{editFilm.FileForFilmFormFile.FileName}»" + Environment.NewLine + "Формат фильмов на сайте должен быть «.mp4»");

                    await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                    return View(editFilm);
                }

                #endregion

                #region Поиск имени файла в базе данных

                if (await filmContext.FilmFiles.Where(film => film.FilmFileName == editFilm.FileForFilmFormFile.FileName).AnyAsync())
                {
                    if (filmUpdate.FilmFileName == editFilm.FileForFilmFormFile.FileName)
                    {
                        ModelState.AddModelError("FileForFilmFormFile", $"Вы выбрали файл «{editFilm.FileForFilmFormFile.FileName}»." + Environment.NewLine + "Это файл редактируемого фильма.");

                        await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                        return View(editFilm);
                    }
                    else
                    {
                        var filmInDB = await filmContext.FilmFiles.FirstAsync(film => film.FilmFileName == editFilm.FileForFilmFormFile.FileName);

                        ModelState.AddModelError("FileForFilmFormFile", $"Вы выбрали файл «{editFilm.FileForFilmFormFile.FileName}»." + Environment.NewLine + $"Это файл фильма «{filmInDB.FilmCaption}».");

                        await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                        return View(editFilm);
                    }
                }

                #endregion

                #region Изменить файл фильма

                else
                {
                    #region Копируем выбранный файл в папку DataConfig.MovieFoldersPath

                    string path = Path.Combine(DataConfig.MovieFoldersPath, editFilm.FileForFilmFormFile.FileName);

                    if (!System.IO.File.Exists(path))
                    {
                        using var stream = new FileStream(path, FileMode.Create);

                        await editFilm.FileForFilmFormFile.CopyToAsync(stream);
                    }

                    #endregion

                    #region Перемещаем старый файл в папку DataConfig.ArchiveFilmsFolderPath

                    string oldFilePath = Path.Combine(DataConfig.MovieFoldersPath, filmUpdate.FilmFileName);
                    string newFilePath = rootPath + Path.Combine(DataConfig.ArchiveFilmsFolderPath, Path.GetFileName(oldFilePath));

                    using (FileStream sourceStream = System.IO.File.OpenRead(oldFilePath))
                    using (FileStream destStream = System.IO.File.Create(newFilePath))
                    {
                        await sourceStream.CopyToAsync(destStream);
                    }

                    System.IO.File.Delete(oldFilePath);

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
                                    ModelState.AddModelError("", "Продолжительность фильма равна 0");

                                    await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                                    return View(editFilm);
                                }
                                else
                                {
                                    filmUpdate.FilmDuration = TimeSpan.Parse(tag.Description);
                                }
                            }

                            #endregion

                            #region Ширина кадра FilmWidth

                            if (movieDirectory.Name == "QuickTime Track Header" && tag.Name == "Width" && Convert.ToInt32(tag.Description) > 0)
                            {
                                filmUpdate.FilmWidth = Convert.ToInt32(tag.Description);
                            }

                            #endregion

                            #region Высота кадра FilmHeight

                            if (movieDirectory.Name == "QuickTime Track Header" && tag.Name == "Height" && Convert.ToInt32(tag.Description) > 0)
                            {
                                filmUpdate.FilmHeight = Convert.ToInt32(tag.Description);
                            }

                            #endregion

                            #region Имя файла

                            if (movieDirectory.Name == "File" && tag.Name == "File Name")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("", "Название файла не определено");

                                    await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                                    return View(editFilm);
                                }
                                else
                                {
                                    filmUpdate.FilmFileName = tag.Description;
                                }
                            }

                            #endregion

                            #region Расширение файла

                            if (movieDirectory.Name == "File Type" && tag.Name == "Expected File Name Extension")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("", "Расширение файла не определено");

                                    await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                                    return View(editFilm);
                                }
                                else
                                {
                                    filmUpdate.FilmFileExtension = tag.Description;
                                }
                            }

                            #endregion

                            #region Определение MIME Type

                            if (movieDirectory.Name == "File Type" && tag.Name == "Detected MIME Type")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("", "MIME/TYPE файла не определен");

                                    await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                                    return View(editFilm);
                                }
                                else
                                {
                                    filmUpdate.FilmMimeType = tag.Description;
                                }
                            }

                            #endregion

                            #region Размер файла

                            if (movieDirectory.Name == "File" && tag.Name == "File Size")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("", "Размер файла равен 0");

                                    await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                                    return View(editFilm);
                                }
                                else
                                {
                                    filmUpdate.FilmFileSize = Convert.ToUInt64(tag.Description[..tag.Description.IndexOf(' ')]);
                                }
                            }

                            #endregion
                        }
                    }

                    #region Проверка ширины и высоты кадра

                    if (filmUpdate.FilmWidth < 1)
                    {
                        ModelState.AddModelError("", "Ширина кадра равна 0");

                        await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                        return View(editFilm);
                    }

                    if (filmUpdate.FilmHeight < 1)
                    {
                        ModelState.AddModelError("", "Высота кадра равна 0");

                        await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                        return View(editFilm);
                    }

                    #endregion

                    #endregion
                }

                #endregion
            }

            #endregion

            #region Ссылка на полную версию фильма

            if (editFilm.FullFilmFormFile != null)
            {
                if (filmUpdate.FilmFileName == editFilm.FullFilmFormFile.FileName)
                {
                    ModelState.AddModelError("FullFilmFormFile", $"Выбран файл «{editFilm.FullFilmFormFile.FileName}» для полной и неполной версии.");

                    await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                    return View(editFilm);
                }

                var fullFilm = await filmContext.FilmFiles
                    .FirstOrDefaultAsync(film => film.FilmFileName == editFilm.FullFilmFormFile.FileName);

                if (fullFilm != null)
                {
                    filmUpdate.FullFilmId = fullFilm.FilmFileModelId;
                }
                else
                {
                    ModelState.AddModelError("FullFilmFormFile", $"Файл «{editFilm.FullFilmFormFile.FileName}» не найден в базе данных.");

                    await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                    return View(editFilm);
                }
            }

            #endregion

            #region Название фильма в базе данных

            string normalizedCaption = WhitespaceRegex().Replace(
                (editFilm.FilmCaption ?? string.Empty).Trim(),
                "--");

            if (string.IsNullOrEmpty(normalizedCaption))
            {
                ModelState.AddModelError(nameof(editFilm.FilmCaption), "Введите название фильма");

                await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                return View(editFilm);
            }

            if (filmUpdate.FilmCaption != normalizedCaption &&
                await filmContext.FilmFiles.Where(film => film.FilmCaption == normalizedCaption).AnyAsync())
            {
                ModelState.AddModelError(nameof(editFilm.FilmCaption), $"Фильм с названием «{normalizedCaption}» уже существует.");

                await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                return View(editFilm);
            }
            else
            {
                filmUpdate.FilmCaption = normalizedCaption;
            }

            #endregion

            #region Оригинальное название фильма

            if (filmUpdate.FilmCaptionOriginal != editFilm.FilmCaptionOriginal &&
                await filmContext.FilmFiles.Where(film => film.FilmCaptionOriginal == editFilm.FilmCaptionOriginal).AnyAsync())
            {
                ModelState.AddModelError(nameof(editFilm.FilmCaptionOriginal), $"Фильм с оригинальным названием «{editFilm.FilmCaptionOriginal}» уже существует.");

                await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                return View(editFilm);
            }
            else
            {
                filmUpdate.FilmCaptionOriginal = editFilm.FilmCaptionOriginal.Trim();
            }

            #endregion

            #region Краткое содержание, примечания админа

            filmUpdate.FilmDescriptionForSchemaOrg = editFilm.FilmDescriptionForSchemaOrg.Trim();

            filmUpdate.FilmDescriptionHtml = editFilm.FilmDescriptionHtml.Trim();

            if (!string.IsNullOrEmpty(editFilm.FilmNote))
            {
                filmUpdate.FilmNote = editFilm.FilmNote.Trim();
            }

            #endregion

            #region Фильм в основном списке. Критерии поиска. Фильм 18+

            filmUpdate.FilmInMainList = editFilm.FilmInMainList;

            if (editFilm.SearchFilterForFilm != null)
            {
                filmUpdate.SearchFilterForFilm = editFilm.SearchFilterForFilm.Trim();
            }

            filmUpdate.FilmGenre = editFilm.FilmGenre.Trim();

            filmUpdate.FilmAdult = editFilm.FilmAdult;

            filmUpdate.FilmIsFamilyFriendly = editFilm.FilmIsFamilyFriendly;

            #endregion

            #region Язык и субтитры

            filmUpdate.FilmInLanguage1 = editFilm.FilmInLanguage1.Trim();

            if (editFilm.FilmInLanguage2 != null)
            {
                filmUpdate.FilmInLanguage2 = editFilm.FilmInLanguage2.Trim();
            }

            if (editFilm.FilmSubtitles1 != null)
            {
                filmUpdate.FilmSubtitles1 = editFilm.FilmSubtitles1.Trim();
            }

            if (editFilm.FilmSubtitles2 != null)
            {
                filmUpdate.FilmSubtitles2 = editFilm.FilmSubtitles2.Trim();
            }

            #endregion

            #region Съёмочная группа

            filmUpdate.FilmProductionCompany = editFilm.FilmProductionCompany.Trim();

            filmUpdate.FilmDirector1 = editFilm.FilmDirector1.Trim();

            if (editFilm.FilmDirector2 != null)
            {
                filmUpdate.FilmDirector2 = editFilm.FilmDirector2.Trim();
            }

            if (editFilm.FilmMusicBy != null)
            {
                filmUpdate.FilmMusicBy = editFilm.FilmMusicBy.Trim();
            }

            if (editFilm.FilmActor01 != null)
            {
                filmUpdate.FilmActor01 = editFilm.FilmActor01.Trim();
            }

            if (editFilm.FilmActor02 != null)
            {
                filmUpdate.FilmActor02 = editFilm.FilmActor02.Trim();
            }

            if (editFilm.FilmActor03 != null)
            {
                filmUpdate.FilmActor03 = editFilm.FilmActor03.Trim();
            }

            if (editFilm.FilmActor04 != null)
            {
                filmUpdate.FilmActor04 = editFilm.FilmActor04.Trim();
            }

            if (editFilm.FilmActor05 != null)
            {
                filmUpdate.FilmActor05 = editFilm.FilmActor05.Trim();
            }

            if (editFilm.FilmActor06 != null)
            {
                filmUpdate.FilmActor06 = editFilm.FilmActor06.Trim();
            }

            if (editFilm.FilmActor07 != null)
            {
                filmUpdate.FilmActor07 = editFilm.FilmActor07.Trim();
            }

            if (editFilm.FilmActor08 != null)
            {
                filmUpdate.FilmActor08 = editFilm.FilmActor08.Trim();
            }

            if (editFilm.FilmActor09 != null)
            {
                filmUpdate.FilmActor09 = editFilm.FilmActor09.Trim();
            }

            if (editFilm.FilmActor10 != null)
            {
                filmUpdate.FilmActor10 = editFilm.FilmActor10.Trim();
            }

            #endregion

            #region Ссылки на видеохостинги

            if (!string.IsNullOrEmpty(filmUpdate.FilmFileName))
            {
                filmUpdate.FilmContentUrl = new Uri("https://sergeyshef.ru/video/" + filmUpdate.FilmFileName);
            }

            if (editFilm.FilmYouTube != null)
            {
                filmUpdate.FilmYouTube = editFilm.FilmYouTube;
            }

            if (editFilm.FilmVkVideo != null)
            {
                filmUpdate.FilmVkVideo = editFilm.FilmVkVideo;
            }

            if (editFilm.FilmMailRuVideo != null)
            {
                filmUpdate.FilmMailRuVideo = editFilm.FilmMailRuVideo;
            }

            if (editFilm.FilmOkVideo != null)
            {
                filmUpdate.FilmOkVideo = editFilm.FilmOkVideo;
            }

            if (editFilm.FilmYandexDiskVideo != null)
            {
                filmUpdate.FilmYandexDiskVideo = editFilm.FilmYandexDiskVideo;
            }

            #endregion

            #region Ссылки на информацию о фильме

            if (editFilm.FilmKinoTeatrRu != null)
            {
                filmUpdate.FilmKinoTeatrRu = editFilm.FilmKinoTeatrRu;
            }

            if (editFilm.FilmKinoPoisk != null)
            {
                filmUpdate.FilmKinoPoisk = editFilm.FilmKinoPoisk;
            }

            if (editFilm.FilmImbd != null)
            {
                filmUpdate.FilmImbd = editFilm.FilmImbd;
            }

            #endregion

            #region Многосерийный фильм

            if (editFilm.SeriesSearchFilter != null)
            {
                filmUpdate.SeriesSearchFilter = editFilm.SeriesSearchFilter.Trim();
            }

            filmUpdate.FilmTotalParts = editFilm.SeriesSearchFilter != null && editFilm.FilmTotalParts != null ? editFilm.FilmTotalParts : null;

            filmUpdate.FilmPart = editFilm.SeriesSearchFilter != null && editFilm.FilmTotalParts != null && editFilm.FilmPart != null ? editFilm.FilmPart : null;

            #endregion

            #region Фильм для альбома кадров

            if (editFilm.FilmForPictureId != null &&
                !await filmContext.FilmFiles.AnyAsync(f => f.FilmFileModelId == editFilm.FilmForPictureId))
            {
                ModelState.AddModelError(nameof(editFilm.FilmForPictureId), "Выбранный фильм не найден в базе данных.");

                await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                return View(editFilm);
            }

            if (editFilm.FilmForPictureId == editFilm.FilmFileModelId)
            {
                ModelState.AddModelError(nameof(editFilm.FilmForPictureId), "Нельзя выбрать текущий фильм как фильм для кадров.");

                await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                return View(editFilm);
            }

            filmUpdate.FilmForPictureId = editFilm.FilmForPictureId;

            #endregion

            #region Постер и картинка фильма

            if (editFilm.PosterForFilmFormFile != null)
            {
                var posterGuid = await imageContext.GetImageGuidByFileNameAsync(editFilm.PosterForFilmFormFile.FileName);

                if (posterGuid != Guid.Empty)
                {
                    filmUpdate.FilmPosterId = posterGuid;
                }
                else
                {
                    ModelState.AddModelError("PosterForFilmFormFile", $"Вы выбрали файл «{editFilm.PosterForFilmFormFile.FileName}»" + Environment.NewLine + "Файла с таким именем нет в базе данных");

                    await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                    return View(editFilm);
                }
            }
            else if (editFilm.FilmPosterId != Guid.Empty
                && await imageContext.ImageFiles.AnyAsync(i => i.ImageFileModelId == editFilm.FilmPosterId))
            {
                filmUpdate.FilmPosterId = editFilm.FilmPosterId;
            }

            if (editFilm.ImageForFilmFormFile != null)
            {
                var imageGuid = await imageContext.GetImageGuidByFileNameAsync(editFilm.ImageForFilmFormFile.FileName);

                if (imageGuid != Guid.Empty)
                {
                    filmUpdate.FilmImageId = imageGuid;
                }
                else
                {
                    ModelState.AddModelError("ImageForFilmFormFile", $"Вы выбрали файл «{editFilm.ImageForFilmFormFile.FileName}»" + Environment.NewLine + "Файла с таким именем нет в базе данных");

                    await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

                    return View(editFilm);
                }
            }
            else if (editFilm.FilmImageId != Guid.Empty
                && await imageContext.ImageFiles.AnyAsync(i => i.ImageFileModelId == editFilm.FilmImageId))
            {
                filmUpdate.FilmImageId = editFilm.FilmImageId;
            }

            #endregion

            #region Сохранить данные

            await filmContext.SaveChangesInFilmAsync();

            var newFilm = await filmContext.FilmFiles.FirstAsync(film => film.FilmCaption == filmUpdate.FilmCaption);

            return RedirectToAction("DetailsFilm", new { filmId = newFilm.FilmFileModelId, Area = "Admin" });

            #endregion
        }
        else
        {
            await FillAllFilmsAsync(editFilm, editFilm.FilmFileModelId);

            return View(editFilm);
        }
    }

    #endregion
}