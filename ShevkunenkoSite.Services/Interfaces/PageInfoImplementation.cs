namespace ShevkunenkoSite.Services.Interfaces;

using Microsoft.AspNetCore.Http;

public class PageInfoImplementation(SiteDbContext siteContext) : IPageInfoRepository
{
    #region Константы путей

    // Вынесено из "магических строк". Имена публичные, чтобы при необходимости
    // можно было использовать их в других местах без дублирования.
    private const string DefaultPagePath = "/shevkunenko/index";
    private const string NotFoundPagePath = "/shevkunenko/error404";

    #endregion

    #region Инициализация PagesInfo

    /// <summary>
    /// Отслеживаемый запрос — оставлен для обратной совместимости.
    /// </summary>
    public IQueryable<PageInfoModel> PagesInfo => BuildPageQuery(tracking: true);

    /// <summary>
    /// Запрос без tracking — рекомендуется для отображения.
    /// </summary>
    public IQueryable<PageInfoModel> PagesInfoNoTracking => BuildPageQuery(tracking: false);

    private IQueryable<PageInfoModel> BuildPageQuery(bool tracking)
    {
        IQueryable<PageInfoModel> query = siteContext.PageInfo
            .Include(iconType => iconType.IconType).ThenInclude(icon => icon.IconList)
            .Include(image => image.ImageFileModel)
            .Include(text => text.TextInfo)
                .ThenInclude(book => book != null ? book.BooksAndArticlesModel : null)
                .ThenInclude(articleLogo => articleLogo!.LogoOfArticle != null ? articleLogo.LogoOfArticle : null)
            .Include(background => background.BackgroundFileModel)
            .Include(audioFile => audioFile.AudioInfo)
            // TODO: убрать nullable для картинки фильма
            .Include(movie => movie.MovieFile)
                .ThenInclude(movieImage => movieImage != null ? movieImage.ImageFileModel : null)
            .Include(movie => movie.MovieFile)
                .ThenInclude(moviePoster => moviePoster != null ? moviePoster.MoviePoster : null);

        return tracking ? query : query.AsNoTracking();
    }

    #endregion

    #region Определить страницу в базе данных по запросу

    public async Task<PageInfoModel> GetPageInfoByPathAsync(HttpContext httpContext)
    {
        #region Путь и данные запроса

        string pagePath = httpContext.Request.Path.ToString().ToLower().TrimEnd('/');

        IQueryCollection pageQuery = httpContext.Request.Query;

        string routData = BuildRouteData(pageQuery);

        #endregion

        #region Поиск совпадения пути и данных

        if (pageQuery.Count > 0 && !string.IsNullOrEmpty(routData))
        {
            // Перебираем постепенно усечённые варианты routData.
            string? currentRoutData = routData;

            while (!string.IsNullOrEmpty(currentRoutData))
            {
                PageInfoModel? page = await FindPageWithRouteDataAsync(pagePath, currentRoutData);
                if (page != null)
                {
                    return page;
                }

                currentRoutData = TrimLastRouteParameter(currentRoutData);
            }

            // Если с данными не нашли — пробуем без данных (routData = "").
        }

        #endregion

        #region Поиск без данных

        return await FindPageWithoutRouteDataAsync(pagePath);

        #endregion
    }

