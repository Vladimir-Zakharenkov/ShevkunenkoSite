var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Map("/", () => "Index Page");
app.Map("/user", () => new Person("Tom", 37));


app.Run();

record class Person(string Name, int Age);