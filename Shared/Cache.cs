using SpookVooper.Api.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Tools;
using static SpookVooper.Api.SpookVooperAPI;
using SpookVooper.Api.Economy;

namespace Shared;
public static class Cache
{
    public static ConcurrentDictionary<string, (string Name, decimal Balance)> EntityCache { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

    public static async Task BalanceUpdater(Transaction transaction)
    {
        Entity from = new(transaction.FromAccount);
        Entity to = new(transaction.ToAccount);

        EntityCache.AddOrUpdate(from.Id, (await from.GetNameAsync(), (await from.GetBalanceAsync()).Data),
            (string _, (string name, decimal balance) old) => (old.name, old.balance -= transaction.Amount));

        EntityCache.AddOrUpdate(to.Id, (await to.GetNameAsync(), (await to.GetBalanceAsync()).Data),
            (string _, (string name, decimal balance) old) => (old.name, old.balance += transaction.Amount));
    }

    public static async Task RefreshCacheBalances()
    {
        foreach ((string svid, (string name, decimal balance) values) in EntityCache)
        {
            Entity entity = new(svid);
            EntityCache.TryUpdate(svid, (values.name, (await entity.GetBalanceAsync()).Data), values);
        }
    }

    public static async Task AddEntityCache(string svid)
    {
        Entity entity = new(svid);
        string name = await entity.GetNameAsync();
        AddEntityCache(entity, name);
    }

    public static async Task AddEntityCache(string svid, string name) => AddEntityCache(new Entity(svid), name);

    public static async Task AddEntityCache(Entity entity, string name)
    {
        EntityCache.TryAdd(entity.Id, (name, (await entity.GetBalanceAsync()).Data));

        SVIDTypes type = SVIDToType(entity.Id);
        if (type == SVIDTypes.User)
        {
            var alt = await GetData($"group/GetSVIDFromName?name={name}");

            if (!alt.StartsWith("HTTP E"))
            {
                Entity altEntitiy = new(alt);
                EntityCache.TryAdd(alt, (name, (await altEntitiy.GetBalanceAsync()).Data));
            }
        }
        else if (type == SVIDTypes.Group)
        {
            var alt = await GetData($"user/GetSVIDFromUsername?username={name}");

            if (!alt.StartsWith("HTTP E"))
            {
                Entity altEntitiy = new(alt);
                EntityCache.TryAdd(alt, (name, (await altEntitiy.GetBalanceAsync()).Data));
            }
        }
    }

    public static async Task<bool> Contains(string svid) 
        => EntityCache.ContainsKey(svid) || !(await GetData($"Entity/GetName?svid={svid}")).StartsWith("Could not find entity");
                
    public static async Task<(List<string> exact, List<(string name, string svid)> nonExact)> GetSVIDs(string name)
    {
        var cache = EntityCache.Where(x => string.Equals(x.Value.Name, name, StringComparison.OrdinalIgnoreCase)).Select(x => x.Key);
        return !cache.Any() ? await SearchNameToSVIDs(name) : (cache.ToList(), new List<(string name, string svid)>());
    }

    public static async Task<(List<(string svid, decimal balance)> exact, List<(string name, string svid)> nonExact)> GetBalance(string name)
    {
        var cache = EntityCache.Where(x => string.Equals(x.Value.Name, name, StringComparison.OrdinalIgnoreCase)).Select(x => (x.Key, x.Value.Balance));
        return !cache.Any() ? await SearchNameToBalances(name) : (cache.ToList(), new List<(string name, string svid)>());
    }

    public static async Task<string> GetName(string svid)
    {
        if (!EntityCache.TryGetValue(svid, out var values))
        {
            string results = await GetData($"Entity/GetName?svid={svid}");
            if (!results.Contains("Could not find entity"))
            {
                values.Name = results;
                AddEntityCache(svid, values.Name);
            }
        }
        return values.Name;
    }
}