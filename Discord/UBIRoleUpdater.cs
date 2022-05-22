using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Discord;
using static Shared.HttpClientExtensions;
using static Discord.Program;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Net.Http;
using Shared;
using Shared.Models;

namespace Discord;

//TODO: Move this to Shared. All this is pure logic and shouldn't be in Discord apart from RankNamesToIds
public class UBIRoleUpdater
{
    private readonly IUbiRoles<ulong> _manager;

    public UBIRoleUpdater(IUbiRoles<ulong> manager)
    {
        _manager = manager;
    }

    public async Task UpdateHourly()
    {
        static async Task<IAsyncEnumerable<UbiUser>> GetData()
        {
            while (true)
            {
                try
                {
                    HttpClient httpClient = new();
                    var response = await httpClient.GetStreamAsync($"https://ubi.vtech.cf/all_user_data?key={UBIKey}");
                    return JsonSerializer.DeserializeAsyncEnumerable<UbiUser>(response);
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("Jacob did a stupid! ubi.vtech.cf is down!");
                    await Task.Delay(60000);
                    continue;
                }
            }
        }

        IAsyncEnumerable<UbiUser> data = await GetData();

        await foreach (UbiUser user in data)
        {
            (bool success, IUbiMember<ulong> member) = await _manager.TryGetMemberAsync(ulong.Parse(user.DiscordId));
            if (!success)
                continue;

            await AssignOnlyCorrectUbiRole(member, user.Rank);
        }
    }

    //Put these in DiscordUbiMember as static methods???
    public async Task AssignOnlyCorrectUbiRole(IUbiMember<ulong> member, string roleName)
    {
        if (roleName == "unranked")
        {
            await RemoveRoles(member, _manager.RankNamesToIds.Values);
            return;
        }

        await GrantRoleAsync(member, roleName);
    }


    private async Task GrantRoleAsync(IUbiMember<ulong> member, string roleName)
    {
        ulong roleId = _manager.RankNamesToIds[roleName];

        await RemoveOtherUbiRoles(member, roleId);

        if (IsRoleAlreadyGranted(member.Roles, roleId))
        {
            await member.GrantRoleAsync(roleId);
        }
    }

    private static bool IsRoleAlreadyGranted(IEnumerable<ulong> roles, ulong roleId) =>
        !roles.Contains(roleId);

    private async Task RemoveOtherUbiRoles(IUbiMember<ulong> member, ulong roleId)
    {
        var onlyOtherRoles = _manager.RankNamesToIds.Values.Where(ubiRoles => ubiRoles != roleId);
        await RemoveRoles(member, onlyOtherRoles);
    }

    private static async Task RemoveRoles(IUbiMember<ulong> member, IEnumerable<ulong> ToRemove)
    {
        List<ulong> rolesThatUserHas = member.Roles.Intersect(ToRemove)
                                                   .ToList(); //DO NOT REMOVE THIS. THIS CAUSES ASYNC ISSUES WHEN REVOKING
        
        await member.RevokeRangeAsync(rolesThatUserHas);
    }
}