namespace ShevkunenkoSite.Models.DataModels;

public class BackgroundFileModel
{
    #region Идентификатор фона

    [Display(Name = "BackgroundFileId:")]
    [Column("BackgroundFileId")]
    public Guid BackgroundFileModelId { get; set; }

    #endregion

    #region Фон страницы в формате PNG

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Фон страницы слева:")]
    [DataType(DataType.Text)]
    public string LeftBackground { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Фон страницы справа:")]
    [DataType(DataType.Text)]
    public string RightBackground { get; set; } = string.Empty;

    #endregion

    #region Фои страницы в формате WEBP

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Фон страницы слева (WebP):")]
    [DataType(DataType.Text)]
    public string WebLeftBackground { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Фон страницы справа (WebP):")]
    [DataType(DataType.Text)]
    public string WebRightBackground { get; set; } = string.Empty;

    #endregion
}