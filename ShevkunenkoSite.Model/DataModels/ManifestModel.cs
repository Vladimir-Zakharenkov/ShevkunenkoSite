namespace ShevkunenkoSite.Models.DataModels;

[NotMapped]
public class ManifestModel
{
    public string Background_color { get; set; } = "antiquewhite";

    //public string Categories { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Display { get; set; } = "standalone";

    public string[] display_override { get; set; } = ["window-controls-overlay"];

    //public string[] File_handlers { get; set; } = [];

    public string[] Icons { get; set; } = [];

    public string Id { get; set; } = string.Empty;

    //public string Launch_handler { get; set; } = string.Empty;

    public string Lang { get; set; } = "ru-Ru";

    public string Name { get; set; } = "Сайт памяти Сергея Шевкуненко";

    //public string Note_taking { get; set; } = string.Empty;

    public string Orientation { get; set; } = "any";

    //public string Prefer_related_applications { get; set; } = string.Empty;

    //public string[] Protocol_handlers { get; set; } = [];

    //public string[] Related_applications { get; set; } = [];

    public string Scope { get; set; } = "/";

    //public string[] Scope_extensions { get; set; } = [];

    //public string[] Screenshots { get; set; } = [];

    //public string Serviceworker { get; set; } = string.Empty;

    //public string Share_target { get; set; } = string.Empty;

    public string Short_name { get; set; } = "Сайт памяти Сергея Шевкуненко";

    //public string[] Shortcuts { get; set; } = [];

    public string Start_url { get; set; } = "https://shevkunenko.site";

    public string Theme_color { get; set; } = "hsl(0 60% 50%)";
}
