using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using System.Threading;
using Telegram.Bot.Types.ReplyMarkups;
using ViTelegramBot;

string confPath = @"C:\Users\sanchous\Desktop\ViTgClinetSecrets.json";
var configuration = new ConfigurationBuilder().AddJsonFile(confPath).Build();
string hostname = configuration.GetSection("Hostname").Value!;
string token = configuration.GetSection("Token").Value!;
ChatId chatId = new();

/*var botClient = new TelegramBotClient(token);

using CancellationTokenSource cancellationTokenSource = new();
CancellationToken cancellationToken = cancellationTokenSource.Token;


InlineKeyboardMarkup inlineKeyboard = new(new[]
{
    // first row
    new []
    {
        InlineKeyboardButton.WithCallbackData(text: "1.1", callbackData: "11"),
        InlineKeyboardButton.WithCallbackData(text: "1.2", callbackData: "12"),
    },
    // second row
    new []
    {
        InlineKeyboardButton.WithCallbackData(text: "2.1", callbackData: "21"),
        InlineKeyboardButton.WithCallbackData(text: "2.2", callbackData: "22"),
    },
});

Message sentMessage = await botClient.SendTextMessageAsync(
    chatId: chatId,
    text: "A message with an inline keyboard markup",
    replyMarkup: inlineKeyboard,
    cancellationToken: cancellationToken);*/





//cancellationTokenSource.Cancel();

var vs = new ViSession(12312, "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRfdfdsterterZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImJlMTRkYTYxLWQ4N2YtNDA2Yi05ZjQwLTdkMTZmY2I4ZjM1NSIsImV4cCI6MTY5MDE0MTI5OSwiaXNzIjoiVmlUb2tlbklzc3VlciIsImF1ZCI6IlZpVXNlckF1ZGllbmNlIn0.io22fWINeCBvpGzs6CrAQSMQGYrXk9SE3P93iUhcX-Q");

var list = new ViSessionList(@"C:\Users\sanchous\Desktop\ViTgSessions.json");
list.Add(vs);
