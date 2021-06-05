using SpookVooper.Api.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Shared.Tools;

namespace Shared.Commands
{
    public class Balance
    {
        public static async Task<string> BalanceAll(string input)
        {
            Dictionary<SVIDTypes, Entity> entities = await ConvertToEntities(input);

            if (entities.Count == 0) return "This is not a valid name or svid!";
            if (entities.Count == 1) return await BalanceMessage(entities[0]);

            string msg = "";

            foreach (var entity in entities)
            {
                msg += entity.Key.ToString() + " ";
                msg += await BalanceMessage(entity.Value) + "\n";
            }
            return msg.Substring(msg.Length - 1);
        }

        private static async Task<string> BalanceMessage(Entity entity)
        {
            return $"{await entity.GetNameAsync()}'s Balance: ¢{Math.Round(await entity.GetBalanceAsync(), 2, MidpointRounding.ToZero)}";
        }
    }
}