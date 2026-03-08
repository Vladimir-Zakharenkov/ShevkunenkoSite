using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

namespace ShevkunenkoSite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class IconController(
    IIconRepository iconContext,
    IWebHostEnvironment hostEnvironment
    ) : Controller
{
    private readonly string rootPath = hostEnvironment.WebRootPath;

    #region Список иконок сайта

    [HttpGet]
    public async Task<IActionResult> Index
        (
        string? iconPath,
        Guid? iconId
        )
    {
        if (string.IsNullOrEmpty(iconPath))
        {
            var typesOfIcons = iconContext.Icons
                .Where(icon => icon.IconFileName.Contains("ms-tile-558."));

            return View(typesOfIcons);
        }
        else
        {
            if (await iconContext.Icons.Where(icon => icon.PathToIcon == iconPath).AnyAsync())
            {
                var listOfIcons = await iconContext.Icons
                    .Where(icon => icon.PathToIcon == iconPath)
                    .OrderBy(ic => ic.IconFileName)
                    .ToArrayAsync();

                return View("IconsList", listOfIcons);
            }
            else
            {
                var typesOfIcons = iconContext.Icons
                    .Where(icon => icon.IconFileName.Contains("ms-tile-558"));

                return View(typesOfIcons);
            }
        }
    }

    #endregion

    #region Добавить иконку сайта

    [HttpGet]
    public ViewResult AddIcon(string? pathForIcon)
    {
        IconModel addIcon = new();

        if (pathForIcon == null)
        {
            addIcon.NewIcon = true;

            return View("AddNewType", addIcon);
        }
        else
        {
            #region Список папок с иконками (не используем)

            string[] dirrectories = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + DataConfig.IconFoldersPath, "*", SearchOption.AllDirectories);

            List<string> paths = [];

            foreach (var item in dirrectories)
            {
                int indexOfSubstring = item.IndexOf(DataConfig.IconFoldersPath);

                paths.Add(item.Substring(indexOfSubstring + DataConfig.IconFoldersPath.Length));
            }

            ViewData["IconPaths"] = new SelectList(paths);

            #endregion

            addIcon.PathToIcon = pathForIcon;

            return View(addIcon);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(5_268_435_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5268435456)]
    public async Task<IActionResult> AddIcon(
    [Bind(
                "IconModelId," +
                "IconFileName," +
                "PathToIcon," +
                "IconMimeType," +
                "RelForIcon," +
                "IconSize," +
                "IconPurpose," +
                "IconFileFormFile," +
                "NewIconPath," +
                "NewIcon"
        )]
        IconModel addIcon)
    {
        if (ModelState.IsValid)
        {
            #region Добавить новый тип иконки

            if (addIcon.NewIcon == true)
            {
                #region Создание нового каталога
                
                if (string.IsNullOrEmpty(addIcon.NewIconPath))
                {
                    ModelState.AddModelError("NewIconPath", "Введите название каталога");
                }
                else
                {
                    addIcon.PathToIcon = addIcon.NewIconPath.Trim('/').Trim('\\') + '/';
                }

                string pathToNewIcon = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), DataConfig.IconFoldersPath, addIcon.PathToIcon)).Replace('\\', '/');

                if (System.IO.Directory.Exists(pathToNewIcon))
                {
                    ModelState.AddModelError("NewIconPath", $"Каталог «{addIcon.PathToIcon}» уже существует");

                    return View("AddNewType", addIcon);
                }
                else
                {
                    System.IO.Directory.CreateDirectory(pathToNewIcon);
                }

                #endregion

                if (addIcon.IconFileFormFile == null)
                {
                    ModelState.AddModelError("IconFileFormFile", $"Выберите файл иконки");

                    return View("AddNewType", addIcon);
                }
                else
                {
                    
                    #region Копируем файл в папку Temp

                    string iconTempPath = Path.Combine(rootPath + DataConfig.TempPath, addIcon.IconFileFormFile.FileName).Replace('\\', '/');

                    FileInfo iconFile = new(iconTempPath);

                    if (!iconFile.Exists)
                    {
                        using FileStream stream = new(iconTempPath, FileMode.Create);

                        await addIcon.IconFileFormFile.CopyToAsync(stream);
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
                                    ModelState.AddModelError("IconFileFormFile", $"Не определить имя файла «{addIcon.IconFileFormFile.FileName}»");

                                    #region Удаляем созданный каталог и файл из папки Temp

                                    DirectoryInfo dirInfo = new(pathToNewIcon);

                                    if (dirInfo.Exists)
                                    {
                                        dirInfo.Delete(true);
                                    }

                                    FileInfo fileInfo = new(iconTempPath);

                                    if (fileInfo.Exists)
                                    {
                                        fileInfo.Delete();
                                        // альтернатива с помощью класса File
                                        // File.Delete(path);
                                    }

                                    #endregion

                                    return View("AddNewType", addIcon);
                                }

                                if (!tag.Description.Contains("ms-tile-558"))
                                {
                                    ModelState.AddModelError("IconFileFormFile", $"Имя файла должно содержать «ms-tile-558»");

                                    #region Удаляем созданный каталог и файл из папки Temp

                                    DirectoryInfo dirInfo = new(pathToNewIcon);

                                    if (dirInfo.Exists)
                                    {
                                        dirInfo.Delete(true);
                                    }

                                    FileInfo fileInfo = new(iconTempPath);

                                    if (fileInfo.Exists)
                                    {
                                        fileInfo.Delete();
                                        // альтернатива с помощью класса File
                                        // File.Delete(path);
                                    }

                                    #endregion

                                    return View("AddNewType", addIcon);
                                }

                                addIcon.IconFileName = tag.Description;
                            }

                            #endregion
                            
                            #region Определяем MIME Type

                            if (tag.Name == "Detected MIME Type")
                            {
                                if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("IconFileFormFile", $"Не определить MIME файла «{addIcon.IconFileFormFile.FileName}»");

                                    #region Удаляем созданный каталог и файл из папки Temp

                                    DirectoryInfo dirInfo = new(pathToNewIcon);

                                    if (dirInfo.Exists)
                                    {
                                        dirInfo.Delete(true);
                                    }

                                    FileInfo fileInf = new(iconTempPath);

                                    if (fileInf.Exists)
                                    {
                                        fileInf.Delete();
                                        // альтернатива с помощью класса File
                                        // File.Delete(path);
                                    }

                                    #endregion

                                    return View("AddNewType", addIcon);
                                }

                                addIcon.IconMimeType = tag.Description;
                            }

                            #endregion
                            
                            #region Определяем ширину файла

                            if (tag.Name == "Image Width")
                            {
                                if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("IconFileFormFile", $"Не определить ширину файла «{addIcon.IconFileFormFile.FileName}»");

                                    #region Удаляем созданный каталог и файл из папки Temp

                                    DirectoryInfo dirInfo = new(pathToNewIcon);

                                    if (dirInfo.Exists)
                                    {
                                        dirInfo.Delete(true);
                                    }

                                    FileInfo fileInf = new(iconTempPath);

                                    if (fileInf.Exists)
                                    {
                                        fileInf.Delete();
                                        // альтернатива с помощью класса File
                                        // File.Delete(path);
                                    }

                                    #endregion

                                    return View("AddNewType", addIcon);
                                }

                                if (tag.Description != "558")
                                {
                                    ModelState.AddModelError("IconFileFormFile", $"Ширина файла должна быть «558 px»");

                                    #region Удаляем созданный каталог и файл из папки Temp

                                    DirectoryInfo dirInfo = new(pathToNewIcon);

                                    if (dirInfo.Exists)
                                    {
                                        dirInfo.Delete(true);
                                    }

                                    FileInfo fileInf = new(iconTempPath);

                                    if (fileInf.Exists)
                                    {
                                        fileInf.Delete();
                                        // альтернатива с помощью класса File
                                        // File.Delete(path);
                                    }

                                    #endregion

                                    return View("AddNewType", addIcon);
                                }

                                addIcon.IconSize = tag.Description + 'x';
                            }

                            #endregion
                            
                            #region Определяем высоту файла

                            if (tag.Name == "Image Height")
                            {
                                if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("IconFileFormFile", $"Не определить высоту файла «{addIcon.IconFileFormFile.FileName}»");

                                    #region Удаляем созданный каталог и файл из папки Temp

                                    DirectoryInfo dirInfo = new(pathToNewIcon);

                                    if (dirInfo.Exists)
                                    {
                                        dirInfo.Delete(true);
                                    }

                                    FileInfo fileInf = new(iconTempPath);

                                    if (fileInf.Exists)
                                    {
                                        fileInf.Delete();
                                        // альтернатива с помощью класса File
                                        // File.Delete(path);
                                    }

                                    #endregion

                                    return View("AddNewType", addIcon);
                                }

                                if (tag.Description != "558")
                                {
                                    ModelState.AddModelError("IconFileFormFile", "Высота файла  должна быть «558 px»");

                                    #region Удаляем созданный каталог и файл из папки Temp

                                    DirectoryInfo dirInfo = new(pathToNewIcon);

                                    if (dirInfo.Exists)
                                    {
                                        dirInfo.Delete(true);
                                    }

                                    FileInfo fileInf = new(iconTempPath);

                                    if (fileInf.Exists)
                                    {
                                        fileInf.Delete();
                                        // альтернатива с помощью класса File
                                        // File.Delete(path);
                                    }

                                    #endregion

                                    return View("AddNewType", addIcon);
                                }

                                addIcon.IconSize += tag.Description;
                            }

                            #endregion
                        }
                    }

                    #endregion

                    addIcon.RelForIcon = "icon";

                    addIcon.IconPurpose = "any";
                    
                    #region Копируем файл в папку иконок и удаляем из папки Temp

                    string iconPath = Path.Combine(rootPath + DataConfig.IconsFolder + addIcon.PathToIcon, addIcon.IconFileFormFile.FileName).Replace('\\', '/');

                    FileInfo iconFileInfo = new(iconPath);

                    if (!iconFileInfo.Exists)
                    {
                        using FileStream stream = new(iconPath, FileMode.Create);

                        await addIcon.IconFileFormFile.CopyToAsync(stream);
                    }

                    FileInfo tempFile = new(iconTempPath);

                    if (tempFile.Exists)
                    {
                        tempFile.Delete();
                    }

                    #endregion
                }
            }

            #endregion

            #region Добавить иконку

            else
            {
                if (addIcon.IconFileFormFile == null)
                {
                    ModelState.AddModelError("IconFileFormFile", "Выберите файл иконки");

                    return View(addIcon);
                }

                if (addIcon.IconFileFormFile.FileName.Contains(".svg"))
                {
                    #region Копируем файл в папку Temp

                    string iconTempPath = Path.Combine(rootPath + DataConfig.TempPath, addIcon.IconFileFormFile.FileName).Replace('\\', '/');

                    FileInfo iconFile = new(iconTempPath);

                    if (!iconFile.Exists)
                    {
                        using FileStream stream = new(iconTempPath, FileMode.Create);

                        await addIcon.IconFileFormFile.CopyToAsync(stream);
                    }

                    #endregion

                    _ = addIcon.PathToIcon;

                    addIcon.IconFileName = addIcon.IconFileFormFile.FileName;

                    addIcon.IconMimeType = "image/svg+xml";

                    addIcon.IconSize = "any";

                    addIcon.RelForIcon = "mask-icon";

                    addIcon.IconPurpose = "maskable";

                    #region Копируем файл в папку иконок и удаляем из папки Temp

                    string iconPath = Path.Combine(rootPath + DataConfig.IconsFolder + addIcon.PathToIcon, addIcon.IconFileFormFile.FileName).Replace('\\', '/');

                    FileInfo iconFileInfo = new(iconPath);

                    if (!iconFileInfo.Exists)
                    {
                        using FileStream stream = new(iconPath, FileMode.Create);

                        await addIcon.IconFileFormFile.CopyToAsync(stream);
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

                    string iconTempPath = Path.Combine(rootPath + DataConfig.TempPath, addIcon.IconFileFormFile.FileName).Replace('\\', '/');

                    FileInfo iconFile = new(iconTempPath);

                    if (!iconFile.Exists)
                    {
                        using FileStream stream = new(iconTempPath, FileMode.Create);

                        await addIcon.IconFileFormFile.CopyToAsync(stream);
                    }

                    #endregion

                    #region Параметр PathToIcon

                    _ = addIcon.PathToIcon;

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
                                    ModelState.AddModelError("IconFileFormFile", $"Не определить имя файла «{addIcon.IconFileFormFile.FileName}»");

                                    #region Удаляем файл из папки Temp

                                    FileInfo fileInfo = new(iconTempPath);

                                    if (fileInfo.Exists)
                                    {
                                        fileInfo.Delete();
                                    }

                                    #endregion

                                    return View(addIcon);
                                }

                                if (await iconContext.Icons.Where(icon => icon.IconFileName == addIcon.IconFileFormFile.FileName & icon.PathToIcon == addIcon.PathToIcon).AnyAsync())
                                {
                                    ModelState.AddModelError("IconFileFormFile", $"Файл «{addIcon.IconFileFormFile.FileName}» существует в каталоге «{addIcon.PathToIcon}»");

                                    #region Удаляем файл из папки Temp

                                    FileInfo fileInfo = new(iconTempPath);

                                    if (fileInfo.Exists)
                                    {
                                        fileInfo.Delete();
                                    }

                                    #endregion

                                    return View(addIcon);
                                }

                                addIcon.IconFileName = tag.Description;
                            }

                            #endregion

                            #region Определяем MIME Type

                            if (tag.Name == "Detected MIME Type")
                            {
                                if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("IconFileFormFile", $"Не определить MIME файла «{addIcon.IconFileFormFile.FileName}»");

                                    #region Удаляем созданный каталог и файл из папки Temp

                                    FileInfo fileInf = new(iconTempPath);

                                    if (fileInf.Exists)
                                    {
                                        fileInf.Delete();
                                    }

                                    #endregion

                                    return View(addIcon);
                                }

                                addIcon.IconMimeType = tag.Description;
                            }

                            #endregion

                            #region Определяем ширину файла

                            if (tag.Name == "Image Width")
                            {
                                if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("IconFileFormFile", $"Не определить ширину файла «{addIcon.IconFileFormFile.FileName}»");

                                    #region Удаляем файл из папки Temp

                                    FileInfo fileInf = new(iconTempPath);

                                    if (fileInf.Exists)
                                    {
                                        fileInf.Delete();
                                    }

                                    #endregion

                                    return View(addIcon);
                                }

                                addIcon.IconWidth = tag.Description;
                            }

                            #endregion

                            #region Определяем высоту файла

                            if (tag.Name == "Image Height")
                            {
                                if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                                {
                                    ModelState.AddModelError("IconFileFormFile", $"Не определить высоту файла «{addIcon.IconFileFormFile.FileName}»");

                                    #region Удаляем файл из папки Temp

                                    FileInfo fileInf = new(iconTempPath);

                                    if (fileInf.Exists)
                                    {
                                        fileInf.Delete();
                                    }

                                    #endregion

                                    return View(addIcon);
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

                        return View(addIcon);
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

                    string iconPath = Path.Combine(rootPath + DataConfig.IconsFolder + addIcon.PathToIcon, addIcon.IconFileFormFile.FileName).Replace('\\', '/');

                    FileInfo iconFileInfo = new(iconPath);

                    if (!iconFileInfo.Exists)
                    {
                        using FileStream stream = new(iconPath, FileMode.Create);

                        await addIcon.IconFileFormFile.CopyToAsync(stream);
                    }

                    FileInfo tempFile = new(iconTempPath);

                    if (tempFile.Exists)
                    {
                        tempFile.Delete();
                    }

                    #endregion
                }
            }

            #endregion

            #region Сохранить в базе данных

            await iconContext.AddNewIconAsync(addIcon);

            #endregion

            #region Открытие страницы Index

            var newIcon = await iconContext.Icons.FirstAsync(icon => icon.IconFileName == addIcon.IconFileName & icon.PathToIcon == addIcon.PathToIcon);

            return RedirectToAction("Index", new { iconPath = addIcon.PathToIcon, iconId = newIcon.IconModelId });

            #endregion
        }
        else
        {
            return View(new IconModel());
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
                "IconFileName," +
                "PathToIcon," +
                "IconMimeType," +
                "RelForIcon," +
                "IconSize," +
                "IconPurpose," +
                "IconFileFormFile," +
                "NewIconPath," +
                "NewIcon"
        )]
        IconModel editIcon)
    {
        if (ModelState.IsValid)
        {
            #region Инициализация экземпляра иконки

            var iconUpdate = await iconContext.Icons
                .FirstAsync(icon => icon.IconModelId == editIcon.IconModelId);

            #endregion

            #region Параметр PathToIcon

            iconUpdate.PathToIcon = editIcon.PathToIcon;

            #endregion

            #region Параметр FileName

            iconUpdate.IconFileName = editIcon.IconFileName;

            #endregion

            #region Параметр MYME Type

            iconUpdate.IconMimeType = editIcon.IconMimeType;

            #endregion

            #region Параметр IconSize

            iconUpdate.IconSize = editIcon.IconSize;

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

            return RedirectToAction("Index", new { iconPath = iconUpdate.PathToIcon, iconId = iconUpdate.IconModelId });

            #endregion
        }
        else
        {
            return View(new IconModel());
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
                "IconFileName," +
                "PathToIcon," +
                "IconMimeType," +
                "RelForIcon," +
                "IconSize," +
                "IconPurpose," +
                "IconFileFormFile," +
                "NewIconPath," +
                "NewIcon"
        )]
        IconModel deleteIcon)
    {
        if (ModelState.IsValid)
        {
            #region Инициализация экземпляра иконки

            var iconDelete = await iconContext.Icons
                .FirstAsync(icon => icon.IconModelId == deleteIcon.IconModelId);

            #endregion

            #region Перемещаем файл иконки в папку Temp

            string iconPath = Path.Combine(rootPath + DataConfig.IconsFolder + iconDelete.PathToIcon, iconDelete.IconFileName).Replace('\\', '/');

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

            return RedirectToAction("Index", new { iconPath = iconDelete.PathToIcon });

            #endregion
        }
        else
        {
            return View(deleteIcon);
        }
    }

    #endregion
}