// Ignore Spelling: Msapplication Asus Teatr Kinopoisk Imdb

namespace ShevkunenkoSite.Models;

public class DataConfig
{
    #region Картинки

    public static Guid NoImage { get; set; }

    public static Guid ImageForFilm { get; set; }

    public static Guid YoutubeImage { get; set; }

    public static Guid VkVideoImage { get; set; }

    public static Guid MailRuVideoImage { get; set; }

    public static Guid OkVideoImage { get; set; }

    public static Guid AsusVideoImage { get; set; }

    public static Guid YandexDiskImage { get; set; }

    public static Guid DownloadFilmImage { get; set; }

    public static Guid KinoTeatrImage { get; set; }

    public static Guid KinopoiskImage { get; set; }

    public static Guid ImdbImage { get; set; }

    public static Guid WordImage { get; set; }

    public static Guid TxtImage { get; set; }

    public static Guid Mp3Image { get; set; }

    public static Guid PdfImage { get; set; }

    public static Guid AudioImage { get; set; }

    public static Guid PrintImage { get; set; }

    #endregion

    #region Папки

    public static string TempPath { get; set; } = null!;

    public static string IconsFolder { get; set; } = null!;

    public static string IconFoldersPath { get; set; } = null!;

    public static string MovieFoldersPath { get; set; } = null!;

    public static string AudioFoldersPath { get; set; } = null!;

    public static string TextsFolderPath { get; set; } = null!;

    public static string ArchiveTextsFolderPath { get; set; } = null!;

    public static string ArchiveFilmsFolderPath { get; set; } = null!;

    #endregion

    #region Пагинация

    public static int NumberOfItemsPerPage { get; set; }

    public static int NumberOfPicturesAround { get; set; }

    #endregion

    #region Инициализация из DataConfigOptions

    /// <summary>
    /// Заполняет static-свойства DataConfig значениями из DataConfigOptions.
    /// Вызывается однократно при старте приложения в Program.cs.
    /// </summary>
    public static void Init(DataConfigOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Папки
        TempPath = options.TempPath;
        IconsFolder = options.IconsFolder;
        IconFoldersPath = options.IconFoldersPath;
        NoImage = options.NoImage;
        MovieFoldersPath = options.MovieFoldersPath;
        AudioFoldersPath = options.AudioFoldersPath;
        TextsFolderPath = options.TextsFolderPath;
        ArchiveTextsFolderPath = options.ArchiveTextsFolderPath;
        ArchiveFilmsFolderPath = options.ArchiveFilmsFolderPath;

        // Guid'ы картинок
        ImageForFilm = options.ImageForFilm;
        YoutubeImage = options.YoutubeImage;
        VkVideoImage = options.VkVideoImage;
        MailRuVideoImage = options.MailRuVideoImage;
        OkVideoImage = options.OkVideoImage;
        AsusVideoImage = options.AsusVideoImage;
        YandexDiskImage = options.YandexDiskImage;
        DownloadFilmImage = options.DownloadFilmImage;
        KinoTeatrImage = options.KinoTeatrImage;
        KinopoiskImage = options.KinopoiskImage;
        ImdbImage = options.ImdbImage;
        WordImage = options.WordImage;
        TxtImage = options.TxtImage;
        Mp3Image = options.Mp3Image;
        PdfImage = options.PdfImage;
        AudioImage = options.AudioImage;
        PrintImage = options.PrintImage;

        // Пагинация
        NumberOfItemsPerPage = options.NumberOfItemsPerPage;
        NumberOfPicturesAround = options.NumberOfPicturesAround;
    }

    #endregion
}