//var builder = WebApplication.CreateBuilder();
//var app = builder.Build();

//#region WriteAsJsonAsync

////app.Run(async (context) =>
////{
////    Person tom = new("Tom", 22);

////    await context.Response.WriteAsJsonAsync(tom);
////});

//#endregion

//#region WriteAsync

////app.Run(async (context) =>
////{
////    var response = context.Response;

////    response.Headers.ContentType = "application/json; charset=utf-8";

////    await response.WriteAsync("{\"name\":\"Tom\",\"age\":37}");
////});

//#endregion

//#region try - catch

////app.Run(async (context) =>
////{
////    var response = context.Response;
////    var request = context.Request;

////    if (request.Path == "/api/user")
////    {
////        var message = "Ќекорректные данные";   // содержание сообщени€ по умолчанию

////        try
////        {
////            // пытаемс€ получить данные json
////            var person = await request.ReadFromJsonAsync<Person>();

////            if (person != null) // если данные сконвертированы в Person
////                message = $"Name: {person.Name}  Age: {person.Age}";
////        }
////        catch { }
///
////        // отправл€ем пользователю данные
////        await response.WriteAsJsonAsync(new { text = message });
////    }
////    else
////    {
////        response.ContentType = "text/html; charset=utf-8";

////        await response.SendFileAsync("html/index.html");
////    }
////});

//#endregion

//#region HasJsonContentType()

//app.Run(async (context) =>
//{
//    var response = context.Response;
//    var request = context.Request;

//    if (request.Path == "/api/user")
//    {
//        var message = "Ќекорректные данные";   // содержание сообщени€ по умолчанию

//        if (request.HasJsonContentType())
//        {
//            var person = await request.ReadFromJsonAsync<Person>();

//            if (person != null)
//                message = $"Name: {person.Name} Age: {person.Age}";
//        }

//        // отправл€ем пользователю данные
//        await response.WriteAsJsonAsync(new { text = message });
//    }
//    else
//    {
//        response.ContentType = "text/html; charset=utf-8";

//        await response.SendFileAsync("html/index.html");
//    }
//});

//#endregion

//app.Run();

//public record Person(string Name, int Age);