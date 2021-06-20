using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static Shared.Database;

namespace Shared
{
    public class Main
    {
        public static MySqlConnection Sqlconnection { get; set; }
        //public static Dictionary<ulong, string> IdSVIDs { get; set; }
        //public static Dictionary<ulong, string> IdTokens { get; set; }

        public static readonly string[] Districts = { "new yam", "voopmont", "san vooperisco", "medievala", "old yam", "new vooperis", "isle of servers past", "server past", "servers past", "los vooperis", "queensland", "netherlands", "vooperia city", "new spudland", "landing cove", "old king", "corgi" };

        public static string OauthSecret { get; set; }

        public static async Task<T> GetConfig<T>()
        {
            string json;

            await using (FileStream fs = File.OpenRead("secret.json"))
            using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task BeginCocaBot(DefaultConfig secret, Platform platform)
        {
            OauthSecret = secret.OauthSecret;

            //IdSVIDs = new();
            //IdTokens = new();

            string cs =
                $"server={secret.Server};userid={secret.UserID};password={secret.Password};database={secret.Database}";
            Sqlconnection = new MySqlConnection(cs);
            Sqlconnection.Open();

            //await CacheDB(platform).ConfigureAwait(false);
        }

        public enum Platform
        {
            Valour,
            Discord
        }
    }
}
