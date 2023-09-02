using Microsoft.EntityFrameworkCore;
using ViApi.Types.Common.Users;
using ViApi.Types.Users;

namespace ViApi.Services.MySql;

public static class UserQueriesExtensions
{
    public static async Task<bool> InsertUserAsync(this MySqlDbContext db, UserBase user, CancellationToken cancellationToken = default)
    {
        if (user is TelegramUser telegramUser)
        {
            bool exists = await db.Set<TelegramUser>().AnyAsync(u => u.TelegramId == telegramUser.TelegramId, cancellationToken);
            if (exists) return false;
        }
        if (user is ApiUser apiUser)
        {
            bool exists = await db.Set<ApiUser>().AnyAsync(u => u.Username == apiUser.Username || u.Email == apiUser.Email, cancellationToken);
            if (exists) return false;
        }
        await db.Users.AddAsync(user, cancellationToken);
        int stringsChanged = await db.SaveChangesAsync(cancellationToken);

        if (stringsChanged == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static async Task<ApiUser?> GetValidUserAsync(this MySqlDbContext db, string username, string email, CancellationToken cancellationToken = default)
    {
        const string INVALID_TEMP_PASSWORD = "Pa55sword!";
        const string INVALID_TEMP_FIRSTNAME = "Billy";
 
        //Создается временный юзер, чтобы введенные в параметр данные прошли проверку.
        var tempUser = new ApiUser(Guid.NewGuid(), INVALID_TEMP_FIRSTNAME, username, email, INVALID_TEMP_PASSWORD);
        var validUser = await db.Set<ApiUser>().FirstOrDefaultAsync(u => u.Email == tempUser.Email && u.Username == tempUser.Username, cancellationToken);
        return validUser;
    }
}
