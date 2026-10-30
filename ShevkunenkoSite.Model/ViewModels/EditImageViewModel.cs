namespace ShevkunenkoSite.Models.ViewModels;

public class EditImageViewModel
{
    #region Инициализация EditImage

    public ImageFileModel EditImage { get; set; } = new();

    #endregion

    #region Ленивая загрузка списка каталогов

    private List<SelectListItem>? _imageDirectories;

    public List<SelectListItem> ImageDirectories =>
        _imageDirectories ??= BuildImageDirectoryItems();

    #endregion

    #region Задать новый каталог

    [Display(Name = "Новый каталог :")]
    public string? NewImagePath { get; set; }

    #endregion

    #region IFormFile для WEBP

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка HD (WebP) :")]
    public IFormFile? WebImageHDFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка 720 px (WebP) :")]
    public IFormFile? WebImageFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 300 px (WebP) :")]
    public IFormFile? WebIconFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 200 px (WebP) :")]
    public IFormFile? WebIcon200FormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 100 px (WebP) :")]
    public IFormFile? WebIcon100FormFile { get; set; }

    #endregion

    #region IFormFile для AI (WebP)

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка HD (WebP AI) :")]
    public IFormFile? WebImageHDFormFileAI { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка  720 (WebP AI) :")]
    public IFormFile? WebImageFormFileAI { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 300 px (WebP AI) :")]
    public IFormFile? WebIconFormFileAI { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 200 px (WebP AI) :")]
    public IFormFile? WebIcon200FormFileAI { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 100 px (WebP AI) :")]
    public IFormFile? WebIcon100FormFileAI { get; set; }

    #endregion

    #region IFormFile для PNG

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка HD (Png) :")]
    public IFormFile? ImageHDFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка 720 px (Png) :")]
    public IFormFile? ImageFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 300 px (Png) :")]
    public IFormFile? IconFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 200 px (Png) :")]
    public IFormFile? Icon200FormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 100 px (Png) :")]
    public IFormFile? Icon100FormFile { get; set; }

    #endregion

    #region Флаг удаления WEBP HD

    [Display(Name = "Удалить картинку HD (WebP) :")]
    public bool DeleteWebImageHD { get; set; } = false;

    #endregion

    #region Флаги удаления PNG

    [Display(Name = "Удалить картинку HD (Png)")]
    public bool DeleteImageHD { get; set; } = false;

    [Display(Name = "Удалить картинку (Png) :")]
    public bool DeleteImage { get; set; } = false;

    [Display(Name = "Удалить иконку 300 px (Png) :")]
    public bool DeleteIcon { get; set; } = false;

    [Display(Name = "Удалить иконку 200 px (Png) :")]
    public bool DeleteIcon200 { get; set; } = false;

    [Display(Name = "Удалить иконку 100 px (Png) :")]
    public bool DeleteIcon100 { get; set; } = false;

    #endregion

    #region Флаги удаления WEBP AI

    [Display(Name = "Удалить картинку HD (WebP AI) :")]
    public bool DeleteWebImageHDAI { get; set; } = false;

    [Display(Name = "Удалить картинку 720 px (WebP AI) :")]
    public bool DeleteWebImageAI { get; set; } = false;

    [Display(Name = "Удалить иконку 300 px (WebP AI) :")]
    public bool DeleteWebIconAI { get; set; } = false;

    [Display(Name = "Удалить иконку 200 px (WebP AI) :")]
    public bool DeleteWebIcon200AI { get; set; } = false;

    [Display(Name = "Удалить иконку 100 px (WebP AI) :")]
    public bool DeleteWebIcon100AI { get; set; } = false;

    #endregion

    #region Метод формирует SelectLIst из каталогов внутри каталога «images»

    private static List<SelectListItem> BuildImageDirectoryItems()
    {
        string imagesRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        if (!Directory.Exists(imagesRoot))
        {
            return [];
        }

        string basePath = imagesRoot.Replace('\\', '/') + "/";

        List<SelectListItem> items = [.. Directory
        .GetDirectories(imagesRoot, "*", SearchOption.AllDirectories)
        .Select(dir => dir.Replace('\\', '/'))
        .Select(full => full.StartsWith(basePath, StringComparison.OrdinalIgnoreCase)
            ? full[basePath.Length..]
            : full)
        .OrderBy(rel => rel, StringComparer.OrdinalIgnoreCase)
        .Select(rel => new SelectListItem
        {
            Value = "/images/" + rel,
            Text  = "/images/" + rel
        })];

        items.Insert(0, new SelectListItem { Value = "/images", Text = "/images" });

        return items;
    }

    #endregion
}