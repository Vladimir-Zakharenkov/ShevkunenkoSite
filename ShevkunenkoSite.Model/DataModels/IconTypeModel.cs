namespace ShevkunenkoSite.Models.DataModels;

public class IconTypeModel
{
    [Key]
    [Display(Name = "ID иконки :")]
    [Column("IconTypeId")]
    public Guid IconTypeModelId { get; set; }

    [Required(ErrorMessage = "Введите каталог иконки")]
    [DataType(DataType.Text)]
    [Display(Name = "Каталог иконки :")]
    public string PathToIcon { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите описание иконки")]
    [DataType(DataType.Text)]
    [Display(Name = "Описание иконки :")]
    public string IconTypeDescription { get; set; } = string.Empty;

    #region Связь с таблицей IconModel

    public ICollection<IconModel> IconList { get; } = [];

    #endregion

    #region Свойства NotMapped

    #region Выбрать файл иконки (NotMapped)

    [NotMapped]
    [Required(ErrorMessage = "Выберите файл иконки")]
    [DataType(DataType.Upload)]
    [Display(Name = "Выбрать файл иконки :")]
    public IFormFile? IconFileFormFile { get; set; }

    #endregion

    #endregion
}