using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Shared;
using System.Threading.Tasks;
using static Discord.DiscordTools;
using static Shared.Commands.Payment;

namespace Discord.Commands;
public class Pay : BaseCommandModule
{
    [Command("pay"), Aliases("p", "payment")]
    [Description("Pay another enitity on SV (Requires you to link your account using c/register and following the instructions)")]
    [Priority(3)]
    public async Task PayDiscord(CommandContext ctx,
        [Description("Money to send")] decimal amount,
        [Description("The user to send the money to (works with ID as well)")] DiscordUser discordUser)
    {
        using CocaBotContext db = new();
        ctx.RespondAsync(await PayAll(amount, await DiscordToSVID(ctx.User.Id, db), await DiscordToSVID(discordUser.Id, db), db));
    }

    [Command("pay")]
    [Priority(2)]
    public async Task PayDiscord2(CommandContext ctx,
        [Description("The user to send the money to (works with ID as well)")] DiscordUser discordUser,
        [Description("Money to send")] decimal amount)
    {
        using CocaBotContext db = new();
        ctx.RespondAsync(await PayAll(amount, await DiscordToSVID(ctx.User.Id, db), await DiscordToSVID(discordUser.Id, db), db));
    }

    [Command("pay")]
    [Priority(1)]
    public async Task PaySVIDFirst(CommandContext ctx,
        [Description("The SVID to send the money to")] string to,
        [Description("Money to send")] decimal amount)
    {
        using CocaBotContext db = new();
        ctx.RespondAsync(await PaySVID(amount, await DiscordToSVID(ctx.User.Id, db), to, db));
    }

    [Command("pay")]
    [Priority(0)]
    public async Task PayAmountFirst(CommandContext ctx,
        [Description("Money to send")] decimal amount,
        [RemainingText, Description("The Entity to send the money to (Either SVID or Name)")] string to)
    {
        using CocaBotContext db = new();
        ctx.RespondAsync(await PayAll(amount, await DiscordToSVID(ctx.User.Id, db), to, db));
    }
}
