using System;
using System.Threading.Tasks;
using System.Linq;
using static Shared.Main;

namespace Shared;
public class Database
{
    public static async Task<string> GetToken(string svid, CocaBotContext db)
    {
        Users user = await db.Users.FindAsync(svid);
        return user?.Token;
    }

    public static async Task<bool> ValourName(ulong discordUserID, string valourName, CocaBotContext db)
    {
        Users user = user = db.Users.FirstOrDefault(x => x.Discord == discordUserID);
        if (user == null) return false;

        user.ValourName = valourName;
        await db.SaveChangesAsync();
        return true;
    }

    public static async Task<bool> ValourDisconnect(ulong discordUserID, CocaBotContext db)
    {
        Users user = db.Users.FirstOrDefault(x => x.Discord == discordUserID);
        if (user == null) return false;

        user.Valour = null;
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
