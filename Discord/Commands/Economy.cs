using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Shared;
using System.Threading.Tasks;
using static Discord.DiscordTools;
using static Shared.Commands.Balance;
using static Shared.Commands.Payment;

namespace Discord.Commands
{
    public class Economy : BaseCommandModule
    {

        [Command("balance"), Aliases("balan", "bal", "b")]
        [Description("Gets a entities balance")]
        [Priority(1)]
        public async Task BalanceDiscord(CommandContext ctx, [Description("A User (works with only id)")]
            DiscordUser discordUser)
        {
            using CocaBotContext db = new();
            await ctx.RespondAsync(await BalanceAll(await DiscordToSVID(discordUser.Id, db))).ConfigureAwait(false);
        }

        [Command("balance")]
        [Priority(0)]
        public async Task Balance(CommandContext ctx, 
        [RemainingText, Description("A Entity (Either SVID, Name or if empty just you)")] string input)
        {
            if (input == null)
            {
                await BalanceDiscord(ctx, ctx.User).ConfigureAwait(false); return;
            }
            
            await ctx.RespondAsync(await BalanceAll(input)).ConfigureAwait(false);
        }

        [Command("pay"), Aliases("p", "payment")]
        [Description("Pay another enitity on SV (Requires you to link your account using /register and following the instructions)")]
        [Priority(2)]
        public async Task PayDiscord(CommandContext ctx, 
            [Description("Money to send")] decimal amount,
            [Description("The user to send the money to (works with only id)")] DiscordUser discordUser)
        {
            using CocaBotContext db = new();
            await ctx.RespondAsync(await Pay(amount, await DiscordToSVID(ctx.User.Id, db), await DiscordToSVID(discordUser.Id, db), db)).ConfigureAwait(false);
        }

        [Command("pay")]
        public async Task PayDiscord2(CommandContext ctx,
            [Description("The user to send the money to (works with only id)")] DiscordUser discordUser,
            [Description("Money to send")] decimal amount)
        {
            using CocaBotContext db = new();
            await ctx.RespondAsync(await Pay(amount, await DiscordToSVID(ctx.User.Id, db), await DiscordToSVID(discordUser.Id, db), db)).ConfigureAwait(false);
        }

        [Command("pay")]
        [Priority(0)]
        public async Task PayAll(CommandContext ctx,
            [Description("Money to send")] decimal amount, 
            [RemainingText, Description("The Entity to send the money to (Either SVID or Name)")] string to)
        {
            using CocaBotContext db = new();
            await ctx.RespondAsync(await Pay(amount, await DiscordToSVID(ctx.User.Id, db), to, db)).ConfigureAwait(false);
        }
    }
}
