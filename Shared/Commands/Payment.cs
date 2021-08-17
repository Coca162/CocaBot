using SpookVooper.Api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;
using static Shared.Database;
using static Shared.Main;
using static Shared.Commands.Balance;
using System;
using SpookVooper.Api;
using static Shared.Tools;
using static Shared.Commands.Shared;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Shared.Commands
{
    public static class Payment
    {
        public static async Task<string> Pay(decimal amount, string from, string to, CocaBotContext db)
        {
            string toSVID;

            if (string.IsNullOrWhiteSpace(to)) return null;
            if (!TrySVID(from, out _, out string fromName)) throw new Exception($"From is not valid for pay. From: {from}");
            if (TrySVID(to, out SVIDTypes toType, out string toName))
            {
                toSVID = to;
            }
            else
            {
                var toSearch = await SearchName(to);

                if (toSearch.Value.Item1.Count == 0 || toSearch == null) return NoExactsMessage(toSearch.Value.Item2);
                else if (toSearch.Value.Item1.Count == 1)
                {
                    var entity = toSearch.Value.Item1.First();
                    toSVID = entity.SVID;
                    toName = entity.Name;
                    toType = SVIDToType(toSVID);
                }
                else return await NameError(toSearch.Value.Item1.ToDictionary(x => SVIDToType(x.SVID), x => x.SVID), to);
            }

            string token = await GetToken(from, db);
            if (token == null) return "Your account is not linked! Do /register and follow the steps!";

            Entity fromEntity = new(from);
            fromEntity.Auth_Key = token + "|" + config.OauthSecret;

            TaskResult results = await fromEntity.SendCreditsAsync(amount, new Entity(toSVID), $"CocaBot {platform} /pay");

            if (results.Succeeded)
                return $"Successfully sent ¢{amount} from {fromName} to {toType} {toName}";
            
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
                    return $"You cannot afford to send {amount}! You only have ¢{await BalanceMessage(fromName, fromEntity.Id)} in your balance.";
                default:
                    Console.WriteLine($"An unknown payment error has occurred: {results.Info}\nAdd to payment error handler!");
                    return "Transaction failed. SVAPI Info:" + results.Info;
            }
        }

        private static async Task<string> NameError(Dictionary<SVIDTypes, string> entities, string name)
        {
            string msg = $"The name ({name}) you used for the thing you were paying was both a group name and a user name! Please one of these SVID's instead:";
            foreach ((SVIDTypes type, string entity) in entities)
                msg += $"\n{type} - {entity}";
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
}
