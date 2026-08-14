using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TwitchStreamTools
{
    internal class Downloader
    {
        private readonly string _DownloaderPath;

        public Downloader(string downloaderPath)
        {
            this._DownloaderPath = downloaderPath;
        }

        public async Task RequestDownload(string videoUrl, DownloaderOptions options, string channel, string outputPath)
        {
            ProcessStartInfo si = new ProcessStartInfo
            {
                FileName = this._DownloaderPath,
                Arguments = $"{options.Parse()} {videoUrl}",
                WorkingDirectory = outputPath,
                RedirectStandardOutput = true,
            };

            Process p = Process.Start(si)!;
            while (true)
            {
                string? s = await p.StandardOutput.ReadLineAsync();

                if (s == null)
                {
                    break;
                }

                Console.WriteLine($"[{channel}] {s}");
            }

            Console.WriteLine("Completed");
        }

        public static async Task<string> GetDownloaderPathAsync()
        {
            string binName = "yt-dlp_linux";

            foreach (string path in Directory.GetFiles(Environment.CurrentDirectory))
            {
                if (path.EndsWith(binName))
                {
                    return path;
                }
            }

            HttpResponseMessage response =
                await new HttpClient().GetAsync($"https://github.com/yt-dlp/yt-dlp/releases/latest/download/{binName}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to download yt-dlp binary");
            }

            File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, binName),
                await response.Content.ReadAsByteArrayAsync());

            return Path.Combine(Environment.CurrentDirectory, binName);
        }
    }
}