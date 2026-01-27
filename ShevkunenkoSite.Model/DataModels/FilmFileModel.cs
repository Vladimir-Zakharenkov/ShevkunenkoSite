// Ignore Spelling: Org Рroduction Imbd Poisk Online Teatr Vk Yandex

namespace ShevkunenkoSite.Models.DataModels;

public class FilmFileModel
{
    #region Guid фильма

    [Key]
    [Display(Name = "Идентификатор фильма :")]
    [Column("FilmFileId")]
    public Guid FilmFileModelId { get; set; } = Guid.Empty;

    #endregion

    #region Название фильма в базе данных

    [Required(ErrorMessage = "Введите название фильма")]
    [DataType(DataType.Text)]
    [Display(Name = "Название фильма:")]
    public string FilmCaption { get; set; } = string.Empty;

    #endregion

    #region Оригинальное название фильма

    [Required(ErrorMessage = "Оригинальное название фильма")]
    [DataType(DataType.Text)]
    [Display(Name = "Оригинальное название:")]
    public string? FilmCaptionOriginal { get; set; }

    #endregion

    #region Содержание фильма

    [Required(ErrorMessage = "Введите содержание фильма")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Краткое содержание:")]
    public string FilmDescriptionForSchemaOrg { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите содержание фильма (HTML)")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Краткое содержание (HTML):")]
    public string FilmDescriptionHtml { get; set; } = string.Empty;

    #endregion

    #region Примечания

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Примечания: ")]
    public string FilmNote { get; set; } = string.Empty;

    #endregion

    #region Включать фильм в MainList

    [Display(Name = "В списке видео сайта:")]
    public bool FilmInMainList { get; set; } = true;

    #endregion

    #region Автозаполнение параметров файла фильма

    [Display(Name = "Продолжительность фильма:")]
    [DataType(DataType.Duration)]
    public TimeSpan FilmDuration { get; set; }

    [Display(Name = "Ширина кадра (px):")]
    [DataType(DataType.Text)]
    public int FilmWidth { get; set; }

    [Display(Name = "Высота кадра (px):")]
    [DataType(DataType.Text)]
    public int FilmHeight { get; set; }

    [Display(Name = "Файл фильма:")]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    public string FilmFileName { get; set; } = string.Empty;

    [Display(Name = "Расширение файл фильма:")]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    public string FilmFileExtension { get; set; } = string.Empty;

    [Display(Name = "MIME Type файла фильма:")]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    public string FilmMimeType { get; set; } = string.Empty;

    [Display(Name = "Размер файла фильма:")]
    [DataType(DataType.Text)]
    public ulong FilmFileSize { get; set; }

    #endregion

    #region Фильтры поиска фильма

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Фильтры поиска фильма: ")]
    public string SearchFilterForFilm { get; set; } = string.Empty;

    #endregion

    #region Жанр фильма

    [Required(ErrorMessage = "Введите жанр фильма")]
    [DataType(DataType.Text)]
    [Display(Name = "Жанр фильма: ")]
    public string FilmGenre { get; set; } = string.Empty;

    #endregion

    #region Ограничения по возрасту

    [Required(ErrorMessage = "Выберите ограничения по возрасту")]
    [Display(Name = "Нет ограничений по возрасту:")]
    public bool FilmIsFamilyFriendly { get; set; } = true;

    [Display(Name = "Фильм 18+ :")]
    public bool FilmAdult { get; set; } = false;

    #endregion

    #region Полная версия фильма

    [Display(Name = "Полная версия фильма:")]
    public Guid? FullFilmId { get; set; } // Guid идентификатор полной версии фильма
    public FilmFileModel? FullFilm { get; set; }

    #endregion

    #region Дата премьеры, создания, загрузки

    [Required(ErrorMessage = "Введите дату создания")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата создания фильма:")]
    public DateTime FilmDateCreated { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Введите дату премьеры")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата премьеры фильма:")]
    public DateTime FilmDatePublished { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Введите дату загрузки на сервер")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата публикации на сайте:")]
    public DateTime FilmUploadDate { get; set; } = DateTime.Today;

    #endregion

    #region Язык фильма и субтитров

    [Required(ErrorMessage = "Введите язык звуковой дорожки")]
    [DataType(DataType.Text)]
    [Display(Name = "Звуковая дорожка (1):")]
    public string FilmInLanguage1 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Звуковая дорожка (2):")]
    public string FilmInLanguage2 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Язык субтитров (1):")]
    public string FilmSubtitles1 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Язык субтитров (2):")]
    public string FilmSubtitles2 { get; set; } = string.Empty;

    #endregion

    #region Кинокомпания и съемочная группа

    [Required(ErrorMessage = "Введите название кинокомпании")]
    [DataType(DataType.Text)]
    [Display(Name = "Кинокомпания:")]
    public string FilmРroductionCompany { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Режиссер: ")]
    public string FilmDirector1 { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Режиссер: ")]
    public string? FilmDirector2 { get; set; }

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Композитор: ")]
    public string FilmMusicBy { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string? FilmActor01 { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string? FilmActor02 { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string? FilmActor03 { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string? FilmActor04 { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string? FilmActor05 { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string? FilmActor06 { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string? FilmActor07 { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string? FilmActor08 { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string? FilmActor09 { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string? FilmActor10 { get; set; }

    #endregion

    #region Ссылки на видеохостинги

    [DataType(DataType.Url)]
    [Display(Name = "sergeyshef.ru : ")]
    public Uri? FilmContentUrl { get; set; } = null!;

    [DataType(DataType.Url)]
    [Display(Name = "YouTube : ")]
    public Uri? FilmYouTube { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "VK : ")]
    public Uri? FilmVkVideo { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "MailRu : ")]
    public Uri? FilmMailRuVideo { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "OK : ")]
    public Uri? FilmOkVideo { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "Яндекс Диск : ")]
    public Uri? FilmYandexDiskVideo { get; set; }

    #endregion

    #region Ссылки на информацию о фильме

    [DataType(DataType.Url)]
    [Display(Name = "Kino-Teatr: ")]
    public Uri? FilmKinoTeatrRu { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "Кино-Поиск: ")]
    public Uri? FilmKinoPoisk { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "IMBD: ")]
    public Uri? FilmImbd { get; set; }

    #endregion

    #region Данные для многосерийного фильм

    [Range(1, 100)]
    [Display(Name = "Количество серий:")]
    public int? FilmTotalParts { get; set; }

    [Range(1, 100)]
    [Display(Name = "Номер серии фильма:")]
    public int? FilmPart { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Фильтр поиска серий: ")]
    public string? SeriesSearchFilter { get; set; }

    #endregion

    #region Картинка и постер фильма

    // картинка для карточки фильма
    [Display(Name = "Картинка к фильму (Guid):")]
    public Guid? FilmImageId { get; set; }
    public ImageFileModel? FilmImage { get; set; }

    // постер для фильм
    [Display(Name = "Постер к фильму (Guid):")]
    public Guid? FilmPosterId { get; set; }
    public ImageFileModel? FilmPoster { get; set; }

    #endregion
}