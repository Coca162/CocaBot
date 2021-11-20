using SpookVooper.Api.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;
using static Shared.Commands.Shared;
using static Shared.Cache;

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
        return BalanceMessage(await GetName(entity), (await entity.GetBalanceAsync()).Data);
    }

    public static string BalanceMessage(SearchReturn entity) => BalanceMessage(entity.Name, entity.Credits);

    public static async Task<string> BalanceMessage(string name, string svid) => BalanceMessage(name, (await new Entity(svid).GetBalanceAsync()).Data);

    private static string BalanceMessage(string name, decimal credits) => $"{name}'s Balance: ¢{Round(credits)}";

    public static string Round(decimal credits) => string.Format("{0:n2}", Math.Round(credits, 2, MidpointRounding.ToZero));
}
