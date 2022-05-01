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
    public CocaBotPoolContext db { private get; set; }

    [Command("balance"), Aliases("balan", "bal", "b")]
    [Description("Gets a entities balance")]
    [Priority(1)]
    public async Task BalanceDiscord(CommandContext ctx, [Description("A User (works with only id)")]
            DiscordUser discordUser)
    {
        string svid = await DiscordToSVID(discordUser.Id, db);

        if (svid is null)
        {
            await ctx.RespondAsync("The person does not have their account registered!");
            return;
        }

        await ctx.RespondAsync(await BalanceMessage(svid));
    }

    [Command("balance")]
    [Priority(0)]
    public async Task BalanceString(CommandContext ctx,
    [RemainingText, Description("A Entity (Either SVID, Name or if empty just you)")] string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            await BalanceDiscord(ctx, ctx.User);
            return;
        }

        await ctx.RespondAsync(await BalanceAll(input));
    }
}
