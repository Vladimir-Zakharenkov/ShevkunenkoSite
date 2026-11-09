namespace ShevkunenkoSite.Services.Interfaces;

using Microsoft.AspNetCore.Http;

public interface IPageInfoRepository
{
    /// <summary>
    /// Отслеживаемый запрос (для редактирования/сохранения).
    /// Сохранён для обратной совместимости с существующим кодом.
    /// </summary>
    IQueryable<PageInfoModel> PagesInfo { get; }

    /// <summary>
    /// Только для чтения (AsNoTracking). Рекомендуется для отображения страниц.
    /// </summary>
    IQueryable<PageInfoModel> PagesInfoNoTracking { get; }

    /// <summary>
    /// Определяет страницу по HTTP-запросу (путь + query-строка).
    /// Оставлено для обратной совместимости — используется в middleware/контроллерах.
    /// </summary>
    Task<PageInfoModel> GetPageInfoByPathAsync(HttpContext httpContext);

    /// <summary>
    /// Определяет страницу по пути и строке запроса без зависимости от HttpContext.
    /// Возвращает null, если страница не найдена.
    /// </summary>
    Task<PageInfoModel?> FindPageByPathAsync(
        string path,
        string? routData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Найти страницу по идентификатору (без tracking).
    /// </summary>
    Task<PageInfoModel?> GetPageByIdAsync(
        Guid pageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить существование страницы по идентификатору.
    /// </summary>
    Task<bool> PageExistsAsync(
        Guid pageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет страницу и сохраняет изменения.
    /// Сохранено для обратной совместимости.
    /// </summary>
    Task AddNewPageAsync(PageInfoModel page);

    /// <summary>
    /// Добавляет страницу без сохранения (для использования в Unit of Work / транзакциях).
    /// </summary>
    Task AddNewPageWithoutSaveAsync(
        PageInfoModel page,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет изменения. Сохранено для обратной совместимости.
    /// </summary>
    Task SaveChangesInPageAsync();

    /// <summary>
    /// Сохраняет изменения с поддержкой отмены.
    /// </summary>
    Task SaveChangesInPageAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет страницу и сохраняет изменения.
    /// Сохранено для обратной совместимости.
    /// </summary>
    Task DeletePageAsync(Guid pageId);

    /// <summary>
    /// Удаляет страницу без сохранения. Возвращает true, если сущность найдена и помечена на удаление.
    /// </summary>
    Task<bool> DeletePageWithoutSaveAsync(
        Guid pageId,
        CancellationToken cancellationToken = default);
}