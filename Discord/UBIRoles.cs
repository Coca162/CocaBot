using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static SpookVooper.Api.SpookVooperAPI;
using Discord;
using static Discord.Bot;
using static Discord.Program;

public static class UBIRoles
{
    public static async Task UpdateHourly()
    {
        JacobHourlyUserData data = null;

        while (true)
        {
            try
            {
                data = await GetDataFromJson<JacobHourlyUserData>($"https://ubi.vtech.cf/all_user_data?key={UBIKey}");
                break;
            }
            catch
            {
                // wait 5s before trying again
                Console.WriteLine("Jacob did a stupid! ubi.vtech.cf is down!");
                await Task.Delay(5000);
                continue;
            }
        }

        DiscordGuild server = await Client.GetGuildAsync(798307000206360588);

        List<DiscordRole> SVRoles = new()
        {
            server.GetRole(894632235326656552),
            server.GetRole(894632423776731157),
            server.GetRole(894632541552791632),
            server.GetRole(894632641477894155),
            server.GetRole(894632682330423377)
        };

        foreach (var item in data.Users)
        {
            Console.WriteLine(item.Id);
            DiscordMember member = null;
            try
            {
                member = await server.GetMemberAsync(item.Id);
            }
            catch (DSharpPlus.Exceptions.NotFoundException e)
            {
                continue;
            }

            if (member == null)
            {
                continue;
            }

            // check if unranked

            if (item.Rank == "Unranked")
            {
                bool HasRole = false;
                DiscordRole RoleToRemove = null;
                foreach (var role in SVRoles.Where(role => member.Roles.Contains(role)))
                {
                    HasRole = true;
                    RoleToRemove = role;
                    break;
                }

                if (HasRole) await member.RevokeRoleAsync(RoleToRemove);
                continue;
            }
            else
            {
                foreach (var role in SVRoles.Where(role => member.Roles.Contains(role) && role.Name != item.Rank))
                {
                    await member.RevokeRoleAsync(role);
                    break;
                }

                DiscordRole ToHave = SVRoles.Find(x => x.Name == item.Rank);
                if (!member.Roles.Contains(ToHave)) await member.GrantRoleAsync(ToHave);
            }
        }
    }
}

public class JacobHourlyUser
{
    [JsonPropertyName("Id")]
    public ulong Id { get; set; }

    [JsonPropertyName("Rank")]
    public string Rank { get; set; }

    [JsonPropertyName("Xp")]
    public int Xp { get; set; }

    [JsonPropertyName("Message Xp")]
    public int MessageXp { get; set; }

    [JsonPropertyName("Messages")]
    public int Messages { get; set; }

    [JsonPropertyName("Daily UBI")]
    public int DailyUBI { get; set; }
}

public class JacobHourlyUserData
{
    [JsonPropertyName("Users")]
    public List<JacobHourlyUser> Users { get; set; }
}