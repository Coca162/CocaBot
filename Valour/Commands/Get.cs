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
using static Shared.Commands.Get;
using static Valour.ValourTools;

namespace Valour.Commands;

public class Get : BaseCommandModule
{
    [Command("get"), Aliases("g", "grab", "svid", "name")]
    [Priority(1)]
    public async Task GetValour(PlanetMessage ctx, User user)
    {
        await using CocaBotContext db = new();
        string valour = await ValourToSVID(user.Id, db);

        if (valour != "") 
            await ctx.ReplyAsync(await GetSVID(valour));
        else 
            await ctx.ReplyAsync(await GetAll(user.Name));
    }

    [Command("get")]
    [Priority(0)]
    public async Task GetString(PlanetMessage ctx, [Remainder] string input)
    {
        if (string.IsNullOrWhiteSpace(input)) 
        { 
            await GetValour(ctx, await ctx.GetAuthorUserAsync()); 
            return; 
        }

        await ctx.ReplyAsync(await GetAll(input));
    }
}