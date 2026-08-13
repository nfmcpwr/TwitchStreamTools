using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TwitchStreamTools
{
    internal class WebHook
    {
        public static async Task SendMessage(string url, string message)
        {
            HttpResponseMessage r = await new HttpClient().PostAsync(url, new StringContent(JsonConvert.SerializeObject(
                new WebHookContent
                {
                    Content = message,
                }), MediaTypeHeaderValue.Parse("application/json")));

            if (!r.IsSuccessStatusCode)
            {
                Console.WriteLine($"Status code: {r.StatusCode}");
                Console.WriteLine(await r.Content.ReadAsStringAsync());
            }
        }
    }

    internal class WebHookContent
    {
        [JsonProperty("content")]
        public string? Content { get; set; }
    }
}