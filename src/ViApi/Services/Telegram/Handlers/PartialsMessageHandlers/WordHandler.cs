using Telegram.Bot;
using ViApi.Types.Enums;

namespace ViApi.Services.Telegram.Handlers.PartialsMessageHandlers;

public partial class MessageHandler
{
    private async Task DeleteWordAsync()
    {
        if (_session.State is not UserState.DeletingWord) { return; }
        bool parseOk = int.TryParse(_recievedMessage, out int id);
        if (parseOk is false)
        {
            await _botClient.SendTextMessageAsync(_session.TelegramId, "Номер слова не распознан, введите заново", cancellationToken: _cancellationToken);
            return;
        }

        var words = await _repository.GetWordsAsync(_session.UserGuid, _session.DictionaryGuid, _cancellationToken);
        var wordToDelete = words?.ElementAtOrDefault(id - 1);

        if (wordToDelete is not null)
        {
            if (wordToDelete.Dictionary is null)
            {
                await _botClient.SendTextMessageAsync(_session.TelegramId, "Не обнаружен Guid пользователя в методе DeleteWordAsync", cancellationToken: _cancellationToken);
                return;
            }

            var delMySqlTask = _repository.DeleteWordAsync(wordToDelete.Dictionary.UserGuid, wordToDelete.DictionaryGuid, wordToDelete.Guid, _cancellationToken);
            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Слово удалено, обновите список слов", cancellationToken: _cancellationToken);
            _session.State = UserState.DictSelected;
            var updateStateTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
            await Task.WhenAll(delMySqlTask, sendMessageTask, updateStateTask);
        }
        else
        {
            await _botClient.SendTextMessageAsync(_session.TelegramId, "Слово с таким номером не найдено, введите заново", cancellationToken: _cancellationToken);
            return;
        }
    }
    private async Task AddNewWordAsync()
    {
        if (_session.State == UserState.AddingWord && _session.DictionaryGuid != default)
        {
            var mas = _recievedMessage.Split(':');
            if (mas.Length != 2)
            {
                await _botClient.SendTextMessageAsync(chatId: _session.TelegramId, "Неверный формат", cancellationToken: _cancellationToken);
                return;
            }

            try
            {
                string sourceWord = mas[0];
                string targetWord = mas[1];
                if (string.IsNullOrEmpty(sourceWord) || string.IsNullOrEmpty(targetWord))
                {
                    await _botClient.SendTextMessageAsync(chatId: _session.TelegramId, "Неверный формат", cancellationToken: _cancellationToken);
                    return;
                }

                var insertTask = _repository.InsertWordAsync(_session.UserGuid, _session.DictionaryGuid, sourceWord, targetWord, cancellationToken: _cancellationToken);
                var sendOkMessage = _botClient.SendTextMessageAsync(_session.TelegramId, "Словосочетание добавлено", cancellationToken: _cancellationToken);
                await Task.WhenAll(insertTask, sendOkMessage);
                _session.State = UserState.Default;
                await _repository.InsertOrUpdateUserSessionAsync(_session, cancellationToken: _cancellationToken);
            }
            catch { }
        }
    }
}
