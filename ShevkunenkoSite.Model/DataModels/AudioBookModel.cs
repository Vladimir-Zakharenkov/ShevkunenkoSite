namespace ShevkunenkoSite.Models.DataModels;

public class AudioBookModel
{
    #region Guid аудиокниги

    [Key]
    [Display(Name = "Идентификатор аудиокниги :")]
    [Column("AudioBookId")]
    public Guid AudioBookModelId { get; set; } = Guid.Empty;

    #endregion

    #region Название аудиокниги

    [Required(ErrorMessage = "Введите название")]
    [DataType(DataType.Text)]
    [Display(Name = "Заголовок :")]
    public string CaptionOfAudioBook { get; set; } = string.Empty;

    #endregion

    #region Описание аудиокниги

    [Required(ErrorMessage = "Добавте описание")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Описание :")]
    public string AudioBookDescription { get; set; } = string.Empty;

    #endregion

    #region Исполнитель текста

    [Required(ErrorMessage = "Введите имя исполнителя")]
    [DataType(DataType.Text)]
    [Display(Name = "Исполнитель :")]
    public string ActorOfAudioBook { get; set; } = string.Empty;

    #endregion

    #region Количество файлов (глав)

    [Range(1, 1000, ErrorMessage = "Недопустимое значение")]
    [Display(Name = "Количество файлов :")]
    public int NumberOfFiles { get; set; } = 1;

    #endregion

    #region Книга связанная с аудиокнигой

    [Display(Name = "Книга :")]
    public Guid? BookForAudioBookId { get; set; }
    public BooksAndArticlesModel? BookForAudioBook { get; set; }

    #endregion

    // TODO: добавить свойство ссылки(Uri) на аудиокнигу
}