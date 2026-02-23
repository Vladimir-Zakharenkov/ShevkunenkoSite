namespace ShevkunenkoSite.Models.DataModels;

public record class IconModel
{
    [Key]
    [Display(Name = "ID иконки :")]
    [Column("IconId")]
    public Guid IconModelId { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки :")]
    public string IconFileName { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Каталог иконки :")]
    public string PathToIcon { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки :")]
    public string IconMimeType { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Параметр rel в метатеге link :")]
    public string RelForIcon { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Размер иконки :")]
    public string IconSize { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Purpose в manifest :")]
    public string IconPurpose { get; set; } = string.Empty;

    #region Свойства NotMapped

    #region Выбрать файл иконки (NotMapped)

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Выбрать файл иконки :")]
    public IFormFile? IconFileFormFile { get; set; }

    #endregion

    #region Новый каталог (NotMapped)

    [NotMapped]
    [Required(ErrorMessage = "Введите название каталога")]
    [DataType(DataType.Text)]
    [Display(Name = "Новый каталог :")]
    public string NewIconPath { get; set; } = string.Empty;

    #endregion

    #region Новый тип иконки (NotMapped)

    [NotMapped]
    [Display(Name = "Новый тип иконки :")]
    public bool NewIcon { get; set; }

    #endregion

    #endregion
}