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
using System.Diagnostics;
using Humanizer;
using Valour.Api.Items.Users;
using static Shared.Commands.Privacy;
using static Shared.Commands.Code;
using static Shared.Commands.Search;

namespace Valour.Commands;

public class Misc : BaseCommandModule
{
    [Command("code"), Aliases("opensource")]
    public async Task Code(PlanetMessage ctx)
        => await ctx.ReplyAsync(CodeMessage).ConfigureAwait(false);

    [Command("privacy")]
    public async Task Privacy(PlanetMessage ctx)
        => await ctx.ReplyAsync(PrivacyMessage).ConfigureAwait(false);

    [Command("website")]
    public async Task Website(PlanetMessage ctx)
        => await ctx.ReplyAsync("https://cocabot.cf");

    [Command("summon")]
    public async Task Summon(PlanetMessage ctx)
        => await ctx.ReplyAsync("Hello!");

    [Command("sheep")]
    public async Task Sheep(PlanetMessage ctx)
        => await ctx.ReplyAsync("beeee!");

    [Command("search")]
    public async Task SearchCommand(PlanetMessage ctx, [Remainder] string input)
        => await ctx.ReplyAsync(await SearchMessage(input));

    [Command("uptime")]
    public async Task Uptime(PlanetMessage ctx)
    {
        TimeSpan time = DateTime.Now - Process.GetCurrentProcess().StartTime;
        await ctx.ReplyAsync("Uptime: " + time.Humanize(2));
    }

    [Command("kill")]
    public async Task Kill(PlanetMessage ctx)
    {
        User user = await ctx.GetAuthorUserAsync();
        if (user.Id != 735182334984219) return;
        await ctx.ReplyAsync("Goodbye world...");
        Environment.Exit(666);
    }

    [Command("confirm"), Aliases("conf")]
    public async Task Confirm(PlanetMessage ctx)
    {
        await using CocaBotContext db = new();
        var sender = await ctx.GetAuthorUserAsync();

        var user = db.Users.Where(x => x.ValourName == sender.Name).SingleOrDefault();
        if (user is null)
        {
            await ctx.ReplyAsync($"Could not find account! Have you done `;valour disconnect {sender.Name}`?");
            return;
        }

        user.Valour = sender.Id;
        user.ValourName = null;
        await db.SaveChangesAsync();

        await ctx.ReplyAsync("Linked Account!");
    }
}