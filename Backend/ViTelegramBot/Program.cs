using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types;
using ViTelegramBot.ApiRequesting;

string confPath = @"C:\Users\sanchous\Desktop\ViTgClinetSecrets.json";
string sessionsPath = @"C:\Users\sanchous\Desktop\ViTgSessions.json";

var configuration = new ConfigurationBuilder().AddJsonFile(confPath).Build();
string hostname = configuration.GetSection("Hostname").Value!;
string token = configuration.GetSection("Token").Value!;
long chatId64 = long.Parse(configuration.GetSection("ChatId").Value!);

ChatId chatId = new(chatId64);

var api = new ViApiClient(hostname, sessionsPath);

var a = await api.EditRatingWord(chatId64, Guid.Parse("5b469f2d-e257-417b-85f1-2c1204cc3605"), RatingAction.Decrease);
var ab = await api.EditRatingWord(chatId64, Guid.Parse("5b469f2d-e257-417b-85f1-2c1204cc3605"), RatingAction.Decrease);
var ad = await api.EditRatingWord(chatId64, Guid.Parse("5b469f2d-e257-417b-85f1-2c1204cc3605"), RatingAction.Decrease);
#error добавить метод на удаление слова из апи и можно уже бота пилить.
Console.WriteLine();
