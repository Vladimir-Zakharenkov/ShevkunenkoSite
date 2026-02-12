namespace Chapter06_06;

public class TextConfigurationProvider(string path) : ConfigurationProvider
{
    public string FilePath { get; set; } = path;

    public override void Load()
    {
        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        using (StreamReader textReader = new(FilePath))
        {
            string? line;

            while ((line = textReader.ReadLine()) != null)
            {
                string key = line.Trim();
                string? value = textReader.ReadLine() ?? "";
                data.Add(key, value);
            }
        }

        Data= data;
    }
}