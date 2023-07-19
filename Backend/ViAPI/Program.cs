using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
app.MapPost("/api/auth/login", (HttpContext http, ViDbContext db) => EndpointMethods.GetJwtByLogin(http, db));


app.MapGet("/api/dicts/get", [Authorize] (HttpContext http, ViDbContext db) => EndpointMethods.GetDictsByUserFromContext(http, db));


app.MapGet("/api/words/get/{dictguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid dictguid) => EndpointMethods.GetWords(http, db, dictguid));
app.MapGet("/api/words/increase/{wordguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid wordguid) => EndpointMethods.EditRating(http, db, wordguid, ViDbContext.RatingAction.Increase));
app.MapGet("/api/words/decrease/{wordguid:guid}", [Authorize] (HttpContext http, ViDbContext db, Guid wordguid) => EndpointMethods.EditRating(http, db, wordguid, ViDbContext.RatingAction.Decrease));



app.Run();
