using SpookVooper.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Tools;
using static SpookVooper.Api.SpookVooperAPI;

namespace Shared;
public static class Cache
{
    public static Dictionary<string, string> EntityCache { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

    public static async Task AddEntityCache(string svid, string name)
    {
        EntityCache.Add(svid, name);

        SVIDTypes type = SVIDToType(svid);
        if (type == SVIDTypes.User)
        {
            var group = await GetData($"group/GetSVIDFromName?name={name}");

            if (!group.StartsWith("HTTP E")) EntityCache.Add(group, name);
        }
        else if (type == SVIDTypes.Group)
        {
            var user = await GetData($"user/GetSVIDFromUsername?username={name}");

            if (!user.StartsWith("HTTP E")) EntityCache.Add(user, name);
        }
    }

    public static async Task<bool> Contains(string svid) 
        => EntityCache.ContainsKey(svid) || !(await GetData($"Entity/GetName?svid={svid}")).StartsWith("Could not find entity");
                
    public static async Task<(List<string> exact, List<(string name, string svid)> nonExact)> GetSVIDs(string name)
    {
        var cache = EntityCache.Where(x => string.Equals(x.Value, name, StringComparison.OrdinalIgnoreCase)).Select(x => x.Key);
        return !cache.Any() ? await SearchNameToSVIDs(name) : (cache.ToList(), new List<(string name, string svid)>());
    }

    public static async Task<string> GetName(string svid)
    {
        if (!EntityCache.TryGetValue(svid, out string name))
        {
            string results = await GetData($"Entity/GetName?svid={svid}");
            if (!results.Contains("Could not find entity"))
            {
                name = results;
                AddEntityCache(svid, name);
            }
        }
        return name;
    }

    public static async Task<string> GetName(Entity entity)
    {
        if (!EntityCache.TryGetValue(entity.Id, out string name))
        {
            string results = await entity.GetNameAsync();
            if (!results.Contains("Could not find entity"))
            {
                name = results;
                AddEntityCache(entity.Id, name);
            }
        }
        return name;
    }
}