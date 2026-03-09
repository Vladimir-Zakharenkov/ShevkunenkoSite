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

    #region Связь с таблицей IconTypeModel

    public Guid IconTypeModelId { get; set; }
    public IconTypeModel IconType { get; set; } = null!;

    #endregion

    #region Свойства NotMapped

    #region Ширина иконки (NotMapped)

    [NotMapped]
    [Display(Name = "Ширина иконки :")]
    public string IconWidth { get; set; } = string.Empty;

    #endregion

    #region Высота иконки (NotMapped)

    [NotMapped]
    [Display(Name = "Высота иконки :")]
    public string IconHeight { get; set; } = string.Empty;

    #endregion

    #endregion
}