namespace ShevkunenkoSite.Services.Extensions;

public static class ImageFileModelExtension
{
    /// <summary>
    /// Поиск картинок по строке. Работает на стороне БД (IQueryable).
    /// Если строка поиска пустая/пробельная — возвращает исходный запрос без фильтрации.
    /// </summary>
    public static IQueryable<ImageFileModel> ImageSearch(
                                                this IQueryable<ImageFileModel> source,
                                                string? imageSearchString)
    {
        if (string.IsNullOrWhiteSpace(imageSearchString))
        {
            return source;
        }

        string search = imageSearchString.Trim();

        string pattern = $"%{search}%";

        // SQLITE-ВАРИАНТ: если нужна регистронезависимость по кириллице в SQLite,
        // замените тело Where на сравнение через ToLower():
        //
        // return source.Where(img =>
        //     img.ImageCaption.ToLower().Contains(search.ToLower()) ||
        //     img.ImageDescription.ToLower().Contains(search.ToLower()) ||
        //     img.SearchFilter.ToLower().Contains(search.ToLower()) ||
        //     img.WebImageFileName.ToLower().Contains(search.ToLower()) ||
        //     img.ImageFileName.ToLower().Contains(search.ToLower()));
        //
        // Внимание: ToLower() в SQLite не всегда транслируется корректно для не-ASCII.
        // Для гарантированной регистронезависимости по кириллице может потребоваться
        // клиентская фильтрация (см. вариант B в обсуждении).

        return source.Where(img =>
            EF.Functions.Like(img.ImageCaption, pattern) ||
            EF.Functions.Like(img.ImageDescription, pattern) ||
            EF.Functions.Like(img.SearchFilter, pattern) ||
            EF.Functions.Like(img.WebImageFileName, pattern) ||
            EF.Functions.Like(img.ImageFileName, pattern));
    }
}