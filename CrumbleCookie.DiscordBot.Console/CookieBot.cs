using Discord.WebSocket;

namespace CrumbleCookie.DiscordBot.ConsoleApp;

internal class CookieBot : IDisposable
{
    public readonly DiscordSocketClient client;
    private string botToken;

    public CookieBot(string botToken)
    {
        this.botToken = botToken;
        client = new DiscordSocketClient();
    }

    public void Dispose()
    {
        client.Dispose();
    }

    public async Task StartAsync()
    {
        await client.LoginAsync(Discord.TokenType.Bot, botToken);
        await client.StartAsync();
    }
}
