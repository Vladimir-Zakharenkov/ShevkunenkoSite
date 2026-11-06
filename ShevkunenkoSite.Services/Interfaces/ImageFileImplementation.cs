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

    #region Получить картинку по GUID или имени файла

    /// <summary>
    /// Получить картинку по GUID или имени файла
    /// </summary>
    /// <param name="imageObject"></param>
    /// <returns></returns>
    public async Task<ImageFileModel> GetImageByGuidOrFileNameAsync(string imageObject)
    {
        // Поиск картинки по GUID
        if (Guid.TryParse(imageObject, out Guid imageIdGuid))
        {
            var imageItem = await siteContext.ImageFile.FirstOrDefaultAsync(img => img.ImageFileModelId == imageIdGuid);

            if (imageItem != null)
            {
                return imageItem;
            }
            else
            {
                return await siteContext.ImageFile.FirstAsync(img => img.ImageFileModelId == DataConfig.NoImage);
            }
        }
        // Поиск картинки по названию файла
        else
        {
            #region WEB

            #region WebImageHD

            var webImageHD = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebImageHDFileName == imageObject);

            if (webImageHD != null)
            {
                return webImageHD;
            }

            #endregion

            #region WebImage

            var webImage = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebImageFileName == imageObject);

            if (webImage != null)
            {
                return webImage;
            }

            #endregion

            #region WebIcon300

            var webIcon300 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIconFileName == imageObject);

            if (webIcon300 != null)
            {
                return webIcon300;
            }

            #endregion

            #region WebIcon200

            var webIcon200 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIcon200FileName == imageObject);

            if (webIcon200 != null)
            {
                return webIcon200;
            }

            #endregion

            #region WebIcon100

            var webIcon100 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIcon100FileName == imageObject);

            if (webIcon100 != null)
            {
                return webIcon100;
            }

            #endregion

            #endregion

            #region WEBAI

            #region WebImageHDAI

            var webImageHDAI = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebImageHDFileNameAI == imageObject);

            if (webImageHDAI != null)
            {
                return webImageHDAI;
            }

            #endregion

            #region WebImageAI

            var webImageAI = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebImageFileNameAI == imageObject);

            if (webImageAI != null)
            {
                return webImageAI;
            }

            #endregion

            #region WebIcon300AI

            var webIcon300AI = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIconFileNameAI == imageObject);

            if (webIcon300AI != null)
            {
                return webIcon300AI;
            }

            #endregion

            #region WebIcon200AI

            var webIcon200AI = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIcon200FileNameAI == imageObject);

            if (webIcon200AI != null)
            {
                return webIcon200AI;
            }

            #endregion

            #region WebIcon100AI

            var webIcon100AI = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIcon100FileNameAI == imageObject);

            if (webIcon100AI != null)
            {
                return webIcon100AI;
            }

            #endregion

            #endregion

            #region PNG

            #region ImageHD

            var imageHD = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.ImageHDFileName == imageObject);

            if (imageHD != null)
            {
                return imageHD;
            }

            #endregion

            #region Image

            var imagePng = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.ImageFileName == imageObject);

            if (imagePng != null)
            {
                return imagePng;
            }

            #endregion

            #region Icon300

            var icon300 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.IconFileName == imageObject);

            if (icon300 != null)
            {
                return icon300;
            }

            #endregion

            #region Icon200

            var icon200 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.Icon200FileName == imageObject);

            if (icon200 != null)
            {
                return icon200;
            }

            #endregion

            #region Icon100

            var icon100 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.Icon100FileName == imageObject);

            if (icon100 != null)
            {
                return icon100;
            }

            #endregion

            #endregion

            return await siteContext.ImageFile.FirstAsync(img => img.ImageFileModelId == DataConfig.NoImage);
        }
    }

    #endregion

    #region Получить Guid картинки по имени файла

    /// <summary>
    /// Получить Guid картинки по имени файла
    /// </summary>
    /// <param name="imageFilename"></param>
    /// <returns></returns>
    public async Task<Guid> GetImageGuidByFileNameAsync(string? imageFilename)
    {
        if (imageFilename != null)
        {
            #region WEB

            #region WebImageHD

            var webImageHD = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebImageHDFileName == imageFilename);

            if (webImageHD != null)
            {
                return webImageHD.ImageFileModelId;
            }

            #endregion

            #region WebImage

            var webImage = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebImageFileName == imageFilename);

            if (webImage != null)
            {
                return webImage.ImageFileModelId;
            }

            #endregion

            #region WebIcon300

            var webIcon300 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIconFileName == imageFilename);

            if (webIcon300 != null)
            {
                return webIcon300.ImageFileModelId;
            }

            #endregion

            #region WebIcon200

            var webIcon200 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIcon200FileName == imageFilename);

            if (webIcon200 != null)
            {
                return webIcon200.ImageFileModelId;
            }

            #endregion

            #region WebIcon100

            var webIcon100 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIcon100FileName == imageFilename);

            if (webIcon100 != null)
            {
                return webIcon100.ImageFileModelId;
            }

            #endregion

            #endregion

            #region WEBAI

            #region WebImageHDAI

            var webImageHDAI = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebImageHDFileNameAI == imageFilename);

            if (webImageHDAI != null)
            {
                return webImageHDAI.ImageFileModelId;
            }

            #endregion

            #region WebImageAI

            var webImageAI = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebImageFileNameAI == imageFilename);

            if (webImageAI != null)
            {
                return webImageAI.ImageFileModelId;
            }

            #endregion

            #region WebIcon300AI

            var webIcon300AI = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIconFileNameAI == imageFilename);

            if (webIcon300AI != null)
            {
                return webIcon300AI.ImageFileModelId;
            }

            #endregion

            #region WebIcon200AI

            var webIcon200AI = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIcon200FileNameAI == imageFilename);

            if (webIcon200AI != null)
            {
                return webIcon200AI.ImageFileModelId;
            }

            #endregion

            #region WebIcon100AI

            var webIcon100AI = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.WebIcon100FileNameAI == imageFilename);

            if (webIcon100AI != null)
            {
                return webIcon100AI.ImageFileModelId;
            }

            #endregion

            #endregion

            #region PNG

            #region ImageHD

            var imageHD = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.ImageHDFileName == imageFilename);

            if (imageHD != null)
            {
                return imageHD.ImageFileModelId;
            }

            #endregion

            #region Image

            var imagePng = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.ImageFileName == imageFilename);

            if (imagePng != null)
            {
                return imagePng.ImageFileModelId;
            }

            #endregion

            #region Icon300

            var icon300 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.IconFileName == imageFilename);

            if (icon300 != null)
            {
                return icon300.ImageFileModelId;
            }

            #endregion

            #region Icon200

            var icon200 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.Icon200FileName == imageFilename);

            if (icon200 != null)
            {
                return icon200.ImageFileModelId;
            }

            #endregion

            #region Icon100

            var icon100 = await siteContext.ImageFile.FirstOrDefaultAsync(image => image.Icon100FileName == imageFilename);

            if (icon100 != null)
            {
                return icon100.ImageFileModelId;
            }

            #endregion

            #endregion

            return Guid.Empty;
        }
        else
        {
            return Guid.Empty;
        }
    }

    #endregion
}