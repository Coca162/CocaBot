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

        var (exact, nonExact) = await GetBalance(input);

        if (!exact.Any()) return NoExactsMessage(input, nonExact.Select(x => x.name));
        else if (exact.Count == 1) return BalanceMessage(input, exact.First().balance);

        string msg = $"{input} Balances:";
        foreach ((string _, decimal balance) in exact)
        {
            msg += "\n" + BalanceMessage(input, balance);
        }
        return msg[..^1];
    }

    public static async Task<string> BalanceMessage(string svid)
    {
        Entity entity = new(svid);
        return BalanceMessage(await GetName(entity.Id), (await entity.GetBalanceAsync()).Data);
    }

    public static string BalanceMessage((string name, decimal balance) user) => BalanceMessage(user.name, user.balance);

    public static async Task<string> BalanceMessage(string name, string svid) => BalanceMessage(name, (await new Entity(svid).GetBalanceAsync()).Data);

    private static string BalanceMessage(string name, decimal credits) => $"{name}'s Balance: ¢{Round(credits)}";

    public static string Round(decimal credits) => string.Format("{0:n2}", Math.Round(credits, 2, MidpointRounding.ToZero));
}
