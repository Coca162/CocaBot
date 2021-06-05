using Newtonsoft.Json;
using Shared;

namespace Discord
{
    public struct DiscordConfig : DefaultConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("server")]
        public string Server { get; private set; }
        [JsonProperty("userid")]
        public string UserID { get; private set; }
        [JsonProperty("password")]
        public string Password { get; private set; }
        [JsonProperty("database")]
        public string Database { get; private set; }
        [JsonProperty("oauth_secret")]
        public string OauthSecret { get; private set; }
        [JsonProperty("prefix")]
        public string[] Prefix { get; private set; }
    }
}