using MetadataExtractor;

var directories = ImageMetadataReader.ReadMetadata(@"F:\TEMP\60-let.mp4");

foreach (var directory in directories)
{
    foreach (var tag in directory.Tags)
    {
        Console.WriteLine($"{directory.Name} - {tag.Name} = {tag.Description}");
    }
}