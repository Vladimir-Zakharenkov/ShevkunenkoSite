WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region Configuration

ConfigurationManager configuration = builder.Configuration;

IWebHostEnvironment environment = builder.Environment;

configuration.AddJsonFile(
    Path.Combine(environment.ContentRootPath, "DataConfig.json"),
    optional: false,
    reloadOnChange: false);

// Заполняем instance-версию из JSON, затем инициализируем static DataConfig
DataConfigOptions dataConfigOptions = new();
configuration.GetSection("ProjectData").Bind(dataConfigOptions);
DataConfig.Init(dataConfigOptions);

// Проверка, что инициализация прошла
if (string.IsNullOrEmpty(DataConfig.TempPath) ||
    string.IsNullOrEmpty(DataConfig.MovieFoldersPath) ||
    DataConfig.NumberOfItemsPerPage <= 0 ||
    DataConfig.NoImage == Guid.Empty)
{
    throw new InvalidOperationException(
        "DataConfig не инициализирован. Проверьте секцию 'ProjectData' в DataConfig.json " +
        "и вызов DataConfig.Init(dataConfigOptions) в Program.cs.");
}

#endregion

#region Add services to the container

#region Коллекция сервисов

IServiceCollection services = builder.Services;

#endregion

#region Работа с MVC

services.AddControllersWithViews();

#endregion

#region Работа с RazorPages

services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaPage("Admin", "/Index");
    options.Conventions.AuthorizeAreaFolder("Admin", "/DBCRUD");
    options.Conventions.AddPageRoute("/Sitemap", "sitemap.xml");
    options.Conventions.AddPageRoute("/Robots", "robots.txt");
    options.Conventions.AddPageRoute("/Browserconfig", "browserconfig.xml");
});

#endregion

#region Подключить аутентификацию

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Admin/Login");
    });

#endregion

#region Указать пути поиска файлов

services.Configure<RazorViewEngineOptions>(options =>
{
    options.PageViewLocationFormats.Add("/Views/Shared/Layouts/{0}" + RazorViewEngine.ViewExtension);
    options.PageViewLocationFormats.Add("/Views/Shared/Partials/{0}" + RazorViewEngine.ViewExtension);
    options.PageViewLocationFormats.Add("/Views/Shared/Components/{0}" + RazorViewEngine.ViewExtension);

    options.AreaPageViewLocationFormats.Add("/Views/Shared/Layouts/{0}" + RazorViewEngine.ViewExtension);
    options.AreaPageViewLocationFormats.Add("/Pages/Shared/Partials/{0}" + RazorViewEngine.ViewExtension);
    options.AreaPageViewLocationFormats.Add("/Views/Shared/Partials/{0}" + RazorViewEngine.ViewExtension);

    options.ViewLocationFormats.Add("/Views/Shared/Layouts/{0}" + RazorViewEngine.ViewExtension);
    options.ViewLocationFormats.Add("/Pages/Shared/Partials/{0}" + RazorViewEngine.ViewExtension);
    options.ViewLocationFormats.Add("/Views/Shared/Partials/{0}" + RazorViewEngine.ViewExtension);
    options.ViewLocationFormats.Add("/Views/Shared/Components/{0}" + RazorViewEngine.ViewExtension);

    options.AreaViewLocationFormats.Clear();
    options.AreaViewLocationFormats.Add("/Views/Shared/Layouts/{0}" + RazorViewEngine.ViewExtension);
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
    options.AreaViewLocationFormats.Add("/Views/Shared/Components/{0}" + RazorViewEngine.ViewExtension);
    options.AreaViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
    options.AreaViewLocationFormats.Add("/Pages/Shared/{0}" + RazorViewEngine.ViewExtension);
    options.AreaViewLocationFormats.Add("/Pages/Shared/Partials/{0}" + RazorViewEngine.ViewExtension);
    options.AreaViewLocationFormats.Add("/Views/Shared/Partials/{0}" + RazorViewEngine.ViewExtension);
});

#endregion

#region RouteOptions

services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
    options.AppendTrailingSlash = true;
});

#endregion

#region MemoryCache

services.AddDistributedMemoryCache();

#endregion

#region AddSession

services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

#endregion

#region AddWebMarkupMin

services.AddWebMarkupMin(
        options =>
        {
            options.AllowMinificationInDevelopmentEnvironment = false;
            options.AllowCompressionInDevelopmentEnvironment = false;
        })
        .AddHtmlMinification(
            options =>
            {
                options.MinificationSettings.RemoveRedundantAttributes = true;
                options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
                options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
            })
        //.AddXmlMinification()
        .AddHttpCompression();

#endregion

#region Работа с базой данных

services.AddDbContextFactory<SiteDbContext>(options =>
{
    options.UseSqlServer(configuration["ConnectionStrings:ShevkunenkoSitePAPA"] ?? throw new InvalidOperationException("Connection string 'ShevkunenkoSitePAPA' not found."));

    if (environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(true);
    }
});

