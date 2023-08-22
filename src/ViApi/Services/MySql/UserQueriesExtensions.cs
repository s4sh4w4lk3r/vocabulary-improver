using Microsoft.EntityFrameworkCore;
using ViApi.Types.Common.Users;
using ViApi.Types.Users;

namespace ViApi.Services.MySql;

public static class UserQueriesExtensions
{
    public static async Task<bool> InsertUserAsync(this MySqlDbContext db, UserBase user)
    {
        if (user is TelegramUser telegramUser)
        {
            bool exists = await db.Set<TelegramUser>().AnyAsync(u => u.TelegramId == telegramUser.TelegramId);
            if (exists) return false;
        }
        if (user is ApiUser apiUser)
        {
            bool exists = await db.Set<ApiUser>().AnyAsync(u => u.Username == apiUser.Username || u.Email == apiUser.Email);
            if (exists) return false;
        }
        db.Users.Add(user);
        int stringsChanged = await db.SaveChangesAsync();

        if (stringsChanged == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
