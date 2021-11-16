using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SpookVooper.Api;
using SpookVooper.Api.Entities;
using static Shared.Main;
using static Shared.Tools;
using static Shared.Commands.Balance;
using static Shared.Commands.Shared;
using static Shared.Cache;

namespace Shared.Commands;
public static class Payment
{
    public static async Task<string> PayAll(decimal amount, string from, string to, string token)
    {
        string toSVID = null;
        string toName;

        if (string.IsNullOrWhiteSpace(to)) return null;

        (bool fromIsSVID, string fromName) = await TryName(from);
        if (!fromIsSVID) throw new Exception($"From is not a valid svid for pay. From: {from}");

        (bool toIsSVID, toName) = await TryName(to);
        if (toIsSVID) toSVID = to;
        else
        {
            (List<string> exact, List<(string _, string svid)> nonExact) = await GetSVIDs(to);

            int count = exact.Count;
            if (count == 1)
            {
                var entity = exact.Single();
                toSVID = entity;
                toName = to;
            }
            else if (count != 0) return await NameError(nonExact.Select(x => x.svid), to);
        }

        return await Pay(amount, from, toSVID, fromName, toName, token);
    }

    public static async Task<string> PaySVID(decimal amount, string from, string to, string token)
    {
        if (string.IsNullOrWhiteSpace(to)) return null;

        (bool fromIsSVID, string fromName) = await TryName(from);
        if (!fromIsSVID) throw new Exception($"From is not a valid svid for pay. From: {from}");

        (bool toIsSVID, string toName) = await TryName(to);
        if (!toIsSVID) return "This is not a valid svid! Did you mean to use `c/pay [Amount] {Name}`?";

        return await Pay(amount, from, to, fromName, toName, token);
    }

    private static async Task<string> Pay(decimal amount, string from, string toSVID, string fromName, string toName, string token)
    {
        SVIDTypes toType = SVIDToType(toSVID);

        Entity fromEntity = new(from);
        fromEntity.Auth_Key = token + "|" + config.OauthSecret;

        TaskResult results = await fromEntity.SendCreditsAsync(amount, toSVID, $"CocaBot {platform} /pay");

        if (results.Succeeded)
            return $"Successfully sent ¢{Round(amount)} from {fromName} to {SVIDTypeToString(toType)} {toName}";
        
        return results.Info switch
        {
            string error when error.Contains("You do not have permission to do that!") => error,
            string error when error.Contains("Transaction must be positive.") => "Transaction must be positive!",
            string error when error.Contains("An entity cannot send credits to itself.") => "You can't send money to yourself!",
            string error when error.Contains("Transaction must have a value.") => "You are either sending no money or the value is too small!",
            string error when error.Contains(" cannot afford to send ¢") => 
            $"You cannot afford to send {amount}! You only have ¢{Round((await fromEntity.GetBalanceAsync()).Data)} in your balance.",
            _ => await Task.Run(() =>
            {
                Console.WriteLine($"An unknown payment error has occurred: {results.Info}");
                return "Transaction failed! <@388454632835514380>";
            })
        };
    }

    private static async Task<string> NameError(IEnumerable<string> svids, string name)
    {
        string msg = $"The name ({name}) you used for the thing you were paying was both a group name and a user name! Please one of these SVID's instead:";
        foreach (string svid in svids)
            msg += $"\n{SVIDToTypeString(svid)} - {svid}";
        return msg;
    }
}
