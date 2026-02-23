namespace ShevkunenkoSite.Services.Extensions;

public static class IconModelExtension
{
    public static IEnumerable<IconModel> IconSearch(this IEnumerable<IconModel> iconModel, string? iconSearchString)
    {
        foreach (var foundIcon in iconModel)
        {
            if (foundIcon.IconFileName.Contains((iconSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    | foundIcon.PathToIcon.Contains((iconSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    | foundIcon.IconMimeType.Contains((iconSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    | foundIcon.RelForIcon.Contains((iconSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    | foundIcon.IconSize.Contains((iconSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    | foundIcon.IconPurpose.Contains((iconSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    )
            {
                yield return foundIcon;
            }
        }
    }
}