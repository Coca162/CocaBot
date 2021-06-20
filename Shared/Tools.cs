using SpookVooper.Api.Entities;
using SpookVooper.Api.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Shared.Main;

namespace Shared
{
    public class Tools
    {
        public static async Task<SVIDTypes> SVIDToType(string svid)
        {
            bool isGroup = svid[0].Equals('g');
            bool isUser = svid[0].Equals('u');

            if (isUser) return SVIDTypes.User;
            else if (isGroup) return SVIDTypes.Group;
            else if (!isGroup & !isUser) throw new Exception($"The SVID {svid} is invalid");
            else throw new Exception("A SVID cannot both have 'g' and 'u' at the first character of a string!");
        }

        public static async Task<Dictionary<SVIDTypes, string>> NameToSVIDs(string name)
        {
            Dictionary<SVIDTypes, string> svids = new();

            string gsvid = await Group.GetSVIDFromNameAsync(name);
            string usvid = await User.GetSVIDFromUsernameAsync(name);

            bool isgroup = gsvid[0].Equals('g');
            bool isuser = usvid[0].Equals('u');

            if (!isgroup && !isuser) return null;
            else if (isgroup && isuser)
            {
                svids.Add(SVIDTypes.User, usvid);
                svids.Add(SVIDTypes.Group, gsvid);
            }
            else if (isuser) svids.Add(SVIDTypes.User, usvid);
            else svids.Add(SVIDTypes.Group, gsvid);

            return svids;
        }

        public static async Task<Dictionary<SVIDTypes, Entity>> ConvertToEntities(string input)
        {
            Dictionary<SVIDTypes, Entity> entities = new();
            foreach (KeyValuePair<SVIDTypes, string> svid in await ConvertToSVIDs(input))
            {
                entities.Add(svid.Key, new Entity(svid.Value));
            }
            return entities;
        }

        public static async Task<Dictionary<SVIDTypes, string>> ConvertToSVIDs(string input)
        {
            if (input == null) return null;
            Dictionary<SVIDTypes, string> entities = new();
            if (input == "Old King") entities.Add(SVIDTypes.Group, "g-oldking");
            StringInputs type = await IdentifyInput(input);
            if (type == StringInputs.SVID) entities.Add(await SVIDToType(input), input);
            else if (type == StringInputs.Name)
            {
                Dictionary<SVIDTypes, string> NameEntities = await NameToSVIDs(input);
                if (NameEntities != null)
                    foreach ((SVIDTypes entityType, string entity) in NameEntities) entities.Add(entityType, entity);
            }
            return entities;
        }


        public static async Task<StringInputs> IdentifyInput(string input)
        {
            if ((input[0] == 'g' || input[0] == 'u') && await VerifySVID(input)) return StringInputs.SVID;
            else return StringInputs.Name;
        }

        public static async Task<bool> VerifySVID(string svid)
        {
            Entity Entity = new Entity(svid);
            string result = await Entity.GetNameAsync();
            return !result.Contains($"Could not find entity {svid}");
        }

        public enum StringInputs
        {
            Name,
            SVID
        }

        public enum SVIDTypes
        {
            User,
            Group
        }
    }
}
