using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using static Shared.Main;

namespace Shared
{
    public class Database
    {
        public static async Task<bool> CheckString(String type, string request)
        {
            string check = $"SELECT EXISTS(SELECT * FROM Tokens WHERE {type} = @id);";
            MySqlCommand cmdCheck = new(check, Sqlconnection);
            cmdCheck.Parameters.AddWithValue("@id", request);
            MySqlDataReader reader = cmdCheck.ExecuteReader();
            reader.Read();
            bool existence = reader.GetBoolean(0);
            reader.Close();

            return existence;
        }

        public static async Task<string> GetSVID(Platform platform, ulong id)
        {
            if (await CheckID(platform, id) == false) return null;

            string get = $"SELECT SVID FROM Tokens WHERE {platform} = @id;";
            MySqlCommand cmdGet = new(get, Sqlconnection);
            cmdGet.Parameters.AddWithValue("@id", id);
            MySqlDataReader getReader = cmdGet.ExecuteReader();
            getReader.Read();
            string svid = getReader.GetString(0);
            getReader.Close();

            return svid;
        }

        public static async Task<string> GetToken(Platform platform, ulong id)
        {
            if (await CheckID(platform, id) == false) return null;

            string get = $"SELECT Token FROM Tokens WHERE {platform} = @id;";
            MySqlCommand cmdGet = new(get, Sqlconnection);
            cmdGet.Parameters.AddWithValue("@id", id);
            MySqlDataReader getReader = cmdGet.ExecuteReader();
            getReader.Read();
            string token = getReader.GetString(0);
            getReader.Close();

            return token;
        }

        public static async Task<bool> CheckID(Platform platform, ulong id)
        {
            string check = $"SELECT EXISTS(SELECT * FROM Tokens WHERE {platform} = @id);";
            MySqlCommand cmdCheck = new(check, Sqlconnection);
            cmdCheck.Parameters.AddWithValue("@id", id);
            MySqlDataReader reader = cmdCheck.ExecuteReader();
            reader.Read();
            bool existence = reader.GetBoolean(0);
            reader.Close();

            return existence;
        }

        public static async Task<bool> Verify(Platform platform, string key, ulong id)
        {
            if (await CheckString(String.VerifKey, key) == false) return false;

            if (await CheckID(platform, id) == true)
            {
                string clean = $"UPDATE people SET {platform} = NULL WHERE {platform} = @id;";
                MySqlCommand cmdClean = new(clean, Sqlconnection);
                Console.WriteLine($"{cmdClean.ExecuteNonQuery()} rows where affected in the database!");
            }

            string verify = $"UPDATE Tokens SET {platform} = @id, VerifKey = NULL WHERE VerifKey = @key;";
            MySqlCommand cmdVerify = new(verify, Sqlconnection);
            cmdVerify.Parameters.AddWithValue("@id", id);
            cmdVerify.Parameters.AddWithValue("@key", key);
            Console.WriteLine($"{cmdVerify.ExecuteNonQuery()} rows where affected in the database!");

            return true;
        }

        public enum Platform
        {
            Valour,
            Discord
        }

        public enum String
        {
            SVID,
            Token,
            VerifKey
        }
    }
}
