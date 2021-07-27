using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Main
    {
        public static readonly CocaBotContext db = new();
        public static Platform platform;
        public static DefaultConfig config;
        //public static Dictionary<ulong, string> IdSVIDs { get; set; }
        //public static Dictionary<ulong, string> IdTokens { get; set; }

        public static readonly string[] Districts = { "new yam", "voopmont", "san vooperisco", "medievala", "old yam", "new vooperis", "isle of servers past", "server past", "servers past", "los vooperis", "queensland", "netherlands", "vooperia city", "new spudland", "landing cove", "old king", "corgi" };

        public static async Task<T> GetConfig<T>()
        {
            string json;

            await using (FileStream fs = File.OpenRead("secret.json"))
            using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task BeginCocaBot(DefaultConfig secret)
        {
            config = secret;
        }

        public enum Platform
        {
            Valour,
            Discord
        }
    }
}
