namespace TwitchStreamTools
{
    internal class DownloaderOptions
    {
        public string? OutputFileFormat     { get; set; }
        public string? OutputFileName       { get; set; }
        public bool    LiveFromStart        { get; set; }
        public string? AdditionalOptions    { get; set; }
        public bool    NotifyOnDownloadTask { get; set; }

        public string Parse()
        {
            string result = "--newline";
            if (!string.IsNullOrEmpty(this.OutputFileFormat))
            {
                result += $" -t {this.OutputFileFormat}";
            }

            if (!string.IsNullOrEmpty(this.OutputFileName))
            {
                result += $" -o \"{this.OutputFileName}\"";
            }

            if (this.LiveFromStart)
            {
                result += " --live-from-start";
            }

            if (!string.IsNullOrEmpty(this.AdditionalOptions))
            {
                result += $" {this.AdditionalOptions}";
            }

            return result;
        }

        public static readonly DownloaderOptions Default = new DownloaderOptions
        {
            OutputFileFormat = Format.MP4,
            OutputFileName = "",
            LiveFromStart = false,
        };
    }
}