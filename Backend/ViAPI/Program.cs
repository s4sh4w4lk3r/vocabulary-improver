using Microsoft.EntityFrameworkCore;
using ViAPI.Database;
using ViAPI.Entities;
using ViAPI.StaticMethods;

/*var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();*/

var db = new ViDbContext();



/*
TelegramUser dima = new(Guid.NewGuid(), "dima", 123);
TelegramUser diva = new(Guid.NewGuid(), "diva", 312);

RegistredUser kolya = new(Guid.NewGuid(), "Nikolas", "Kolyan", "kolya@mylo.ru", "423472346238423");
RegistredUser masha = new(Guid.NewGuid(), "Masha", "Maria", "masha@mylo.ru", "423fsdfsdfsd");

Word word1 = new(Guid.NewGuid(), "привет", "hello");
Word word2 = new(Guid.NewGuid(), "пока", "poka");
Word word3 = new(Guid.NewGuid(), "привет", "bjonjur");
Word word4 = new(Guid.NewGuid(), "пока", "apcvhjo");

var words = new List<Word>()
{
    word1, word2, word3, word4
};

ViDictionary dict1 = new(Guid.NewGuid(), "dict1", dima, words);


db.AddRange(dima, diva, kolya, masha, dict1);

db.SaveChanges();*/

var users = db.Users.ToList();
var dicts = db.Dictionaries.ToList();
var wordss = db.Words.ToList();
Console.WriteLine();


/*app.MapGet("/", () => "Hello");

app.Run();*/
