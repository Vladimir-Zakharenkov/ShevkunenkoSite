using System.Text.RegularExpressions;

namespace ShevkunenkoSite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public partial class ImageInfoController(
    IImageFileRepository imageContext,
    IBooksAndArticlesRepository bookContext) : Controller
{
    [GeneratedRegex(@"^[a-zA-Z0-9_\-]+$")]
    private static partial Regex ImagePathRegex();

    public ImageFileModel? ImageItem { get; set; }

    private readonly record struct ImageMetadata(
            string FileName,
            string Extension,
            string MimeType,
            uint FileSize,
            uint Width,
            uint Height);

    private static readonly (string Name, Func<EditImageViewModel, IFormFile?> Selector)[] UploadFields =
[
    (nameof(EditImageViewModel.ImageHDFormFile),     m => m.ImageHDFormFile),
    (nameof(EditImageViewModel.ImageFormFile),       m => m.ImageFormFile),
    (nameof(EditImageViewModel.IconFormFile),        m => m.IconFormFile),
    (nameof(EditImageViewModel.Icon200FormFile),     m => m.Icon200FormFile),
    (nameof(EditImageViewModel.Icon100FormFile),     m => m.Icon100FormFile),
    (nameof(EditImageViewModel.WebImageHDFormFile),  m => m.WebImageHDFormFile),
    (nameof(EditImageViewModel.WebImageFormFile),    m => m.WebImageFormFile),
    (nameof(EditImageViewModel.WebIconFormFile),     m => m.WebIconFormFile),
    (nameof(EditImageViewModel.WebIcon200FormFile),  m => m.WebIcon200FormFile),
    (nameof(EditImageViewModel.WebIcon100FormFile),  m => m.WebIcon100FormFile),
];

    #region Список картинок

    public async Task<IActionResult> Index
        (
        string? searchString,
        int pageNumber = 1,
        bool pageCard = false
        )
    {
        // Серверная выборка: фильтр + сортировка + пагинация
        IQueryable<ImageFileModel> query = imageContext.ImageFiles
                                                        .AsNoTracking()
                                                        .ImageSearch(searchString)
                                                        .OrderBy(siteImage => siteImage.SortOfPicture);

        List<ImageFileModel> pageItems = await query
                                            .Skip((pageNumber - 1) * DataConfig.NumberOfItemsPerPage)
                                            .Take(DataConfig.NumberOfItemsPerPage)
                                            .ToListAsync();

        #region Выбор представления - список или иконки

        ItemsListViewModel itemList = new()
        {
            AllImageFiles = pageItems,

            #region Свойства PagingInfoViewModel

            TotalItems = await query.CountAsync(),

            ItemsPerPage = DataConfig.NumberOfItemsPerPage,

            CurrentPage = pageNumber,

            SearchString = searchString ?? string.Empty,

            PageCard = pageCard

            #endregion
        };

        return pageCard == false
            ? View(itemList)
            : View("IconList", itemList);

        #endregion
    }

    #endregion

    #region Информация о картинке

    public async Task<IActionResult> DetailsImage(Guid? imageId, string? imageIcon)
    {
        DetailsImageIconViewModel detailsView = new();

        if (!imageId.HasValue) return RedirectToAction(nameof(Index));

        var image = await imageContext.ImageFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.ImageFileModelId == imageId);

        if (image == null)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            detailsView.ImageItem = image;
        }

        #region Определяем тип иконки в запросе

        string[] allowedIcons =
                [
                    "webhd", "hd", "webimage", "image",
                    "webicon300", "icon300", "webicon200", "icon200",
                    "webicon100", "icon100"
                ];

        detailsView.IconType = allowedIcons.Contains(imageIcon, StringComparer.OrdinalIgnoreCase)
                                ? allowedIcons.First(i => i.Equals(imageIcon, StringComparison.OrdinalIgnoreCase))
                                : "webimage";

        #endregion

        #region Определяем каталог картинки

        // Проверяем, что итоговый путь начинается с корня wwwroot
        string wwwRoot = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot"))
                     .Replace('\\', '/');

        string fullPath = Path.GetFullPath(Path.Join(wwwRoot, image.ImagePath)).Replace('\\', '/');

        if (!fullPath.Equals(wwwRoot, StringComparison.OrdinalIgnoreCase) &&
            !fullPath.StartsWith(wwwRoot + "/", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(Index));
        }

        // Проверка, что итоговый путь начинается с корня wwwroot
        // (альтернативный вариант — Path.GetRelativePath + проверка "..", не используется)
        //string relative = Path.GetRelativePath(wwwRoot, fullPath);
        //if (relative.StartsWith("..") || Path.IsPathRooted(relative))
        //    return RedirectToAction(nameof(Index));

        #endregion

        #region Определяем, css для ViewImage соответствует выбранному imageIcon

        string? cssForImage = detailsView.IconType switch
        {
            "webimage" => "maxwidth600px",
            "image" => "maxwidth600px",
            "webhd" => "maxwidth800px",
            "hd" => "maxwidth800px",
            "webicon300" => "maxwidth300px",
            "icon300" => "maxwidth300px",
            "webicon200" => "maxwidth200px",
            "icon200" => "maxwidth200px",
            "webicon100" => "maxwidth100px",
            "icon100" => "maxwidth100px",
            _ => null
        };

        detailsView.CurrentCss = cssForImage;

        #endregion

        #region Определяем, какой файл соответствует выбранному imageIcon

        string? fileForIcon = detailsView.IconType switch
        {
            "webimage" => image.WebImageFileName,
            "image" => image.ImageFileName,
            "webhd" => image.WebImageHDFileName,
            "hd" => image.ImageHDFileName,
            "webicon300" => image.WebIconFileName,
            "icon300" => image.IconFileName,
            "webicon200" => image.WebIcon200FileName,
            "icon200" => image.Icon200FileName,
            "webicon100" => image.WebIcon100FileName,
            "icon100" => image.Icon100FileName,
            _ => null
        };

        #endregion

        #region Проверяем наличие файла в каталоге

        if (!string.IsNullOrEmpty(fileForIcon))
        {
            detailsView.FileName = fileForIcon;
            detailsView.FileExists = System.IO.File.Exists(Path.Join(fullPath, fileForIcon));
        }

        // Отдельно — проверка остальных 10 файлов, чтобы установить красный стиль для отсутствующих,
        // даже если они не выбраны (это нужно для таблицы в представлении)
        ApplyIfFileMissing(image.WebImageFileName, fullPath, () => detailsView.WebImageButtonInfostyle = $"{DetailsImageIconViewModel.MainInfostyle} btn-outline-danger");
        ApplyIfFileMissing(image.ImageFileName, fullPath, () => detailsView.ImageButtonInfostyle = $"{DetailsImageIconViewModel.MainInfostyle} btn-outline-danger");
        ApplyIfFileMissing(image.WebImageHDFileName, fullPath, () => detailsView.WebhdButtonInfostyle = $"{DetailsImageIconViewModel.MainInfostyle} btn-outline-danger");
        ApplyIfFileMissing(image.ImageHDFileName, fullPath, () => detailsView.HdButtonInfostyle = $"{DetailsImageIconViewModel.MainInfostyle} btn-outline-danger");
        ApplyIfFileMissing(image.WebIconFileName, fullPath, () => detailsView.WebIcon300ButtonInfostyle = $"{DetailsImageIconViewModel.MainInfostyle} btn-outline-danger");
        ApplyIfFileMissing(image.IconFileName, fullPath, () => detailsView.Icon300ButtonInfostyle = $"{DetailsImageIconViewModel.MainInfostyle} btn-outline-danger");
        ApplyIfFileMissing(image.WebIcon200FileName, fullPath, () => detailsView.WebIcon200ButtonInfostyle = $"{DetailsImageIconViewModel.MainInfostyle} btn-outline-danger");
        ApplyIfFileMissing(image.Icon200FileName, fullPath, () => detailsView.Icon200ButtonInfostyle = $"{DetailsImageIconViewModel.MainInfostyle} btn-outline-danger");
        ApplyIfFileMissing(image.WebIcon100FileName, fullPath, () => detailsView.WebIcon100ButtonInfostyle = $"{DetailsImageIconViewModel.MainInfostyle} btn-outline-danger");
        ApplyIfFileMissing(image.Icon100FileName, fullPath, () => detailsView.Icon100ButtonInfostyle = $"{DetailsImageIconViewModel.MainInfostyle} btn-outline-danger");

        #endregion

        return View(detailsView);
    }

    #endregion

    #region Добавить картинку в базу данных

    [HttpGet]
    public ViewResult AddImage()
    {
        EditImageViewModel editImageViewModel = new();

        return View(editImageViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddImage(EditImageViewModel imageItem)
    {
        if (!ModelState.IsValid)
        {
            return View(imageItem);
        }

        #region Проверка, что выбран хотя бы один файл для загрузки

        if (UploadFields.All(f => f.Selector(imageItem) is null))
        {
            ModelState.AddModelError(string.Empty, "Загрузите хотя бы один файл");

            return View(imageItem);
        }

        #endregion

        #region Обрезка Caption + защита от пустой строки

        imageItem.EditImage.ImageCaption = imageItem.EditImage.ImageCaption?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(imageItem.EditImage.ImageCaption))
        {
            ModelState.AddModelError(nameof(imageItem.EditImage.ImageCaption), "Введите название картинки");

            return View(imageItem);
        }

        #endregion

        #region Проверка расширения «.webp» всех вводимых файлов

        if (!ValidateWebpFiles(imageItem))
        {
            return View(imageItem);
        }

        // Картинка WebP 720 px должна быть обязательно
        if (imageItem.WebImageFormFile is null)
        {
            ModelState.AddModelError(nameof(imageItem.WebImageFormFile),
                "Для добавления картинки требуется файл WebP 720 px (основной)");

            return View(imageItem);
        }

        #endregion

        #region Описание картинки

        imageItem.EditImage.ImageDescription = string.IsNullOrWhiteSpace(imageItem.EditImage.ImageDescription)
                        ? imageItem.EditImage.ImageCaption
                        : imageItem.EditImage.ImageDescription.Trim();

        #endregion

        #region Теги alt и title

        imageItem.EditImage.ImageAltTitle = string.IsNullOrWhiteSpace(imageItem.EditImage.ImageAltTitle)
                ? char.ToLowerInvariant(imageItem.EditImage.ImageCaption[0]) + imageItem.EditImage.ImageCaption[1..]
                : imageItem.EditImage.ImageAltTitle.Trim();

        #endregion

        #region Фильтр поиска

        imageItem.EditImage.SearchFilter = string.IsNullOrWhiteSpace(imageItem.EditImage.SearchFilter)
                ? null
                : imageItem.EditImage.SearchFilter.Trim();

        #endregion

        #region Каталог картинки

        string imagePath = imageItem.EditImage.ImagePath?.Trim('/') ?? string.Empty;

        imageItem.EditImage.ImagePath = string.IsNullOrEmpty(imagePath) || imagePath == "images"
            ? "/images"
            : "/images/" + imagePath;

        #endregion

        #region Ввод нового каталога

        if (!string.IsNullOrWhiteSpace(imageItem.NewImagePath))
        {
            string newPath = imageItem.NewImagePath.Trim('/');

            if (!ImagePathRegex().IsMatch(newPath))
            {
                ModelState.AddModelError(nameof(imageItem.NewImagePath),
                    "Имя каталога может содержать только буквы, цифры, «_» и «-»");
                return View(imageItem);
            }

            imageItem.EditImage.ImagePath = "/images/" + newPath;
        }

        #endregion

        #region Проверка отсутствия одинаковых файлов в IFormFile

        if (!ValidateUniqueFormFiles(imageItem))
        {
            return View(imageItem);
        }

        #endregion

        #region Проверка дублей в БД

        if (!await ValidateUniqueInDbAsync(imageItem))
        {
            return View(imageItem);
        }

        #endregion

        #region Проверяем, что итоговый путь начинается с корня wwwroot

        // ✅ проверка ПОСЛЕ применения NewImagePath
        string wwwRoot = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot"))
                             .Replace('\\', '/');

        string fullPath = Path.GetFullPath(Path.Join(wwwRoot, imageItem.EditImage.ImagePath))
                              .Replace('\\', '/');

        if (!fullPath.Equals(wwwRoot, StringComparison.OrdinalIgnoreCase) &&
            !fullPath.StartsWith(wwwRoot + "/", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(imageItem.EditImage.ImagePath),
                    $"Формат каталога «{imageItem.EditImage.ImagePath}» указан не верно");

            return View(imageItem);
        }

        string path = fullPath;

        if (System.IO.Directory.Exists(path) && !string.IsNullOrWhiteSpace(imageItem.NewImagePath))
        {
            ModelState.AddModelError(nameof(imageItem.NewImagePath), $"Каталог «{imageItem.NewImagePath}» уже существует");

            return View(imageItem);
        }

        #endregion

        #region Очистка полей для незагруженных IFormFile

        ClearUnusedFormFields(imageItem);

        #endregion

        #region Создание нового каталога

        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }

        #endregion

        #region Запись файлов в каталог + мета данные

        // 1. ImageHDFormFile — PNG HD
        if (imageItem.ImageHDFormFile is not null)
        {
            bool ok = await SaveFileWithMetadata(
                imageItem.ImageHDFormFile,
                path,
                meta =>
                {
                    imageItem.EditImage.ImageHDFileName = meta.FileName;
                    imageItem.EditImage.ImageHDNameExtension = meta.Extension;
                    imageItem.EditImage.ImageHDMimeType = meta.MimeType;
                    imageItem.EditImage.ImageHDFileSize = meta.FileSize;
                    imageItem.EditImage.ImageHDWidth = meta.Width;
                    imageItem.EditImage.ImageHDHeight = meta.Height;
                },
                fieldName: nameof(imageItem.ImageHDFormFile));

            if (!ok) return View(imageItem);
        }

        // 2. ImageFormFile — PNG 720px
        if (imageItem.ImageFormFile is not null)
        {
            bool ok = await SaveFileWithMetadata(
                imageItem.ImageFormFile,
                path,
                meta =>
                {
                    imageItem.EditImage.ImageFileName = meta.FileName;
                    imageItem.EditImage.ImageFileNameExtension = meta.Extension;
                    imageItem.EditImage.ImageMimeType = meta.MimeType;
                    imageItem.EditImage.ImageFileSize = meta.FileSize;
                    imageItem.EditImage.ImageWidth = meta.Width;
                    imageItem.EditImage.ImageHeight = meta.Height;
                },
                fieldName: nameof(imageItem.ImageFormFile));

            if (!ok) return View(imageItem);
        }

        // 3. IconFormFile — PNG 300px
        if (imageItem.IconFormFile is not null)
        {
            bool ok = await SaveFileWithMetadata(
                imageItem.IconFormFile,
                path,
                meta =>
                {
                    imageItem.EditImage.IconFileName = meta.FileName;
                    imageItem.EditImage.IconFileNameExtension = meta.Extension;
                    imageItem.EditImage.IconMimeType = meta.MimeType;
                    imageItem.EditImage.IconFileSize = meta.FileSize;
                    imageItem.EditImage.IconWidth = meta.Width;
                    imageItem.EditImage.IconHeight = meta.Height;
                },
                fieldName: nameof(imageItem.IconFormFile));

            if (!ok) return View(imageItem);
        }

        // 4. Icon200FormFile — PNG 200px
        if (imageItem.Icon200FormFile is not null)
        {
            bool ok = await SaveFileWithMetadata(
                imageItem.Icon200FormFile,
                path,
                meta =>
                {
                    imageItem.EditImage.Icon200FileName = meta.FileName;
                    imageItem.EditImage.Icon200FileNameExtension = meta.Extension;
                    imageItem.EditImage.Icon200MimeType = meta.MimeType;
                    imageItem.EditImage.Icon200FileSize = meta.FileSize;
                    imageItem.EditImage.Icon200Width = meta.Width;
                    imageItem.EditImage.Icon200Height = meta.Height;
                },
                fieldName: nameof(imageItem.Icon200FormFile));

            if (!ok) return View(imageItem);
        }

        // 5. Icon100FormFile — PNG 100px
        if (imageItem.Icon100FormFile is not null)
        {
            bool ok = await SaveFileWithMetadata(
                imageItem.Icon100FormFile,
                path,
                meta =>
                {
                    imageItem.EditImage.Icon100FileName = meta.FileName;
                    imageItem.EditImage.Icon100FileNameExtension = meta.Extension;
                    imageItem.EditImage.Icon100MimeType = meta.MimeType;
                    imageItem.EditImage.Icon100FileSize = meta.FileSize;
                    imageItem.EditImage.Icon100Width = meta.Width;
                    imageItem.EditImage.Icon100Height = meta.Height;
                },
                fieldName: nameof(imageItem.Icon100FormFile));

            if (!ok) return View(imageItem);
        }

        // 6. WebImageHDFormFile — WebP HD
        if (imageItem.WebImageHDFormFile is not null)
        {
            bool ok = await SaveFileWithMetadata(
                imageItem.WebImageHDFormFile,
                path,
                meta =>
                {
                    imageItem.EditImage.WebImageHDFileName = meta.FileName;
                    imageItem.EditImage.WebImageHDNameExtension = meta.Extension;
                    imageItem.EditImage.WebImageHDMimeType = meta.MimeType;
                    imageItem.EditImage.WebImageHDFileSize = meta.FileSize;
                    imageItem.EditImage.WebImageHDWidth = meta.Width;
                    imageItem.EditImage.WebImageHDHeight = meta.Height;
                },
                fieldName: nameof(imageItem.WebImageHDFormFile));

            if (!ok) return View(imageItem);
        }

        // 7. WebImageFormFile — WebP 720px
        if (imageItem.WebImageFormFile is not null)
        {
            bool ok = await SaveFileWithMetadata(
                imageItem.WebImageFormFile,
                path,
                meta =>
                {
                    imageItem.EditImage.WebImageFileName = meta.FileName;
                    imageItem.EditImage.WebImageFileNameExtension = meta.Extension;
                    imageItem.EditImage.WebImageMimeType = meta.MimeType;
                    imageItem.EditImage.WebImageFileSize = meta.FileSize;
                    imageItem.EditImage.WebImageWidth = meta.Width;
                    imageItem.EditImage.WebImageHeight = meta.Height;
                },
                fieldName: nameof(imageItem.WebImageFormFile));

            if (!ok) return View(imageItem);
        }

        // 8. WebIconFormFile — WebP 300px
        if (imageItem.WebIconFormFile is not null)
        {
            bool ok = await SaveFileWithMetadata(
                imageItem.WebIconFormFile,
                path,
                meta =>
                {
                    imageItem.EditImage.WebIconFileName = meta.FileName;
                    imageItem.EditImage.WebIconFileNameExtension = meta.Extension;
                    imageItem.EditImage.WebIconMimeType = meta.MimeType;
                    imageItem.EditImage.WebIconFileSize = meta.FileSize;
                    imageItem.EditImage.WebIconWidth = meta.Width;
                    imageItem.EditImage.WebIconHeight = meta.Height;
                },
                fieldName: nameof(imageItem.WebIconFormFile));

            if (!ok) return View(imageItem);
        }

        // 9. WebIcon200FormFile — WebP 200px
        if (imageItem.WebIcon200FormFile is not null)
        {
            bool ok = await SaveFileWithMetadata(
                imageItem.WebIcon200FormFile,
                path,
                meta =>
                {
                    imageItem.EditImage.WebIcon200FileName = meta.FileName;
                    imageItem.EditImage.WebIcon200FileNameExtension = meta.Extension;
                    imageItem.EditImage.WebIcon200MimeType = meta.MimeType;
                    imageItem.EditImage.WebIcon200FileSize = meta.FileSize;
                    imageItem.EditImage.WebIcon200Width = meta.Width;
                    imageItem.EditImage.WebIcon200Height = meta.Height;
                },
                fieldName: nameof(imageItem.WebIcon200FormFile));

            if (!ok) return View(imageItem);
        }

        // 10. WebIcon100FormFile — WebP 100px
        if (imageItem.WebIcon100FormFile is not null)
        {
            bool ok = await SaveFileWithMetadata(
                imageItem.WebIcon100FormFile,
                path,
                meta =>
                {
                    imageItem.EditImage.WebIcon100FileName = meta.FileName;
                    imageItem.EditImage.WebIcon100FileNameExtension = meta.Extension;
                    imageItem.EditImage.WebIcon100MimeType = meta.MimeType;
                    imageItem.EditImage.WebIcon100FileSize = meta.FileSize;
                    imageItem.EditImage.WebIcon100Width = meta.Width;
                    imageItem.EditImage.WebIcon100Height = meta.Height;
                },
                fieldName: nameof(imageItem.WebIcon100FormFile));

            if (!ok) return View(imageItem);
        }

        #endregion

        await imageContext.AddNewImageAsync(imageItem.EditImage);

        return RedirectToAction(nameof(DetailsImage), new { imageId = imageItem.EditImage.ImageFileModelId, Area = "Admin" });
    }

    #endregion

    #region Изменить данные картинки

    [HttpGet]
    public async Task<IActionResult> EditImage(Guid? imageId)
    {
        EditImageViewModel editImage = new();

        if (imageId.HasValue & await imageContext.ImageFiles.Where(i => i.ImageFileModelId == imageId).AnyAsync())
        {
            editImage.EditImage = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileModelId == imageId);

            return View(editImage);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditImage(EditImageViewModel imageItem)
    {
        if (!ModelState.IsValid)
        {
            return View(imageItem);
        }

        #region Заголовок картинки

        imageItem.EditImage.ImageCaption = imageItem.EditImage.ImageCaption?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(imageItem.EditImage.ImageCaption))
        {
            ModelState.AddModelError(nameof(imageItem.EditImage.ImageCaption), "Введите название картинки");

            return View(imageItem);
        }

        #endregion

        #region Ранняя проверка расширений «.webp»

        if (!ValidateWebpFiles(imageItem))
        {
            return View(imageItem);
        }

        #endregion

        #region Получаем запись и обновляем скалярные поля

        ImageFileModel imageUpdate = await imageContext.ImageFiles
            .FirstAsync(img => img.ImageFileModelId == imageItem.EditImage.ImageFileModelId);

        #region Заголовок картинки

        imageUpdate.ImageCaption = imageItem.EditImage.ImageCaption;

        #endregion

        #region Описание картинки

        imageUpdate.ImageDescription = imageItem.EditImage.ImageDescription.Trim();

        #endregion

        #region Теги alt и title

        imageUpdate.ImageAltTitle = imageItem.EditImage.ImageAltTitle.Trim();

        #endregion

        #region Фильтр поиска

        if (!string.IsNullOrEmpty(imageItem.EditImage.SearchFilter))
        {
            imageUpdate.SearchFilter = imageItem.EditImage.SearchFilter.Trim();
        }

        #endregion

        #region Индекс сортировки

        imageUpdate.SortOfPicture = imageItem.EditImage.SortOfPicture;

        #endregion

        #endregion

        #region Каталог картинки

        string oldPath = Path.GetFullPath(
                    Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", imageUpdate.ImagePath)
                            ).Replace('\\', '/');

        string imagePath = imageItem.EditImage.ImagePath?.TrimEnd('/') ?? string.Empty;

        imageUpdate.ImagePath = string.IsNullOrEmpty(imagePath) || imagePath == "/images"
            ? "/images"
            : $"{imagePath}";

        #region Ввод нового каталога

        if (!string.IsNullOrWhiteSpace(imageItem.NewImagePath))
        {
            imageUpdate.ImagePath = "/images/" + imageItem.NewImagePath.Trim('/');
        }

        #endregion

        string path = Path.GetFullPath(
                    Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", imageUpdate.ImagePath)
                            ).Replace('\\', '/');

        #endregion

        #region Перенос файлов в новый каталог

        if (oldPath != path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            void MoveIfExists(string? fileName)
            {
                if (string.IsNullOrEmpty(fileName)) return;

                FileInfo oldFile = new(Path.Join(oldPath, fileName));
                FileInfo newFile = new(Path.Join(path, fileName));

                if (oldFile.Exists)
                {
                    System.IO.File.Move(oldFile.FullName, newFile.FullName, overwrite: true);
                }
            }

            MoveIfExists(imageUpdate.ImageHDFileName);
            MoveIfExists(imageUpdate.ImageFileName);
            MoveIfExists(imageUpdate.IconFileName);
            MoveIfExists(imageUpdate.Icon200FileName);
            MoveIfExists(imageUpdate.Icon100FileName);
            MoveIfExists(imageUpdate.WebImageHDFileName);
            MoveIfExists(imageUpdate.WebImageFileName);
            MoveIfExists(imageUpdate.WebIconFileName);
            MoveIfExists(imageUpdate.WebIcon200FileName);
            MoveIfExists(imageUpdate.WebIcon100FileName);
        }

        #endregion

        #region Проверки дублей между IFormFile

        if (!ValidateUniqueFormFiles(imageItem))
        {
            return View(imageItem);
        }

        #endregion

        #region Проверки дублей в БД (исключая текущую запись)

        if (!await ValidateUniqueInDbAsync(imageItem, imageUpdate.ImageFileModelId))
        {
            return View(imageItem);
        }

        #endregion

        #region Очистка полей для незагруженных файлов

        ClearUnusedFormFields(imageItem);

        #endregion

        #region Запись новых файлов + обновление метаданных

        // 1. ImageHDFormFile — PNG HD
        if (imageItem.ImageHDFormFile is not null)
        {
            if (!CheckExtensionMatches(
                    imageItem.ImageHDFormFile,
                    imageItem.EditImage.ImageHDNameExtension,
                    nameof(imageItem.ImageHDFormFile)))
            {
                return View(imageItem);
            }

            BackupOldFile(path, imageUpdate.ImageHDFileName);

            bool ok = await SaveFileWithMetadata(
                imageItem.ImageHDFormFile,
                path,
                meta =>
                {
                    imageUpdate.ImageHDFileName = meta.FileName;
                    imageUpdate.ImageHDNameExtension = meta.Extension;
                    imageUpdate.ImageHDMimeType = meta.MimeType;
                    imageUpdate.ImageHDFileSize = meta.FileSize;
                    imageUpdate.ImageHDWidth = meta.Width;
                    imageUpdate.ImageHDHeight = meta.Height;
                },
                fieldName: nameof(imageItem.ImageHDFormFile));

            if (!ok) return View(imageItem);
        }

        // 2. ImageFormFile — PNG 720px
        if (imageItem.ImageFormFile is not null)
        {
            if (!CheckExtensionMatches(
                    imageItem.ImageFormFile,
                    imageItem.EditImage.ImageFileNameExtension,
                    nameof(imageItem.ImageFormFile)))
            {
                return View(imageItem);
            }

            BackupOldFile(path, imageUpdate.ImageFileName);

            bool ok = await SaveFileWithMetadata(
                imageItem.ImageFormFile,
                path,
                meta =>
                {
                    imageUpdate.ImageFileName = meta.FileName;
                    imageUpdate.ImageFileNameExtension = meta.Extension;
                    imageUpdate.ImageMimeType = meta.MimeType;
                    imageUpdate.ImageFileSize = meta.FileSize;
                    imageUpdate.ImageWidth = meta.Width;
                    imageUpdate.ImageHeight = meta.Height;
                },
                fieldName: nameof(imageItem.ImageFormFile));

            if (!ok) return View(imageItem);
        }

        // 3. IconFormFile — PNG 300px
        if (imageItem.IconFormFile is not null)
        {
            if (!CheckExtensionMatches(
                    imageItem.IconFormFile,
                    imageItem.EditImage.IconFileNameExtension,
                    nameof(imageItem.IconFormFile)))
            {
                return View(imageItem);
            }

            BackupOldFile(path, imageUpdate.IconFileName);

            bool ok = await SaveFileWithMetadata(
                imageItem.IconFormFile,
                path,
                meta =>
                {
                    imageUpdate.IconFileName = meta.FileName;
                    imageUpdate.IconFileNameExtension = meta.Extension;
                    imageUpdate.IconMimeType = meta.MimeType;
                    imageUpdate.IconFileSize = meta.FileSize;
                    imageUpdate.IconWidth = meta.Width;
                    imageUpdate.IconHeight = meta.Height;
                },
                fieldName: nameof(imageItem.IconFormFile));

            if (!ok) return View(imageItem);
        }

        // 4. Icon200FormFile — PNG 200px
        if (imageItem.Icon200FormFile is not null)
        {
            if (!CheckExtensionMatches(
                    imageItem.Icon200FormFile,
                    imageItem.EditImage.Icon200FileNameExtension,
                    nameof(imageItem.Icon200FormFile)))
            {
                return View(imageItem);
            }

            BackupOldFile(path, imageUpdate.Icon200FileName);

            bool ok = await SaveFileWithMetadata(
                imageItem.Icon200FormFile,
                path,
                meta =>
                {
                    imageUpdate.Icon200FileName = meta.FileName;
                    imageUpdate.Icon200FileNameExtension = meta.Extension;
                    imageUpdate.Icon200MimeType = meta.MimeType;
                    imageUpdate.Icon200FileSize = meta.FileSize;
                    imageUpdate.Icon200Width = meta.Width;
                    imageUpdate.Icon200Height = meta.Height;
                },
                fieldName: nameof(imageItem.Icon200FormFile));

            if (!ok) return View(imageItem);
        }

        // 5. Icon100FormFile — PNG 100px
        if (imageItem.Icon100FormFile is not null)
        {
            if (!CheckExtensionMatches(
                    imageItem.Icon100FormFile,
                    imageItem.EditImage.Icon100FileNameExtension,
                    nameof(imageItem.Icon100FormFile)))
            {
                return View(imageItem);
            }

            BackupOldFile(path, imageUpdate.Icon100FileName);

            bool ok = await SaveFileWithMetadata(
                imageItem.Icon100FormFile,
                path,
                meta =>
                {
                    imageUpdate.Icon100FileName = meta.FileName;
                    imageUpdate.Icon100FileNameExtension = meta.Extension;
                    imageUpdate.Icon100MimeType = meta.MimeType;
                    imageUpdate.Icon100FileSize = meta.FileSize;
                    imageUpdate.Icon100Width = meta.Width;
                    imageUpdate.Icon100Height = meta.Height;
                },
                fieldName: nameof(imageItem.Icon100FormFile));

            if (!ok) return View(imageItem);
        }

        // 6. WebImageHDFormFile — WebP HD
        if (imageItem.WebImageHDFormFile is not null)
        {
            if (!CheckExtensionMatches(
                    imageItem.WebImageHDFormFile,
                    imageItem.EditImage.WebImageHDNameExtension,
                    nameof(imageItem.WebImageHDFormFile)))
            {
                return View(imageItem);
            }

            BackupOldFile(path, imageUpdate.WebImageHDFileName);

            bool ok = await SaveFileWithMetadata(
                imageItem.WebImageHDFormFile,
                path,
                meta =>
                {
                    imageUpdate.WebImageHDFileName = meta.FileName;
                    imageUpdate.WebImageHDNameExtension = meta.Extension;
                    imageUpdate.WebImageHDMimeType = meta.MimeType;
                    imageUpdate.WebImageHDFileSize = meta.FileSize;
                    imageUpdate.WebImageHDWidth = meta.Width;
                    imageUpdate.WebImageHDHeight = meta.Height;
                },
                fieldName: nameof(imageItem.WebImageHDFormFile));

            if (!ok) return View(imageItem);
        }

        // 7. WebImageFormFile — WebP 720px
        if (imageItem.WebImageFormFile is not null)
        {
            if (!CheckExtensionMatches(
                    imageItem.WebImageFormFile,
                    imageItem.EditImage.WebImageFileNameExtension,
                    nameof(imageItem.WebImageFormFile)))
            {
                return View(imageItem);
            }

            BackupOldFile(path, imageUpdate.WebImageFileName);

            bool ok = await SaveFileWithMetadata(
                imageItem.WebImageFormFile,
                path,
                meta =>
                {
                    imageUpdate.WebImageFileName = meta.FileName;
                    imageUpdate.WebImageFileNameExtension = meta.Extension;
                    imageUpdate.WebImageMimeType = meta.MimeType;
                    imageUpdate.WebImageFileSize = meta.FileSize;
                    imageUpdate.WebImageWidth = meta.Width;
                    imageUpdate.WebImageHeight = meta.Height;
                },
                fieldName: nameof(imageItem.WebImageFormFile));

            if (!ok) return View(imageItem);
        }

        // 8. WebIconFormFile — WebP 300px
        if (imageItem.WebIconFormFile is not null)
        {
            if (!CheckExtensionMatches(
                    imageItem.WebIconFormFile,
                    imageItem.EditImage.WebIconFileNameExtension,
                    nameof(imageItem.WebIconFormFile)))
            {
                return View(imageItem);
            }

            BackupOldFile(path, imageUpdate.WebIconFileName);

            bool ok = await SaveFileWithMetadata(
                imageItem.WebIconFormFile,
                path,
                meta =>
                {
                    imageUpdate.WebIconFileName = meta.FileName;
                    imageUpdate.WebIconFileNameExtension = meta.Extension;
                    imageUpdate.WebIconMimeType = meta.MimeType;
                    imageUpdate.WebIconFileSize = meta.FileSize;
                    imageUpdate.WebIconWidth = meta.Width;
                    imageUpdate.WebIconHeight = meta.Height;
                },
                fieldName: nameof(imageItem.WebIconFormFile));

            if (!ok) return View(imageItem);
        }

        // 9. WebIcon200FormFile — WebP 200px
        if (imageItem.WebIcon200FormFile is not null)
        {
            if (!CheckExtensionMatches(
                    imageItem.WebIcon200FormFile,
                    imageItem.EditImage.WebIcon200FileNameExtension,
                    nameof(imageItem.WebIcon200FormFile)))
            {
                return View(imageItem);
            }

            BackupOldFile(path, imageUpdate.WebIcon200FileName);

            bool ok = await SaveFileWithMetadata(
                imageItem.WebIcon200FormFile,
                path,
                meta =>
                {
                    imageUpdate.WebIcon200FileName = meta.FileName;
                    imageUpdate.WebIcon200FileNameExtension = meta.Extension;
                    imageUpdate.WebIcon200MimeType = meta.MimeType;
                    imageUpdate.WebIcon200FileSize = meta.FileSize;
                    imageUpdate.WebIcon200Width = meta.Width;
                    imageUpdate.WebIcon200Height = meta.Height;
                },
                fieldName: nameof(imageItem.WebIcon200FormFile));

            if (!ok) return View(imageItem);
        }

        // 10. WebIcon100FormFile — WebP 100px
        if (imageItem.WebIcon100FormFile is not null)
        {
            if (!CheckExtensionMatches(
                    imageItem.WebIcon100FormFile,
                    imageItem.EditImage.WebIcon100FileNameExtension,
                    nameof(imageItem.WebIcon100FormFile)))
            {
                return View(imageItem);
            }

            BackupOldFile(path, imageUpdate.WebIcon100FileName);

            bool ok = await SaveFileWithMetadata(
                imageItem.WebIcon100FormFile,
                path,
                meta =>
                {
                    imageUpdate.WebIcon100FileName = meta.FileName;
                    imageUpdate.WebIcon100FileNameExtension = meta.Extension;
                    imageUpdate.WebIcon100MimeType = meta.MimeType;
                    imageUpdate.WebIcon100FileSize = meta.FileSize;
                    imageUpdate.WebIcon100Width = meta.Width;
                    imageUpdate.WebIcon100Height = meta.Height;
                },
                fieldName: nameof(imageItem.WebIcon100FormFile));

            if (!ok) return View(imageItem);
        }

        #endregion

        #region Удаление файлов (чекбоксы Delete*)

        if (imageItem.DeleteImageHD)
        {
            DeleteFileIfExists(path, imageItem.EditImage.ImageHDFileName);

            imageUpdate.ImageHDFileName = null;
            imageUpdate.ImageHDNameExtension = null;
            imageUpdate.ImageHDMimeType = null;
            imageUpdate.ImageHDFileSize = 0;
            imageUpdate.ImageHDWidth = 0;
            imageUpdate.ImageHDHeight = 0;
        }

        if (imageItem.DeleteWebImageHD)
        {
            DeleteFileIfExists(path, imageItem.EditImage.WebImageHDFileName);

            imageUpdate.WebImageHDFileName = null;
            imageUpdate.WebImageHDNameExtension = null;
            imageUpdate.WebImageHDMimeType = null;
            imageUpdate.WebImageHDFileSize = 0;
            imageUpdate.WebImageHDWidth = 0;
            imageUpdate.WebImageHDHeight = 0;
        }

        if (imageItem.DeleteImage)
        {
            DeleteFileIfExists(path, imageItem.EditImage.ImageFileName);

            imageUpdate.ImageFileName = string.Empty;
            imageUpdate.ImageFileNameExtension = string.Empty;
            imageUpdate.ImageMimeType = string.Empty;
            imageUpdate.ImageFileSize = 0;
            imageUpdate.ImageWidth = 0;
            imageUpdate.ImageHeight = 0;
        }

        if (imageItem.DeleteIcon)
        {
            DeleteFileIfExists(path, imageItem.EditImage.IconFileName);

            imageUpdate.IconFileName = null;
            imageUpdate.IconFileNameExtension = null;
            imageUpdate.IconMimeType = null;
            imageUpdate.IconFileSize = 0;
            imageUpdate.IconWidth = 0;
            imageUpdate.IconHeight = 0;
        }

        if (imageItem.DeleteIcon200)
        {
            DeleteFileIfExists(path, imageItem.EditImage.Icon200FileName);

            imageUpdate.Icon200FileName = null;
            imageUpdate.Icon200FileNameExtension = null;
            imageUpdate.Icon200MimeType = null;
            imageUpdate.Icon200FileSize = 0;
            imageUpdate.Icon200Width = 0;
            imageUpdate.Icon200Height = 0;
        }

        if (imageItem.DeleteIcon100)
        {
            DeleteFileIfExists(path, imageItem.EditImage.Icon100FileName);

            imageUpdate.Icon100FileName = null;
            imageUpdate.Icon100FileNameExtension = null;
            imageUpdate.Icon100MimeType = null;
            imageUpdate.Icon100FileSize = 0;
            imageUpdate.Icon100Width = 0;
            imageUpdate.Icon100Height = 0;
        }

        #endregion

        await imageContext.SaveChangesInImageAsync();

        return RedirectToAction(nameof(DetailsImage), new { imageId = imageUpdate.ImageFileModelId });
    }

    #endregion

    #region Удалить картинку

    [HttpGet]
    public async Task<IActionResult> DeleteImage(Guid? imageId)
    {
        ImageFileModel deleteImage = new();

        if (imageId.HasValue & await imageContext.ImageFiles.Where(i => i.ImageFileModelId == imageId).AnyAsync())
        {
            deleteImage = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileModelId == imageId);

            return View(deleteImage);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(ImageFileModel deleteImage)
    {
        if (deleteImage != null)
        {
            if (await bookContext.BooksAndArticles.Where(book => book.LogoOfArticleId == deleteImage.ImageFileModelId).AnyAsync())
            {
                ModelState.AddModelError("", "Ссылки на картинку в базе текстов сайта");

                return View(deleteImage);
            }

            if (await imageContext.ImageFiles.Where(i => i.ImageFileModelId == deleteImage.ImageFileModelId).AnyAsync())
            {
                await imageContext.DeleteImageAsync(deleteImage.ImageFileModelId);

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

    #region Вспомогательные функции

    /// <summary>
    /// Проверка расширения вводимых файлов
    /// </summary>
    private static bool HasExtension(IFormFile file, string extension)
    {
        return Path.GetExtension(file.FileName)
                   .Equals(extension, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Проверка всех вводимых файлов на расширение .webp
    /// </summary>
    private bool ValidateWebpFiles(EditImageViewModel imageItem)
    {
        bool ok = true;

        void Check(IFormFile? file, string fieldName)
        {
            if (file == null) return;

            if (!HasExtension(file, ".webp"))
            {
                ModelState.AddModelError(fieldName,
                    $"Файл «{file.FileName}» должен иметь расширение «.webp»");
                ok = false;
            }
        }

        Check(imageItem.WebImageHDFormFile, nameof(imageItem.WebImageHDFormFile));
        Check(imageItem.WebImageFormFile, nameof(imageItem.WebImageFormFile));
        Check(imageItem.WebIconFormFile, nameof(imageItem.WebIconFormFile));
        Check(imageItem.WebIcon200FormFile, nameof(imageItem.WebIcon200FormFile));
        Check(imageItem.WebIcon100FormFile, nameof(imageItem.WebIcon100FormFile));

        return ok;
    }

    /// <summary>
    /// Проверяет, что среди выбранных файлов нет двух с одинаковым именем.
    /// Работает для всех полей из UploadFields.
    /// </summary>
    private bool ValidateUniqueFormFiles(EditImageViewModel item)
    {
        var files = UploadFields
            .Select(f => (f.Name, File: f.Selector(item)))
            .Where(x => x.File is not null)
            .ToList();

        bool ok = true;

        for (int i = 0; i < files.Count; i++)
        {
            for (int j = i + 1; j < files.Count; j++)
            {
                if (string.Equals(files[i].File!.FileName, files[j].File!.FileName,
                          StringComparison.OrdinalIgnoreCase))
                {
                    string msg = $"Выбран один и тот же файл «{files[i].File!.FileName}» " +
                                 $"для полей {files[i].Name} и {files[j].Name}";

                    ModelState.AddModelError(files[i].Name, msg);
                    ModelState.AddModelError(files[j].Name, msg);

                    ok = false;
                }
            }
        }

        return ok;
    }

    /// <summary>
    /// Проверяет, что имена выбранных файлов ещё не встречаются в БД
    /// ни в одном из файловых полей ImageFileModel.
    /// </summary>
    private async Task<bool> ValidateUniqueInDbAsync(EditImageViewModel item, Guid? excludeId = null)
    {
        bool ok = true;

        foreach (var (name, selector) in UploadFields)
        {
            IFormFile? file = selector(item);

            if (file is null) continue;

            string fileName = file.FileName;

            bool exists = await imageContext.ImageFiles.AnyAsync(x =>
                (x.ImageFileName == fileName ||
                 x.ImageHDFileName == fileName ||
                 x.IconFileName == fileName ||
                 x.Icon200FileName == fileName ||
                 x.Icon100FileName == fileName ||
                 x.WebImageFileName == fileName ||
                 x.WebImageHDFileName == fileName ||
                 x.WebIconFileName == fileName ||
                 x.WebIcon200FileName == fileName ||
                 x.WebIcon100FileName == fileName)
                && (!excludeId.HasValue || x.ImageFileModelId != excludeId.Value));

            if (exists)
            {
                ModelState.AddModelError(name, $"Файл «{fileName}» уже есть в базе данных");
                ok = false;
            }
        }

        return ok;
    }

    /// <summary>
    /// Очистка полей для незагруженных IFormFile
    /// </summary>
    private static void ClearUnusedFormFields(EditImageViewModel item)
    {
        if (item.ImageHDFormFile is null)
        {
            item.EditImage.ImageHDFileName = string.Empty;
            item.EditImage.ImageHDNameExtension = string.Empty;
            item.EditImage.ImageHDMimeType = string.Empty;
            item.EditImage.ImageHDFileSize = 0;
            item.EditImage.ImageHDWidth = 0;
            item.EditImage.ImageHDHeight = 0;
        }

        if (item.ImageFormFile is null)
        {
            item.EditImage.ImageFileName = string.Empty;
            item.EditImage.ImageFileNameExtension = string.Empty;
            item.EditImage.ImageMimeType = string.Empty;
            item.EditImage.ImageFileSize = 0;
            item.EditImage.ImageWidth = 0;
            item.EditImage.ImageHeight = 0;
        }

        if (item.IconFormFile is null)
        {
            item.EditImage.IconFileName = string.Empty;
            item.EditImage.IconFileNameExtension = string.Empty;
            item.EditImage.IconMimeType = string.Empty;
            item.EditImage.IconFileSize = 0;
            item.EditImage.IconWidth = 0;
            item.EditImage.IconHeight = 0;
        }

        if (item.Icon200FormFile is null)
        {
            item.EditImage.Icon200FileName = string.Empty;
            item.EditImage.Icon200FileNameExtension = string.Empty;
            item.EditImage.Icon200MimeType = string.Empty;
            item.EditImage.Icon200FileSize = 0;
            item.EditImage.Icon200Width = 0;
            item.EditImage.Icon200Height = 0;
        }

        if (item.Icon100FormFile is null)
        {
            item.EditImage.Icon100FileName = string.Empty;
            item.EditImage.Icon100FileNameExtension = string.Empty;
            item.EditImage.Icon100MimeType = string.Empty;
            item.EditImage.Icon100FileSize = 0;
            item.EditImage.Icon100Width = 0;
            item.EditImage.Icon100Height = 0;
        }

        if (item.WebImageHDFormFile is null)
        {
            item.EditImage.WebImageHDFileName = string.Empty;
            item.EditImage.WebImageHDNameExtension = string.Empty;
            item.EditImage.WebImageHDMimeType = string.Empty;
            item.EditImage.WebImageHDFileSize = 0;
            item.EditImage.WebImageHDWidth = 0;
            item.EditImage.WebImageHDHeight = 0;
        }

        if (item.WebImageFormFile is null)
        {
            item.EditImage.WebImageFileName = string.Empty;
            item.EditImage.WebImageFileNameExtension = string.Empty;
            item.EditImage.WebImageMimeType = string.Empty;
            item.EditImage.WebImageFileSize = 0;
            item.EditImage.WebImageWidth = 0;
            item.EditImage.WebImageHeight = 0;
        }

        if (item.WebIconFormFile is null)
        {
            item.EditImage.WebIconFileName = string.Empty;
            item.EditImage.WebIconFileNameExtension = string.Empty;
            item.EditImage.WebIconMimeType = string.Empty;
            item.EditImage.WebIconFileSize = 0;
            item.EditImage.WebIconWidth = 0;
            item.EditImage.WebIconHeight = 0;
        }

        if (item.WebIcon200FormFile is null)
        {
            item.EditImage.WebIcon200FileName = string.Empty;
            item.EditImage.WebIcon200FileNameExtension = string.Empty;
            item.EditImage.WebIcon200MimeType = string.Empty;
            item.EditImage.WebIcon200FileSize = 0;
            item.EditImage.WebIcon200Width = 0;
            item.EditImage.WebIcon200Height = 0;
        }

        if (item.WebIcon100FormFile is null)
        {
            item.EditImage.WebIcon100FileName = string.Empty;
            item.EditImage.WebIcon100FileNameExtension = string.Empty;
            item.EditImage.WebIcon100MimeType = string.Empty;
            item.EditImage.WebIcon100FileSize = 0;
            item.EditImage.WebIcon100Width = 0;
            item.EditImage.WebIcon100Height = 0;
        }
    }

    /// <summary>
    /// Копирует IFormFile в целевой каталог (если ещё не скопирован),
    /// читает метаданные и передаёт их в callback для заполнения свойств модели.
    /// При ошибке чтения метаданных добавляет ModelState.AddModelError
    /// и возвращает false.
    /// </summary>
    private async Task<bool> SaveFileWithMetadata(
        IFormFile file,
        string path,
        Action<ImageMetadata> assign,
        string fieldName)
    {
        // Проверяем файл на «0»
        if (file.Length == 0)
        {
            ModelState.AddModelError(fieldName, $"Файл «{file.FileName}» пуст");

            return false;
        }

        // 1. Копируем файл, если его ещё нет в целевом каталоге
        string targetPath = Path.Join(path, file.FileName);

        FileInfo target = new(targetPath);

        bool wasExisting = target.Exists;

        if (wasExisting)
        {
            BackupOldFile(path, file.FileName);  // переместит в %TEMP%/ShevkunenkoSite_Backups
        }

        using FileStream stream = new(targetPath, FileMode.Create);

        await file.CopyToAsync(stream);

        // 2. Читаем метаданные
        IReadOnlyList<MetadataExtractor.Directory> directories;
        try
        {
            directories = ImageMetadataReader.ReadMetadata(targetPath);
        }
        catch (Exception ex)
        {
            if (!wasExisting)
            {
                try { System.IO.File.Delete(targetPath); } catch { /* ignore */ }
            }

            ModelState.AddModelError(fieldName, $"Не удалось прочитать метаданные файла «{file.FileName}»: {ex.Message}");

            return false;
        }

        string? fileName = null;
        string? extension = null;
        string? mimeType = null;
        uint? fileSize = null;
        uint? width = null;
        uint? height = null;

        foreach (var directory in directories)
        {
            foreach (var tag in directory.Tags)
            {
                switch (tag.Name)
                {
                    case "File Name":
                        fileName = tag.Description;
                        break;

                    case "Expected File Name Extension":
                        extension = tag.Description;
                        break;

                    case "Detected MIME Type":
                        mimeType = tag.Description;
                        break;

                    case "File Size":
                        fileSize = ParseFileSize(tag.Description);
                        break;

                    case "Image Width":
                        width = ParseUInt(tag.Description);
                        break;

                    case "Image Height":
                        height = ParseUInt(tag.Description);
                        break;
                }
            }
        }

        // 3. Проверяем, что все метаданные определены
        if (string.IsNullOrWhiteSpace(fileName))
        {
            ModelState.AddModelError(fieldName,
                $"Не удалось определить имя файла «{file.FileName}»");
            return false;
        }

        if (string.IsNullOrWhiteSpace(extension))
        {
            ModelState.AddModelError(fieldName,
                $"Не удалось определить расширение файла «{file.FileName}»");
            return false;
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            ModelState.AddModelError(fieldName,
                $"Не удалось определить MIME файла «{file.FileName}»");
            return false;
        }

        if (fileSize is null or 0)
        {
            ModelState.AddModelError(fieldName,
                $"Не удалось определить размер файла «{file.FileName}»");
            return false;
        }

        if (width is null or 0)
        {
            ModelState.AddModelError(fieldName,
                $"Не удалось определить ширину файла «{file.FileName}»");
            return false;
        }

        if (height is null or 0)
        {
            ModelState.AddModelError(fieldName,
                $"Не удалось определить высоту файла «{file.FileName}»");
            return false;
        }

        // 4. Отдаём метаданные в callback
        assign(new ImageMetadata(
            FileName: fileName,
            Extension: extension,
            MimeType: mimeType,
            FileSize: fileSize.Value,
            Width: width.Value,
            Height: height.Value));

        return true;
    }

    /// <summary>
    /// Функция для метаданных
    /// </summary>
    private static uint? ParseUInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        return uint.TryParse(value.Trim(), out uint result) ? result : null;
    }

    /// <summary>
    /// Функция для метаданных
    /// </summary>
    private static uint? ParseFileSize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        // "12345 bytes" → берём первое слово до пробела
        int spaceIndex = value.IndexOf(' ');
        string numberPart = spaceIndex > 0 ? value[..spaceIndex] : value;

        return uint.TryParse(numberPart.Trim(), out uint result) ? result : null;
    }

    /// <summary>
    /// Перемещает старый файл в папку бэкапа. Имя нового файла содержит
    /// исходное имя, суффикс "-save-" и метку времени в формате dd-MM-yyyy-HH-mm-ss.
    /// Если файла нет — ничего не делает.
    /// </summary>
    private static void BackupOldFile(string path, string? oldFileName)
    {
        if (string.IsNullOrEmpty(oldFileName)) return;

        FileInfo oldFile = new(Path.Join(path, oldFileName));
        if (!oldFile.Exists) return;

        string backupDir = Path.Join(Path.GetTempPath(), "ShevkunenkoSite_Backups");
        if (!System.IO.Directory.Exists(backupDir))
        {
            System.IO.Directory.CreateDirectory(backupDir);
        }

        string baseName = Path.GetFileNameWithoutExtension(oldFileName);
        string extension = Path.GetExtension(oldFileName);

        string backupName = $"{baseName}-save-{DateTime.Now:dd-MM-yyyy-HH-mm-ss}{extension}";

        System.IO.File.Move(oldFile.FullName, Path.Join(backupDir, backupName), overwrite: true);
    }

    /// <summary>
    /// Проверяет, что расширение загружаемого файла совпадает с ранее сохранённым.
    /// Если ожидаемое расширение пустое — проверка не требуется (первая загрузка).
    /// </summary>
    private bool CheckExtensionMatches(
        IFormFile file,
        string? expectedExtension,
        string fieldName)
    {
        if (string.IsNullOrEmpty(expectedExtension)) return true;

        string actualExt = Path.GetExtension(file.FileName).TrimStart('.');

        if (!actualExt.Equals(expectedExtension, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(fieldName,
                $"Расширение файла должно быть «.{expectedExtension}»");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Удалить файл если существует
    /// </summary>
    private static void DeleteFileIfExists(string path, string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;

        FileInfo file = new(Path.Join(path, fileName));

        if (file.Exists)
        {
            file.Delete();
        }
    }

    /// <summary>
    /// Если файл существует в каталоге fullPath — ничего не делай; если его нет — выполнить onMissing 
    /// </summary>
    private static void ApplyIfFileMissing(string? fileName, string directory, Action onMissing)
    {
        string? safeName = Path.GetFileName(fileName);

        if (string.IsNullOrWhiteSpace(safeName)) return;

        string fullPath = Path.Join(directory, safeName);

        if (!System.IO.File.Exists(fullPath)) onMissing();
    }

    #endregion
}