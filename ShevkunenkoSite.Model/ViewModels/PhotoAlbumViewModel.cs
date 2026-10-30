namespace ShevkunenkoSite.Models.ViewModels;

public class PhotoAlbumViewModel : ItemsListViewModel
{
    #region Заголовок альбома

    /// <summary>
    /// Заголовок альбома
    /// </summary>
    public string CaptionOfAlbum { get; set; } = string.Empty;

    #endregion

    #region Примечание для заголовка альбома

    /// <summary>
    /// Примечание под заголовком альбома
    /// </summary>
    public string? NoteForCaptionOfAlbum { get; set; }

    #endregion

    #region Список объектов

    /// <summary>
    /// Список картинок на странице
    /// </summary>
    public ImageFileModel[] ItemsOnPage { get; set; } = [];

    #endregion

    #region Id (Guid) текущей картинки

    /// <summary>
    /// Id (Guid) текущей картинки 
    /// </summary>
    public Guid? CurrentImageId { get; set; }

    #endregion

    #region Страница альбома (true) или страница картинки (false)

    /// <summary>
    /// true - страница альбома, 
    /// false — страница картинки
    /// </summary>
    public bool AlbumOrPhoto { get; set; }

    #endregion

    #region Показывать AI-версии картинок

    /// <summary>
    /// Показывать AI-версии картинок (webimageAI, webicon300AI, ...).
    /// false — обычные webimage, webicon300, ...
    /// </summary>
    public bool ShowAI { get; set; } = true;

    #endregion

    #region Есть ли у текущей картинки AI-варианты

    /// <summary>
    /// true — у текущей картинки есть хотя бы один AI-файл
    /// (webimageAI, webimageHDAI, webicon300AI, webicon200AI, webicon100AI).
    /// Используется для показа кнопки переключения AI/Оригинал.
    /// </summary>
    public bool CurrentImageHasAI { get; set; } = false;

    #endregion
}