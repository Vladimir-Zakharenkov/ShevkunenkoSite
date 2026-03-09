namespace ShevkunenkoSite.Models.DataModels;

[NotMapped]
public record class IconDTOModel
{
    [DataType(DataType.Text)]
    [Display(Name = "Параметр rel в метатеге link :")]
    public string RelForIcon { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Purpose в manifest :")]
    public string IconPurpose { get; set; } = string.Empty;

    public Guid IconTypeModelId { get; set; }

    #region Выбрать файл иконки (NotMapped)

    [DataType(DataType.Upload)]
    [Display(Name = "Выбрать файл иконки :")]
    public IFormFile? IconFileFormFile { get; set; }

    #endregion
}