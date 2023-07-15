using Microsoft.EntityFrameworkCore;
using ViAPI.Database;
using ViAPI.Entities;
using ViAPI.StaticMethods;

/*var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run();*/

ViDbContext db = new ViDbContext();

ReloadDb(db);
/*FillDb(db);*/
var user = db.AddTelegramUser(23141234, "Pashka");
var dict = db.AddDictionary("dict1", Guid.Empty);
var word = db.AddWord("sun", "������", dict.Guid);







void FillDb(ViDbContext database)
{

    TelegramUser dima = new(Guid.NewGuid(), "dima", 124234233);
    TelegramUser diva = new(Guid.NewGuid(), "diva", 365756812);

    RegistredUser kolya = new(Guid.NewGuid(), "Nikolas", "Kolyan", "kolya@mylo.ru", "423472346238423");
    RegistredUser masha = new(Guid.NewGuid(), "Masha", "Maria", "masha@mylo.ru", "423fsdfsdfsd");

    var rnd = new Random();

    ViDictionary dict1 = new(Guid.NewGuid(), "dict1", dima.Guid);
    ViDictionary dict2 = new(Guid.NewGuid(), "dict2", dima.Guid);
    ViDictionary dict3 = new(Guid.NewGuid(), "dict3", masha.Guid);


    Word word1 = new(Guid.NewGuid(), "figure out", "�����������", dict1.Guid, rnd.Next(0, 11));
    Word word2 = new(Guid.NewGuid(), "as well", "� �����", dict1.Guid, rnd.Next(0, 11));
    Word word3 = new(Guid.NewGuid(), "on-demand", "�� ����������", dict1.Guid, rnd.Next(0, 11));
    Word word4 = new(Guid.NewGuid(), "distraction", "����������", dict1.Guid, rnd.Next(0, 11));
    Word word5 = new(Guid.NewGuid(), "up to date", "����������", dict1.Guid, rnd.Next(0, 11));
    Word word6 = new(Guid.NewGuid(), "goosebumps ", "�������", dict2.Guid, rnd.Next(0, 11));
    Word word7 = new(Guid.NewGuid(), "spread the word", "�������������� ����", dict2.Guid, rnd.Next(0, 11));
    Word word8 = new(Guid.NewGuid(), "sporadically", "����� �� ������� ", dict2.Guid, rnd.Next(0, 11));
    Word word9 = new(Guid.NewGuid(), "due to", "��������� ", dict3.Guid, rnd.Next(0, 11));
    Word word10 = new(Guid.NewGuid(), "whereas", "� �� ����� ���", dict2.Guid, rnd.Next(0, 11));
    Word word11 = new(Guid.NewGuid(), "put it another way", "������� �������", dict3.Guid, rnd.Next(0, 11));
    Word word12 = new(Guid.NewGuid(), "referred to as", "����������", dict3.Guid, rnd.Next(0, 11));





    database.AddRange(dima, diva, masha, kolya, dict1, dict2, dict3, word1, 
        word2, word3, word4, word5, word6, word7, word8, word9, word10, word11, word12);

    database.SaveChanges();
}
void ReloadDb(ViDbContext database)
{
    database.Database.EnsureDeleted();
    database.Database.EnsureCreated();
}