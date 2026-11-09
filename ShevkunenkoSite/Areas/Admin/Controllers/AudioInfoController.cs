using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using NAudio.Wave;

namespace ShevkunenkoSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AudioInfoController
        (
        IAudioInfoRepository audioFileContext,
        IAudioBookRepository audioBookContext,
        IPageInfoRepository pagesOfSiteContext,
        ITextInfoRepository textsOfSiteContext,
        IWebHostEnvironment hostEnvironment
        ) : Controller
    {
        private readonly string rootPath = hostEnvironment.WebRootPath;

        #region Список аудиофайлов
        public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
        {
            var allAudioFiles = await audioFileContext.AudioFiles.ToListAsync();

            if (!searchString.IsNullOrEmpty())
            {
                allAudioFiles = [.. allAudioFiles.AudioFileSearch(searchString).OrderBy(audioBook => audioBook.AudioFileName)];
            }

            return View(new ItemsListViewModel
            {
                AllAudioFiles = [.. allAudioFiles
                     .Skip((pageNumber - 1) * DataConfig.NumberOfItemsPerPage)
                     .Take(DataConfig.NumberOfItemsPerPage)],

                #region Свойства PagingInfoViewModel

                TotalItems = allAudioFiles.Count,

                ItemsPerPage = DataConfig.NumberOfItemsPerPage,

                CurrentPage = pageNumber,

                SearchString = searchString ?? string.Empty

                #endregion
            });
        }

        #endregion

        #region Информация об аудиофайле

        public async Task<IActionResult> DetailsAudioFile(Guid? audioFileId)
        {
            if (audioFileId.HasValue &&
                await audioFileContext.AudioFiles
                    .Where(audioFile => audioFile.AudioInfoModelId == audioFileId)
                    .AnyAsync())
            {
                var audioInfoModel = await audioFileContext.AudioFiles
                    .Include(a => a.AudioBookModel)
                        .ThenInclude(b => b!.BookForAudioBook)  // TODO: убрать ! для BookForAudioBook
                    .AsNoTracking()
                    .FirstAsync(audioFile => audioFile.AudioInfoModelId == audioFileId);

                if (audioInfoModel.TranscriptId != null
                        && await textsOfSiteContext.Texts
                            .Where(text => text.TextInfoModelId == audioInfoModel.TranscriptId)
                            .AnyAsync())
                {
                    var transcript = await textsOfSiteContext.Texts
                        .Include(text => text.BooksAndArticlesModel)
                        .FirstAsync(text => text.TextInfoModelId == audioInfoModel.TranscriptId);

                    audioInfoModel.Transcript = transcript;

                    using StreamReader clearText = new(rootPath + DataConfig.TextsFolderPath + transcript.FolderForText + transcript.TxtFileName);

                    audioInfoModel.ClearText = clearText.ReadToEnd();
                }

                return View(audioInfoModel);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Добавить аудиофайл

        [HttpGet]
        public IActionResult AddAudioFile()
        {
            AudioInfoModel newAudioFIle = new();

            // Список аудиокниг
            ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

            // Список текстовых файлов
            ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

            // Список папок для аудиофайлов
            ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

            return View(newAudioFIle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        [RequestSizeLimit(5_268_435_456)]
        [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
        public async Task<IActionResult> AddAudioFile(
            [Bind("ChooseAudioFile," +
                    "FolderForAudioFile," +
                    "AuthorOfText," +
                    "CaptionOfTextInAudioFile," +
                    "TextInfoModelId," +
                    "AudioFileDescription," +
                    "InternetRefToAudioFile," +
                    "PodsterFmRefToAudioFile," +
                    "YandexDiskRefToAudioFile," +
                    "PlayerPodsterFm," +
                    "AudioFileUploadDate," +
                    "AudioBookModelId," +
                    "SequenceNumber")]
            AudioInfoModel audioFileForAdding)
        {
            if (ModelState.IsValid)
            {
                if (audioFileForAdding.ChooseAudioFile == null)
                {
                    ModelState.AddModelError("audioFileForAdding.AudioFileBitRate", "Битрейт аудиофайла равен 0");

                    // Список аудиокниг
                    ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                    // Список страниц сайта
                    ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                    // Список текстовых файлов
                    ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                    // Список папок для аудиофайлов
                    ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                    return View(audioFileForAdding);
                }

                #region Автор текста

                _ = audioFileForAdding.AuthorOfText.Trim();

                #endregion

                #region Название текста аудиофайла

                _ = audioFileForAdding.CaptionOfTextInAudioFile.Trim();

                #endregion

                #region Папка аудиофайла

                _ = audioFileForAdding.FolderForAudioFile.Trim();

                #endregion

                #region Transcript - GUID файла с текстом

                _ = audioFileForAdding.TranscriptId;

                #endregion

                #region Аудиокнига

                _ = audioFileForAdding.AudioBookModelId;

                #endregion

                #region Номер по порядку - Значение для сортировки

                _ = audioFileForAdding.SequenceNumber;

                audioFileForAdding.SortOfAudioFile = audioFileForAdding.SequenceNumber ?? 0;

                #endregion

                #region Ссылки на файл в интернете

                _ = audioFileForAdding.InternetRefToAudioFile;
                _ = audioFileForAdding.PodsterFmRefToAudioFile;
                _ = audioFileForAdding.YandexDiskRefToAudioFile;

                #endregion

                #region Код плейера

                _ = audioFileForAdding.PlayerPodsterFm;

                #endregion

                #region Автоматическое определение значений

                #region Продолжительность (время воспроизведения)

                //string audioFile = string.Empty;

                //if (audioFileForAdding.ChooseAudioFile != null)
                //{
                //    var tempFile = Path.GetTempFileName();

                //    using FileStream stream = new(tempFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None, Int16.MaxValue, FileOptions.DeleteOnClose);

                //    await audioFileForAdding.ChooseAudioFile.CopyToAsync(stream);

                //    Mp3FileReader mp3File = new(tempFile);

                //    audioFileForAdding.AudioFileDuration = mp3File.TotalTime;
                //}

                string audioFile = Path.Combine(audioFileForAdding.FolderForAudioFile, audioFileForAdding.ChooseAudioFile.FileName);

                Mp3FileReader mp3File = new(audioFile);

                audioFileForAdding.AudioFileDuration = mp3File.TotalTime;

                #endregion

                IReadOnlyList<MetadataExtractor.Directory> audioDirectories = ImageMetadataReader.ReadMetadata(audioFile);

                foreach (var audioDirectory in audioDirectories)
                {
                    foreach (var tag in audioDirectory.Tags)
                    {
                        #region Битрейт аудиофайла

                        if (audioDirectory.Name == "MP3" && tag.Name == "Bitrate")
                        {
                            if (string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("audioFileForAdding.AudioFileBitRate", "Битрейт аудиофайла равен 0");

                                // Список аудиокниг
                                ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                // Список страниц сайта
                                ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                // Список текстовых файлов
                                ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                // Список папок для аудиофайлов
                                ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                return View(audioFileForAdding);
                            }
                            else
                            {
                                audioFileForAdding.AudioFileBitRate = int.Parse(tag.Description);
                            }
                        }

                        #endregion

                        #region Частота аудиофайла

                        if (audioDirectory.Name == "MP3" && tag.Name == "Frequency")
                        {
                            if (string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("audioFileForAdding.AudioFileFrequency", "Частота аудиофайла равен 0");

                                // Список аудиокниг
                                ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                // Список страниц сайта
                                ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                // Список текстовых файлов
                                ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                // Список папок для аудиофайлов
                                ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                return View(audioFileForAdding);
                            }
                            else
                            {
                                audioFileForAdding.AudioFileFrequency = int.Parse(tag.Description);
                            }
                        }

                        #endregion

                        #region Размер аудиофайла

                        if (audioDirectory.Name == "File" && tag.Name == "File Size")
                        {
                            if (string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("audioFileForAdding.AudioFileFrequency", "Размер аудиофайла равен 0");

                                // Список аудиокниг
                                ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                // Список страниц сайта
                                ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                // Список текстовых файлов
                                ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                // Список папок для аудиофайлов
                                ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                return View(audioFileForAdding);
                            }
                            else
                            {
                                audioFileForAdding.AudioFileSize = int.Parse(tag.Description[..tag.Description.IndexOf(' ')]);
                            }
                        }

                        #endregion

                        #region MIME Type аудиофайла

                        if (audioDirectory.Name == "File Type" && tag.Name == "Detected MIME Type")
                        {
                            if (string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("audioFileForAdding.AudioFileMimeType", "MIME Type не определен");

                                // Список аудиокниг
                                ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                // Список страниц сайта
                                ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                // Список текстовых файлов
                                ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                // Список папок для аудиофайлов
                                ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                return View(audioFileForAdding);
                            }
                            else
                            {
                                audioFileForAdding.AudioFileMimeType = tag.Description;
                            }
                        }

                        #endregion

                        #region Расширение аудиофайла

                        if (audioDirectory.Name == "File Type" && tag.Name == "Expected File Name Extension")
                        {
                            if (string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("audioFileForAdding.AudioFileType", "Расширение файла не определено");

                                // Список аудиокниг
                                ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                // Список страниц сайта
                                ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                // Список текстовых файлов
                                ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                // Список папок для аудиофайлов
                                ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                return View(audioFileForAdding);
                            }
                            else
                            {
                                audioFileForAdding.AudioFileType = tag.Description;
                            }
                        }

                        #endregion

                        #region Имя аудиофайла

                        if (audioDirectory.Name == "File" && tag.Name == "File Name")
                        {
                            if (string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("audioFileForAdding.AudioFileName", "Имя файла не определено");

                                // Список аудиокниг
                                ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                // Список страниц сайта
                                ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                // Список текстовых файлов
                                ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                // Список папок для аудиофайлов
                                ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                return View(audioFileForAdding);
                            }
                            else
                            {
                                audioFileForAdding.AudioFileName = tag.Description;
                            }
                        }

                        #endregion
                    }
                }

                #endregion

                #region Добавить в БД

                await audioFileContext.AddAudioFileAsync(audioFileForAdding);

                #endregion

                #region Открытие параметров добавленного аудиофайла

                var newAudioFile = await audioFileContext
                    .AudioFiles
                    .FirstAsync(audioFile => audioFile.AudioFileName == audioFileForAdding.AudioFileName);

                return RedirectToAction(nameof(DetailsAudioFile), new { audioFileId = newAudioFile.AudioInfoModelId });

                #endregion
            }
            else
            {
                // Список аудиокниг
                ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                // Список текстовых файлов
                ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                // Список папок для аудиофайлов
                ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                return View(audioFileForAdding);
            }
        }

        #endregion

        #region Изменить аудиофайл

        [HttpGet]
        public async Task<IActionResult> EditAudioFile(Guid? audioFileId)
        {
            if (audioFileId.HasValue &&
               await audioFileContext.AudioFiles
                   .Where(audioFile => audioFile.AudioInfoModelId == audioFileId)
                   .AnyAsync())
            {
                var audioFileForEditing = await audioFileContext.AudioFiles
                    .FirstAsync(audioFile => audioFile.AudioInfoModelId == audioFileId);

                // Список аудиокниг
                ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                // Список страниц сайта
                ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                // Список текстовых файлов
                ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                // Список папок для аудиофайлов
                ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                return View(audioFileForEditing);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        [RequestSizeLimit(5_268_435_456)]
        [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
        public async Task<IActionResult> EditAudioFile(
            [Bind( "AudioInfoModelId," +
                    "ChooseAudioFile," +
                    "FolderForAudioFile," +
                    "AuthorOfText," +
                    "CaptionOfTextInAudioFile," +
                    "TranscriptId," +
                    "TextInfoModelId," +
                    "AudioFileDescription," +
                    "InternetRefToAudioFile," +
                    "PodsterFmRefToAudioFile," +
                    "YandexDiskRefToAudioFile," +
                    "PlayerPodsterFm," +
                    "AudioFileUploadDate," +
                    "AudioBookModelId," +
                    "SequenceNumber," +
                    "AudioFileDuration," +
                    "AudioFileBitRate," +
                    "AudioFileFrequency," +
                    "AudioFileSize," +
                    "AudioFileMimeType," +
                    "AudioFileType," +
                    "AudioFileName")]
            AudioInfoModel audioFileForEditing)
        {
            if (ModelState.IsValid)
            {
                AudioInfoModel audioFileForUpdate = await audioFileContext.AudioFiles
                     .FirstAsync(audioFile => audioFile.AudioInfoModelId == audioFileForEditing.AudioInfoModelId);

                #region Автор текста

                audioFileForUpdate.AuthorOfText = audioFileForEditing.AuthorOfText.Trim();

                #endregion

                #region Название текста аудиофайла

                audioFileForUpdate.CaptionOfTextInAudioFile = audioFileForEditing.CaptionOfTextInAudioFile.Trim();

                #endregion

                #region Содержание аудиофайла

                audioFileForUpdate.AudioFileDescription = audioFileForEditing.AudioFileDescription.Trim();

                #endregion

                #region Папка аудиофайла

                audioFileForUpdate.FolderForAudioFile = audioFileForEditing.FolderForAudioFile.Trim();

                #endregion

                #region Transcript - GUID файла с текстом

                audioFileForUpdate.TranscriptId = audioFileForEditing.TranscriptId;

                #endregion

                #region Аудиокнига

                audioFileForUpdate.AudioBookModelId = audioFileForEditing.AudioBookModelId;

                #endregion

                #region Номер по порядку - Значение для сортировки

                audioFileForUpdate.SequenceNumber = audioFileForEditing.SequenceNumber;

                audioFileForUpdate.SortOfAudioFile = audioFileForUpdate.SequenceNumber ?? 0;

                #endregion

                #region Ссылки на файл в интернете

                audioFileForUpdate.InternetRefToAudioFile = audioFileForEditing.InternetRefToAudioFile;
                audioFileForUpdate.PodsterFmRefToAudioFile = audioFileForEditing.PodsterFmRefToAudioFile;
                audioFileForUpdate.YandexDiskRefToAudioFile = audioFileForEditing.YandexDiskRefToAudioFile;

                #endregion

                #region Код плейера

                audioFileForUpdate.PlayerPodsterFm = audioFileForEditing.PlayerPodsterFm;

                #endregion

                #region Автоматическое определение значений

                if (audioFileForEditing.ChooseAudioFile != null)
                {
                    #region Продолжительность (время воспроизведения)

                    //string audioFile = string.Empty;

                    //if (audioFileForAdding.ChooseAudioFile != null)
                    //{
                    //    var tempFile = Path.GetTempFileName();

                    //    using FileStream stream = new(tempFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None, Int16.MaxValue, FileOptions.DeleteOnClose);

                    //    await audioFileForEditing.ChooseAudioFile.CopyToAsync(stream);

                    //    Mp3FileReader mp3File = new(tempFile);

                    //    audioFileForEditing.AudioFileDuration = mp3File.TotalTime;
                    //}

                    string audioFile = Path.Combine(audioFileForEditing.FolderForAudioFile, audioFileForEditing.ChooseAudioFile.FileName);

                    Mp3FileReader mp3File = new(audioFile);

                    audioFileForUpdate.AudioFileDuration = mp3File.TotalTime;

                    #endregion

                    IReadOnlyList<MetadataExtractor.Directory> audioDirectories = ImageMetadataReader.ReadMetadata(audioFile);

                    foreach (var audioDirectory in audioDirectories)
                    {
                        foreach (var tag in audioDirectory.Tags)
                        {
                            #region Битрейт аудиофайла

                            if (audioDirectory.Name == "MP3" && tag.Name == "Bitrate")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("audioFileForEditing.AudioFileBitRate", "Битрейт аудиофайла равен 0");

                                    // Список аудиокниг
                                    ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                    // Список страниц сайта
                                    ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                    // Список текстовых файлов
                                    ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                    // Список папок для аудиофайлов
                                    ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                    return View(audioFileForEditing);
                                }
                                else
                                {
                                    audioFileForUpdate.AudioFileBitRate = int.Parse(tag.Description);
                                }
                            }

                            #endregion

                            #region Частота аудиофайла

                            if (audioDirectory.Name == "MP3" && tag.Name == "Frequency")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("audioFileForEditing.AudioFileFrequency", "Частота аудиофайла равен 0");

                                    // Список аудиокниг
                                    ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                    // Список страниц сайта
                                    ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                    // Список текстовых файлов
                                    ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                    // Список папок для аудиофайлов
                                    ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                    return View(audioFileForEditing);
                                }
                                else
                                {
                                    audioFileForUpdate.AudioFileFrequency = int.Parse(tag.Description);
                                }
                            }

                            #endregion

                            #region Размер аудиофайла

                            if (audioDirectory.Name == "File" && tag.Name == "File Size")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("audioFileForEditing.AudioFileFrequency", "Размер аудиофайла равен 0");

                                    // Список аудиокниг
                                    ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                    // Список страниц сайта
                                    ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                    // Список текстовых файлов
                                    ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                    // Список папок для аудиофайлов
                                    ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                    return View(audioFileForEditing);
                                }
                                else
                                {
                                    audioFileForUpdate.AudioFileSize = int.Parse(tag.Description[..tag.Description.IndexOf(' ')]);
                                }
                            }

                            #endregion

                            #region MIME Type аудиофайла

                            if (audioDirectory.Name == "File Type" && tag.Name == "Detected MIME Type")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("audioFileForEditing.AudioFileMimeType", "MIME Type не определен");

                                    // Список аудиокниг
                                    ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                    // Список страниц сайта
                                    ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                    // Список текстовых файлов
                                    ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                    // Список папок для аудиофайлов
                                    ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                    return View(audioFileForEditing);
                                }
                                else
                                {
                                    audioFileForUpdate.AudioFileMimeType = tag.Description;
                                }
                            }

                            #endregion

                            #region Расширение аудиофайла

                            if (audioDirectory.Name == "File Type" && tag.Name == "Expected File Name Extension")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("audioFileForEditing.AudioFileType", "Расширение файла не определено");

                                    // Список аудиокниг
                                    ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                    // Список страниц сайта
                                    ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                    // Список текстовых файлов
                                    ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                    // Список папок для аудиофайлов
                                    ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                    return View(audioFileForEditing);
                                }
                                else
                                {
                                    audioFileForUpdate.AudioFileType = tag.Description;
                                }
                            }

                            #endregion

                            #region Имя аудиофайла

                            if (audioDirectory.Name == "File" && tag.Name == "File Name")
                            {
                                if (string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("audioFileForEditing.AudioFileName", "Имя файла не определено");

                                    // Список аудиокниг
                                    ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                                    // Список страниц сайта
                                    ViewData["PagesOfSite"] = new SelectList(pagesOfSiteContext.PagesInfoNoTracking.OrderBy(page => page.PageTitle), "PageInfoModelId", "PageTitle");

                                    // Список текстовых файлов
                                    ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                                    // Список папок для аудиофайлов
                                    ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                                    return View(audioFileForEditing);
                                }
                                else
                                {
                                    audioFileForUpdate.AudioFileName = tag.Description;
                                }
                            }

                            #endregion
                        }
                    }
                }

                #endregion

                #region Изменить в БД

                await audioFileContext.SaveChangesInAudioFileAsync();

                #endregion

                #region Открытие параметров измененного аудиофайла

                return RedirectToAction(nameof(DetailsAudioFile), new { audioFileId = audioFileForUpdate.AudioInfoModelId });

                #endregion
            }
            else
            {
                // Список аудиокниг
                ViewData["AudioBooks"] = new SelectList(audioBookContext.AudioBooks, "AudioBookModelId", "CaptionOfAudioBook");

                // Список текстовых файлов
                ViewData["TextFIles"] = new SelectList(textsOfSiteContext.Texts, "TextInfoModelId", "TxtFileName");

                // Список папок для аудиофайлов
                ViewData["AudioFileFolders"] = new SelectList(System.IO.Directory.GetDirectories(DataConfig.AudioFoldersPath, "*", SearchOption.AllDirectories));

                return View(audioFileForEditing);
            }
        }

        #endregion

        #region Временный код

        //// GET: Admin/AudioInfoModels/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var audioInfoModel = await _context.AudioInfoModel
        //        .Include(a => a.AudioBookModel)
        //        .Include(a => a.PageInfoModel)
        //        .Include(a => a.TextInfoModel)
        //        .FirstOrDefaultAsync(m => m.AudioInfoModelId == id);
        //    if (audioInfoModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(audioInfoModel);
        //}

        //// POST: Admin/AudioInfoModels/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var audioInfoModel = await _context.AudioInfoModel.FindAsync(id);
        //    if (audioInfoModel != null)
        //    {
        //        _context.AudioInfoModel.Remove(audioInfoModel);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool AudioInfoModelExists(Guid id)
        //{
        //    return _context.AudioInfoModel.Any(e => e.AudioInfoModelId == id);
        //}

        #endregion
    }
}