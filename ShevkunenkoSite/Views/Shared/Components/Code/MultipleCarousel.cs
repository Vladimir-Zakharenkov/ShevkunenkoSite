namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class MultipleCarousel(
    IImageFileRepository imageContext, 
    IPageInfoRepository pageInfoContext, 
    IMovieFileRepository movieContext
    ) : ViewComponent
{
    // Три группы картинок
    public ImageFileModel[] firstGroup = [];
    public ImageFileModel[] secondGroup = [];
    public ImageFileModel[] thirdGroup = [];

    public async Task<IViewComponentResult> InvokeAsync(MovieFileModel movieCarousel)
    {
        // Экземпляр текущей страницы
        PageInfoModel pageInfoModel = await pageInfoContext.GetPageInfoByPathAsync(HttpContext);

        // Массив картинок
        ImageFileModel[] pictures = [];

        // Экземпляр полной версии фильма
        MovieFileModel? fullMovie = null;

        // Создаем экземпляр полной версии фильма если она существует
        if (await movieContext.MovieFiles.Where(m => m.MovieFileModelId == movieCarousel.FullMovieID).AnyAsync())
        {
            fullMovie = await movieContext.MovieFiles.FirstAsync(m => m.MovieFileModelId == movieCarousel.FullMovieID);
        }

        if (movieCarousel.MovieCaption.Contains("Интервью"))
        {
            pictures = await imageContext.ImageFiles
                .Where(p => p.SearchFilter.Contains("Криминальная звезда," ?? string.Empty))
                .ToArrayAsync();
        }
        else
        {
            pictures = await imageContext.ImageFiles
            .Where(p => p.SearchFilter.Contains($"{movieCarousel.MovieCaption}," ?? string.Empty))
            .ToArrayAsync();
        }

        int numberOfPictures = pictures.Length;

        if (numberOfPictures >= 18)
        {
            firstGroup = pictures.Take(6).ToArray();
            secondGroup = pictures.Skip(6).Take(6).ToArray();
            thirdGroup = pictures.Skip(12).Take(6).ToArray();
        }
        else if (numberOfPictures >= 12 & numberOfPictures < 18)
        {
            firstGroup = pictures.Take(4).ToArray();
            secondGroup = pictures.Skip(4).Take(4).ToArray();
            thirdGroup = pictures.Skip(8).Take(4).ToArray();
        }
        else
        {
            firstGroup = pictures;
            secondGroup = pictures;
            thirdGroup = pictures;
        }

        return View(new ThreeCarouselsViewModel
        {
            PageInfo = pageInfoModel,
            MovieCarousel = movieCarousel,
            FirstCarousel = firstGroup,
            SecondCarousel = secondGroup,
            ThirdCarousel = thirdGroup
        });
    }
}