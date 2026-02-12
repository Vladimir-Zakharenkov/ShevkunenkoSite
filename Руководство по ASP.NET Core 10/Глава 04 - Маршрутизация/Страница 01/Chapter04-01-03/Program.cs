var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Map("/", () => "Index Page");
app.Map("/user", () => Console.WriteLine("Request Path: /user"));

app.Run();