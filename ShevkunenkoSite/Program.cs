WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region Configuration

ConfigurationManager configuration = builder.Configuration;

configuration.AddJsonFile("DataConfig.json");

configuration.Bind("ProjectData", new DataConfig());

IWebHostEnvironment environment = builder.Environment;

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

#region Кодирование в Unicode

services.Configure<WebEncoderOptions>(options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

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

builder.Services.AddDistributedMemoryCache();

#endregion

#region AddSession

builder.Services.AddSession();

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
    if (environment.IsDevelopment())
    {
        options.UseSqlServer(configuration["ConnectionStrings:ShevkunenkoSitePAPA"] ?? throw new InvalidOperationException("Connection string 'ShevkunenkoSitePAPA' not found."));
    }
    else
    {
        options.UseSqlServer(configuration["ConnectionStrings:ShevkunenkoSite"] ?? throw new InvalidOperationException("Connection string 'ShevkunenkoSite' not found."));
    }

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
services.AddScoped<IIconFileRepository, IconFileImplementation>();
services.AddScoped<IImageFileRepository, ImageFileImplementation>();
services.AddScoped<IMovieFileRepository, MovieFileImplementation>();
services.AddScoped<IAccessRepository, AccessImplementation>();
services.AddScoped<ITopicMovieRepository, TopicMovieImplementation>();
services.AddScoped<ITextInfoRepository, TextInfoImplementation>();
services.AddScoped<IBooksAndArticlesRepository, BooksAndArticlesImplementation>();
services.AddScoped<IAudioBookRepository, AudioBookImplementation>();
services.AddScoped<IAudioInfoRepository, AudioInfoImplementation>();

#endregion

#endregion

#region  Configure the HTTP request pipeline

WebApplication app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/Shevkunenko/Error{0}");

// The default HSTS value is 30 days. You may want to change this
// for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();

#region Принудительное перенаправление на HTTPS

app.UseHttpsRedirection();

#endregion

#region Работа со статическими файлами

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
    pattern: "{area}/{controller=Shevkunenko}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
        name: "videos_area",
        areaName: "videos",
        pattern: "videos/{controller=Номе}/{action=Index}/{id?}");

app.MapControllerRoute("pagination", "Video-na-saite/Stranica{pageNumber}",
    new { Controller = "AllVideo", Action = "Index" });

app.MapControllerRoute("PhotoAlbum", "Shevkunenko/PhotoAlbum/Page{pageNumber}/Photo-{imageId}",
        new { Controller = "Shevkunenko", Action = "PhotoAlbum" });

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
    defaults: new { Controller = "Books", Action = "PhotoAlbum" });

app.MapControllerRoute(
    name: "album_paging",
    pattern: "{albumCaption}/Альбом/Страница-{pageNumber}",
    defaults: new { Controller = "Books", Action = "PhotoAlbum" });

app.MapControllerRoute(
    name: "album_photo",
    pattern: "{albumCaption}/Альбом/Фото-{imageId}",
    defaults: new { Controller = "Books", Action = "PhotoAlbum" });

#endregion

app.MapControllerRoute(
        name: "default",
        pattern: "{Controller=Shevkunenko}/{Action=Index}/{id?}");

app.MapRazorPages();

#endregion

#endregion

app.Run();