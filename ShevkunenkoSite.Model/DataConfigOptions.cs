// Ignore Spelling: Msapplication Asus Teatr Kinopoisk Imdb

namespace ShevkunenkoSite.Models;

/// <summary>
/// Instance-версия DataConfig для привязки из конфигурации (configuration.Bind).
/// Заполняется из секции "ProjectData" в Program.cs,
/// затем передаётся в DataConfig.Init(options) для установки static-свойств.
/// </summary>
public class DataConfigOptions
{
    #region Папки

    public string TempPath { get; set; } = null!;

    public string IconsFolder { get; set; } = null!;

    public string IconFoldersPath { get; set; } = null!;

    public string MovieFoldersPath { get; set; } = null!;

    public string AudioFoldersPath { get; set; } = null!;

    public string TextsFolderPath { get; set; } = null!;

    public string ArchiveTextsFolderPath { get; set; } = null!;

    public string ArchiveFilmsFolderPath { get; set; } = null!;

    #endregion

    #region Картинки

    public Guid NoImage { get; set; }

    public Guid ImageForFilm { get; set; }

    public Guid YoutubeImage { get; set; }

    public Guid VkVideoImage { get; set; }

    public Guid MailRuVideoImage { get; set; }

    public Guid OkVideoImage { get; set; }

    public Guid AsusVideoImage { get; set; }

    public Guid YandexDiskImage { get; set; }

    public Guid DownloadFilmImage { get; set; }

    public Guid KinoTeatrImage { get; set; }

    public Guid KinopoiskImage { get; set; }

    public Guid ImdbImage { get; set; }

    public Guid WordImage { get; set; }

    public Guid TxtImage { get; set; }

    public Guid Mp3Image { get; set; }

    public Guid PdfImage { get; set; }

    public Guid AudioImage { get; set; }

    public Guid PrintImage { get; set; }

    #endregion

    #region Пагинация

    public int NumberOfItemsPerPage { get; set; }

    public int NumberOfPicturesAround { get; set; }

    #endregion
}