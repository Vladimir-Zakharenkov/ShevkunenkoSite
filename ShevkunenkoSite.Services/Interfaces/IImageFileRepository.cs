namespace ShevkunenkoSite.Services.Interfaces;

public interface IImageFileRepository
{
    #region Все файлы картинок в БД

    IQueryable<ImageFileModel> ImageFiles { get; }

    #endregion

    #region Добавить новую картинку

    Task AddNewImageAsync(ImageFileModel image);

    #endregion

    #region Сохранить данные картинки в БД

    Task SaveChangesInImageAsync();

    #endregion

    #region Удалить картинку из БД

    Task DeleteImageAsync(Guid imageId);

    #endregion

    #region Получить картинку из БД по GUID или имени файла

    ImageFileModel GetImageByGuidOrFileNameAsync(string imageObject);

    #endregion

    #region Получить GUID картинки по имени файла

    Task<Guid?>? GetImageGuidByFileNameAsync(string imageFillename);

    #endregion
}