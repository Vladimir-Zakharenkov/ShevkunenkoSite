//Ignore Spelling: Org
using MetadataExtractor;

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

    readonly FilmFileModel filmItem = new();

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
                "FilmImbd"
        )]
        FilmFileModel filmItem)
    {
        if (!ModelState.IsValid)
        {
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
                            filmItem.FilmDuration = TimeSpan.Parse(tag.Description);
                        }
                    }

                    #endregion

                    #region Ширина кадра FilmWidth

                    if (movieDirectory.Name == "QuickTime Track Header" && tag.Name == "Width" && Convert.ToInt32(tag.Description) > 0)
                    {
                        filmItem.FilmWidth = Convert.ToInt32(tag.Description);
                    }

                    #endregion

                    #region Высота кадра FilmHeight

                    if (movieDirectory.Name == "QuickTime Track Header" && tag.Name == "Height" && Convert.ToInt32(tag.Description) > 0)
                    {
                        filmItem.FilmHeight = Convert.ToInt32(tag.Description);
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
                            filmItem.FilmFileName = tag.Description;
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
                            filmItem.FilmFileExtension = tag.Description;
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
                            filmItem.FilmMimeType = tag.Description;
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
                            filmItem.FilmFileSize = Convert.ToUInt64(tag.Description[..tag.Description.IndexOf(' ')]);
                        }
                    }

                    #endregion
                }
            }

            #region Проверка ширины и высоты кадра

            if (filmItem.FilmWidth < 1)
            {
                ModelState.AddModelError("filmItem.FilmWidth", "Ширина кадра равна 0");

                return View(filmItem);
            }

            if (filmItem.FilmHeight < 1)
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

                        filmItem.FullFilmId = fullFilm.FilmFileModelId;
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
                filmItem.FilmCaption = filmItem.FilmCaption.Trim();
            }

            #endregion

            #region Оригинальное название фильма

            if (filmItem.FilmCaptionOriginal != null && string.IsNullOrWhiteSpace(filmItem.FilmCaptionOriginal))
            {
                if (await filmContext.FilmFiles.Where(film => film.FilmCaptionOriginal == filmItem.FilmCaptionOriginal).AnyAsync())
                {
                    ModelState.AddModelError("filmItem.FilmCaptionOriginal", $"Фильм «{filmItem.FilmCaptionOriginal}» уже существует.");

                    return View(filmItem);
                }
                else
                {
                    filmItem.FilmCaptionOriginal = filmItem.FilmCaptionOriginal.Trim();
                }
            }

            #endregion

            #region Краткое содержание, примечания админа

            filmItem.FilmDescriptionForSchemaOrg = filmItem.FilmDescriptionForSchemaOrg.Trim();

            filmItem.FilmDescriptionHtml = filmItem.FilmDescriptionHtml.Trim();

            if (!string.IsNullOrEmpty(filmItem.FilmNote))
            {
                filmItem.FilmNote = filmItem.FilmNote.Trim();
            }

            #endregion

            #region Критерии поиска

            filmItem.FilmInMainList = filmItem.FilmInMainList;

            if (filmItem.SearchFilterForFilm != null)
            {
                filmItem.SearchFilterForFilm = filmItem.SearchFilterForFilm.Trim();
            }

            filmItem.FilmGenre = filmItem.FilmGenre.Trim();

            filmItem.FilmAdult = filmItem.FilmAdult;

            #endregion

            #region Язык и субтитры

            filmItem.FilmInLanguage1 = filmItem.FilmInLanguage1.Trim();

            if (filmItem.FilmInLanguage2 != null)
            {
                filmItem.FilmInLanguage2 = filmItem.FilmInLanguage2.Trim();
            }

            if (filmItem.FilmSubtitles1 != null)
            {
                filmItem.FilmSubtitles1 = filmItem.FilmSubtitles1.Trim();
            }

            if (filmItem.FilmSubtitles2 != null)
            {
                filmItem.FilmSubtitles2 = filmItem.FilmSubtitles2.Trim();
            }

            #endregion

            #region Съёмочная группа

            filmItem.FilmРroductionCompany = filmItem.FilmРroductionCompany.Trim();

            filmItem.FilmDirector1 = filmItem.FilmDirector1.Trim();

            if (filmItem.FilmDirector2 != null)
            {
                filmItem.FilmDirector2 = filmItem.FilmDirector2.Trim();
            }

            if (filmItem.FilmMusicBy != null)
            {
                filmItem.FilmMusicBy = filmItem.FilmMusicBy.Trim();
            }

            if (filmItem.FilmActor01 != null)
            {
                filmItem.FilmActor01 = filmItem.FilmActor01.Trim();
            }

            if (filmItem.FilmActor02 != null)
            {
                filmItem.FilmActor02 = filmItem.FilmActor02.Trim();
            }

            if (filmItem.FilmActor03 != null)
            {
                filmItem.FilmActor03 = filmItem.FilmActor03.Trim();
            }

            if (filmItem.FilmActor04 != null)
            {
                filmItem.FilmActor04 = filmItem.FilmActor04.Trim();
            }

            if (filmItem.FilmActor05 != null)
            {
                filmItem.FilmActor05 = filmItem.FilmActor05.Trim();
            }

            if (filmItem.FilmActor06 != null)
            {
                filmItem.FilmActor06 = filmItem.FilmActor06.Trim();
            }

            if (filmItem.FilmActor07 != null)
            {
                filmItem.FilmActor07 = filmItem.FilmActor07.Trim();
            }

            if (filmItem.FilmActor08 != null)
            {
                filmItem.FilmActor08 = filmItem.FilmActor08.Trim();
            }

            if (filmItem.FilmActor09 != null)
            {
                filmItem.FilmActor09 = filmItem.FilmActor09.Trim();
            }

            if (filmItem.FilmActor10 != null)
            {
                filmItem.FilmActor10 = filmItem.FilmActor10.Trim();
            }

            #endregion

            #region Ссылки на видеохостинги

            if (filmItem.FilmContentUrl != null)
            {
                filmItem.FilmContentUrl = new Uri("https://sergeyshef.ru/video/" + filmItem.FilmFileName);
            }

            if (filmItem.FilmYouTube != null)
            {
                filmItem.FilmYouTube = filmItem.FilmYouTube;
            }

            if (filmItem.FilmVkVideo != null)
            {
                filmItem.FilmVkVideo = filmItem.FilmVkVideo;
            }

            if (filmItem.FilmMailRuVideo != null)
            {
                filmItem.FilmMailRuVideo = filmItem.FilmMailRuVideo;
            }

            if (filmItem.FilmOkVideo != null)
            {
                filmItem.FilmOkVideo = filmItem.FilmOkVideo;
            }

            if (filmItem.FilmYandexDiskVideo != null)
            {
                filmItem.FilmYandexDiskVideo = filmItem.FilmYandexDiskVideo;
            }

            #endregion

            #region Ссылки на информацию о фильме

            if (filmItem.FilmKinoTeatrRu != null)
            {
                filmItem.FilmKinoTeatrRu = filmItem.FilmKinoTeatrRu;
            }

            if (filmItem.FilmKinoPoisk != null)
            {
                filmItem.FilmKinoPoisk = filmItem.FilmKinoPoisk;
            }

            if (filmItem.FilmImbd != null)
            {
                filmItem.FilmImbd = filmItem.FilmMailRuVideo;
            }

            #endregion

            #region Сохранить данные

            return View(filmItem);

            #endregion

        }
        else
        {
            return View(filmItem);
        }
    }

    #endregion
}