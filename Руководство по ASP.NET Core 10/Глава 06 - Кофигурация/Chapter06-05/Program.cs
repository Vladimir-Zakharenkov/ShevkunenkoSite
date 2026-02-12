#region Example01 Получение секций

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//builder.Configuration.AddJsonFile("config.json");

//app.Map("/", (IConfiguration appConfig) =>
//{
//    IConfigurationSection connStrings = appConfig.GetSection("ConnectionStrings");

//    //string? defaultConnection = connStrings.GetSection("DefaultConnection").Value;

//    //string? defaultConnection = appConfig.GetSection("ConnectionStrings:DefaultConnection").Value;

//    //string? defaultConnection = appConfig["ConnectionStrings:DefaultConnection"];

//    string? defaultConnection = appConfig.GetConnectionString("DefaultConnection");

//    return defaultConnection;
//});

//app.Run();

#endregion

#region Example02 Анализ файла конфигурации

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

builder.Configuration.AddJsonFile("project.json");

app.Map("/", (IConfiguration appConfig) => GetSectionContent(appConfig.GetSection("projectConfig")));

app.Run();

static string GetSectionContent(IConfiguration configSection)
{
    System.Text.StringBuilder contentBuilder = new();

    foreach (var section in configSection.GetChildren())
    {
        contentBuilder.Append($"\"{section.Key}\":");

        if (section.Value == null)
        {
            string subSectionContent = GetSectionContent(section);
            contentBuilder.Append($"{{\n{subSectionContent}}},\n");
        }
        else
        {
            contentBuilder.Append($"\"{section.Value}\",\n");
        }
    }
    return contentBuilder.ToString();
}

#endregion