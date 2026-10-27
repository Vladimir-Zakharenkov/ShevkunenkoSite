namespace ShevkunenkoSite.Models.ValidationAttributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class NotWhitespaceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string str && string.IsNullOrWhiteSpace(str))
        {
            return new ValidationResult(
                ErrorMessage ?? "Поле не может состоять только из пробелов",
                [validationContext.MemberName ?? string.Empty]);
        }

        return ValidationResult.Success;
    }
}