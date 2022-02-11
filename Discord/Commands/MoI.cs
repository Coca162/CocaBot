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
    [Command("list")]
    public async Task List(CommandContext ctx)
    {
        StringBuilder sb = new("MoI new member helpers:");

        foreach (ulong id in Helpers)
        {
            DiscordMember member = null;
            try
            {
                member = await ctx.Guild.GetMemberAsync(id);
            }
            catch (DSharpPlus.Exceptions.NotFoundException) { }

            sb.Append('\n');
            sb.Append(member.Username);
            sb.Append('#');
            sb.Append(member.Discriminator);
        }

        await ctx.RespondAsync(sb.ToString());
    }
}
