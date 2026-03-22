// Ignore Spelling: Org Рroduction Imbd Poisk Online Teatr Vk Yandex

namespace ShevkunenkoSite.Models.DataModels;

public class MovieFileModel
{
    #region Guid фильма

    [Key]
    [Display(Name = "Идентификатор фильма :")]
    [Column("MovieFileId")]
    public Guid MovieFileModelId { get; set; } = Guid.Empty;

    #endregion

    #region Название фильма в базе данных

    [Required(ErrorMessage = "Введите название фильма")]
    [DataType(DataType.Text)]
    [Display(Name = "Название фильма:")]
    public string MovieCaption { get; set; } = string.Empty;

    #endregion

    #region Заголовок страницы видео

    [Required(ErrorMessage = "Введите заголовок страницы")]
    [DataType(DataType.Text)]
    [Display(Name = "Заголовок страницы: ")]
    public string MovieCaptionForOnline { get; set; } = string.Empty;

    #endregion

    #region Содержание фильма

    [Required(ErrorMessage = "Введите содержание фильма")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Краткое содержание:")]
    public string MovieDescriptionForSchemaOrg { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите содержание фильма (HTML)")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Краткое содержание (HTML):")]
    public string MovieDescriptionHtml { get; set; } = string.Empty;

    #endregion

    #region Примечания

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Примечания: ")]
    public string MovieNote { get; set; } = string.Empty;

    #endregion

    #region Включать фильм в MainList

    [Display(Name = "В списке видео сайта:")]
    public bool MovieInMainList { get; set; } = true;

    #endregion

    #region Автозаполнение параметров файла фильма

    //[Required(ErrorMessage = "Введите продолжительность фильма")]
    [DataType(DataType.Duration)]
    public TimeSpan MovieDuration { get; set; }

    //[Required(ErrorMessage = "Введите ширину кадра")]
    [DataType(DataType.Text)]
    public uint MovieWidth { get; set; }

    //[Required(ErrorMessage = "Введите высоту кадра")]
    [DataType(DataType.Text)]
    public uint MovieHeight { get; set; }

    //[Required(ErrorMessage = "Введите имя файла", AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    public string MovieFileName { get; set; } = string.Empty;

    //[Required(ErrorMessage = "Введите расширение файла", AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    public string MovieFileExtension { get; set; } = string.Empty;

    //[Required(ErrorMessage = "Введите MIME Type файла", AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    public string MovieMimeType { get; set; } = string.Empty;

    //[Required(ErrorMessage = "Введите размер файла")]
    [DataType(DataType.Text)]
    public ulong MovieFileSize { get; set; }

    #endregion

    #region Фильтры поиска фильма

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Фильтры поиска: ")]
    public string SearchFilter { get; set; } = string.Empty;

    #endregion

    #region Жанр фильма

    [Required(ErrorMessage = "Введите жанр фильма")]
    [DataType(DataType.Text)]
    [Display(Name = "Жанр фильма: ")]
    public string MovieGenre { get; set; } = string.Empty;

    #endregion

    #region Список тем фильма

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Список тем: ")]
    public string TopicGuidList { get; set; } = string.Empty;

    #endregion

    #region Ограничения по возрасту

    [Required(ErrorMessage = "Выберите ограничения по возрасту")]
    [Display(Name = "Нет ограничений по возрасту:")]
    public bool MovieIsFamilyFriendly { get; set; } = true;

    [Display(Name = "Фильм 18+ :")]
    public bool MovieAdult { get; set; } = false;

    #endregion

    #region Полная версия фильма

    [Display(Name = "Полная версия фильма:")]
    public Guid? FullMovieID { get; set; } // Guid идентификатор полной версии фильма
    public MovieFileModel? FullMovie { get; set; }

    #endregion

    #region Дата премьеры, создания, загрузки

