namespace ShevkunenkoSite.Models.DataModels;

public class IconTypeModel
{
    [Key]
    [Display(Name = "ID иконки :")]
    [Column("IconTypeId")]
    public Guid IconTypeModelId { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Каталог иконки :")]
    public string PathToIcon { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Описание иконки :")]
    public string IconTypeDescription { get; set; } = string.Empty;
}