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

namespace Valour;

public static class Commands
{
    [Command("confirm", "conf")]
    public static async Task Confirm(PlanetMessage ctx)
    {
        CocaBotContext db = new();
        var sender = await (await ctx.GetAuthorAsync()).GetUserAsync();

        var user = db.Users.Where(x => x.ValourName == sender.Name).FirstOrDefault();
        if (user is null) return;

        user.Valour = sender.Id;
        user.ValourName = null;
        await db.SaveChangesAsync();

        await ctx.ReplyAsync("Linked Account!");
    }

    //for testing

    //[Command("call")]
    //public static async Task Call(PlanetMessage ctx, [Remainder] string call)
    //{
    //    await ctx.ReplyAsync(call);
    //}

    //[Command("call1")]
    //public static async Task Call1(PlanetMessage ctx, string call)
    //{
    //    await ctx.ReplyAsync(call);
    //}

    //[Command("decimal")]
    //public static async Task Decimal(PlanetMessage ctx, decimal number)
    //{
    //    await ctx.ReplyAsync(number.ToString());
    //}

    //[Command("member")]
    //public static async Task Member(PlanetMessage ctx, Api.Items.Users.User member, Api.Items.Users.User member2)
    //{
    //    await ctx.ReplyAsync(member.Id + member.Name);
    //    await ctx.ReplyAsync(member2.Id + member2.Name);
    //}
}