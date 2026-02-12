using Chapter06_06;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

builder.Configuration.AddTextFile("config.txt");

app.Map("/", (IConfiguration appConfig) => $"{appConfig["name"]} - {appConfig["age"]}");

app.Run();