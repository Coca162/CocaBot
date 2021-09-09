using SpookVooper.Api.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;
using static Shared.Commands.Shared;

namespace Shared.Commands;
public static class Balance
{
    public static async Task<string> BalanceAll(string input)
    {
        bool isSVID = TrySVID(input, out _, out string name);

        if (isSVID) return await BalanceMessage(name, input);

        var search = await SearchName(input);

        if (search.Value.Item1.Count == 0 || search == null) return NoExactsMessage(search.Value.Item2);
        else if (search.Value.Item1.Count == 1) return BalanceMessage(search.Value.Item1.First());

        string msg = "";

        foreach (var item in search.Value.Item1)
        {
            msg += SVIDToType(item.SVID) + " ";
            msg += BalanceMessage(item) + "\n";
        }
        return msg[0..^1];
    }

    public static async Task<string> BalanceSVID(string svid)
    {
        Entity entity = new(svid);
        return BalanceMessage(await entity.GetNameAsync(), await entity.GetBalanceAsync());
    }

    public static string BalanceMessage(SearchReturn entity) => BalanceMessage(entity.Name, entity.Credits);

    public static async Task<string> BalanceMessage(string name, string svid) => BalanceMessage(name, await new Entity(svid).GetBalanceAsync());

    private static string BalanceMessage(string name, decimal credits) => $"{name}'s Balance: ¢{Round(credits)}";

    public static decimal Round(decimal credits) => Math.Round(credits, 2, MidpointRounding.ToZero);
}
