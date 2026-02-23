namespace ShevkunenkoSite.Models.ViewModels;

public class ItemsListViewModel : PagingInfoViewModel
{
    #region Аудиокниги

    public AudioBookModel[]? AllAudioBooks { get; set; }

    #endregion

    #region Аудиофайлы

    public AudioInfoModel[]? AllAudioFiles { get; set; }

    #endregion

    #region Статьи и книги

    public BooksAndArticlesModel[]? AllBooksAndArticlesFiles { get; set; }

    #endregion

    #region Тексты

    public TextInfoModel[]? AllTextFiles { get; set; }

    #endregion

    #region Картинки

    public ImageFileModel[]? AllImageFiles { get; set; }

    #endregion

    #region Иконки

    public IconModel[]? AllIconFiles { get; set; }

    #endregion

    #region Страницы сайта

    public PageInfoModel[]? AllSitePages { get; set; }

    #endregion

    #region Фильмы

    public FilmFileModel[]? AllFilmFiles { get; set; }

    #endregion
}