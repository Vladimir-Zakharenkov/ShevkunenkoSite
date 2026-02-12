var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Map("/", () => "Index Page");

app.Map("/about", async (context) =>
{
    await context.Response.WriteAsync("About Page");
});

app.Run();