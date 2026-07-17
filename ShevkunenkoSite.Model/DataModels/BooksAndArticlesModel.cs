// Ignore Spelling: Pdf

namespace ShevkunenkoSite.Models.DataModels;

public class BooksAndArticlesModel
{
    #region Guid книги или статьи

    [Key]
    [Display(Name = "Идентификатор :")]
    [Column("BooksArticlesId")]
    public Guid BooksAndArticlesModelId { get; set; } = Guid.Empty;

    #endregion

    #region Обложка книги

    [Display(Name = "Обложка книги :")]
    public Guid? ImageFileModelId { get; set; }
    public ImageFileModel? ImageFileModel { get; set; }

    #endregion

    #region Тип текста

    [DataType(DataType.Text)]
    [Display(Name = "Тип текста :")]
    public string TypeOfText { get; set; } = string.Empty;

    [NotMapped]
    public string[] TypesOfText =
        [
            "book",
            "article"
        ];

    #endregion

    #region Издатель

    [Required(ErrorMessage = "Введите издателя текста", AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Издатель текста :")]
    public string Publisher { get; set; } = string.Empty;

    #endregion

    #region Логотип издания

    [Display(Name = "Логотип издания :")]
    [DataType(DataType.Upload)]
    public Guid? LogoOfArticleId { get; set; }
    public ImageFileModel? LogoOfArticle { get; set; }

    #endregion

    #region Скан статьи

    [Display(Name = "Скан статьи :")]
    public Guid? ScanOfArticleId { get; set; }
    public ImageFileModel? ScanOfArticle { get; set; }

    #endregion
    
    #region Видео связанное с книгой (статьёй)

    [Display(Name = "Видео для текста :")]
    public Guid? VideoForBookOrArticleId { get; set; }
    public MovieFileModel? VideoForBookOrArticle { get; set; }

    #endregion

    #region Фильм связанный с книгой (статьёй)

    [Display(Name = "Фильм по книге :")]
    public Guid? FilmForBookOrArticleId { get; set; }
    public FilmFileModel? FilmForBookOrArticle { get; set; }

    #endregion

    #region Автор книги (статьи)

    [Required(ErrorMessage = "Введите автора текста", AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Автор текста :")]
    public string AuthorOfText { get; set; } = string.Empty;

    #endregion

    #region Название книги (статьи)

    [DataType(DataType.Text)]
    [Display(Name = "Заголовок :")]
    public string CaptionOfText { get; set; } = string.Empty;

    #endregion

    #region Связь с таблицей BookCaptionForURLModel (one-to-one)

    public BookCaptionForURLModel? CaptionForURL { get; set; }

    #endregion

    #region Подзаголовок книги (статьи)

    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Подзаголовок :")]
    public string TheSubtitle { get; set; } = string.Empty;

    #endregion

    #region Описание книги (статьи)

    [Required(ErrorMessage = "Добавте описание")]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Описание текста :")]
    public string BookDescription { get; set; } = string.Empty;

    #endregion

    #region Количество страниц

    [Display(Name = "Количество страниц :")]
    public int? NumberOfPages { get; set; }

    #endregion

    #region Дата публикации

    [Required(ErrorMessage = "Укажите дату публикации")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата публикации :")]
    public DateTime DateOfPublication { get; set; } = DateTime.Today;

    #endregion

    #region Ссылка на статью или издателя

    [DataType(DataType.Url)]
    [Display(Name = "Ссылка на статью :")]
    public Uri? UrlOfArticle { get; set; }
    #endregion

    #region Теги по содержанию книги (статьи)

    [Required(ErrorMessage = "Добавте теги")]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Теги по содержанию :")]
    public string TagsForBook { get; set; } = string.Empty;

    #endregion

    #region Ссылки на загрузку

    #region Ссылка загрузки Word

    [DataType(DataType.Url)]
    [Display(Name = "Документ Word : ")]
    public Uri? RefToWordDoc { get; set; }

    #endregion

    #region Ссылка загрузки PDF

    [DataType(DataType.Url)]
    [Display(Name = "Документ PDF : ")]
    public Uri? RefToPdf { get; set; }

    #endregion

    #region Ссылка загрузки аудиокниги

    [DataType(DataType.Url)]
    [Display(Name = "Аудиокнига : ")]
    public Uri? RefToAudio { get; set; }

    #endregion

    #endregion

    #region NotMapped

    #region Выбрать обложку

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Выбрать обложку :")]
    public IFormFile? CoverForBookFormFile { get; set; }

    #endregion

    #region Выбрать логотип

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Выбрать логотип :")]
    public IFormFile? LogoOfArticleFormFile { get; set; }

    #endregion

    #region Выбрать скан

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Выбрать скан :")]
    public IFormFile? ScanOfArticleFormFile { get; set; }

    #endregion

    #region Выбрать видео

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Выбрать видео :")]
    public IFormFile? VideoForBookOrArticleFormFile { get; set; }

    #endregion

    #region Выбрать фильм

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Выбрать фильм :")]
    public IFormFile? FilmForBookOrArticleFormFile { get; set; }

    #endregion

    #endregion
}