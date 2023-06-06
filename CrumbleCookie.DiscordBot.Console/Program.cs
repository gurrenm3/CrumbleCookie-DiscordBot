using CrumbleCookie.DiscordBot.ConsoleApp;
using Discord;
using Discord.WebSocket;
using System.Text;

if (DateTime.Now.DayOfWeek != DayOfWeek.Monday)
{
    Console.WriteLine("It is not Monday so there is no need to check for latest cookies.");
    Console.WriteLine("Closing program...");
    Task.Run(async () => await Task.Delay(3000)).Wait();
    return;
}

var settings = BotSettings.Load();
if (string.IsNullOrEmpty(settings.DiscordBotToken))
{
    Console.WriteLine("You need to enter your discord bot token in the BotSettings for this to work.");
    Console.WriteLine($"Add it to the file: \"{BotSettings.filePath}\"");
    return;
}

if (!settings.ChannelIds.Any())
{
    Console.WriteLine("You need to enter the channel IDs you wish to post the notification in.");
    Console.WriteLine($"Add them to the file: \"{BotSettings.filePath}\"");

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\nNotice: Make sure this channel doesn't get used for anything else! All messages will be deleted each time there are new cookies.");
    Console.ForegroundColor = ConsoleColor.White;
    return;
}


using var bot = new CookieBot(settings.DiscordBotToken);
bot.StartAsync().Wait();

bot.client.Ready += Client_Ready;

async Task Client_Ready()
{
    List<CookieDefinition> cookies = null;
    settings.ChannelIds.ForEach(channel => cookies = SendCookieNotification(channel).Result);

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("New cookies have dropped!!!\n");
    Console.ForegroundColor = ConsoleColor.White;

    cookies?.ForEach(cookie => Console.WriteLine(cookie));

    // exit program once we've notified everyone of the new cookies.
    Environment.Exit(0);
}

async Task<List<CookieDefinition>> SendCookieNotification(ulong channelId)
{
    SocketTextChannel channel = bot.client.GetChannel(channelId) as SocketTextChannel;
    if (channel == null)
    {
        Console.WriteLine("Channel not found!");
        return null;
    }

    // delete previous messages.
    var channelMessages = await channel.GetMessagesAsync().ToListAsync();
    foreach (var channelMsg in channelMessages)
    {
        foreach (var msg in channelMsg)
        {
            await channel.DeleteMessageAsync(msg);
        }
    }

    await channel.SendMessageAsync("@everyone New cookies have dropped!!!\n");

    var cookies = await new OnlineCookieChecker(debug: false).GetCookies();
    var cookieImages = new List<Embed>();

    cookies.ForEach(cookie =>
    {
        // Add cookie to message as an embedded image.
        cookieImages.Add(new EmbedBuilder
        {
            Title = cookie.Name,
            Description = cookie.Description,
            ImageUrl = cookie.ImageURL
        }.Build());
    });

    await channel.SendMessageAsync(embeds: cookieImages.ToArray());
    return cookies;
}

Task.Run(async () => await Task.Delay(-1)).Wait();