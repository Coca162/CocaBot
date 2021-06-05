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

namespace Discord
{
    public class Misc : BaseCommandModule
    {
        [Command("get")]
        [Aliases("g", "grab")]
        [Priority(1)]
        public async Task GetDiscord(CommandContext ctx, DiscordUser discordUser)
        { await ctx.RespondAsync(await GetAll(await DiscordToSVID(discordUser.Id))).ConfigureAwait(false); }

        [Command("get")]
        [Priority(0)]
        public async Task Get(CommandContext ctx, [RemainingText] string input)
        {
            if (input == null)
            {
                await GetDiscord(ctx, ctx.User).ConfigureAwait(false); return; 
            }

            await ctx.RespondAsync(await GetAll(input)).ConfigureAwait(false);
        }

        [Command("register")]
        [Aliases("reg", "login")]
        public async Task Register(CommandContext ctx)
        { await ctx.RespondAsync(RegisterMessage).ConfigureAwait(false); }

        [Command("verify")]
        [Aliases("verif")]
        public async Task Verify(CommandContext ctx, string key)
        { await ctx.RespondAsync(await VerifyAll(Shared.Database.Platform.Discord, ctx.User.Id, key)).ConfigureAwait(false); }
    }
}
