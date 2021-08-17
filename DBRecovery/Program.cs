using Newtonsoft.Json;
using Shared;
using System.Threading.Tasks;
using static Shared.Main;
using System.IO;
using System.Text;

namespace DBRecovery
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Config config = await GetConfig<Config>();
            await BeginCocaBot(config);

            CocaBotContext db = new();

            string json;

            await using (FileStream fs = File.OpenRead("FUCK.json"))
            using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            Users[] FUCK = JsonConvert.DeserializeObject<Users[]>(json);

            db.Users.AddRange(FUCK);
            await db.SaveChangesAsync();
        }
    }

    public struct Config : IDefaultConfig
    {
        [JsonProperty("clientsecret")]
        public string ClientSecret { get; private set; }
        [JsonProperty("clientid")]
        public string ClientId { get; private set; }
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
    }
}
