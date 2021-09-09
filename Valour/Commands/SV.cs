using ProfanityFilter;
using Shared;
using SpookVooper.Api.Entities;
using System;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;
using Valour.Net.CommandHandling.Attributes;
using static Shared.Commands.Get;
using static Shared.Database;

namespace Valour.Commands;
public class SV : CommandModuleBase
{
    [Command("get"), Alias("g", "grab", "svid", "name")]
    public async Task Get(CommandContext ctx, [Remainder] string input)
    {
        if (input == null)
        {
            await ctx.ReplyAsync(await GetAll(ctx.Member.Nickname)).ConfigureAwait(false);
        }

        await ctx.ReplyAsync(await GetAll(input)).ConfigureAwait(false);
    }

    [Command("confirm"), Alias("conf")]
    public async Task Verify(CommandContext ctx)
    {
        using (CocaBotContext db = new())
            if (!await ValourConnect(ctx.Member.Nickname, ctx.Member.User_Id, db))
            {
                await ctx.ReplyAsync($"Could not find name to confirm in db. Retry `c/valour connect {ctx.Member.Nickname}` in discord (to find out where CocaBot is located do c/discord)").ConfigureAwait(false);
                return;
            }
        await ctx.ReplyAsync("Linked account!").ConfigureAwait(false);
    }
}
