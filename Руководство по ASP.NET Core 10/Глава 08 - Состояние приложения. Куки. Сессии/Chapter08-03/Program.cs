#region Default

//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

//app.Run();

#endregion

#region Example01

var builder = WebApplication.CreateBuilder();

builder.Services.AddDistributedMemoryCache();// добавляем IDistributedMemoryCache
builder.Services.AddSession();  // добавляем сервисы сессии

var app = builder.Build();

app.UseSession();   // добавляем middleware для работы с сессиями

app.Run(async (context) =>
{
    if (context.Session.Keys.Contains("name"))
        await context.Response.WriteAsync($"Hello {context.Session.GetString("name")}!");
    else
    {
        context.Session.SetString("name", "Tom");
        await context.Response.WriteAsync("Hello World!");
    }
});

app.Run();

#endregion

#region Example02

//var builder = WebApplication.CreateBuilder();

//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(options =>
//{
//    options.Cookie.Name = ".MyApp.Session";
//    options.IdleTimeout = TimeSpan.FromSeconds(3600);
//    options.Cookie.IsEssential = true;
//});

//var app = builder.Build();

//app.UseSession();   // добавляем middleware для работы с сессиями

//app.Run(async (context) =>
//{
//    if (context.Session.Keys.Contains("name"))
//        await context.Response.WriteAsync($"Hello {context.Session.GetString("name")}!");
//    else
//    {
//        context.Session.SetString("name", "Tom");
//        await context.Response.WriteAsync("Hello World!");
//    }
//});

//app.Run();

#endregion

#region Example03

//using Chapter08_03;

//var builder = WebApplication.CreateBuilder();

//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession();

//var app = builder.Build();

//app.UseSession();

//app.Run(async (context) =>
//{
//    if (context.Session.Keys.Contains("person"))
//    {
//        Person? person = context.Session.Get<Person>("person");
//        await context.Response.WriteAsync($"Hello {person?.Name}, your age: {person?.Age}!");
//    }
//    else
//    {
//        Person person = new() { Name = "Tom", Age = 22 };

//        context.Session.Set<Person>("person", person);

//        await context.Response.WriteAsync("Hello World!");
//    }
//});

//app.Run();

#endregion
