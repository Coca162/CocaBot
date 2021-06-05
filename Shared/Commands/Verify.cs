using System.Threading.Tasks;
using static Shared.Database;

namespace Shared.Commands
{
    public class Verify
    {
        public static async Task<string> VerifyAll(Platform platform, ulong id, string key)
        {
            if (await Database.Verify(platform, key, id)) 
                return $"Successfully linked your {platform} to SpookVooper account {await GetSVID(platform, id)}";
            else return $"Failed to link {platform} and SV accounts!";
        }
    }
}