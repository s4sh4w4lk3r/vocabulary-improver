using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using ViTelegramBot.ApiRequesting;
using ViTelegramBot.ViBot;

var configuration = new ConfigurationBuilder().AddJsonFile(@"C:\Users\sanchous\Desktop\ViTgConfig.json").Build();
var botClient = new TelegramBotClient(configuration.GetRequiredSection("BotToken").Value!);

ServiceCollection services = new ServiceCollection();
services.AddHttpClient();
services.AddSingleton<IConfiguration>(configuration);
services.AddSingleton<ITelegramBotClient>(botClient);

ServiceProvider serviceProvider = services.BuildServiceProvider();


var api = new ViApiClient(serviceProvider);
var vibot = new ViBot(serviceProvider, api);




_ = Task.Run(vibot.Start);

Console.CancelKeyPress += delegate
{
    vibot.Stop();
    Environment.Exit(0);
};

while (true) { }
