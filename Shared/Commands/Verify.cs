using System.Threading.Tasks;
using static Shared.Database;
using static Shared.Main;

namespace Shared.Commands;
public static class Verify
{
    public static async Task<string> VerifyAll(ulong id, string key, CocaBotWebContext db)
    {
        //if (await Verify(key, id, db)) 
        //    return $"Successfully linked your {platform} to SpookVooper account {await GetString(String.SVID, id, db)}";
        return $"Failed to link {platform} and SV accounts!";
    }
}
