using System;
using System.Collections.Generic;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace TwitchStreamTools
{
    internal class StreamMonitor
    {
        private LiveStreamMonitorService MonitorSvc;
        public readonly TwitchAPI TwitchApi;

        public event EventHandler<OnStreamOnlineArgs>? OnStreamOnline;
        public event EventHandler<OnStreamOfflineArgs>? OnStreamOffline;
        public event EventHandler<OnStreamUpdateArgs>? OnStreamUpdate;

        public StreamMonitor(string clientId, string clientSecret, int duration = 60)
        {
            this.TwitchApi = new TwitchAPI();
            this.TwitchApi.Settings.ClientId = clientId;
            this.TwitchApi.Settings.Secret = clientSecret;

            this.MonitorSvc = new LiveStreamMonitorService(this.TwitchApi, duration);
            this.MonitorSvc.OnStreamOnline += MonitorSvc_OnStreamOnline;
            this.MonitorSvc.OnStreamOffline += MonitorSvc_OnStreamOffline;
            this.MonitorSvc.OnStreamUpdate += MonitorSvc_OnStreamUpdate;
        }

        public void Start()
        {
            this.MonitorSvc.Start();
            Console.WriteLine("Monitor service start");

            Console.Write("Monitoring channels: ");
            foreach (string channel in this.MonitorSvc.ChannelsToMonitor)
            {
                Console.Write($" {channel}");
            }
            Console.WriteLine();
        }

        public void Stop()
        {
            this.MonitorSvc.Stop();
            Console.WriteLine("Monitor service stop");
        }

        public void SetChannels(List<string> channels)
        {
            this.MonitorSvc.SetChannelsByName(channels);
        }

        private void MonitorSvc_OnStreamOffline(object? sender, OnStreamOfflineArgs e)
        {
            if (this.OnStreamOffline != null)
            {
                this.OnStreamOffline(sender, e);
            }
        }

        private void MonitorSvc_OnStreamOnline(object? sender, OnStreamOnlineArgs e)
        {
            if (this.OnStreamOnline != null)
            {
                this.OnStreamOnline(sender, e);
            }
        }

        private void MonitorSvc_OnStreamUpdate(object? sender, OnStreamUpdateArgs e)
        {
            if (this.OnStreamUpdate != null)
            {
                this.OnStreamUpdate(sender, e);
            }
        }
    }
}
