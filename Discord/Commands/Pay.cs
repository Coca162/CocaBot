using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Discord.DiscordTools;
using static Shared.Commands.Payment;

namespace Discord.Commands;
public class Pay : BaseCommandModule
{
    public CocaBotPoolContext db { private get; set; }

    [Command("pay"), Aliases("p", "payment")]
    [Description("Pay another enitity on SV (Requires you to link your account using c/register and following the instructions)")]
    [Priority(4)]
    public async Task PayDiscord(CommandContext ctx,
        [Description("Money to send")] decimal amount,
        [Description("The user to send the money to (works with ID as well)")] DiscordUser discordUser)
    {
        var (svid, token) = await DiscordToUserToken(ctx.User.Id, db);

        string message = svid is not null ? await PayAll(amount, svid, await DiscordToSVID(discordUser.Id, db), token) 
                                          : "You do not have a account!";

        await ctx.RespondAsync(message);
    }

    [Command("pay")]
    [Priority(3)]
    public async Task PayDiscord2(CommandContext ctx,
        [Description("The user to send the money to (works with ID as well)")] DiscordUser discordUser,
        [Description("Money to send")] decimal amount)
    {
        var (svid, token) = await DiscordToUserToken(ctx.User.Id, db);

        string message = svid is not null ? await PayAll(amount, svid, await DiscordToSVID(discordUser.Id, db), token) 
                                          : "You do not have a account!";

        await ctx.RespondAsync(message);
    }

    [Command("pay")]
    [Priority(2)]
    public async Task PaySVIDFirst(CommandContext ctx,
        [Description("The SVID to send the money to")] string to,
        [Description("Money to send")] decimal amount)
    {
        var (svid, token) = await DiscordToUserToken(ctx.User.Id, db);

        string message = svid is not null ? await PayAll(amount, svid, to, token) 
                                          : "You do not have a account!";

        await ctx.RespondAsync(message);
    }

    [Command("pay")]
    [Priority(1)]
    public async Task PayAmountFirst(CommandContext ctx,
        [Description("Money to send")] decimal amount,
        [RemainingText, Description("The Entity to send the money to (Either SVID or Name)")] string to)
    {
        var (svid, token) = await DiscordToUserToken(ctx.User.Id, db);

        string message = svid is not null ? await PayAll(amount, svid, to, token) 
                                          : "You do not have a account!";

        await ctx.RespondAsync(message);
    }

    [Command("pay")]
    [Priority(0)]
    public async Task PayAmountSecond(CommandContext ctx,
    [RemainingText, Description("The Entity to send the money to (Either SVID or Name) and the amount at the end")] string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            await ExecuteCommand(ctx, ctx.Member, "help pay");
            return;
        }

        IEnumerable<string> array = input.Split(" ");

        string amount = array.Last();
        if (!decimal.TryParse(amount, out _)) return;

        string message = $"Did you mean to do?\n`c/pay {amount} {string.Join(" ", array.SkipLast(1))}`";

        await ctx.RespondAsync(message);
    }

    private static async Task ExecuteCommand(CommandContext ctx, DiscordMember member, string command)
    {
        var cmd = ctx.CommandsNext.FindCommand(command, out var customArgs);

        var fakeContext = ctx.CommandsNext.CreateFakeContext(member, ctx.Channel, command, ctx.Prefix, cmd, customArgs);

        await ctx.CommandsNext.ExecuteCommandAsync(fakeContext);
    }
}
