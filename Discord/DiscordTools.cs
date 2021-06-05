using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Shared.Tools;
using static Shared.Database;
using System.Threading.Tasks;
using SpookVooper.Api.Entities;
using MySql.Data.MySqlClient;

namespace Discord
{
    public class DiscordTools
    {
        public static async Task<string> DiscordToSVID(ulong id)
        {
            string DbSVID = await GetSVID(Platform.Discord, id);
            if (DbSVID != null) return DbSVID;

            string DiscordSVID = await User.GetSVIDFromDiscordAsync(id);
            if (DiscordSVID != "u-2a0057e6-356a-4a49-b825-c37796cb7bd9") return DiscordSVID;

            return "";
        }
    }
}