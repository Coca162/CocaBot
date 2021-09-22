using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SpookVooper.Api;
using SpookVooper.Api.Entities;
using static Shared.Main;
using static Shared.Tools;
using static Shared.Database;
using static Shared.Commands.Balance;
using static Shared.Commands.Shared;

namespace Shared.Commands;
public static class Payment
{
    public static async Task<string> PayAll(decimal amount, string from, string to, CocaBotContext db)
    {
        string toSVID;

        if (string.IsNullOrWhiteSpace(to)) return null;
        if (!TrySVID(from, out string fromName)) throw new Exception($"From is not a valid svid for pay. From: {from}");
        if (TrySVID(to, out SVIDTypes toType, out string toName)) toSVID = to;
        else
        {
            var toSearch = await SearchName(to);

            if (toSearch.Item1.Count == 0) return NoExactsMessage(toSearch.Item2);
            else if (toSearch.Item1.Count == 1)
            {
                var entity = toSearch.Item1.First();
                toSVID = entity.SVID;
                toName = entity.Name;
                toType = SVIDToType(toSVID);
            }
            else return await NameError(toSearch.Item1.ToDictionary(x => SVIDToType(x.SVID), x => x.SVID), to);
        }

        return await Pay(amount, from, db, toSVID, fromName, toType, toName);
    }

    public static async Task<string> PaySVID(decimal amount, string from, string to, CocaBotContext db)
    {
        if (string.IsNullOrWhiteSpace(to)) return null;
        if (!TrySVID(from, out string fromName)) throw new Exception($"From is not a valid svid for pay. From: {from}");
        if (!TrySVID(to, out SVIDTypes toType, out string toName)) 
            return "This is not a valid svid! Did you mean to use `c/pay [Amount] {Name}`?";

        return await Pay(amount, from, db, to, fromName, toType, toName);
    }

    private static async Task<string> Pay(decimal amount, string from, CocaBotContext db, string toSVID, string fromName, SVIDTypes toType, string toName)
    {
        string token = await GetToken(from, db);
        if (token == null) return "Your account is not linked! Do /register and follow the steps!";

        Entity fromEntity = new(from);
        fromEntity.Auth_Key = token + "|" + config.OauthSecret;

        TaskResult results = await fromEntity.SendCreditsAsync(amount, toSVID, toType == SVIDTypes.User ? $"CocaBot {platform} User-User /pay" : "Corporate");

        if (results.Succeeded)
            return $"Successfully sent ¢{amount} from {fromName} to {SVIDTypeToString(toType)} {toName}";

        string error = results.Info;
        PaymentErrors paymentErrors;
        if (error.Contains("You do not have permission to do that!")) paymentErrors = PaymentErrors.Unauthorized;
        else if (error.Contains("Transaction must be positive.")) paymentErrors = PaymentErrors.Positive;
        else if (error.Contains("An entity cannot send credits to itself.")) paymentErrors = PaymentErrors.Self;
        else if (error.Contains("Transaction must have a value.")) paymentErrors = PaymentErrors.NoValue;
        else if (error.Contains(" cannot afford to send ¢")) paymentErrors = PaymentErrors.LacksFunds;
        else paymentErrors = PaymentErrors.Unknown;

        switch (paymentErrors)
        {
            case PaymentErrors.Unauthorized:
                return "You do not have permission to do that!";
            case PaymentErrors.Positive:
                return "Transaction must be positive!";
            case PaymentErrors.Self:
                return "You can't send money to yourself!";
            case PaymentErrors.NoValue:
                return "You are either sending no money or the value is too small!";
            case PaymentErrors.LacksFunds:
                return $"You cannot afford to send {amount}! You only have ¢{Round((await fromEntity.GetBalanceAsync()).Data)} in your balance.";
            default:
                Console.WriteLine($"An unknown payment error has occurred: {results.Info}\nAdd to payment error handler!");
                return "Transaction failed. SVAPI Info:" + results.Info;
        }
    }

    private static async Task<string> NameError(Dictionary<SVIDTypes, string> entities, string name)
    {
        string msg = $"The name ({name}) you used for the thing you were paying was both a group name and a user name! Please one of these SVID's instead:";
        foreach ((SVIDTypes type, string entity) in entities)
            msg += $"\n{SVIDTypeToString(type)} - {entity}";
        return msg;
    }

    private enum PaymentErrors
    {
        Unauthorized,
        Positive,
        Self,
        LacksFunds,
        NoValue,
        Unknown
    }
}
