using ViAPI.Database;
using ViAPI.Entities;
using ViAPI.Entities.Users;

/*var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();*/

var db = new ViDbContext();

User dima = new TelegramUser(Guid.NewGuid(), "dima", 123);
User diva = new TelegramUser(Guid.NewGuid(), "diva", 312);

User kolya = new RegistredUser(Guid.NewGuid(), "Nikolas", "Kolyan", "kolya@mylo.ru", "423472346238423");
User masha = new RegistredUser(Guid.NewGuid(), "Masha", "Maria", "masha@mylo.ru", "423fsdfsdfsd");

db.Users.AddRange(dima, diva);
db.Users.AddRange(kolya, masha);
db.SaveChanges();




/*app.MapGet("/", () => "Hello");

app.Run();*/
