using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Videos.GetVideos;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using Stream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;

namespace TwitchStreamTools
{
    internal class Program
    {
        private static StreamMonitor?               _Monitor;
        private static Downloader?                  _Downloader = null;
        private static Config?                      _Config     = null;
        private static Dictionary<string, Stream?>? _Stream     = null;

        private static void Main(string[] args)
        {
            _Config = Config.Load("/data/config.json");

            if (_Config == null)
            {
                Console.WriteLine("Failed to load config file");

                File.WriteAllText("/data/config.json",
                    JsonConvert.SerializeObject(Config.DefaultConfig, Formatting.Indented));

                Console.WriteLine("Created default config file");

                return;
            }

            if (_Config.Equals(Config.DefaultConfig))
            {
                Console.WriteLine("Loaded default config file. Please update");

                return;
            }

            _Stream = new Dictionary<string, Stream?>();

            WebHook.SetWebHookUrl(_Config.WebHookUrl);

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

        private static async void Mon_OnStreamUpdate(
            object?            sender,
            OnStreamUpdateArgs e)
        {
            Console.WriteLine(
                $"Stream update: Channel={e.Channel}, Title={e.Stream.Title} Game={e.Stream.GameName}, ID={e.Stream.Id}");

            if (e.Stream.Id != _Stream![e.Channel]!.Id)
            {
                Console.WriteLine($"Stream changed: {_Stream![e.Channel]!.Id} => {e.Stream.Id}");

                if (!string.IsNullOrEmpty(_Config!.WebHookUrl) && !string.IsNullOrEmpty(_Config.WebHookMessage))
                {
                    await WebHook.SendStreamNotification(_Config.WebHookMessage, e.Stream);
                }

                if (_Config!.DownloaderOptions != null && !_Config.DownloaderOptions.LiveFromStart)
                {
                    await DownloadVideo(_Stream![e.Channel]!, _Config.DownloaderOptions);
                }

                if (_Config!.DownloaderOptions != null && _Config.DownloaderOptions.LiveFromStart)
                {
                    await DownloadVideo(e.Stream, _Config.DownloaderOptions);
                }

                _Stream![e.Channel] = e.Stream;
            }
        }

        private static async void Mon_OnStreamOffline(
            object?             sender,
            OnStreamOfflineArgs e)
        {
            Console.WriteLine(
                $"Stream offline: Channel={e.Channel}, Title={e.Stream.Title} Game={e.Stream.GameName}, ID={e.Stream.Id}");

            _Stream![e.Channel] = null;

            if (_Config!.DownloaderOptions != null && !_Config.DownloaderOptions.LiveFromStart)
            {
                await DownloadVideo(e.Stream, _Config.DownloaderOptions);
            }
        }

        private static async void Mon_OnStreamOnline(
            object?            sender,
            OnStreamOnlineArgs e)
        {
            Console.WriteLine(
                $"Stream online: Channel={e.Channel}, Title={e.Stream.Title}, Game={e.Stream.GameName}, ID={e.Stream.Id}");

            _Stream![e.Channel] = e.Stream;

            if (!string.IsNullOrEmpty(_Config!.WebHookUrl) && !string.IsNullOrEmpty(_Config.WebHookMessage))
            {
                await WebHook.SendStreamNotification(_Config.WebHookMessage, e.Stream);
            }

            if (_Config!.DownloaderOptions != null && _Config!.DownloaderOptions.LiveFromStart)
            {
                await DownloadVideo(e.Stream, _Config.DownloaderOptions);
            }
        }

        private static async Task DownloadVideo(Stream stream, DownloaderOptions options)
        {
            if (_Downloader == null)
            {
                Console.WriteLine("Request downloader");
                _Downloader = new Downloader(await Downloader.GetDownloaderPathAsync());
            }

            GetVideosResponse r = await _Monitor!.TwitchApi.Helix.Videos.GetVideosAsync(
                new List<string>
                {
                    stream.Id,
                },
                first: 1);

            if (options.NotifyOnDownloadTask)
            {
                await WebHook.SendDownloadTaskNotification(r.Videos[0].Url, r.Videos[0].Title);
            }

            Console.WriteLine($"[{stream.UserLogin}]Download request: {r.Videos[0].Url}");

            string downloadDir = $"/Downloads/{stream.UserLogin}";
            if (!Directory.Exists(downloadDir))
            {
                Directory.CreateDirectory(downloadDir);
            }

            bool result = await _Downloader.RequestDownload(r.Videos[0].Url, options, stream.UserLogin, downloadDir);

            if (options.NotifyOnDownloadTask)
            {
                await WebHook.SendDownloadTaskCompletedNotification(r.Videos[0].Url, r.Videos[0].Title, result);
            }
        }
    }
}