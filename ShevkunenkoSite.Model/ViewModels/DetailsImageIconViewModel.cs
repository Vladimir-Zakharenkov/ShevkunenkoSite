// Ignore Spelling: Infostyle

namespace ShevkunenkoSite.Models.ViewModels;

public class DetailsImageIconViewModel
{
    public const string MainInfostyle = "btn btn-outline-primary p-1 block-shadow";

    public ImageFileModel ImageItem { get; set; } = new();

    public string? IconType { get; set; }

    public bool FileExists { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string WebImageButtonInfostyle { get; set; } = MainInfostyle;

    public string ImageButtonInfostyle { get; set; } = MainInfostyle;

    public string WebhdButtonInfostyle { get; set; } = MainInfostyle;

    public string HdButtonInfostyle { get; set; } = MainInfostyle;

    public string Icon300ButtonInfostyle { get; set; } = MainInfostyle;

    public string WebIcon300ButtonInfostyle { get; set; } = MainInfostyle;

    public string Icon200ButtonInfostyle { get; set; } = MainInfostyle;

    public string WebIcon200ButtonInfostyle { get; set; } = MainInfostyle;

    public string Icon100ButtonInfostyle { get; set; } = MainInfostyle;

    public string WebIcon100ButtonInfostyle { get; set; } = MainInfostyle;

    public string? CurrentCss { get; set; }
}