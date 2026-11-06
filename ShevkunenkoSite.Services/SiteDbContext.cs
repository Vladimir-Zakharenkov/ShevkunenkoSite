namespace ShevkunenkoSite.Services;

public class SiteDbContext(DbContextOptions<SiteDbContext> options) : DbContext(options)
{
    #region Таблицы в базе данных по моделям

    public DbSet<PageInfoModel> PageInfo => Set<PageInfoModel>();

    public DbSet<BackgroundFileModel> BackgroundFile => Set<BackgroundFileModel>();

    public DbSet<ImageFileModel> ImageFile => Set<ImageFileModel>();

    public DbSet<IconModel> Icons => Set<IconModel>();

    public DbSet<MovieFileModel> MovieFile => Set<MovieFileModel>();

    public DbSet<AccessModel> Access => Set<AccessModel>();

    public DbSet<TopicMovieModel> TopicMovie => Set<TopicMovieModel>();

    public DbSet<TextInfoModel> TextFile => Set<TextInfoModel>();

    public DbSet<BooksAndArticlesModel> BooksAndArticles => Set<BooksAndArticlesModel>();

    public DbSet<BookCaptionForURLModel> BookCaptionForURL => Set<BookCaptionForURLModel>();

    public DbSet<AudioBookModel> AudioBookModel => Set<AudioBookModel>();

    public DbSet<AudioInfoModel> AudioInfoModel => Set<AudioInfoModel>();

    public DbSet<FilmFileModel> Films => Set<FilmFileModel>();

    public DbSet<IconTypeModel> IconTypes => Set<IconTypeModel>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Вычисляемые поля базы данных

        modelBuilder.Entity<PageInfoModel>()
            .Property(p => p.PageFullPath)
            .HasComputedColumnSql("[PageArea] + [Controller] + [PageLoc]");

        modelBuilder.Entity<PageInfoModel>()
            .Property(p => p.PageFullPathWithData)
            .HasComputedColumnSql("[PageArea] + [Controller] + [PageLoc] + [RoutData]");

        modelBuilder.Entity<PageInfoModel>()
            .Property(p => p.PagePathNickNameWithData)
            .HasComputedColumnSql("[PagePathNickName] + [RoutData]");

        #endregion

        #region Отменить каскадное удаление во всех индексах

        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        #endregion

        modelBuilder.Entity<MovieFileModel>()
            .HasOne(o => o.PageForMovieSeries)
            .WithMany()
            .HasForeignKey(o => o.PageForMovieSeriesId);

        #region Индексы для ImageFileModel (поиск по имени файла)

        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.WebImageHDFileName));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.WebImageFileName));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.WebIconFileName));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.WebIcon200FileName));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.WebIcon100FileName));

        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.WebImageHDFileNameAI));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.WebImageFileNameAI));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.WebIconFileNameAI));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.WebIcon200FileNameAI));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.WebIcon100FileNameAI));

        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.ImageHDFileName));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.ImageFileName));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.IconFileName));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.Icon200FileName));
        ConfigureImageFileIndex(modelBuilder, nameof(ImageFileModel.Icon100FileName));

        #endregion
    }

    /// <summary>
    /// Создаёт уникальный индекс по колонке имени файла с фильтром «не NULL и не пусто».
    /// Защищает от дублей и не индексирует «пустые» значения (не сгенерированные файлы).
    /// </summary>
    private static void ConfigureImageFileIndex(ModelBuilder mb, string propertyName)
    {
        mb.Entity<ImageFileModel>()
            .HasIndex(propertyName)
            .IsUnique()
            .HasFilter($"[{propertyName}] IS NOT NULL AND [{propertyName}] <> ''");
    }
}