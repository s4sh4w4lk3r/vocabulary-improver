using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;
using ViAPI.Auth;
using ViAPI.Database;
using ViAPI.StaticMethods;
using static ViAPI.StaticMethods.EndpointMethods;

var builder = WebApplication.CreateBuilder();

builder.Services.AddDbContext<ViDbContext>(options => ViConfiguration.GetDatabaseOptions(options));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => Accounting.GetJwtOptions(options));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


//app.MapGet("/api/auth/login/{userguid}", (string userguid) => Accounting.GenerateJwt(Guid.Parse(userguid), 120));

app.MapGet("/api/auth/login/tg/{telegramid}", (string telegramid, ViDbContext db, HttpContext context) => Accounting.TelegramLoginHandler(telegramid, db, context));


app.MapGet("/api/dicts/get", [Authorize] (HttpContext context, ViDbContext db) => GetDicts(context, db));


app.MapGet("/api/words/get/{dictguid:guid}", [Authorize] (ViDbContext db, Guid dictguid) => GetWords(db, dictguid));
app.MapGet("/api/words/increase/{wordguid:guid}", [Authorize] (ViDbContext db, Guid wordguid) => EditRating(db, wordguid, ViDbContext.RatingAction.Increase));
app.MapGet("/api/words/decrease/{wordguid:guid}", [Authorize] (ViDbContext db, Guid wordguid) => EditRating(db, wordguid, ViDbContext.RatingAction.Decrease));


app.Run();
