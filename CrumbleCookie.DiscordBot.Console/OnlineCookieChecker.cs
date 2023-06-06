namespace CrumbleCookie.DiscordBot.ConsoleApp;

internal class OnlineCookieChecker
{
    public const string url = "https://crumblcookies.com/";
    private readonly bool useDebug;

    public OnlineCookieChecker(bool debug)
    {
        useDebug = debug;
    }

    public async Task<List<CookieDefinition>> GetCookies()
    {
        var cookiesHtml = !useDebug ? await GetCookiesHTML() : GetCookiesHtmlFromFile();
        var cookieContainer = cookiesHtml.Split("<li");

        var cookies = new List<CookieDefinition>();
        Array.ForEach(cookieContainer, cookieHtml => cookies.Add(new(cookieHtml)));
        return cookies.Where(cookie => !string.IsNullOrEmpty(cookie.Name) && !string.IsNullOrEmpty(cookie.Description)).ToList();
    }

    private async Task<string> GetCookiesHTML()
    {
        const string url = "https://crumblcookies.com/";
        string html = await new HttpClient().GetStringAsync(url);
        return ParseHTML(html);
    }

    string GetCookiesHtmlFromFile()
    {
        const string path = "D:\\Repos\\C#\\CrumbleCookie.DiscordBot\\CrumbleCookie.DiscordBot.Console\\Html.txt";
        string html = File.ReadAllText(path);
        return ParseHTML(html);
    }

    string ParseHTML(string html)
    {
        const string startText = "<ul id=\"weekly-cookie-flavors\">";
        const string endText = "</ul>";

        int startIndex = html.IndexOf(startText);
        int endIndex = html.IndexOf(endText, startIndex);

        string cookiesContainer = html.Substring(startIndex + startText.Length, (endIndex) - startIndex);
        return cookiesContainer;
    }
}
