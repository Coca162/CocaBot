using static Shared.Database;
using System.Threading.Tasks;
using SpookVooper.Api.Entities;
using Valour.Net.Models;
using Shared;

namespace Valour
{
    public class ValourTools
    {
        public static async Task<string> ValourToSVID(ValourUser user, CocaBotContext db)
        {
            string DbSVID = await GetString(String.SVID, user.Id, db);
            if (DbSVID != null) return DbSVID;

            return await NameToSVID(user.Username);
        }

        public static async Task<string> NameToSVID(string name)
        {
            string svid = await User.GetSVIDFromUsernameAsync(name);

            return svid != $"Could not find user {name}" ? svid : null;
        }
    }
}