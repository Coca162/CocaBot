using SpookVooper.Api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;
using static Shared.Commands.Shared;
using static Shared.Main;

namespace Shared.Commands;
public static class Get
{
    public static async Task<string> GetAll(string input)
    {
        (bool isSVID, string name) = await TryName(input);

        if (isSVID) return GetMessage(name, input);

        var search = await SearchName(input);

        if (!search.Item1.Any()) return NoExactsMessage(input, search.Item2);
        else if (search.Item1.Count == 1) return GetMessage(search.Item1.First());

        string msg = "";

        foreach (var item in search.Item1)
        {
            msg += GetMessage(item.Name, item.SVID) + "\n";
        }
        return msg[0..^1];
    }

    public static async Task<string> GetSVID(string svid) => GetMessage(await new Entity(svid).GetNameAsync(), svid);

    private static string GetMessage(BasicEntity search) => GetMessage(search.Name, search.SVID);

    private static string GetMessage(string name, string svid) => 
        $"{SVIDTypeToString(SVIDToType(svid))}:\nSVID - {svid}\nName - {name}";
}
