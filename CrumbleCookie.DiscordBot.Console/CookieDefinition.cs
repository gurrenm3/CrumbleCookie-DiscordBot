using HtmlObjectBuilder;
using System.Web;

namespace CrumbleCookie.DiscordBot.ConsoleApp
{
    internal class CookieDefinition : HtmlObject
    {
        [HtmlProperty(new string[] { "text-3xl", "font-bold", "lg:text-5xl" })]
        public string Name { get; set; }

        [HtmlProperty(new string[] { "hidden", "md:block", "md:visible" })]
        public string Description { get; set; }

        public string ImageURL { get; private set; }

        public CookieDefinition(string html) : base(html)
        {
            Name = HttpUtility.HtmlDecode(Name);
            Description = HttpUtility.HtmlDecode(Description);
            ImageURL = GetImageURLFromHTML(html);
        }

        private string GetImageURLFromHTML(string html, int startIndex = 0)
        {
            const string startText = "src=\"";
            startIndex = html.IndexOf(startText, startIndex);

            // Something went wrong and it doesn't have an image. This may not be a real cookie
            if (startIndex == -1)
                return null;

            int endIndex = html.IndexOf("\"", startIndex + startText.Length);
            string url = html.Substring(startIndex, endIndex - startIndex)?.Replace(startText, "")?.Trim('\"');

            // the chocolate chip cookie is structured different. Make sure we get an actual picture.
            if (url.Contains("ajax.cloudflare.com"))
                return GetImageURLFromHTML(html, startIndex + url.Length);

            return url;
        }

        public override string ToString()
        {
            return $"{Name}: {Description}";
        }
    }
}
