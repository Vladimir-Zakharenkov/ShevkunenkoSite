namespace ShevkunenkoSite.Models.DataModels;

[NotMapped]
public class ManifestModel
{
    public string Background_color { get; set; } = "antiquewhite";

    public string Description { get; set; } = string.Empty;

    public string Display { get; set; } = "standalone";

    public string[] Display_override { get; set; } = ["window-controls-overlay"];

    public IEnumerable<IconForManifest> Icons { get; set; } = [];

    public string Id { get; set; } = string.Empty;

    public string Lang { get; set; } = "ru-Ru";

    public string Name { get; set; } = "Сайт памяти Сергея Шевкуненко";

    public string Orientation { get; set; } = "any";

    public string Scope { get; set; } = "/";

    public string Short_name { get; set; } = "Сайт памяти Сергея Шевкуненко";

    public string Start_url { get; set; } = "https://shevkunenko.site";

    public string Theme_color { get; set; } = "hsl(0 60% 50%)";

    #region Не используемые параметры

    //public string Categories { get; set; } = string.Empty;

    //public string[] File_handlers { get; set; } = [];

    //public string Launch_handler { get; set; } = string.Empty;

    //public string Note_taking { get; set; } = string.Empty;

    //public string[] Scope_extensions { get; set; } = [];

    //public string[] Screenshots { get; set; } = [];

    //public string Serviceworker { get; set; } = string.Empty;

    //public string Share_target { get; set; } = string.Empty;

    //public string[] Shortcuts { get; set; } = [];

    //public string Prefer_related_applications { get; set; } = string.Empty;

    //public string[] Protocol_handlers { get; set; } = [];

    //public string[] Related_applications { get; set; } = [];

    #endregion
}
