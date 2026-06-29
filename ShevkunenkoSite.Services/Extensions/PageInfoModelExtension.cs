namespace ShevkunenkoSite.Services.Extensions;

public static class PageInfoModelExtension
{
    public static IEnumerable<PageInfoModel> PageSearch(this IEnumerable<PageInfoModel> pageInfoModel, string? pageSearchString)
    {
        if (pageInfoModel.Any())
        {
            foreach (var foundPage in pageInfoModel)
            {
                if (foundPage.PageTitle.Contains((pageSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    || foundPage.PageDescription.Contains((pageSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    || foundPage.PageKeyWords.Contains((pageSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    || foundPage.PageCardText.Contains((pageSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    )
                {
                    yield return foundPage;
                }
            }
        }
    }
}