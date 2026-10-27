// Ignore Spelling: Infostyle

namespace ShevkunenkoSite.Models.ViewModels;

public class DetailsImageIconViewModel
{
    #region Основной CSS для кнопок Info

    public const string MainInfostyle = "btn btn-outline-primary p-1 block-shadow";

    #endregion

    #region Инициализация ImageItem
    public ImageFileModel ImageItem { get; set; } = new();

    #endregion

    #region Тип иконки

    public string? IconType { get; set; }

    #endregion

    #region Наличие файла в каталоге

    public bool FileExists { get; set; }

    #endregion

    #region Имя файла

    public string FileName { get; set; } = string.Empty;

    #endregion

    #region Текущий CSS

    public string? CurrentCss { get; set; }

    #endregion

    #region Infostyle для картинок PNG

    public string HdButtonInfostyle { get; set; } = MainInfostyle;

    public string ImageButtonInfostyle { get; set; } = MainInfostyle;

    public string Icon300ButtonInfostyle { get; set; } = MainInfostyle;

    public string Icon200ButtonInfostyle { get; set; } = MainInfostyle;

    public string Icon100ButtonInfostyle { get; set; } = MainInfostyle;

    #endregion

    #region Infostyle для картинок WEBP

    public string WebImageButtonInfostyle { get; set; } = MainInfostyle;

    public string WebhdButtonInfostyle { get; set; } = MainInfostyle;

    public string WebIcon300ButtonInfostyle { get; set; } = MainInfostyle;

    public string WebIcon200ButtonInfostyle { get; set; } = MainInfostyle;

    public string WebIcon100ButtonInfostyle { get; set; } = MainInfostyle;

    #endregion

    #region Infostyle для картинок WEBP AI

    public string WebhdAIButtonInfostyle { get; set; } = MainInfostyle;

    public string WebImageAIButtonInfostyle { get; set; } = MainInfostyle;

    public string WebIcon300AIButtonInfostyle { get; set; } = MainInfostyle;

    public string WebIcon200AIButtonInfostyle { get; set; } = MainInfostyle;

    public string WebIcon100AIButtonInfostyle { get; set; } = MainInfostyle;

    #endregion
}