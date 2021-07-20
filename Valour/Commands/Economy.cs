using ProfanityFilter;
using SpookVooper.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;
using Valour.Net.CommandHandling.Attributes;
using static Shared.Tools;
using static Shared.Commands.Balance;
using static Shared.Main;
using static Shared.Commands.Payment;
using static Shared.Database;

namespace Valour.Commands
{
    public class Economy : CommandModuleBase
    {
        [Command("balance"), Alias("balan", "bal", "b")]
        public async Task Balance(CommandContext ctx, [Remainder] string input)
        {
            if (input == null)
            {
                await ctx.ReplyAsync(await BalanceAll(ctx.Member.Nickname)).ConfigureAwait(false);
            }

            await ctx.ReplyAsync(await BalanceAll(input)).ConfigureAwait(false);
        }

        [Command("pay"), Alias("p", "payment")]
        public async Task PayAll(CommandContext ctx, decimal amount, [Remainder] string to)
        {
            /*
            var from = DiscordToSVID(ctx.User.Id);
            var token = DiscordToToken(ctx.User.Id);

            await Task.WhenAll(from, token);
            
            await ctx.RespondAsync(await Shared.Commands.Payment.Pay(amount, await from, to, 
                                 Platform.Valour, ctx.User.Id, await token)).ConfigureAwait(false);
            */

            await ctx.ReplyAsync(await Pay(amount, await GetSVID(Platform.Valour, ctx.Member.User_Id), to,
                     Platform.Valour, ctx.Member.User_Id)).ConfigureAwait(false);
        }
    }
}
