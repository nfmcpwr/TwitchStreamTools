using System;

namespace TwitchStreamTools
{
    internal class DownloaderOptions
    {
        public string? OutputFileFormat { get; set; }
        public string? OutputFileName { get; set; }
        public string? OutputDir { get; set; }
        public bool LiveFromStart { get; set; }
        public string? AdditionalOptions { get; set; }

        public string Parse()
        {
            string result = "--newline";
            if (!string.IsNullOrEmpty(OutputFileFormat))
            {
                result += $" -t {OutputFileFormat}";
            }

            if (!string.IsNullOrEmpty(OutputFileName))
            {
                result += $" -o \"{OutputFileName}\"";
            }

            if (LiveFromStart)
            {
                result += " --live-from-start";
            }

            if (!string.IsNullOrEmpty(AdditionalOptions))
            {
                result += $" {AdditionalOptions}";
            }

            return result;
        }

        public static readonly DownloaderOptions Default = new DownloaderOptions()
        {
            OutputFileFormat = Format.MP4,
            OutputFileName = "",
            LiveFromStart = false,
        };
    }
}
