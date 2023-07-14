using Microsoft.EntityFrameworkCore;
using ViAPI.Database;
using ViAPI.Entities;
using ViAPI.StaticMethods;

/*var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run();*/

ViDbContext db = new ViDbContext();

FillDb(db);

/*var users = db.Users.ToList();
var dicts = db.Dictionaries.ToList();
var words = db.Words.ToList();*/


Console.WriteLine();





void FillDb(ViDbContext database)
{

    TelegramUser dima = new(Guid.NewGuid(), "dima", 124234233);
    TelegramUser diva = new(Guid.NewGuid(), "diva", 365756812);

    RegistredUser kolya = new(Guid.NewGuid(), "Nikolas", "Kolyan", "kolya@mylo.ru", "423472346238423");
    RegistredUser masha = new(Guid.NewGuid(), "Masha", "Maria", "masha@mylo.ru", "423fsdfsdfsd");

    Word word1 = new(Guid.NewGuid(), "figure out", "разобраться");
    Word word2 = new(Guid.NewGuid(), "as well", "а также");
    Word word3 = new(Guid.NewGuid(), "on-demand", "по требованию");
    Word word4 = new(Guid.NewGuid(), "distraction", "отвлечение");
    Word word5 = new(Guid.NewGuid(), "up to date", "актуальный");
    Word word6 = new(Guid.NewGuid(), "goosebumps ", "мурашки");
    Word word7 = new(Guid.NewGuid(), "spread the word", "распространять инфу");
    Word word8 = new(Guid.NewGuid(), "sporadically", "время от времени ");
    Word word9 = new(Guid.NewGuid(), "due to", "благодаря ");
    Word word10 = new(Guid.NewGuid(), "whereas", "в то время как");
    Word word11 = new(Guid.NewGuid(), "put it another way", "другими словами");
    Word word12 = new(Guid.NewGuid(), "referred to as", "называется");


    ViDictionary dict1 = new(Guid.NewGuid(), "dict1", dima) {word1, word2, word3, word4 };
    ViDictionary dict2 = new(Guid.NewGuid(), "dict2", dima) { word5, word6, word7, word8 };
    ViDictionary dict3 = new(Guid.NewGuid(), "dict3", masha) { word9, word10, word11, word12 };


    database.AddRange(dict1, dict2, dict3);

    database.SaveChanges();
}