using ShevkunenkoSite.Services.Attributes;

namespace ShevkunenkoSite.Models.DataModels;

public class ImageFileModel
{
    #region Идентификатор (Guid)

    [Key]
    [Display(Name = "Идентификатор картинки :")]
    [Column("ImageFileId")]
    public Guid ImageFileModelId { get; set; }

    #endregion

    #region Название

    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите название картинки")]
    [NotWhitespace(ErrorMessage = "Название не может состоять только из пробелов")]
    [DataType(DataType.Text)]
    [MaxLength(60, ErrorMessage = "{0} не должно содержать больше {1} символов")]
    [Display(Name = "Название картинки :")]
    public string ImageCaption { get; set; } = string.Empty;

    #endregion

    #region Описание

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [MaxLength(300, ErrorMessage = "{0} не должно содержать больше {1} символов")]
    [Display(Name = "Описание картинки :")]
    public string ImageDescription { get; set; } = string.Empty;

    #endregion

    #region Теги  «alt» и «title»

    [Required(AllowEmptyStrings = false)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [MaxLength(60, ErrorMessage = "{0} не должно содержать больше {1} символов")]
    [Display(Name = "Свойства «alt» и «title» :")]
    public string ImageAltTitle { get; set; } = string.Empty;

    #endregion

    #region Каталог картинки

    [DataType(DataType.Text)]
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

    [Required(ErrorMessage = "Выберите значение")]
    [Range(0, 1000000, ErrorMessage = "Недопустимое значение")]
    [Display(Name = "Индекс сортировки:")]
    public int SortOfPicture { get; set; } = 1;

    #endregion

    #region AutoFillWebImageHD (webp fullHD)

    [DataType(DataType.Text)]
    [Display(Name = "Файл картинки HD (WebP) :")]
    public string? WebImageHDFileName { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла картинки HD (WebP) :")]
    public string? WebImageHDNameExtension { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла картинки HD (WebP) :")]
    public string? WebImageHDMimeType { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла картинки HD (WebP) :")]
    public uint? WebImageHDFileSize { get; set; } = 0;

    [Display(Name = "Ширина картинки HD (WebP) в px :")]
    [Range(0, 4000)]
    public uint? WebImageHDWidth { get; set; } = 0;

    [Display(Name = "Высота картинки HD (WebP) в px :")]
    [Range(0, 4000)]
    public uint? WebImageHDHeight { get; set; } = 0;

    #endregion

    #region AutoFillWebImageHD (AI webp fullHD )

    [DataType(DataType.Text)]
    [Display(Name = "Файл картинки HD (WebP) :")]
    public string? WebImageHDFileNameAI { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла картинки HD (WebP) :")]
    public string? WebImageHDNameExtensionAI { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла картинки HD (WebP) :")]
    public string? WebImageHDMimeTypeAI { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла картинки HD (WebP) :")]
    public int? WebImageHDFileSizeAI { get; set; }

    [Display(Name = "Ширина картинки HD (WebP) в px :")]
    [Range(0, 4000)]
    public int? WebImageHDWidthAI { get; set; } = 0;

    [Display(Name = "Высота картинки HD (WebP) в px :")]
    [Range(0, 4000)]
    public int? WebImageHDHeightAI { get; set; } = 0;

    #endregion

    #region AutoFillImageHD (png fullHD)

    [DataType(DataType.Text)]
    [Display(Name = "Файл картинки HD :")]
    public string? ImageHDFileName { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла картинки HD :")]
    public string? ImageHDNameExtension { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла картинки HD :")]
    public string? ImageHDMimeType { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла картинки HD :")]
    public uint? ImageHDFileSize { get; set; }

    [Display(Name = "Ширина картинки HD в px :")]
    [Range(0, 4000)]
    public uint? ImageHDWidth { get; set; }

    [Display(Name = "Высота картинки HD в px :")]
    [Range(0, 4000)]
    public uint? ImageHDHeight { get; set; }

    #endregion

    #region AutoFillWebImage (webp 720 px)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Файл картинки (WebP) :")]
    public string? WebImageFileName { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла картинки (WebP) :")]
    public string? WebImageFileNameExtension { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла картинки (WebP) :")]
    public string? WebImageMimeType { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла картинки (WebP) :")]
    public uint? WebImageFileSize { get; set; } = 0;

    [Display(Name = "Ширина картинки (WebP) в px :")]
    public uint? WebImageWidth { get; set; } = 0;

    [Display(Name = "Высота картинки (WebP) в px :")]
    public uint? WebImageHeight { get; set; } = 0;

    #endregion

    #region AutoFillWebImage (AI webp 720 px)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Файл картинки (WebP) :")]
    public string? WebImageFileNameAI { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла картинки (WebP) :")]
    public string? WebImageFileNameExtensionAI { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла картинки (WebP) :")]
    public string? WebImageMimeTypeAI { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла картинки (WebP) :")]
    public uint? WebImageFileSizeAI { get; set; } = 0;

    [Display(Name = "Ширина картинки (WebP) в px :")]
    public uint? WebImageWidthAI { get; set; } = 0;

    [Display(Name = "Высота картинки (WebP) в px :")]
    public uint? WebImageHeightAI { get; set; } = 0;

    #endregion

    #region AutoFillImage (png 720 px)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Файл картинки :")]
    public string? ImageFileName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла картинки :")]
    public string? ImageFileNameExtension { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла картинки :")]
    public string? ImageMimeType { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла картинки :")]
    public uint? ImageFileSize { get; set; } = 0;

    [Display(Name = "Ширина картинки в px :")]
    public uint? ImageWidth { get; set; } = 0;

    [Display(Name = "Высота картинки в px :")]
    public uint? ImageHeight { get; set; } = 0;

    #endregion

    #region AutoFillWebIcon300 (webp 300 px)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки (WebP) :")]
    public string? WebIconFileName { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки (WebP) :")]
    public string? WebIconFileNameExtension { get; set; } = "webp";

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки (WebP) :")]
    public string? WebIconMimeType { get; set; } = "image/webp";

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла иконки (WebP) :")]
    public uint? WebIconFileSize { get; set; } = 0;

    [Display(Name = "Ширина иконки (WebP) в px :")]
    public uint? WebIconWidth { get; set; } = 0;

    [Display(Name = "Высота иконки (WebP) в px :")]
    public uint? WebIconHeight { get; set; } = 0;

    #endregion

    #region AutoFillWebIcon300 (AI webp 300 px)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки (WebP) :")]
    public string? WebIconFileNameAI { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки (WebP) :")]
    public string? WebIconFileNameExtensionAI { get; set; } = "webp";

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки (WebP) :")]
    public string? WebIconMimeTypeAI { get; set; } = "image/webp";

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла иконки (WebP) :")]
    public uint? WebIconFileSizeAI { get; set; } = 0;

    [Display(Name = "Ширина иконки (WebP) в px :")]
    public uint? WebIconWidthAI { get; set; } = 0;

    [Display(Name = "Высота иконки (WebP) в px :")]
    public uint? WebIconHeightAI { get; set; } = 0;

    #endregion

    #region AutoFillIcon300 (png 300 px)

    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки :")]
    public string? IconFileName { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки :")]
    public string? IconFileNameExtension { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки :")]
    public string? IconMimeType { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла иконки :")]
    public uint? IconFileSize { get; set; }

    [Display(Name = "Ширина иконки в px :")]
    public uint? IconWidth { get; set; }

    [Display(Name = "Высота иконки в px :")]
    public uint? IconHeight { get; set; }

    #endregion

    #region AutoFillWebIcon200 (webp 200 px)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки (WebP) :")]
    public string? WebIcon200FileName { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки (WebP) :")]
    public string? WebIcon200FileNameExtension { get; set; } = "webp";

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки (WebP) :")]
    public string? WebIcon200MimeType { get; set; } = "image/webp";

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла иконки (WebP) :")]
    public uint? WebIcon200FileSize { get; set; } = 0;

    [Display(Name = "Ширина иконки200 (WebP) в px :")]
    public uint? WebIcon200Width { get; set; } = 0;

    [Display(Name = "Высота иконки200 (WebP) в px :")]
    public uint? WebIcon200Height { get; set; } = 0;

    #endregion

    #region AutoFillWebIcon200 (AI webp 200 px)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки (WebP) :")]
    public string? WebIcon200FileNameAI { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки (WebP) :")]
    public string? WebIcon200FileNameExtensionAI { get; set; } = "webp";

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки (WebP) :")]
    public string? WebIcon200MimeTypeAI { get; set; } = "image/webp";

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла иконки (WebP) :")]
    public uint? WebIcon200FileSizeAI { get; set; } = 0;

    [Display(Name = "Ширина иконки200 (WebP) в px :")]
    public uint? WebIcon200WidthAI { get; set; } = 0;

    [Display(Name = "Высота иконки200 (WebP) в px :")]
    public uint? WebIcon200HeightAI { get; set; } = 0;

    #endregion

    #region AutoFillIcon200 (png 200 px)

    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки :")]
    public string? Icon200FileName { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки :")]
    public string? Icon200FileNameExtension { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки :")]
    public string? Icon200MimeType { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла иконки :")]
    public uint? Icon200FileSize { get; set; }

    [Display(Name = "Ширина иконки в px :")]
    public uint? Icon200Width { get; set; }

    [Display(Name = "Высота иконки в px :")]
    public uint? Icon200Height { get; set; }

    #endregion

    #region AutoFillWebIcon100 (webp 100 px)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки (WebP) :")]
    public string? WebIcon100FileName { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки (WebP) :")]
    public string? WebIcon100FileNameExtension { get; set; } = "webp";

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки (WebP) :")]
    public string? WebIcon100MimeType { get; set; } = "image/webp";

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла иконки (WebP) :")]
    public uint? WebIcon100FileSize { get; set; } = 0;

    [Display(Name = "Ширина иконки в px :")]
    public uint? WebIcon100Width { get; set; } = 0;

    [Display(Name = "Высота иконки в px :")]
    public uint? WebIcon100Height { get; set; } = 0;

    #endregion

    #region AutoFillWebIcon100 (AI webp 100 px)

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки (WebP) :")]
    public string? WebIcon100FileNameAI { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки (WebP) :")]
    public string? WebIcon100FileNameExtensionAI { get; set; } = "webp";

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки (WebP) :")]
    public string? WebIcon100MimeTypeAI { get; set; } = "image/webp";

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла иконки (WebP) :")]
    public uint? WebIcon100FileSizeAI { get; set; } = 0;

    [Display(Name = "Ширина иконки в px :")]
    public uint? WebIcon100WidthAI { get; set; } = 0;

    [Display(Name = "Высота иконки в px :")]
    public uint? WebIcon100HeightAI { get; set; } = 0;

    #endregion

    #region AutoFillIcon100 (png 100 px)

    [DataType(DataType.Text)]
    [Display(Name = "Файл иконки :")]
    public string? Icon100FileName { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Расширение файла иконки :")]
    public string? Icon100FileNameExtension { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "MimeType файла иконки :")]
    public string? Icon100MimeType { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Размер файла иконки :")]
    public uint? Icon100FileSize { get; set; }

    [Display(Name = "Ширина иконки в px :")]
    public uint? Icon100Width { get; set; }

    [Display(Name = "Высота иконки в px :")]
    public uint? Icon100Height { get; set; }

    #endregion
}