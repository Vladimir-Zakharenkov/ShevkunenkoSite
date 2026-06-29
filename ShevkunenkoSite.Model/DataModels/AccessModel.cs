namespace ShevkunenkoSite.Models.DataModels;

public class AccessModel
{
    [Key]
    public Guid AccessModelId { get; set; }

    [Required(ErrorMessage = "Введите адрес электронной почты")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Адрес электронной почты")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;
}