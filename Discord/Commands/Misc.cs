using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Discord.DiscordTools;
using static Shared.Commands.Get;
using static Shared.Commands.Register;
using static Shared.Commands.Verify;
using static Shared.Main;

namespace Discord.Commands
{
    public class Misc : BaseCommandModule
    {
        [Command("get"), Aliases("g", "grab", "svid", "name")]
        [Description("Gets basic information about a entity")]
        [Priority(1)]
        public async Task GetDiscord(CommandContext ctx, [Description("A User (works with only id)")] DiscordUser discordUser)
        {
            string discord = await DiscordToSVID(discordUser.Id);
            if (discord != "") await ctx.RespondAsync(await GetAll(discord)).ConfigureAwait(false);
            else await ctx.RespondAsync(await GetAll(discordUser.Username)).ConfigureAwait(false);
        }

        [Command("get")]
        [Priority(0)]
        public async Task Get(CommandContext ctx, 
            [RemainingText, Description("A Entity (Either SVID, Name or if empty just you)")] string input)
        {
            if (input == null)
            {
                await GetDiscord(ctx, ctx.User).ConfigureAwait(false); return; 
            }

            await ctx.RespondAsync(await GetAll(input)).ConfigureAwait(false);
        }

        [Command("register"), Aliases("reg", "login")]
        [Description("Gives link for linking your SV account to your discord account")]
        public async Task Register(CommandContext ctx)
        { await ctx.RespondAsync(RegisterMessage).ConfigureAwait(false); }

        [Command("verify"), Aliases("verif")]
        [Description("Links your account by the key that is given to after doing /register. Make sure to do in DMs!")]
        public async Task Verify(CommandContext ctx, [Description("The key provided to you")] string key)
        { await ctx.RespondAsync(await VerifyAll(Platform.Discord, ctx.User.Id, key)).ConfigureAwait(false); }

        [Command("kill")]
        [Description("Kills the bot incase of a emergency. Coca only command for obiovus reasons!")]
        public async Task Kill(CommandContext ctx)
        { 
            if (ctx.User.Id == 388454632835514380)
            {
                Environment.Exit(666);
            }
        }
    }
}
