#region Example 01

//var builder = WebApplication.CreateBuilder();

//var app = builder.Build();

//app.Run(async (context) => await context.Response.WriteAsync("Hello METANIT.COM"));

//app.Run();

#endregion

#region Example 02

//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();
//app.Run(HandleRequst);
//app.Run();

//static async Task HandleRequst(HttpContext context)
//{
//    await context.Response.WriteAsync("Hello METANIT.COM 2");
//}

#endregion

#region Example 03

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

int x = 2;

app.Run(async (context) =>
{
    x *= 2;  //  2 * 2 = 4

    await context.Response.WriteAsync($"Result: {x}");
});

app.Run();

#endregion