using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Discord.DiscordTools;
using static Shared.Commands.Balance;
using static Shared.Commands.Payment;
using static Shared.Database;

namespace Discord
{
    public class Economy : BaseCommandModule
    {
        [Command("balance")]
        [Aliases("balan", "bal", "b")]
        [Priority(1)]
        public async Task BalanceDiscord(CommandContext ctx, DiscordUser discordUser)
        { await ctx.RespondAsync(await BalanceAll(await DiscordToSVID(discordUser.Id))).ConfigureAwait(false); }

        [Command("balance")]
        [Priority(0)]
        public async Task Balance(CommandContext ctx, [RemainingText] string input)
        {
            if (input == null)
            {
                await BalanceDiscord(ctx, ctx.User).ConfigureAwait(false); return;
            }
            
            await ctx.RespondAsync(await BalanceAll(input)).ConfigureAwait(false);
        }

        [Command("pay")]
        [Aliases("p", "payment")]
        [Priority(2)]
        public async Task PayDiscord(CommandContext ctx, decimal amount, DiscordUser discordUser)
        {
            await ctx.RespondAsync(await Shared.Commands.Payment.Pay(amount, await DiscordToSVID(ctx.User.Id), await DiscordToSVID(discordUser.Id), 
                                   Platform.Discord, ctx.User.Id)).ConfigureAwait(false); }

        [Command("pay")]
        [Priority(1)]
        public async Task Pay(CommandContext ctx, decimal amount, string from, string to)
        { await ctx.RespondAsync(await Shared.Commands.Payment.Pay(amount, from, to, Platform.Discord, ctx.User.Id)).ConfigureAwait(false); }

        [Command("pay")]
        [Priority(0)]
        public async Task Pay(CommandContext ctx, decimal amount, [RemainingText] string to)
        { await ctx.RespondAsync(await Shared.Commands.Payment.Pay(amount, await DiscordToSVID(ctx.User.Id), to, 
                                 Platform.Discord, ctx.User.Id)).ConfigureAwait(false);}
    }
}
