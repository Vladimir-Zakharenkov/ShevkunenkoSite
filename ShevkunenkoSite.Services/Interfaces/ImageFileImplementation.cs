using ShevkunenkoSite.Models;

namespace ShevkunenkoSite.Services.Interfaces;

public class ImageFileImplementation(SiteDbContext siteContext) : IImageFileRepository
{
    #region Все файлы картинок в БД

    public IQueryable<ImageFileModel> ImageFiles => siteContext.ImageFile;

    #endregion

    #region Сохранить данные картинки в БД

    public async Task SaveChangesInImageAsync() => await siteContext.SaveChangesAsync();

    #endregion

    #region Добавить новую картинку в БД

    public async Task AddNewImageAsync(ImageFileModel image)
    {
        await siteContext.ImageFile.AddAsync(image);
        await SaveChangesInImageAsync();
    }

    #endregion

    #region  Удалить картинку из БД

    public async Task DeleteImageAsync(Guid imageId)
    {
        var image = await siteContext.ImageFile
                .FirstOrDefaultAsync(i => i.ImageFileModelId == imageId);

        if (image is not null)
        {
            siteContext.ImageFile.Remove(image); await siteContext.SaveChangesAsync();
        }
    }

    #endregion

    #region Получить картинку по GUID или имени файла

    /// <summary>
    /// Получить картинку по GUID или имени файла.
    /// Если ничего не найдено — возвращает картинку-заглушку DataConfig.NoImage.
    /// </summary>
    /// <param name="imageObject">GUID картинки или имя файла (в любой из колонок)</param>
    /// <returns>ImageFileModel</returns>
    public async Task<ImageFileModel> GetImageByGuidOrFileNameAsync(string imageObject)
    {
        if (string.IsNullOrWhiteSpace(imageObject))
        {
            return await siteContext.ImageFile
                .FirstAsync(img => img.ImageFileModelId == DataConfig.NoImage);
        }

        // Поиск картинки по GUID
        if (Guid.TryParse(imageObject, out Guid imageIdGuid))
        {
            var imageById = await siteContext.ImageFile
                .FirstOrDefaultAsync(img => img.ImageFileModelId == imageIdGuid);

            if (imageById is not null)
            {
                return imageById;
            }

            return await siteContext.ImageFile
                .FirstAsync(img => img.ImageFileModelId == DataConfig.NoImage);
        }

        // Поиск картинки по имени файла — одним запросом по всем колонкам
        var imageByFileName = await siteContext.ImageFile
            .FirstOrDefaultAsync(img =>
                img.WebImageHDFileName == imageObject ||
                img.WebImageFileName == imageObject ||
                img.WebIconFileName == imageObject ||
                img.WebIcon200FileName == imageObject ||
                img.WebIcon100FileName == imageObject ||

                img.WebImageHDFileNameAI == imageObject ||
                img.WebImageFileNameAI == imageObject ||
                img.WebIconFileNameAI == imageObject ||
                img.WebIcon200FileNameAI == imageObject ||
                img.WebIcon100FileNameAI == imageObject ||

                img.ImageHDFileName == imageObject ||
                img.ImageFileName == imageObject ||
                img.IconFileName == imageObject ||
                img.Icon200FileName == imageObject ||
                img.Icon100FileName == imageObject);

        return imageByFileName ?? await siteContext.ImageFile
            .FirstAsync(img => img.ImageFileModelId == DataConfig.NoImage);
    }

    #endregion

    #region Получить Guid картинки по имени файла

    /// <summary>
    /// Получить Guid картинки по имени файла.
    /// Если ничего не найдено — возвращает Guid.Empty.
    /// </summary>
    /// <param name="imageFilename">Имя файла (в любой из колонок)</param>
    /// <returns>Guid картинки или Guid.Empty</returns>
    public async Task<Guid> GetImageGuidByFileNameAsync(string? imageFilename)
    {
        if (string.IsNullOrWhiteSpace(imageFilename))
        {
            return Guid.Empty;
        }

        var image = await siteContext.ImageFile
            .FirstOrDefaultAsync(img =>
                img.WebImageHDFileName == imageFilename ||
                img.WebImageFileName == imageFilename ||
                img.WebIconFileName == imageFilename ||
                img.WebIcon200FileName == imageFilename ||
                img.WebIcon100FileName == imageFilename ||

                img.WebImageHDFileNameAI == imageFilename ||
                img.WebImageFileNameAI == imageFilename ||
                img.WebIconFileNameAI == imageFilename ||
                img.WebIcon200FileNameAI == imageFilename ||
                img.WebIcon100FileNameAI == imageFilename ||

                img.ImageHDFileName == imageFilename ||
                img.ImageFileName == imageFilename ||
                img.IconFileName == imageFilename ||
                img.Icon200FileName == imageFilename ||
                img.Icon100FileName == imageFilename);

        return image?.ImageFileModelId ?? Guid.Empty;
    }

    #endregion
}