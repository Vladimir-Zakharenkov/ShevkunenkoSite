#region Default

//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

//app.Run();

#endregion

#region Example01

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//app.Run(async (context) =>
//{
//    if (context.Request.Cookies.ContainsKey("name"))
//    {
//        string? name = context.Request.Cookies["name"];
//        await context.Response.WriteAsync($"Hello {name}!");
//    }
//    else
//    {
//        context.Response.Cookies.Append("name", "Tom");
//        await context.Response.WriteAsync("Hello World!");
//    }
//});

//app.Run();

#endregion

#region Example02

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Run(async (context) =>
{
    if (context.Request.Cookies.TryGetValue("name", out var login))
    {
        await context.Response.WriteAsync($"Hello {login}!");
    }
    else
    {
        context.Response.Cookies.Append("name", "Tom");
        await context.Response.WriteAsync("Hello World!");
    }
});

app.Run();

#endregion
