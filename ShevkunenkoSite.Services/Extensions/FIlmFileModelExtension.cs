namespace ShevkunenkoSite.Services.Extensions;

public static class FilmFileModelExtension
{
    public static IEnumerable<FilmFileModel> FilmSearch(this IEnumerable<FilmFileModel> filmFileModel, string? filmSearchString)
    {
        foreach (var foundFilm in filmFileModel)
        {
            if (
                foundFilm.FilmFileName.Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | foundFilm.FilmCaption.Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmCaptionOriginal ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | foundFilm.FilmDescriptionForSchemaOrg.Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmNote ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.SearchFilterForFilm ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | foundFilm.FilmGenre.Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | foundFilm.FilmРroductionCompany.Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | foundFilm.FilmDirector1.Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmDirector2 ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmMusicBy ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmActor01 ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmActor02 ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmActor03 ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmActor04 ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmActor05 ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmActor06 ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmActor07 ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmActor08 ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmActor09 ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.FilmActor10 ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                | (foundFilm.SeriesSearchFilter ?? string.Empty).Contains((filmSearchString ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                )
            {
                yield return foundFilm;
            }
        }
    }
}