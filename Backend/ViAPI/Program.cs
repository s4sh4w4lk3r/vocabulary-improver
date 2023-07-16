using ViAPI.Database;
using ViAPI.Entities;
using ViAPI.StaticMethods;
using static ViAPI.Tests.TestDatabase;



Word word;
using (ViDbContext db = new ViDbContext())
{
    /*    ReloadDb(db);
        FillDb(db);*/

    word = db.Words.First();
}


