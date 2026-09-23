namespace ShevkunenkoSite.Services.Extensions;

public static class HttpRequestExtensions
{
    public static bool IsAdminRequest(this HttpRequest request)
        => request.Path.Value?.Contains("admin", StringComparison.OrdinalIgnoreCase) == true;
}