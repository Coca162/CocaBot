using SpookVooper.Api.Entities;
using SpookVooper.Api.Entities.Groups;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Shared.Main;
using static Shared.Tools;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Shared;
public class Tools
{
    public static async Task<List<string>> NameToSVIDs(string name)
    {
        if (name == "Old King")
            return new List<string>() { { "g-oldking" } };

        List<string> svids = new();

        string gsvid = await Group.GetSVIDFromNameAsync(name);
        string usvid = await User.GetSVIDFromUsernameAsync(name);

        bool isgroup = gsvid[0].Equals('g');
        bool isuser = usvid[0].Equals('u');

        if (!isgroup && !isuser) return null;
        else if (isgroup && isuser)
        {
            svids.Add(usvid);
            svids.Add(gsvid);
        }
        else if (isuser) svids.Add(usvid);
        else svids.Add(gsvid);

        return svids;
    }

    public static async Task<List<Entity>> ConvertToEntities(string input)
    {
        List<Entity> entities = new();
        List<string> list = await ConvertToSVIDs(input);
        for (int i = 0; i < list.Count; i++)
        {
            string svid = list[i];
            entities.Add(new Entity(svid));
        }
        return entities;
    }

    public static async Task<List<string>> ConvertToSVIDs(string input)
    {
        if (input == null) return null;
        List<string> entities = new();
        bool isSVID = TrySVID(input, out _, out _);
        if (isSVID) entities.Add(input);
        else
        {
            List<string> NameEntities = await NameToSVIDs(input);
            if (NameEntities != null)
                for (int i = 0; i < NameEntities.Count; i++)
                {
                    string entity = NameEntities[i];
                    entities.Add(entity);
                }
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
        string result = Entity.GetNameAsync().Result;
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
        Stream response = await client.GetStreamAsync($"https://api.spookvooper.com/Entity/Search?name={input}");
        SearchReturn[] entities = await System.Text.Json.JsonSerializer.DeserializeAsync<SearchReturn[]>(response);


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

    public static string SVIDTypeToString(SVIDTypes type)
    {
        return type switch
        {
            SVIDTypes.User => nameof(SVIDTypes.User),
            SVIDTypes.Group => nameof(SVIDTypes.Group),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    public enum SVIDTypes
    {
        User,
        Group
    }

    public enum StringInputs
    {
        Name,
        SVID
    }

    public class SearchReturn
    {
        [JsonPropertyName("id")]
        public string SVID { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("credits")]
        public decimal Credits { get; set; }
        [JsonPropertyName("image_Url")]
        public string Pfp { get; set; }
    }
}