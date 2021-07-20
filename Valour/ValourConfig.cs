using Newtonsoft.Json;
using Shared;
using System.Collections.Generic;

namespace Valour
{
    public struct ValourConfig : DefaultConfig
    {
        [JsonProperty("botpassword")]
        public string BotPassword { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
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
        public List<string> Prefix { get; private set; }
    }
}