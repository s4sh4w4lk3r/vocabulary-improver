using ViApi.Types.Common;
using ViApi.Types.Common.Users;
using ViApi.Types.Telegram;
using ViApi.Types.Users;

namespace ViApi.Services.Repository
{
    public interface IRepository
    {
        Task InsertOrUpdateUserSessionAsync(TelegramSession userSession, CancellationToken cancellationToken = default);
        Task<TelegramSession?> GetUserSessionAsync(Guid userGuid, CancellationToken cancellationToken = default);
        Task<TelegramSession?> GetUserSessionAsync(long telegramId, CancellationToken cancellationToken = default);
        Task DeleteUserSessionAsync(TelegramSession userSession, CancellationToken cancellationToken = default);

        Task<bool> InsertDictionaryAsync(Dictionary dictionary, CancellationToken cancellationToken = default);
        Task<bool> DeleteDictionaryAsync(Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default);
        Task<bool> RenameDictionaryAsync(Guid userGuid, Guid dictGuid, string newName, CancellationToken cancellationToken = default);

        Task<bool> InsertUserAsync(UserBase user, CancellationToken cancellationToken = default);
        Task<ApiUser?> GetValidUserAsync(string username, string email, CancellationToken cancellationToken = default);
        Task<TelegramSession> RegisterUserAsync(long chatId64, string firstname);
        Task<TelegramUser?> TryGetUserFromSqlAsync(long chatId64);
        Task<TelegramSession?> TryGetSessionFromMongoAsync(long chatId64);

        Task<bool> InsertWordAsync(Guid userGuid, Guid dictGuid, string sourceWord, string targetWord, CancellationToken cancellationToken = default);
        Task<bool> DeleteWordAsync(Guid userGuid, Guid dictGuid, Guid wordGuid, CancellationToken cancellationToken = default);
        Task<List<Word>?> GetWordsAsync(Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default);
    }
}
