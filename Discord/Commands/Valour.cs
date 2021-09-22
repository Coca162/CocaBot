using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using Shared;

namespace Discord.Commands;

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
