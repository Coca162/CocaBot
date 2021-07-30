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

namespace Shared.Commands
{
    public static class Payment
    {
        public static async Task<string> Pay(decimal amount, string from, string to, CocaBotContext db)
        {
            Dictionary<SVIDTypes, Entity> fromEntities = await ConvertToEntities(from);
            Dictionary<SVIDTypes, Entity> toEntities = await ConvertToEntities(to);

            if (fromEntities == null || fromEntities.Count == 0) return "The from Entity is not a valid name or svid!";
            if (toEntities == null || toEntities.Count == 0) return "The to Entity is not a valid name or svid!";
            if (fromEntities.Count != 1) return await NameError(fromEntities, "from");
            if (toEntities.Count != 1) return await NameError(toEntities, "to");

            (SVIDTypes fromType, Entity fromEntity) = fromEntities.First();
            (SVIDTypes toType, Entity toEntity) = toEntities.First();

            string token = await GetToken(from, db);
            if (token == null) return "Your account is not linked! Do /register and follow the steps!";

            fromEntity.Auth_Key = token + "|" + config.OauthSecret;

            TaskResult results = await fromEntity.SendCreditsAsync(amount, toEntity, $"CocaBot {platform} /pay");
            
            if (results.Succeeded)
                return $"Successfully sent ¢{amount} from {fromType} {await fromEntity.GetNameAsync()} to {toType} {await toEntity.GetNameAsync()}";
            
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
                    return $"You cannot afford to send {amount}! You only have ${await GetBalance(fromEntity)} in your balance.";
                default:
                    Console.WriteLine($"An unknown payment error has occurred: {results.Info}\nAdd to payment error handler!");
                    return "Transaction failed. SVAPI Info:" + results.Info;
            }
        }

        private static async Task<string> NameError(Dictionary<SVIDTypes, Entity> entities, string toOrFrom)
        {
            string msg = $"The name you used for your {toOrFrom} was both a group name and a user name! Please one of these SVID's instead:";
            foreach ((SVIDTypes type, Entity entity) in entities)
                msg += $"\n{type} - {entity.Id}";
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
