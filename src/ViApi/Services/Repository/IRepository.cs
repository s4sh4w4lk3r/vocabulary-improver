using ViApi.Types.Common;
using ViApi.Types.Common.Users;
using ViApi.Types.Telegram;
using ViApi.Types.Users;

namespace ViApi.Services.Repository
{
    public interface IRepository
    {
        Task InsertOrUpdateUserSessionAsync(TelegramSession userSession, CancellationToken cancellationToken = default);

        Task<bool> InsertDictionaryAsync(Dictionary dictionary, CancellationToken cancellationToken = default);
        Task<bool> DeleteDictionaryAsync(Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default);
        Task<bool> RenameDictionaryAsync(Guid userGuid, Guid dictGuid, string newName, CancellationToken cancellationToken = default);
        Task<bool> CheckDictionaryIsExistAsync(Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default);

        Task<bool> InsertUserAsync(UserBase user, CancellationToken cancellationToken = default);
        Task<ApiUser?> GetValidUserAsync(string username, string email, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserAndSessionAsync(TelegramSession session, CancellationToken cancellationToken = default);
        Task<TelegramSession> GetOrRegisterSessionAsync(long chatId64, string firstname, CancellationToken cancellationToken = default);

        Task<bool> InsertWordAsync(Guid userGuid, Guid dictGuid, string sourceWord, string targetWord, CancellationToken cancellationToken = default);
        Task<bool> DeleteWordAsync(Guid userGuid, Guid dictGuid, Guid wordGuid, CancellationToken cancellationToken = default);
        Task<List<Word>?> GetWordsAsync(Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default);
        Task UpdateWordRating(Guid userGuid, Guid dictGuid, Guid wordGuid, RatingAction action, CancellationToken cancellationToken = default);
        Task<List<Dictionary>> GetDicionariesList(Guid userGuid, CancellationToken cancellationToken = default);
    }
    public enum RatingAction { Decrease, Increase};
}