    [Required(ErrorMessage = "Введите дату создания")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата создания фильма:")]
    public DateTime MovieDateCreated { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Введите дату премьеры")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата премьеры фильма:")]
    public DateTime MovieDatePublished { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Введите дату загрузки на сервер")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата публикации на сайте:")]
    public DateTime MovieUploadDate { get; set; } = DateTime.Today;

    #endregion

    #region Язык фильма и субтитров

    [Required(ErrorMessage = "Введите язык звуковой дорожки")]
    [DataType(DataType.Text)]
    [Display(Name = "Звуковая дорожка (1):")]
    public string MovieInLanguage1 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Звуковая дорожка (2):")]
    public string MovieInLanguage2 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Язык субтитров (1):")]
    public string MovieSubtitles1 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Язык субтитров (2):")]
    public string MovieSubtitles2 { get; set; } = string.Empty;

    #endregion

    #region Кинокомпания и съемочная группа

    [Required(ErrorMessage = "Введите название кинокомпании")]
    [DataType(DataType.Text)]
    [Display(Name = "Кинокомпания:")]
    public string MovieРroductionCompany { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Режиссер: ")]
    public string MovieDirector1 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Режиссер: ")]
    public string MovieDirector2 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Композитор: ")]
    public string MovieMusicBy { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string MovieActor01 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string MovieActor02 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string MovieActor03 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string MovieActor04 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string MovieActor05 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string MovieActor06 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string MovieActor07 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string MovieActor08 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string MovieActor09 { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Актёр (актриса): ")]
    public string MovieActor10 { get; set; } = string.Empty;

    #endregion

    #region Ссылки на видеохостинги

    [DataType(DataType.Url)]
    [Display(Name = "sergeyshef.ru : ")]
    public Uri? MovieContentUrl { get; set; } = null!;

    [DataType(DataType.Url)]
    [Display(Name = "YouTube : ")]
    public Uri? MovieYouTube { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "VK : ")]
    public Uri? MovieVkVideo { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "MailRu : ")]
    public Uri? MovieMailRuVideo { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "OK : ")]
    public Uri? MovieOkVideo { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "Яндекс Диск : ")]
    public Uri? MovieYandexDiskVideo { get; set; }

    #endregion

    #region Ссылки на информацию о фильме

    [DataType(DataType.Url)]
    [Display(Name = "Kino-Teatr: ")]
    public Uri? MovieKinoTeatrRu { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "Кино-Поиск: ")]
    public Uri? MovieKinoPoisk { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "IMBD: ")]
    public Uri? MovieImbd { get; set; }

    #endregion

    #region Данные для многосерийного фильм

    [Required(ErrorMessage = "Введите количество серий")]
    [Range(1, 100)]
    [Display(Name = "Количество серий:")]
    public int MovieTotalParts { get; set; } = 1;

    [Required(ErrorMessage = "Введите номер серии фильма")]
    [Range(1, 100)]
    [Display(Name = "Номер серии фильма:")]
    public int MoviePart { get; set; } = 1;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Фильтр поиска серий: ")]
    public string SeriesSearchFilter { get; set; } = string.Empty;

    public Guid? PageForMovieSeriesId { get; set; } // Guid страницы фильма из нескольких серий
    public PageInfoModel? PageForMovieSeries { get; set; }

    public Guid? ImageForHeadSeriesId { get; set; } // Guid картинки заголовка страницы серий
    public ImageFileModel? ImageForHeadSeries { get; set; }

    #endregion

    #region Страница фильма

    public Guid? PageInfoModelId { get; set; }
    public PageInfoModel? PageInfoModel { get; set; }

    #endregion

    #region Картинка и постер фильма

    public Guid? ImageFileModelId { get; set; } // картинка для карточки фильма
    public ImageFileModel? ImageFileModel { get; set; }

    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Постер к фильму FileName:")] // постер к фильму FileName
    public string? MoviePosterString { get; set; }

    [Display(Name = "Постер к фильму Guid:")] // постер к фильм Guid
    public Guid? MoviePosterId { get; set; }
    public ImageFileModel? MoviePoster { get; set; }

    #endregion

    #region Карусель кадров

    [Display(Name = "Карусель кадров:")] 
    public bool Carousel { get; set; } = false;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Фильтр ленты кадров:")]
    public string FramesAroundMovie { get; set; } = string.Empty;

    #endregion

    #region Ссылки на видео (1)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Заголовок")]
    public string HeadTitleForVideoLinks1 { get; set; } = string.Empty; // заголовок 1 связанных видео

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Фильтры поиска")]
    public string SearchFilter1 { get; set; } = string.Empty;

    [Display(Name = "Выбрать постер или картинку для видео")]
    // true -> картинку для видео, false -> постер для видео, null -> картинка для страницы видео
    public bool? IsImage1 { get; set; } = false;

    [Display(Name = "Тип картинки")]
    // "hd", "icon300", "icon200", "icon100", "image", "webhd", "webicon300", "webicon200", "webicon100", "webimage" 
    public string IconType1 { get; set; } = string.Empty;

    [Display(Name = "Страница фильма")]
    // true -> определяем путь к главной странице многосерийного фильма
    public bool IsPartsMoreOne1 { get; set; } = true;

    [Display(Name = "Фильмы сайта")]
    // true -> все найденные фильмы, false - фильмы с параметром InMainList (true)
    public bool AllMoviesFromDB1 { get; set; } = true;

    #endregion

    #region Ссылки на видео (2)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Заголовок")]
    public string HeadTitleForVideoLinks2 { get; set; } = string.Empty; // заголовок 2 связанных видео

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Фильтры поиска")]
    public string SearchFilter2 { get; set; } = string.Empty;

    [Display(Name = "Выбрать постер или картинку для видео")]
    // true -> картинку для видео, false -> постер для видео, null -> картинка для страницы видео
    public bool? IsImage2 { get; set; } = false;

    [Display(Name = "Тип картинки")]
    // "hd", "icon300", "icon200", "icon100", "image", "webhd", "webicon300", "webicon200", "webicon100", "webimage" 
    public string IconType2 { get; set; } = string.Empty;

    [Display(Name = "Страница фильма")]
    // true -> определяем путь к главной странице многосерийного фильма
    public bool IsPartsMoreOne2 { get; set; } = true;

    [Display(Name = "Фильмы сайта")]
    // true -> все найденные фильмы, false - фильмы с параметром InMainList (true)
    public bool AllMoviesFromDB2 { get; set; } = true;

    #endregion

    #region Ссылки на видео (3)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Заголовок")]
    public string HeadTitleForVideoLinks3 { get; set; } = string.Empty; // заголовок 2 связанных видео

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Фильтры поиска")]
    public string SearchFilter3 { get; set; } = string.Empty;

    [Display(Name = "Выбрать постер или картинку для видео")]
    // true -> картинку для видео, false -> постер для видео, null -> картинка для страницы видео
    public bool? IsImage3 { get; set; } = false;

    [Display(Name = "Тип картинки")]
    // "hd", "icon300", "icon200", "icon100", "image", "webhd", "webicon300", "webicon200", "webicon100", "webimage" 
    public string IconType3 { get; set; } = string.Empty;

    [Display(Name = "Страница фильма")]
    // true -> определяем путь к главной странице многосерийного фильма
    public bool IsPartsMoreOne3 { get; set; } = true;

    [Display(Name = "Фильмы сайта")]
    // true -> все найденные фильмы, false - фильмы с параметром InMainList (true)
    public bool AllMoviesFromDB3 { get; set; } = true;

    #endregion

    #region Статья 1 о фильме

    [Display(Name = "GUID статьи")]
    public Guid? TextInfoModelId { get; set; }
    public TextInfoModel? TextInfoModel { get; set; }

    #endregion
}