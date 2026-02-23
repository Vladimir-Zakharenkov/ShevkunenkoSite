namespace ShevkunenkoSite.Services.Extensions;

public static class ImageFileModelExtension
{
    public static IEnumerable<ImageFileModel> ImageSearch(this IEnumerable<ImageFileModel> imageFileModel, string? imageSearchString)
    {
        foreach (var foundImage in imageFileModel)
        {
            if (foundImage.ImageCaption.Contains((imageSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    | foundImage.ImageDescription.Contains((imageSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    | foundImage.SearchFilter.Contains((imageSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    | foundImage.WebImageFileName.Contains((imageSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    | foundImage.ImageFileName.Contains((imageSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                )
            {
                yield return foundImage;
            }
        }
    }
}