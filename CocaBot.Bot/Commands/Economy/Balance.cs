using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CocaBot.Commands
{
    public class Balance : BaseCommandModule
    {
        [Command("balance"), EnableBlacklist]
        [Aliases("balan", "bal", "b")]
        [Priority(1)]
        public async Task BalanceUser(CommandContext ctx, DiscordUser discordUser)
        {
            ulong discordID = discordUser.Id;
            string SVID = await User.GetSVIDFromDiscordAsync(discordID);
            User user = new User(SVID);

            await ctx.RespondAsync($"{await user.GetUsernameAsync()} Balance: ¢{await user.GetBalanceAsync()}").ConfigureAwait(false);
        }

        [Command("balance")]
        [Priority(0)]
        public async Task BalanceAll(CommandContext ctx, [RemainingText] string Input)
        {
            if (Input == null)
            {
                ulong discordID = ctx.Member.Id;
                string SVID = await User.GetSVIDFromDiscordAsync(discordID);
                User user = new User(SVID);

                await ctx.RespondAsync($"{await user.GetUsernameAsync()}'s Balance: ¢{await user.GetBalanceAsync()}").ConfigureAwait(false);
            }
            else if (Input[0].Equals('u') || Input[0].Equals('g'))
            {
                Entity entity = new Entity(Input, null);
                await ctx.RespondAsync($"{await SVTools.SVIDToName(Input)}'s Balance: ¢{await entity.GetBalanceAsync()}").ConfigureAwait(false);
            }
            else
            {
                Dictionary<string, string> svids = await SVTools.NameToSVID(Input);

                if (svids != null)
                {
                    if (svids.Count == 1)
                    {
                        KeyValuePair<string, string> svid = svids.First();
                        Entity entity = new Entity(svid.Value, "");
                        await ctx.RespondAsync($"{Input}'s Balance: ¢{await entity.GetBalanceAsync()}").ConfigureAwait(false);
                    }
                    else
                    {
                        string msgCont = null;
                        foreach (KeyValuePair<string, string> svid in svids)
                        {
                            msgCont += $"\n{svid.Key}: ${svid.Value}";
                        }
                        await ctx.RespondAsync($"List of Balances for {Input}: {msgCont}").ConfigureAwait(false);
                    }
                }
                else
                {
                    await ctx.RespondAsync($"This name does not match to any entity (user/group) SVID or Name!").ConfigureAwait(false);
                }
            }
        }
    }
}