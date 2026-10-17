namespace ShevkunenkoSite.Services.Extensions;

public static class StringExtensions
{
    public static string StartOfDescription(this string description)
    {
        int whitespace;

        if (description.Length < 100)
        {
            whitespace = description.Length;
        }
        else
        {
            whitespace = description.LastIndexOf(' ', 100);
        }

        var startOfDescription = description[..whitespace] + "...";

        return startOfDescription;
    }

    public static string SpaceToDash(this string s) =>
        string.IsNullOrWhiteSpace(s) ? string.Empty : s.Replace(' ', '-');

}