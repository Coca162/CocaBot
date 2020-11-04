using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api;
using System.Threading.Tasks;

namespace CocaBot.Commands
{
    public class MainEco : BaseCommandModule
    {
        [Command("balance")]
        public async Task BalanceUser(CommandContext ctx, DiscordUser discordUser)
        {
            if (ctx.User.Id != 470203136771096596)
            {
                ulong discordID = discordUser.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                string SVname = await SpookVooperAPI.Users.GetUsername(SVID);
                decimal Balance = await SpookVooperAPI.Economy.GetBalance(SVID);

                await ctx.Channel.SendMessageAsync($"{SVname} Balance: ¢{Balance}").ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }

        [Command("balance")]
        public async Task Balance(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (ctx.User.Id != 470203136771096596)
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
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }

        [Command("balanceloop")]
        public async Task BalanceLoop(CommandContext ctx, DiscordUser discordUser)
        {
            if (ctx.User.Id != 470203136771096596)
            {
                bool ifowner = ctx.Member.IsOwner;
                if (ifowner == false)
                {
                    await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
                }
                else
                {
                    ulong discordID = discordUser.Id;
                    string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                    string SVname = await SpookVooperAPI.Users.GetUsername(SVID);

                    while (true)
                    {
                        decimal Balance = await SpookVooperAPI.Economy.GetBalance(SVID);
                        await ctx.Channel.SendMessageAsync($"{SVname} Balance: ¢{Balance}").ConfigureAwait(false);
                        await Task.Delay(3600000);
                    }

                }
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }

        [Command("balanceloop")]
        public async Task BalanceLoop(CommandContext ctx, string SVID)
        {
            if (ctx.User.Id != 470203136771096596)
            {
                bool test = ctx.Member.IsOwner;
                if (test != false)
                {
                    string SVname = await SpookVooperAPI.Groups.GetName(SVID);

                    while (true)
                    {
                        decimal Balance = await SpookVooperAPI.Economy.GetBalance(SVID);
                        await ctx.Channel.SendMessageAsync($"{SVname} Balance: ¢{Balance}").ConfigureAwait(false);
                        await Task.Delay(3600000);
                    }

                }
                else
                {
                    await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
                }
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }
    }
}

