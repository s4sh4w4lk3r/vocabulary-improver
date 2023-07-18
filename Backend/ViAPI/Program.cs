using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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


app.Map("/token/{guid}", (string guid) => Accounting.GenerateJwt(Guid.Parse(guid)));

app.MapGet("/get/dicts", [Authorize] (HttpContext context, ViDbContext db) => GetDicts(context, db));


app.Run();
