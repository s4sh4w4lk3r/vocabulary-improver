using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types;
using ViTelegramBot;

string confPath = @"C:\Users\sanchous\Desktop\ViTgClinetSecrets.json";
string sessionsPath = @"C:\Users\sanchous\Desktop\ViTgSessions.json";

var configuration = new ConfigurationBuilder().AddJsonFile(confPath).Build();
string hostname = configuration.GetSection("Hostname").Value!;
string token = configuration.GetSection("Token").Value!;
long chatId64 = long.Parse(configuration.GetSection("ChatId").Value!);

ChatId chatId = new(chatId64);

var api = new ViApiClient(hostname, sessionsPath);

var a = await api.AddDictionary(chatId64, "Мой слоdварик");
Console.WriteLine();

#error сделать добавление и изменение имени словаря не через url строку, а через json, чтобы можно любоые названия юзать.