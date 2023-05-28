using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using RobsDiscordBot.Modules;
using Serilog;

Console.Write("Discord Token: ");
var tokenFromUserInput = Console.ReadLine();
if(string.IsNullOrEmpty(tokenFromUserInput))
{
    Console.WriteLine("The input you provided was null or empty");
    return;
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .CreateLogger();

var loggerFactory = new LoggerFactory().AddSerilog();

var discord = new DiscordClient(new DiscordConfiguration()
{
    Token = tokenFromUserInput,
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
    MinimumLogLevel = LogLevel.Debug,
    LoggerFactory = loggerFactory,
});

var slash = discord.UseSlashCommands();


slash.RegisterCommands<SlashCommands>(1103217850040188948);


await discord.ConnectAsync();
Log.Information("Bot is ready!");

await Task.Delay(-1);
