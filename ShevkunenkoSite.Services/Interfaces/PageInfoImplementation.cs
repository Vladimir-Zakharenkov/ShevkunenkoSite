namespace ShevkunenkoSite.Services.Interfaces;

public class PageInfoImplementation(SiteDbContext siteContext) : IPageInfoRepository
{
    #region Инициализация PagesInfo

    public IQueryable<PageInfoModel> PagesInfo => siteContext.PageInfo
        .Include(iconType => iconType.IconType).ThenInclude(icon => icon.IconList)
        .Include(image => image.ImageFileModel)
        .Include(text => text.TextInfo).ThenInclude(book => book != null ? book.BooksAndArticlesModel : null).ThenInclude(articleLogo => articleLogo!.LogoOfArticle != null ? articleLogo.LogoOfArticle : null)
        .Include(background => background.BackgroundFileModel)
        .Include(audioFile => audioFile.AudioInfo)
        // TODO: убрать nullable для картинки фильма
        .Include(movie => movie.MovieFile).ThenInclude(movieImage => movieImage != null ? movieImage.ImageFileModel : null)
        .Include(movie => movie.MovieFile).ThenInclude(moviePoster => moviePoster != null ? moviePoster.MoviePoster : null)
        //.Include(film => film.Film).ThenInclude(filmImage => filmImage != null ? filmImage.FilmImage : null)
        //.Include(film => film.Film).ThenInclude(filmPoster => filmPoster != null ? filmPoster.FilmPoster : null)
        ;

    #endregion

    #region Определить страницу в базе данных по запросу

