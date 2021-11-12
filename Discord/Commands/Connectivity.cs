using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using Shared;
using System.Linq;
using Shared.Models;

namespace Discord.Commands;
public class Connectivity : BaseCommandModule
{
    private static Random random = new();
    private const string chars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890-_~";
    public CocaBotWebContext db { private get; set; }

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

        Register register = db.Registers.Where(x => x.Discord == ctx.User.Id).FirstOrDefault();
        if (register != null) db.Registers.Remove(register);

        register = new();
        register.VerifKey = key;
        register.Discord = ctx.User.Id;

        await db.Registers.AddAsync(register);
        await db.SaveChangesAsync();
    }
}