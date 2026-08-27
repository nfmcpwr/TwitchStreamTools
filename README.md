# TwitchStreamTools

A Docker container that monitors Twitch channels, sends webhook notifications when a stream goes live, and optionally downloads live streams or VODs using `yt-dlp`.

## Features

- Monitor one or more Twitch channels
- Detect stream online, offline, and update events
- Send webhook notifications when a stream starts
- Automatically download Twitch videos with `yt-dlp`
- Run in Docker with mounted config and download volumes

## Requirements

- Twitch API client ID and client secret
- Docker environment
- Optional: Discord webhook URL and message

## Configuration

The application reads its configuration from: `/data/config.json`

If the file does not exist or fails to load, the app will create a default config file.

### Example config

```json
{
  "TwitchClientId": "<YOUR_CLIENT_ID>",
  "TwitchClientSecret": "<YOUR_CLIENT_SECRET>",
  "WebHookUrl": "<YOUR_WEBHOOK_URL>",
  "WebHookMessage": "<WEBHOOK_MESSAGE>",
  "Channels": [
    "k4sen"
  ],
  "DownloaderOptions": {
    "OutputFileFormat": "mp4",
    "OutputFileName": "",
    "LiveFromStart": false,
    "AdditionalOptions": null,
    "NotifyOnDownloadTask": false
  }
}
```

### Downloader options

`DownloaderOptions` is passed to `yt-dlp` and supports:

- `OutputFileFormat` - output format such as `mp4`, `mp3`, `mkv`, `aac` (same as `-t` option in `yt-dlp`)
- `OutputFileName` - output filename template (same as `-o` option in `yt-dlp`)
- `LiveFromStart` - start downloading from the beginning of a live stream
- `AdditionalOptions` - extra arguments passed directly to yt-dlp
- `NotifyOnDownloadTask` - send download status to webhook URL

If `DownloaderOptions` is `null`, video download feature is disabled.

## Installation

### Download Docker image

Docker images are available on `ghcr.io`

```bash
docker pull ghcr.io/nfmcpwr/twitchstreamtools:latest
```

### Build manually

```bash
docker build -f ./TwitchStreamTools/Dockerfile -t twitchstreamtools:latest .
```

#### Run

```bash
docker run --rm \
  -v $(pwd)/data:/data \
  -v $(pwd)/Downloads:/Downloads \
  twitchstreamtools
```

## Volumes

The container expects:

- `/data` — config file location
- `/Downloads` — download output directory

## Notes

- The app downloads the `yt-dlp` binary automatically if it is not already present in the working directory.
- If no valid config is found, a default config file will be written and the app will exit so you can update it.

## License

`TwitchStreamTools` is licenced under the [MIT License](https://github.com/nfmcpwr/TwitchStreamTools/blob/master/LICENSE)
