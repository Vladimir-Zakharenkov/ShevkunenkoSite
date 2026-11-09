namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class VideoCard(
    IPageInfoRepository pageInfoContext, 
    IImageFileRepository imageFileContext
    ) : ViewComponent
{
    string pathForSeries = string.Empty;

    string imageForVideo = string.Empty;

    Guid? imageForVideoGuid;

    PageInfoModel pageForMoreOneParts = new();

    ImageFileModel imageVideo = new();

    public async Task<IViewComponentResult> InvokeAsync(
        MovieFileModel movieFileModel, 
        bool IsPartsMoreOne, 
        bool? isImage, 
        string?  iconType = "webicon300", 
        bool? linkToInfoAboutMovie = null)
    {
        // определяем страницу серий многосерийного фильма
        if (IsPartsMoreOne == true & movieFileModel.PageForMovieSeriesId != null & linkToInfoAboutMovie !=true)
        {
            if (await pageInfoContext.PagesInfoNoTracking.Where(p => p.PageInfoModelId == movieFileModel.PageForMovieSeriesId).AnyAsync())
            {
                pageForMoreOneParts = await pageInfoContext.PagesInfoNoTracking.FirstAsync(p => p.PageInfoModelId == movieFileModel.PageForMovieSeriesId);

                pathForSeries = pageForMoreOneParts.PageFullPathWithData;
            }
            else
            {
                pathForSeries = string.Empty;
            }
        }

        if (isImage == true & await imageFileContext.ImageFiles.Where(i => i.ImageFileModelId == movieFileModel.ImageFileModelId).AnyAsync())
        {
            imageVideo = await imageFileContext.ImageFiles.FirstAsync(i => i.ImageFileModelId == movieFileModel.ImageFileModelId);

            imageForVideo = imageVideo.ImageFileName;

            imageForVideoGuid = imageVideo.ImageFileModelId;
        }
        else if (isImage == false & movieFileModel.MoviePosterId != null)
        {
            imageForVideoGuid = movieFileModel.MoviePosterId!;
        }
        else
        {
            imageForVideo = "no-image.png";
        }

        return View(new VideoCardViewModel()
        {
            MovieInfo = movieFileModel,
            PathForSeries = pathForSeries,
            ImageForVideo = imageForVideo,
            ImageForVideoGuid = imageForVideoGuid,
            IsImageIcon = isImage,
            IconType = iconType ?? "webicon300",
            LinkToInfoAboutMovie = linkToInfoAboutMovie
        });
    }
}