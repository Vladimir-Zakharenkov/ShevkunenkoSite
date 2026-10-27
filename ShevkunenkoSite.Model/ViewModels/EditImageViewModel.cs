namespace ShevkunenkoSite.Models.ViewModels;

public class EditImageViewModel
{
    public ImageFileModel EditImage { get; set; } = new();

    #region Ленивая загрузка списка каталогов

    private List<SelectListItem>? _imageDirectories;

    public List<SelectListItem> ImageDirectories =>
        _imageDirectories ??= BuildImageDirectoryItems();

    #endregion

    #region Задать новый каталог

    [Display(Name = "Новый каталог :")]
    public string? NewImagePath { get; set; }

    #endregion

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка :")]
    public IFormFile? ImageFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка HD :")]
    public IFormFile? ImageHDFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 300 px :")]
    public IFormFile? IconFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 200 px :")]
    public IFormFile? Icon200FormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 100 px :")]
    public IFormFile? Icon100FormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка (WebP) :")]
    public IFormFile? WebImageFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка HD (WebP) :")]
    public IFormFile? WebImageHDFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 300 px (WebP) :")]
    public IFormFile? WebIconFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 200 px (WebP) :")]
    public IFormFile? WebIcon200FormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Иконка 100 px (WebP) :")]
    public IFormFile? WebIcon100FormFile { get; set; }

    [Display(Name = "Удалить картинку HD")]
    public bool DeleteImageHD { get; set; } = false;

    [Display(Name = "Удалить картинку HD (WebP) :")]
    public bool DeleteWebImageHD { get; set; } = false;

    [Display(Name = "Удалить картинку :")]
    public bool DeleteImage { get; set; } = false;

    [Display(Name = "Удалить иконку 300 px :")]
    public bool DeleteIcon { get; set; } = false;

    [Display(Name = "Удалить иконку 200 px :")]
    public bool DeleteIcon200 { get; set; } = false;

    [Display(Name = "Удалить иконку 100 px :")]
    public bool DeleteIcon100 { get; set; } = false;

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