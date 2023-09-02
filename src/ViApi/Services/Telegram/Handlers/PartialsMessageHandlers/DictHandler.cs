using Telegram.Bot;
using ViApi.Types.Common;
using ViApi.Types.Enums;

namespace ViApi.Services.Telegram.Handlers.PartialsMessageHandlers;

public partial class MessageHandler
{
    private async Task AddNewDictinaryAsync()
    {
        if (_session.State is not UserState.AddingDict)
        {
            _session.State = UserState.AddingDict;
            _session.DictionaryGuid = default;
            var insertTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Введите название для нового словаря:", cancellationToken: _cancellationToken);
            await Task.WhenAll(insertTask, sendMessageTask);
            return;
        }
        if (_session.State is UserState.AddingDict)
        {
            string newDictName = _recievedMessage;
            var newDict = new Dictionary(Guid.NewGuid(), newDictName, _session.UserGuid);
            bool dictInserted = await _repository.InsertDictionaryAsync(newDict, _cancellationToken);

            if (dictInserted is true)
            {
                _session.State = UserState.Default;
                var insertTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                var sendOkTask = _botClient.SendTextMessageAsync(_session.TelegramId, $"Словарь \"{newDictName}\" добавлен", cancellationToken: _cancellationToken);
                var sendDictListTask = SendDictionariesListAsync();
                await Task.WhenAll(insertTask, sendOkTask, sendDictListTask);
                return;
            }
            else
            {
                await _botClient.SendTextMessageAsync(_session.TelegramId, $"Словарь почему-то не добавился в базу данных", cancellationToken: _cancellationToken);
                return;
            }
        }
        await _botClient.SendTextMessageAsync(_session.TelegramId, $"Ошибка сессии", cancellationToken: _cancellationToken);
        return;
    }
    private async Task RenameDictAsync()
    {
        if (_session.State is not UserState.RenamingDict || _session.DictionaryGuid == default) { return; }

        if (string.IsNullOrWhiteSpace(_recievedMessage))
        {
            _session.State = UserState.Default;
            var sendMsgTask = _botClient.SendTextMessageAsync(_session.TelegramId, text: "Словарь не переименовался из-за неверного формата, попробуйте еще", cancellationToken: _cancellationToken);
            var mongoTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
            await Task.WhenAll(sendMsgTask, mongoTask);
            return;
        }
        string newDictName = _recievedMessage;
        bool renameOk = await _repository.RenameDictionaryAsync(_session.UserGuid, _session.DictionaryGuid, newDictName, cancellationToken: _cancellationToken);

        if (renameOk)
        {
            _session.State = UserState.Default;
            var sendMsgTask = _botClient.SendTextMessageAsync(_session.TelegramId, text: "Словарь переименован", cancellationToken: _cancellationToken);
            var mongoTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
            await Task.WhenAll(sendMsgTask, mongoTask);
        }
    }
}
