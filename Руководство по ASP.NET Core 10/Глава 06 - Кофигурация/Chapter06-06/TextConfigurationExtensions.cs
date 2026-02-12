namespace Chapter06_06;

public static class TextConfigurationExtensions
{
    public static IConfigurationBuilder AddTextFile(
        this IConfigurationBuilder builder, string path)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Путь к файлу не указан");
        }

        var source = new TextConfigurationSource(path);

        builder.Add(source);

        return builder;
    }
}
