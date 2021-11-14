using SpookVooper.Api.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;
using static Shared.Commands.Shared;
using static Shared.Main;

namespace Shared.Commands;
public static class Balance
{
    public static async Task<string> BalanceAll(string input)
    {
        (bool isSVID, string name) = await TryName(input);
        if (isSVID) return await BalanceMessage(name, input);

        var search = await SearchName(input);

        if (!search.Item1.Any()) return NoExactsMessage(input, search.Item2);
        if (search.Item1.Count == 1) return BalanceMessage(search.Item1.First());

        string msg = $"{search.Item1.First().Name} Balances:";
        foreach (var item in search.Item1)
        {
            msg += "\n" + BalanceMessage(item);
}
        return msg[..^1];
    }

    public static async Task<string> BalanceMessage(string svid)
    {
        Entity entity = new(svid);
        return BalanceMessage(await entity.GetNameAsync(), (await entity.GetBalanceAsync()).Data);
    }

    public static string BalanceMessage(SearchReturn entity) => $"{SVIDTypeToString(SVIDToType(entity.SVID))}: ¢{Round(entity.Credits)}";

    public static async Task<string> BalanceMessage(string name, string svid) => BalanceMessage(name, (await new Entity(svid).GetBalanceAsync()).Data);

    private static string BalanceMessage(string name, decimal credits) => $"{name}'s Balance: ¢{string.Format("0,###.###", Round(credits))}";

    public static decimal Round(decimal credits) => Math.Round(credits, 2, MidpointRounding.ToZero);
}
