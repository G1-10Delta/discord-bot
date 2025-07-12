using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

public class Bot
{
    private DiscordSocketClient? _client;
    private CommandService? _commands;
    private IServiceProvider? _services;

    public async Task RunAsync()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All
        });

        _commands = new CommandService();

        _client.Log += Log;
        _client.Ready += OnReady;

        await RegisterCommandsAsync();

        string token = " "; // Replace with bot token
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }

    private Task OnReady()
    {
        Console.WriteLine($"Logged in as {_client!.CurrentUser}");
        return Task.CompletedTask;
    }

    public async Task RegisterCommandsAsync()
    {
        _client!.MessageReceived += HandleCommandAsync;
        await _commands!.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        var message = arg as SocketUserMessage;
        if (message == null || message.Author.IsBot) return;

        int argPos = 0;
        if (message.HasCharPrefix('!', ref argPos))
        {
            var context = new SocketCommandContext(_client!, message);
            await _commands!.ExecuteAsync(context, argPos, _services);
        }
    }
}

public class VibeModule : ModuleBase<SocketCommandContext>
{
    [Command("pokeCaden")]
    public async Task PokeCadenAsync()
    {
        await ReplyAsync("👀 Poking Caden for a response...");

        // Wait one minute, then follow up once
        await Task.Delay(TimeSpan.FromMinutes(1));
        await ReplyAsync("Caden said you are not worth his time. Sorry...");
    }
}
