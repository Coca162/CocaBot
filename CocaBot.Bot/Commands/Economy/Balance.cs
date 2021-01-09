using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api.Entities;
using SpookVooper.Api.Entities.Groups;
using System;
using System.Threading.Tasks;

namespace CocaBot.Commands
{
    public class Balance : BaseCommandModule
    {
        System.Timers.Timer timer;

        [Command("balance"), EnableBlacklist]
        [Aliases("balan", "bal", "b")]
        [Priority(1)]
        public async Task BalanceUser(CommandContext ctx, DiscordUser discordUser)
        {
            ulong discordID = discordUser.Id;
            string SVID = await User.GetSVIDFromDiscordAsync(discordID);
            User user = new User(SVID);

            await ctx.Channel.SendMessageAsync($"{await user.GetUsernameAsync()} Balance: ¢{await user.GetBalanceAsync()}").ConfigureAwait(false);
        }

        [Command("balance")]
        [Priority(0)]
        public async Task BalanceAll(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (Inputname == null)
            {
                ulong discordID = ctx.Member.Id;
                string SVID = await User.GetSVIDFromDiscordAsync(discordID);
                User user = new User(SVID);

                await ctx.Channel.SendMessageAsync($"{await user.GetUsernameAsync()} Balance: ¢{await user.GetBalanceAsync()}").ConfigureAwait(false);
            }
            else
            {
                string gSVID = await Group.GetSVIDFromNameAsync(Inputname);
                string uSVID = await User.GetSVIDFromUsernameAsync(Inputname);

                if (gSVID.Contains("g-") == false)
                {
                    User entity = new User(uSVID);
                    await ctx.Channel.SendMessageAsync($"{await entity.GetUsernameAsync()} Balance: ¢{await entity.GetBalanceAsync()}").ConfigureAwait(false);
                }
                else if (uSVID.Contains("u-") == false)
                {
                    Group entity = new Group(gSVID);
                    await ctx.Channel.SendMessageAsync($"{await entity.GetNameAsync()} Balance: ¢{await entity.GetBalanceAsync()}").ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendMessageAsync($"{Inputname} is not a user or a group!").ConfigureAwait(false);
                }
            }
        }
    }
}