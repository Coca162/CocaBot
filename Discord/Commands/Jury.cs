using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Shared;
using System.Threading.Tasks;
using static Discord.DiscordTools;
using static Shared.Commands.Balance;
using System.Text.Json.Serialization;
using static SpookVooper.Api.SpookVooperAPI;
using System.Collections.Generic;
using System;
using System.Linq;
using static Discord.Program;
using static System.Math;

namespace Discord.Commands;
public class Jury : BaseCommandModule
{
    private static bool gotRoles = false;

    private static DiscordRole Supreme;
    private static DiscordRole Imperial;
    private static DiscordRole District;

    [Command("jury"), Description("Get 4 jurors")]
    public async Task JuryCommand(CommandContext ctx) 
        => await JuryCommand(ctx, 4);

    [Command("jury"), Description("Get the amount of jurors you want")]
    public async Task JuryCommand(CommandContext ctx, int amount)
    {
        GetRoles(ctx);

        var roles = ctx.Member.Roles;

        if (!(roles.Contains(Supreme) || roles.Contains(Imperial) || roles.Contains(District)) 
            && ctx.User.Id != 388454632835514380)
        {
            await ctx.RespondAsync("You are not a judge!");
            return;
        }

        var everyone = (await GetDataFromJson<JacobHourlyUserData>($"https://ubi.vtech.cf/all_user_data?key={UBIKey}")).Users;

        List<(ulong id, int xp)> filtered = new();

        foreach (var item in everyone)
        {
            if (item.Roles.Contains("Jury"))
                filtered.Add((item.Id, 100 + (int)Pow(item.Xp, 0.5)));
        }

        if (filtered.Count < amount) throw new Exception("Too big jury request!");


        List<(ulong id, int xp)> finals = new();

        while (finals.Count != amount)
        {
            Random rnd = new();
            int number = rnd.Next(0, everyone.Sum(x => x.Xp));

            int xpTotal = 0;
            foreach (var item in filtered)
            {
                xpTotal += item.xp;
                if (xpTotal > number)
                {
                    finals.Add(item);
                    filtered.Remove(item);
                    break;
                }
            }
        }

        string message = $"Selected {amount} judges to be picked!";

        foreach (var (id, xp) in finals)
        {
            DiscordMember member = null;
            try
            {
                member = await ctx.Guild.GetMemberAsync(id);
            }
            catch (DSharpPlus.Exceptions.NotFoundException) { }

            message += $"\n{member.Username}#{member.Discriminator} ({xp} Scaled XP)";
        }

        await ctx.RespondAsync(message);
    }

    private static void GetRoles(CommandContext ctx)
    {
        if (gotRoles == false)
        {
            Supreme = ctx.Guild.GetRole(798365091435249674);
            Imperial = ctx.Guild.GetRole(798367610461749259);
            District = ctx.Guild.GetRole(800088731355971585);
            gotRoles = true;
        }
    }
}