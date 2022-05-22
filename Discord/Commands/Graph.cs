using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Discord.Commands;

[Group("graph"), Description("Get's graphs. Default is XP")]
public class Graphs : BaseCommandModule
{
    [GroupCommand, Priority(1)]
    public async Task GraphDefault(CommandContext ctx, DiscordUser user)
        => await GraphXp(ctx, user);


    [GroupCommand, Priority(0)]
    public async Task GraphDefault(CommandContext ctx)
        => await GraphDefault(ctx, ctx.User);


    [Command("xp"), Priority(1)]
    public async Task GraphXp(CommandContext ctx, DiscordUser user)
        => await ctx.RespondAsync($"https://ubi.vtech.cf/graph.png?userid={user.Id}&type=xp");

    [Command("xp"), Priority(0)]
    public async Task GraphXp(CommandContext ctx)
        => await GraphXp(ctx, ctx.User);


    [Command("messages"), Priority(1)]
    public async Task GraphMessages(CommandContext ctx, DiscordUser user)
        => await ctx.RespondAsync($"https://ubi.vtech.cf/graph.png?userid={user.Id}&type=messages");

    [Command("messages"), Priority(0)]
    public async Task GraphMessages(CommandContext ctx)
        => await GraphMessages(ctx, ctx.User);
}