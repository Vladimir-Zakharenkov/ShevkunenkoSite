var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.UseWelcomePage();   // подключение WelcomePageMiddleware

app.Run();
