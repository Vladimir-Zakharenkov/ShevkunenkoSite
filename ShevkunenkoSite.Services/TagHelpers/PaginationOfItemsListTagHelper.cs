namespace ShevkunenkoSite.Services.TagHelpers;

[HtmlTargetElement("div", Attributes = "pagination")]
public class PaginationOfItemsListTagHelper(IUrlHelperFactory helperFactory) : TagHelper
{
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext? ViewContext { get; set; }

    [HtmlAttributeName("pagination")]
    public PagingInfoViewModel? Paging { get; set; }

    public string? AlbumCaption { get; set; }

    private const string PhotoAlbumAction = "PhotoAlbum";
    private const string FilmPhotoAlbumAction = "FilmPhotoAlbum";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (ViewContext is null || Paging is null || Paging.TotalPages <= 1)
        {
            output.SuppressOutput();

            return;
        }

        string actionName = ViewContext.ActionDescriptor?.RouteValues["action"]
                    ?? ViewContext.RouteData.Values["action"]?.ToString()
                    ?? string.Empty;

        if (string.IsNullOrEmpty(actionName))
        {
            output.SuppressOutput();

            return;
        }

        IUrlHelper urlHelper = helperFactory.GetUrlHelper(ViewContext);

        output.Attributes.Clear();                 // убираем pagination="..." из финального HTML

        output.Attributes.SetAttribute("class",
            "d-flex flex-wrap justify-content-center my-3 p-2 mx-auto");

        for (int i = 1; i <= Paging.TotalPages; i++)
        {
            TagBuilder newRef = new("a");

            if (actionName == PhotoAlbumAction)
            {
                newRef.Attributes["href"] = urlHelper.Action(actionName,
                new
                {
                    albumCaption = AlbumCaption,
                    pageNumber = i,
                    imageId = string.Empty
                });
            }
            else if (actionName == FilmPhotoAlbumAction)
            {
                newRef.Attributes["href"] = urlHelper.Action(actionName,
                new
                {
                    filmCaption = AlbumCaption,
                    pageNumber = i,
                    imageId = string.Empty
                });
            }
            else
            {
                newRef.Attributes["href"] = urlHelper.Action(actionName,
                    new
                    {
                        searchString = Paging.SearchString ?? string.Empty,
                        pageNumber = i,
                        pageCard = Paging.PageCard
                    });
            }

            newRef.Attributes["title"] = $"страница {i}";

            newRef.AddCssClass($"btn border border-1 border-secondary rounded ten width60px mx-2 my-1 py-0 " +
                               $"{(i == Paging.CurrentPage ? "btn-danger" : "btn-outline-dark")}");

            newRef.InnerHtml.Append(i.ToString());

            output.Content.AppendHtml(newRef);
        }
    }
}