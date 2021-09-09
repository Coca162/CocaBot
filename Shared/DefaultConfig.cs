using Newtonsoft.Json;

namespace Shared;
public interface IDefaultConfig
{
    [JsonProperty("server")]
    public string Server { get; }
    [JsonProperty("userid")]
    public string UserID { get; }
    [JsonProperty("password")]
    public string Password { get; }
    [JsonProperty("database")]
    public string Database { get; }
    [JsonProperty("oauth_secret")]
    public string OauthSecret { get; }
}
