using Microsoft.IdentityModel.Tokens;
using ShevkunenkoSite.Pages.Shared.Components.Code;

namespace ShevkunenkoSite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ImageInfoController(
    IImageFileRepository imageContext,
    IBooksAndArticlesRepository bookContext) : Controller
{
    public ImageFileModel? ImageItem { get; set; }

    private static readonly char[] separator = [','];

    #region Список картинок

    public async Task<ViewResult> Index
        (
        string? searchString,
        int pageNumber = 1,
        bool pageCard = false
        )
    {
        var allSiteImages = await imageContext.ImageFiles.ToListAsync();

        if (!searchString.IsNullOrEmpty())
        {
            allSiteImages = [.. allSiteImages.ImageSearch(searchString).OrderBy(siteImage => siteImage.SortOfPicture)];
        }

        #region Выбор представления - список или иконки

        ItemsListViewModel itemList = new()
        {
            AllImageFiles = [.. allSiteImages
                     .Skip((pageNumber - 1) * DataConfig.NumberOfItemsPerPage)
                     .Take(DataConfig.NumberOfItemsPerPage)],

            #region Свойства PagingInfoViewModel

            TotalItems = allSiteImages.Count,

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
            return View("IconList", itemList);
        }

        #endregion
    }

    #endregion

    #region Информация о картинке

    public async Task<IActionResult> DetailsImage(Guid? imageId, string? imageIcon)
    {
        if (imageId.HasValue && await imageContext.ImageFiles.Where(i => i.ImageFileModelId == imageId).AnyAsync())
        {
            ImageItem = await imageContext.ImageFiles.FirstAsync(i => i.ImageFileModelId == imageId);

            bool fileExists = true;

            string buttonInfoStyle = "btn btn-outline-primary py-1 px-1 shadw";

            string webImageButtonInfostyle = buttonInfoStyle;
            string imageButtonInfostyle = buttonInfoStyle;
            string webhdButtonInfostyle = buttonInfoStyle;
            string hdButtonInfostyle = buttonInfoStyle;
            string icon300ButtonInfostyle = buttonInfoStyle;
            string webIcon300ButtonInfostyle = buttonInfoStyle;
            string icon200ButtonInfostyle = buttonInfoStyle;
            string webIcon200ButtonInfostyle = buttonInfoStyle;
            string icon100ButtonInfostyle = buttonInfoStyle;
            string webIcon100ButtonInfostyle = buttonInfoStyle;

            string fileName = string.Empty;

            #region Проверка наличия файлов в каталоге

            if (!string.IsNullOrEmpty(ImageItem.WebImageFileName))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.WebImageFileName));

                if (!imageFile.Exists)
                {
                    webImageButtonInfostyle = "btn btn-outline-danger py-1 px-1 shadw";
                }
            }

            if (!string.IsNullOrEmpty(ImageItem.ImageFileName))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.ImageFileName));

                if (!imageFile.Exists)
                {
                    imageButtonInfostyle = "btn btn-outline-danger py-1 px-1 shadw";
                }
            }

            if (!string.IsNullOrEmpty(ImageItem.WebImageHDFileName))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.WebImageHDFileName));

                if (!imageFile.Exists)
                {
                    webhdButtonInfostyle = "btn btn-outline-danger py-1 px-1 shadw";
                }
            }

            if (!string.IsNullOrEmpty(ImageItem.ImageHDFileName))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.ImageHDFileName));

                if (!imageFile.Exists)
                {
                    hdButtonInfostyle = "btn btn-outline-danger py-1 px-1 shadw";
                }
            }

            if (!string.IsNullOrEmpty(ImageItem.WebIconFileName))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.WebIconFileName));

                if (!imageFile.Exists)
                {
                    webIcon300ButtonInfostyle = "btn btn-outline-danger py-1 px-1 shadw";
                }
            }

            if (!string.IsNullOrEmpty(ImageItem.IconFileName))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.IconFileName));

                if (!imageFile.Exists)
                {
                    icon300ButtonInfostyle = "btn btn-outline-danger py-1 px-1 shadw";
                }
            }

            if (!string.IsNullOrEmpty(ImageItem.WebIcon200FileName))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.WebIcon200FileName));

                if (!imageFile.Exists)
                {
                    webIcon200ButtonInfostyle = "btn btn-outline-danger py-1 px-1 shadw";
                }
            }

            if (!string.IsNullOrEmpty(ImageItem.Icon200FileName))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.Icon200FileName));

                if (!imageFile.Exists)
                {
                    icon200ButtonInfostyle = "btn btn-outline-danger py-1 px-1 shadw";
                }
            }

            if (!string.IsNullOrEmpty(ImageItem.WebIcon100FileName))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.WebIcon100FileName));

                if (!imageFile.Exists)
                {
                    webIcon100ButtonInfostyle = "btn btn-outline-danger py-1 px-1 shadw";
                }
            }

            if (!string.IsNullOrEmpty(ImageItem.Icon100FileName))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.Icon100FileName));

                if (!imageFile.Exists)
                {
                    icon100ButtonInfostyle = "btn btn-outline-danger py-1 px-1 shadw";
                }
            }

            #endregion

            if (imageIcon == "webimage" & !string.IsNullOrEmpty(ImageItem.WebImageFileName))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.WebImageFileName));

                if (imageFile.Exists)
                {
                    fileExists = true;
                }
                else
                {
                    fileExists = false;
                }

                DetailsImageIconViewModel iconView = new()
                {
                    ImageItem = ImageItem,
                    IconType = imageIcon,
                    FileExists = fileExists,
                    FileName = ImageItem.WebImageFileName,
                    WebImageButtonInfostyle = webImageButtonInfostyle
                };

                return View(iconView);
            }
            else if ((imageIcon == "image" & !string.IsNullOrEmpty(ImageItem.ImageFileName)))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.ImageFileName));

                if (imageFile.Exists)
                {
                    fileExists = true;
                }
                else
                {
                    fileExists = false;
                }

                DetailsImageIconViewModel iconView = new()
                {
                    ImageItem = ImageItem,
                    IconType = imageIcon,
                    FileExists = fileExists,
                    FileName = ImageItem.WebImageFileName,
                    ImageButtonInfostyle = imageButtonInfostyle
                };

                return View(iconView);
            }
            else if ((imageIcon == "webhd" & !string.IsNullOrEmpty(ImageItem.WebImageHDFileName)))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.WebImageHDFileName!));

                if (imageFile.Exists)
                {
                    fileExists = true;
                }
                else
                {
                    fileExists = false;
                }

                DetailsImageIconViewModel iconView = new()
                {
                    ImageItem = ImageItem,
                    IconType = imageIcon,
                    FileExists = fileExists,
                    FileName = ImageItem.WebImageFileName,
                    WebhdButtonInfostyle = webhdButtonInfostyle
                };

                return View(iconView);
            }
            else if ((imageIcon == "hd" & !string.IsNullOrEmpty(ImageItem.ImageHDFileName)))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.ImageHDFileName!));

                if (imageFile.Exists)
                {
                    fileExists = true;
                }
                else
                {
                    fileExists = false;
                }

                DetailsImageIconViewModel iconView = new()
                {
                    ImageItem = ImageItem,
                    IconType = imageIcon,
                    FileExists = fileExists,
                    FileName = ImageItem.WebImageFileName,
                    HdButtonInfostyle = hdButtonInfostyle
                };

                return View(iconView);
            }
            else if ((imageIcon == "webicon300" & !string.IsNullOrEmpty(ImageItem.WebIconFileName)))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.WebIconFileName));

                if (imageFile.Exists)
                {
                    fileExists = true;
                }
                else
                {
                    fileExists = false;
                }

                DetailsImageIconViewModel iconView = new()
                {
                    ImageItem = ImageItem,
                    IconType = imageIcon,
                    FileExists = fileExists,
                    FileName = ImageItem.WebImageFileName,
                    WebIcon300ButtonInfostyle = webIcon300ButtonInfostyle
                };

                return View(iconView);
            }
            else if ((imageIcon == "icon300" & !string.IsNullOrEmpty(ImageItem.IconFileName)))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.IconFileName!));

                if (imageFile.Exists)
                {
                    fileExists = true;
                }
                else
                {
                    fileExists = false;
                }

                DetailsImageIconViewModel iconView = new()
                {
                    ImageItem = ImageItem,
                    IconType = imageIcon,
                    FileExists = fileExists,
                    FileName = ImageItem.WebImageFileName,
                    Icon300ButtonInfostyle = icon300ButtonInfostyle
                };

                return View(iconView);
            }
            else if ((imageIcon == "webicon200" & !string.IsNullOrEmpty(ImageItem.WebIcon200FileName)))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.WebIcon200FileName));

                if (imageFile.Exists)
                {
                    fileExists = true;
                }
                else
                {
                    fileExists = false;
                }

                DetailsImageIconViewModel iconView = new()
                {
                    ImageItem = ImageItem,
                    IconType = imageIcon,
                    FileExists = fileExists,
                    FileName = ImageItem.WebImageFileName,
                    WebIcon200ButtonInfostyle = webIcon200ButtonInfostyle
                };

                return View(iconView);
            }
            else if ((imageIcon == "icon200" & !string.IsNullOrEmpty(ImageItem.Icon200FileName)))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.Icon200FileName!));

                if (imageFile.Exists)
                {
                    fileExists = true;
                }
                else
                {
                    fileExists = false;
                }

                DetailsImageIconViewModel iconView = new()
                {
                    ImageItem = ImageItem,
                    IconType = imageIcon,
                    FileExists = fileExists,
                    FileName = ImageItem.WebImageFileName,
                    Icon200ButtonInfostyle = icon200ButtonInfostyle
                };

                return View(iconView);
            }
            else if ((imageIcon == "webicon100" & !string.IsNullOrEmpty(ImageItem.WebIcon100FileName)))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.WebIcon100FileName));

                if (imageFile.Exists)
                {
                    fileExists = true;
                }
                else
                {
                    fileExists = false;
                }

                DetailsImageIconViewModel iconView = new()
                {
                    ImageItem = ImageItem,
                    IconType = imageIcon,
                    FileExists = fileExists,
                    FileName = ImageItem.WebImageFileName,
                    WebIcon100ButtonInfostyle = webIcon100ButtonInfostyle
                };

                return View(iconView);
            }
            else if ((imageIcon == "icon100" & !string.IsNullOrEmpty(ImageItem.Icon100FileName)))
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.Icon100FileName!));

                if (imageFile.Exists)
                {
                    fileExists = true;
                }
                else
                {
                    fileExists = false;
                }

                DetailsImageIconViewModel iconView = new()
                {
                    ImageItem = ImageItem,
                    IconType = imageIcon,
                    FileExists = fileExists,
                    FileName = ImageItem.WebImageFileName,
                    Icon100ButtonInfostyle = icon100ButtonInfostyle
                };

                return View(iconView);
            }
            else
            {
                FileInfo imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.WebImageFileName));

                if (!imageFile.Exists)
                {
                    fileExists = false;
                }

                if (!fileExists)
                {
                    imageFile = new(Path.Combine(Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", ImageItem.ImagePath)).Replace('\\', '/'), ImageItem.ImageFileName));

                    if (!imageFile.Exists)
                    {
                        fileExists = false;

                        fileName = ImageItem.ImageFileName;
                    }
                    else
                    {
                        fileExists = true;
                    }
                }

                DetailsImageIconViewModel iconView = new()
                {
                    ImageItem = ImageItem,
                    FileName = fileName,
                    FileExists = fileExists,
                    WebImageButtonInfostyle = webImageButtonInfostyle,
                    ImageButtonInfostyle = imageButtonInfostyle,
                    WebhdButtonInfostyle = webhdButtonInfostyle,
                    HdButtonInfostyle = hdButtonInfostyle,
                    Icon300ButtonInfostyle = icon300ButtonInfostyle,
                    WebIcon300ButtonInfostyle = webIcon300ButtonInfostyle,
                    Icon200ButtonInfostyle = icon200ButtonInfostyle,
                    WebIcon200ButtonInfostyle = webIcon200ButtonInfostyle,
                    Icon100ButtonInfostyle = icon100ButtonInfostyle,
                    WebIcon100ButtonInfostyle = webIcon100ButtonInfostyle
                };

                return View(iconView);
            }
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
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
        if (ModelState.IsValid)
        {
            #region Заголовок картинки

            imageItem.EditImage.ImageCaption = imageItem.EditImage.ImageCaption.Trim();

            #endregion

            #region Описание картинки

            if (string.IsNullOrEmpty(imageItem.EditImage.ImageDescription.Trim()))
            {
                imageItem.EditImage.ImageDescription = imageItem.EditImage.ImageCaption;
            }
            else
            {
                imageItem.EditImage.ImageDescription = imageItem.EditImage.ImageDescription.Trim();
            }

            #endregion

            #region Теги alt и title

            if (string.IsNullOrEmpty(imageItem.EditImage.ImageAltTitle.Trim()))
            {
                imageItem.EditImage.ImageAltTitle = char.ToLowerInvariant(imageItem.EditImage.ImageCaption[0]) + imageItem.EditImage.ImageCaption[1..];
            }
            else
            {
                imageItem.EditImage.ImageAltTitle = imageItem.EditImage.ImageAltTitle.Trim();
            }

            #endregion

            #region Фильтр поиска

            imageItem.EditImage.SearchFilter = imageItem.EditImage.SearchFilter.Trim();

            #endregion

            #region Индекс сортировки

            imageItem.EditImage.SortOfPicture = imageItem.EditImage.SortOfPicture;

            #endregion

            #region Каталог картинки

            if (imageItem.EditImage.ImagePath != "images")
            {
                imageItem.EditImage.ImagePath = Path.Combine("\\images", imageItem.EditImage.ImagePath).Replace('\\', '/');
            }
            else
            {
                imageItem.EditImage.ImagePath = "/images";
            }

            if (!string.IsNullOrEmpty(imageItem.NewImagePath))
            {
                imageItem.EditImage.ImagePath = Path.Combine("\\images", imageItem.NewImagePath).Replace('\\', '/');
            }

            string path = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", imageItem.EditImage.ImagePath)).Replace('\\', '/');

            #endregion

            #region Проверка отсутствия одинаковых файлов в IFormFile

            if (imageItem.ImageHDFormFile != null)
            {
                if (imageItem.WebImageHDFormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.WebImageHDFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebHD");
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebHD");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.ImageFormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.ImageFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Image");
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Image");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebImageFormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.WebImageFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebImage");
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebImage");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.IconFormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.IconFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon300");
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon300");
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon200FormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.Icon200FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon200");
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon100FormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.Icon100FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon100");
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon100");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.ImageFormFile != null)
            {
                if (imageItem.WebImageHDFormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.WebImageHDFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebHD");
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebHD");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.IconFormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.IconFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon300");
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon300");
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon200FormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.Icon200FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon200");
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon100FormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.Icon100FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon100");
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon100");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.IconFormFile != null)
            {
                if (imageItem.WebImageHDFormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.WebImageHDFormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebHD");
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebHD");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebImageFormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.WebImageFormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebImage");
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebImage");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon200FormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.Icon200FormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и Icon200");
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и Icon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon100FormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.Icon100FormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и Icon100");
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и Icon100");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.Icon200FormFile != null)
            {
                if (imageItem.WebImageHDFormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.WebImageHDFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebHD");
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebHD");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebImageFormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.WebImageFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebImage");
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebImage");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon300");
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon100FormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.Icon100FormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и Icon100");
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и Icon100");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.Icon100FormFile != null)
            {
                if (imageItem.WebImageHDFormFile != null)
                {
                    if (imageItem.Icon100FormFile.FileName == imageItem.WebImageHDFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebHD");
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebHD");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebImageFormFile != null)
                {
                    if (imageItem.Icon100FormFile.FileName == imageItem.WebImageFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebImage");
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebImage");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.Icon100FormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon300");
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.Icon100FormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.Icon100FormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.WebImageHDFormFile != null)
            {
                if (imageItem.WebImageFormFile != null)
                {
                    if (imageItem.WebImageHDFormFile.FileName == imageItem.WebImageFormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebImage");
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebImage");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.WebImageHDFormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon300");
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.WebImageHDFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.WebImageHDFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.WebImageFormFile != null)
            {
                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.WebImageFormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon300");
                        ModelState.AddModelError("WebIconFormFilee", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.WebImageFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFilee", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.WebImageFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFilee", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.WebIconFormFile != null)
            {
                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.WebIconFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.WebIconFormFile.FileName} для WebIcon300 и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFilee", $"выбран файл {imageItem.WebIconFormFile.FileName} для WebIcon300 и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.WebIconFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.WebIconFormFile.FileName} для WebIcon300 и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFilee", $"выбран файл {imageItem.WebIconFormFile.FileName} для WebIcon300 и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.WebIcon200FormFile != null)
            {
                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.WebIcon200FormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.WebIcon200FormFile.FileName} для WebIcon200 и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFilee", $"выбран файл {imageItem.WebIcon200FormFile.FileName} для WebIcon200 и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            #endregion

            #region Проверка ввода ImageHDFormFile

            if (imageItem.ImageHDFormFile != null)
            {
                #region Проверка наличия файла в базе данных с таким же именем

                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.ImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.ImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл картинки (HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.ImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.ImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.ImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (100 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.ImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.ImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл картинки (WebP HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.ImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.ImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.ImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (Web 100 px)");

                    return View(new EditImageViewModel());
                }

                #endregion
            }
            else
            {
                imageItem.EditImage.ImageHDFileName = string.Empty;
                imageItem.EditImage.ImageHDNameExtension = string.Empty;
                imageItem.EditImage.ImageHDMimeType = string.Empty;
                imageItem.EditImage.ImageHDFileSize = 0;
                imageItem.EditImage.ImageHDWidth = 0;
                imageItem.EditImage.ImageHDHeight = 0;
            }

            #endregion

            #region Проверка ввода ImageFormFile

            if (imageItem.ImageFormFile != null)
            {
                #region Проверка наличия файла в базе данных с таким же именем

                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.ImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.ImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл картинки (HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.ImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки (300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.ImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки (200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.ImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки (100 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.ImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.ImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл картинки (WebP HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.ImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.ImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.ImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки (Web 100 px)");

                    return View(new EditImageViewModel());
                }

                #endregion
            }
            else
            {
                imageItem.EditImage.ImageFileName = string.Empty;
                imageItem.EditImage.ImageFileNameExtension = string.Empty;
                imageItem.EditImage.ImageMimeType = string.Empty;
                imageItem.EditImage.ImageFileSize = 0;
                imageItem.EditImage.ImageWidth = 0;
                imageItem.EditImage.ImageHeight = 0;
            }

            #endregion

            #region Проверка ввода IconFormFile

            if (imageItem.IconFormFile != null)
            {
                #region Проверка наличия файла в базе данных с таким же именем

                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.IconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.IconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл картинки (HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.IconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки (300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.IconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки (200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.IconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки (100 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.IconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.IconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл картинки (WebP HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.IconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.IconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.IconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки (Web 100 px)");

                    return View(new EditImageViewModel());
                }

                #endregion
            }
            else
            {
                imageItem.EditImage.IconFileName = string.Empty;
                imageItem.EditImage.IconFileNameExtension = string.Empty;
                imageItem.EditImage.IconMimeType = string.Empty;
                imageItem.EditImage.IconFileSize = 0;
                imageItem.EditImage.IconWidth = 0;
                imageItem.EditImage.IconHeight = 0;
            }

            #endregion

            #region Проверка ввода Icon200FormFile

            if (imageItem.Icon200FormFile != null)
            {
                #region Проверка наличия файла в базе данных с таким же именем

                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.Icon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.Icon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл картинки (HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.Icon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки (300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.Icon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки (200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.Icon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки (100 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.Icon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.Icon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл картинки (WebP HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.Icon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки (WebP 300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.Icon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки (WebP 200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.Icon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки (Web 100 px)");

                    return View(new EditImageViewModel());
                }

                #endregion
            }
            else
            {
                imageItem.EditImage.Icon200FileName = string.Empty;
                imageItem.EditImage.Icon200FileNameExtension = string.Empty;
                imageItem.EditImage.Icon200MimeType = string.Empty;
                imageItem.EditImage.Icon200FileSize = 0;
                imageItem.EditImage.Icon200Width = 0;
                imageItem.EditImage.Icon200Height = 0;
            }

            #endregion

            #region Проверка ввода Icon100FormFile

            if (imageItem.Icon100FormFile != null)
            {
                #region Проверка наличия файла в базе данных с таким же именем

                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.Icon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.Icon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл картинки (HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.Icon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки (300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.Icon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки (200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.Icon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки (100 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.Icon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.Icon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл картинки (WebP HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.Icon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки (WebP 300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.Icon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки (WebP 200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.Icon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки (Web 100 px)");

                    return View(new EditImageViewModel());
                }

                #endregion
            }
            else
            {
                imageItem.EditImage.Icon100FileName = string.Empty;
                imageItem.EditImage.Icon100FileNameExtension = string.Empty;
                imageItem.EditImage.Icon100MimeType = string.Empty;
                imageItem.EditImage.Icon100FileSize = 0;
                imageItem.EditImage.Icon100Width = 0;
                imageItem.EditImage.Icon100Height = 0;
            }

            #endregion

            #region Проверка ввода WebImageHDFormFile

            if (imageItem.WebImageHDFormFile != null)
            {
                #region Проверка наличия файла в базе данных с таким же именем

                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.WebImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.WebImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл картинки (HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.WebImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.WebImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.WebImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (100 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.WebImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.WebImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл картинки (WebP HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.WebImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.WebImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.WebImageHDFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (Web 100 px)");

                    return View(new EditImageViewModel());
                }

                #endregion

                if (!imageItem.WebImageHDFormFile.FileName.Contains("webp", StringComparison.CurrentCultureIgnoreCase))
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» должен иметь расширение «webp»");

                    return View(new EditImageViewModel());
                }
            }
            else
            {
                imageItem.EditImage.WebImageHDFileName = string.Empty;
                imageItem.EditImage.WebImageHDNameExtension = string.Empty;
                imageItem.EditImage.WebImageHDMimeType = string.Empty;
                imageItem.EditImage.WebImageHDFileSize = 0;
                imageItem.EditImage.WebImageHDWidth = 0;
                imageItem.EditImage.WebImageHDHeight = 0;
            }

            #endregion

            #region Проверка ввода WebImageFormFile

            if (imageItem.WebImageFormFile != null)
            {
                #region Проверка наличия файла в базе данных с таким же именем

                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.WebImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.WebImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл картинки (HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.WebImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки (300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.WebImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки (200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.WebImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки (100 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.WebImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.WebImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл картинки (WebP HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.WebImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.WebImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.WebImageFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки (Web 100 px)");

                    return View(new EditImageViewModel());
                }

                #endregion

                if (!imageItem.WebImageFormFile.FileName.Contains("webp", StringComparison.CurrentCultureIgnoreCase))
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» должен иметь расширение «webp»");

                    return View(new EditImageViewModel());
                }
            }
            else
            {
                ModelState.AddModelError("WebImageFormFile", "Выберите файл картинки (WebP)");

                return View(new EditImageViewModel());
            }

            #endregion

            #region Проверка ввода WebIconFormFile

            if (imageItem.WebIconFormFile != null)
            {
                #region Проверка наличия файла в базе данных с таким же именем

                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.WebIconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.WebIconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл картинки (HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.WebIconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки (300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.WebIconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки (200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.WebIconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки (100 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.WebIconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.WebIconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл картинки (WebP HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.WebIconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.WebIconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки (WebP 200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.WebIconFormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки (Web 100 px)");

                    return View(new EditImageViewModel());
                }

                #endregion

                if (!imageItem.WebIconFormFile.FileName.Contains("webp", StringComparison.CurrentCultureIgnoreCase))
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» должен иметь расширение «webp»");

                    return View(new EditImageViewModel());
                }
            }
            else
            {
                imageItem.EditImage.WebIconFileName = string.Empty;
                imageItem.EditImage.WebIconFileNameExtension = string.Empty;
                imageItem.EditImage.WebIconMimeType = string.Empty;
                imageItem.EditImage.WebIconFileSize = 0;
                imageItem.EditImage.WebIconWidth = 0;
                imageItem.EditImage.WebIconHeight = 0;
            }

            #endregion

            #region Проверка ввода WebIcon200FormFile

            if (imageItem.WebIcon200FormFile != null)
            {
                #region Проверка наличия файла в базе данных с таким же именем

                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.WebIcon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.WebIcon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл картинки (HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.WebIcon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки (300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.WebIcon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки (200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.WebIcon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки (100 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.WebIcon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.WebIcon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл картинки (WebP HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.WebIcon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки (WebP 300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.WebIcon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки (WebP 200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.WebIcon200FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки (Web 100 px)");

                    return View(new EditImageViewModel());
                }

                #endregion

                if (!imageItem.WebIcon200FormFile.FileName.Contains("webp", StringComparison.CurrentCultureIgnoreCase))
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» должен иметь расширение «webp»");

                    return View(new EditImageViewModel());
                }
            }
            else
            {
                imageItem.EditImage.WebIcon200FileName = string.Empty;
                imageItem.EditImage.WebIcon200FileNameExtension = string.Empty;
                imageItem.EditImage.WebIcon200MimeType = string.Empty;
                imageItem.EditImage.WebIcon200FileSize = 0;
                imageItem.EditImage.WebIcon200Width = 0;
                imageItem.EditImage.WebIcon200Height = 0;
            }

            #endregion

            #region Проверка ввода WebIcon100FormFile

            if (imageItem.WebIcon100FormFile != null)
            {
                #region Проверка наличия файла в базе данных с таким же именем

                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.WebIcon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.WebIcon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл картинки (HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.WebIcon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки (300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.WebIcon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки (200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.WebIcon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки (100 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.WebIcon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.WebIcon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл картинки (WebP HD)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.WebIcon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки (WebP 300 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.WebIcon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки (WebP 200 px)");

                    return View(new EditImageViewModel());
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.WebIcon100FormFile.FileName).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки (Web 100 px)");

                    return View(new EditImageViewModel());
                }

                #endregion

                if (!imageItem.WebIcon100FormFile.FileName.Contains("webp", StringComparison.CurrentCultureIgnoreCase))
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» должен иметь расширение «webp»");

                    return View(new EditImageViewModel());
                }
            }
            else
            {
                imageItem.EditImage.WebIcon100FileName = string.Empty;
                imageItem.EditImage.WebIcon100FileNameExtension = string.Empty;
                imageItem.EditImage.WebIcon100MimeType = string.Empty;
                imageItem.EditImage.WebIcon100FileSize = 0;
                imageItem.EditImage.WebIcon100Width = 0;
                imageItem.EditImage.WebIcon100Height = 0;
            }

            #endregion

            #region Создание нового каталога

            if (System.IO.Directory.Exists(path) & imageItem.NewImagePath != string.Empty)
            {
                ModelState.AddModelError("NewImagePath", $"Каталог «{imageItem.NewImagePath}» уже существует");

                return View(new EditImageViewModel());
            }

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            #endregion

            #region Запись файлов в каталог

            if (imageItem.ImageHDFormFile != null)
            {
                FileInfo hdImage = new(Path.Combine(path, imageItem.ImageHDFormFile.FileName));

                if (!hdImage.Exists)
                {
                    string hdImageFormFileCopyPath = Path.Combine(path, imageItem.ImageHDFormFile.FileName).Replace('\\', '/');

                    using FileStream stream = new(hdImageFormFileCopyPath, FileMode.Create);
                    await imageItem.ImageHDFormFile.CopyToAsync(stream);
                }

                #region AutoFillForHD

                IReadOnlyList<MetadataExtractor.Directory> hdDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.ImageHDFormFile.FileName));

                foreach (var imageDirectory in hdDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить имя файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageHDFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить расширение файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageHDNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить MIME файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageHDMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить размер файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageHDFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить ширину файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageHDWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить высоту файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageHDHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }

            if (imageItem.ImageFormFile != null)
            {
                FileInfo imgImage = new(Path.Combine(path, imageItem.ImageFormFile.FileName));

                if (!imgImage.Exists)
                {
                    string imgImageFormFileCopyPath = Path.Combine(path, imageItem.ImageFormFile.FileName).Replace('\\', '/');

                    using FileStream stream = new(imgImageFormFileCopyPath, FileMode.Create);
                    await imageItem.ImageFormFile.CopyToAsync(stream);
                }

                #region AutoFillForImage

                IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.ImageFormFile.FileName));

                foreach (var imageDirectory in imageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить имя файла «{imageItem.ImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить расширение файла «{imageItem.ImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageFileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить MIME файла «{imageItem.ImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить размер файла «{imageItem.ImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить ширину файла «{imageItem.ImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить высоту файла «{imageItem.ImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.ImageHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }

            if (imageItem.IconFormFile != null)
            {
                FileInfo iconImage = new(Path.Combine(path, imageItem.IconFormFile.FileName));

                if (!iconImage.Exists)
                {
                    string iconImageFormFileCopyPath = Path.Combine(path, imageItem.IconFormFile.FileName).Replace('\\', '/');

                    using FileStream stream = new(iconImageFormFileCopyPath, FileMode.Create);
                    await imageItem.IconFormFile.CopyToAsync(stream);
                }

                #region AutoFillForIcon300

                IReadOnlyList<MetadataExtractor.Directory> iconDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.IconFormFile.FileName));

                foreach (var imageDirectory in iconDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить имя файла «{imageItem.IconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.IconFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить расширение файла «{imageItem.IconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.IconFileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить MIME файла «{imageItem.IconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.IconMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить размер файла «{imageItem.IconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.IconFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить ширину файла «{imageItem.IconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.IconWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить высоту файла «{imageItem.IconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.IconHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }

            if (imageItem.Icon200FormFile != null)
            {
                FileInfo icon200Image = new(Path.Combine(path, imageItem.Icon200FormFile.FileName));

                if (!icon200Image.Exists)
                {
                    string icon200ImageFormFileCopyPath = Path.Combine(path, imageItem.Icon200FormFile.FileName).Replace('\\', '/');

                    using FileStream stream = new(icon200ImageFormFileCopyPath, FileMode.Create);
                    await imageItem.Icon200FormFile.CopyToAsync(stream);
                }

                #region AutoFillForIcon200

                IReadOnlyList<MetadataExtractor.Directory> icon200Directories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.Icon200FormFile.FileName));

                foreach (var imageDirectory in icon200Directories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить имя файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon200FileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить расширение файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon200FileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить MIME файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon200MimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить размер файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon200FileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить ширину файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon200Width = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить высоту файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon200Height = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }

            if (imageItem.Icon100FormFile != null)
            {
                FileInfo icon100Image = new(Path.Combine(path, imageItem.Icon100FormFile.FileName));

                if (!icon100Image.Exists)
                {
                    string icon100ImageFormFileCopyPath = Path.Combine(path, imageItem.Icon100FormFile.FileName).Replace('\\', '/');

                    using FileStream stream = new(icon100ImageFormFileCopyPath, FileMode.Create);
                    await imageItem.Icon100FormFile.CopyToAsync(stream);
                }

                #region AutoFillForIcon100

                IReadOnlyList<MetadataExtractor.Directory> icon100Directories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.Icon100FormFile.FileName));

                foreach (var imageDirectory in icon100Directories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить имя файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon100FileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить расширение файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon100FileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить MIME файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon100MimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить размер файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon100FileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить ширину файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon100Width = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить высоту файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.Icon100Height = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }

            if (imageItem.WebImageHDFormFile != null)
            {
                FileInfo webHdImage = new(Path.Combine(path, imageItem.WebImageHDFormFile.FileName));

                if (!webHdImage.Exists)
                {
                    string webHdImageFormFileCopyPath = Path.Combine(path, imageItem.WebImageHDFormFile.FileName).Replace('\\', '/');

                    using FileStream stream = new(webHdImageFormFileCopyPath, FileMode.Create);
                    await imageItem.WebImageHDFormFile.CopyToAsync(stream);
                }

                #region AutoFillForWebHD

                IReadOnlyList<MetadataExtractor.Directory> webHdDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.WebImageHDFormFile.FileName));

                foreach (var imageDirectory in webHdDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить имя файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageHDFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить расширение файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageHDNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить MIME файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageHDMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить размер файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageHDFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить ширину файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageHDWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить высоту файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageHDHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }

            if (imageItem.WebImageFormFile != null)
            {
                FileInfo webImage = new(Path.Combine(path, imageItem.WebImageFormFile.FileName));

                if (!webImage.Exists)
                {
                    string webImageFormFileCopyPath = Path.Combine(path, imageItem.WebImageFormFile.FileName).Replace('\\', '/');

                    using FileStream stream = new(webImageFormFileCopyPath, FileMode.Create);
                    await imageItem.WebImageFormFile.CopyToAsync(stream);
                }

                #region AutoFillForWebImage

                IReadOnlyList<MetadataExtractor.Directory> webImageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.WebImageFormFile.FileName));

                foreach (var imageDirectory in webImageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить имя файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить расширение файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageFileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить MIME файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить размер файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить ширину файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить высоту файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebImageHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }

            if (imageItem.WebIconFormFile != null)
            {
                FileInfo webIcon = new(Path.Combine(path, imageItem.WebIconFormFile.FileName));

                if (!webIcon.Exists)
                {
                    string webIconFormFileCopyPath = Path.Combine(path, imageItem.WebIconFormFile.FileName).Replace('\\', '/');

                    using FileStream stream = new(webIconFormFileCopyPath, FileMode.Create);
                    await imageItem.WebIconFormFile.CopyToAsync(stream);
                }

                #region AutoFillForWebIcon300

                IReadOnlyList<MetadataExtractor.Directory> webIconDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.WebIconFormFile.FileName));

                foreach (var imageDirectory in webIconDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIconFormFile", $"Не определить имя файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIconFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIconFormFile", $"Не определить расширение файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIconFileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIconFormFile", $"Не определить MIME файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIconMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("WebIconFormFilee", $"Не определить размер файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIconFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIconFormFile", $"Не определить ширину файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIconWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIconFormFile", $"Не определить высоту файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIconHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }

            if (imageItem.WebIcon200FormFile != null)
            {
                FileInfo webIcon200 = new(Path.Combine(path, imageItem.WebIcon200FormFile.FileName));

                if (!webIcon200.Exists)
                {
                    string webIcon200FormFileCopyPath = Path.Combine(path, imageItem.WebIcon200FormFile.FileName).Replace('\\', '/');

                    using FileStream stream = new(webIcon200FormFileCopyPath, FileMode.Create);
                    await imageItem.WebIcon200FormFile.CopyToAsync(stream);
                }

                #region AutoFillForWebIcon200

                IReadOnlyList<MetadataExtractor.Directory> webIcon200Directories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.WebIcon200FormFile.FileName));

                foreach (var imageDirectory in webIcon200Directories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить имя файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon200FileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить расширение файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon200FileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить MIME файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon200MimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить размер файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon200FileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить ширину файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon200Width = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить высоту файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon200Height = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }

            if (imageItem.WebIcon100FormFile != null)
            {
                FileInfo webIcon100 = new(Path.Combine(path, imageItem.WebIcon100FormFile.FileName));

                if (!webIcon100.Exists)
                {
                    string webIcon100FormFileCopyPath = Path.Combine(path, imageItem.WebIcon100FormFile.FileName).Replace('\\', '/');

                    using FileStream stream = new(webIcon100FormFileCopyPath, FileMode.Create);
                    await imageItem.WebIcon100FormFile.CopyToAsync(stream);
                }

                #region AutoFillForWebIcon100

                IReadOnlyList<MetadataExtractor.Directory> webIcon100Directories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.WebIcon100FormFile.FileName));

                foreach (var imageDirectory in webIcon100Directories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon100FormFile", $"Не определить имя файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon100FileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon100FormFile", $"Не определить расширение файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon100FileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon100FormFile", $"Не определить MIME файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon100MimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("WebIcon100FormFile", $"Не определить размер файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon100FileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIcon100FormFile", $"Не определить ширину файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon100Width = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIcon100FormFile", $"Не определить высоту файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(new EditImageViewModel());
                            }

                            imageItem.EditImage.WebIcon100Height = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }

            #endregion

            await imageContext.AddNewImageAsync(imageItem.EditImage);

            return RedirectToAction(nameof(DetailsImage), new { imageId = imageItem.EditImage.ImageFileModelId, Area = "Admin" });
        }
        else
        {
            return View(new EditImageViewModel());
        }
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
        if (ModelState.IsValid)
        {
            ImageFileModel imageUpdate = await imageContext.ImageFiles.FirstAsync(img => img.ImageFileModelId == imageItem.EditImage.ImageFileModelId);

            #region Заголовок картинки

            imageUpdate.ImageCaption = imageItem.EditImage.ImageCaption.Trim();

            #endregion

            #region Описание картинки

            imageUpdate.ImageDescription = imageItem.EditImage.ImageDescription.Trim();

            #endregion

            #region Теги alt и title

            imageUpdate.ImageAltTitle = imageItem.EditImage.ImageAltTitle.Trim();

            #endregion

            #region Фильтр поиска

            imageUpdate.SearchFilter = imageItem.EditImage.SearchFilter.Trim();

            #endregion

            #region Индекс сортировки

            imageUpdate.SortOfPicture = imageItem.EditImage.SortOfPicture;

            #endregion

            #region Каталог картинки

            string oldPath = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", imageUpdate.ImagePath));

            if (imageUpdate.ImagePath != Path.Combine("\\images", imageItem.EditImage.ImagePath).Replace('\\', '/'))
            {
                imageUpdate.ImagePath = Path.Combine("\\images", imageItem.EditImage.ImagePath).Replace('\\', '/');
            }

            if (!string.IsNullOrEmpty(imageItem.NewImagePath))
            {
                imageUpdate.ImagePath = Path.Combine("\\images", imageItem.NewImagePath).Replace('\\', '/');
            }

            string path = Path.GetFullPath(Path.Join(System.IO.Directory.GetCurrentDirectory(), "wwwroot", imageUpdate.ImagePath));

            #region Перенос файлов картинок в новый каталог

            if (oldPath != path)
            {
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                if (!string.IsNullOrEmpty(imageUpdate.ImageHDFileName))
                {
                    FileInfo oldFileInfo = new(Path.Combine(oldPath, imageUpdate.ImageHDFileName));

                    FileInfo FileInfo = new(Path.Combine(path, imageUpdate.ImageHDFileName));

                    if (oldFileInfo.Exists)
                    {
                        System.IO.File.Move(oldFileInfo.FullName, FileInfo.FullName, true);
                    }
                }

                if (!string.IsNullOrEmpty(imageUpdate.ImageFileName))
                {
                    FileInfo oldFileInfo = new(Path.Combine(oldPath, imageUpdate.ImageFileName));

                    FileInfo FileInfo = new(Path.Combine(path, imageUpdate.ImageFileName));

                    if (oldFileInfo.Exists)
                    {
                        System.IO.File.Move(oldFileInfo.FullName, FileInfo.FullName, true);
                    }
                }

                if (!string.IsNullOrEmpty(imageUpdate.IconFileName))
                {
                    FileInfo oldFileInfo = new(Path.Combine(oldPath, imageUpdate.IconFileName));

                    FileInfo FileInfo = new(Path.Combine(path, imageUpdate.IconFileName));

                    if (oldFileInfo.Exists)
                    {
                        System.IO.File.Move(oldFileInfo.FullName, FileInfo.FullName, true);
                    }
                }

                if (!string.IsNullOrEmpty(imageUpdate.Icon200FileName))
                {
                    FileInfo oldFileInfo = new(Path.Combine(oldPath, imageUpdate.Icon200FileName));

                    FileInfo FileInfo = new(Path.Combine(path, imageUpdate.Icon200FileName));

                    if (oldFileInfo.Exists)
                    {
                        System.IO.File.Move(oldFileInfo.FullName, FileInfo.FullName, true);
                    }
                }

                if (!string.IsNullOrEmpty(imageUpdate.Icon100FileName))
                {
                    FileInfo oldFileInfo = new(Path.Combine(oldPath, imageUpdate.Icon100FileName));

                    FileInfo FileInfo = new(Path.Combine(path, imageUpdate.Icon100FileName));

                    if (oldFileInfo.Exists)
                    {
                        System.IO.File.Move(oldFileInfo.FullName, FileInfo.FullName, true);
                    }
                }

                if (!string.IsNullOrEmpty(imageUpdate.WebImageHDFileName))
                {
                    FileInfo oldFileInfo = new(Path.Combine(oldPath, imageUpdate.WebImageHDFileName));

                    FileInfo FileInfo = new(Path.Combine(path, imageUpdate.WebImageHDFileName));

                    if (oldFileInfo.Exists)
                    {
                        System.IO.File.Move(oldFileInfo.FullName, FileInfo.FullName, true);
                    }
                }

                if (!string.IsNullOrEmpty(imageUpdate.WebImageFileName))
                {
                    FileInfo oldFileInfo = new(Path.Combine(oldPath, imageUpdate.WebImageFileName));

                    FileInfo FileInfo = new(Path.Combine(path, imageUpdate.WebImageFileName));

                    if (oldFileInfo.Exists)
                    {
                        System.IO.File.Move(oldFileInfo.FullName, FileInfo.FullName, true);
                    }
                }

                if (!string.IsNullOrEmpty(imageUpdate.WebIconFileName))
                {
                    FileInfo oldFileInfo = new(Path.Combine(oldPath, imageUpdate.WebIconFileName));

                    FileInfo FileInfo = new(Path.Combine(path, imageUpdate.WebIconFileName));

                    if (oldFileInfo.Exists)
                    {
                        System.IO.File.Move(oldFileInfo.FullName, FileInfo.FullName, true);
                    }
                }

                if (!string.IsNullOrEmpty(imageUpdate.WebIcon200FileName))
                {
                    FileInfo oldFileInfo = new(Path.Combine(oldPath, imageUpdate.WebIcon200FileName));

                    FileInfo FileInfo = new(Path.Combine(path, imageUpdate.WebIcon200FileName));

                    if (oldFileInfo.Exists)
                    {
                        System.IO.File.Move(oldFileInfo.FullName, FileInfo.FullName, true);
                    }
                }

                if (!string.IsNullOrEmpty(imageUpdate.WebIcon100FileName))
                {
                    FileInfo oldFileInfo = new(Path.Combine(oldPath, imageUpdate.WebIcon100FileName));

                    FileInfo FileInfo = new(Path.Combine(path, imageUpdate.WebIcon100FileName));

                    if (oldFileInfo.Exists)
                    {
                        System.IO.File.Move(oldFileInfo.FullName, FileInfo.FullName, true);
                    }
                }
            }

            #endregion

            #endregion

            #region Проверка отсутствия одинаковых файлов в IFormFile

            if (imageItem.ImageHDFormFile != null)
            {
                if (imageItem.WebImageHDFormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.WebImageHDFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebHD");
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebHD");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.ImageFormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.ImageFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Image");
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Image");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebImageFormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.WebImageFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebImage");
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebImage");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.IconFormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.IconFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon300");
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon300");
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon200FormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.Icon200FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon200");
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon100FormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.Icon100FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon100");
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и Icon100");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.ImageHDFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.ImageHDFormFile.FileName} для HD и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.ImageFormFile != null)
            {
                if (imageItem.WebImageHDFormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.WebImageHDFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebHD");
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebHD");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.IconFormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.IconFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon300");
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon300");
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon200FormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.Icon200FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon200");
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon100FormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.Icon100FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon100");
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и Icon100");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.ImageFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("ImageFormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.ImageFormFile.FileName} для Image и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.IconFormFile != null)
            {
                if (imageItem.WebImageHDFormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.WebImageHDFormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebHD");
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebHD");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebImageFormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.WebImageFormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebImage");
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebImage");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon200FormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.Icon200FormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и Icon200");
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и Icon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon100FormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.Icon100FormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и Icon100");
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и Icon100");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.IconFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("IconFormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.IconFormFile.FileName} для Icon300 и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.Icon200FormFile != null)
            {
                if (imageItem.WebImageHDFormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.WebImageHDFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebHD");
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebHD");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebImageFormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.WebImageFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebImage");
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebImage");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon300");
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.Icon100FormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.Icon100FormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и Icon100");
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и Icon100");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.Icon200FormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.Icon200FormFile.FileName} для Icon200 и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.Icon100FormFile != null)
            {
                if (imageItem.WebImageHDFormFile != null)
                {
                    if (imageItem.Icon100FormFile.FileName == imageItem.WebImageHDFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebHD");
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebHD");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebImageFormFile != null)
                {
                    if (imageItem.Icon100FormFile.FileName == imageItem.WebImageFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebImage");
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebImage");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.Icon100FormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon300");
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.Icon100FormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.Icon100FormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("Icon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.Icon100FormFile.FileName} для Icon100 и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.WebImageHDFormFile != null)
            {
                if (imageItem.WebImageFormFile != null)
                {
                    if (imageItem.WebImageHDFormFile.FileName == imageItem.WebImageFormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebImage");
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebImage");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.WebImageHDFormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon300");
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.WebImageHDFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.WebImageHDFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageHDFormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFile", $"выбран файл {imageItem.WebImageHDFormFile.FileName} для WebHD и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.WebImageFormFile != null)
            {
                if (imageItem.WebIconFormFile != null)
                {
                    if (imageItem.WebImageFormFile.FileName == imageItem.WebIconFormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon300");
                        ModelState.AddModelError("WebIconFormFilee", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon300");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.WebImageFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFilee", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.WebImageFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("WebImageFormFile", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFilee", $"выбран файл {imageItem.WebImageFormFile.FileName} для WebImage и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.WebIconFormFile != null)
            {
                if (imageItem.WebIcon200FormFile != null)
                {
                    if (imageItem.WebIconFormFile.FileName == imageItem.WebIcon200FormFile.FileName)
                    {
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.WebIconFormFile.FileName} для WebIcon300 и WebIcon200");
                        ModelState.AddModelError("WebIcon200FormFilee", $"выбран файл {imageItem.WebIconFormFile.FileName} для WebIcon300 и WebIcon200");

                        return View(new EditImageViewModel());
                    }
                }

                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.WebIconFormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("WebIconFormFile", $"выбран файл {imageItem.WebIconFormFile.FileName} для WebIcon300 и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFilee", $"выбран файл {imageItem.WebIconFormFile.FileName} для WebIcon300 и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            if (imageItem.WebIcon200FormFile != null)
            {
                if (imageItem.WebIcon100FormFile != null)
                {
                    if (imageItem.WebIcon200FormFile.FileName == imageItem.WebIcon100FormFile.FileName)
                    {
                        ModelState.AddModelError("WebIcon200FormFile", $"выбран файл {imageItem.WebIcon200FormFile.FileName} для WebIcon200 и WebIcon100");
                        ModelState.AddModelError("WebIcon100FormFilee", $"выбран файл {imageItem.WebIcon200FormFile.FileName} для WebIcon200 и WebIcon100");

                        return View(new EditImageViewModel());
                    }
                }
            }

            #endregion

            #region Проверка ввода ImageHDFormFile

            if (imageItem.ImageHDFormFile != null)
            {
                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.ImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.ImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл картинки HD");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.ImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.ImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.ImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки 100 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.ImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.ImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл картинки HD (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.ImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.ImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.ImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageHDFormFile", $"Файл «{imageItem.ImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 100 px");

                    return View(imageItem);
                }
            }

            #endregion

            #region Проверка ввода ImageFormFile

            if (imageItem.ImageFormFile != null)
            {
                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.ImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.ImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл картинки HD");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.ImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.ImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.ImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки 100 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.ImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.ImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл картинки HD (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.ImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.ImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.ImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("ImageFormFile", $"Файл «{imageItem.ImageFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 100 px");

                    return View(imageItem);
                }
            }

            #endregion

            #region Проверка ввода IconFormFile

            if (imageItem.IconFormFile != null)
            {
                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.IconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.IconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл картинки HD");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.IconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.IconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.IconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки 100 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.IconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.IconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл картинки HD (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.IconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.IconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.IconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("IconFormFile", $"Файл «{imageItem.IconFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 100 px");

                    return View(imageItem);
                }
            }

            #endregion

            #region Проверка ввода Icon200FormFile

            if (imageItem.Icon200FormFile != null)
            {
                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.Icon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.Icon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл картинки HD");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.Icon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.Icon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.Icon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки 100 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.Icon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.Icon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл картинки HD (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.Icon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.Icon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.Icon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon200FormFile", $"Файл «{imageItem.Icon200FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 100 px");

                    return View(imageItem);
                }
            }

            #endregion

            #region Проверка ввода Icon100FormFile

            if (imageItem.Icon100FormFile != null)
            {
                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.Icon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.Icon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл картинки HD");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.Icon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.Icon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.Icon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки 100 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.Icon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.Icon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл картинки HD (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.Icon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.Icon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.Icon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("Icon100FormFile", $"Файл «{imageItem.Icon100FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 100 px");

                    return View(imageItem);
                }
            }

            #endregion

            #region Проверка ввода WebImageHDFormFile

            if (imageItem.WebImageHDFormFile != null)
            {
                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.WebImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.WebImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл картинки HD");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.WebImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.WebImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.WebImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки 100 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.WebImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.WebImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл картинки HD (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.WebImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.WebImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.WebImageHDFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageHDFormFile", $"Файл «{imageItem.WebImageHDFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 100 px");

                    return View(imageItem);
                }
            }

            #endregion

            #region Проверка ввода WebImageFormFile

            if (imageItem.WebImageFormFile != null)
            {
                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.WebImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.WebImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл картинки HD");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.WebImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.WebImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.WebImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки 100 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.WebImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.WebImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл картинки HD (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.WebImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.WebImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.WebImageFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebImageFormFile", $"Файл «{imageItem.WebImageFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 100 px");

                    return View(imageItem);
                }
            }

            #endregion

            #region Проверка ввода WebIconFormFile

            if (imageItem.WebIconFormFile != null)
            {
                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.WebIconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.WebIconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл картинки HD");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.WebIconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.WebIconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.WebIconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки 100 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.WebIconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.WebIconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл картинки HD (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.WebIconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.WebIconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.WebIconFormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIconFormFile", $"Файл «{imageItem.WebIconFormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 100 px");

                    return View(imageItem);
                }
            }

            #endregion

            #region Проверка ввода WebIcon200FormFile

            if (imageItem.WebIcon200FormFile != null)
            {
                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.WebIcon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.WebIcon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл картинки HD");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.WebIcon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.WebIcon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.WebIcon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки 100 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.WebIcon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.WebIcon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл картинки HD (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.WebIcon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.WebIcon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.WebIcon200FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon200FormFile", $"Файл «{imageItem.WebIcon200FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 100 px");

                    return View(imageItem);
                }
            }

            #endregion

            #region Проверка ввода WebIcon100FormFile

            if (imageItem.WebIcon100FormFile != null)
            {
                if (await imageContext.ImageFiles.Where(x => x.ImageFileName == imageItem.WebIcon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл картинки");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.ImageHDFileName == imageItem.WebIcon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл картинки HD");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.IconFileName == imageItem.WebIcon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon200FileName == imageItem.WebIcon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.Icon100FileName == imageItem.WebIcon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки 100 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageFileName == imageItem.WebIcon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл картинки (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebImageHDFileName == imageItem.WebIcon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл картинки HD (WebP)");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIconFileName == imageItem.WebIcon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 300 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon200FileName == imageItem.WebIcon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 200 px");

                    return View(imageItem);
                }

                if (await imageContext.ImageFiles.Where(x => x.WebIcon100FileName == imageItem.WebIcon100FormFile.FileName & x.ImageFileModelId != imageItem.EditImage.ImageFileModelId).AnyAsync())
                {
                    ModelState.AddModelError("WebIcon100FormFile", $"Файл «{imageItem.WebIcon100FormFile.FileName}» уже существует в базе данных как файл иконки (WebP) 100 px");

                    return View(imageItem);
                }
            }

            #endregion

            #region Edit ImageHD

            if (imageItem.ImageHDFormFile != null)
            {
                if (!string.IsNullOrEmpty(imageItem.EditImage.ImageHDFileName))
                {
                    if (imageItem.ImageHDFormFile.FileName[(imageItem.ImageHDFormFile.FileName.IndexOf('.') + 1)..] != imageItem.EditImage.ImageHDNameExtension)
                    {
                        ModelState.AddModelError("ImageHDFormFile", $"Расширение файла должно быть «.{imageItem.EditImage.ImageHDNameExtension}»");

                        return View(imageItem);
                    }

                    FileInfo oldFileInfo = new(Path.Combine(path, imageItem.EditImage.ImageHDFileName));

                    if (oldFileInfo.Exists)
                    {
                        string newFullNameForOldFile = @"D:\TEMP\" + imageItem.EditImage.ImageHDFileName[..imageItem.EditImage.ImageHDFileName.IndexOf('.')] + "-save-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "." + imageItem.EditImage.ImageHDNameExtension;

                        System.IO.File.Move(oldFileInfo.FullName, newFullNameForOldFile);
                    }

                    string imageFormFileCopyPath = Path.Combine(path, imageItem.ImageHDFormFile.FileName);

                    using (FileStream stream = new(imageFormFileCopyPath, FileMode.Create))
                    {
                        await imageItem.ImageHDFormFile.CopyToAsync(stream);
                    }

                    FileInfo newFileInfo = new(Path.Combine(path, imageItem.ImageHDFormFile.FileName));

                    if (imageItem.ImageHDFormFile.FileName != imageItem.EditImage.ImageFileName)
                    {
                        System.IO.File.Move(newFileInfo.FullName, oldFileInfo.FullName);
                    }
                }
                else
                {
                    string imageFormFileCopyPath = Path.Combine(path, imageItem.ImageHDFormFile.FileName);

                    using var stream = new FileStream(imageFormFileCopyPath, FileMode.Create);
                    await imageItem.ImageHDFormFile.CopyToAsync(stream);

                    imageItem.EditImage.ImageHDFileName = imageItem.ImageHDFormFile.FileName;
                }

                #region AutoFillForHD

                IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.EditImage.ImageHDFileName));

                foreach (var imageDirectory in imageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить имя файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageHDFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить расширение файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageHDNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить MIME файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageHDMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить размер файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageHDFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить ширину файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageHDWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("ImageHDFormFile", $"Не определить высоту файла «{imageItem.ImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageHDHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }
            else
            {
                imageUpdate.ImageHDFileName = imageItem.EditImage.ImageHDFileName;
                imageUpdate.ImageHDFileSize = imageItem.EditImage.ImageHDFileSize;
                imageUpdate.ImageHDWidth = imageItem.EditImage.ImageHDWidth;
                imageUpdate.ImageHDHeight = imageItem.EditImage.ImageHDHeight;
                imageUpdate.ImageHDMimeType = imageItem.EditImage.ImageHDMimeType;
                imageUpdate.ImageHDNameExtension = imageItem.EditImage.ImageHDNameExtension;
            }

            #endregion

            #region Edit ImageFile

            if (imageItem.ImageFormFile != null)
            {
                if (!string.IsNullOrEmpty(imageItem.EditImage.ImageFileName))
                {
                    if (imageItem.ImageFormFile.FileName.ToString()[(imageItem.ImageFormFile.FileName.ToString().IndexOf('.') + 1)..] != imageItem.EditImage.ImageFileNameExtension)
                    {
                        ModelState.AddModelError("ImageFormFile", $"Расширение файла должно быть «.{imageItem.EditImage.ImageFileNameExtension}»");

                        return View(nameof(EditImage), imageItem);
                    }

                    FileInfo oldFileInfo = new(Path.Combine(path, imageItem.EditImage.ImageFileName));

                    if (oldFileInfo.Exists)
                    {
                        string newFullNameForOldFile = @"D:\TEMP\" + imageItem.EditImage.ImageFileName[..imageItem.EditImage.ImageFileName.IndexOf('.')] + "-save-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "." + imageItem.EditImage.ImageFileNameExtension;

                        System.IO.File.Move(oldFileInfo.FullName, newFullNameForOldFile);
                    }

                    string imageFormFileCopyPath = Path.Combine(path, imageItem.ImageFormFile.FileName);

                    using (FileStream stream = new(imageFormFileCopyPath, FileMode.Create))
                    {
                        await imageItem.ImageFormFile.CopyToAsync(stream);
                    }

                    FileInfo newFileInfo = new(Path.Combine(path, imageItem.ImageFormFile.FileName));

                    if (imageItem.ImageFormFile.FileName != imageItem.EditImage.ImageFileName)
                    {
                        System.IO.File.Move(newFileInfo.FullName, oldFileInfo.FullName);
                    }
                }
                else
                {
                    string imageFormFileCopyPath = Path.Combine(path, imageItem.ImageFormFile.FileName);

                    using var stream = new FileStream(imageFormFileCopyPath, FileMode.Create);
                    await imageItem.ImageFormFile.CopyToAsync(stream);

                    imageItem.EditImage.ImageFileName = imageItem.ImageFormFile.FileName;
                }

                #region AutoFillForImageFile

                IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.EditImage.ImageFileName));

                foreach (var imageDirectory in imageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить имя файла «{imageItem.ImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить расширение файла «{imageItem.ImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageFileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить MIME файла «{imageItem.ImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить размер файла «{imageItem.ImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить ширину файла «{imageItem.ImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("ImageFormFile", $"Не определить высоту файла «{imageItem.ImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.ImageHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }
            else
            {
                imageUpdate.ImageFileName = imageItem.EditImage.ImageFileName;
                imageUpdate.ImageFileSize = imageItem.EditImage.ImageFileSize;
                imageUpdate.ImageWidth = imageItem.EditImage.ImageWidth;
                imageUpdate.ImageHeight = imageItem.EditImage.ImageHeight;
                imageUpdate.ImageMimeType = imageItem.EditImage.ImageMimeType;
                imageUpdate.ImageFileNameExtension = imageItem.EditImage.ImageFileNameExtension;
            }

            #endregion

            #region Edit IconFile (Icon300)

            if (imageItem.IconFormFile != null)
            {
                if (!string.IsNullOrEmpty(imageItem.EditImage.IconFileName))
                {
                    if (imageItem.IconFormFile.FileName.ToString()[(imageItem.IconFormFile.FileName.ToString().IndexOf('.') + 1)..] != imageItem.EditImage.IconFileNameExtension)
                    {
                        ModelState.AddModelError("IconFormFile", $"Расширение файла должно быть «.{imageItem.EditImage.IconFileNameExtension}»");

                        return View(nameof(EditImage), imageItem);
                    }

                    FileInfo oldFileInfo = new(Path.Combine(path, imageItem.EditImage.IconFileName));

                    if (oldFileInfo.Exists)
                    {
                        string newFullNameForOldFile = @"D:\TEMP\" + imageItem.EditImage.IconFileName[..imageItem.EditImage.IconFileName.IndexOf('.')] + "-save-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "." + imageItem.EditImage.IconFileNameExtension;

                        System.IO.File.Move(oldFileInfo.FullName, newFullNameForOldFile);
                    }

                    string imageFormFileCopyPath = Path.Combine(path, imageItem.IconFormFile.FileName);

                    using (FileStream stream = new(imageFormFileCopyPath, FileMode.Create))
                    {
                        await imageItem.IconFormFile.CopyToAsync(stream);
                    }

                    FileInfo newFileInfo = new(Path.Combine(path, imageItem.IconFormFile.FileName));

                    if (imageItem.IconFormFile.FileName != imageItem.EditImage.ImageFileName)
                    {
                        System.IO.File.Move(newFileInfo.FullName, oldFileInfo.FullName);
                    }
                }
                else
                {
                    string imageFormFileCopyPath = Path.Combine(path, imageItem.IconFormFile.FileName);

                    using var stream = new FileStream(imageFormFileCopyPath, FileMode.Create);
                    await imageItem.IconFormFile.CopyToAsync(stream);

                    imageItem.EditImage.IconFileName = imageItem.IconFormFile.FileName;
                }

                #region AutoFillForIcon300

                IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.EditImage.IconFileName));

                foreach (var imageDirectory in imageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить имя файла «{imageItem.IconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.IconFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить расширение файла «{imageItem.IconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.IconFileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить MIME файла «{imageItem.IconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.IconMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить размер файла «{imageItem.IconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.IconFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить ширину файла «{imageItem.IconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.IconWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("IconFormFile", $"Не определить высоту файла «{imageItem.IconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.IconHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }
            else
            {
                imageUpdate.IconFileName = imageItem.EditImage.IconFileName;
                imageUpdate.IconFileSize = imageItem.EditImage.IconFileSize;
                imageUpdate.IconWidth = imageItem.EditImage.IconWidth;
                imageUpdate.IconHeight = imageItem.EditImage.IconHeight;
                imageUpdate.IconMimeType = imageItem.EditImage.IconMimeType;
                imageUpdate.IconFileNameExtension = imageItem.EditImage.IconFileNameExtension;
            }

            #endregion

            #region Edit Icon200

            if (imageItem.Icon200FormFile != null)
            {
                if (!string.IsNullOrEmpty(imageItem.EditImage.Icon200FileName))
                {
                    if (imageItem.Icon200FormFile.FileName.ToString()[(imageItem.Icon200FormFile.FileName.ToString().IndexOf('.') + 1)..] != imageItem.EditImage.Icon200FileNameExtension)
                    {
                        ModelState.AddModelError("Icon200FormFile", $"Расширение файла должно быть «.{imageItem.EditImage.Icon200FileNameExtension}»");

                        return View(nameof(EditImage), imageItem);
                    }

                    FileInfo oldFileInfo = new(Path.Combine(path, imageItem.EditImage.Icon200FileName));

                    if (oldFileInfo.Exists)
                    {
                        string newFullNameForOldFile = @"D:\TEMP\" + imageItem.EditImage.Icon200FileName[..imageItem.EditImage.Icon200FileName.IndexOf('.')] + "-save-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "." + imageItem.EditImage.Icon200FileNameExtension;

                        System.IO.File.Move(oldFileInfo.FullName, newFullNameForOldFile);
                    }

                    string imageFormFileCopyPath = Path.Combine(path, imageItem.Icon200FormFile.FileName);

                    using (FileStream stream = new(imageFormFileCopyPath, FileMode.Create))
                    {
                        await imageItem.Icon200FormFile.CopyToAsync(stream);
                    }

                    FileInfo newFileInfo = new(Path.Combine(path, imageItem.Icon200FormFile.FileName));

                    if (imageItem.Icon200FormFile.FileName != imageItem.EditImage.ImageFileName)
                    {
                        System.IO.File.Move(newFileInfo.FullName, oldFileInfo.FullName);
                    }
                }
                else
                {
                    string imageFormFileCopyPath = Path.Combine(path, imageItem.Icon200FormFile.FileName);

                    using var stream = new FileStream(imageFormFileCopyPath, FileMode.Create);
                    await imageItem.Icon200FormFile.CopyToAsync(stream);

                    imageItem.EditImage.Icon200FileName = imageItem.Icon200FormFile.FileName;
                }

                #region AutoFillForIcon200

                IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.EditImage.Icon200FileName));

                foreach (var imageDirectory in imageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить имя файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon200FileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить расширение файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon200FileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить MIME файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon200MimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить размер файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon200FileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить ширину файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon200Width = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("Icon200FormFile", $"Не определить высоту файла «{imageItem.Icon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon200Height = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }
            else
            {
                imageUpdate.Icon200FileName = imageItem.EditImage.Icon200FileName;
                imageUpdate.Icon200FileSize = imageItem.EditImage.Icon200FileSize;
                imageUpdate.Icon200Width = imageItem.EditImage.Icon200Width;
                imageUpdate.Icon200Height = imageItem.EditImage.Icon200Height;
                imageUpdate.Icon200MimeType = imageItem.EditImage.Icon200MimeType;
                imageUpdate.Icon200FileNameExtension = imageItem.EditImage.Icon200FileNameExtension;
            }

            #endregion

            #region Edit Icon100

            if (imageItem.Icon100FormFile != null)
            {
                if (!string.IsNullOrEmpty(imageItem.EditImage.Icon100FileName))
                {
                    if (imageItem.Icon100FormFile.FileName.ToString()[(imageItem.Icon100FormFile.FileName.ToString().IndexOf('.') + 1)..] != imageItem.EditImage.Icon100FileNameExtension)
                    {
                        ModelState.AddModelError("Icon100FormFile", $"Расширение файла должно быть «.{imageItem.EditImage.Icon100FileNameExtension}»");

                        return View(nameof(EditImage), imageItem);
                    }

                    FileInfo oldFileInfo = new(Path.Combine(path, imageItem.EditImage.Icon100FileName));

                    if (oldFileInfo.Exists)
                    {
                        string newFullNameForOldFile = @"D:\TEMP\" + imageItem.EditImage.Icon100FileName[..imageItem.EditImage.Icon100FileName.IndexOf('.')] + "-save-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "." + imageItem.EditImage.Icon100FileNameExtension;

                        System.IO.File.Move(oldFileInfo.FullName, newFullNameForOldFile);
                    }

                    string imageFormFileCopyPath = Path.Combine(path, imageItem.Icon100FormFile.FileName);

                    using (FileStream stream = new(imageFormFileCopyPath, FileMode.Create))
                    {
                        await imageItem.Icon100FormFile.CopyToAsync(stream);
                    }

                    FileInfo newFileInfo = new(Path.Combine(path, imageItem.Icon100FormFile.FileName));

                    if (imageItem.Icon100FormFile.FileName != imageItem.EditImage.ImageFileName)
                    {
                        System.IO.File.Move(newFileInfo.FullName, oldFileInfo.FullName);
                    }
                }
                else
                {
                    string imageFormFileCopyPath = Path.Combine(path, imageItem.Icon100FormFile.FileName);

                    using var stream = new FileStream(imageFormFileCopyPath, FileMode.Create);
                    await imageItem.Icon100FormFile.CopyToAsync(stream);

                    imageItem.EditImage.Icon100FileName = imageItem.Icon100FormFile.FileName;
                }

                #region AutoFillForIcon100

                IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.EditImage.Icon100FileName));

                foreach (var imageDirectory in imageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить имя файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon100FileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить расширение файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon100FileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить MIME файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon100MimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить размер файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon100FileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить ширину файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon100Width = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("Icon100FormFile", $"Не определить высоту файла «{imageItem.Icon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.Icon100Height = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }
            else
            {
                imageUpdate.Icon100FileName = imageItem.EditImage.Icon100FileName;
                imageUpdate.Icon100FileSize = imageItem.EditImage.Icon100FileSize;
                imageUpdate.Icon100Width = imageItem.EditImage.Icon100Width;
                imageUpdate.Icon100Height = imageItem.EditImage.Icon100Height;
                imageUpdate.Icon100MimeType = imageItem.EditImage.Icon100MimeType;
                imageUpdate.Icon100FileNameExtension = imageItem.EditImage.Icon100FileNameExtension;
            }

            #endregion

            #region Edit WebImageHD

            if (imageItem.WebImageHDFormFile != null)
            {
                if (!string.IsNullOrEmpty(imageItem.EditImage.WebImageHDFileName))
                {
                    if (imageItem.WebImageHDFormFile.FileName[(imageItem.WebImageHDFormFile.FileName.IndexOf('.') + 1)..] != imageItem.EditImage.WebImageHDNameExtension)
                    {
                        ModelState.AddModelError("WebImageHDFormFile", $"Расширение файла должно быть «.{imageItem.EditImage.WebImageHDNameExtension}»");

                        return View(imageItem);
                    }

                    FileInfo oldFileInfo = new(Path.Combine(path, imageItem.EditImage.WebImageHDFileName));

                    if (oldFileInfo.Exists)
                    {
                        string newFullNameForOldFile = @"D:\TEMP\" + imageItem.EditImage.WebImageHDFileName[..imageItem.EditImage.WebImageHDFileName.IndexOf('.')] + "-save-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "." + imageItem.EditImage.WebImageHDNameExtension;

                        System.IO.File.Move(oldFileInfo.FullName, newFullNameForOldFile);
                    }

                    string imageFormFileCopyPath = Path.Combine(path, imageItem.WebImageHDFormFile.FileName);

                    using (FileStream stream = new(imageFormFileCopyPath, FileMode.Create))
                    {
                        await imageItem.WebImageHDFormFile.CopyToAsync(stream);
                    }

                    FileInfo newFileInfo = new(Path.Combine(path, imageItem.WebImageHDFormFile.FileName));

                    if (imageItem.WebImageHDFormFile.FileName != imageItem.EditImage.WebImageHDFileName)
                    {
                        System.IO.File.Move(newFileInfo.FullName, oldFileInfo.FullName);
                    }
                }
                else
                {
                    string imageFormFileCopyPath = Path.Combine(path, imageItem.WebImageHDFormFile.FileName);

                    using var stream = new FileStream(imageFormFileCopyPath, FileMode.Create);
                    await imageItem.WebImageHDFormFile.CopyToAsync(stream);

                    imageItem.EditImage.WebImageHDFileName = imageItem.WebImageHDFormFile.FileName;
                }

                #region AutoFillForWebHD

                IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.EditImage.WebImageHDFileName));

                foreach (var imageDirectory in imageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить имя файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageHDFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить расширение файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageHDNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить MIME файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageHDMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить размер файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageHDFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить ширину файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageHDWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebImageHDFormFile", $"Не определить высоту файла «{imageItem.WebImageHDFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageHDHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }
            else
            {
                imageUpdate.WebImageHDFileName = imageItem.EditImage.WebImageHDFileName;
                imageUpdate.WebImageHDFileSize = imageItem.EditImage.WebImageHDFileSize;
                imageUpdate.WebImageHDWidth = imageItem.EditImage.WebImageHDWidth;
                imageUpdate.WebImageHDHeight = imageItem.EditImage.WebImageHDHeight;
                imageUpdate.WebImageHDMimeType = imageItem.EditImage.WebImageHDMimeType;
                imageUpdate.WebImageHDNameExtension = imageItem.EditImage.WebImageHDNameExtension;
            }

            #endregion

            #region Edit WebImage

            if (imageItem.WebImageFormFile != null)
            {
                if (!string.IsNullOrEmpty(imageItem.EditImage.WebImageFileName))
                {
                    if (imageItem.WebImageFormFile.FileName[(imageItem.WebImageFormFile.FileName.IndexOf('.') + 1)..] != imageItem.EditImage.WebImageFileNameExtension)
                    {
                        ModelState.AddModelError("WebImageFormFile", $"Расширение файла должно быть «.{imageItem.EditImage.WebImageFileNameExtension}»");

                        return View(imageItem);
                    }

                    FileInfo oldFileInfo = new(Path.Combine(path, imageItem.EditImage.WebImageFileName));

                    if (oldFileInfo.Exists)
                    {
                        string newFullNameForOldFile = @"D:\TEMP\" + imageItem.EditImage.WebImageFileName[..imageItem.EditImage.WebImageFileName.IndexOf('.')] + "-save-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "." + imageItem.EditImage.WebImageFileNameExtension;

                        System.IO.File.Move(oldFileInfo.FullName, newFullNameForOldFile);
                    }

                    string imageFormFileCopyPath = Path.Combine(path, imageItem.WebImageFormFile.FileName);

                    using (FileStream stream = new(imageFormFileCopyPath, FileMode.Create))
                    {
                        await imageItem.WebImageFormFile.CopyToAsync(stream);
                    }

                    FileInfo newFileInfo = new(Path.Combine(path, imageItem.WebImageFormFile.FileName));

                    if (imageItem.WebImageFormFile.FileName != imageItem.EditImage.WebImageFileName)
                    {
                        System.IO.File.Move(newFileInfo.FullName, oldFileInfo.FullName);
                    }
                }
                else
                {
                    string imageFormFileCopyPath = Path.Combine(path, imageItem.WebImageFormFile.FileName);

                    using var stream = new FileStream(imageFormFileCopyPath, FileMode.Create);
                    await imageItem.WebImageFormFile.CopyToAsync(stream);

                    imageItem.EditImage.WebImageFileName = imageItem.WebImageFormFile.FileName;
                }

                #region AutoFillForWebImage

                IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.EditImage.WebImageFileName));

                foreach (var imageDirectory in imageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить имя файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить расширение файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageFileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить MIME файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить размер файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить ширину файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebImageFormFile", $"Не определить высоту файла «{imageItem.WebImageFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebImageHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }
            else
            {
                imageUpdate.WebImageFileName = imageItem.EditImage.WebImageFileName;
                imageUpdate.WebImageFileSize = imageItem.EditImage.WebImageFileSize;
                imageUpdate.WebImageWidth = imageItem.EditImage.WebImageWidth;
                imageUpdate.WebImageHeight = imageItem.EditImage.WebImageHeight;
                imageUpdate.WebImageMimeType = imageItem.EditImage.WebImageMimeType;
                imageUpdate.WebImageFileNameExtension = imageItem.EditImage.WebImageFileNameExtension;
            }

            #endregion

            #region Edit WebIcon300

            if (imageItem.WebIconFormFile != null)
            {
                if (!string.IsNullOrEmpty(imageItem.EditImage.WebIconFileName))
                {
                    if (imageItem.WebIconFormFile.FileName.ToString()[(imageItem.WebIconFormFile.FileName.ToString().IndexOf('.') + 1)..] != imageItem.EditImage.WebIconFileNameExtension)
                    {
                        ModelState.AddModelError("WebIconFormFile", $"Расширение файла должно быть «.{imageItem.EditImage.WebIconFileNameExtension}»");

                        return View(nameof(EditImage), imageItem);
                    }

                    FileInfo oldFileInfo = new(Path.Combine(path, imageItem.EditImage.WebIconFileName));

                    if (oldFileInfo.Exists)
                    {
                        string newFullNameForOldFile = @"D:\TEMP\" + imageItem.EditImage.WebIconFileName[..imageItem.EditImage.WebIconFileName.IndexOf('.')] + "-save-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "." + imageItem.EditImage.WebIconFileNameExtension;

                        System.IO.File.Move(oldFileInfo.FullName, newFullNameForOldFile);
                    }

                    string imageFormFileCopyPath = Path.Combine(path, imageItem.WebIconFormFile.FileName);

                    using (FileStream stream = new(imageFormFileCopyPath, FileMode.Create))
                    {
                        await imageItem.WebIconFormFile.CopyToAsync(stream);
                    }

                    FileInfo newFileInfo = new(Path.Combine(path, imageItem.WebIconFormFile.FileName));

                    if (imageItem.WebIconFormFile.FileName != imageItem.EditImage.WebIconFileName)
                    {
                        System.IO.File.Move(newFileInfo.FullName, oldFileInfo.FullName);
                    }
                }
                else
                {
                    string imageFormFileCopyPath = Path.Combine(path, imageItem.WebIconFormFile.FileName);

                    using var stream = new FileStream(imageFormFileCopyPath, FileMode.Create);
                    await imageItem.WebIconFormFile.CopyToAsync(stream);

                    imageItem.EditImage.WebIconFileName = imageItem.WebIconFormFile.FileName;
                }

                #region AutoFillForWebIcon300

                IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.EditImage.WebIconFileName));

                foreach (var imageDirectory in imageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIconFormFile", $"Не определить имя файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIconFileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIconFormFile", $"Не определить расширение файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIconFileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIconFormFile", $"Не определить MIME файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIconMimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("WebIconFormFile", $"Не определить размер файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIconFileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIconFormFile", $"Не определить ширину файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIconWidth = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIconFormFile", $"Не определить высоту файла «{imageItem.WebIconFormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIconHeight = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }
            else
            {
                imageUpdate.WebIconFileName = imageItem.EditImage.WebIconFileName;
                imageUpdate.WebIconFileSize = imageItem.EditImage.WebIconFileSize;
                imageUpdate.WebIconWidth = imageItem.EditImage.WebIconWidth;
                imageUpdate.WebIconHeight = imageItem.EditImage.WebIconHeight;
                imageUpdate.WebIconMimeType = imageItem.EditImage.WebIconMimeType;
                imageUpdate.WebIconFileNameExtension = imageItem.EditImage.WebIconFileNameExtension;
            }

            #endregion

            #region Edit WebIcon200

            if (imageItem.WebIcon200FormFile != null)
            {
                if (!string.IsNullOrEmpty(imageItem.EditImage.WebIcon200FileName))
                {
                    if (imageItem.WebIcon200FormFile.FileName.ToString()[(imageItem.WebIcon200FormFile.FileName.ToString().IndexOf('.') + 1)..] != imageItem.EditImage.WebIcon200FileNameExtension)
                    {
                        ModelState.AddModelError("WebIcon200FormFile", $"Расширение файла должно быть «.{imageItem.EditImage.WebIcon200FileNameExtension}»");

                        return View(nameof(EditImage), imageItem);
                    }

                    FileInfo oldFileInfo = new(Path.Combine(path, imageItem.EditImage.WebIcon200FileName));

                    if (oldFileInfo.Exists)
                    {
                        string newFullNameForOldFile = @"D:\TEMP\" + imageItem.EditImage.WebIcon200FileName[..imageItem.EditImage.WebIcon200FileName.IndexOf('.')] + "-save-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "." + imageItem.EditImage.WebIcon200FileNameExtension;

                        System.IO.File.Move(oldFileInfo.FullName, newFullNameForOldFile);
                    }

                    string imageFormFileCopyPath = Path.Combine(path, imageItem.WebIcon200FormFile.FileName);

                    using (FileStream stream = new(imageFormFileCopyPath, FileMode.Create))
                    {
                        await imageItem.WebIcon200FormFile.CopyToAsync(stream);
                    }

                    FileInfo newFileInfo = new(Path.Combine(path, imageItem.WebIcon200FormFile.FileName));

                    if (imageItem.WebIcon200FormFile.FileName != imageItem.EditImage.WebIcon200FileName)
                    {
                        System.IO.File.Move(newFileInfo.FullName, oldFileInfo.FullName);
                    }
                }
                else
                {
                    string imageFormFileCopyPath = Path.Combine(path, imageItem.WebIcon200FormFile.FileName);

                    using var stream = new FileStream(imageFormFileCopyPath, FileMode.Create);
                    await imageItem.WebIcon200FormFile.CopyToAsync(stream);

                    imageItem.EditImage.WebIcon200FileName = imageItem.WebIcon200FormFile.FileName;
                }

                #region AutoFillForWebIcon200

                IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.EditImage.WebIcon200FileName));

                foreach (var imageDirectory in imageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить имя файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon200FileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить расширение файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon200FileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить MIME файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon200MimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить размер файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon200FileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить ширину файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon200Width = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить высоту файла «{imageItem.WebIcon200FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon200Height = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }
            else
            {
                imageUpdate.WebIcon200FileName = imageItem.EditImage.WebIcon200FileName;
                imageUpdate.WebIcon200FileSize = imageItem.EditImage.WebIcon200FileSize;
                imageUpdate.WebIcon200Width = imageItem.EditImage.WebIcon200Width;
                imageUpdate.WebIcon200Height = imageItem.EditImage.WebIcon200Height;
                imageUpdate.WebIcon200MimeType = imageItem.EditImage.WebIcon200MimeType;
                imageUpdate.WebIcon200FileNameExtension = imageItem.EditImage.WebIcon200FileNameExtension;
            }

            #endregion

            #region Edit WebIcon100

            if (imageItem.WebIcon100FormFile != null)
            {
                if (!string.IsNullOrEmpty(imageItem.EditImage.WebIcon100FileName))
                {
                    if (imageItem.WebIcon100FormFile.FileName.ToString()[(imageItem.WebIcon100FormFile.FileName.ToString().IndexOf('.') + 1)..] != imageItem.EditImage.WebIcon100FileNameExtension)
                    {
                        ModelState.AddModelError("WebIcon100FormFile", $"Расширение файла должно быть «.{imageItem.EditImage.WebIcon100FileNameExtension}»");

                        return View(nameof(EditImage), imageItem);
                    }

                    FileInfo oldFileInfo = new(Path.Combine(path, imageItem.EditImage.WebIcon100FileName));

                    if (oldFileInfo.Exists)
                    {
                        string newFullNameForOldFile = @"D:\TEMP\" + imageItem.EditImage.WebIcon100FileName[..imageItem.EditImage.WebIcon100FileName.IndexOf('.')] + "-save-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "." + imageItem.EditImage.WebIcon100FileNameExtension;

                        System.IO.File.Move(oldFileInfo.FullName, newFullNameForOldFile);
                    }

                    string imageFormFileCopyPath = Path.Combine(path, imageItem.WebIcon100FormFile.FileName);

                    using (FileStream stream = new(imageFormFileCopyPath, FileMode.Create))
                    {
                        await imageItem.WebIcon100FormFile.CopyToAsync(stream);
                    }

                    FileInfo newFileInfo = new(Path.Combine(path, imageItem.WebIcon100FormFile.FileName));

                    if (imageItem.WebIcon100FormFile.FileName != imageItem.EditImage.WebIcon100FileName)
                    {
                        System.IO.File.Move(newFileInfo.FullName, oldFileInfo.FullName);
                    }
                }
                else
                {
                    string imageFormFileCopyPath = Path.Combine(path, imageItem.WebIcon100FormFile.FileName);

                    using var stream = new FileStream(imageFormFileCopyPath, FileMode.Create);
                    await imageItem.WebIcon100FormFile.CopyToAsync(stream);

                    imageItem.EditImage.WebIcon100FileName = imageItem.WebIcon100FormFile.FileName;
                }

                #region AutoFillForWebIcon100

                IReadOnlyList<MetadataExtractor.Directory> imageDirectories = ImageMetadataReader.ReadMetadata(Path.Combine(path, imageItem.EditImage.WebIcon100FileName));

                foreach (var imageDirectory in imageDirectories)
                {
                    foreach (var tag in imageDirectory.Tags)
                    {
                        if (tag.Name == "File Name")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon100FormFile", $"Не определить имя файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon100FileName = tag.Description;
                        }

                        if (tag.Name == "Expected File Name Extension")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon100FormFile", $"Не определить расширение файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon100FileNameExtension = tag.Description;
                        }

                        if (tag.Name == "Detected MIME Type")
                        {
                            if (tag.Description == null || string.IsNullOrWhiteSpace(tag.Description) || string.IsNullOrEmpty(tag.Description))
                            {
                                ModelState.AddModelError("WebIcon100FormFile", $"Не определить MIME файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon100MimeType = tag.Description;
                        }

                        if (tag.Name == "File Size")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]) <= 0)
                            {
                                ModelState.AddModelError("WebIcon100FormFile", $"Не определить размер файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon100FileSize = Convert.ToUInt32(tag.Description[..tag.Description.ToString().IndexOf(' ')]);
                        }

                        if (tag.Name == "Image Width")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIcon100FormFile", $"Не определить ширину файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon100Width = Convert.ToUInt32(tag.Description);
                        }

                        if (tag.Name == "Image Height")
                        {
                            if (tag.Description == null || Convert.ToUInt32(tag.Description) <= 0)
                            {
                                ModelState.AddModelError("WebIcon200FormFile", $"Не определить высоту файла «{imageItem.WebIcon100FormFile.FileName}»");

                                return View(nameof(EditImage), imageItem);
                            }

                            imageUpdate.WebIcon100Height = Convert.ToUInt32(tag.Description);
                        }
                    }
                }

                #endregion
            }
            else
            {
                imageUpdate.WebIcon100FileName = imageItem.EditImage.WebIcon100FileName;
                imageUpdate.WebIcon100FileSize = imageItem.EditImage.WebIcon100FileSize;
                imageUpdate.WebIcon100Width = imageItem.EditImage.WebIcon100Width;
                imageUpdate.WebIcon100Height = imageItem.EditImage.WebIcon100Height;
                imageUpdate.WebIcon100MimeType = imageItem.EditImage.WebIcon100MimeType;
                imageUpdate.WebIcon100FileNameExtension = imageItem.EditImage.WebIcon100FileNameExtension;
            }

            #endregion

            #region Удалить файлы

            if (imageItem.DeleteImageHD)
            {
                string deletePath = path + "\\" + imageItem.EditImage.ImageHDFileName;

                FileInfo deleteFile = new(deletePath);

                if (deleteFile.Exists)
                {
                    deleteFile.Delete();

                    // альтернатива с помощью класса File
                    // File.Delete(deletePath);
                }

                imageUpdate.ImageHDFileName = null;
                imageUpdate.ImageHDNameExtension = null;
                imageUpdate.ImageHDMimeType = null;
                imageUpdate.ImageHDFileSize = 0;
                imageUpdate.ImageHDWidth = 0;
                imageUpdate.ImageHDHeight = 0;
            }

            if (imageItem.DeleteWebImageHD)
            {
                string deletePath = path + "\\" + imageItem.EditImage.WebImageHDFileName;

                FileInfo deleteFile = new(deletePath);

                if (deleteFile.Exists)
                {
                    deleteFile.Delete();

                    // альтернатива с помощью класса File
                    // File.Delete(deletePath);
                }

                imageUpdate.WebImageHDFileName = null;
                imageUpdate.WebImageHDNameExtension = null;
                imageUpdate.WebImageHDMimeType = null;
                imageUpdate.WebImageHDFileSize = 0;
                imageUpdate.WebImageHDWidth = 0;
                imageUpdate.WebImageHDHeight = 0;
            }

            if (imageItem.DeleteImage)
            {
                string deletePath = path + "\\" + imageItem.EditImage.ImageFileName;

                FileInfo deleteFile = new(deletePath);

                if (deleteFile.Exists)
                {
                    deleteFile.Delete();

                    // альтернатива с помощью класса File
                    // File.Delete(deletePath);
                }

                imageUpdate.ImageFileName = string.Empty;
                imageUpdate.ImageFileNameExtension = string.Empty;
                imageUpdate.ImageMimeType = string.Empty;
                imageUpdate.ImageFileSize = 0;
                imageUpdate.ImageWidth = 0;
                imageUpdate.ImageHeight = 0;
            }

            if (imageItem.DeleteIcon)
            {
                string deletePath = path + "\\" + imageItem.EditImage.IconFileName;

                FileInfo deleteFile = new(deletePath);

                if (deleteFile.Exists)
                {
                    deleteFile.Delete();

                    // альтернатива с помощью класса File
                    // File.Delete(deletePath);
                }

                imageUpdate.IconFileName = null;
                imageUpdate.IconFileNameExtension = null;
                imageUpdate.IconMimeType = null;
                imageUpdate.IconFileSize = 0;
                imageUpdate.IconWidth = 0;
                imageUpdate.IconHeight = 0;
            }

            if (imageItem.DeleteIcon200)
            {
                string deletePath = path + "\\" + imageItem.EditImage.Icon200FileName;

                FileInfo deleteFile = new(deletePath);

                if (deleteFile.Exists)
                {
                    deleteFile.Delete();

                    // альтернатива с помощью класса File
                    // File.Delete(deletePath);
                }

                imageUpdate.Icon200FileName = null;
                imageUpdate.Icon200FileNameExtension = null;
                imageUpdate.Icon200MimeType = null;
                imageUpdate.Icon200FileSize = 0;
                imageUpdate.Icon200Width = 0;
                imageUpdate.Icon200Height = 0;
            }

            if (imageItem.DeleteIcon100)
            {
                string deletePath = path + "\\" + imageItem.EditImage.Icon100FileName;

                FileInfo deleteFile = new(deletePath);

                if (deleteFile.Exists)
                {
                    deleteFile.Delete();

                    // альтернатива с помощью класса File
                    // File.Delete(deletePath);
                }

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
        else
        {
            return View(imageItem);
        }
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
}