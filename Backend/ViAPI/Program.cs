using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Xml.Linq;
using ViAPI.Auth;
using ViAPI.Database;
using ViAPI.StaticMethods;

var builder = WebApplication.CreateBuilder();

builder.Services.AddDbContext<ViDbContext>(options => ViConfiguration.GetDatabaseOptions(options));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => Accounting.GetJwtOptions(options));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/api/auth/login/tg/{telegramid}", (ViDbContext db, string telegramid) => EndpointMethods.GetJwtByTelegramId(db, telegramid));
/*app.MapGet("/api/auth/regiser/tg/{telegramid}/{firstname}", (ViDbContext db, string telegramid, string firstname) => EndpointMethods.GetJwtByTelegramId(db, telegramid));*/

app.MapPost("/api/auth/login", (HttpContext http, ViDbContext db) => EndpointMethods.GetJwtByLogin(http, db));
//app.MapPost("/api/auth/register", (HttpContext http, ViDbContext db) => );


app.MapGet("/api/dicts/get", [Authorize] (HttpContext http, ViDbContext db) => EndpointMethods.GetDictsByUserFromContext(http, db));
//app.MapGet("/api/dicts/add/name", [Authorize] (HttpContext http, ViDbContext db, string name) => EndpointMethods.AddDictionary(http, db, name));
//app.MapGet("/api/dicts/remove/dictguid:guid", [Authorize] (HttpContext http, ViDbContext db, Guid dictguid) => EndpointMethods.RemoveDictionary(http, db, dictguid));
//app.MapGet("/api/dicts/editname/dictguid:guid/name", [Authorize] (HttpContext http, ViDbContext db, Guid dictguid, string name) => EndpointMethods.EditDictionaryName(http, db, dictguid, name));


app.MapGet("/api/words/get/{dictguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid dictguid) => EndpointMethods.GetWords(http, db, dictguid));
app.MapGet("/api/words/add", [Authorize] (HttpContext http, ViDbContext db) => EndpointMethods.AddWord(http, db));
app.MapGet("/api/words/remove", [Authorize] (HttpContext http, ViDbContext db) => EndpointMethods.AddWord(http, db));
app.MapGet("/api/words/increase/{wordguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid wordguid) => EndpointMethods.EditRating(http, db, wordguid, ViDbContext.RatingAction.Increase));
app.MapGet("/api/words/decrease/{wordguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid wordguid) => EndpointMethods.EditRating(http, db, wordguid, ViDbContext.RatingAction.Decrease));



app.Run();
