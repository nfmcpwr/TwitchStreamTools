using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;

namespace TwitchStreamTools
{
    internal class WebHook
    {
        private static string? _WebHookUrl = null;

        private static async Task SendMessage(string messageJson)
        {
            if (_WebHookUrl == null)
            {
                return;
            }

            HttpResponseMessage r = await new HttpClient().PostAsync(_WebHookUrl,
                new StringContent(messageJson, MediaTypeHeaderValue.Parse("application/json")));

            if (!r.IsSuccessStatusCode)
            {
                Console.WriteLine($"Status code: {r.StatusCode}");
                Console.WriteLine(await r.Content.ReadAsStringAsync());
            }
        }

        public static void SetWebHookUrl(string? webhookUrl)
        {
            _WebHookUrl = webhookUrl;
        }

        public static async Task SendStreamNotification(string? message, Stream stream)
        {
            await SendMessage(new WebHookContent
            {
                Content = message == null ? "" : message,
                Embeds = new Embed[]
                {
                    new Embed
                    {
                        Title = $"twitch.tv/{stream.UserLogin}",
                        Description = $"### [{stream.Title}](https://twitch.tv/{stream.UserLogin})",
                        Url = $"https://twitch.tv/{stream.UserLogin}",
                        Footer = new Footer
                        {
                            Text = "Live started at",
                        },
                        Thumbnail = new Thumbnail
                        {
                            Url = stream.ThumbnailUrl,
                        },
                        Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    },
                },
            }.GetWebHookJson());
        }

        public static async Task SendDownloadTaskNotification(string videoUrl, string title)
        {
            await SendMessage(new WebHookContent
            {
                Embeds = new Embed[]
                {
                    new Embed
                    {
                        Title = videoUrl,
                        Description = $"### [{title}]({videoUrl})",
                        Url = videoUrl,
                        Footer = new Footer
                        {
                            Text = "Task started at",
                        },
                        Author = new Author
                        {
                            Name = "Download task started",
                        },
                        Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    },
                },
            }.GetWebHookJson());
        }

        public static async Task SendDownloadTaskCompletedNotification(
            string videoUrl,
            string title,
            bool   error = false)
        {
            await SendMessage(new WebHookContent
            {
                Embeds = new Embed[]
                {
                    new Embed
                    {
                        Title = videoUrl,
                        Description = $"### [{title}]({videoUrl})",
                        Url = videoUrl,
                        Footer = new Footer
                        {
                            Text = error ? "Task failed at" : "Task completed at",
                        },
                        Author = new Author
                        {
                            Name = error ? "Download task failed" : "Download task completed successfully",
                        },
                        Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    },
                },
            }.GetWebHookJson());
        }
    }
}