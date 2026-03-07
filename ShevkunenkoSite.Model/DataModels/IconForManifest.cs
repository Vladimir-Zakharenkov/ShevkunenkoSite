namespace ShevkunenkoSite.Models.DataModels;

[NotMapped]
public record class IconForManifest
{
    public string src { get; set; } = string.Empty;

    public string sizes { get; set; } = string.Empty;

    public string type { get; set; } = string.Empty;

    public string purpose { get; set; } = string.Empty;
}