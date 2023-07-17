using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ViAPI.Auth;
using ViAPI.StaticMethods;


var builder = WebApplication.CreateBuilder();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => Accounting.GetJwtOptions(options));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.Map("/token/{guid}", (string guid) => Accounting.GenerateJwt(Guid.Parse(guid)));

app.Map("/data", [Authorize] (HttpContext context) => $"Hello World!");
app.Run();