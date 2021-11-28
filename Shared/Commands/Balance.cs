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

        var (exact, nonExact) = await NameToBalance(input);
        var first = exact.FirstOrDefault();
        if (first == default) return NoExactsMessage(input, nonExact.Select(x => x.name));
        else if (exact.Count == 1) return BalanceMessage(first.svid, input, first.balance);

        string msg = $"{input} Balances:";
        foreach ((string svid, decimal balance) in exact)
        {
            msg += "\n" + BalanceMessage(svid, input, balance);
        }
        return msg[..^1];
    }

    //public static async Task<string> BalanceSVIDorName(string svid, string name)
    //{
    //    var (exact, nonExact) = await GetBalance(input);

    //    if (!exact.Any()) return NoExactsMessage(input, nonExact.Select(x => x.name));

    //}

    public static async Task<string> BalanceMessage(string svid)
    {
        Entity entity = new(svid);
        return BalanceMessage(svid, await GetName(entity.Id), (await entity.GetBalanceAsync()).Data);
    }

    public static async Task<string> BalanceMessage(string name, string svid) => 
        BalanceMessage(svid, name, (await new Entity(svid).GetBalanceAsync()).Data);

    private static string BalanceMessage(string svid, string name, decimal credits) => 
        $"{SVIDToTypeString(svid)} {name} Balance: ¢{Round(credits)}";

    public static string Round(decimal credits) => string.Format("{0:n2}", Math.Round(credits, 2, MidpointRounding.ToZero));
}
