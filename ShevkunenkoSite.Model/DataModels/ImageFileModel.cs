namespace ShevkunenkoSite.Models.DataModels;

public class ImageFileModel
{
    #region Идентификатор (Guid)

    [Key]
    [Display(Name = "Идентификатор картинки :")]
    [Column("ImageFileId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ImageFileModelId { get; set; }

    #endregion

    #region Название

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите название картинки")]
    [NotWhitespace(ErrorMessage = "Название не может состоять только из пробелов")]
    [MaxLength(60, ErrorMessage = "{0} не должно содержать больше {1} символов")]
    [Display(Name = "Название картинки :")]
    public string ImageCaption { get; set; } = string.Empty;

    #endregion

    #region Описание

    [MaxLength(300, ErrorMessage = "{0} не должно содержать больше {1} символов")]
    [Display(Name = "Описание картинки :")]
    public string ImageDescription { get; set; } = string.Empty;

    #endregion

    #region Теги  «alt» и «title»

    [MaxLength(60, ErrorMessage = "{0} не должно содержать больше {1} символов")]
    [Display(Name = "Свойства «alt» и «title» :")]
    public string ImageAltTitle { get; set; } = string.Empty;

    #endregion

    #region Каталог картинки

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите каталог картинки")]
    [NotWhitespace(ErrorMessage = "Каталог не может состоять только из пробелов")]
    [Display(Name = "Каталог картинки :")]
    public string ImagePath { get; set; } = "/images";

    #endregion

    #region Фильтры поиска

    [DisplayFormat(ConvertEmptyStringToNull = true)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Фильтры поиска: ")]
    public string? SearchFilter { get; set; }

    #endregion

    #region Индекс сортировки

    [Range(0, 1000000, ErrorMessage = "Недопустимое значение")]
    [Display(Name = "Индекс сортировки:")]
    public int SortOfPicture { get; set; } = 1;

    #endregion

    #region WEBP

    #region AutoFillWebImageHD (webp fullHD)

    [Display(Name = "Файл картинки HD (WebP) :")]
    public string? WebImageHDFileName { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла картинки HD (WebP) :")]
    public string? WebImageHDNameExtension { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла картинки HD (WebP) :")]
    public string? WebImageHDMimeType { get; set; }

    [Display(Name = "Размер файла картинки HD (WebP) :")]
    [Range(1, 4000000, ErrorMessage = "Недопустимое значение")]
    public uint? WebImageHDFileSize { get; set; }

    [Display(Name = "Ширина картинки HD (WebP) в px :")]
    [Range(1, 4000, ErrorMessage = "Недопустимое значение")]
    public uint? WebImageHDWidth { get; set; }

    [Display(Name = "Высота картинки HD (WebP) в px :")]
    [Range(1, 4000, ErrorMessage = "Недопустимое значение")]
    public uint? WebImageHDHeight { get; set; }

    #endregion

    #region AutoFillWebImage (webp 720 px)

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите имя файла 720 px (Webp)")]
    [NotWhitespace(ErrorMessage = "Имя файла не может состоять только из пробелов")]
    [Display(Name = "Файл картинки (WebP) :")]
    public string WebImageFileName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите расширение картинки")]
    [NotWhitespace(ErrorMessage = "Расширение файла не может состоять только из пробелов")]
    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла картинки 720 px (WebP) :")]
    public string WebImageFileNameExtension { get; set; } = "webp";

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите MimeType файла картинки 720 px (Webp)")]
    [NotWhitespace(ErrorMessage = "MimeType файла картинки не может состоять только из пробелов")]
    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла картинки 720 px (WebP) :")]
    public string WebImageMimeType { get; set; } = "image/webp";

    [Display(Name = "Размер файла картинки 720 px (WebP) :")]
    [Range(1, 4000000, ErrorMessage = "Недопустимое значение")]
    public uint WebImageFileSize { get; set; } = 1;

    [Display(Name = "Ширина картинки720 (WebP) в px :")]
    [Range(1, 720, ErrorMessage = "Недопустимое значение")]
    public uint WebImageWidth { get; set; } = 1;

    [Display(Name = "Высота картинки720 (WebP) в px :")]
    [Range(1, 1440, ErrorMessage = "Недопустимое значение")]
    public uint WebImageHeight { get; set; } = 1;

    #endregion

    #region AutoFillWebIcon300 (webp 300 px)

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите имя файла иконки 300 px (Webp)")]
    [NotWhitespace(ErrorMessage = "Имя файла не может состоять только из пробелов")]
    [Display(Name = "Файл иконки 300 px (WebP) :")]
    public string WebIconFileName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите расширение файла иконки 300 px (Webp)")]
    [NotWhitespace(ErrorMessage = "Расширение файла не может состоять только из пробелов")]
    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки 300 px (WebP) :")]
    public string WebIconFileNameExtension { get; set; } = "webp";

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите MimeType файла иконки 300 px (Webp)")]
    [NotWhitespace(ErrorMessage = "MimeType файла не может состоять только из пробелов")]
    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки 300 px (WebP) :")]
    public string WebIconMimeType { get; set; } = "image/webp";

    [Display(Name = "Размер файла иконки 300 px (WebP) :")]
    [Range(1, 2000000, ErrorMessage = "Недопустимое значение")]
    public uint WebIconFileSize { get; set; } = 1;

    [Display(Name = "Ширина иконки300 (WebP) px :")]
    [Range(1, 300, ErrorMessage = "Недопустимое значение")]
    public uint WebIconWidth { get; set; } = 1;

    [Display(Name = "Высота иконки300 (WebP) в px :")]
    [Range(1, 600, ErrorMessage = "Недопустимое значение")]
    public uint WebIconHeight { get; set; } = 1;

    #endregion

    #region AutoFillWebIcon200 (webp 200 px)

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите имя файла иконки 200 px (WebP)")]
    [NotWhitespace(ErrorMessage = "Имя файла не может состоять только из пробелов")]
    [Display(Name = "Файл иконки 200 px (WebP) :")]
    public string WebIcon200FileName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите расширение файла иконки 200 px (WebP)")]
    [NotWhitespace(ErrorMessage = "Расширение файла не может состоять только из пробелов")]
    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки 200 px (WebP) :")]
    public string WebIcon200FileNameExtension { get; set; } = "webp";

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите MimeType файла иконки 200 px (WebP)")]
    [NotWhitespace(ErrorMessage = "MimeType файла иконки не может состоять только из пробелов")]
    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки 200 px (WebP) :")]
    public string WebIcon200MimeType { get; set; } = "image/webp";

    [Display(Name = "Размер файла иконки 200 px (WebP) :")]
    [Range(1, 2000000, ErrorMessage = "Недопустимое значение")]
    public uint WebIcon200FileSize { get; set; } = 1;

    [Display(Name = "Ширина иконки200 (WebP) в px :")]
    [Range(1, 200, ErrorMessage = "Недопустимое значение")]
    public uint WebIcon200Width { get; set; } = 1;

    [Display(Name = "Высота иконки200 (WebP) в px :")]
    [Range(1, 400, ErrorMessage = "Недопустимое значение")]
    public uint WebIcon200Height { get; set; } = 1;

    #endregion

    #region AutoFillWebIcon100 (webp 100 px)

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите имя файла иконки 100 px (WebP)")]
    [NotWhitespace(ErrorMessage = "Имя файла не может состоять только из пробелов")]
    [Display(Name = "Файл иконки 100 px (WebP) :")]
    public string WebIcon100FileName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите расширение файла иконки 100 px (WebP)")]
    [NotWhitespace(ErrorMessage = "Расширение файла не может состоять только из пробелов")]
    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки 100 px (WebP) :")]
    public string WebIcon100FileNameExtension { get; set; } = "webp";

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите MimeType файла иконки 100 px (WebP)")]
    [NotWhitespace(ErrorMessage = "MimeType файла иконки не может состоять только из пробелов")]
    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки 100 px (WebP) :")]
    public string WebIcon100MimeType { get; set; } = "image/webp";

    [Display(Name = "Размер файла иконки 100 px (WebP) :")]
    [Range(1, 2000000, ErrorMessage = "Недопустимое значение")]
    public uint WebIcon100FileSize { get; set; } = 1;

    [Display(Name = "Ширина иконки100 (WebP) в px :")]
    [Range(1, 100, ErrorMessage = "Недопустимое значение")]
    public uint WebIcon100Width { get; set; } = 1;

    [Display(Name = "Высота иконки100 (WebP) в px :")]
    [Range(1, 200, ErrorMessage = "Недопустимое значение")]
    public uint WebIcon100Height { get; set; } = 1;

    #endregion

    #endregion

    #region AI

    #region AutoFillWebImageHD (AI webp fullHD )

    [Display(Name = "Файл картинки HD (WebP AI) :")]
    public string? WebImageHDFileNameAI { get; set; }

    [Display(Name = "Расширение файла картинки HD (WebP AI) :")]
    public string? WebImageHDNameExtensionAI { get; set; }

    [Display(Name = "MimeType файла картинки HD (WebP AI) :")]
    public string? WebImageHDMimeTypeAI { get; set; }

    [Display(Name = "Размер файла картинки HD (WebP AI) :")]
    [Range(1, 4000000, ErrorMessage = "Недопустимое значение")]
    public uint? WebImageHDFileSizeAI { get; set; }

    [Display(Name = "Ширина картинки HD (WebP AI) в px :")]
    [Range(1, 4000, ErrorMessage = "Недопустимое значение")]
    public uint? WebImageHDWidthAI { get; set; }

    [Display(Name = "Высота картинки HD (WebP AI) в px :")]
    [Range(1, 4000, ErrorMessage = "Недопустимое значение")]
    public uint? WebImageHDHeightAI { get; set; }

    #endregion

    #region AutoFillWebImage (AI webp 720 px)

    [DataType(DataType.Text)]
    [Display(Name = "Файл картинки (WebP AI) :")]
    public string? WebImageFileNameAI { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла картинки (WebP AI) :")]
    public string? WebImageFileNameExtensionAI { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла картинки (WebP AI) :")]
    public string? WebImageMimeTypeAI { get; set; }

    [Display(Name = "Размер файла картинки (WebP AI) :")]
    [Range(1, 4000000, ErrorMessage = "Недопустимое значение")]
    public uint? WebImageFileSizeAI { get; set; }

    [Display(Name = "Ширина картинки (WebP AI) в px :")]
    [Range(1, 720, ErrorMessage = "Недопустимое значение")]
    public uint? WebImageWidthAI { get; set; }

    [Display(Name = "Высота картинки (WebP AI) в px :")]
    [Range(1, 1440, ErrorMessage = "Недопустимое значение")]
    public uint? WebImageHeightAI { get; set; }

    #endregion

    #region AutoFillWebIcon300 (AI webp 300 px)

    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки 300 px (WebP AI) :")]
    public string? WebIconFileNameAI { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки 300 px (WebP AI) :")]
    public string? WebIconFileNameExtensionAI { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки 300 px (WebP AI) :")]
    public string? WebIconMimeTypeAI { get; set; }

    [Display(Name = "Размер файла иконки 300 px (WebP AI) :")]
    [Range(1, 2000000, ErrorMessage = "Недопустимое значение")]
    public uint? WebIconFileSizeAI { get; set; }

    [Display(Name = "Ширина иконки300 (WebP AI) в px :")]
    [Range(1, 300, ErrorMessage = "Недопустимое значение")]
    public uint? WebIconWidthAI { get; set; }

    [Display(Name = "Высота иконки300 (WebP AI) в px :")]
    [Range(1, 600, ErrorMessage = "Недопустимое значение")]
    public uint? WebIconHeightAI { get; set; }

    #endregion

    #region AutoFillWebIcon200 (AI webp 200 px)

    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки 200 px (WebP AI) :")]
    public string? WebIcon200FileNameAI { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки 200 px (WebP AI) :")]
    public string? WebIcon200FileNameExtensionAI { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки 200 px (WebP AI) :")]
    public string? WebIcon200MimeTypeAI { get; set; }

    [Display(Name = "Размер файла иконки 200 px (WebP AI) :")]
    [Range(1, 2000000, ErrorMessage = "Недопустимое значение")]
    public uint? WebIcon200FileSizeAI { get; set; }

    [Display(Name = "Ширина иконки200 (WebP AI) в px :")]
    [Range(1, 200, ErrorMessage = "Недопустимое значение")]
    public uint? WebIcon200WidthAI { get; set; }

    [Display(Name = "Высота иконки200 (WebP AI) в px :")]
    [Range(1, 400, ErrorMessage = "Недопустимое значение")]
    public uint? WebIcon200HeightAI { get; set; }

    #endregion

    #region AutoFillWebIcon100 (AI webp 100 px)

    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки 100 px (WebP AI) :")]
    public string? WebIcon100FileNameAI { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки 100 px (WebP AI) :")]
    public string? WebIcon100FileNameExtensionAI { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки 100 px (WebP AI) :")]
    public string? WebIcon100MimeTypeAI { get; set; }

    [Display(Name = "Размер файла иконки 100 px (WebP AI) :")]
    [Range(1, 2000000, ErrorMessage = "Недопустимое значение")]
    public uint? WebIcon100FileSizeAI { get; set; }

    [Display(Name = "Ширина иконки100 (WebP AI) в px :")]
    [Range(1, 100, ErrorMessage = "Недопустимое значение")]
    public uint? WebIcon100WidthAI { get; set; }

    [Display(Name = "Высота иконки100 (WebP AI) в px :")]
    [Range(1, 200, ErrorMessage = "Недопустимое значение")]
    public uint? WebIcon100HeightAI { get; set; }

    #endregion

    #endregion

    #region PNG

    #region AutoFillImageHD (png fullHD)

    [DataType(DataType.Text)]
    [Display(Name = "Файл картинки HD (Png) :")]
    public string? ImageHDFileName { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла картинки HD (Png) :")]
    public string? ImageHDNameExtension { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла картинки HD (Png) :")]
    public string? ImageHDMimeType { get; set; }

    [Display(Name = "Размер файла картинки HD (Png) :")]
    [Range(1, 4000000, ErrorMessage = "Недопустимое значение")]
    public uint? ImageHDFileSize { get; set; }

    [Display(Name = "Ширина картинки HD (Png) в px :")]
    [Range(1, 4000, ErrorMessage = "Недопустимое значение")]
    public uint? ImageHDWidth { get; set; }

    [Display(Name = "Высота картинки HD (Png) в px :")]
    [Range(1, 4000, ErrorMessage = "Недопустимое значение")]
    public uint? ImageHDHeight { get; set; }

    #endregion

    #region AutoFillImage (png 720 px)

    [DataType(DataType.Text)]
    [Display(Name = "Файл картинки (Png) :")]
    public string? ImageFileName { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла картинки (Png) :")]
    public string? ImageFileNameExtension { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла картинки (Png) :")]
    public string? ImageMimeType { get; set; }

    [Display(Name = "Размер файла картинки (Png) :")]
    [Range(1, 4000000, ErrorMessage = "Недопустимое значение")]
    public uint? ImageFileSize { get; set; }

    [Display(Name = "Ширина картинки (Png) в px :")]
    [Range(1, 720, ErrorMessage = "Недопустимое значение")]
    public uint? ImageWidth { get; set; }

    [Display(Name = "Высота картинки (Png) в px :")]
    [Range(1, 1440, ErrorMessage = "Недопустимое значение")]
    public uint? ImageHeight { get; set; }

    #endregion

    #region AutoFillIcon300 (png 300 px)

    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки 300 px (Png) :")]
    public string? IconFileName { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки 300 px (Png) :")]
    public string? IconFileNameExtension { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки 300 px (Png) :")]
    public string? IconMimeType { get; set; }

    [Display(Name = "Размер файла иконки 300 px (Png) :")]
    [Range(1, 2000000, ErrorMessage = "Недопустимое значение")]
    public uint? IconFileSize { get; set; }

    [Display(Name = "Ширина иконки300 (Png) в px :")]
    [Range(1, 300, ErrorMessage = "Недопустимое значение")]
    public uint? IconWidth { get; set; }

    [Display(Name = "Высота иконки300 (Png) в px :")]
    [Range(1, 600, ErrorMessage = "Недопустимое значение")]
    public uint? IconHeight { get; set; }

    #endregion

    #region AutoFillIcon200 (png 200 px)

    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки 200 px (Png) :")]
    public string? Icon200FileName { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки 200 px (Png) :")]
    public string? Icon200FileNameExtension { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки 200 px (Png) :")]
    public string? Icon200MimeType { get; set; }

    [Display(Name = "Размер файла иконки 200 px (Png) :")]
    [Range(1, 2000000, ErrorMessage = "Недопустимое значение")]
    public uint? Icon200FileSize { get; set; }

    [Display(Name = "Ширина иконки200 (Png) в px :")]
    [Range(1, 200, ErrorMessage = "Недопустимое значение")]
    public uint? Icon200Width { get; set; }

    [Display(Name = "Высота иконки200 (Png) в px :")]
    [Range(1, 400, ErrorMessage = "Недопустимое значение")]
    public uint? Icon200Height { get; set; }

    #endregion

    #region AutoFillIcon100 (png 100 px)

    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки 100 px (Png) :")]
    public string? Icon100FileName { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки 100 px (Png) :")]
    public string? Icon100FileNameExtension { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки 100 px (Png) :")]
    public string? Icon100MimeType { get; set; }

    [Display(Name = "Размер файла иконки 100 px (Png) :")]
    [Range(1, 2000000, ErrorMessage = "Недопустимое значение")]
    public uint? Icon100FileSize { get; set; }

    [Display(Name = "Ширина иконки100 (Png) в px :")]
    [Range(1, 100, ErrorMessage = "Недопустимое значение")]
    public uint? Icon100Width { get; set; }

    [Display(Name = "Высота иконки100 (Png) в px :")]
    [Range(1, 200, ErrorMessage = "Недопустимое значение")]
    public uint? Icon100Height { get; set; }

    #endregion

    #endregion
}