using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using static Discord.DiscordTools;
using static Shared.Commands.Get;
using Shared;

namespace Discord.Commands;
public class Get : BaseCommandModule
{
    public CocaBotWebContext db { private get; set; }

    [Command("get"), Aliases("g", "grab", "svid", "name")]
    [Description("Gets basic information about a entity")]
    [Priority(1)]
    public async Task GetDiscord(CommandContext ctx, [Description("A User (works with only id)")] DiscordUser discordUser)
    {
        string discord = await DiscordToSVID(discordUser.Id, db);

        if (discord != "") ctx.RespondAsync(await GetSVID(discord));
        else ctx.RespondAsync(await GetAll(discordUser.Username));
    }

    [Command("get")]
    [Priority(0)]
    public async Task GetString(CommandContext ctx,
        [RemainingText, Description("A Entity (Either SVID, Name or if empty just you)")] string input)
    {
        if (input == null){ GetDiscord(ctx, ctx.User); return; }
        ctx.RespondAsync(await GetAll(input));
    }
}