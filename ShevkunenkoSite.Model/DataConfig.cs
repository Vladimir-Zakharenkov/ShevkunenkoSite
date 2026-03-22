// Ignore Spelling: Msapplication

namespace ShevkunenkoSite.Models;

public class DataConfig
{
    #region Папка временного хранения

    public static string TempPath { get; set; } = null!;

    #endregion

    #region Папка с иконками

    public static string IconsFolder { get; set; } = null!;

    #endregion

    #region Папка с иконками на диске

    public static string IconFoldersPath { get; set; } = null!;

    #endregion

    #region Картинка NoImage

    public static string NoImage { get; set; } = null!;

    #endregion

    #region Папка для фильмов

    public static string MovieFoldersPath { get; set; } = null!;

    #endregion

    #region Папка для аудиофайлов

    public static string AudioFoldersPath { get; set; } = null!;

    #endregion

    #region Папка для текстов

    public static string TextsFolderPath { get; set; } = null!;

    #endregion

    #region Папка-архив для текста

    public static string ArchiveTextsFolderPath { get; set; } = null!;

    #endregion

    #region Картинка YouTube

    public static Guid YoutubeImage {  get; set; }

    #endregion

    #region Картинка VkVideo

    public static Guid VkVideoImage { get; set; }

    #endregion

    #region Картинка MailRuVideo

    public static Guid MailRuVideoImage { get; set; }

    #endregion

    #region Картинка OkVideo

    public static Guid OkVideoImage { get; set; }

    #endregion

    #region Картинка AsusVideo

    public static Guid AsusVideoImage { get; set; }

    #endregion

    #region Картинка YandexDisk

    public static Guid YandexDiskImage { get; set; }

    #endregion

    #region Картинка Kino-Teatr.ru

    public static Guid KinoTeatrImage { get; set; }

    #endregion

    #region Картинка Kinopoisk.ru

    public static Guid KinopoiskImage { get; set; }

    #endregion

    #region Картинка IMDB.com

    public static Guid ImdbImage { get; set; }

    #endregion

    #region Число ссылок на странице для постраничного представления

    public static int NumberOfItemsPerPage { get; set; }

    #endregion

    #region Число картинок  слева (справа) вокруг основного содержания

    public static int NumberOfPicturesAround { get; set; }

    #endregion
}