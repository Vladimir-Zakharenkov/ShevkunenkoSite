using ShevkunenkoSite.Models.DataModels;

namespace ShevkunenkoSite.Views.Shared.Components.Code;

public class ViewImageViewModel
{
    public ImageFileModel ImageItem { get; set; } = default!;

    /// <summary>
    /// Тип иконки: webhd, webimage, webicon300, webicon200, webicon100,
    /// hd, image, icon300, icon200, icon100.
    /// </summary>
    public string IconType { get; set; } = "webimage";

    public string CssClass { get; set; } = "img-fluid w-100";

    /// <summary>
    /// Есть ли у модели валидный основной файл для отображения.
    /// </summary>
    public bool HasImage => GetImageData().ContentFileName is not null;

    /// <summary>
    /// Данные для отображения картинки: имена файлов и размеры.
    /// Внутри — fallback-логика: если у предпочтительного варианта
    /// нет файла, используется резервный.
    /// </summary>
    public (string? ContentFileName,
            string? ThumbnailFileName,
            uint? Width,
            uint? Height) GetImageData()
    {
        var i = ImageItem;

        return IconType switch
        {
            // ------- webimage (Web → Image) -------
            "webimage" => FirstAvailable(
                (Empty(i.WebImageFileName), Empty(i.WebIconFileName),
                 i.WebImageWidth, i.WebImageHeight),
                (Empty(i.ImageFileName), Empty(i.IconFileName),
                 i.ImageWidth, i.ImageHeight)),

            // ------- image (Image → Web) -------
            "image" => FirstAvailable(
                (Empty(i.ImageFileName), Empty(i.IconFileName),
                 i.ImageWidth, i.ImageHeight),
                (Empty(i.WebImageFileName), Empty(i.WebIconFileName),
                 i.WebImageWidth, i.WebImageHeight)),

            // ------- webicon300 (Web 300 → Icon 300) -------
            "webicon300" => FirstAvailable(
                (Empty(i.WebIconFileName), Empty(i.WebIcon200FileName),
                 i.WebIconWidth, i.WebIconHeight),
                (Empty(i.IconFileName), Empty(i.Icon200FileName),
                 i.IconWidth, i.IconHeight)),

            // ------- icon300 (Icon 300 → Web 300) -------
            "icon300" => FirstAvailable(
                (Empty(i.IconFileName), Empty(i.Icon200FileName),
                 i.IconWidth, i.IconHeight),
                (Empty(i.WebIconFileName), Empty(i.WebIcon200FileName),
                 i.WebIconWidth, i.WebIconHeight)),

            // ------- webicon200 (Web 200 → Icon 200) -------
            "webicon200" => FirstAvailable(
                (Empty(i.WebIcon200FileName), Empty(i.WebIcon100FileName),
                 i.WebIcon200Width, i.WebIcon200Height),
                (Empty(i.Icon200FileName), Empty(i.Icon100FileName),
                 i.Icon200Width, i.Icon200Height)),

            // ------- icon200 (Icon 200 → Web 200) -------
            "icon200" => FirstAvailable(
                (Empty(i.Icon200FileName), Empty(i.Icon100FileName),
                 i.Icon200Width, i.Icon200Height),
                (Empty(i.WebIcon200FileName), Empty(i.WebIcon100FileName),
                 i.WebIcon200Width, i.WebIcon200Height)),

            // ------- webicon100 (Web 100 → Icon 100) -------
            "webicon100" => FirstAvailable(
                (Empty(i.WebIcon100FileName), null,
                 i.WebIcon100Width, i.WebIcon100Height),
                (Empty(i.Icon100FileName), null,
                 i.Icon100Width, i.Icon100Height)),

            // ------- icon100 (Icon 100 → Web 100) -------
            "icon100" => FirstAvailable(
                (Empty(i.Icon100FileName), null,
                 i.Icon100Width, i.Icon100Height),
                (Empty(i.WebIcon100FileName), null,
                 i.WebIcon100Width, i.WebIcon100Height)),

            // ------- webhd (WebHD → ImageHD) -------
            "webhd" => FirstAvailable(
                (Empty(i.WebImageHDFileName), Empty(i.WebImageFileName),
                 i.WebImageHDWidth, i.WebImageHDHeight),
                (Empty(i.ImageHDFileName), Empty(i.ImageFileName),
                 i.ImageHDWidth, i.ImageHDHeight)),

            // ------- hd (ImageHD → WebHD) -------
            "hd" => FirstAvailable(
                (Empty(i.ImageHDFileName), Empty(i.ImageFileName),
                 i.ImageHDWidth, i.ImageHDHeight),
                (Empty(i.WebImageHDFileName), Empty(i.WebImageFileName),
                 i.WebImageHDWidth, i.WebImageHDHeight)),

            _ => (null, null, null, null)
        };
    }

    /// <summary>
    /// Относительный путь к основному изображению (ImagePath + имя файла).
    /// </summary>
    public string? GetContentUrl()
    {
        var name = GetImageData().ContentFileName;
        return string.IsNullOrEmpty(name)
            ? null
            : $"{ImageItem.ImagePath}/{name}";
    }

    /// <summary>
    /// Относительный путь к thumbnail. Если его нет — используется основной файл.
    /// </summary>
    public string? GetThumbnailUrl()
    {
        var data = GetImageData();
        var name = string.IsNullOrEmpty(data.ThumbnailFileName)
            ? data.ContentFileName
            : data.ThumbnailFileName;

        return string.IsNullOrEmpty(name)
            ? null
            : $"{ImageItem.ImagePath}/{name}";
    }

    // -------------------------------------------------------------------
    //  Приватные помощники
    // -------------------------------------------------------------------

    /// <summary>
    /// Возвращает первый кортеж, в котором задан основной файл.
    /// Если основной файл есть, но thumbnail пуст —
    /// thumbnail заменяется на основной файл.
    /// </summary>
    private static (string?, string?, uint?, uint?) FirstAvailable(
        params (string? main, string? thumb, uint? width, uint? height)[] candidates)
    {
        foreach (var c in candidates)
        {
            if (string.IsNullOrEmpty(c.main))
                continue;

            var thumb = string.IsNullOrEmpty(c.thumb) ? c.main : c.thumb;
            return (c.main, thumb, c.width, c.height);
        }

        return (null, null, null, null);
    }

    /// <summary>
    /// Преобразует непустую не-nullable строку в nullable.
    /// Возвращает null, если строка пуста или состоит из пробелов.
    /// </summary>
    private static string? Empty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value;
}