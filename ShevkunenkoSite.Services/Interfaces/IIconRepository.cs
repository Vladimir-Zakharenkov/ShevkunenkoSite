namespace ShevkunenkoSite.Services.Interfaces;

public interface IIconRepository
{
    #region Все файлы иконок в БД

    IQueryable<IconModel> Icons { get; }

    #endregion

    #region Добавить новую иконку

    Task AddNewIconAsync(IconModel icon);

    #endregion

    #region Сохранить данные иконки в БД

    Task SaveChangesInIconsAsync();

    #endregion

    #region Удалить иконку из БД

    Task DeleteIconAsync(Guid iconId);

    #endregion

    #region Получить иконку из БД по GUID или имени файла

    Task<IconModel> GetIconByGuidOrFileNameAsync(string iconObject);

    #endregion
}
