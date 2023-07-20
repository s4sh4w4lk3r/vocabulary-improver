using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Xml.Linq;
using ViAPI.Auth;
using ViAPI.Database;
using ViAPI.Other;

var builder = WebApplication.CreateBuilder();

builder.Services.AddDbContext<ViDbContext>(options => ViConfiguration.GetDatabaseOptions(options));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => Accounting.GetJwtOptions(options));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/api/auth/login/tg/{telegramid}", (ViDbContext db, string telegramid) => EndpointMethods.GetJwtByTelegramId(db, telegramid)); //OK
app.MapGet("/api/auth/register/tg", (HttpContext http, ViDbContext db) => EndpointMethods.RegisterTelegramUser(http, db)); //OK

app.MapPost("/api/auth/login", (HttpContext http, ViDbContext db) => EndpointMethods.GetJwtByLogin(http, db)); //OK
app.MapPost("/api/auth/register", (HttpContext http, ViDbContext db) => EndpointMethods.RegisterRegistredUser(http, db));


app.MapGet("/api/dicts/get", [Authorize] (HttpContext http, ViDbContext db) => EndpointMethods.GetDictsByUserFromContext(http, db)); //OK
app.MapGet("/api/dicts/add/{name}", [Authorize] (HttpContext http, ViDbContext db, string name) => EndpointMethods.AddDictionary(http, db, name)); //OK
app.MapGet("/api/dicts/remove/{dictguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid dictguid) => EndpointMethods.RemoveDictionary(http, db, dictguid)); //OK
app.MapGet("/api/dicts/editname/{dictguid:guid}/{name}", [Authorize] (HttpContext http, ViDbContext db, Guid dictguid, string name) => EndpointMethods.EditDictionaryName(http, db, dictguid, name)); //OK


app.MapGet("/api/words/get/{dictguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid dictguid) => EndpointMethods.GetWords(http, db, dictguid)); //OK
app.MapPost("/api/words/add", [Authorize] (HttpContext http, ViDbContext db) => EndpointMethods.AddWord(http, db)); //OK
app.MapGet("/api/words/remove/{wordguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid wordguid) => EndpointMethods.RemoveWord(http, db, wordguid)); //OK
app.MapGet("/api/words/increase/{wordguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid wordguid) => EndpointMethods.EditRating(http, db, wordguid, ViDbContext.RatingAction.Increase)); //OK
app.MapGet("/api/words/decrease/{wordguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid wordguid) => EndpointMethods.EditRating(http, db, wordguid, ViDbContext.RatingAction.Decrease)); //OK



app.Run();
