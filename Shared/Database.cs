using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using static Shared.Main;

namespace Shared
{
    public class Database
    {
        /*
        public static async Task CacheDB(Platform platform)
        {
            string get = $"SELECT {platform}, SVID, Token FROM Tokens WHERE {platform} IS NOT NULL;";
            MySqlCommand cmdGet = new(get, Sqlconnection);
            MySqlDataReader getReader = cmdGet.ExecuteReader();
            while (getReader.Read())
            {
                ulong id = getReader.GetUInt64(0);
                IdSVIDs.Add(id, getReader.GetString(1));
                IdTokens.Add(id, getReader.GetString(2));
            }
        }
        */

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
                string clean = $"UPDATE Tokens SET {platform} = NULL WHERE {platform} = @id;";
                MySqlCommand cmdClean = new(clean, Sqlconnection);
                Console.WriteLine($"{cmdClean.ExecuteNonQuery()} rows where affected in the database!");
            }

            string verify = $"UPDATE Tokens SET {platform} = @id, VerifKey = NULL WHERE VerifKey = @key;";
            MySqlCommand cmdVerify = new(verify, Sqlconnection);
            cmdVerify.Parameters.AddWithValue("@id", id);
            cmdVerify.Parameters.AddWithValue("@key", key);
            Console.WriteLine($"{cmdVerify.ExecuteNonQuery()} rows where affected in the database!");

            //await UpdateAllCache(platform, id).ConfigureAwait(false);

            return true;
        }

        public static async Task<bool> ValourName(ulong discordUserID, string valourName)
        {
            if (await CheckID(Platform.Discord, discordUserID) != true)
            {
                return false;
            }

            string verify = $"UPDATE Tokens SET {String.ValourName} = @valourName WHERE {Platform.Discord} = @discordID;";
            MySqlCommand cmdVerify = new(verify, Sqlconnection);
            cmdVerify.Parameters.AddWithValue("@valourName", valourName);
            cmdVerify.Parameters.AddWithValue("@discordID", discordUserID);
            Console.WriteLine($"{cmdVerify.ExecuteNonQuery()} rows where affected in the database!");

            //await UpdateAllCache(platform, id).ConfigureAwait(false);

            return true;
        }

        public static async Task<bool> ValourDisconnect(ulong discordUserID)
        {
            if (await CheckID(Platform.Discord, discordUserID) == true)
            {
                string clean = $"UPDATE Tokens SET {String.ValourName} = NULL, Valour = NULL WHERE {Platform.Discord} = @id;";
                MySqlCommand cmdClean = new(clean, Sqlconnection);
                cmdClean.Parameters.AddWithValue("@id", discordUserID);
                Console.WriteLine($"{cmdClean.ExecuteNonQuery()} rows where affected in the database!");
                return true;
            }
            return false;
        }

        public static async Task<bool> ValourConnect(string name, ulong id)
        {
            if (await CheckString(String.ValourName, name) != true) return false;

            if (await CheckID(Platform.Valour, id) == true)
            {
                string clean = $"UPDATE Tokens SET {Platform.Valour} = NULL WHERE {Platform.Valour} = @id;";
                MySqlCommand cmdClean = new(clean, Sqlconnection);
                Console.WriteLine($"{cmdClean.ExecuteNonQuery()} rows where affected in the database!");
            }

            string verify = $"UPDATE Tokens SET {Platform.Valour} = @id WHERE {String.ValourName} = @name;";
            MySqlCommand cmdVerify = new(verify, Sqlconnection);
            cmdVerify.Parameters.AddWithValue("@id", id);
            cmdVerify.Parameters.AddWithValue("@name", name);
            Console.WriteLine($"{cmdVerify.ExecuteNonQuery()} rows where affected in the database!");

            //await UpdateAllCache(platform, id).ConfigureAwait(false);

            return true;
        }

        /*
        public static async Task UpdateAllCache(Platform platform, ulong id)
        {
            string get = $"SELECT SVID, Token FROM Tokens WHERE {platform} = @id;";
            MySqlCommand cmdGet = new(get, Sqlconnection);
            cmdGet.Parameters.AddWithValue("@id", id);
            MySqlDataReader getReader = cmdGet.ExecuteReader();
            IdSVIDs.Add(id, getReader.GetString(0));
            IdTokens.Add(id, getReader.GetString(1));
        }
        
        public static async Task UpdateIdSVIDCache(Platform platform, ulong id)
        {
            string get = $"SELECT SVID FROM Tokens WHERE {platform} = @id;";
            MySqlCommand cmdGet = new(get, Sqlconnection);
            cmdGet.Parameters.AddWithValue("@id", id);
            MySqlDataReader getReader = cmdGet.ExecuteReader();
            string svid = getReader.GetString(0);
            IdSVIDs.Add(id, svid);
        }

        public static async Task UpdateIdTokenCache(Platform platform, ulong id)
        {
            string get = $"SELECT Token FROM Tokens WHERE {platform} = @id;";
            MySqlCommand cmdGet = new(get, Sqlconnection);
            cmdGet.Parameters.AddWithValue("@id", id);
            MySqlDataReader getReader = cmdGet.ExecuteReader();
            IdTokens.Add(id, getReader.GetString(0));
        }
        */
        public enum String
        {
            SVID,
            Token,
            VerifKey,
            ValourName
        }
    }
}
