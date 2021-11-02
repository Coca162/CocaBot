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
public static class Tools
{
    public static string Humanize(double number)
    {
        string[] suffix = { "f", "a", "p", "n", "μ", "m", string.Empty, " thousand", " million", " billion", " trillion", "P", "E" };

        int mag;
        if (number < 1)
        {
            mag = (int)Math.Floor(Math.Floor(Math.Log10(number)) / 3);
        }
        else
        {
            mag = (int)(Math.Floor(Math.Log10(number)) / 3);
        }

        var shortNumber = number / Math.Pow(10, mag * 3);

        return $"{shortNumber:0,###}{suffix[mag + 6]}";
    }

    public static Dictionary<string, string> EntityCache { get; set; } = new();

    public static async Task AddEntityCache(string svid, string name)
    {
        EntityCache.Add(svid, name);

        SVIDTypes type = SVIDToType(svid);

        if (type == SVIDTypes.User)
        {
            var group = await SpookVooper.Api.SpookVooperAPI.GetData($"group/GetSVIDFromName?name={name}");

            if (!group.StartsWith("HTTP E"))
            {
                EntityCache.Add(group, name);
            }
        }
        else if (type == SVIDTypes.Group)
        {
            var user = await SpookVooper.Api.SpookVooperAPI.GetData($"user/GetSVIDFromUsername?username={name}");

            if (!user.StartsWith("HTTP E"))
            {
                EntityCache.Add(user, name);
            }
        }
    }

    public static async Task<List<string>> NameToSVIDs(string name)
    {
        if (name == "Old King")
            return new List<string> { "g-oldking" };

        List<string> svids = new();

        string gsvid = await Group.GetSVIDFromNameAsync(name);
        string usvid = await SpookVooper.Api.Entities.User.GetSVIDFromUsernameAsync(name);

        bool isgroup = gsvid.StartsWith('g');
        bool isuser = usvid.StartsWith('u');

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
        bool isSVID = await TestSVID(input);
        if (isSVID) entities.Add(input);
        else
        {
            List<string> nameEntities = await NameToSVIDs(input);
            if (nameEntities != null) entities.AddRange(nameEntities);
        }
        return entities;
    }

    public static async Task<(bool, string)> TryName(string svid)
    {
        if (EntityCache.TryGetValue(svid, out string name)) return (true, name);

        if (!(svid.StartsWith('u') || svid.StartsWith('g'))) return (false, null);

        Entity entity = new(svid);
        string result = await entity.GetNameAsync();
        if (!result.StartsWith("Could not find entity"))
        {
            await AddEntityCache(svid, result);
            return (true, result);
        }
        return (false, null);
    }

    public static async Task<bool> TestSVID(string svid)
    {
        if (EntityCache.ContainsKey(svid)) return true;

        if (!(svid.StartsWith('u') || svid.StartsWith('g'))) return true;

        Entity entity = new(svid);
        string result = await entity.GetNameAsync();
        if (!result.Contains("Could not find entity"))
        {
            await AddEntityCache(svid, result);
            return true;
        }
        return false;
    }

    public static SVIDTypes SVIDToType(string svid)
    {
        if (svid.StartsWith('u')) return SVIDTypes.User;
        if (svid.StartsWith('g')) return SVIDTypes.Group;
        return SVIDTypes.None;
    }

    public static async Task<(List<SearchReturn>, List<SearchReturn>)> SearchName(string input)
    {
        List<SearchReturn> exact = new();
        List<SearchReturn> notExact = new();

        Stream response = await client.GetStreamAsync($"https://api.spookvooper.com/Entity/Search?name={input}");
        SearchReturn[] entities = await JsonSerializer.DeserializeAsync<SearchReturn[]>(response);

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

    public static string SVIDToTypeString(string svid)
    {
        if (svid.StartsWith('u')) return nameof(SVIDTypes.User);
        if (svid.StartsWith('g')) return nameof(SVIDTypes.Group);
        return null;
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
        Group,
        None
    }

    public enum StringInputs
    {
        Name,
        SVID
    }

    public class SearchReturn : BasicEntity
    {
        [JsonPropertyName("image_Url")]
        public string Pfp { get; set; }
        [JsonPropertyName("credits")]
        public decimal Credits { get; set; }
    }

    public class BasicEntity
    {
        [JsonPropertyName("id")]
        public string SVID { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}