using Telegram.Bot.Types.ReplyMarkups;

namespace ViApi.Types.Telegram;

public static class KeyboardSet
{
    public static ReplyKeyboardMarkup GetDefaultKeyboard()
    {
        var buttons = new List<KeyboardButton[]>()
        {
            new KeyboardButton[] { "Выбрать словарь", "Добавить новый словарь" },
        };

        ReplyKeyboardMarkup replyKeyboardMarkup = new(buttons)
        {
            ResizeKeyboard = true
        };

        return replyKeyboardMarkup;
    }

    public static ReplyKeyboardMarkup GetDictKeyboard()
    {
        var buttons = new List<KeyboardButton[]>()
        {
            new KeyboardButton[] { "Добавить слово", "Удалить слово" },
            new KeyboardButton [] { "Переименовать словарь", "Удалить словарь", "Угадай-ка" }
        };

        ReplyKeyboardMarkup replyKeyboardMarkup = new(buttons)
        {
            ResizeKeyboard = true
        };

        return replyKeyboardMarkup;
    }
    public static ReplyKeyboardRemove RemoveKeyboard() => new ReplyKeyboardRemove();
}
