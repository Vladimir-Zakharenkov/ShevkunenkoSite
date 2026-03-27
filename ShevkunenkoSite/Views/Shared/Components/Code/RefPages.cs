namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class RefPages(
    IPageInfoRepository pageInfoContext,
    IMovieFileRepository movieContext,
    IFilmFileRepository filmContext,
    IImageFileRepository imageContext) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        #region Инициализация страницы (HttpContext)

        var pageInfoModel = await pageInfoContext.GetPageInfoByPathAsync(HttpContext);

        #endregion

        #region Инициализация RefPagesViewModel

        RefPagesViewModel refsUnderPage = new();

        #endregion

        if (pageInfoModel.PageLinks == false
            & pageInfoModel.PageLinks2 == false
            & pageInfoModel.PageLinksByFilters == false
            & pageInfoModel.VideoLinks == false
            & pageInfoModel.PhotoLinks == false)
        {
            return View("Empty");
        }
        else if (string.IsNullOrEmpty(pageInfoModel.PageFilterOut)
            & string.IsNullOrEmpty(pageInfoModel.RefPages)
            & string.IsNullOrEmpty(pageInfoModel.RefPages2)
            & string.IsNullOrEmpty(pageInfoModel.VideoFilterOut)
            & string.IsNullOrEmpty(pageInfoModel.PhotoFilterOut))
        {
            return View("Empty");
        }
        else
        {
            #region Словарь страниц по текстовому фильтрам

            if (pageInfoModel.PageFilterOut != null && pageInfoModel.PageFilterOut != string.Empty && pageInfoModel.PageLinksByFilters == true)
            {
                refsUnderPage.DictionaryOfPages = [];

                string[] pageFilterOut = pageInfoModel.PageFilterOut.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pageFilterOut.Length > 0)
                {
                    for (int i = 0; i < pageFilterOut.Length; i++)
                    {
                        if (await pageInfoContext.PagesInfo.Where(sitePage => sitePage.PageFilter.Contains((pageFilterOut[i] + ',').Trim())).AnyAsync())
                        {
                            var listOfFilterOut = await pageInfoContext.PagesInfo.Where(p => p.PageFilter.Contains((pageFilterOut[i] + ',').Trim())).ToListAsync();

                            _ = listOfFilterOut.Distinct();

                            listOfFilterOut.Sort((page1, page2) => page1.SortOfPage.CompareTo(page2.SortOfPage));

                            refsUnderPage.DictionaryOfPages[pageFilterOut[i]] = listOfFilterOut;
                        }
                    }
                }
            }

            #endregion

            #region Словарь картинок по текстовому фильтрам

            if (pageInfoModel.PhotoFilterOut != null && pageInfoModel.PhotoFilterOut != string.Empty && pageInfoModel.PhotoLinks == true)
            {
                refsUnderPage.DictionaryOfPictures = [];

                string[] photoFilterOut = pageInfoModel.PhotoFilterOut.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (photoFilterOut.Length > 0)
                {
                    for (int i = 0; i < photoFilterOut.Length; i++)
                    {
                        if (await imageContext.ImageFiles.Where(p => p.SearchFilter.Contains(photoFilterOut[i] + ',')).AnyAsync())
                        {
                            var listOfFilterOut = await imageContext.ImageFiles.Where(p => p.SearchFilter.Contains(photoFilterOut[i] + ',')).ToListAsync();

                            _ = listOfFilterOut.Distinct().OrderBy(s => s.SortOfPicture);

                            refsUnderPage.DictionaryOfPictures[photoFilterOut[i]] = listOfFilterOut;
                        }
                    }
                }
            }

            #endregion

            #region Словарь списков фильмов (films) по текстовому фильтрам

            if (string.IsNullOrEmpty(pageInfoModel.VideoFilterOut) == false && pageInfoModel.VideoLinks == true)
            {
                string[] videoFilterOut = pageInfoModel.VideoFilterOut.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (videoFilterOut.Length > 0)
                {
                    for (int i = 0; i < videoFilterOut.Length; i++)
                    {
                        if (await filmContext.FilmFiles.Where(film => film.SearchFilterForFilm != null && film.SearchFilterForFilm.Contains(videoFilterOut[i])).AnyAsync())
                        {
                            var listOfFilm = await filmContext.FilmFiles.Where(film => film.SearchFilterForFilm != null && film.SearchFilterForFilm.Contains(videoFilterOut[i] + ',')).ToListAsync();

                            _ = listOfFilm.Distinct();

                            refsUnderPage.DictionaryOfFilms[videoFilterOut[i]] = listOfFilm;
                        }
                    }
                }
            }

            #endregion

            #region Список списков фильмов (movies) по текстовому фильтрам

            if (pageInfoModel.VideoFilterOut != null && pageInfoModel.VideoFilterOut != string.Empty && pageInfoModel.VideoLinks == true)
            {
                refsUnderPage.ListOfVideoLinksViewModel = [];

                string[] videoFilterOut = pageInfoModel.VideoFilterOut.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (videoFilterOut.Length > 0)
                {
                    for (int i = 0; i < videoFilterOut.Length; i++)
                    {
                        if (await movieContext.MovieFiles.Where(p => p.SearchFilter.Contains(videoFilterOut[i])).AnyAsync())
                        {
                            VideoLinksViewModel videoLinksViewModel = new()
                            {
                                HeadTitleForVideoLinks = videoFilterOut[i],
                                IsImage = false,
                                IconType = "webicon300",
                                SearchFilter = videoFilterOut[i],
                                MovieInMainList = true,
                                IsPartsMoreOne = true
                            };

                            refsUnderPage.ListOfVideoLinksViewModel.Add(videoLinksViewModel);
                        }
                    }
                    _ = refsUnderPage.ListOfVideoLinksViewModel.Distinct();
                }
            }

            #endregion

            #region Ссылки на связанные страницы по GUID (1)

            if (pageInfoModel.RefPages != null && pageInfoModel.RefPages != string.Empty && pageInfoModel.PageLinks == true)
            {
                refsUnderPage.LinksToPagesByGuid = [];

                string[] pageIdOut = pageInfoModel.RefPages.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pageIdOut.Length > 0)
                {
                    foreach (string pageId in pageIdOut)
                    {
                        if (Guid.TryParse(pageId, out Guid pageGuid))
                        {
                            if (await pageInfoContext.PagesInfo.Where(p => p.PageInfoModelId == pageGuid).AnyAsync())
                            {
                                refsUnderPage.LinksToPagesByGuid.Add(await pageInfoContext.PagesInfo.FirstAsync(p => p.PageInfoModelId == pageGuid));
                            }
                        }
                    }

                    _ = refsUnderPage.LinksToPagesByGuid.Distinct().OrderBy(p => p.SortOfPage);
                }
            }

            #endregion

            #region Ссылки на связанные страницы по GUID (2)

            if (pageInfoModel.RefPages2 != null && pageInfoModel.RefPages2 != string.Empty && pageInfoModel.PageLinks2 == true)
            {
                refsUnderPage.LinksToPagesByGuid2 = [];

                string[] pageIdOut2 = pageInfoModel.RefPages2.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (pageIdOut2.Length > 0)
                {
                    foreach (string pageId2 in pageIdOut2)
                    {
                        if (Guid.TryParse(pageId2, out Guid pageGuid2))
                        {
                            if (await pageInfoContext.PagesInfo.Where(p => p.PageInfoModelId == pageGuid2).AnyAsync())
                            {
                                refsUnderPage.LinksToPagesByGuid2.Add(await pageInfoContext.PagesInfo.FirstAsync(p => p.PageInfoModelId == pageGuid2));
                            }
                        }
                    }

                    _ = refsUnderPage.LinksToPagesByGuid2.Distinct().OrderBy(p => p.SortOfPage);
                }
            }

            #endregion
        }

        if (refsUnderPage.DictionaryOfPages.Count < 1
            & refsUnderPage.DictionaryOfPictures.Count < 1
            & refsUnderPage.DictionaryOfFilms.Count < 1
            & refsUnderPage.ListOfVideoLinksViewModel.Count < 1
            & refsUnderPage.LinksToPagesByGuid.Count < 1
            & refsUnderPage.LinksToPagesByGuid2.Count < 1
            )
        {
            return View("Empty");
        }
        else
        {
            return View(refsUnderPage);
        }
    }
}