using SpookVooper.Api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;
using static Shared.Database;
using static Shared.Main;
using static Shared.Commands.Balance;
using System;

namespace Shared.Commands
{
    public class Payment
    {
        public static async Task<string> Pay(decimal amount, string from, string to, Platform platform, ulong id)
        {
            Dictionary<SVIDTypes, Entity> fromEntities = await ConvertToEntities(from);
            Dictionary<SVIDTypes, Entity> toEntities = await ConvertToEntities(to);
            KeyValuePair<SVIDTypes, Entity> fromEntity, toEntity;

            if (toEntities == null || fromEntities.Count == 0) return "The from Entity is not a valid name or svid!";
            if (toEntities == null || toEntities.Count == 0) return "The to Entity is not a valid name or svid!";
            if (fromEntities.Count != 1) return await NameError(fromEntities, "from");
            if (toEntities.Count != 1) return await NameError(toEntities, "to");

            fromEntity = fromEntities.First();
            toEntity = toEntities.First();

            
            fromEntity.Value.Auth_Key = await GetToken(platform, id) + "|" + OauthSecret;

            var results = await fromEntity.Value.SendCreditsAsync(amount, toEntity.Value, $"CocaBot {platform} /pay");
            if (results.Succeeded)
                return $"Successfully sent ¢{amount} from {fromEntity.Key} {await fromEntity.Value.GetNameAsync()} to {toEntity.Key} {await toEntity.Value.GetNameAsync()}";
            else
            {
                string error = results.Info;
                PaymentErrors paymentErrors;
                if (error.Contains("You do not have permission to do that!")) paymentErrors = PaymentErrors.Unauthorized;
                else if (error.Contains("Transaction must be positive.")) paymentErrors = PaymentErrors.Positive;
                else if (error.Contains("An entity cannot send credits to itself.")) paymentErrors = PaymentErrors.Self;
                else if (error.Contains("Transaction must have a value.")) paymentErrors = PaymentErrors.NoValue;
                else if (error.Contains(" cannot afford to send ¢")) paymentErrors = PaymentErrors.LacksFunds;
                else paymentErrors = PaymentErrors.Unknow;

                switch (paymentErrors)
                {
                    case PaymentErrors.Unauthorized:
                        return "Your account is not linked! Do /register and follow the steps!";
                    case PaymentErrors.Positive:
                        return "Transaction must be positive!";
                    case PaymentErrors.Self:
                        return "You can't send money to yourself!";
                    case PaymentErrors.NoValue:
                        return "You are either sending no money or the value is too small!";
                    case PaymentErrors.LacksFunds:
                        return $"You cannot afford to send {amount}! You only have ${await GetBalance(fromEntity.Value)} in your balance.";
                    default:
                        Console.WriteLine($"An unknow payment error has accured: {results.Info}\nAdd to payment error handler!");
                        return "Transaction failed. SVAPI Info:" + results.Info;
                }
            }
        }

        private static async Task<string> NameError(Dictionary<SVIDTypes, Entity> entities, string toorfrom)
        {
            string msg = $"The name you used for your {toorfrom} was both a group name and a user name! Please one of these SVID's instead:";
            foreach (var entity in entities)
                msg += $"\n{entity.Key} - {await entity.Value.GetNameAsync()}";
            return msg;
        }

        private enum PaymentErrors
        {
            Unauthorized,
            Positive,
            Self,
            LacksFunds,
            NoValue,
            Unknow
        }
    }
}
