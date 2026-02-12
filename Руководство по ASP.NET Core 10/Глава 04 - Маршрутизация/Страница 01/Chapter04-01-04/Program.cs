var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Map("/", IndexHandler);
app.Map("/user", UserHandler);

app.Run();

string IndexHandler()
{
    return "Index Page";
}
Person UserHandler()
{
    return new Person("Tom", 37);
}

record class Person(string Name, int Age);