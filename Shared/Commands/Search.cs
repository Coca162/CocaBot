using SpookVooper.Api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;
using static Shared.Commands.Shared;
using static Shared.Cache;

namespace Shared.Commands;
public static class Search
{
    public static async Task<string> SearchMessage(string input)
    {
        var (exact, nonExact) = await SearchName(input);

        if (!exact.Any() && !nonExact.Any()) return "No name is similar to this!";

        exact.AddRange(nonExact);

        string msg = "";
        foreach (SearchReturn entity in exact.OrderBy(x => StringDifference(input, x.Name)))
        {
            msg += $"{entity.Name} ({entity.SVID})\n";
        }
        return msg[0..^1];
    }
}
