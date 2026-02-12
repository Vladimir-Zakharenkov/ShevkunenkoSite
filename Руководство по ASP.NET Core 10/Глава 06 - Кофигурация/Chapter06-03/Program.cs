#region Example01 Конфигурация через json

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//builder.Configuration.AddJsonFile("config.json");

//app.Map("/", (IConfiguration appConfig) => $"{appConfig["person"]} - {appConfig["company"]}");

//app.Run();

#endregion

#region Example02 Конфигурация через json

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//builder.Configuration.AddJsonFile("config2.json");

//app.Map("/", (IConfiguration appConfig) =>
//{
//    var personName = appConfig["person:profile:name"];
//    var companyName = appConfig["company:name"];

//    return $"{personName} - {companyName}";
//});

//app.Run();

#endregion

#region Example03 Конфигурация через xml

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//builder.Configuration.AddXmlFile("config.xml");

//app.Map("/", (IConfiguration appConfig) => $"{appConfig["person"]} - {appConfig["company"]}");

//app.Run();

#endregion

#region Example04 Конфигурация через xml

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//builder.Configuration.AddXmlFile("config2.xml");

//app.Map("/", (IConfiguration appConfig) =>
//{
//    var personName = appConfig["person:profile:name"];
//    var companyName = appConfig["company:name"];

//    return $"{personName} - {companyName}";
//});

//app.Run();

#endregion

#region Example05 Конфигурация через ini

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

builder.Configuration.AddIniFile("config.ini");

app.Map("/", (IConfiguration appConfig) => $"{appConfig["person"]} - {appConfig["company"]}");

app.Run();

#endregion