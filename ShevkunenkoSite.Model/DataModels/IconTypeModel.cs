namespace ShevkunenkoSite.Models.DataModels;

public class IconTypeModel
{
    #region Идентификатор иконки в базе данных

    [Key]
    [Display(Name = "ID иконки :")]
    [Column("IconTypeId")]
    public Guid IconTypeModelId { get; set; }

    #endregion

    #region Каталог иконки

    [Required(ErrorMessage = "Введите каталог иконки")]
    [DataType(DataType.Text)]
    [Display(Name = "Каталог иконки :")]
    public string PathToIcon { get; set; } = string.Empty;

    #endregion

    #region Описание иконки

    [Required(ErrorMessage = "Введите описание иконки")]
    [DataType(DataType.Text)]
    [Display(Name = "Описание иконки :")]
    public string IconTypeDescription { get; set; } = string.Empty;

    #endregion

    #region One-to-Many with IconModel as One

    public ICollection<IconModel> IconList { get; } = [];

    #endregion

    #region One-to-Many with IconTypeModel as One

    public ICollection<PageInfoModel> PageCollection { get; set; } = [];

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