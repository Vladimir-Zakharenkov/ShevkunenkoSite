namespace ShevkunenkoSite.Services.TagHelpers;

[HtmlTargetElement("div", Attributes = "pagination")]
public class PaginationOfItemsListTagHelper(IUrlHelperFactory helperFactory) : TagHelper
{
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext? ViewContext { get; set; }

    public PagingInfoViewModel? Pagination { get; set; }

    public required string AlbumCaption { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (ViewContext != null && Pagination != null)
        {
            IUrlHelper urlHelper = helperFactory.GetUrlHelper(ViewContext);

            TagBuilder divForPagination = new("div");
            divForPagination.AddCssClass("flex-wrap justify-content-center my-3 p-2 maxwidth90percent mx-auto");

            for (int i = 1; i <= Pagination.TotalPages; i++)
            {
                TagBuilder newRef = new("a");

                if (ViewContext.RouteData.Values.TryGetValue("action", out object? actionValue))
                {
                    if (actionValue != null)
                    {
                        if (actionValue.ToString() == "PhotoAlbum")
                        {
                            newRef.Attributes["href"] = urlHelper.Action(actionValue.ToString(),
                            new
                            {
                                albumCaption = AlbumCaption,
                                pageNumber = i,
                                imageId = string.Empty
                            });

                            newRef.Attributes["title"] = "страница " + i;
                        }
                        else if (actionValue.ToString() == "FilmPhotoAlbum")
                        {
                            newRef.Attributes["href"] = urlHelper.Action(actionValue.ToString(),
                            new
                            {
                                filmCaption = AlbumCaption,
                                pageNumber = i,
                                imageId = string.Empty
                            });

                            newRef.Attributes["title"] = "страница " + i;
                        }
                        else
                        {
                            newRef.Attributes["href"] = urlHelper.Action(actionValue.ToString(),
                                new
                                {
                                    searchString = ViewContext.HttpContext.Request.Query["searchString"].ToString() ?? string.Empty,
                                    pageNumber = i,
                                    pageCard = Pagination.PageCard
                                });
                        }

                        newRef.Attributes["title"] = "страница " + i;
                    }
                }

                newRef.AddCssClass("btn border border-1 border-secondary rounded ten width60px mx-2 my-1 py-0");
                newRef.AddCssClass(i == Pagination.CurrentPage ? "btn-danger" : "btn-outline-dark");

                newRef.InnerHtml.Append(i.ToString());

                divForPagination.InnerHtml.AppendHtml(newRef);
            }

            output.Content.AppendHtml(divForPagination);
        }
    }
}