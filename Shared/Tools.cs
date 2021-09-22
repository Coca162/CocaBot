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
using System.IO;
using System.Text.Json;

namespace Shared;
public class Tools
{
    public static async Task<List<string>> NameToSVIDs(string name)
    {
        if (name == "Old King")
            return new List<string> { "g-oldking" };

        List<string> svids = new();

        string gsvid = await Group.GetSVIDFromNameAsync(name);
        string usvid = await User.GetSVIDFromUsernameAsync(name);

        bool isgroup = gsvid[0].Equals('g');
        bool isuser = usvid[0].Equals('u');

        if (!isgroup && !isuser) return null;
        if (isgroup && isuser)
        {
            svids.Add(usvid);
            svids.Add(gsvid);
        }
        else svids.Add(isuser ? usvid : gsvid);

        return svids;
    }

    public static async Task<List<string>> ConvertToSVIDs(string input)
    {
        if (input == null) return null;
        List<string> entities = new();
        bool isSVID = TrySVID(input, out _);
        if (isSVID) entities.Add(input);
        else
        {
            List<string> nameEntities = await NameToSVIDs(input);
            if (nameEntities != null) entities.AddRange(nameEntities);
        }
        return entities;
    }

    public static bool TrySVID(string input, out SVIDTypes type, out string name)
    {
        name = null;
        type = default;

        if (input.StartsWith('u')) type = SVIDTypes.User;
        else if (input.StartsWith('g')) type = SVIDTypes.Group;
        else return false;

        return ConfirmSVID(input, ref name);
    }

    public static bool TrySVID(string input, out string name)
    {
        name = null;

        return (input.StartsWith('u') || input.StartsWith('g')) && ConfirmSVID(input, ref name);
    }

    private static bool ConfirmSVID(string input, ref string name)
    {
        Entity entity = new(input);
        string result = entity.GetNameAsync().Result;
        if (!result.Contains($"Could not find entity {input}"))
        {
            name = result;
            return true;
        }
        return false;
    }

    public static SVIDTypes SVIDToType(string svid)
    {
        bool isGroup = svid.StartsWith('g');
        bool isUser = svid.StartsWith('u');

        if (isUser) return SVIDTypes.User;
        if (isGroup) return SVIDTypes.Group;
        if (!isGroup & !isUser) throw new Exception($"The SVID {svid} is invalid");
        throw new Exception("A SVID cannot both have 'g' and 'u' at the first character of a string!");
    }

    public static async Task<(List<SearchReturn>, List<SearchReturn>)> SearchName(string input)
    {
        Stream response = await client.GetStreamAsync($"https://api.spookvooper.com/Entity/Search?name={input}");
        SearchReturn[] entities = await JsonSerializer.DeserializeAsync<SearchReturn[]>(response);

        List<SearchReturn> exact = new();
        List<SearchReturn> notExact = new();

        foreach (SearchReturn entity in entities)
        {
            if (string.Equals(entity.Name, input, StringComparison.OrdinalIgnoreCase))
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
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
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