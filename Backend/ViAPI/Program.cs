using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using ViAPI.Auth;
using ViAPI.Database;
using ViAPI.Other;

var builder = WebApplication.CreateBuilder();

ViDbContext.EnsureDatabaseWorking();
builder.Services.AddDbContext<ViDbContext>(options => ViConfiguration.GetDatabaseOptions(options));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => Accounting.GetJwtOptions(options));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => Results.Ok("Hello World."));

app.MapGet("/api/auth/login/tg/{telegramid}", (ViDbContext db, string telegramid) => EndpointMethods.GetJwtByTelegramId(db, telegramid));
app.MapPost("/api/auth/register/tg", (HttpContext http, ViDbContext db) => EndpointMethods.RegisterTelegramUserAsync(http, db));

app.MapPost("/api/auth/login", (HttpContext http, ViDbContext db) => EndpointMethods.GetJwtByLoginAsync(http, db));
app.MapPost("/api/auth/register", (HttpContext http, ViDbContext db) => EndpointMethods.RegisterRegistredUserAsync(http, db));
//reset password, change password, username, firstname ���� ��������.


app.MapGet("/api/dicts/get", [Authorize] (HttpContext http, ViDbContext db) => EndpointMethods.GetDictsByUserFromContext(http, db));
app.MapGet("/api/dicts/add/{name}", [Authorize] (HttpContext http, ViDbContext db, string name) => EndpointMethods.AddDictionary(http, db, name));
app.MapGet("/api/dicts/remove/{dictguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid dictguid) => EndpointMethods.RemoveDictionary(http, db, dictguid));
app.MapGet("/api/dicts/editname/{dictguid:guid}/{name}", [Authorize] (HttpContext http, ViDbContext db, Guid dictguid, string name) => EndpointMethods.EditDictionaryName(http, db, dictguid, name));


app.MapGet("/api/words/get/{dictguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid dictguid) => EndpointMethods.GetWords(http, db, dictguid));
app.MapPost("/api/words/add", [Authorize] (HttpContext http, ViDbContext db) => EndpointMethods.AddWordAsync(http, db));
app.MapGet("/api/words/remove/{wordguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid wordguid) => EndpointMethods.RemoveWord(http, db, wordguid));
app.MapGet("/api/words/increase/{wordguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid wordguid) => EndpointMethods.EditRating(http, db, wordguid, RatingAction.Increase));
app.MapGet("/api/words/decrease/{wordguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid wordguid) => EndpointMethods.EditRating(http, db, wordguid, RatingAction.Decrease));



app.Run();
