using ViAPI.Database;
using ViAPI.Entities;

/*var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();*/

var db = new ViDbContext();

TelegramUser dima = new(Guid.NewGuid(), "dima", 123);
TelegramUser diva = new(Guid.NewGuid(), "diva", 312);

RegistredUser kolya = new(Guid.NewGuid(), "Nikolas", "Kolyan", "kolya@mylo.ru", "423472346238423");
RegistredUser masha = new(Guid.NewGuid(), "Masha", "Maria", "masha@mylo.ru", "423fsdfsdfsd");

db.AddRange(dima, diva, kolya, masha);

ViDictionary dict1 = new(Guid.NewGuid(), "Dict1", dima);
ViDictionary dict2 = new(Guid.NewGuid(), "Dict2", dima);

Word word1 = new(Guid.NewGuid(), "привет", "hello");
Word word2 = new(Guid.NewGuid(), "пока", "poka");
Word word3 = new(Guid.NewGuid(), "привет", "bjonjur");
Word word4 = new(Guid.NewGuid(), "пока", "apcvhjo");

dict1.Add(word1); dict1.Add(word2); dict1.Add(word3); dict1.Add(word4);

db.AddRange(dict1, dict2);
db.SaveChanges();

/*app.MapGet("/", () => "Hello");

app.Run();*/
