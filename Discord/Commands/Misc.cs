using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using static Discord.DiscordTools;
using static Shared.Commands.Get;
using static Shared.Commands.Privacy;
using static Shared.Commands.Verify;
using static Shared.Commands.Code;
using static Shared.Main;
using Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Diagnostics;
using Humanizer;

namespace Discord.Commands;
public class Misc : BaseCommandModule
{
    public static Random random = new();
    public const string chars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890-._~";

    [Command("get"), Aliases("g", "grab", "svid", "name")]
    [Description("Gets basic information about a entity")]
    [Priority(1)]
    public async Task GetDiscord(CommandContext ctx, [Description("A User (works with only id)")] DiscordUser discordUser)
    {
        string discord;
        using (CocaBotContext db = new())
            discord = await DiscordToSVID(discordUser.Id, db);
        if (discord != "") ctx.RespondAsync(await GetSVID(discord));
        else ctx.RespondAsync(await GetAll(discordUser.Username));
    }

    [Command("get")]
    [Priority(0)]
    public async Task Get(CommandContext ctx,
        [RemainingText, Description("A Entity (Either SVID, Name or if empty just you)")] string input)
    {
        if (input == null)
        {
            GetDiscord(ctx, ctx.User); return;
        }

        ctx.RespondAsync(await GetAll(input));
    }

    [Command("register"), Aliases("reg", "login")]
    [Description("Gives link for linking your SV account to your discord account")]
    public async Task Register(CommandContext ctx)
    {
        string key = new(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());

        DiscordDmChannel dms;
        try
        {
            dms = await ctx.Member.CreateDmChannelAsync();
        }
        catch (NullReferenceException)
        {
            ctx.RespondAsync("Cannot do this command in dms!");
            return;
        }
        try
        {
            await dms.SendMessageAsync("https://cocabot.cf/login?key=" + key);
        }
        catch (DSharpPlus.Exceptions.UnauthorizedException)
        {
            ctx.RespondAsync("Please enable direct messages from server members in the server privacy seetings!");
            return;
        }
        await ctx.RespondAsync("Look at your DM!");

        using CocaBotContext db = new();
        Registers register = db.Registers.Where(x => x.Discord == ctx.User.Id).FirstOrDefault();
        if (register != null) db.Registers.Remove(register);

        register = new();
        register.VerifKey = key;
        register.Discord = ctx.User.Id;

        await db.Registers.AddAsync(register);
        await db.SaveChangesAsync();
    }

    [Command("code"), Aliases("opensource")]
    [Description("Gives link for linking your SV account to your discord account")]
    public async Task Code(CommandContext ctx) => await ctx.RespondAsync(CodeMessage).ConfigureAwait(false);

    [Command("privacy")]
    [Description("Gives link for your privacy")]
    public async Task Privacy(CommandContext ctx) => await ctx.RespondAsync(PrivacyMessage).ConfigureAwait(false);

    [Command("website")]
    public async Task Website(CommandContext ctx) => await ctx.RespondAsync("https://cocabot.cf");

    [Command("kill"), Hidden()]
    [Description("Kills the bot incase of a emergency. Coca only command for obiovus reasons!")]
    public async Task Kill(CommandContext ctx)
    {
        if (ctx.User.Id == 388454632835514380)
        {
            Environment.Exit(666);
        }
    }

    [Command("ping")]
    [Description("pong!")]
    public async Task Ping(CommandContext ctx)
    {
        await ctx.RespondAsync(ctx.Client.Ping.ToString() + " ms");
    }

    [Command("uptime")]
    [Description("existence")]
    public async Task Uptime(CommandContext ctx)
    {
        TimeSpan time = DateTime.Now - Process.GetCurrentProcess().StartTime;
        await ctx.RespondAsync("Uptime: " + time.Humanize(2));
    }
}

[Group("valour")] // let's mark this class as a command group
[Description("Valour related commands")] // give it a description for help purposes
public class Valour : BaseCommandModule
{
    [Command("connect"), Aliases("link")]
    [Description("Connects your valour account so that it can do /pay and self /balance")]
    public async Task Connect(CommandContext ctx, [Description("Valour Name")] string name)
    {
        using (CocaBotContext db = new())
        {
            if (!await Database.ValourName(ctx.User.Id, name, db))
            {
                await ctx.RespondAsync("Your discord account is not linked to a SV account! Do c/register first!").ConfigureAwait(false);
                return;
            }
        }
        await ctx.RespondAsync("Do `c/confirm` on your valour account to complete connection process!").ConfigureAwait(false);
    }

    [Command("disconnect"), Aliases("unlink")]
    [Description("Removes valour name and valour id from db.")]
    public async Task Disconnect(CommandContext ctx)
    {
        using (CocaBotContext db = new())
        {
            if (!await Database.ValourDisconnect(ctx.User.Id, db))
            {
                await ctx.RespondAsync("Your discord account is not linked to a SV account! Do c/register first and then c/connect then you can do this command!").ConfigureAwait(false);
                return;
            }
        }
        await ctx.RespondAsync("Your Valour accounts have been wiped").ConfigureAwait(false);
    }
}
