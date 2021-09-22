using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Shared;
using System.Threading.Tasks;
using static Discord.DiscordTools;
using static Shared.Commands.Balance;

namespace Discord.Commands;
public class Balance : BaseCommandModule
{

    [Command("balance"), Aliases("balan", "bal", "b")]
    [Description("Gets a entities balance")]
    [Priority(1)]
    public async Task BalanceDiscord(CommandContext ctx, [Description("A User (works with only id)")]
            DiscordUser discordUser)
    {
        using CocaBotContext db = new();
        ctx.RespondAsync(await BalanceMessage(await DiscordToSVID(discordUser.Id, db)));
    }

    [Command("balance")]
    [Priority(0)]
    public async Task BalanceString(CommandContext ctx,
    [RemainingText, Description("A Entity (Either SVID, Name or if empty just you)")] string input)
    {
        if (input == null)
        {
            BalanceDiscord(ctx, ctx.User); return;
        }

        ctx.RespondAsync(await BalanceAll(input));
    }
}
