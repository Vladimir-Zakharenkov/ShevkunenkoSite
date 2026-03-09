namespace ShevkunenkoSite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class IconTypeController(
    IIconTypeRepository iconTypeContext,
    IIconRepository iconContext,
    IWebHostEnvironment hostEnvironment
    ) : Controller
{
    private readonly string rootPath = hostEnvironment.WebRootPath;

    #region Список иконок

    [HttpGet]
    public async Task<ViewResult> Index()
    {
        List<IconModel> iconList = [];

        var typeOfIcons = await iconTypeContext.IconTypes.ToListAsync();

        foreach (var iconType in typeOfIcons ?? Enumerable.Empty<IconTypeModel>())
        {
            var icon = await iconContext.Icons.FirstAsync(icon => icon.IconTypeModelId == iconType.IconTypeModelId && icon.IconSize == "558x558");

            if (icon != null)
            {
                iconList.Add(icon);
            }
        }

        return View(iconList);
    }

    #endregion

    #region Добавить иконку

    [HttpGet]
    public ActionResult AddIconType()
    {
        IconTypeModel addIconType = new();

        return View(addIconType);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(5_268_435_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
    public async Task<IActionResult> AddIconType(
    [Bind(
                "IconTypeModelId," +
                "PathToIcon," +
                "IconTypeDescription," +
                "IconFileFormFile"
        )]
        IconTypeModel addIconType)
    {
        IconModel newIcon = new();

        if (ModelState.IsValid)
        {
            if (addIconType.IconFileFormFile == null)
            {
                ModelState.AddModelError("IconFileFormFile", "Выберите файл иконки");

                return View(addIconType);
            }
            else
            {
                #region Копируем файл в папку Temp

                string iconTempPath = Path.Combine(rootPath + DataConfig.TempPath, addIconType.IconFileFormFile.FileName).Replace('\\', '/');

                FileInfo iconFile = new(iconTempPath);

                if (!iconFile.Exists)
                {
                    using FileStream stream = new(iconTempPath, FileMode.Create);

                    await addIconType.IconFileFormFile.CopyToAsync(stream);
                }

                #endregion

                #region Определение параметров выбранного файла 

                IReadOnlyList<MetadataExtractor.Directory> iconProperties = ImageMetadataReader.ReadMetadata(iconTempPath);

                foreach (var iconProperty in iconProperties)
                {
                    foreach (var tag in iconProperty.Tags)
                    {
                        #region Определяем FileName

                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFileFormFile", $"Не определить имя файла «{addIconType.IconFileFormFile.FileName}»");

                                #region Удаляем файл из папки Temp

                                FileInfo fileInfo = new(iconTempPath);

                                if (fileInfo.Exists)
                                {
                                    fileInfo.Delete();
                                }

                                #endregion

                                return View(addIconType);
                            }

                            if (!tag.Description.Contains("ms-tile-558."))
                            {
                                ModelState.AddModelError("IconFileFormFile", $"Имя файла должно содержать «ms-tile-558.»");

                                #region Удаляем файл из папки Temp

                                FileInfo fileInfo = new(iconTempPath);

                                if (fileInfo.Exists)
                                {
                                    fileInfo.Delete();
                                }

                                #endregion

                                return View(addIconType);
                            }

                            newIcon.IconFileName = tag.Description;
                        }

                        #endregion

                        #region Определяем MIME Type

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFileFormFile", $"Не определить MIME файла «{addIconType.IconFileFormFile.FileName}»");

                                #region Удаляем файл из папки Temp

                                FileInfo fileInf = new(iconTempPath);

                                if (fileInf.Exists)
                                {
                                    fileInf.Delete();
                                }

                                #endregion

                                return View(addIconType);
                            }

                            newIcon.IconMimeType = tag.Description;
                        }

                        #endregion

                        #region Определяем ширину файла

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFileFormFile", $"Не определить ширину файла «{addIconType.IconFileFormFile.FileName}»");

                                #region Удаляем файл из папки Temp

                                FileInfo fileInf = new(iconTempPath);

                                if (fileInf.Exists)
                                {
                                    fileInf.Delete();
                                }

                                #endregion

                                return View(addIconType);
                            }

                            if (tag.Description != "558")
                            {
                                ModelState.AddModelError("IconFileFormFile", $"Ширина файла должна быть «558 px»");

                                #region Удаляем файл из папки Temp

                                FileInfo fileInf = new(iconTempPath);

                                if (fileInf.Exists)
                                {
                                    fileInf.Delete();
                                }

                                #endregion

                                return View(addIconType);
                            }

                            newIcon.IconSize = tag.Description + 'x';
                        }

                        #endregion

                        #region Определяем высоту файла

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFileFormFile", $"Не определить высоту файла «{addIconType.IconFileFormFile.FileName}»");

                                #region Удаляем файл из папки Temp

                                FileInfo fileInf = new(iconTempPath);

                                if (fileInf.Exists)
                                {
                                    fileInf.Delete();
                                }

                                #endregion

                                return View(addIconType);
                            }

                            if (tag.Description != "558")
                            {
                                ModelState.AddModelError("IconFileFormFile", "Высота файла  должна быть «558 px»");

                                #region Удаляем файл из папки Temp

                                FileInfo fileInf = new(iconTempPath);

                                if (fileInf.Exists)
                                {
                                    fileInf.Delete();
                                }

                                #endregion

                                return View(addIconType);
                            }

                            newIcon.IconSize += tag.Description;
                        }

                        #endregion
                    }
                }

                newIcon.RelForIcon = "icon";

                newIcon.IconPurpose = "any";

                #endregion

                #region Описание иконки

                _ = addIconType.IconTypeDescription.Trim();

                #endregion

                #region Каталог иконки

                addIconType.PathToIcon = addIconType.PathToIcon.Trim('/').Trim('\\') + '/';

                if (await iconTypeContext.IconTypes.Where(iconType => iconType.PathToIcon == addIconType.PathToIcon).AnyAsync())
                {
                    ModelState.AddModelError("PathToIcon", $"Каталог {addIconType.PathToIcon} уже существует");

                    return View(addIconType);
                }
                else
                {
                    string pathToNewIcon = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), DataConfig.IconFoldersPath, addIconType.PathToIcon)).Replace('\\', '/');

                    if (!System.IO.Directory.Exists(pathToNewIcon))
                    {
                        System.IO.Directory.CreateDirectory(pathToNewIcon);
                    }

                    await iconTypeContext.AddNewIconTypeAsync(addIconType);
                }

                #endregion

                #region Сохраняем выбранную иконку в базе данных

                var newIconType = await iconTypeContext.IconTypes.FirstAsync(iconType => iconType.PathToIcon == addIconType.PathToIcon);

                newIcon.IconTypeModelId = newIconType.IconTypeModelId;

                await iconContext.AddNewIconAsync(newIcon);

                #endregion

                #region Копируем иконку в новую папку и удаляем из папки Temp

                string iconPath = Path.Combine(rootPath + DataConfig.IconsFolder + newIconType.PathToIcon, addIconType.IconFileFormFile.FileName).Replace('\\', '/');

                FileInfo iconFileInfo = new(iconPath);

                if (!iconFileInfo.Exists)
                {
                    using FileStream stream = new(iconPath, FileMode.Create);

                    await addIconType.IconFileFormFile.CopyToAsync(stream);
                }

                FileInfo tempFile = new(iconTempPath);

                if (tempFile.Exists)
                {
                    tempFile.Delete();
                }

                #endregion

                return RedirectToAction("Index", "IconType", new { area = "Admin" });
            }
        }
        else
        {
            return View(addIconType);
        }
    }

    #endregion
}