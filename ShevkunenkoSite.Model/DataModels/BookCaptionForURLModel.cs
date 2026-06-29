namespace ShevkunenkoSite.Models.DataModels;

public class BookCaptionForURLModel
{
    #region Идентификатор в базе данных

    [Key]
    [Display(Name = "ID заголовка для URL :")]
    [Column("BookCaptionForURLId")]
    public Guid BookCaptionForURLModelId { get; set; } = Guid.Empty;

    #endregion

    #region Заголовок книги для URL

    [DataType(DataType.Text)]
    [Display(Name = "Заголовок для URL:")]
    public string BookCaptionForURL { get; set; } = string.Empty;

    #endregion

    #region Связь с таблицей BooksAndArticlesModel (one-to-one)

    public Guid BooksAndArticlesModelId { get; set; }

    #endregion
}