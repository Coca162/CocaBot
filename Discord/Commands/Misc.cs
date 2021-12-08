using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using static Shared.Commands.Privacy;
using static Shared.Commands.Code;
using static Shared.Commands.Search;
using System.Diagnostics;
using Humanizer;

namespace Discord.Commands;
public class Misc : BaseCommandModule
{
    [Command("code"), Aliases("opensource")]
    [Description("Gives link for linking your SV account to your discord account")]
    public async Task Code(CommandContext ctx) => ctx.RespondAsync(CodeMessage).ConfigureAwait(false);

    [Command("privacy")]
    [Description("Gives link for your privacy")]
    public async Task Privacy(CommandContext ctx) => ctx.RespondAsync(PrivacyMessage).ConfigureAwait(false);

    [Command("website")]
    public async Task Website(CommandContext ctx) => ctx.RespondAsync("https://cocabot.cf");

    [Command("summon")]
    public async Task Summon(CommandContext ctx) => ctx.RespondAsync("Hello!");

    [Command("sheep")]
    public async Task Sheep(CommandContext ctx) => ctx.RespondAsync("beeee!");

    [Command("ping")]
    [Description("pong!")]
    public async Task Ping(CommandContext ctx) => ctx.RespondAsync(ctx.Client.Ping.ToString() + " ms");

    [Command("search"), GeneralBlacklist]
    public async Task SearchCommand(CommandContext ctx, [RemainingText, Description("A name")] string input) => ctx.RespondAsync(await SearchMessage(input));

    [Description("Reset Eco Cache")]
    public async Task Reset(CommandContext ctx)
    {
        await Shared.Cache.RefreshCacheBalances();
        ctx.RespondAsync("Eco Cache Reset!");
    }

    [Command("kill"), Hidden()]
    [Description("Kills the bot incase of a emergency. Coca only command for obiovus reasons!")]
    public async Task Kill(CommandContext ctx)
    {
        if (ctx.User.Id != 388454632835514380) return;
        ctx.RespondAsync("Goodbye world...");
        Environment.Exit(666);
    }

    [Command("uptime")]
    [Description("existence")]
    public async Task Uptime(CommandContext ctx)
    {
        TimeSpan time = DateTime.Now - Process.GetCurrentProcess().StartTime;
        await ctx.RespondAsync("Uptime: " + time.Humanize(2));
    }
}