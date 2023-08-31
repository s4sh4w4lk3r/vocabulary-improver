using Telegram.Bot.Types.ReplyMarkups;
using ViApi.Types.Common;

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
    public static InlineKeyboardMarkup GetDictKeyboard(Guid dictGuid)
    {
        var row1 = new List<InlineKeyboardButton>()
        {
            InlineKeyboardButton.WithCallbackData("🆕Добавить слово", $"addword:{dictGuid}"),
            InlineKeyboardButton.WithCallbackData("✖️Удалить слово", $"deleteword:{dictGuid}")
        };
        var row2 = new List<InlineKeyboardButton>()
        {
            InlineKeyboardButton.WithCallbackData("✏️Переименовать словарь", $"renamedict:{dictGuid}"),
            InlineKeyboardButton.WithCallbackData("❌Удалить словарь", $"deletedict:{dictGuid}")

        };
        var row3 = new List<InlineKeyboardButton>()
        {
            InlineKeyboardButton.WithCallbackData("🔙Назад", $"backtodictlist"),
            InlineKeyboardButton.WithCallbackData("🎮Угадай-ка", $"play:{dictGuid}")
        };


        var buttonList = new List<IEnumerable<InlineKeyboardButton>>()
        {
            row1, row2, row3
        };

        return new InlineKeyboardMarkup(buttonList);
    }
    public static InlineKeyboardMarkup GetDictListAsKeyboardButtons(IEnumerable<Dictionary> dictionaries)
    {
        var dictqueue = new Queue<Dictionary>(dictionaries);
        int dictCount = dictionaries.Count();
        int rows;
        bool IsCountEven = dictCount % 2 == 0;
        var buttonList = new List<IEnumerable<InlineKeyboardButton>>();

        if (IsCountEven)
        {
            rows = dictCount / 2;
        }
        else
        {
            rows = (dictCount / 2) + 1;
        }

        for (int i = 0; i < rows; i++)
        {
            var buttonPair = new List<InlineKeyboardButton>();

            bool dict1Dequeued = dictqueue.TryDequeue(out Dictionary? dict1);
            bool dict2Dequeued = dictqueue.TryDequeue(out Dictionary? dict2);

            if ((dict1Dequeued is true && dict2Dequeued is true) && ((dict1 is not null) && (dict2 is not null)))
            {
                buttonPair.Add(InlineKeyboardButton.WithCallbackData($"{dict1.Name}", $"{dict1.Guid}"));
                buttonPair.Add(InlineKeyboardButton.WithCallbackData($"{dict2.Name}", $"{dict2.Guid}"));
                buttonList.Add(buttonPair);
            }
            if ((dict1Dequeued is true && dict2Dequeued is false) && ((dict1 is not null) && (dict2 is null)))
            {
                buttonPair.Add(InlineKeyboardButton.WithCallbackData($"{dict1.Name}", $"{dict1.Guid}"));
                buttonList.Add(buttonPair);
            }
            else if ((dict1Dequeued is false && dict2Dequeued is false) && ((dict1 is null) && (dict2 is null)))
            {
                break;
            }
        }

        return new InlineKeyboardMarkup(buttonList);
    }
}
