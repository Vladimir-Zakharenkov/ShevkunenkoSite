namespace ShevkunenkoSite.Models.DataModels;

public class TextInfoModel
{
    #region Идентификатор текста

    [Display(Name = "TextInfoId:")]
    [Column("TextInfoId")]
    public Guid TextInfoModelId { get; set; }

    #endregion

    #region Описание текста

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Описание текста :")]
    public string TextDescription { get; set; } = string.Empty;

    #endregion

    #region Файл TXT

    [DataType(DataType.Text)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Файл (txt) :")]
    public string TxtFileName { get; set; } = string.Empty;

    [Display(Name = "Размер файла (txt) :")]
    public int TxtFileSize { get; set; }

    #endregion

    #region Файл HTML

    [DataType(DataType.Text)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Файл (html) :")]
    public string HtmlFileName { get; set; } = string.Empty;

    [Display(Name = "Размер файла (html) :")]
    public int HtmlFileSize { get; set; }

    #endregion

    #region Папка для текста

    [DataType(DataType.Text)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Папка :")]
    public string FolderForText { get; set; } = string.Empty;

    #endregion

    #region Номер страницы книги (статьи)

    [Display(Name = "Номер страницы :")]
    [Range(0, 3000)]
    public int? SequenceNumber { get; set; }

    #endregion

    #region Связанные книга или статья

    [Display(Name = "Связанная книга (статья) :")]
    public Guid? BooksAndArticlesModelId { get; set; }
    public BooksAndArticlesModel? BooksAndArticlesModel { get; set; }

    #endregion

    #region Связанный аудиофайл

    [Display(Name = "Аудиофайл :")]
    public Guid? AudioInfoModelId { get; set; }
    public AudioInfoModel? AudioInfoModel { get; set; }

    #endregion

    #region One-to-One with PageInfoModel as parent

    public PageInfoModel? PageInfoModel { get; set; }

    #endregion

    #region NotMapped

    #region Текст без разметки TXT (NotMapped)

    [NotMapped]
    public string? ClearText { get; set; }

    #endregion

    #region Текст с разметкой HTML (NotMapped)

    [NotMapped]
    public string? HtmlText { get; set; }

    #endregion

    #region Новый каталог для текста (NotMapped)

    [NotMapped]
    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Новый каталог :")]
    public string NewTextFolder { get; set; } = string.Empty;

    #endregion

    #region Текущий каталог для текста (NotMapped)

    [NotMapped]
    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Текущий каталог :")]
    public string CurrentTextFolder { get; set; } = string.Empty;

    #endregion

    #region Выбрать файл TXT (NotMapped)

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Файл (txt) :")]
    public IFormFile? TxtFileFormFile { get; set; }

    #endregion

    #region Выбрать файл HTML (NotMapped)

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Файл (html) :")]
    public IFormFile? HtmlFileFormFile { get; set; }

    #endregion

    #region Сообщение о ссылках на текст (NotMapped)

    [NotMapped]
    public string? RefInMovies { get; set; }

    #endregion

    #endregion
}