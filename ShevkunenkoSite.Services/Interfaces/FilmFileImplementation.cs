namespace ShevkunenkoSite.Services.Interfaces;

public class FilmFileImplementation : IFilmFileRepository
{
    private readonly SiteDbContext _siteContext;
    public FilmFileImplementation(SiteDbContext siteContext) => _siteContext = siteContext;

    public IQueryable<FilmFileModel> FilmFiles => _siteContext.Films.Include(i => i.FilmImage).Include(i => i.FilmPoster);

    public async Task SaveChangesInFilmAsync()
    {
        await _siteContext.SaveChangesAsync();
    }

    public async Task AddNewFilmAsync(FilmFileModel film)
    {
        await _siteContext.Films.AddAsync(film);
        await SaveChangesInFilmAsync();
    }

    public async Task DeleteFilmAsync(Guid filmId)
    {
        if (await _siteContext.Films.Where(i => i.FilmFileModelId == filmId).AnyAsync())
        {
            FilmFileModel filmToDelete = await _siteContext.Films.FirstAsync(i => i.FilmFileModelId == filmId);

            _ = _siteContext.Films.Remove(filmToDelete);
            _ = await _siteContext.SaveChangesAsync();
        }
    }
}