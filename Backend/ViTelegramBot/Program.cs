using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Telegram.Bot.Types;
using ViTelegramBot.ApiRequesting;
using ViTelegramBot.ViBot;

ServiceCollection services = new ServiceCollection();
services.AddHttpClient();
ServiceProvider serviceProvider = services.BuildServiceProvider();


string confPath = @"C:\Users\sanchous\Desktop\ViTgClinetSecrets.json";
string sessionsPath = @"C:\Users\sanchous\Desktop\ViTgSessions.json";

var configuration = new ConfigurationBuilder().AddJsonFile(confPath).Build();
string apiHostname = configuration.GetSection("ApiHostname").Value!;
string botToken = configuration.GetSection("BotToken").Value!;
string ngrokApiToken = configuration.GetSection("ngrokToken").Value!;
long chatId64 = long.Parse(configuration.GetSection("ChatId").Value!);

ChatId chatId = new(chatId64);

/*var api = new ViApiClient(hostname, sessionsPath, serviceProvider);*/

var wh = new ConfigureWebhook(serviceProvider, ngrokApiToken, botToken, "test123");
await wh.StartAsync();
Thread.Sleep(10000);
await wh.StopAsync();

