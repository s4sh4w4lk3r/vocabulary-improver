using Telegram.Bot;
using ViApi.Types.Common;
using ViApi.Types.Enums;

namespace ViApi.Services.Telegram.Handlers.PartialsMessageHandlers;

public partial class MessageHandler
{
    private async Task PlayAsync()
    {
        if (_session.State is not UserState.Playing || _session.DictionaryGuid == default)
        {
            await _botClient.SendTextMessageAsync(_session.TelegramId, "Ошибка состояния или пустой гуид в сессиях", cancellationToken: _cancellationToken);
            return;
        }

        if (_session.GameQueue is not null && _session.GameQueue.Count > 0)
        {
            if (_session.GameQueue.TryDequeue(out Word? currentWord) is true && currentWord is not null)
            {
                if (currentWord is null)
                {
                    await _botClient.SendTextMessageAsync(_session.TelegramId, $"Произошла ошибка получения из базу данных, конец игры", cancellationToken: _cancellationToken);
                    _session.State = UserState.DictSelected;
                    return;
                }

                if (currentWord.TargetWord == _recievedMessage)
                {
                    var updateTask = _repository.UpdateWordRating(_session.UserGuid, _session.DictionaryGuid, currentWord.Guid, Repository.RatingAction.Increase, _cancellationToken);
                    var messageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Правильно, молодец", cancellationToken: _cancellationToken);
                    await Task.WhenAll(updateTask, messageTask);
                }
                else
                {
                    var updateTask = _repository.UpdateWordRating(_session.UserGuid, _session.DictionaryGuid, currentWord.Guid, Repository.RatingAction.Decrease, _cancellationToken);
                    var messageTask = _botClient.SendTextMessageAsync(_session.TelegramId, $"Неправильно, верный ответ: {currentWord.TargetWord}", cancellationToken: _cancellationToken);
                    await Task.WhenAll(updateTask, messageTask);
                }

                await _repository.InsertOrUpdateUserSessionAsync(_session);

                if (_session.GameQueue.TryPeek(out Word? nextWord) is true && nextWord is not null)
                {
                    await _botClient.SendTextMessageAsync(_session.TelegramId, $"{nextWord.SourceWord} ---> ?", cancellationToken: _cancellationToken);
                }

            }
            if (_session.GameQueue.Count == 0)
            {
                _session.State = UserState.DictSelected;
                var messageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Game Over.", cancellationToken: _cancellationToken);
                var mongoTask = _repository.InsertOrUpdateUserSessionAsync(_session);
                await Task.WhenAll(messageTask, mongoTask);
            }
            return;
        }
        else
        {
            _session.State = UserState.DictSelected;
            var messageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Game Over.", cancellationToken: _cancellationToken);
            var mongoTask = _repository.InsertOrUpdateUserSessionAsync(_session);
            await Task.WhenAll(messageTask, mongoTask);
            return;
        }

    }
    private async Task StartGameAsync()
    {
        var words = await _repository.GetWordsAsync(_session.UserGuid, _session.DictionaryGuid, _cancellationToken);
        if (words is null || words.Count == 0)
        {
            await _botClient.SendTextMessageAsync(_session.TelegramId, "Добавьте слова, чтобы сыграть", cancellationToken: _cancellationToken);
            _session.State = UserState.DictSelected;
            await _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
            return;
        }

        Random rand = new Random();
        words = words.OrderBy(_ => rand.Next()).ThenBy(w => w.Rating).ToList();
        _session.GameQueue = new Queue<Word>(words);
        _session.State = UserState.Playing;
        await _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
        await _botClient.SendTextMessageAsync(_session.TelegramId, $"{_session.GameQueue.Peek().SourceWord} ---> ?", cancellationToken: _cancellationToken);
    }
}
