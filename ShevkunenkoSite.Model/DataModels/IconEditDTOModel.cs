namespace ShevkunenkoSite.Models.DataModels;

[NotMapped]
public record class IconEditDTOModel
{
    [DataType(DataType.Text)]
    [Display(Name = "Параметр rel в метатеге link :")]
    public string RelForIcon { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Purpose в manifest :")]
    public string IconPurpose { get; set; } = string.Empty;

    public Guid IconTypeModelId { get; set; }

    public Guid IconModelId { get; set; }
}