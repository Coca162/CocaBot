using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Discord.Events.MemberJoinEvents;

namespace Discord.Commands;

[Group("moi")] // let's mark this class as a command group
public class MoI : BaseCommandModule
{
    private static readonly List<ulong> EditPerms = new()
    {
        388454632835514380, //Coca
        553041789976838154, //Snov
        702313549451755671, //Suth
    };

    [Command("list")]
    public async Task List(CommandContext ctx)
    {
        StringBuilder sb = new("MoI new member helpers:", 13 * Helpers.Count + 23);

        foreach (ulong id in Helpers)
        {
            DiscordMember member;
            try
            {
                member = await ctx.Guild.GetMemberAsync(id);
            }
            catch (DSharpPlus.Exceptions.NotFoundException) {
                await Helpers.Remove(id);
                continue; 
            }

            sb.Append('\n');
            sb.Append(member.Username);
            sb.Append('#');
            sb.Append(member.Discriminator);
        }

        await ctx.RespondAsync(sb.ToString());
    }

    [Command("add")]
    public async Task Add(CommandContext ctx, DiscordUser user)
    {
        if (!EditPerms.Contains(ctx.User.Id)) return;
        ulong id = user.Id;

        if (Helpers.Contains(id))
        {
            await ctx.RespondAsync("Already a helper!");
            return;
        }

        await Helpers.Add(id);
        await ctx.RespondAsync("Added helper!");
    }

    [Command("remove"), Priority(1)]
    public async Task Remove(CommandContext ctx, DiscordUser user)
    {
        if (EditPerms.Contains(ctx.User.Id)) 
            await Remove(ctx, user.Id);
    }

    [Command("remove"), Priority(0)]
    public async Task Remove(CommandContext ctx) => 
        await Remove(ctx, ctx.User.Id);

    public async Task Remove(CommandContext ctx, ulong id)
    {
        if (!Helpers.Contains(id))
        {
            await ctx.RespondAsync("They are not a helper!");
            return;
        }

        await Helpers.Remove(id);
        await ctx.RespondAsync("Removed helper!");
    }
}
