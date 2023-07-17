using ViAPI.Database;
using ViAPI.Entities;
using ViAPI.StaticMethods;
using static ViAPI.Tests.TestDatabase;



using (ViDbContext db = new ViDbContext())
{
    Console.WriteLine(db.ValidateHash("maria", "423fsdfsdfssd")); ;

}


