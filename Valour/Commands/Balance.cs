using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Valour.Api.Items.Messages;
using ValourSharp;
using Valour.Api.Items.Planets.Members;
using Shared;
using ValourSharp.Attributes;
using Valour.Api.Items.Users;
using static Valour.ValourTools;
using static Shared.Commands.Balance;

namespace Valour.Commands;

public class Balance : BaseCommandModule
{
    [Command("balance"), Aliases("balan", "bal", "b")]
    [Priority(1)]
    public async Task BalanceUser(PlanetMessage ctx, User discordUser)
    {
        await using CocaBotContext db = new();
        string svid = await ValourToSVID(discordUser.Id, db);

        if (svid is null)
        {
            await ctx.ReplyAsync("The person does not have their account registered!");
            return;
        }

        await ctx.ReplyAsync(await BalanceMessage(svid));
    }

    [Command("balance")]
    [Priority(0)]
    public async Task BalanceString(PlanetMessage ctx, [Remainder] string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            await BalanceUser(ctx, await ctx.GetAuthorUserAsync());
            return;
        }

        await ctx.ReplyAsync(await BalanceAll(input));
    }
}
