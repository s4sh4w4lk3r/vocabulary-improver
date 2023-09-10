using Telegram.Bot;
using ViApi.Types.Common;
using ViApi.Types.Enums;
using ViApi.Types.Telegram;

namespace ViApi.Services.Telegram.Handlers.PartialsMessageHandlers;

public partial class MessageHandler
{
    private async Task PlayAsync(Guid answeredWordGuid, bool startNewGame = false)
    {
        if (_session.DictionaryGuid == default)
        {
            await _botClient.SendTextMessageAsync(_session.TelegramId, "Словарь не выбран", cancellationToken: _cancellationToken);
            return;
        }

        if (startNewGame is true)
        {
            bool gameStarted = await StartGameAsync();
            if (gameStarted is false)
            {
                return;
            }
            await _botClient.SendTextMessageAsync(_session.TelegramId, "Игра началась!", cancellationToken: _cancellationToken);
        }

        if (answeredWordGuid == default && ((_session.GameQueue is null) || (_session.GameQueue.Count == 0)))
        {
            //Если что-то пошло не так
            return;
        }

        if ((startNewGame is true) && (_session.GameQueue is not null) && (_session.GameQueue.Count > 0) && _session.State is UserState.Playing)
        {
            await GamePlayIfFirstWord();
            return;
        }

        if ((startNewGame is false) && (_session.GameQueue is not null) && (_session.GameQueue.Count > 0) && _session.State is UserState.Playing)
        {
            await GamePlayAsync();
            return;
        }
        
        async Task<bool> StartGameAsync()
        {
            //Локальная функция. Вернет труе если игра запустилась, фалсе, если не запустилась
            var words = await _repository.GetWordsAsync(_session.UserGuid, _session.DictionaryGuid, _cancellationToken);
            if (words is null || words.Count < 4)
            {
                await _botClient.SendTextMessageAsync(_session.TelegramId, "Добавьте минимум 4 слова, чтобы сыграть", cancellationToken: _cancellationToken);
                _session.State = UserState.DictSelected;
                await _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                return false;
            }

            var rand = new Random();
            words = words.OrderBy(_ => rand.Next()).ThenBy(w => w.Rating).ToList();
            _session.GameQueue = new Queue<Word>(words);
            _session.State = UserState.Playing;
            await _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
            return true;
        }
        async Task StopGameAsync()
        {
            if (answeredWordGuid != default && ((_session.GameQueue is null) || (_session.GameQueue.Count == 0)) && _session.State is UserState.Playing)
            {
                //Если слова закончились и близится конец игры
                await _botClient.SendTextMessageAsync(_session.TelegramId, "Game over!", cancellationToken: _cancellationToken);
                _session.State = UserState.DictSelected;
                await _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                return;
            }
        }
        async Task GamePlayAsync()
        {
            //Если юзер уже начал угадывать слова
            bool canPeek = _session.GameQueue.TryPeek(out Word? wordToGuess);
            if (canPeek is false || wordToGuess is null)
            {
                await StopGameAsync();
                return;
            }

            if (wordToGuess.Guid == answeredWordGuid)
            {

                await _botClient.SendTextMessageAsync(_session.TelegramId, "Вы угадали!", cancellationToken: _cancellationToken);
                await _repository.UpdateWordRating(_session.UserGuid, _session.DictionaryGuid, wordToGuess.Guid, Repository.RatingAction.Increase, _cancellationToken);
            }
            else
            {
                await _botClient.SendTextMessageAsync(_session.TelegramId, "Вы не угадали!", cancellationToken: _cancellationToken);
                await _repository.UpdateWordRating(_session.UserGuid, _session.DictionaryGuid, wordToGuess.Guid, Repository.RatingAction.Decrease, _cancellationToken);
            }
            _session.GameQueue.Dequeue();
            await _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);



            canPeek = _session.GameQueue.TryPeek(out wordToGuess);
            if (canPeek is false || wordToGuess is null)
            {
                await StopGameAsync();
                return;
            }

            var oppositeWord = await _repository.GetRandomWord(_session.UserGuid, _session.DictionaryGuid, wordToGuess.Guid, _cancellationToken);
            await _botClient.SendTextMessageAsync(_session.TelegramId, $"{wordToGuess.SourceWord} ---> ?", cancellationToken: _cancellationToken, replyMarkup: KeyboardSet.GetRandomButtonsAsWord(wordToGuess, oppositeWord));
        }
        async Task GamePlayIfFirstWord()
        {
            //Если игра только началась и надо предложить первое слово
            var wordToGuess = _session.GameQueue.Peek();
            var oppositeWord = await _repository.GetRandomWord(_session.UserGuid, _session.DictionaryGuid, wordToGuess.Guid, _cancellationToken);
            await _botClient.SendTextMessageAsync(_session.TelegramId, $"{wordToGuess.SourceWord} ---> ?", cancellationToken: _cancellationToken, replyMarkup: KeyboardSet.GetRandomButtonsAsWord(wordToGuess, oppositeWord));
        }
    }
}
