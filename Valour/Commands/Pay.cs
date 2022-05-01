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
using static Shared.Commands.Payment;

namespace Valour.Commands;

public class Pay : BaseCommandModule
{
    public CocaBotPoolContext db { private get; set; }

    [Command("pay"), Aliases("p", "payment")]
    [Priority(4)]
    public async Task PayUserLast(PlanetMessage ctx, decimal amount, User discordUser)
    {
        await using CocaBotContext db = new();
        var user = await ctx.GetAuthorUserAsync();
        var (svid, token) = await ValourToUserToken(user.Id, db);

        string message = svid is not null ? await PayAll(amount, svid, await ValourToSVID(discordUser.Id, db), token) 
                                          : "You do not have a account!";

        await ctx.ReplyAsync(message);
    }

    [Command("pay")]
    [Priority(3)]
    public async Task PayUserFirst(PlanetMessage ctx, User discordUser, decimal amount)
    {
        await using CocaBotContext db = new();
        var user = await ctx.GetAuthorUserAsync();
        var (svid, token) = await ValourToUserToken(user.Id, db);

        string message = svid is not null ? await PayAll(amount, svid, await ValourToSVID(discordUser.Id, db), token) 
                                          : $"You do not have a account! Do `;valour connect {user.Name}` on discord";

        await ctx.ReplyAsync(message);
    }

    [Command("pay")]
    [Priority(2)]
    public async Task PaySVIDFirst(PlanetMessage ctx, string to, decimal amount)
    {
        await using CocaBotContext db = new();
        var user = await ctx.GetAuthorUserAsync();
        var (svid, token) = await ValourToUserToken(user.Id, db);

        string message = svid is not null ? await PayAll(amount, svid, to, token)
                                          : $"You do not have a account! Do `;valour connect {user.Name}` on discord";

        await ctx.ReplyAsync(message);
    }

    [Command("pay")]
    [Priority(1)]
    public async Task PayAmountFirst(PlanetMessage ctx, decimal amount, [Remainder] string to)
    {
        await using CocaBotContext db = new();
        var user = await ctx.GetAuthorUserAsync();
        var (svid, token) = await ValourToUserToken(user.Id, db);

        string message = svid is not null ? await PayAll(amount, svid, to, token)
                                          : $"You do not have a account! Do `;valour connect {user.Name}` on discord";

        await ctx.ReplyAsync(message);
    }

    [Command("pay")]
    [Priority(0)]
    public async Task PayAmountSecond(PlanetMessage ctx, [Remainder] string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;

        IEnumerable<string> array = input.Split(" ");

        string amount = array.Last();
        if (!decimal.TryParse(amount, out _)) return;

        string message = $"Did you mean to do?\n`c/pay {amount} {string.Join(" ", array.SkipLast(1))}`";

        await ctx.ReplyAsync(message);
    }
}
