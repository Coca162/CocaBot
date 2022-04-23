using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using Shared;
using System.Linq;
using Shared.Models;

namespace Discord.Commands;

[Group("valour")] // let's mark this class as a command group
[Description("Valour related commands")] // give it a description for help purposes
public class Valour : BaseCommandModule
{
    public CocaBotPoolContext db { private get; set; }

    [Command("connect"), Aliases("link")]
    [Description("Connects your valour account so that it can do /pay and self /balance")]
    public async Task Connect(CommandContext ctx, [Description("Valour Name")] string name)
    {
        var user = db.Users.SingleOrDefault(x => x.Discord == ctx.User.Id);
        if (user is null)
        {
            await ctx.RespondAsync("Your discord account is not linked to a SV account! Do c/register first!").ConfigureAwait(false);
            return;
        }

        user.ValourName = name;
        await db.SaveChangesAsync();

        await ctx.RespondAsync("Do `c/confirm` on your valour account to complete connection process!").ConfigureAwait(false);
    }

    [Command("disconnect"), Aliases("unlink")]
    [Description("Removes valour name and valour id from db.")]
    public async Task Disconnect(CommandContext ctx)
    {
        var user = db.Users.SingleOrDefault(x => x.Discord == ctx.User.Id);
        if (user is null)
        {
            await ctx.RespondAsync("Your discord account is not linked to a SV account! Do c/register first and then c/connect to be able to do this command!").ConfigureAwait(false);
            return;
        }

        user.Valour = null;
        user.ValourName = null;
        await db.SaveChangesAsync();

        await ctx.RespondAsync("Your Valour accounts have been wiped").ConfigureAwait(false);
    }
}
