using Newtonsoft.Json;
using SpookVooper.Api.Entities;
using SpookVooper.Api.Entities.Groups;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Shared.Main;
using static Shared.Tools;

namespace Shared
{
    public class Tools
    {
        public static async Task<Dictionary<SVIDTypes, string>> NameToSVIDs(string name)
        {
            if (name == "Old King")
                return new Dictionary<SVIDTypes, string>() { { SVIDTypes.Group, "g-oldking" } };

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
            foreach ((SVIDTypes type, string svid) in await ConvertToSVIDs(input))
            {
                entities.Add(type, new Entity(svid));
            }
            return entities;
        }

        public static async Task<Dictionary<SVIDTypes, string>> ConvertToSVIDs(string input)
        {
            if (input == null) return null;
            Dictionary<SVIDTypes, string> entities = new();
            bool isSVID = TrySVID(input, out SVIDTypes type, out _);
            if (isSVID) entities.Add(type, input);
            else
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

        public static bool TrySVID(string input, out SVIDTypes type, out string name)
        {
            name = null;
            type = default;
            bool isGroup = input[0].Equals('g');
            bool isUser = input[0].Equals('u');

            if (isUser) type = SVIDTypes.User;
            else if (isGroup) type = SVIDTypes.Group;
            else if (!isGroup & !isUser) return false;

            Entity Entity = new(input);
            string result = Entity.GetName();
            if (!result.Contains($"Could not find entity {input}"))
            {
                name = result;
                return true;
            }
            return false;
        }

        public static SVIDTypes SVIDToType(string svid)
        {
            bool isGroup = svid[0].Equals('g');
            bool isUser = svid[0].Equals('u');

            if (isUser) return SVIDTypes.User;
            else if (isGroup) return SVIDTypes.Group;
            else if (!isGroup & !isUser) throw new Exception($"The SVID {svid} is invalid");
            else throw new Exception("A SVID cannot both have 'g' and 'u' at the first character of a string!");
        }

        public static async Task<bool> VerifySVID(string svid)
        {
            Entity Entity = new(svid);
            string result = await Entity.GetNameAsync();
            return !result.Contains($"Could not find entity {svid}");
        }

        public static async Task<(List<SearchReturn>, List<SearchReturn>)?> SearchName(string input)
        {
            var response = await client.GetAsync($"https://api.spookvooper.com/Entity/Search?name={input}");
            if (response == null) return null;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.ReasonPhrase);
                return null;
            }

            List<SearchReturn> entities = JsonConvert.DeserializeObject<List<SearchReturn>>(await response.Content.ReadAsStringAsync());

            List<SearchReturn> exact = new();
            List<SearchReturn> notExact = new();

            foreach (var entity in entities)
            {
                if (entity.Name.ToLower() == input.ToLower())
                {
                    exact.Add(entity);
                }
                else notExact.Add(entity);
            }

            return (exact, notExact);
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

        public class SearchReturn
        {
            [JsonProperty("id")]
            public string SVID;
            [JsonProperty("name")]
            public string Name;
            [JsonProperty("credits")]
            public decimal Credits;
            [JsonProperty("image_Url")]
            public string Pfp;
        }
    }
}