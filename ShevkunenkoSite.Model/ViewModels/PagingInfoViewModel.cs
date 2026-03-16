namespace ShevkunenkoSite.Models.ViewModels;

public class PagingInfoViewModel
{
    #region Полное количество объектов

    public int TotalItems { get; set; } = 1;

    #endregion

    #region Количество объектов на странице

    public int ItemsPerPage { get; set; } = DataConfig.NumberOfItemsPerPage;

    #endregion

    #region Текущая страница

    public int CurrentPage { get; set; } = 1;

    #endregion

    #region Количество страниц (вычисляемое свойство)

    public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);

    #endregion

    #region Строка поиска

    public string? SearchString { get; set; }

    #endregion

    #region Иконка объекта

    public bool PageCard { get; set; }

    #endregion
}