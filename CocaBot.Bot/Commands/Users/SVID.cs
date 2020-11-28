using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api;
using SpookVooper.Api.Entities;
using SpookVooper.Api.Entities.Groups;
using System;
using System.Threading.Tasks;

namespace CocaBot.Commands
{
    public class SVID : BaseCommandModule
    {
        [Command("svid"), EnableBlacklist]
        [Aliases("s", "sv")]
        [Priority(0)]
        public async Task SVIDUser(CommandContext ctx, DiscordUser discordUser)
        {
            ulong discordID = discordUser.Id;
            string svid = await User.GetSVIDFromDiscordAsync(discordID);
            User user = new User(svid);

            await ctx.Channel.SendMessageAsync($"{user.GetUsernameAsync()}'s SVID: {svid}").ConfigureAwait(false);
        }

        [Command("svid")]
        [Priority(1)]
        public async Task SVIDAll(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (Inputname != null)
            {
                bool isgroup = Inputname.Contains("g-");
                bool isuser = Inputname.Contains("u-");

                if (isgroup == false && isuser == false) { await ctx.RespondAsync($"{Inputname} is not a SVID for a user or a group!").ConfigureAwait(false); }
                else if (isgroup == true && isuser == true) { await ctx.RespondAsync("That SVID is for 2 entities! Please conctact Coca about this!"); }
                else
                {
                    Group group = new Group(Inputname);
                    User user = new User(Inputname);

                    await ctx.Channel.SendMessageAsync($"SVID: {group.GetNameAsync()} {user.GetUsernameAsync()}").ConfigureAwait(false);
                }
            }
            else
            {
                ulong discordID = ctx.Member.Id;
                User user = new User(await User.GetSVIDFromDiscordAsync(discordID));

                await ctx.Channel.SendMessageAsync($"{ctx.Member.Username}'s SV Name: {user.Id}").ConfigureAwait(false);
            }
        }
    }
}

