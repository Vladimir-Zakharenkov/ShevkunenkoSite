namespace ShevkunenkoSite.Models.ViewModels;

public class EditMovieViewModel : DetailsMovieViewModel
{
    [DataType(DataType.Upload)]
    [Display(Name = "Файл фильма :")]
    public IFormFile? FileForMovieFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Полный вариант фильма :")]
    public IFormFile? FullMovieFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка фильма :")]
    public IFormFile? ImageForMovieFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Постер фильма :")]
    public IFormFile? PosterForMovieFormFile { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Картинка страницы серий :")]
    public IFormFile? ImageHeadForSeriesFormFile { get; set; } = null;

    [DataType(DataType.Upload)]
    [Display(Name = "Выбрать статью :")]
    public IFormFile? ArticleAboutMovie1FormFile { get; set; } = null;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Страница фильма :")]
    public string PageForMovie { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Страница серий :")]
    public string PageForSeriesString { get; set; } = string.Empty;

    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Темы видео :")]
    public List<TopicMovieModel> TopicsForMovie { get; set; } = [];

    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Тип картинки для ссылок (1) :")]
    public string ImageTypeForRef1 { get; set; } = string.Empty;

    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Тип картинки для ссылок (2) :")]
    public string ImageTypeForRef2 { get; set; } = string.Empty;

    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [DataType(DataType.Text)]
    [Display(Name = "Тип картинки для ссылок (3) :")]
    public string ImageTypeForRef3 { get; set; } = string.Empty;
}