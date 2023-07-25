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

var a = await api.UpdateDictName(chatId64, Guid.NewGuid(), "пиздак");
Console.WriteLine();
