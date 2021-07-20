using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using static Discord.DiscordTools;
using static Shared.Commands.Get;
using static Shared.Commands.Register;
using static Shared.Commands.Verify;
using static Shared.Commands.Code;
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

        [Command("code"), Aliases("opensource")]
        [Description("Gives link for linking your SV account to your discord account")]
        public async Task Code(CommandContext ctx)
        { await ctx.RespondAsync(CodeLink).ConfigureAwait(false); }

        [Command("verify"), Aliases("verif")]
        [Description("Links your account by the key that is given to after doing c/register. Make sure to do in DMs!")]
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

    [Group("valour")] // let's mark this class as a command group
    [Description("Valour related commands")] // give it a description for help purposes
    public class Valour : BaseCommandModule
    {
        [Command("connect"), Aliases("link")]
        [Description("Connects your valour account so that it can do /pay and self /balance")]
        public async Task Connect(CommandContext ctx, [Description("Valour Name")] string name)
        {
            if (!(await Shared.Database.ValourName(ctx.User.Id, name)))
            {
                await ctx.RespondAsync("Your discord account is not linked to a SV account! Do c/register first!").ConfigureAwait(false);
                return;
            }
            await ctx.RespondAsync("Do c/confirm on your valour account to complete connection process!").ConfigureAwait(false);
        }

        [Command("disconnect"), Aliases("unlink")]
        [Description("Removes valour name and valour id from db.")]
        public async Task Disconnect(CommandContext ctx)
        {
            if (!(await Shared.Database.ValourDisconnect(ctx.User.Id)))
            {
                await ctx.RespondAsync("Your discord account is not linked to a SV account! Do c/register first and then c/connect then you can do this command!").ConfigureAwait(false);
                return;
            }
            await ctx.RespondAsync("Your Valour accounts have been wiped").ConfigureAwait(false);
        }
    }
}
