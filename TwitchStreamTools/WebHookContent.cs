using Newtonsoft.Json;

namespace TwitchStreamTools
{
    internal class WebHookContent
    {
        [JsonProperty("content")]
        public string? Content { get; set; }

        [JsonProperty("embeds")]
        public Embed[]? Embeds { get; set; }

        public string GetWebHookJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    internal class Embed
    {
        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("footer")]
        public Footer? Footer { get; set; }

        [JsonProperty("author")]
        public Author? Author { get; set; }

        [JsonProperty("thumbnail")]
        public Thumbnail? Thumbnail { get; set; }

        [JsonProperty("timestamp")]
        public string? Timestamp { get; set; }
    }

    internal class Footer
    {
        [JsonProperty("text")]
        public string? Text { get; set; }
    }

    internal class Author
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
    }

    internal class Thumbnail
    {
        [JsonProperty("url")]
        public string? Url { get; set; }
    }
}