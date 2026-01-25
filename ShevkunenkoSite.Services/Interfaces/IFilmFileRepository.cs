namespace ShevkunenkoSite.Services.Interfaces;

public interface IFilmFileRepository
{
    IQueryable<FilmFileModel> FilmFiles { get; }

    Task AddNewFilmAsync(FilmFileModel film);

    Task SaveChangesInFilmAsync();

    Task DeleteFilmAsync(Guid movieId);
}