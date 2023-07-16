using ViAPI.Database;
using ViAPI.Entities;
using ViAPI.StaticMethods;
using static ViAPI.Tests.TestDatabase;



User user;
using (ViDbContext db = new ViDbContext())
{
/*    ReloadDb(db);
    FillDb(db);*/
    user = db.Users.Find(Guid.Parse("2e3bb8ff-8195-489e-bd45-d710e5743902"));

}


using (ViDbContext db = new ViDbContext())

{
    user.Firstname = "pizdeccccc";
    db.SaveEditedUser(user);
}

