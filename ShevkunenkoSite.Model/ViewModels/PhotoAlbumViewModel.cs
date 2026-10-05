namespace ShevkunenkoSite.Models.ViewModels;

public class PhotoAlbumViewModel : ItemsListViewModel
{
    #region Заголовок альбома

    public string CaptionOfAlbum { get; set; } = string.Empty;

    #endregion

    #region Примечание для заголовка альбома

    public string? NoteForCaptionOfAlbum { get; set; }

    #endregion

    #region Список объектов

    public IEnumerable<ImageFileModel> ItemsOnPage { get; set; } = [];

    #endregion

    #region Id (Guid) текущей картинки

    public Guid? CurrentImageId { get; set; }

    #endregion

    #region Страница альбома (true) или страница картинки (false)

    public bool AlbumOrPhoto { get; set; }

    #endregion
}