#region Example01 Связка конфигурации с классом C#

//using Chapter06_07;

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//builder.Configuration.AddJsonFile("person.json");

//var tom = new Person();

//app.Configuration.Bind(tom);    // связываем конфигурацию с объектом tom

//app.Run(async (context) => await context.Response.WriteAsync($"{tom.Name} - {tom.Age}"));

//app.Run();

#endregion

#region Example02 Связка методом GET<T>

//using Chapter06_07;

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//builder.Configuration.AddJsonFile("person.json");

//app.Map("/", (IConfiguration appConfig) =>
//{
//    var tom = appConfig.Get<Person>();  // связываем конфигурацию с объектом tom

//    return $"{tom!.Name} - {tom.Age}";
//});

//app.Run();

#endregion

#region Example03 Сложная схема привязки

//using Chapter06_07;

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//builder.Configuration.AddJsonFile("person2.json");


//var tom = new Person2();

//app.Configuration.Bind(tom);

//app.Run(async (context) =>
//{
//    context.Response.ContentType = "text/html; charset=utf-8";
//    string name = $"<p>Name: {tom.Name}</p>";
//    string age = $"<p>Age: {tom.Age}</p>";
//    string company = $"<p>Company: {tom.Company?.Title}</p>";

//    string langs = "<p>Languages:</p><ul>";
//    foreach (var lang in tom.Languages)
//    {
//        langs += $"<li><p>{lang}</p></li>";
//    }
//    langs += "</ul>";

//    await context.Response.WriteAsync($"{name}{age}{company}{langs}");
//});

//app.Run();

#endregion

#region Example04 Связка конфигурации с XML

//using Chapter06_07;

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//builder.Configuration.AddXmlFile("person.xml");

//var tom = new Person2();
//app.Configuration.Bind(tom);

//app.Run(async (context) =>
//{
//    context.Response.ContentType = "text/html; charset=utf-8";
//    string name = $"<p>Name: {tom.Name}</p>";
//    string age = $"<p>Age: {tom.Age}</p>";
//    string company = $"<p>Company: {tom.Company?.Title}</p>";

//    string langs = "<p>Languages:</p><ul>";
//    foreach (var lang in tom.Languages)
//    {
//        langs += $"<li><p>{lang}</p></li>";
//    }
//    langs += "</ul>";

//    await context.Response.WriteAsync($"{name}{age}{company}{langs}");
//});

//app.Run();

#endregion

#region Example05 Привязка секций конфигурации

using Chapter06_07;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

builder.Configuration.AddJsonFile("person2.json");

Company? company = app.Configuration.GetSection("company").Get<Company>();

app.Run(async (context) =>
{
    await context.Response.WriteAsync($"{company!.Title} - {company.Country}");
});

app.Run();

#endregion
