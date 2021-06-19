using SpookVooper.Api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Tools;

namespace Shared.Commands
{
    public class Get
    {
        public static async Task<string> GetAll(string input)
        {
            Dictionary<SVIDTypes, Entity> entities = await ConvertToEntities(input);

            if (entities.Count == 0) return "This is not a valid name or svid!";
            if (entities.Count == 1) return await GetMessage(entities.First());

            string msg = "";

            foreach (var entity in entities)
            {
                msg += await GetMessage(entity) + "\n";
            }
            msg = msg.Substring(0, msg.Length - 1);
            return msg;
        }

        private static async Task<string> GetMessage(KeyValuePair<SVIDTypes, Entity> entity)
        {
            return $"{entity.Key}:\nSVID - {entity.Value.Id}\nName - {await entity.Value.GetNameAsync()}";
        }
    }
}