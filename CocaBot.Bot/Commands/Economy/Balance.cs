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

                if (gSVID != null)
                {
                    User entity = new User(uSVID);
                    await ctx.Channel.SendMessageAsync($"{await entity.GetUsernameAsync()} Balance: ¢{await entity.GetBalanceAsync()}").ConfigureAwait(false);
                }
                else if (uSVID != null)
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

        [Command("balanceloop"), EnableBlacklist]
        [Aliases("balloop", "baloop", "bloop", "ball", "bl")]
        [Priority(1)]
        public async Task BalanceLoopUser(CommandContext ctx, DiscordUser discordUser, float time, float start)
        {
            bool test = ctx.Member.IsOwner;
            if (test != false)
            {
                string SVID = await User.GetSVIDFromDiscordAsync(discordUser.Id);
                User entity = new User(SVID);

                float delay = (DateTime.Now.Minute + (DateTime.Now.Second / 60) - start) * 60000;
                if (delay < 0) { delay = (float)(start + (60 - (DateTime.Now.Minute + (DateTime.Now.Second / 60))) * 60000); };
                await Task.Delay((int)delay);
                BalanceEntityUpdateAsync(ctx, entity);
                timer = new System.Timers.Timer
                {
                    Interval = (float)(time * 60000),
                    Enabled = true
                };
                timer.Elapsed += (sender, e) => BalanceEntityUpdateAsync(ctx, entity);
            }
            else
            {
                await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
            }
        }

        [Command("balanceloop")]
        [Priority(0)]
        public async Task BalanceLoopSV(CommandContext ctx, string SVID, float time, float start)
        {
            bool test = ctx.Member.IsOwner;
            if (test != false)
            {
                DateTime datetime = DateTime.Now;
                float delay = (datetime.Minute + (datetime.Second / 60) - start) * 60000;
                if (delay < 0) { delay = (float)(start + (60 - (datetime.Minute + (datetime.Second / 60))) * 60000); };
                timer = new System.Timers.Timer
                {
                    Interval = (float)(time * 60000),
                    Enabled = true
                };

                bool isgroup = SVID.Contains("g-");
                bool isuser = SVID.Contains("u-");

                if (isgroup == false && isuser == false) { await ctx.RespondAsync("That is not a valid SVID!").ConfigureAwait(false); }
                else if (isgroup == true && isuser == true) { await ctx.RespondAsync("That SVID is for 2 entities! Please conctact Coca about this!"); }
                else if (isgroup == true)
                {
                    Group group = new Group(SVID);
                    string gname = await group.GetNameAsync();
                    if (gname == null) { await ctx.RespondAsync("That is not a valid SVID!").ConfigureAwait(false); }
                    else
                    {
                        await Task.Delay((int)delay);
                        BalanceEntityUpdateAsync(ctx, group);
                        timer.Elapsed += (sender, e) => BalanceEntityUpdateAsync(ctx, group);
                        await ctx.RespondAsync($"Balance loop for group {await group.GetNameAsync().ConfigureAwait(false)}:").ConfigureAwait(false);
                    }
                }
                if (isuser == true)
                {
                    User user = new User(SVID);
                    string uname = await user.GetUsernameAsync();
                    if (uname == null) { await ctx.RespondAsync("That is not a valid SVID!").ConfigureAwait(false); }
                    else
                    {
                        await Task.Delay((int)delay);
                        BalanceEntityUpdateAsync(ctx, user);
                        timer.Elapsed += (sender, e) => BalanceEntityUpdateAsync(ctx, user);
                        await ctx.RespondAsync($"Balance loop for user {await user.GetUsernameAsync().ConfigureAwait(false)}:").ConfigureAwait(false);
                    }
                }
            }
            else
            {
                await ctx.RespondAsync("You are not server owner!").ConfigureAwait(false);
            }
        }

        private static async void BalanceEntityUpdateAsync(CommandContext ctx, Entity entity)
        {
            DateTime time = DateTime.Now;
            await ctx.Channel.SendMessageAsync($"{time.Day}/{time.Month}/{time.Year} {time.Hour}:{time.Minute}:{time.Second}\n¢{await entity.GetBalanceAsync()}").ConfigureAwait(false);
        }
    }
}