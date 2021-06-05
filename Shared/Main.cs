using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Main
    {
        public static MySqlConnection Sqlconnection { get; set; }

        public static string OauthSecret; 

        public static async Task<T> GetConfig<T>()
        {
            string json;

            using (FileStream fs = File.OpenRead("secret.json"))
            using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task BeginMysql(DefaultConfig secret)
        {
            OauthSecret = secret.OauthSecret;

            string cs =
                $"server={secret.Server};userid={secret.UserID};password={secret.Password};database={secret.Database}";
            Sqlconnection = new MySqlConnection(cs);
            Sqlconnection.Open();
        }
    }
}
