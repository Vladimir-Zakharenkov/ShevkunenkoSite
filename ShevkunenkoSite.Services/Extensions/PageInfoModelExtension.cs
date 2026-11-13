namespace ShevkunenkoSite.Services.Extensions;

public static class PageInfoModelExtension
{
    #region Поиск в памяти — существующая логика

    public static IEnumerable<PageInfoModel> PageSearch(
        this IEnumerable<PageInfoModel> pages,
        string? searchString)
    {
        ArgumentNullException.ThrowIfNull(pages);

        string search = searchString?.Trim() ?? string.Empty;

        if (search.Length == 0)
            return pages;

        return pages.Where(page =>
            page.PageTitle.Contains(
                search, StringComparison.OrdinalIgnoreCase)
            || page.PageDescription.Contains(
                search, StringComparison.OrdinalIgnoreCase)
            || page.PageKeyWords.Contains(
                search, StringComparison.OrdinalIgnoreCase)
            || page.PageCardText.Contains(
                search, StringComparison.OrdinalIgnoreCase)
            || page.PageFullPathWithData.Contains(
                search, StringComparison.OrdinalIgnoreCase)
            || page.PagePathNickName.Contains(
                search, StringComparison.OrdinalIgnoreCase)
            || page.PagePathNickName2.Contains(
                search, StringComparison.OrdinalIgnoreCase)
            || page.PagePathNickName3.Contains(
                search, StringComparison.OrdinalIgnoreCase)
            || page.PagePathNickName4.Contains(
                search, StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region Поиск непосредственно в SQL Server

    public static IQueryable<PageInfoModel> PageSearchQuery(
    this IQueryable<PageInfoModel> pages,
    string? searchString)
    {
        ArgumentNullException.ThrowIfNull(pages);

        string search = searchString?.Trim() ?? string.Empty;

        if (search.Length == 0)
            return pages;

        // Экранируем специальные символы LIKE
        string escaped = search
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_")
            .Replace("[", "[[]");

        string pattern = $"%{escaped}%";

        return pages.Where(page =>
            EF.Functions.Like(page.PageTitle, pattern, "\\")
            || EF.Functions.Like(page.PageDescription, pattern, "\\")
            || EF.Functions.Like(page.PageKeyWords, pattern, "\\")
            || EF.Functions.Like(page.PageCardText, pattern, "\\")
            || EF.Functions.Like(page.PageFullPathWithData, pattern, "\\")
            || EF.Functions.Like(page.PagePathNickName, pattern, "\\")
            || EF.Functions.Like(page.PagePathNickNameWithData, pattern, "\\")
            || EF.Functions.Like(page.PagePathNickName2, pattern, "\\")
            || EF.Functions.Like(page.PagePathNickName3, pattern, "\\")
            || EF.Functions.Like(page.PagePathNickName4, pattern, "\\"));
    }

    #endregion
}