using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CrumbleCookie.DiscordBot.ConsoleApp
{
    [Serializable]
    internal class BotSettings
    {
        public string DiscordBotToken { get; set; }
        public List<ulong> ChannelIds { get; set; } = new();
        public const string fileName = "bot settings.json";
        public static readonly string filePath;
        static BotSettings()
        {
            filePath = $"{Environment.CurrentDirectory}\\{fileName}";
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this);
            File.WriteAllText(filePath, json);
        }

        public static BotSettings Load()
        {
            BotSettings settings = null;

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                try { settings = JsonSerializer.Deserialize<BotSettings>(json); }
                catch (Exception) { }
            }

            if (settings == null)
            {
                settings = new BotSettings();
                settings.Save();
            }

            return settings;
        }
    }
}