    public async Task<PageInfoModel?> FindPageByPathAsync(
        string path,
        string? routData,
        CancellationToken cancellationToken = default)
    {
        string pagePath = (path ?? string.Empty).ToLower().TrimEnd('/');

        if (!string.IsNullOrEmpty(routData))
        {
            PageInfoModel? page = await FindPageWithRouteDataAsync(pagePath, routData, cancellationToken);
            if (page != null)
            {
                return page;
            }

            // Пробуем без данных
        }

        try
        {
            return await FindPageWithoutRouteDataAsync(pagePath, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            // Страница не найдена и нет error404 — вернём null вместо исключения.
            return null;
        }
    }

    #endregion

    #region Вспомогательные методы поиска

    /// <summary>
    /// Собирает строку вида "?key1=value1&key2=value2" из коллекции query.
    /// </summary>
    private static string BuildRouteData(IQueryCollection pageQuery)
    {
        string routData = string.Empty;
        foreach (var item in pageQuery)
        {
            routData = routData.Length == 0
                ? $"?{item.Key}={item.Value}"
                : $"{routData}&{item.Key}={item.Value}";
        }

        return routData;
    }

    /// <summary>
    /// Усекает последний параметр в строке запроса.
    /// Специфичная логика для videohosting сохранена для совместимости.
    /// </summary>
    private static string TrimLastRouteParameter(string routData)
    {
        if (routData.Contains("&videohosting=https://vk.com"))
        {
            return routData[..routData.LastIndexOf("&videohosting=https://vk.com", StringComparison.Ordinal)];
        }

        if (routData.Contains("&videohosting=https://vkvideo.ru"))
        {
            return routData[..routData.LastIndexOf("&videohosting=https://vkvideo.ru", StringComparison.Ordinal)];
        }

        int lastAmp = routData.LastIndexOf('&');
        return lastAmp >= 0 ? routData[..lastAmp] : string.Empty;
    }

    /// <summary>
    /// Поиск страницы с совпадением пути и данных.
    /// Все варианты (FullPath, FullPath+/index, NickName1..4, NickName+/index)
    /// проверяются одним запросом на вариант через FirstOrDefaultAsync.
    /// </summary>
    private async Task<PageInfoModel?> FindPageWithRouteDataAsync(
        string pagePath,
        string routData,
        CancellationToken cancellationToken = default)
    {
        string pathWithIndex = pagePath + "/index";

        // Один LINQ-запрос с OR-условиями — вместо 10 отдельных Any/First.
        // EF Core транслирует в один SQL-запрос.
        PageInfoModel? page = await PagesInfo.FirstOrDefaultAsync(
            p => (p.PageFullPath == pagePath
                  || p.PageFullPath == pathWithIndex
                  || p.PagePathNickName == pagePath
                  || p.PagePathNickName2 == pagePath
                  || p.PagePathNickName3 == pagePath
                  || p.PagePathNickName4 == pagePath
                  || p.PagePathNickName == pathWithIndex
                  || p.PagePathNickName2 == pathWithIndex
                  || p.PagePathNickName3 == pathWithIndex
                  || p.PagePathNickName4 == pathWithIndex)
                 && p.RoutData == routData,
            cancellationToken);

        return page;
    }

    /// <summary>
    /// Поиск страницы без учёта данных (routData).
    /// Сохраняет прежнее поведение: если у найденной страницы есть RoutData,
    /// но путь не заканчивается на "index" — вернуть error404.
    /// </summary>
    private async Task<PageInfoModel> FindPageWithoutRouteDataAsync(
        string pagePath,
        CancellationToken cancellationToken = default)
    {
        #region Пустой путь -> главная

        if (string.IsNullOrEmpty(pagePath))
        {
            return await GetPageOrThrowAsync(DefaultPagePath, cancellationToken);
        }

        #endregion

        #region Совпадение FullPath

        PageInfoModel? byFullPath = await PagesInfo.FirstOrDefaultAsync(
            page => page.PageFullPath == pagePath,
            cancellationToken);

        if (byFullPath != null)
        {
            // Если у страницы есть RoutData, а путь без "/index" — отдаём 404 (как раньше).
            if (!string.IsNullOrEmpty(byFullPath.RoutData) && !pagePath.EndsWith("index", StringComparison.Ordinal))
            {
                return await GetPageOrThrowAsync(NotFoundPagePath, cancellationToken);
            }

            return byFullPath;
        }

        #endregion

        #region Совпадения псевдонимов

        PageInfoModel? byNickName = await PagesInfo.FirstOrDefaultAsync(
            p => p.PagePathNickName == pagePath
                 || p.PagePathNickName2 == pagePath
                 || p.PagePathNickName3 == pagePath
                 || p.PagePathNickName4 == pagePath,
            cancellationToken);

        if (byNickName != null)
        {
            return byNickName;
        }

        #endregion

        #region Совпадение FullPath + /index

        string pathWithIndex = pagePath + "/index";

        PageInfoModel? byFullPathWithIndex = await PagesInfo.FirstOrDefaultAsync(
            p => p.PageFullPath == pathWithIndex,
            cancellationToken);

        if (byFullPathWithIndex != null)
        {
            return byFullPathWithIndex;
        }

        #endregion

        #region Совпадение псевдонимов + /index

        PageInfoModel? byNickNameWithIndex = await PagesInfo.FirstOrDefaultAsync(
            p => p.PagePathNickName == pathWithIndex
                 || p.PagePathNickName2 == pathWithIndex
                 || p.PagePathNickName3 == pathWithIndex
                 || p.PagePathNickName4 == pathWithIndex,
            cancellationToken);

        if (byNickNameWithIndex != null)
        {
            return byNickNameWithIndex;
        }

        #endregion

        #region Ничего не найдено -> error404

        return await GetPageOrThrowAsync(NotFoundPagePath, cancellationToken);

        #endregion
    }

    /// <summary>
    /// Возвращает страницу по полному пути или бросает InvalidOperationException,
    /// если её нет (сохраняем поведение FirstAsync).
    /// </summary>
    private async Task<PageInfoModel> GetPageOrThrowAsync(
        string fullPath,
        CancellationToken cancellationToken = default)
    {
        PageInfoModel? page = await PagesInfo.FirstOrDefaultAsync(
            p => p.PageFullPath == fullPath,
            cancellationToken);

        return page ?? throw new InvalidOperationException(
            $"Страница с путём '{fullPath}' не найдена в базе данных.");
    }

    #endregion

    #region Поиск по идентификатору

    public async Task<PageInfoModel?> GetPageByIdAsync(
        Guid pageId,
        CancellationToken cancellationToken = default)
    {
        return await PagesInfo.FirstOrDefaultAsync(
            p => p.PageInfoModelId == pageId,
            cancellationToken);
    }

    public async Task<bool> PageExistsAsync(
        Guid pageId,
        CancellationToken cancellationToken = default)
    {
        return await siteContext.PageInfo.AnyAsync(
            p => p.PageInfoModelId == pageId,
            cancellationToken);
    }

    #endregion

    #region Сохранить изменения

    public async Task SaveChangesInPageAsync()
    {
        _ = await siteContext.SaveChangesAsync();
    }

    public async Task SaveChangesInPageAsync(CancellationToken cancellationToken = default)
    {
        _ = await siteContext.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Добавить страницу

    public async Task AddNewPageAsync(PageInfoModel page)
    {
        _ = await siteContext.PageInfo.AddAsync(page);
        await SaveChangesInPageAsync();
    }

    public async Task AddNewPageWithoutSaveAsync(
        PageInfoModel page,
        CancellationToken cancellationToken = default)
    {
        _ = await siteContext.PageInfo.AddAsync(page, cancellationToken);
    }

    #endregion

    #region Удалить страницу

    public async Task DeletePageAsync(Guid pageId)
    {
        PageInfoModel? pageToDelete = await siteContext.PageInfo
            .FirstOrDefaultAsync(i => i.PageInfoModelId == pageId);

        if (pageToDelete != null)
        {
            _ = siteContext.PageInfo.Remove(pageToDelete);
            _ = await siteContext.SaveChangesAsync();
        }
    }

    public async Task<bool> DeletePageWithoutSaveAsync(
        Guid pageId,
        CancellationToken cancellationToken = default)
    {
        PageInfoModel? pageToDelete = await siteContext.PageInfo
            .FirstOrDefaultAsync(i => i.PageInfoModelId == pageId, cancellationToken);

        if (pageToDelete == null)
        {
            return false;
        }

        _ = siteContext.PageInfo.Remove(pageToDelete);
        return true;
    }

    #endregion
}