using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace CocaBot.Commands
{
    public class Balance : BaseCommandModule
    {
        Timer timer;

        [Command("balance"), EnableBlacklist]
        [Aliases("balan", "bal", "b")]
        [Priority(0)]
        public async Task BalanceUser(CommandContext ctx, DiscordUser discordUser)
        {
            ulong discordID = discordUser.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SVname = await SpookVooperAPI.Users.GetUsername(SVID);
            decimal Balance = await SpookVooperAPI.Economy.GetBalance(SVID);

            await ctx.Channel.SendMessageAsync($"{SVname} Balance: ¢{Balance}").ConfigureAwait(false);
        }

        [Command("balance")]
        [Priority(1)]
        public async Task BalanceAll(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (Inputname == null)
            {
                ulong discordID = ctx.Member.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                string SVname = await SpookVooperAPI.Users.GetUsername(SVID);
                decimal Balance = await SpookVooperAPI.Economy.GetBalance(SVID);

                await ctx.Channel.SendMessageAsync($"{SVname} Balance: ¢{Balance}").ConfigureAwait(false);
            }
            else
            {
                string gSVID = await SpookVooperAPI.Groups.GetSVIDFromName(Inputname);
                string uSVID = await SpookVooperAPI.Users.GetSVIDFromUsername(Inputname);

                if (gSVID != null)
                {
                    decimal Balance = await SpookVooperAPI.Economy.GetBalance(gSVID);
                    string SVname = await SpookVooperAPI.Groups.GetName(gSVID);
                    await ctx.Channel.SendMessageAsync($"{SVname} Balance: ¢{Balance}").ConfigureAwait(false);
                }
                else if (uSVID != null)
                {
                    string SVname = await SpookVooperAPI.Users.GetUsername(uSVID);
                    decimal Balance = await SpookVooperAPI.Economy.GetBalance(uSVID);
                    await ctx.Channel.SendMessageAsync($"{SVname} Balance: ¢{Balance}").ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendMessageAsync($"{Inputname} is not a user or a group!").ConfigureAwait(false);
                }
            }
        }

        [Command("balanceloop"), EnableBlacklist]
        [Aliases("balloop", "baloop", "bloop", "ball", "bl")]
        [Priority(0)]
        public async Task BalanceLoopUser(CommandContext ctx, DiscordUser discordUser, float time, float start)
        {
            bool test = ctx.Member.IsOwner;
            if (test != false)
            {
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordUser.Id);
                string SVname = await SpookVooperAPI.Users.GetUsername(SVID);

                float delay = (DateTime.Now.Minute + (DateTime.Now.Second / 60) - start) * 60000;
                if (delay < 0) { delay = (float)(start + (60 - (DateTime.Now.Minute + (DateTime.Now.Second / 60))) * 60000); };
                await Task.Delay((int)delay);
                BalanceUpdateAsync(ctx, SVID, discordUser.Username);
                if (time == 60) { time = 0; };
                timer = new Timer();
                timer.Interval = (float)(time * 60000);
                timer.Enabled = true;
                timer.Elapsed += (sender, e) => BalanceUpdateAsync(ctx, SVID, discordUser.Username);
            }
            else
            {
                await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
            }
        }

        [Command("balanceloop")]
        [Priority(1)]
        public async Task BalanceLoopSV(CommandContext ctx, string SVID, float time, float start)
        {
            bool test = ctx.Member.IsOwner;
            if (test != false)
            {
                string gname = await SpookVooperAPI.Groups.GetName(SVID);
                string uname = await SpookVooperAPI.Users.GetUsername(SVID);
                string name = null;
                if (gname == null && uname == null)
                {
                    await ctx.RespondAsync("That is not a valid SVID!").ConfigureAwait(false);
                    Environment.Exit(0);
                }
                if (gname != null) { name = gname; }
                else if (uname != null) { name = uname; }

                float delay = (DateTime.Now.Minute + (DateTime.Now.Second / 60) - start) * 60000;
                if (delay < 0) { delay = (float)(start + (60 - (DateTime.Now.Minute + (DateTime.Now.Second / 60))) * 60000); };
                await Task.Delay((int)delay);
                BalanceUpdateAsync(ctx, SVID, name);
                if (time == 60) { time = 0; };
                timer = new Timer();
                timer.Interval = (float)(time * 60000);
                timer.Enabled = true;
                timer.Elapsed += (sender, e) => BalanceUpdateAsync(ctx, SVID, name);
            }
            else
            {
                await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
            }
        }
        private async void BalanceUpdateAsync(CommandContext ctx, string svid, string name)
        {
            decimal balance = await SpookVooperAPI.Economy.GetBalance(svid);

            await ctx.Channel.SendMessageAsync($"{DateTime.Now.TimeOfDay}\n¢{balance}").ConfigureAwait(false);
            await Task.Delay(3600000);
        }
    }
}

