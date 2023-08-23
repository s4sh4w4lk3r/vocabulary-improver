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
        await db.Users.AddAsync(user);
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
    public static async Task<ApiUser?> GetUserToValidateAsync(this MySqlDbContext db, ApiUser user)
    {
        user.ThrowIfNull("В метод GetUserToValidateAsync пришел null user.").IfDefault(g => g.Guid, "В метод GetUserToValidateAsync пришел пустой Guid.");
        var validUser = await db.Set<ApiUser>().FirstOrDefaultAsync(u => u.Email == user.Email && u.Username == user.Username);
        return validUser;
    }
}
