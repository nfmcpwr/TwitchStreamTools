using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Videos.GetVideos;

namespace TwitchStreamTools
{
    internal class Program
    {
        private static StreamMonitor? _Monitor;
        private static Downloader? _Downloader = null;
        private static Config? _Config = null;

        private static void Main(string[] args)
        {
            _Config = Config.Load("/data/config.json");

            if (_Config == null)
            {
                Console.WriteLine("Failed to load config file");

                File.WriteAllText("/data/config.json",
                    JsonConvert.SerializeObject(Config.DefaultConfig, Formatting.Indented));

                return;
            }

            _Monitor = new StreamMonitor(_Config.TwitchClientId!, _Config.TwitchClientSecret!);
            _Monitor.OnStreamOnline += Mon_OnStreamOnline;
            _Monitor.OnStreamOffline += Mon_OnStreamOffline;
            _Monitor.OnStreamUpdate += Mon_OnStreamUpdate;

            _Monitor.SetChannels(new List<string>(_Config.Channels!));

            _Monitor.Start();

            Console.CancelKeyPress += Console_CancelKeyPress;
            while (true)
            {
                Task.Delay(1000).Wait();
            }
        }

        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            _Monitor?.Stop();
        }

        private static void Mon_OnStreamUpdate(object? sender,
            TwitchLib.Api.Services.Events.LiveStreamMonitor.OnStreamUpdateArgs e)
        {
            Console.WriteLine(
                $"Stream Update: Channel={e.Channel}, Title={e.Stream.Title} Game={e.Stream.GameName}, ID={e.Stream.Id}");
        }

        private static async void Mon_OnStreamOffline(object? sender,
            TwitchLib.Api.Services.Events.LiveStreamMonitor.OnStreamOfflineArgs e)
        {
            Console.WriteLine(
                $"Stream Offline: Channel={e.Channel}, Title={e.Stream.Title} Game={e.Stream.GameName}, ID={e.Stream.Id}");

            if (_Config!.DownloaderOptions != null && !_Config.DownloaderOptions.LiveFromStart)
            {
                await DownloadVideo(e.Channel, _Config.DownloaderOptions);
            }
        }

        private static async void Mon_OnStreamOnline(object? sender,
            TwitchLib.Api.Services.Events.LiveStreamMonitor.OnStreamOnlineArgs e)
        {
            Console.WriteLine(
                $"Stream Online: Channel={e.Channel}, Title={e.Stream.Title}, Game={e.Stream.GameName}, ID={e.Stream.Id}");

            if (!string.IsNullOrEmpty(_Config!.WebHookUrl) && !string.IsNullOrEmpty(_Config.WebHookMessage))
            {
                await WebHook.SendMessage(_Config.WebHookUrl,
                    $"{_Config.WebHookMessage} \n https://twitch.tv/{e.Channel}");
            }

            if (_Config!.DownloaderOptions != null && _Config!.DownloaderOptions.LiveFromStart)
            {
                await DownloadVideo(e.Channel, _Config.DownloaderOptions);
            }
        }

        private static async Task DownloadVideo(string channel, DownloaderOptions options)
        {
            if (_Downloader == null)
            {
                Console.WriteLine("Request downloader");
                _Downloader = new Downloader(await Downloader.GetDownloaderPathAsync());
            }

            GetVideosResponse r = await _Monitor!.TwitchApi.Helix.Videos.GetVideosAsync(
                userId: (await _Monitor.TwitchApi.Helix.Users.GetUsersAsync(logins: new List<string>
                {
                    channel,
                })).Users[0].Id,
                first: 1);

            Console.WriteLine($"[{channel}]Download request: {r.Videos[0].Url}");

            string downloadDir = $"/Downloads/{channel}";
            if (!Directory.Exists(downloadDir))
            {
                Directory.CreateDirectory(downloadDir);
            }

            await _Downloader.RequestDownload(r.Videos[0].Url, options, channel, downloadDir);
        }
    }
}