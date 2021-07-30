﻿using static Shared.Database;
using System.Threading.Tasks;
using SpookVooper.Api.Entities;
using Shared;

namespace Discord
{
    public class DiscordTools
    {
        public static async Task<string> DiscordToSVID(ulong id, CocaBotContext db)
        {
            //if (IdSVIDs.TryGetValue(id, out string svid)) return svid;

            string DbSVID = await GetString(String.SVID, id, db);
            if (DbSVID != null)
            {
                //IdSVIDs.Add(id, DbSVID);
                return DbSVID;
            }

            string DiscordSVID = await User.GetSVIDFromDiscordAsync(id);
            
            return DiscordSVID != "u-2a0057e6-356a-4a49-b825-c37796cb7bd9" ? DiscordSVID : "";
        }

        /*
        public static async Task<string> DiscordToToken(ulong id)
        {
            if (IdTokens.TryGetValue(id, out string token)) return token;

            string DbToken = await GetToken(Platform.Discord, id);
            if (DbToken != null)
            {
                IdSVIDs.Add(id, DbToken);
                return DbToken;
            }

            return "";
        }
        */
    }
}