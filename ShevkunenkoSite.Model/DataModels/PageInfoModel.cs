// Ignore Spelling: Loc Lastmod Changefreq Og

namespace ShevkunenkoSite.Models.DataModels;

using ShevkunenkoSite.Models.ViewModels;

public class PageInfoModel
{
    #region Идентификатор страницы в базе данных

    [Display(Name = "PageInfoId:")]
    [Column("PageInfoId")]
    public Guid PageInfoModelId { get; set; }

    #endregion

    #region Адрес страницы

    [Required(ErrorMessage = "Выберите MVC или RazorPages")]
    [Display(Name = "MVC или RazorPages :")]
    public bool PageAsRazorPage { get; set; } = false;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Область :")]
    [DataType(DataType.Text)]
    public string PageArea { get; set; } = "Root";

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Контроллер :")]
    [DataType(DataType.Text)]
    public string Controller { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Действие :")]
    [DataType(DataType.Text)]
    public string Action { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Данные :")]
    [DataType(DataType.Text)]
    public string RoutData { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    public string PageFullPath { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    public string PageFullPathWithData { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Псевдоним адреса (1) :")]
    [DataType(DataType.Text)]
    public string PagePathNickName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Псевдоним адреса (2) :")]
    [DataType(DataType.Text)]
    public string PagePathNickName2 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Псевдоним адреса :")]
    [DataType(DataType.Text)]
    public string PagePathNickNameWithData { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Адрес для RazorPages :")]
    [DataType(DataType.Text)]
    public string PageLoc { get; set; } = string.Empty;

    #endregion

    #region Теги title, description, keywords

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Заголовок страницы :")]
    public string PageTitle { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Описание страницы :")]
    public string PageDescription { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Ключевые слова :")]
    public string PageKeyWords { get; set; } = string.Empty;

    #endregion

    #region SiteMap

    [Required(ErrorMessage = "Укажите дату изменения")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата изменения :")]
    public DateTime PageLastmod { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Выберите значение")]
    [Display(Name = "Частота изменения :")]
    [DataType(DataType.Text)]
    [MaxLength(7)]
    public string Changefreq { get; set; } = "monthly";

    [Required(ErrorMessage = "Выберите значения от 0.1 до 1.0")]
    [Display(Name = "Приоритет страницы :")]
    [DataType(DataType.Text)]
    [MaxLength(3)]
    public string Priority { get; set; } = "0.5";

    #endregion

    #region BrowserConfig и т.п.

    [Required(ErrorMessage = "Необходимо указать файл browserconfig")]
    [Display(Name = "Файл browserconfig :")]
    [DataType(DataType.Text)]
    public string BrowserConfig { get; set; } = "main.xml";

    [Required(ErrorMessage = "Укажите каталог иконок browserconfig")]
    [Display(Name = "Каталог иконок browserconfig :")]
    [DataType(DataType.Text)]
    public string BrowserConfigFolder { get; set; } = "main";

    [Required(ErrorMessage = "Необходимо указать файл manifest")]
    [Display(Name = "Файл manifest :")]
    [DataType(DataType.Text)]
    public string Manifest { get; set; } = "main.json";

    [Required(ErrorMessage = "Выберите папку с иконками")]
    [Display(Name = "Папка с иконками :")]
    [DataType(DataType.Text)]
    public string PageIconPath { get; set; } = "0.5";

    #endregion

    #region OpenGraph

    [Required(ErrorMessage = "Необходимо указать og:type")]
    [Display(Name = "Open Graph :")]
    [DataType(DataType.Text)]
    public string OgType { get; set; } = "website";

    #endregion

    #region Фон страницы (фотопленка)

    public Guid BackgroundFileModelId { get; set; }
    public BackgroundFileModel? BackgroundFileModel { get; set; }

    #endregion

    #region Картинка страницы

    public Guid ImageFileModelId { get; set; }
    public ImageFileModel? ImageFileModel { get; set; }

    #endregion

    #region Текст карточки

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Текст карточки :")]
    [DataType(DataType.Text)]
    public string PageCardText { get; set; } = string.Empty;

    #endregion

    #region Связанный текстовый файл

    public Guid? TextInfoId { get; set; }
    public TextInfoModel? TextInfo { get; set; }

    #endregion

    #region Связанный аудиофайл

    public Guid? AudioInfoId { get; set; }
    public AudioInfoModel? AudioInfo { get; set; }

    #endregion

    #region Связанный фильм

    public Guid? FilmInfoId { get; set; }
    public FilmFileModel? FilmInfo { get; set; }

    #endregion

    #region Заголовок, картинка и текст страницы

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Заголовок страницы (<h1>...</h1>) :")]
    [DataType(DataType.Text)]
    public string PageHeading { get; set; } = string.Empty;

    [Display(Name = "Картинка страницы :")]
    public Guid? ImagePageHeadingId { get; set; }
    public ImageFileModel? ImagePageHeading { get; set; }

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Текст (HTML) :")]
    [DataType(DataType.Text)]
    public string TextOfPage { get; set; } = string.Empty;

    #endregion

    #region Фильтры поиска текущей страницы

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Фильтры поиска :")]
    [DataType(DataType.Text)]
    public string PageFilter { get; set; } = string.Empty;

    #endregion

    #region Значение для сортировки страницы

    [Required(ErrorMessage = "Выберите значение")]
    [Display(Name = "Индекс сортировки:")]
    public int SortOfPage { get; set; } = 1;

    #endregion

    #region Фотографии, страницы и видео внизу страницы

    #region Ссылки на страницы

    #region Включить ссылки на страницы по текстовому фильтру

    [Required(ErrorMessage = "Выберите значение")]
    [Display(Name = "Ссылки на страницы :")]
    public bool PageLinksByFilters { get; set; } = false;

    #endregion

    #region Строка фильтров страниц сайта, для формирования ссылок на них

    [Display(Name = "Фильтры поиска страниц :")]
    [DataType(DataType.Text)]
    public string? PageFilterOut { get; set; }

    #endregion

    #endregion

    #region Ссылки на связанные фото

    #region Включить ссылки на фото

    [Required(ErrorMessage = "Выберите значение")]
    [Display(Name = "Ссылки на фото :")]
    public bool PhotoLinks { get; set; } = false;

    #endregion

    #region Строка фильтров фото, для формирования ссылок на них

    [Display(Name = "Фильтры поиска фото :")]
    [DataType(DataType.Text)]
    public string? PhotoFilterOut { get; set; }

    #endregion

    #endregion

    #region Ссылки на видео

    #region Включить ссылки на видео

    [Required(ErrorMessage = "Выберите значение")]
    [Display(Name = "Ссылки на видео :")]
    public bool VideoLinks { get; set; } = false;

    #endregion

    #region Строка фильтров видео, для формирования ссылок на них

    [Display(Name = "Фильтры поиска видео :")]
    [DataType(DataType.Text)]
    public string? VideoFilterOut { get; set; }

    #endregion

    #endregion

    #region Ссылки GUID1

    #region Включить ссылки по GUID1

    [Required(ErrorMessage = "Выберите значение")]
    [Display(Name = "Ссылки по GUID (1) :")]
    public bool PageLinks { get; set; } = false;

    #endregion

    #region Строка GUID1 страниц

    [Display(Name = "Ссылки по GUID (1) :")]
    public string? RefPages { get; set; }

    #endregion

    #endregion

    #region Ссылки по GUID2

    #region Включить ссылки по GUID2

    [Required(ErrorMessage = "Выберите значение")]
    [Display(Name = "Ссылки по GUID (2) :")]
    public bool PageLinks2 { get; set; } = false;

    #endregion

    #region Строка GUID2 страниц

    [Display(Name = "Ссылки по GUID (2) :")]
    public string? RefPages2 { get; set; }

    #endregion

    #endregion

    #endregion
    
    #region Навигационное свойство MovieFileModel

    public MovieFileModel? MovieFile { get; set; }

    #endregion

    #region Свойства NotMapped

    #region Список областей (Area) (NotMapped)

    [NotMapped]
    public string[] AreaItems =
    [
        "Root",
        "Admin",
        "Movies",
        "Rybakov"
    ];

    #endregion

    #region Список типов  (OgType) (NotMapped)

    [NotMapped]
    public string[] OgTypeItems =
    [
        "movie",
        "article",
        "book",
        "audiobook",
        "website"
    ];

    #endregion

    #region Выбрать файл картинки (NotMapped)

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Картинка страницы :")]
    public IFormFile? ImageFileFormFile { get; set; }

    #endregion

    #region Выбрать файл текста (NotMapped)

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Файл текста :")]
    public IFormFile? TextFileFormFile { get; set; }

    #endregion

    #region Выбрать файл фона (фотопленки) (NotMapped)

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Фон страницы :")]
    public IFormFile? BackgroundFormFile { get; set; }

    #endregion

    #region Выбрать аудиофайл (NotMapped)

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Аудиофайл страницы :")]
    public IFormFile? AudioInfoFormFile { get; set; }

    #endregion

    #region Выбрать файл картинки (картинка под верхним заголовком) (NotMapped)

    [NotMapped]
    [DataType(DataType.Upload)]
    [Display(Name = "Картинка под заголовком :")]
    public IFormFile? ImagePageHeadingFormFile { get; set; }

    #endregion

    #region Экземпляр иконки странницы (NotMapped)

    [NotMapped]
    public IconFileModel? IconItem { get; set; }

    #endregion

    #region Список ссылок на страницы по GUID (1) (NotMapped)

    [NotMapped]
    public List<PageInfoModel>? LinksToPagesByGuid { get; set; }

    #endregion

    #region Список ссылок на страницы по GUID (2) (NotMapped)

    [NotMapped]
    public List<PageInfoModel>? LinksToPagesByGuid2 { get; set; }

    #endregion

    #region Список ссылок на видео по фильтру (NotMapped)

    [NotMapped]
    public List<VideoLinksViewModel>? LinksToVideosByFilterOut { get; set; }

    #endregion

    #region Список списков видео (NotMapped)

    [NotMapped]
    public List<List<MovieFileModel>>? ListsMoviesFileModel { get; set; }

    #endregion

    #region Список ссылок на текущую страницу по GUID (1) (NotMapped)

    [NotMapped]
    public List<PageInfoModel>? LinksFromPagesByGuid { get; set; }

    #endregion

    #region Список ссылок на текущую страницу по GUID (2) (NotMapped)

    [NotMapped]
    public List<PageInfoModel>? LinksFromPagesByGuid2 { get; set; }

    #endregion

    #region Словарь страниц ссылающихся на текущую по текстовым фильтрам (NotMapped)

    [NotMapped]
    public Dictionary<string, List<PageInfoModel>>? DictionaryOfOutPages { get; set; }

    #endregion

    #region Словарь ссылок на страницы по текстовым фильтрам (NotMapped)

    [NotMapped]
    public Dictionary<string, List<PageInfoModel>>? DictionaryOfLinksByPageFilterOut { get; set; }

    #endregion

    #region Словарь ссылок на видео по текстовым фильтрам (NotMapped)

    [NotMapped]
    public Dictionary<string, VideoLinksViewModel>? DictionaryOfLinksByVideoFilterOut { get; set; }

    #endregion

    #region Словарь ссылок на картинки по текстовым фильтрам (NotMapped)

    [NotMapped]
    public Dictionary<string, ImageListViewModel>? DictionaryOfLinksByFotoFilterOut { get; set; }

    #endregion

    #region Кадры слева и справа от текста (NotMapped)

    [NotMapped]
    public FramesAroundMainContentModel? FramesAroundMainContent { get; set; }

    #endregion

    #region Текст без разметки TXT (NotMapped)

    [NotMapped]
    public string? ClearText { get; set; }

    #endregion

    #region Текст с разметкой HTML (NotMapped)

    [NotMapped]
    public string? HtmlText { get; set; }

    #endregion

    #endregion
}