namespace ShevkunenkoSite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class IconInfoController(IIconFileRepository iconContext) : Controller
{
    public async Task<IActionResult> Index(Guid? iconId)
    {
        IconFileModel icon;

#pragma warning disable CA1862
        var typesOfIcons = iconContext.IconFiles
            .Where(icon => icon.IconFileNameExtension.ToLower().Contains("svg"));
#pragma warning restore CA1862

        if (!iconId.HasValue)
        {
            return View(typesOfIcons);
        }
        else
        {
            if (await iconContext.IconFiles.Where(icon => icon.IconFileModelId == iconId).AnyAsync())
            {
                icon = await iconContext.IconFiles.FirstAsync(icon => icon.IconFileModelId == iconId);

                var listOfIcons = await iconContext.IconFiles
                    .Where(ic => ic.IconPath == icon.IconPath)
                    .OrderBy(ic => ic.IconFileName)
                    .ToArrayAsync();

                return View("IconsList", listOfIcons);
            }
            else
            {
                return View(typesOfIcons);
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditIcon(Guid? iconId)
    {
        EditIconViewModel editIcon = new();

        if (await iconContext.IconFiles.Where(icon => icon.IconFileModelId == iconId).AnyAsync())
        {
            editIcon.EditIcon = await iconContext.IconFiles.FirstAsync(icon => icon.IconFileModelId == iconId);

            return View(editIcon);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditIcon(EditIconViewModel iconItem)
    {
        if (ModelState.IsValid)
        {
            IconFileModel iconUpdate = await iconContext.IconFiles.FirstAsync(icon => icon.IconFileModelId == iconItem.EditIcon.IconFileModelId);

            #region Проверка FormFile на null

            if (iconItem.IconFormFile == null)
            {
                ModelState.AddModelError("IconFormFile", "выберите файл картинки");

                return View(iconItem);
            }

            #endregion

            #region Проверка расширения файла

            var iconFormFileExtansion = iconItem.IconFormFile.FileName[(iconItem.IconFormFile.FileName.IndexOf('.') + 1)..];

            if (iconItem.EditIcon.IconRel.Contains("apple-touch-icon") || iconItem.EditIcon.IconRel == "icon")
            {
                if (iconFormFileExtansion != "png" & iconFormFileExtansion != "webp")
                {
                    ModelState.AddModelError("IconFormFile", "Расширение файла должно быть «png» или «webp»");

                    return View(iconItem);
                }
            }
            else if (iconItem.EditIcon.IconRel == "mask - icon")
            {
                if (iconFormFileExtansion != "svg")
                {
                    ModelState.AddModelError("IconFormFile", "Расширение файла должно быть «svg»");

                    return View(iconItem);
                }
            }
            else
            {
                if (iconFormFileExtansion != "ico")
                {
                    ModelState.AddModelError("IconFormFile", "Расширение файла должно быть «ico»");

                    return View(iconItem);
                }
            }

            #endregion

            #region Копируем файл в папку Temp

            string newNameForIconFormFile =
                iconItem.EditIcon.IconFileName[..iconItem.EditIcon.IconFileName.IndexOf('.')] +
                iconItem.IconFormFile.FileName[iconItem.IconFormFile.FileName.IndexOf('.')..];

            string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Temp", newNameForIconFormFile);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await iconItem.IconFormFile.CopyToAsync(fileStream);
            }

            #endregion

            #region Получаем параметры файла

            IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path));

            foreach (var imageDirectory in imageDirectories)
            {
                foreach (var tag in imageDirectory.Tags)
                {
                    #region Имя файла

                    if (tag.Name == "File Name")
                    {
                        if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                        {
                            ModelState.AddModelError("IconFormFile", $"Не определить имя файла «{iconItem.IconFormFile.FileName}»");

                            return View(iconItem);
                        }

                        if (tag.Description != newNameForIconFormFile)
                        {
                            ModelState.AddModelError("IconFormFile", $"Имя файла должно быть «{newNameForIconFormFile}»");

                            return View(iconItem);
                        }

                        iconUpdate.IconFileName = tag.Description;
                    }

                    #endregion

                    #region Расширение имени файла

                    if (tag.Name == "Expected File Name Extension")
                    {
                        if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                        {
                            ModelState.AddModelError("IconFormFile", $"Не определить расширение файла «{newNameForIconFormFile}»");

                            return View(iconItem);
                        }

                        iconUpdate.IconFileNameExtension = tag.Description;
                    }

                    #endregion

                    #region MIME Type

                    if (tag.Name == "Detected MIME Type")
                    {
                        if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                        {
                            ModelState.AddModelError("IconFormFile", $"Не определить MIME файла «{newNameForIconFormFile}»");

                            return View(iconItem);
                        }

                        iconUpdate.IconMimeType = tag.Description;
                    }

                    #endregion

                    #region Размер файла

                    if (tag.Name == "File Size")
                    {
                        if (tag.Description == null || Convert.ToInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                        {
                            ModelState.AddModelError("IconFormFile", $"Не определить размер файла «{newNameForIconFormFile}»");

                            return View(iconItem);
                        }

                        iconUpdate.IconFileSize = Convert.ToInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                    }

                    #endregion

                    #region Ширина иконки

                    if (tag.Name == "Image Width")
                    {
                        if (tag.Description == null || Convert.ToInt32(tag.Description) <= 0)
                        {
                            ModelState.AddModelError("IconFormFile", $"Не определить ширину файла «{newNameForIconFormFile}»");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-48") && Convert.ToInt32(tag.Description) != 48)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 48px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-57x57")
                            | newNameForIconFormFile.Contains("apple-touch-icon-57x57-precomposed")
                            && Convert.ToInt32(tag.Description) != 57)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 57px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-60x60")
                            | newNameForIconFormFile.Contains("apple-touch-icon-60x60-precomposed")
                            && Convert.ToInt32(tag.Description) != 60)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 60px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-72x72")
                            | newNameForIconFormFile.Contains("apple-touch-icon-72x72-precomposed")
                            && Convert.ToInt32(tag.Description) != 72)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 72px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-76x76")
                            | newNameForIconFormFile.Contains("apple-touch-icon-76x76-precomposed")
                            && Convert.ToInt32(tag.Description) != 76)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 76px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-96") && Convert.ToInt32(tag.Description) != 96)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 96px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-114x114")
                           | newNameForIconFormFile.Contains("apple-touch-icon-114x114-precomposed")
                           && Convert.ToInt32(tag.Description) != 114)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 114px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-120x120")
                           | newNameForIconFormFile.Contains("apple-touch-icon-120x120-precomposed")
                           && Convert.ToInt32(tag.Description) != 120)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 120px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-144x144")
                           | newNameForIconFormFile.Contains("apple-touch-icon-144x144-precomposed")
                           | newNameForIconFormFile.Contains("icon-144")
                           && Convert.ToInt32(tag.Description) != 144)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 144px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-152x152")
                           | newNameForIconFormFile.Contains("apple-touch-icon-152x152-precomposed")
                           && Convert.ToInt32(tag.Description) != 152)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 152px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-167x167")
                           | newNameForIconFormFile.Contains("apple-touch-icon-167x167-precomposed")
                           && Convert.ToInt32(tag.Description) != 167)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 167px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-180x180")
                           | newNameForIconFormFile.Contains("apple-touch-icon-180x180-precomposed")
                           && Convert.ToInt32(tag.Description) != 180)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 180px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-192") && Convert.ToInt32(tag.Description) != 192)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 192px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-256") && Convert.ToInt32(tag.Description) != 256)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 256px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-384") && Convert.ToInt32(tag.Description) != 384)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 384px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-512") && Convert.ToInt32(tag.Description) != 512)
                        {
                            ModelState.AddModelError("IconFormFile", $"Ширина иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 512px");

                            return View(iconItem);
                        }

                        iconUpdate.IconWidth = Convert.ToInt32(tag.Description);
                    }

                    #endregion

                    #region Высота иконки

                    if (tag.Name == "Image Height")
                    {
                        if (tag.Description == null || Convert.ToInt32(tag.Description) <= 0)
                        {
                            ModelState.AddModelError("IconFormFile", $"Не определить высоту файла «{newNameForIconFormFile}»");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-48") && Convert.ToInt32(tag.Description) != 48)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 48px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-57x57")
                            | newNameForIconFormFile.Contains("apple-touch-icon-57x57-precomposed")
                            && Convert.ToInt32(tag.Description) != 57)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 57px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-60x60")
                            | newNameForIconFormFile.Contains("apple-touch-icon-60x60-precomposed")
                            && Convert.ToInt32(tag.Description) != 60)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 60px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-72x72")
                            | newNameForIconFormFile.Contains("apple-touch-icon-72x72-precomposed")
                            && Convert.ToInt32(tag.Description) != 72)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 72px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-76x76")
                            | newNameForIconFormFile.Contains("apple-touch-icon-76x76-precomposed")
                            && Convert.ToInt32(tag.Description) != 76)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 76px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-96") && Convert.ToInt32(tag.Description) != 96)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 96px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-114x114")
                           | newNameForIconFormFile.Contains("apple-touch-icon-114x114-precomposed")
                           && Convert.ToInt32(tag.Description) != 114)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 114px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-120x120")
                           | newNameForIconFormFile.Contains("apple-touch-icon-120x120-precomposed")
                           && Convert.ToInt32(tag.Description) != 120)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 120px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-144x144")
                           | newNameForIconFormFile.Contains("apple-touch-icon-144x144-precomposed")
                           | newNameForIconFormFile.Contains("icon-144")
                           && Convert.ToInt32(tag.Description) != 144)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 144px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-152x152")
                           | newNameForIconFormFile.Contains("apple-touch-icon-152x152-precomposed")
                           && Convert.ToInt32(tag.Description) != 152)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 152px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-167x167")
                           | newNameForIconFormFile.Contains("apple-touch-icon-167x167-precomposed")
                           && Convert.ToInt32(tag.Description) != 167)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 167px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("apple-touch-icon-180x180")
                           | newNameForIconFormFile.Contains("apple-touch-icon-180x180-precomposed")
                           && Convert.ToInt32(tag.Description) != 180)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 180px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-192") && Convert.ToInt32(tag.Description) != 192)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 192px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-256") && Convert.ToInt32(tag.Description) != 256)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 256px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-384") && Convert.ToInt32(tag.Description) != 384)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 384px");

                            return View(iconItem);
                        }

                        if (newNameForIconFormFile.Contains("icon-512") && Convert.ToInt32(tag.Description) != 512)
                        {
                            ModelState.AddModelError("IconFormFile", $"Высота иконки «{newNameForIconFormFile}»={Convert.ToInt32(tag.Description)}px. Должна быть 512px");

                            return View(iconItem);
                        }

                        iconUpdate.IconHeight = Convert.ToInt32(tag.Description);
                    }

                    #endregion
                }
            }

            #endregion

            iconUpdate.IconPath = iconItem.EditIcon.IconPath;
            iconUpdate.IconRel = iconItem.EditIcon.IconRel;
            iconUpdate.IconType = iconUpdate.IconMimeType;

            await iconContext.SaveChangesInIconAsync();

            #region Удалить старый файл иконки

            string pathForIconToDelete = Path.Join(System.IO.Directory.GetCurrentDirectory(), DataConfig.IconFoldersPath, iconItem.EditIcon.IconPath, iconItem.EditIcon.IconFileName);

            FileInfo fileInfDelete = new(pathForIconToDelete);

            if (fileInfDelete.Exists)
            {
                fileInfDelete.Delete();
            }

            #endregion

            #region Записать новый файл иконки

            string newPath = Path.Join(System.IO.Directory.GetCurrentDirectory(), DataConfig.IconFoldersPath, iconItem.EditIcon.IconPath, newNameForIconFormFile);

            FileInfo fileInfMove = new(path);

            if (fileInfMove.Exists)
            {
                fileInfMove.MoveTo(newPath, true);
            }

            #endregion

            return RedirectToAction(nameof(Index), new { iconId = iconItem.EditIcon.IconFileModelId });
        }
        else
        {
            return View(iconItem);
        }
    }
}