    public async Task<PageInfoModel> GetPageInfoByPathAsync(HttpContext httpContext)
    {
        #region Запрос без данных

        string pagePath = httpContext.Request.Path.ToString().ToLower().TrimEnd('/');

        #endregion

        #region Коллекция данных запроса

        IQueryCollection pageQuery = httpContext.Request.Query;

        string routData = string.Empty; //Данные запроса

        #endregion

        #region Поиск совпадения пути и данных

        if (pageQuery.Count > 0)
        {
            #region Определить данные запроса

            foreach (var item in pageQuery)
            {
                routData = routData == string.Empty ? $"?{item.Key}={item.Value}" : $"{routData}&{item.Key}={item.Value}";
            }

            #endregion

            for (int i = 0; i < pageQuery.Count; i++)
            {
                #region Если совпали пути и данные

                if (await PagesInfo.Where(p => p.PageFullPath == pagePath & p.RoutData == routData).AnyAsync())
                {
                    return await PagesInfo.FirstAsync(p => p.PageFullPath == pagePath & p.RoutData == routData);
                }

                #endregion

                #region Если совпал путь + /index и данные

                else if (await PagesInfo.Where(p => p.PageFullPath == pagePath + "/index" & p.RoutData == routData).AnyAsync())
                {
                    return await PagesInfo.FirstAsync(p => p.PageFullPath == pagePath + "/index" & p.RoutData == routData);
                }

                #endregion

                #region Если совпал псевдоним (1) и данные

                else if (await PagesInfo.Where(p => p.PagePathNickName == pagePath & p.RoutData == routData).AnyAsync())
                {
                    return await PagesInfo.FirstAsync(p => p.PagePathNickName == pagePath & p.RoutData == routData);
                }

                #endregion

                #region Если совпал псевдоним (2) и данные

                else if (await PagesInfo.Where(p => p.PagePathNickName2 == pagePath & p.RoutData == routData).AnyAsync())
                {
                    return await PagesInfo.FirstAsync(p => p.PagePathNickName2 == pagePath & p.RoutData == routData);
                }

                #endregion

                else
                {
                    if (i == httpContext.Request.Query.Count - 1)
                    {
                        routData = string.Empty;
                    }
                    else
                    {
                        if (routData.Contains("&videohosting=https://vk.com"))
                        {
                            routData = routData[..routData.LastIndexOf("&videohosting=https://vk.com")];
                        }
                        else if (routData.Contains("&videohosting=https://vkvideo.ru"))
                        {
                            routData = routData[..routData.LastIndexOf("&videohosting=https://vkvideo.ru")];
                        }
                        else
                        {
                            routData = routData[..routData.LastIndexOf('&')];
                        }
                    }
                }
            }
        }

        #endregion

        #region Если в адресе нет данных или с данными поиск не удался

        if (routData == string.Empty)
        {
            #region Если нет строки запроса

            if (string.IsNullOrEmpty(pagePath))
            {
                return await PagesInfo.FirstAsync(page => page.PageFullPath == "/shevkunenko/index");
            }

            #endregion

            #region Совпадение пути

            else if (await PagesInfo.Where(page => page.PageFullPath == pagePath).AnyAsync())
            {
                var pageInfo = await PagesInfo.FirstAsync(page => page.PageFullPath == pagePath);

                #region Если совпал путь и не совпали данные (кроме пути с index )
                
                if (!string.IsNullOrEmpty(pageInfo.RoutData) & !pagePath.EndsWith("index"))
                {
                    return await PagesInfo.FirstAsync(page => page.PageFullPath == "/shevkunenko/error404");
                }

                #endregion

                return (pageInfo);
            }

            #endregion

            #region Совпадение пути и псевдонима (1)

            else if (await PagesInfo.Where(p => p.PagePathNickName == pagePath).AnyAsync())
            {
                return await PagesInfo.FirstAsync(p => p.PagePathNickName == pagePath);
            }

            #endregion

            #region Совпадение пути и псевдонима (2)

            else if (await PagesInfo.Where(p => p.PagePathNickName2 == pagePath).AnyAsync())
            {
                return await PagesInfo.FirstAsync(p => p.PagePathNickName2 == pagePath);
            }

            #endregion

            #region Совпадение пути и псевдонима (3)

            else if (await PagesInfo.Where(p => p.PagePathNickName3 == pagePath).AnyAsync())
            {
                return await PagesInfo.FirstAsync(p => p.PagePathNickName3 == pagePath);
            }

            #endregion

            #region Совпадение пути и псевдонима (4)

            else if (await PagesInfo.Where(p => p.PagePathNickName4 == pagePath).AnyAsync())
            {
                return await PagesInfo.FirstAsync(p => p.PagePathNickName4 == pagePath);
            }

            #endregion

            #region Совпадение пути + /index

            else if (await PagesInfo.Where(p => p.PageFullPath == pagePath + "/index").AnyAsync())
            {
                return await PagesInfo.FirstAsync(p => p.PageFullPath == pagePath + "/index");
            }

            #endregion

            #region Совпадение пути с псевдонимом (1) + /index

            else if (await PagesInfo.Where(p => p.PagePathNickName == pagePath + "/index").AnyAsync())
            {
                return await PagesInfo.FirstAsync(p => p.PagePathNickName == pagePath + "/index");
            }

            #endregion

            #region Совпадение пути с псевдонимом (2) + /index

            else if (await PagesInfo.Where(p => p.PagePathNickName2 == pagePath + "/index").AnyAsync())
            {
                return await PagesInfo.FirstAsync(p => p.PagePathNickName2 == pagePath + "/index");
            }

            #endregion

            #region Совпадение пути с псевдонимом (3) + /index

            else if (await PagesInfo.Where(p => p.PagePathNickName3 == pagePath + "/index").AnyAsync())
            {
                return await PagesInfo.FirstAsync(p => p.PagePathNickName3 == pagePath + "/index");
            }

            #endregion

            #region Совпадение пути с псевдонимом (4) + /index

            else if (await PagesInfo.Where(p => p.PagePathNickName4 == pagePath + "/index").AnyAsync())
            {
                return await PagesInfo.FirstAsync(p => p.PagePathNickName4 == pagePath + "/index");
            }

            #endregion

            else
            {
                return await PagesInfo.FirstAsync(p => p.PageFullPath == "/shevkunenko/error404");
            }
        }

        #endregion

        else
        {
            return await PagesInfo.FirstAsync(p => p.PageFullPath == "/shevkunenko/error404");
        }
    }

    #endregion

    #region Сохранить изменения

    public async Task SaveChangesInPageAsync()
    {
        _ = await siteContext.SaveChangesAsync();
    }

    #endregion

    #region Добавить страницу

    public async Task AddNewPageAsync(PageInfoModel page)
    {
        _ = await siteContext.PageInfo.AddAsync(page);
        await SaveChangesInPageAsync();
    }

    #endregion

    #region Удалить страницу

    public async Task DeletePageAsync(Guid pageId)
    {
        if (await siteContext.PageInfo.Where(i => i.PageInfoModelId == pageId).AnyAsync())
        {
            PageInfoModel pageToDelete = await siteContext.PageInfo.FirstAsync(i => i.PageInfoModelId == pageId);

            _ = siteContext.PageInfo.Remove(pageToDelete);
            _ = await siteContext.SaveChangesAsync();
        }
    }

    #endregion
}