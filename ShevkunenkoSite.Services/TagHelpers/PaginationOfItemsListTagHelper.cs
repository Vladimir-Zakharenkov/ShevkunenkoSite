
namespace ShevkunenkoSite.Services.TagHelpers;

[HtmlTargetElement("div", Attributes = "pagination")]
public class PaginationOfItemsListTagHelper(
    IUrlHelperFactory helperFactory) : TagHelper
{
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext? ViewContext { get; set; }

    [HtmlAttributeName("pagination")]
    public PagingInfoViewModel? Paging { get; set; }

    public string? AlbumCaption { get; set; }

    [HtmlAttributeName("show-ai")]
    public bool? ShowAI { get; set; }

    private const string PhotoAlbumAction = "PhotoAlbum";
    private const string FilmPhotoAlbumAction = "FilmPhotoAlbum";

    private const int VisiblePagesAroundCurrent = 2;

    public override void Process(
        TagHelperContext context,
        TagHelperOutput output)
    {
        if (ViewContext is null
            || Paging is null
            || Paging.TotalPages <= 1)
        {
            output.SuppressOutput();
            return;
        }

        string actionName =
            ViewContext.RouteData.Values["action"]?.ToString()
            ?? ViewContext.ActionDescriptor.RouteValues["action"]
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(actionName))
        {
            output.SuppressOutput();
            return;
        }

        IUrlHelper urlHelper =
            helperFactory.GetUrlHelper(ViewContext);

        int totalPages = Paging.TotalPages;

        int currentPage = Math.Clamp(
            Paging.CurrentPage, 1, totalPages);

        int startPage = Math.Max(
            1, currentPage - VisiblePagesAroundCurrent);

        int endPage = Math.Min(
            totalPages,
            currentPage + VisiblePagesAroundCurrent);

        output.TagName = "nav";
        output.Attributes.Clear();

        output.Attributes.SetAttribute(
            "class",
            "d-flex flex-wrap justify-content-center " +
            "align-items-center p-2 mx-auto");

        output.Attributes.SetAttribute(
            "aria-label", "Навигация по страницам");

        // Формирование адреса с сохранением маршрутов
        string? GetPageUrl(int pageNumber)
        {
            if (string.Equals(
                actionName,
                PhotoAlbumAction,
                StringComparison.OrdinalIgnoreCase))
            {
                return urlHelper.Action(actionName, new
                {
                    albumCaption = AlbumCaption,
                    pageNumber,
                    imageId = string.Empty
                });
            }

            if (string.Equals(
                actionName,
                FilmPhotoAlbumAction,
                StringComparison.OrdinalIgnoreCase))
            {
                return urlHelper.Action(actionName, new
                {
                    filmCaption = AlbumCaption,
                    pageNumber,
                    imageId = string.Empty,
                    showAI = ShowAI ?? true
                });
            }

            return urlHelper.Action(actionName, new
            {
                searchString = Paging.SearchString ?? string.Empty,
                pageNumber,
                pageCard = Paging.PageCard
            });
        }

        // Создание кнопки страницы
        void AddPageLink(
            int pageNumber,
            string text,
            string? title = null)
        {
            TagBuilder link = new("a");

            link.Attributes["href"] =
                GetPageUrl(pageNumber) ?? "#";

            link.Attributes["title"] =
                title ?? $"Страница {pageNumber}";

            link.AddCssClass(
                "btn border border-1 border-secondary " +
                "rounded ten width60px mx-1 my-1 py-0");

            if (pageNumber == currentPage)
            {
                link.AddCssClass("btn-danger");

                link.Attributes["aria-current"] = "page";
            }
            else
            {
                link.AddCssClass("btn-outline-dark");
            }

            link.InnerHtml.Append(text);

            output.Content.AppendHtml(link);
        }

        // Многоточие между диапазонами
        void AddEllipsis()
        {
            TagBuilder dots = new("span");

            dots.AddCssClass(
                "mx-2 my-1 text-secondary");

            dots.Attributes["aria-hidden"] = "true";

            dots.InnerHtml.Append("…");

            output.Content.AppendHtml(dots);
        }

        // Кнопка назад
        if (currentPage > 1)
        {
            AddPageLink(
                currentPage - 1,
                "«",
                "Предыдущая страница");
        }

        // Первая страница
        if (startPage > 1)
        {
            AddPageLink(1, "1");

            if (startPage > 2)
            {
                AddEllipsis();
            }
        }

        // Страницы вокруг текущей
        for (int i = startPage; i <= endPage; i++)
        {
            AddPageLink(i, i.ToString());
        }

        // Последняя страница
        if (endPage < totalPages)
        {
            if (endPage < totalPages - 1)
            {
                AddEllipsis();
            }

            AddPageLink(
                totalPages,
                totalPages.ToString());
        }

        // Кнопка вперёд
        if (currentPage < totalPages)
        {
            AddPageLink(
                currentPage + 1,
                "»",
                "Следующая страница");
        }
    }
}
