using SpookVooper.Api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;
using static Shared.Database;
using static Shared.Main;

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
            if (fromEntities.Count != 1) return await PayNameError(fromEntities, "from");
            if (toEntities.Count != 1) return await PayNameError(toEntities, "to");

            fromEntity = fromEntities.First();
            toEntity = toEntities.First();

            fromEntity.Value.Auth_Key = (await GetToken(platform, id)) + "|" + OauthSecret;

            var results = await fromEntity.Value.SendCreditsAsync(amount, toEntity.Value, $"CocaBot {platform} /pay");
            if (results.Succeeded)
                return $"Successfully sent ¢{amount} from {fromEntity.Key} {await fromEntity.Value.GetNameAsync()} to {toEntity.Key} {await toEntity.Value.GetNameAsync()}";
            else return "Transaction failed. SVAPI Info:" + results.Info;
        }

        private static async Task<string> PayNameError(Dictionary<SVIDTypes, Entity> entities, string toorfrom)
        {
            string msg = $"The name you used for your {toorfrom} was both a group name and a user name! Please one of these SVID's instead:";
            foreach (var entity in entities)
                msg += $"\n{entity.Key} - {await entity.Value.GetNameAsync()}";
            return msg;
        }
    }
}
