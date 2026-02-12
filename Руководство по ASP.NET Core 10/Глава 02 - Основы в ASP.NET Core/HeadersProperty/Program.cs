#region Example 01

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//app.Run(async (context) =>
//{
//    context.Response.ContentType = "text/html; charset=utf-8";
//    var stringBuilder = new System.Text.StringBuilder("<table>");

//    foreach (var header in context.Request.Headers)
//    {
//        stringBuilder.Append($"<tr><td>{header.Key}</td><td>{header.Value}</td></tr>");
//    }
//    stringBuilder.Append("</table>");

//    await context.Response.WriteAsync(stringBuilder.ToString());
//});

//app.Run();

#endregion

#region Example02

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//app.Run(async (context) =>
//{
//    //var acceptHeaderValue = context.Request.Headers.Accept;
//    var acceptHeaderValue = context.Request.Headers["accept"];

//    await context.Response.WriteAsync($"Accept: {acceptHeaderValue}");
//});

//app.Run();

#endregion

#region Example 03 Свойство Path

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//app.Run(async (context) => await context.Response.WriteAsync($"Path: {context.Request.Path}"));

//app.Run();

#endregion

#region Example 04 Свойство Path

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//app.Run(async (context) =>
//{
//    var path = context.Request.Path;
//    var now = DateTime.Now;
//    var response = context.Response;

//    if (path == "/date")
//        await response.WriteAsync($"Date: {now.ToShortDateString()}");
//    else if (path == "/time")
//        await response.WriteAsync($"Time: {now.ToShortTimeString()}");
//    else
//        await response.WriteAsync("Hello METANIT.COM");
//});

//app.Run();

#endregion

#region Example 05 Свойство QueryString

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//app.Run(async (context) =>
//{
//    context.Response.ContentType = "text/html; charset=utf-8";

//    await context.Response.WriteAsync($"<p>Path: {context.Request.Path}</p>" +
//        $"<p>QueryString: {context.Request.QueryString}</p>");
//});

//app.Run();

#endregion

#region Example 06 Свойство Query

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//app.Run(async (context) =>
//{
//    context.Response.ContentType = "text/html; charset=utf-8";
//    var stringBuilder = new System.Text.StringBuilder("<h3>Параметры строки запроса</h3><table>");
//    stringBuilder.Append("<tr><td>Параметр</td><td>Значение</td></tr>");

//    foreach (var param in context.Request.Query)
//    {
//        stringBuilder.Append($"<tr><td>{param.Key}</td><td>{param.Value}</td></tr>");
//    }
//    stringBuilder.Append("</table>");

//    await context.Response.WriteAsync(stringBuilder.ToString());
//});

//app.Run();

#endregion

#region Example 07 Свойство Query

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Run(async (context) =>
{
    string name = context.Request.Query["name"];
    string age = context.Request.Query["age"];
    await context.Response.WriteAsync($"{name} - {age}");
});

app.Run();

#endregion