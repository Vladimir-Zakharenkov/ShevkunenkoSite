#region Example01

//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();

//app.Map("/", (IConfiguration appConfig) => $"{appConfig["name"]} - {appConfig["age"]}");

//app.Run();

#endregion

#region Example02

//string[] commandLineArgs = { "name=Alice", "age=29" };  // псевдопараметры командной строки

//var builder = WebApplication.CreateBuilder(commandLineArgs);
//var app = builder.Build();

//app.Map("/", (IConfiguration appConfig) => $"{appConfig["name"]} - {appConfig["age"]}");

//app.Run();

#endregion

#region Example03

//var builder = WebApplication.CreateBuilder();

//string[] commandLineArgs = { "name=Sam", "age=25" };  // псевдопараметры командной строки

//builder.Configuration.AddCommandLine(commandLineArgs);  // передаем параметры в качестве конфигурации

//var app = builder.Build();
//app.Map("/", (IConfiguration appConfig) => $"{appConfig["name"]} - {appConfig["age"]}");

//app.Run();

#endregion

#region Example04

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//app.Map("/", (IConfiguration appConfig) => $"JAVA_HOME: {appConfig["JAVA_HOME"] ?? "not set"}");

//app.Run();

#endregion

#region Example05

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
{
    {"name", "Tom"},
    {"age", "47"}
}!);

app.Map("/", (IConfiguration appConfig) =>
{
    var name = appConfig["name"];
    var age = appConfig["age"];
    return $"{name} - {age}";
});

app.Run();

#endregion