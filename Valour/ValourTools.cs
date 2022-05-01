using System.Threading.Tasks;
using Shared;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using User = Shared.Models.User;

namespace Valour;
public static class ValourTools
{
    public static async Task<string> ValourToSVID(ulong id, CocaBotContext db) => 
        await db.Users
                .Where(x => x.Valour == id) // Same as: WHERE Discord = id
                .Select(x => x.SVID) // Same as: SELECT SVID
                .SingleOrDefaultAsync(); // Where it executes
}