if (environment.IsDevelopment())
{
    services.AddDatabaseDeveloperPageExceptionFilter();
}

services.AddScoped<IPageInfoRepository, PageInfoImplementation>();
services.AddScoped<IBackgroundFotoRepository, BackGroundFotoImplementation>();
services.AddScoped<IImageFileRepository, ImageFileImplementation>();
services.AddScoped<IMovieFileRepository, MovieFileImplementation>();
services.AddScoped<IAccessRepository, AccessImplementation>();
services.AddScoped<ITopicMovieRepository, TopicMovieImplementation>();
services.AddScoped<ITextInfoRepository, TextInfoImplementation>();
services.AddScoped<IBooksAndArticlesRepository, BooksAndArticlesImplementation>();
services.AddScoped<IAudioBookRepository, AudioBookImplementation>();
services.AddScoped<IAudioInfoRepository, AudioInfoImplementation>();
services.AddScoped<IFilmFileRepository, FilmFileImplementation>();
services.AddScoped<IIconRepository, IconImplementation>();
services.AddScoped<IIconTypeRepository, IconTypeImplementation>();

#endregion

#region Открыть папку на компьютере

services.AddScoped<IFolderOpener, FolderOpener>();

#endregion

#endregion

#region Configure the HTTP request pipeline

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Shevkunenko/Error{0}");

#region Принудительное перенаправление на HTTPS

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

#endregion

#region Работа со статическими файлами

app.UseStaticFiles();
app.MapStaticAssets();

#endregion

#region Работа с сессиями

app.UseSession();

#endregion

#region Минификация HTML

app.UseWebMarkupMin();

#endregion

#region Маршруты

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area?}/{controller=Shevkunenko}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
        name: "videos_area",
        areaName: "Videos",
        pattern: "videos/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute("pagination", "Video-na-saite/Stranica{pageNumber}",
    new { Controller = "AllVideo", Action = "Index" });

#region Книги о Сергее Шевкуненко

app.MapControllerRoute(
    name: "books_about_shevkunenko",
    pattern: "Книги-о-Сергее-Шевкуненко",
    defaults: new { Controller = "Books", Action = "Index" });

#endregion

#region Страницы книги и аудиокниги

app.MapControllerRoute(
    name: "pages_of_book",
    pattern: "Книга/{bookCaption}/Страница-{pageNumber}",
    defaults: new { Controller = "Books", Action = "Book" });

app.MapControllerRoute(
    name: "pages_of_audiobook",
    pattern: "Аудиокнига/{audioBookCaption}/Часть-{audioBookPart}/Читает-{audioActor}",
    defaults: new { Controller = "Books", Action = "AudioBook" });

#endregion

#region Альбом фотографий из книги

app.MapControllerRoute(
    name: "album_page_photo",
    pattern: "{albumCaption}/Альбом/Страница-{pageNumber}/Фото-{imageId}",
    new { Controller = "Books", Action = "PhotoAlbum" });

app.MapControllerRoute(
    name: "album_paging",
    pattern: "{albumCaption}/Альбом/Страница-{pageNumber}",
    new { Controller = "Books", Action = "PhotoAlbum" });

app.MapControllerRoute(
    name: "album_photo",
    pattern: "{albumCaption}/Альбом/Фото-{imageId}",
    new { Controller = "Books", Action = "PhotoAlbum" });

#endregion

#region Страница фильма

app.MapControllerRoute(
    name: "search_of_films",
    pattern: "Фильмы--на--сайте/Страница--{pageNumber}",
    defaults: new { Controller = "Films", Action = "Index" });

app.MapControllerRoute(
    name: "list_of_films",
    pattern: "Фильмы--на--сайте",
    defaults: new { Controller = "Films", Action = "Index" });

app.MapControllerRoute(
    name: "film",
    pattern: "Фильм--«{filmCaption}»",
    defaults: new { Controller = "Films", Action = "Film" });

#endregion

#region Альбом фотографий из фильма

app.MapControllerRoute(
    name: "album_page_photo_film",
    pattern: "Фильм--«{filmCaption}»/Кадры/Страница-{pageNumber}/Фото-{imageId}",
    defaults: new { Controller = "Films", Action = "FilmPhotoAlbum" });

app.MapControllerRoute(
    name: "album_paging_film",
    pattern: "Фильм--«{filmCaption}»/Кадры/Страница-{pageNumber}",
    defaults: new { Controller = "Films", Action = "FilmPhotoAlbum" });

app.MapControllerRoute(
    name: "album_photo_film",
    pattern: "Фильм--«{filmCaption}»/Кадры/Фото-{imageId}",
    defaults: new { Controller = "Films", Action = "FilmPhotoAlbum" });

#endregion

app.MapControllerRoute(
        name: "default",
        pattern: "{Controller=Shevkunenko}/{Action=Index}/{id?}");

app.MapRazorPages();

#endregion

#endregion

app.Run();