namespace ShevkunenkoSite.Services.Interfaces;

public class IconTypeImplementation(SiteDbContext siteContext) : IIconTypeRepository
{
    #region Все файлы иконок в БД

    public IQueryable<IconTypeModel> IconTypes => siteContext.IconTypes;

    #endregion

    //#region Добавить новую иконку в БД

    //public async Task AddNewIconAsync(IconModel icon)
    //{
    //    await siteContext.Icons.AddAsync(icon);
    //    await SaveChangesInIconsAsync();
    //}

    //#endregion

    //#region Удалить иконку из БД

    //public async Task DeleteIconAsync(Guid iconId)
    //{
    //    if (await siteContext.Icons.Where(icon => icon.IconModelId == iconId).AnyAsync())
    //    {
    //        IconModel iconToDelete = await siteContext.Icons.FirstAsync(icon => icon.IconModelId == iconId);

    //        _ = siteContext.Icons.Remove(iconToDelete);
    //        _ = await siteContext.SaveChangesAsync();
    //    }
    //}

    //#endregion

    //#region Получить иконку из БД по GUID или имени файла

    //public async Task<IconModel> GetIconByGuidOrFileNameAsync(string iconObject)
    //{
    //    // Поиск иконки по GUID
    //    if (Guid.TryParse(iconObject, out Guid iconIdGuid) & await siteContext.Icons.Where(icon => icon.IconModelId == iconIdGuid).AnyAsync())
    //    {
    //        return await siteContext.Icons.FirstAsync(icon => icon.IconModelId == iconIdGuid);
    //    }
    //    // Поиск иконки по названию файла
    //    else if (await siteContext.Icons.Where(icon => icon.IconFileName == iconObject).AnyAsync())
    //    {
    //        return await siteContext.Icons.FirstAsync(icon => icon.IconFileName == iconObject);
    //    }
    //    // Если ничего не найдено, выводим картинку NoImage
    //    else
    //    {
    //        return await siteContext.Icons.FirstAsync(icon => icon.IconFileName == "icon-144.webp");
    //    }
    //}
    //#endregion

    //#region Сохранить данные иконки в БД

    //public async Task SaveChangesInIconsAsync() => await siteContext.SaveChangesAsync();

    //#endregion
}
