namespace ShevkunenkoSite.Services.Interfaces;

public class IconTypeImplementation(SiteDbContext siteContext) : IIconTypeRepository
{
    #region Все файлы иконок в БД

    public IQueryable<IconTypeModel> IconTypes => siteContext.IconTypes
        .Include(icons => icons.IconList);

    #endregion

    #region Добавить новую иконку в БД

    public async Task AddNewIconTypeAsync(IconTypeModel iconType)
    {
        await siteContext.IconTypes.AddAsync(iconType);

        await SaveChangesInIconTypesAsync();
    }

    #endregion

    #region Сохранить данные иконки в БД

    public async Task SaveChangesInIconTypesAsync() => await siteContext.SaveChangesAsync();

    #endregion
}