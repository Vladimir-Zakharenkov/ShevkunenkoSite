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
        if (await siteContext.ImageFile.Where(i => i.ImageFileModelId == imageId).AnyAsync())
        {
            ImageFileModel imageToDelete = await siteContext.ImageFile.FirstAsync(i => i.ImageFileModelId == imageId);

            _ = siteContext.ImageFile.Remove(imageToDelete);
            _ = await siteContext.SaveChangesAsync();
        }
    }

    #endregion

    #region Получить картинку из БД по GUID

    public ImageFileModel GetImageByGuidOrFileNameAsync(string imageObject)
    {
        // Поиск картинки по GUID
        if (Guid.TryParse(imageObject, out Guid imageIdGuid) & siteContext.ImageFile.Where(img => img.ImageFileModelId == imageIdGuid).Any())
        {
            return siteContext.ImageFile.First(img => img.ImageFileModelId == imageIdGuid);
        }
        // Поиск картинки по названию файла
        else if (siteContext.ImageFile.Where(img => img.WebImageFileName == imageObject || img.ImageFileName == imageObject).Any())
        {
            return siteContext.ImageFile.First(img => img.WebImageFileName == imageObject || img.ImageFileName == imageObject);
        }
        // Если ничего не найдено, выводим картинку NoImage
        else
        {
            return siteContext.ImageFile.First(img => img.ImageFileModelId == Guid.Parse(DataConfig.NoImage));
        }
    }

    #endregion

    #region Получить Guid картинки по имени файла

    public async Task<Guid> GetImageGuidByFileNameAsync(string? imageFilename)
    {
        if (imageFilename != null)
        {
            if (await siteContext.ImageFile.Where(image => image.WebImageFileName == imageFilename).AnyAsync())
            {
                var imageObject = await siteContext.ImageFile.FirstAsync(image => image.WebImageFileName == imageFilename);

                return imageObject.ImageFileModelId;
            }
            else if (await siteContext.ImageFile.Where(image => image.WebIconFileName == imageFilename).AnyAsync())
            {
                var imageObject = await siteContext.ImageFile.FirstAsync(image => image.WebIconFileName == imageFilename);

                return imageObject.ImageFileModelId;
            }
            else if (await siteContext.ImageFile.Where(image => image.WebIcon200FileName == imageFilename).AnyAsync())
            {
                var imageObject = await siteContext.ImageFile.FirstAsync(image => image.WebIcon200FileName == imageFilename);

                return imageObject.ImageFileModelId;
            }
            else if (await siteContext.ImageFile.Where(image => image.WebIcon100FileName == imageFilename).AnyAsync())
            {
                var imageObject = await siteContext.ImageFile.FirstAsync(image => image.WebIcon100FileName == imageFilename);

                return imageObject.ImageFileModelId;
            }
            else if (await siteContext.ImageFile.Where(image => image.ImageFileName == imageFilename).AnyAsync())
            {
                var imageObject = await siteContext.ImageFile.FirstAsync(image => image.ImageFileName == imageFilename);

                return imageObject.ImageFileModelId;
            }
            else if (await siteContext.ImageFile.Where(image => image.IconFileName == imageFilename).AnyAsync())
            {
                var imageObject = await siteContext.ImageFile.FirstAsync(image => image.IconFileName == imageFilename);

                return imageObject.ImageFileModelId;
            }
            else if (await siteContext.ImageFile.Where(image => image.Icon200FileName == imageFilename).AnyAsync())
            {
                var imageObject = await siteContext.ImageFile.FirstAsync(image => image.Icon200FileName == imageFilename);

                return imageObject.ImageFileModelId;
            }
            else if (await siteContext.ImageFile.Where(image => image.Icon100FileName == imageFilename).AnyAsync())
            {
                var imageObject = await siteContext.ImageFile.FirstAsync(image => image.Icon100FileName == imageFilename);

                return imageObject.ImageFileModelId;
            }
            else
            {
                return Guid.Empty;
            }
        }
        else
        {
            return Guid.Empty;
        }
    }
}

#endregion