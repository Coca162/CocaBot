using SpookVooper.Api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;
using static Shared.Commands.Shared;

namespace Shared.Commands;
public static class Get
{
    public static async Task<string> GetAll(string input)
    {
        bool isSVID = TrySVID(input, out SVIDTypes type, out string name);

        if (isSVID) return GetMessage(type, name, input);

        var search = await SearchName(input);

        if (search.Value.Item1.Count == 0 || search == null) return NoExactsMessage(search.Value.Item2);
        else if (search.Value.Item1.Count == 1) return await GetMessage(search.Value.Item1.First());

        string msg = "";

        foreach (var item in search.Value.Item1)
        {
            msg += await GetMessage(item) + "\n";
        }
        return msg[0..^1];
    }

    public static async Task<string> GetSVID(string svid) => GetMessage(SVIDToType(svid), await new Entity(svid).GetNameAsync(), svid);

    private static async Task<string> GetMessage(SearchReturn search) => GetMessage(SVIDToType(search.SVID), search.Name, search.SVID);

    private static string GetMessage(SVIDTypes type, string name, string svid) => $"{type}:\nSVID - {svid}\nName - {name}";
}
