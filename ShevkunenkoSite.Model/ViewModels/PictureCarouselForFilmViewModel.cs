namespace ShevkunenkoSite.Models.ViewModels;

public class PictureCarouselForFilmViewModel
{
    public ImageFileModel[]? FirstCarousel { get; set; }

    public ImageFileModel[]? SecondCarousel { get; set; }

    public ImageFileModel[]? ThirdCarousel { get; set; }

    public FilmFileModel FilmFile { get; set; } = new();
}