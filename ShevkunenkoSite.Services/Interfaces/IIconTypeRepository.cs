namespace ShevkunenkoSite.Services.Interfaces;

public interface IIconTypeRepository
{
    #region Все типы иконок в БД

    IQueryable<IconTypeModel> IconTypes { get; }

    #endregion

    #region Добавить новый тип иконки

    Task AddNewIconTypeAsync(IconTypeModel iconType);

    #endregion

    #region Сохранить данные иконки в БД

    Task SaveChangesInIconTypesAsync();

    #endregion

    //#region Удалить иконку из БД

    //Task DeleteIconTypeAsync(Guid iconId);

    //#endregion

    //#region Получить тип иконки из БД по GUID

    //Task<IconTypeModel> GetIconTypeByGuid(Guid iconType);

    //#endregion
}