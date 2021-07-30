using System;
using System.Threading.Tasks;
using System.Linq;
using static Shared.Main;

namespace Shared
{
    public class Database
    {
        public static async Task<Tokens> GetUser(ulong id, CocaBotContext db)
        {
            Tokens user;
            if (platform == Platform.Discord)
            {
                user = db.Tokens.Where(x => x.Discord == id).FirstOrDefault();
            }
            else
            {
                user = db.Tokens.Where(x => x.Valour == id).FirstOrDefault();
            }

            return user;
        }

        public static async Task<bool> CheckString(String type, string request, CocaBotContext db)
        {
            return type switch
            {
                String.SVID => db.Tokens.Any(x => x.SVID == request),
                String.Token => db.Tokens.Any(x => x.Token == request),
                String.ValourName => db.Tokens.Any(x => x.ValourName == request),
                String.VerifKey => db.Tokens.Any(x => x.VerifKey == request),
                _ => throw new ArgumentException("This string is not valid"),
            };
        }

        public static async Task<string> GetString(String type, ulong id, CocaBotContext db)
        {
            Tokens user = await GetUser(id, db);
            if (user == null) return null;

            return type switch
            {
                String.SVID => user.SVID,
                String.Token => user.Token,
                String.ValourName => user.ValourName,
                String.VerifKey => user.VerifKey,
                _ => throw new ArgumentException("This string is not valid"),
            };
        }

        public static async Task<string> GetToken(string svid, CocaBotContext db)
        {
            Tokens user = await db.Tokens.FindAsync(svid);
            if (user == null) return null;
            return user.Token;
        }

        public static async Task<bool> CheckID(ulong id, CocaBotContext db)
        {
            if (platform == Platform.Discord)
            {
                return db.Tokens.Any(x => x.Discord == id);
            }
            return db.Tokens.Any(x => x.Valour == id);
        }

        public static async Task<bool> Verify(string key, ulong id, CocaBotContext db)
        {
            Tokens user = db.Tokens.Where(x => x.VerifKey == key).FirstOrDefault();
            if (user == null) return false;
            user.Discord = id;
            user.VerifKey = null;
            await db.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> ValourName(ulong discordUserID, string valourName, CocaBotContext db)
        {
            Tokens user = db.Tokens.Where(x => x.Discord == discordUserID).FirstOrDefault();
            if (user == null) return false;

            user.ValourName = valourName;
            await db.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> ValourDisconnect(ulong discordUserID, CocaBotContext db)
        {
            Tokens user = db.Tokens.Where(x => x.Discord == discordUserID).FirstOrDefault();
            if (user == null) return false;

            user.Valour = null;
            user.ValourName = null;
            await db.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> ValourConnect(string name, ulong id, CocaBotContext db)
        {
            Tokens user = db.Tokens.Where(x => x.ValourName == name).FirstOrDefault();
            if (user == null) return false;

            user.Valour = id;
            user.ValourName = null;
            await db.SaveChangesAsync();
            return true;
        }

        public enum String
        {
            SVID,
            Token,
            VerifKey,
            ValourName
        }
    }
}
