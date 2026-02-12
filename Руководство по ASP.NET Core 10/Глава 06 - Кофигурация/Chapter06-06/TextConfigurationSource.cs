namespace Chapter06_06;

public class TextConfigurationSource(string filename) : IConfigurationSource
{
    public string FilePath { get; } = filename;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        // получаем полный путь для файла
        string? filePath = builder.GetFileProvider().GetFileInfo(FilePath).PhysicalPath;

        return new TextConfigurationProvider(filePath!);
    }
}
