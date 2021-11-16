using SpookVooper.Api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;
using static Shared.Commands.Shared;
using static Shared.Cache;

namespace Shared.Commands;
public static class Get
{
    public static async Task<string> GetAll(string input)
    {
        (bool isSVID, string name) = await TryName(input);

        if (isSVID) return GetMessage(name, input);

        var (exact, nonExact) = await GetSVIDs(input);

        if (!exact.Any()) return NoExactsMessage(input, nonExact.Select(x => x.name));
        else if (exact.Count == 1) return GetMessage(input, exact.First());

        string msg = "";

        foreach (string svid in exact)
        {
            msg += GetMessage(input, svid) + "\n";
        }
        return msg[0..^1];
    }

    public static async Task<string> GetSVID(string svid) => GetMessage(await GetName(svid), svid);
    private static string GetMessage(string name, string svid) => 
        $"{SVIDTypeToString(SVIDToType(svid))}:\nSVID - {svid}\nName - {name}";
}
