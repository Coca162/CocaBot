using System.Linq;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;
using Valour.Net.CommandHandling.Attributes;
using static Shared.Commands.Balance;
using static Shared.Main;
using static Shared.Commands.Payment;
using static Shared.Database;
using static Valour.ValourTools;
using Valour.Net.Models;
using Shared;

namespace Valour.Commands
{
    public class Economy : CommandModuleBase
    {
        //[Command("balance"), Alias("balan", "bal", "b")]
        //public async Task BalanceUser(CommandContext ctx, ValourUser user)
        //{
        //    using (CocaBotContext db = new())
        //        await ctx.ReplyAsync(await BalanceAll(await ValourToSVID(user, db))).ConfigureAwait(false);
        //}

        [Command("balance"), Alias("balan", "bal", "b")]
        public async Task Balance(CommandContext ctx, [Remainder] string input) => await ctx.ReplyAsync(await BalanceAll(input)).ConfigureAwait(false);

        [Command("balance"), Alias("balan", "bal", "b")]
        public async Task BalanceSelf(CommandContext ctx) => await ctx.ReplyAsync(await BalanceAll(ctx.Member.Nickname)).ConfigureAwait(false);

        //[Command("pay"), Alias("p", "payment")]
        //public async Task PayUser(CommandContext ctx, decimal amount, ValourUser user)
        //{
        //    using CocaBotContext db = new();
        //    string from = await GetString(String.SVID, ctx.Member.User_Id, db);
        //    if (from == null)
        //    {
        //        await ctx.ReplyAsync($"You are not connected! Do `c/valour connect {ctx.Member.Nickname}` on discord then `c/confirm` on valour and try again!").ConfigureAwait(false);
        //        return;
        //    }
            
        //    await ctx.ReplyAsync(await Pay(amount, from, await ValourToSVID(user, db), db)).ConfigureAwait(false);
        //}

        //[Command("pay"), Alias("p", "payment")]
        //public async Task PayUser2(CommandContext ctx, ValourUser user, decimal amount) => await PayUser(ctx, amount, user).ConfigureAwait(false);

        //[Command("pay"), Alias("p", "payment")]
        //public async Task PayAll(CommandContext ctx, decimal amount, ulong to)
        //{
        //    using CocaBotContext db = new();
        //    string from = await GetString(String.SVID, ctx.Member.User_Id, db);
        //    if (from == null)
        //    {
        //        await ctx.ReplyAsync($"You are not connected! Do `c/valour connect {ctx.Member.Nickname}` on discord then `c/confirm` on valour and try again!").ConfigureAwait(false);
        //        return;
        //    }

        //    await ctx.ReplyAsync(await Pay(amount, from, db.Users.Where(x => x.Discord == to).First().SVID, db)).ConfigureAwait(false);
        //}

        [Command("pay"), Alias("p", "payment")]
        public async Task PayAll(CommandContext ctx, decimal amount, [Remainder] string to)
        {
            using CocaBotContext db = new();
            string from = await GetString(String.SVID, ctx.Member.User_Id, db);
            if (from == null)
            {
                await ctx.ReplyAsync($"You are not connected! Do `c/valour connect {ctx.Member.Nickname}` on discord (to find out where CocaBot is located do c/discord) then `c/confirm` on valour and try again!").ConfigureAwait(false);
                return;
            }

            await ctx.ReplyAsync(await Pay(amount, from, to, db)).ConfigureAwait(false);
        }
    }
}
