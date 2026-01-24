using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ShevkunenkoSite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class TextInfoController(
    ITextInfoRepository textContext,
    IBooksAndArticlesRepository bookContext,
    IMovieFileRepository movieContext,
    IAudioInfoRepository audioFIleContext,
    IWebHostEnvironment hostEnvironment
    ) : Controller
{
    private readonly string rootPath = hostEnvironment.WebRootPath;

    #region Список текстов

    public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
    {
        var allTextFiles = await textContext.Texts.ToListAsync();

        if (!searchString.IsNullOrEmpty())
        {
            allTextFiles = [.. allTextFiles.TextSearch(searchString, hostEnvironment).OrderBy(text => text.TextDescription)];
        }

        return View(new ItemsListViewModel
        {
            AllTextFiles = [.. allTextFiles
                     .Skip((pageNumber - 1)* DataConfig.NumberOfItemsPerPage)
                     .Take(DataConfig.NumberOfItemsPerPage)],

            #region Свойства PagingInfoViewModel

            TotalItems = allTextFiles.Count,

            ItemsPerPage = DataConfig.NumberOfItemsPerPage,

            CurrentPage = pageNumber,

            SearchString = searchString ?? string.Empty

            #endregion
        });
    }

    #endregion

    #region Информация о тексте

    public async Task<IActionResult> DetailsText(Guid? textId)
    {
        if (textId.HasValue & await textContext.Texts
                                        .Where(p => p.TextInfoModelId == textId)
                                        .AnyAsync())
        {
            var textItem = await textContext.Texts
                .Include(book => book.BooksAndArticlesModel)
                .Include(audiofile => audiofile.AudioInfoModel)
                .AsNoTracking()
                .FirstAsync(p => p.TextInfoModelId == textId);

            using StreamReader clearText = new(rootPath + DataConfig.TextsFolderPath + textItem.FolderForText + textItem.TxtFileName);

            textItem.ClearText = clearText.ReadToEnd();

            using StreamReader htmlText = new(rootPath + DataConfig.TextsFolderPath + textItem.FolderForText + textItem.HtmlFileName);

            textItem.HtmlText = htmlText.ReadToEnd();

            return View(textItem);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

    #region Добавить текст

    [HttpGet]
    public IActionResult AddText()
    {
        TextInfoModel newText = new();

        #region Список папок для текстов

        var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

        var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

        ViewData["TextFileFolders"] = new SelectList(folders
                .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

        #endregion

        #region Список книг и статей

        ViewData["BooksAndArticles"] = new SelectList(
            bookContext.BooksAndArticles
                    .OrderBy(book => book.CaptionOfText),
            "BooksAndArticlesModelId",
            "CaptionOfText");

        #endregion

        #region Список аудиофайлов

        ViewData["AudioFiles"] = new SelectList(
           audioFIleContext.AudioFiles
                .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
            "AudioInfoModelId",
            "CaptionOfTextInAudioFile");

        #endregion

        return View(newText);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(5_268_435_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
    public async Task<IActionResult> AddText(
                    [Bind(
                    "NewTextFolder," +
                    "TxtFileFormFile," +
                    "HtmlFileFormFile," +
                    "TextDescription," +
                    "FolderForText," +
                    "BooksAndArticlesModelId," +
                    "SequenceNumber"
        )]
        TextInfoModel addText)
    {
        if (ModelState.IsValid)
        {
            #region TXT файл - имя файла, размер файла

            if (addText.TxtFileFormFile != null)
            {
                if (!addText.TxtFileFormFile.FileName.EndsWith(".txt"))
                {
                    ModelState.AddModelError("TxtFileFormFile", "Выберите файл формата «.txt»");

                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion

                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion

                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(addText);
                }

                if (await textContext.Texts.Where(file => file.TxtFileName == addText.TxtFileFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("TxtFileFormFile", $"Файл с именем «{addText.TxtFileFormFile.FileName}» уже существует в БД");

                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion

                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion

                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(addText);
                }

                addText.TxtFileName = addText.TxtFileFormFile.FileName;

                addText.TxtFileSize = (int)addText.TxtFileFormFile.Length;
            }
            else
            {
                ModelState.AddModelError("TxtFileFormFile", "Вы не выбрали файл");

                #region Список папок для текстов

                var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                ViewData["TextFileFolders"] = new SelectList(folders
                        .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                #endregion

                #region Список книг и статей

                ViewData["BooksAndArticles"] = new SelectList(
                    bookContext.BooksAndArticles
                            .OrderBy(book => book.CaptionOfText),
                    "BooksAndArticlesModelId",
                    "CaptionOfText");

                #endregion

                #region Список аудиофайлов

                ViewData["AudioFiles"] = new SelectList(
                   audioFIleContext.AudioFiles
                        .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                    "AudioInfoModelId",
                    "CaptionOfTextInAudioFile");

                #endregion

                return View(addText);
            }

            #endregion
            
            #region HTML файл - имя файла, размер файла

            if (addText.HtmlFileFormFile != null)
            {
                if (!addText.HtmlFileFormFile.FileName.EndsWith(".html"))
                {
                    ModelState.AddModelError("HtmlFileFormFile", "Выберите файл формата «.html»");
                    
                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion
                    
                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion
                    
                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(addText);
                }

                if (await textContext.Texts.Where(file => file.HtmlFileName == addText.HtmlFileFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("HtmlFileFormFile", $"Файл с именем «{addText.HtmlFileFormFile.FileName}» уже существует в БД");
                    
                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion
                    
                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion
                    
                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(addText);
                }

                addText.HtmlFileName = addText.HtmlFileFormFile.FileName;

                addText.HtmlFileSize = (int)addText.HtmlFileFormFile.Length;
            }
            else
            {
                ModelState.AddModelError("HtmlFileFormFile", "Вы не выбрали файл");
                
                #region Список папок для текстов

                var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                ViewData["TextFileFolders"] = new SelectList(folders
                        .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                #endregion
                
                #region Список книг и статей

                ViewData["BooksAndArticles"] = new SelectList(
                    bookContext.BooksAndArticles
                            .OrderBy(book => book.CaptionOfText),
                    "BooksAndArticlesModelId",
                    "CaptionOfText");

                #endregion
                
                #region Список аудиофайлов

                ViewData["AudioFiles"] = new SelectList(
                   audioFIleContext.AudioFiles
                        .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                    "AudioInfoModelId",
                    "CaptionOfTextInAudioFile");

                #endregion

                return View(addText);
            }

            #endregion
            
            #region Номер страницы

            if (addText.SequenceNumber == null)
            {
                _ = addText.SequenceNumber;
            }
            else
            {
                if (addText.BooksAndArticlesModelId != null
                                    & await textContext.Texts.Where(text =>
                                        text.BooksAndArticlesModelId == addText.BooksAndArticlesModelId
                                        & text.SequenceNumber == addText.SequenceNumber)
                                    .AnyAsync())
                {
                    ModelState.AddModelError("SequenceNumber", $"Для книги (статьи) «{addText.BooksAndArticlesModelId}», страница «{addText.SequenceNumber}» уже существует");
                    
                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion
                    
                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion
                    
                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(addText);
                }
                else if (addText.BooksAndArticlesModelId != null)
                {
                    var book = await bookContext
                        .BooksAndArticles
                        .FirstAsync(book => book.BooksAndArticlesModelId == addText.BooksAndArticlesModelId);

                    if (addText.SequenceNumber > book.NumberOfPages)
                    {
                        ModelState.AddModelError("SequenceNumber", $"У книги (статьи) «{addText.BooksAndArticlesModelId}», всего «{book.NumberOfPages}» страниц");
                        
                        #region Список папок для текстов

                        var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                        var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                        ViewData["TextFileFolders"] = new SelectList(folders
                                .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                        #endregion
                        
                        #region Список книг и статей

                        ViewData["BooksAndArticles"] = new SelectList(
                            bookContext.BooksAndArticles
                                .Where(book => book.TypeOfText == "book")
                                    .OrderBy(book => book.CaptionOfText),
                            "BooksAndArticlesModelId",
                            "CaptionOfText");

                        #endregion
                        
                        #region Список аудиофайлов

                        ViewData["AudioFiles"] = new SelectList(
                           audioFIleContext.AudioFiles
                                .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                            "AudioInfoModelId",
                            "CaptionOfTextInAudioFile");

                        #endregion

                        return View(addText);
                    }
                }
                else
                {
                    _ = addText.SequenceNumber;
                }
            }

            #endregion

            #region Описание текста

            addText.TextDescription = addText.HtmlFileFormFile.FileName.Contains("sledstvie-prodoljaetsya-")
                ? "Николай Модестов «Следствие продолжается» страница " + addText.SequenceNumber + "."
                : addText.TextDescription.Trim();

            // Проверка наличия описания в базе данных
            if (await textContext.Texts.Where(file => file.TextDescription == addText.TextDescription).AnyAsync())
            {
                ModelState.AddModelError("TextDescription", $"Описание «{addText.TextDescription}» уже существует в БД");

                #region Список папок для текстов

                var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                ViewData["TextFileFolders"] = new SelectList(folders
                        .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                #endregion

                #region Список книг и статей

                ViewData["BooksAndArticles"] = new SelectList(
                    bookContext.BooksAndArticles
                            .OrderBy(book => book.CaptionOfText),
                    "BooksAndArticlesModelId",
                    "CaptionOfText");

                #endregion

                #region Список аудиофайлов

                ViewData["AudioFiles"] = new SelectList(
                   audioFIleContext.AudioFiles
                        .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                    "AudioInfoModelId",
                    "CaptionOfTextInAudioFile");

                #endregion

                return View(addText);
            }

            #endregion

            #region Аудиофайл для текста

            _ = addText.AudioInfoModelId;

            #endregion

            #region Связанная книга (статья)

            if (addText.HtmlFileFormFile.FileName.Contains("sledstvie-prodoljaetsya-"))
            {
                addText.BooksAndArticlesModelId = new Guid("2e08e370-6c55-4aa8-1bb3-08de5b313618");
            }
            else
            {
                _ = addText.BooksAndArticlesModelId;
            }

            #endregion

            #region Каталог текста

            if (addText.HtmlFileFormFile.FileName.Contains("sledstvie-prodoljaetsya-"))
            {
                addText.FolderForText = "sledstvie-prodoljaetsya";
            }
            
            if (addText.FolderForText != "texts")
            {
                addText.FolderForText += '/';
            }
            else
            {
                addText.FolderForText = string.Empty;
            }

            if (!string.IsNullOrEmpty(addText.NewTextFolder))
            {
                addText.FolderForText = addText.NewTextFolder.Trim().Trim('/') + '/';
            }

            string path = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot/texts", addText.FolderForText)).Replace('\\', '/');

            #endregion

            #region Создание нового каталога

            if (System.IO.Directory.Exists(path) & addText.NewTextFolder != string.Empty)
            {
                ModelState.AddModelError("NewTextFolder", $"Каталог «{addText.NewTextFolder.Trim('/')}» уже существует");

                #region Список папок для текстов

                var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                ViewData["TextFileFolders"] = new SelectList(folders
                        .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                #endregion

                #region Список книг и статей

                ViewData["BooksAndArticles"] = new SelectList(
                    bookContext.BooksAndArticles
                            .OrderBy(book => book.CaptionOfText),
                    "BooksAndArticlesModelId",
                    "CaptionOfText");

                #endregion

                #region Список аудиофайлов

                ViewData["AudioFiles"] = new SelectList(
                   audioFIleContext.AudioFiles
                        .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                    "AudioInfoModelId",
                    "CaptionOfTextInAudioFile");

                #endregion

                return View();
            }

            if (!System.IO.Directory.Exists(path))
            {
                _ = System.IO.Directory.CreateDirectory(path);
            }

            #endregion

            #region Копировать файлы в формате UTF8 с BOM

            if (addText.TxtFileFormFile != null)
            {
                // путь к TXT файлу 
                var pathForCopyTxt = rootPath + DataConfig.TextsFolderPath + addText.FolderForText + addText.TxtFileFormFile.FileName;

                using StreamReader readerTxt = new(addText.TxtFileFormFile.OpenReadStream());

                string txtWrite = await readerTxt.ReadToEndAsync();

                // полная перезапись файла 
                using StreamWriter writer = new(pathForCopyTxt, false, new UTF8Encoding(true));

                await writer.WriteLineAsync(txtWrite);

                writer.Close();
            }

            if (addText.HtmlFileFormFile != null)
            {
                // путь к HTML файлу 
                var pathForCopyHtml = rootPath + DataConfig.TextsFolderPath + addText.FolderForText + addText.HtmlFileFormFile.FileName;

                using StreamReader readerHtml = new(addText.HtmlFileFormFile.OpenReadStream());

                string htmlWrite = await readerHtml.ReadToEndAsync();

                // полная перезапись файла 
                using StreamWriter writer = new(pathForCopyHtml, false, new UTF8Encoding(true));

                await writer.WriteLineAsync(htmlWrite);

                writer.Close();
            }

            #endregion

            #region Добавить в БД

            await textContext.AddNewTextAsync(addText);

            #endregion

            #region Открытие параметров добавленного файла

            var newText = await textContext.Texts.FirstAsync(file => file.TxtFileName == addText.TxtFileName);

            return RedirectToAction(nameof(DetailsText), new { textId = newText.TextInfoModelId });

            #endregion
        }
        else
        {
            #region Список папок для текстов

            var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

            var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

            ViewData["TextFileFolders"] = new SelectList(folders
                    .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

            #endregion

            #region Список книг и статей

            ViewData["BooksAndArticles"] = new SelectList(
                bookContext.BooksAndArticles
                    .Where(book => book.TypeOfText == "book")
                        .OrderBy(book => book.CaptionOfText),
                "BooksAndArticlesModelId",
                "CaptionOfText");

            #endregion

            #region Список аудиофайлов

            ViewData["AudioFiles"] = new SelectList(
               audioFIleContext.AudioFiles
                    .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                "AudioInfoModelId",
                "CaptionOfTextInAudioFile");

            #endregion

            return View(addText);
        }
    }

    #endregion

    #region Изменить текст

    [HttpGet]
    public async Task<IActionResult> EditText(Guid? textId)
    {
        if (textId.HasValue & await textContext.Texts.Where(i => i.TextInfoModelId == textId).AnyAsync())
        {
            #region Список папок для текстов

            var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

            var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

            ViewData["TextFileFolders"] = new SelectList(folders
                    .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

            #endregion

            #region Список книг и статей

            ViewData["BooksAndArticles"] = new SelectList(
                bookContext.BooksAndArticles
                        .OrderBy(book => book.CaptionOfText),
                "BooksAndArticlesModelId",
                "CaptionOfText");

            #endregion

            #region Список аудиофайлов

            ViewData["AudioFiles"] = new SelectList(
               audioFIleContext.AudioFiles
                    .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                "AudioInfoModelId",
                "CaptionOfTextInAudioFile");

            #endregion

            TextInfoModel textInfoForEditing = await textContext.Texts
                .Include(book => book.BooksAndArticlesModel)
                .Include(audioFile => audioFile.AudioInfoModel)
                .FirstAsync(i => i.TextInfoModelId == textId);

            #region Текущий каталог

            textInfoForEditing.CurrentTextFolder = textInfoForEditing.FolderForText;

            #endregion

            using StreamReader clearText = new(rootPath + DataConfig.TextsFolderPath + textInfoForEditing.FolderForText + textInfoForEditing.TxtFileName);

            textInfoForEditing.ClearText = await clearText.ReadToEndAsync();

            using StreamReader htmlText = new(rootPath + DataConfig.TextsFolderPath + textInfoForEditing.FolderForText + textInfoForEditing.HtmlFileName);

            textInfoForEditing.HtmlText = await htmlText.ReadToEndAsync();

            return View(textInfoForEditing);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditText(
         [Bind(
        "TextInfoModelId," +
                    "TextDescription," +
                    "TxtFileName," +
                    "TxtFileSize," +
                    "HtmlFileName," +
                    "HtmlFileSize," +
                    "TxtFileFormFile," +
                    "HtmlFileFormFile," +
                    "ClearText," +
                    "HtmlText," +
                    "FolderForText," +
                    "CurrentTextFolder," +
                    "NewTextFolder," +
                    "SequenceNumber," +
                    "BooksAndArticlesModelId," +
                    "BooksAndArticlesModel," +
                    "AudioInfoModelId"
        )]
        TextInfoModel textInfoForEditing)
    {
        if (ModelState.IsValid)
        {
            TextInfoModel textForUpdate = await textContext.Texts
                .Include(book => book.BooksAndArticlesModel)
                .FirstAsync(text => text.TextInfoModelId == textInfoForEditing.TextInfoModelId);

            #region Папка файлов текста

            string oldPathToTxtFile = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot/texts", textInfoForEditing.CurrentTextFolder, textForUpdate.TxtFileName)).Replace('\\', '/');

            string oldPathToHtmlFile = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot/texts", textInfoForEditing.CurrentTextFolder, textForUpdate.HtmlFileName)).Replace('\\', '/');

            if (string.IsNullOrEmpty(textInfoForEditing.NewTextFolder))
            {
                if (textInfoForEditing.CurrentTextFolder.Trim('/') != textInfoForEditing.FolderForText)
                {
                    textForUpdate.FolderForText = textInfoForEditing.FolderForText + '/';

                    string newPathToTxtFile = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot/texts", textInfoForEditing.FolderForText, textForUpdate.TxtFileName)).Replace('\\', '/');

                    string newPathToHtmlFile = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot/texts", textInfoForEditing.FolderForText, textForUpdate.HtmlFileName)).Replace('\\', '/');

                    System.IO.File.Move(oldPathToTxtFile, newPathToTxtFile, true);

                    System.IO.File.Move(oldPathToHtmlFile, newPathToHtmlFile, true);
                }
            }
            else
            {
                #region Создание нового каталога

                string newPathToFiles = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot/texts", textInfoForEditing.NewTextFolder)).Replace('\\', '/');

                if (System.IO.Directory.Exists(newPathToFiles))
                {
                    ModelState.AddModelError("NewTextFolder", $"Каталог «{textInfoForEditing.NewTextFolder.Trim('/')}» уже существует");

                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion

                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                            .Where(book => book.TypeOfText == "book")
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion

                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(textInfoForEditing);
                }
                else
                {
                    _ = System.IO.Directory.CreateDirectory(newPathToFiles);

                    textForUpdate.FolderForText = textInfoForEditing.NewTextFolder.Trim('/') + '/';

                    string newPathToTxtFile = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot/texts", textInfoForEditing.NewTextFolder, textForUpdate.TxtFileName)).Replace('\\', '/');

                    string newPathToHtmlFile = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot/texts", textInfoForEditing.NewTextFolder, textForUpdate.HtmlFileName)).Replace('\\', '/');

                    System.IO.File.Move(oldPathToTxtFile, newPathToTxtFile);

                    System.IO.File.Move(oldPathToHtmlFile, newPathToHtmlFile);
                }

                #endregion
            }

            #endregion

            #region Описание текста

            textForUpdate.TextDescription = textInfoForEditing.TextDescription.Trim();

            if (await textContext.Texts
                .Where(text =>
                    text.TextInfoModelId != textInfoForEditing.TextInfoModelId
                    & text.TextDescription == textForUpdate.TextDescription)
                .AnyAsync())
            {
                ModelState.AddModelError("TextDescription", $"Описание «{textInfoForEditing.TextDescription}», уже существует для другог текста.");

                #region Список папок для текстов

                var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                ViewData["TextFileFolders"] = new SelectList(folders
                        .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                #endregion

                #region Список книг и статей

                ViewData["BooksAndArticles"] = new SelectList(
                    bookContext.BooksAndArticles
                        .Where(book => book.TypeOfText == "book")
                            .OrderBy(book => book.CaptionOfText),
                    "BooksAndArticlesModelId",
                    "CaptionOfText");

                #endregion

                #region Список аудиофайлов

                ViewData["AudioFiles"] = new SelectList(
                   audioFIleContext.AudioFiles
                        .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                    "AudioInfoModelId",
                    "CaptionOfTextInAudioFile");

                #endregion

                return View(textInfoForEditing);
            }

            #endregion

            #region Связанная книга (статья)

            textForUpdate.BooksAndArticlesModelId = textInfoForEditing.BooksAndArticlesModelId;

            #endregion

            #region Номер страницы

            if (textInfoForEditing.BooksAndArticlesModelId == null)
            {
                textForUpdate.SequenceNumber = null;
            }
            else if (textForUpdate.SequenceNumber != null & textInfoForEditing.BooksAndArticlesModelId != null & textInfoForEditing.SequenceNumber == null)
            {
                ModelState.AddModelError("SequenceNumber", $"Если указана книга (статья), нужно указать страницу");

                #region Список папок для текстов

                var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                ViewData["TextFileFolders"] = new SelectList(folders
                        .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                #endregion

                #region Список книг и статей

                ViewData["BooksAndArticles"] = new SelectList(
                    bookContext.BooksAndArticles
                            .OrderBy(book => book.CaptionOfText),
                    "BooksAndArticlesModelId",
                    "CaptionOfText");

                #endregion

                #region Список аудиофайлов

                ViewData["AudioFiles"] = new SelectList(
                   audioFIleContext.AudioFiles
                        .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                    "AudioInfoModelId",
                    "CaptionOfTextInAudioFile");

                #endregion

                return View(textInfoForEditing);
            }
            else if (textInfoForEditing.SequenceNumber != textForUpdate.SequenceNumber
                    && await textContext.Texts
                        .Where(text =>
                            text.BooksAndArticlesModelId == textInfoForEditing.BooksAndArticlesModelId
                            & text.SequenceNumber == textInfoForEditing.SequenceNumber)
                    .AnyAsync())
            {
                ModelState.AddModelError("SequenceNumber", $"Для выбранной книги (статьи), страница «{textInfoForEditing.SequenceNumber}» уже существует");

                textInfoForEditing.SequenceNumber = textForUpdate.SequenceNumber;

                #region Список папок для текстов

                var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                ViewData["TextFileFolders"] = new SelectList(folders
                        .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                #endregion

                #region Список книг и статей

                ViewData["BooksAndArticles"] = new SelectList(
                    bookContext.BooksAndArticles
                            .OrderBy(book => book.CaptionOfText),
                    "BooksAndArticlesModelId",
                    "CaptionOfText");

                #endregion

                #region Список аудиофайлов

                ViewData["AudioFiles"] = new SelectList(
                   audioFIleContext.AudioFiles
                        .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                    "AudioInfoModelId",
                    "CaptionOfTextInAudioFile");

                #endregion

                return View(textInfoForEditing);
            }
            else if (textInfoForEditing.BooksAndArticlesModelId != null
                    & textInfoForEditing.SequenceNumber > textForUpdate.BooksAndArticlesModel?.NumberOfPages)
            {
                ModelState.AddModelError("SequenceNumber", $"У книги (статьи) «{textInfoForEditing.BooksAndArticlesModel?.CaptionOfText}», всего «{textForUpdate.BooksAndArticlesModel?.NumberOfPages}» страниц");

                #region Список папок для текстов

                var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                ViewData["TextFileFolders"] = new SelectList(folders
                        .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                #endregion

                #region Список книг и статей

                ViewData["BooksAndArticles"] = new SelectList(
                    bookContext.BooksAndArticles
                            .OrderBy(book => book.CaptionOfText),
                    "BooksAndArticlesModelId",
                    "CaptionOfText");

                #endregion

                #region Список аудиофайлов

                ViewData["AudioFiles"] = new SelectList(
                   audioFIleContext.AudioFiles
                        .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                    "AudioInfoModelId",
                    "CaptionOfTextInAudioFile");

                #endregion

                return View(textInfoForEditing);
            }
            else
            {
                textForUpdate.SequenceNumber = textInfoForEditing.SequenceNumber;
            }

            #endregion

            #region Текст без разметки (TXT)

            // если выбран новый файл
            if (textInfoForEditing.TxtFileFormFile != null)
            {
                if (!textInfoForEditing.TxtFileFormFile.FileName.EndsWith(".txt"))
                {
                    ModelState.AddModelError("TxtFileFormFile", "Выберите файл формата «.txt»");

                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion

                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                            .Where(book => book.TypeOfText == "book")
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion

                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(textInfoForEditing);
                }

                if (await textContext.Texts.Where(file => file.TxtFileName == textInfoForEditing.TxtFileFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("TxtFileFormFile", $"Файл с именем «{textInfoForEditing.TxtFileFormFile.FileName}» уже существует в БД");

                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion

                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                            .Where(book => book.TypeOfText == "book")
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion

                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(textInfoForEditing);
                }

                textForUpdate.TxtFileName = textInfoForEditing.TxtFileFormFile.FileName;

                textForUpdate.TxtFileSize = (int)textInfoForEditing.TxtFileFormFile.Length;

                using FileStream txtStream = new(rootPath + DataConfig.TextsFolderPath + textForUpdate.FolderForText + textInfoForEditing.TxtFileFormFile.FileName, FileMode.Create);

                await textInfoForEditing.TxtFileFormFile.CopyToAsync(txtStream);
            }
            // если новый файл не выбран
            else
            {
                if (string.IsNullOrEmpty(textInfoForEditing.ClearText))
                {
                    ModelState.AddModelError("ClearTextInput", $"Вы пытаетесь сохранить файл без текста.");

                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion

                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                            .Where(book => book.TypeOfText == "book")
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion

                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(textInfoForEditing);
                }

                _ = textInfoForEditing.ClearText.Trim();

                // путь к файлу 
                var pathForCopyTxt = rootPath + DataConfig.TextsFolderPath + textForUpdate.FolderForText + textInfoForEditing.TxtFileName;

                // полная перезапись файла 
                using StreamWriter writer = new(pathForCopyTxt, false, new UTF8Encoding(true));

                await writer.WriteLineAsync(textInfoForEditing.ClearText);

                writer.Close();

                // новый размер файла txt
                textForUpdate.TxtFileSize = (int)new FileInfo(pathForCopyTxt).Length;
            }

            #endregion

            #region Текст с разметкой (HTML)

            // если выбран новый файл
            if (textInfoForEditing.HtmlFileFormFile != null)
            {
                if (!textInfoForEditing.HtmlFileFormFile.FileName.EndsWith(".html"))
                {
                    ModelState.AddModelError("HtmlFileFormFile", "Выберите файл формата «.html»");

                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion

                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                            .Where(book => book.TypeOfText == "book")
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion

                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(textInfoForEditing);
                }

                if (await textContext.Texts.Where(file => file.HtmlFileName == textInfoForEditing.HtmlFileFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("HtmlFileFormFile", $"Файл с именем «{textInfoForEditing.HtmlFileFormFile.FileName}» уже существует в БД");

                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion

                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                            .Where(book => book.TypeOfText == "book")
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion

                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(textInfoForEditing);
                }

                textForUpdate.HtmlFileName = textInfoForEditing.HtmlFileFormFile.FileName;

                textForUpdate.HtmlFileSize = (int)textInfoForEditing.HtmlFileFormFile.Length;

                using FileStream txtStream = new(rootPath + DataConfig.TextsFolderPath + textForUpdate.FolderForText + textInfoForEditing.HtmlFileFormFile.FileName, FileMode.Create);

                await textInfoForEditing.HtmlFileFormFile.CopyToAsync(txtStream);
            }
            // если новый файл не выбран
            else
            {
                if (string.IsNullOrEmpty(textInfoForEditing.HtmlText))
                {
                    ModelState.AddModelError("HtmlTextInput", $"Вы пытаетесь сохранить файл без текста.");

                    #region Список папок для текстов

                    var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

                    var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

                    ViewData["TextFileFolders"] = new SelectList(folders
                            .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

                    #endregion

                    #region Список книг и статей

                    ViewData["BooksAndArticles"] = new SelectList(
                        bookContext.BooksAndArticles
                            .Where(book => book.TypeOfText == "book")
                                .OrderBy(book => book.CaptionOfText),
                        "BooksAndArticlesModelId",
                        "CaptionOfText");

                    #endregion

                    #region Список аудиофайлов

                    ViewData["AudioFiles"] = new SelectList(
                       audioFIleContext.AudioFiles
                            .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                        "AudioInfoModelId",
                        "CaptionOfTextInAudioFile");

                    #endregion

                    return View(textInfoForEditing);
                }

                _ = textInfoForEditing.HtmlText.Trim();

                // путь к файлу 
                var pathForCopyHtml = rootPath + DataConfig.TextsFolderPath + textForUpdate.FolderForText + textInfoForEditing.HtmlFileName;

                // полная перезапись файла 
                using StreamWriter writer = new(pathForCopyHtml, false, new UTF8Encoding(true));

                await writer.WriteLineAsync(textInfoForEditing.HtmlText);

                writer.Close();

                // новый размер файла txt
                textForUpdate.TxtFileSize = (int)new FileInfo(pathForCopyHtml).Length;
            }

            #endregion

            #region Аудиофайл 

            textForUpdate.AudioInfoModelId = textInfoForEditing.AudioInfoModelId;

            #endregion

            #region Сохранить и перейти к DetailsText

            await textContext.SaveChangesInTextAsync();

            return RedirectToAction(nameof(DetailsText), new { textId = textForUpdate.TextInfoModelId });

            #endregion
        }
        else
        {
            #region Список папок для текстов

            var folders = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot" + DataConfig.TextsFolderPath, "*", SearchOption.AllDirectories);

            var trimDirectorieFolder = folders[0].IndexOf("texts") + 6;

            ViewData["TextFileFolders"] = new SelectList(folders
                    .Select(x => new { value = x[trimDirectorieFolder..], text = x[trimDirectorieFolder..] }), "value", "text");

            #endregion

            #region Список книг и статей

            ViewData["BooksAndArticles"] = new SelectList(
                bookContext.BooksAndArticles
                        .OrderBy(book => book.CaptionOfText),
                "BooksAndArticlesModelId",
                "CaptionOfText");

            #endregion

            #region Список аудиофайлов

            ViewData["AudioFiles"] = new SelectList(
               audioFIleContext.AudioFiles
                    .OrderBy(audioFile => audioFile.CaptionOfTextInAudioFile),
                "AudioInfoModelId",
                "CaptionOfTextInAudioFile");

            #endregion

            return View(textInfoForEditing);
        }
    }

    #endregion

    #region Удалить текст

    [HttpGet]
    public async Task<IActionResult> DeleteText(Guid? textId)
    {
        TextInfoModel deleteText = new();

        if (textId.HasValue & await textContext.Texts.Where(i => i.TextInfoModelId == textId).AnyAsync())
        {
            deleteText = await textContext.Texts.FirstAsync(i => i.TextInfoModelId == textId);

            using StreamReader clearText = new(rootPath + DataConfig.TextsFolderPath + deleteText.FolderForText + deleteText.TxtFileName);

            deleteText.ClearText = clearText.ReadToEnd();

            using StreamReader htmlText = new(rootPath + DataConfig.TextsFolderPath + deleteText.FolderForText + deleteText.HtmlFileName);

            deleteText.HtmlText = htmlText.ReadToEnd();

            return View(deleteText);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteText(TextInfoModel? deleteText)
    {
        if (deleteText != null)
        {
            if (await movieContext.MovieFiles.Where(i => i.TextInfoModelId == deleteText.TextInfoModelId).AnyAsync())
            {
                deleteText = await textContext.Texts.FirstAsync(i => i.TextInfoModelId == deleteText.TextInfoModelId);

                using StreamReader clearText = new(rootPath + DataConfig.TextsFolderPath + deleteText.TxtFileName);

                using StreamReader htmlText = new(rootPath + DataConfig.TextsFolderPath + deleteText.HtmlFileName);

                deleteText.RefInMovies = "Ссылка на файл в базе фильмов сайта!";

                return View(deleteText);

            }

            if (await textContext.Texts.Where(i => i.TextInfoModelId == deleteText.TextInfoModelId).AnyAsync())
            {
                deleteText = await textContext.Texts.FirstAsync(i => i.TextInfoModelId == deleteText.TextInfoModelId);

                var txtPath = rootPath + DataConfig.TextsFolderPath + deleteText.TxtFileName;

                FileInfo txtFile = new(txtPath);

                if (txtFile.Exists)
                {
                    var txtMoveToPath = rootPath + DataConfig.ArchiveTextsFolderPath + deleteText.TxtFileName;

                    txtFile.MoveTo(txtMoveToPath, true);
                }

                var htmlPath = rootPath + DataConfig.TextsFolderPath + deleteText.HtmlFileName;

                FileInfo htmlFile = new(htmlPath);

                if (htmlFile.Exists)
                {
                    var htmlMoveToPath = rootPath + DataConfig.ArchiveTextsFolderPath + deleteText.HtmlFileName;

                    htmlFile.MoveTo(htmlMoveToPath, true);
                }

                await textContext.DeleteTextAsync(deleteText.TextInfoModelId);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion
}