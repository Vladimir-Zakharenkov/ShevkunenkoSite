namespace ShevkunenkoSite.Models.ViewModels;

public class HeadViewModel
{
    public PageInfoModel PageInfo { get; set; } = new PageInfoModel();

    public BooksAndArticlesModel? BookOrArticle { get; set; }

    public List<IconModel> IconsForHead { get; set; } = [];
}