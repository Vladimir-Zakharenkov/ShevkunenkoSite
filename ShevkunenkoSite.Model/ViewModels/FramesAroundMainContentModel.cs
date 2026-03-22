namespace ShevkunenkoSite.Models.ViewModels;

public class FramesAroundMainContentModel
{
    [Display(Name = "Кадры слева :")]
    public IEnumerable<ImageFileModel> FramesOnTheLeft { get; set; } = [];

    [Display(Name = "Кадры справа :")]
    public IEnumerable<ImageFileModel> FramesOnTheRight { get; set; } = [];
}