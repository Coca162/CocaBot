using static Shared.Database;
using System.Threading.Tasks;
using SpookVooper.Api.Entities;
using Valour.Net.Models;

namespace Valour
{
    public class ValourTools
    {
        public static async Task<string> ValourToSVID(ValourUser user)
        {
            string DbSVID = await GetString(Shared.Database.String.SVID, user.Id);
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