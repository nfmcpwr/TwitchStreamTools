using Newtonsoft.Json;
using System.IO;

namespace TwitchStreamTools
{
    internal class Config
    {
        public string? TwitchClientId { get; set; }
        public string? TwitchClientSecret { get; set; }
        public string? WebHookUrl { get; set; }
        public string? WebHookMessage { get; set; }
        public string[]? Channels { get; set; }
        public DownloaderOptions? DownloaderOptions { get; set; }

        public static Config? Load(string path)
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
        }

        public static readonly Config DefaultConfig = new Config
        {
            TwitchClientId = "<YOUR_CLIENT_ID>",
            TwitchClientSecret = "<YOUR_CLIENT_SECRET>",
            WebHookUrl = "<YOUR_WEBHOOK_URL>",
            WebHookMessage = "<WEBHOOK_MESSAGE>",
            Channels = new string[]
            {
                "k4sen",
            },
            DownloaderOptions = null,
        };
    }
}