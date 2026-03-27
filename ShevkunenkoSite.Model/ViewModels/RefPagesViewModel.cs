namespace ShevkunenkoSite.Models.ViewModels;

public class RefPagesViewModel
{
    #region Словарь страниц по текстовым фильтрам

    public Dictionary<string, List<PageInfoModel>> DictionaryOfPages { get; set; } = [];

    #endregion

    #region Словарь картинок по текстовым фильтрам

    public Dictionary<string, List<ImageFileModel>> DictionaryOfPictures { get; set; } = [];

    #endregion

    #region Словарь фильмов по текстовым фильтрам

    public Dictionary<string, List<FilmFileModel>> DictionaryOfFilms { get; set; } = [];

    #endregion

    // связанные страницы по GUID (1)
    public List<PageInfoModel> LinksToPagesByGuid { get; set; } = [];

    // связанные страницы по GUID (2)
    public List<PageInfoModel> LinksToPagesByGuid2 { get; set; } = [];

    // список VideoLinksViewModel связанных видео
    public List<VideoLinksViewModel> ListOfVideoLinksViewModel { get; set; } = [];

    // TODO: Списки переделать в словари
}