namespace ShevkunenkoSite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class IconController(
    IIconRepository iconContext,
    IIconTypeRepository iconTypeContext,
    IWebHostEnvironment hostEnvironment
    ) : Controller
{
    private readonly string rootPath = hostEnvironment.WebRootPath;

    #region Список иконок сайта

    [HttpGet]
    public async Task<IActionResult> Index
        (
        Guid? iconId
        )
    {
        if (iconId == null)
        {
            return RedirectToAction("Index", "IconType", new { area = "Admin" });
        }
        else if (await iconTypeContext.IconTypes.Where(iconType => iconType.IconTypeModelId == iconId).AnyAsync())
        {
            var listOfIcons = await iconContext.Icons
                .Where(icon => icon.IconTypeModelId == iconId)
                .OrderBy(icon => icon.IconFileName)
                .ToArrayAsync();

            return View(listOfIcons);
        }
        else if (await iconContext.Icons.Where(icon => icon.IconModelId == iconId).AnyAsync())
        {
            var currentIcon = await iconContext.Icons.FirstAsync(icon => icon.IconModelId == iconId);

            var listOfIcons = await iconContext.Icons
                .Where(icon => icon.IconTypeModelId == currentIcon.IconTypeModelId)
                .OrderBy(icon => icon.IconFileName)
                .ToArrayAsync();

            return View(listOfIcons);
        }
        else
        {
            return RedirectToAction("Index", "IconType", new { area = "Admin" });
        }
    }

    #endregion

    #region Добавить новый размер к существующему типу иконки

    [HttpGet]
    public async Task<IActionResult> AddIcon(Guid iconId)
    {

        if (await iconContext.Icons.Where(icon => icon.IconTypeModelId == iconId).AnyAsync())
        {
            IconDTOModel addIconDTO = new()
            {
                IconTypeModelId = iconId
            };

            return View(addIconDTO);
        }
        else
        {
            return RedirectToAction("Index", "IconType", new { area = "Admin" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(5_268_435_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
    public async Task<IActionResult> AddIcon(
    [Bind(
                "IconTypeModelId," +
                "RelForIcon," +
                "IconPurpose," +
                "IconFileFormFile"
        )]
        IconDTOModel addIconDTO)
    {
        if (ModelState.IsValid)
        {
            #region Добавить иконку

            if (addIconDTO.IconFileFormFile == null)
            {
                ModelState.AddModelError("IconFileFormFile", "Выберите файл иконки");

                return View(addIconDTO);
            }

            IconModel addIcon = new()
            {
                IconTypeModelId = addIconDTO.IconTypeModelId,
                RelForIcon = addIconDTO.RelForIcon,
                IconPurpose = addIconDTO.IconPurpose
            };

            if (await iconTypeContext.IconTypes.Where(iconType => iconType.IconTypeModelId == addIconDTO.IconTypeModelId).AnyAsync())
            {
                IconTypeModel typeOfIcon = await iconTypeContext.IconTypes.FirstAsync(iconType => iconType.IconTypeModelId == addIconDTO.IconTypeModelId);

                addIcon.IconType = typeOfIcon;
            }

            if (addIconDTO.IconFileFormFile.FileName.Contains(".svg"))
            {
                #region Копируем файл в папку Temp

                string iconTempPath = Path.Combine(rootPath + DataConfig.TempPath, addIconDTO.IconFileFormFile.FileName).Replace('\\', '/');

                FileInfo iconFile = new(iconTempPath);

                if (!iconFile.Exists)
                {
                    using FileStream stream = new(iconTempPath, FileMode.Create);

                    await addIconDTO.IconFileFormFile.CopyToAsync(stream);
                }

                #endregion

                addIcon.IconFileName = addIconDTO.IconFileFormFile.FileName;

                addIcon.IconMimeType = "image/svg+xml";

                addIcon.IconSize = "any";

                addIcon.RelForIcon = "mask-icon";

                addIcon.IconPurpose = "maskable";

                #region Копируем файл в папку иконок и удаляем из папки Temp

                string iconPath = Path.Combine(rootPath + DataConfig.IconsFolder + addIcon.IconType.PathToIcon, addIconDTO.IconFileFormFile.FileName).Replace('\\', '/');

                FileInfo iconFileInfo = new(iconPath);

                if (!iconFileInfo.Exists)
                {
                    using FileStream stream = new(iconPath, FileMode.Create);

                    await addIconDTO.IconFileFormFile.CopyToAsync(stream);
                }

                FileInfo tempFile = new(iconTempPath);

                if (tempFile.Exists)
                {
                    tempFile.Delete();
                }

                #endregion
            }
            else
            {
                #region Копируем файл в папку Temp

                string iconTempPath = Path.Combine(rootPath + DataConfig.TempPath, addIconDTO.IconFileFormFile.FileName).Replace('\\', '/');

                FileInfo iconFile = new(iconTempPath);

                if (!iconFile.Exists)
                {
                    using FileStream stream = new(iconTempPath, FileMode.Create);

                    await addIconDTO.IconFileFormFile.CopyToAsync(stream);
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
                                ModelState.AddModelError("IconFileFormFile", $"Не определить имя файла «{addIconDTO.IconFileFormFile.FileName}»");

                                #region Удаляем файл из папки Temp

                                FileInfo fileInfo = new(iconTempPath);

                                if (fileInfo.Exists)
                                {
                                    fileInfo.Delete();
                                }

                                #endregion

                                return View(addIconDTO);
                            }

                            if (await iconContext.Icons.Where(icon => icon.IconFileName == addIconDTO.IconFileFormFile.FileName & icon.IconTypeModelId == addIcon.IconTypeModelId).AnyAsync())
                            {
                                ModelState.AddModelError("IconFileFormFile", $"Файл «{addIconDTO.IconFileFormFile.FileName}» существует в каталоге «{addIcon.IconType.PathToIcon}»");

                                #region Удаляем файл из папки Temp

                                FileInfo fileInfo = new(iconTempPath);

                                if (fileInfo.Exists)
                                {
                                    fileInfo.Delete();
                                }

                                #endregion

                                return View(addIconDTO);
                            }

                            addIcon.IconFileName = tag.Description;
                        }

                        #endregion

                        #region Определяем MIME Type

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFileFormFile", $"Не определить MIME файла «{addIconDTO.IconFileFormFile.FileName}»");

                                #region Удаляем созданный каталог и файл из папки Temp

                                FileInfo fileInf = new(iconTempPath);

                                if (fileInf.Exists)
                                {
                                    fileInf.Delete();
                                }

                                #endregion

                                return View(addIconDTO);
                            }

                            addIcon.IconMimeType = tag.Description;
                        }

                        #endregion

                        #region Определяем ширину файла

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFileFormFile", $"Не определить ширину файла «{addIconDTO.IconFileFormFile.FileName}»");

                                #region Удаляем файл из папки Temp

                                FileInfo fileInf = new(iconTempPath);

                                if (fileInf.Exists)
                                {
                                    fileInf.Delete();
                                }

                                #endregion

                                return View(addIconDTO);
                            }

                            addIcon.IconWidth = tag.Description;
                        }

                        #endregion

                        #region Определяем высоту файла

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFileFormFile", $"Не определить высоту файла «{addIconDTO.IconFileFormFile.FileName}»");

                                #region Удаляем файл из папки Temp

                                FileInfo fileInf = new(iconTempPath);

                                if (fileInf.Exists)
                                {
                                    fileInf.Delete();
                                }

                                #endregion

                                return View(addIconDTO);
                            }

                            addIcon.IconHeight = tag.Description;
                        }

                        #endregion
                    }
                }

                #endregion

                #region Параметр IconSize

                if (addIcon.IconWidth != addIcon.IconHeight & addIcon.IconWidth != "580" & addIcon.IconHeight != "270")
                {
                    ModelState.AddModelError("IconFileFormFile", $"Ширина «{addIcon.IconWidth}» и высота «{addIcon.IconHeight}» иконки должны быть равны");

                    return View(addIconDTO);
                }
                else if (addIcon.IconFileName.Contains("favicon"))
                {
                    addIcon.IconSize = "any";
                }
                else
                {
                    addIcon.IconSize = addIcon.IconWidth + 'x' + addIcon.IconHeight;
                }

                #endregion

                #region Параметр RelForIcon

                _ = addIcon.RelForIcon;

                if (addIcon.IconFileName.Contains("maskable"))
                {
                    addIcon.RelForIcon = "mask-icon";
                }

                #endregion

                #region Параметр IconPurpose

                _ = addIcon.IconPurpose;

                if (addIcon.IconFileName.Contains("maskable"))
                {
                    addIcon.IconPurpose = "maskable";
                }

                #endregion

                #region Копируем файл в папку иконок и удаляем из папки Temp

                string iconPath = Path.Combine(rootPath + DataConfig.IconsFolder + addIcon.IconType.PathToIcon, addIconDTO.IconFileFormFile.FileName).Replace('\\', '/');

                FileInfo iconFileInfo = new(iconPath);

                if (!iconFileInfo.Exists)
                {
                    using FileStream stream = new(iconPath, FileMode.Create);

                    await addIconDTO.IconFileFormFile.CopyToAsync(stream);
                }

                FileInfo tempFile = new(iconTempPath);

                if (tempFile.Exists)
                {
                    tempFile.Delete();
                }

                #endregion
            }

            #endregion

            #region Сохранить в базе данных

            await iconContext.AddNewIconAsync(addIcon);

            #endregion

            #region Открытие страницы Index

            var newIcon = await iconContext.Icons.FirstAsync(icon => icon.IconFileName == addIcon.IconFileName & icon.IconTypeModelId == addIcon.IconTypeModelId);

            return RedirectToAction("Index", new { iconId = newIcon.IconModelId });

            #endregion
        }
        else
        {
            return View(addIconDTO);
        }
    }

    #endregion

    #region Изменить параметры иконки

    [HttpGet]
    public async Task<IActionResult> EditIcon(Guid? iconId)
    {
        IconModel editIcon = new();

        if (iconId.HasValue
            && await iconContext.Icons
                .Where(icon => icon.IconModelId == iconId)
                .AnyAsync())
        {
            #region Инициализация экземпляра иконки

            editIcon = await iconContext.Icons
                .FirstAsync(icon => icon.IconModelId == iconId);

            #endregion

            return View(editIcon);
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
    public async Task<IActionResult> EditIcon(
    [Bind(
            "IconModelId," +
            "IconTypeModelId," +
            "RelForIcon," +
            "IconPurpose"
    )]
    IconEditDTOModel editIcon)
    {
        if (ModelState.IsValid)
        {
            #region Инициализация экземпляра иконки

            var iconUpdate = await iconContext.Icons
                .FirstAsync(icon => icon.IconModelId == editIcon.IconModelId);

            #endregion

            #region Параметр RelForIcon

            iconUpdate.RelForIcon = editIcon.RelForIcon;

            #endregion

            #region Параметр IconPurpose

            iconUpdate.IconPurpose = editIcon.IconPurpose;

            #endregion

            #region Сохранить изменения

            await iconContext.SaveChangesInIconsAsync();

            #endregion

            #region Открытие страницы Index

            return RedirectToAction("Index", new { iconId = iconUpdate.IconModelId });

            #endregion
        }
        else
        {
            return RedirectToAction("EditIccon", "Icon", new { iconId = editIcon.IconModelId });
        }
    }

    #endregion

    #region Удалить иконку

    [HttpGet]
    public async Task<IActionResult> DeleteIcon(Guid? iconId)
    {
        IconModel deleteIcon = new();

        if (iconId.HasValue
            && await iconContext.Icons
                .Where(icon => icon.IconModelId == iconId)
                .AnyAsync())
        {
            #region Инициализация экземпляра иконки

            deleteIcon = await iconContext.Icons
                .FirstAsync(icon => icon.IconModelId == iconId);

            #endregion

            return View(deleteIcon);
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
    public async Task<IActionResult> DeleteIcon(
    [Bind(
            "IconModelId," +
            "IconTypeModelId"
        )]
        IconDeleteDTOModel deleteIcon)
    {
        if (ModelState.IsValid)
        {
            #region Инициализация экземпляра иконки

            var iconDelete = await iconContext.Icons
                .FirstAsync(icon => icon.IconModelId == deleteIcon.IconModelId);

            #endregion

            #region Перемещаем файл иконки в папку Temp

            string iconPath = Path.Combine(rootPath + DataConfig.IconsFolder + iconDelete.IconType.PathToIcon, iconDelete.IconFileName).Replace('\\', '/');

            string iconTempPath = Path.Combine(rootPath + DataConfig.TempPath, iconDelete.IconFileName).Replace('\\', '/');

            FileInfo iconFile = new(iconPath);

            if (iconFile.Exists)
            {
                iconFile.MoveTo(iconTempPath);
            }

            #endregion

            #region Удалить иконку

            await iconContext.DeleteIconAsync(iconDelete.IconModelId);

            #endregion

            #region Открытие страницы Index

            return RedirectToAction("Index", new { iconId = iconDelete.IconTypeModelId });

            #endregion
        }
        else
        {
            return RedirectToAction("Index", "Icon", new { iconId = deleteIcon.IconTypeModelId });
        }
    }

    #endregion
}