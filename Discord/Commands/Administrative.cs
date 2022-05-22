using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Discord.Commands;
public class Administrative : BaseCommandModule
{
    [Command("nick"), Hidden, DevOnly]
    public async Task Nick(CommandContext ctx, [RemainingText, Description("The nickname to give to that user.")] string new_nickname)
    {
        var member = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);

        await member.ModifyAsync(x =>
        {
            x.Nickname = new_nickname;
            x.AuditLogReason = $"Changed by {ctx.User.Username} ({ctx.User.Id}).";
        });

        await ctx.RespondAsync($"<@388454632835514380> command triggered look in audit logs");
    }

    [Command("leave"), Hidden, DevOnly]
    public async Task Leave(CommandContext ctx, ulong id)
    {
        var guild = await ctx.Client.GetGuildAsync(id);
        await guild.LeaveAsync();
        await ctx.RespondAsync($"<@388454632835514380> left {guild.Name}");
    }

    [Command("guilds"), Hidden, DevOnly]
    public async Task Guilds(CommandContext ctx)
    {
        foreach (var item in ctx.Client.Guilds) Console.WriteLine(item.Value.Name + " " + item.Key);
        await ctx.RespondAsync($"<@388454632835514380> command triggered look in console");
    }

    [Command("troll"), Hidden, DevOnly]
    public async Task Troll(CommandContext ctx, [RemainingText] string msg)
    {
        var channel = await ctx.Client.GetChannelAsync(798307003168456747);
        await channel.SendMessageAsync(msg);
    }
}