using SpookVooper.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;

namespace Shared.Commands
{
    public class Balance
    {
        public static async Task<string> BalanceAll(string input)
        {
            Dictionary<SVIDTypes, Entity> entities = await ConvertToEntities(input);

            if (entities == null || entities.Count == 0) return "This is not a valid name or svid!";
            if (entities.Count == 1) return await BalanceMessage(entities.First().Value);

            string msg = "";

            foreach (var entity in entities)
            {
                msg += entity.Key.ToString() + " ";
                msg += await BalanceMessage(entity.Value) + "\n";
            }
            msg = msg.Substring(0, msg.Length - 1);
            return msg;
        }

        private static async Task<string> BalanceMessage(Entity entity)
        {
            string name = await entity.GetNameAsync();
            decimal balance = await GetBalance(entity);
            return $"{name}'s Balance: ¢{balance}";
        }

        public static async Task<decimal> GetBalance(Entity entity)
        {
            decimal balance = await entity.GetBalanceAsync();
            return Math.Round(balance, 2, MidpointRounding.ToZero);
        }
    }
}