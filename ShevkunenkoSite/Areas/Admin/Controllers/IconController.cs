using Microsoft.AspNetCore.Mvc.Rendering;

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
                .Where(icon => icon.IconFileName.Contains("ms-tile-558"));

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

        #region Список папок с иконками

        string[] dirrectories = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + DataConfig.IconFoldersPath, "*", SearchOption.AllDirectories);

        List<string> paths = [];

        foreach (var item in dirrectories)
        {
            int indexOfSubstring = item.IndexOf(DataConfig.IconFoldersPath);

            paths.Add(item.Substring(indexOfSubstring + DataConfig.IconFoldersPath.Length));
        }

        ViewData["IconPaths"] = new SelectList(paths);

        #endregion

        return View(addIcon);
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
            if (addIcon.NewIcon == true)
            {
                #region Создание нового каталога

                addIcon.PathToIcon = addIcon.NewIconPath.Trim('/').Trim('\\') + '/';

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
                        // альтернатива с помощью класса File
                        // File.Delete(path);
                    }

                    #endregion
                }
            }

            #region Сохранить в базе данных

            await iconContext.AddNewIconAsync(addIcon);

            #endregion

            #region Открытие страницы Index

            return RedirectToAction("Index");

            #endregion
        }
        else
        {
            return View("AddNewType", new IconModel());
        }
    }

    #endregion
}