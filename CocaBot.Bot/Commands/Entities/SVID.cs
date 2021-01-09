using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api.Entities;
using System.Collections.Generic;
using System.Linq;
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

            await ctx.RespondAsync($"{user.GetUsernameAsync()}'s SVID: {svid}").ConfigureAwait(false);
        }

        [Command("svid")]
        [Priority(1)]
        public async Task SVIDAll(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (Inputname != null)
            {
                Dictionary<string, string> svids = await SVTools.NameToSVID(Inputname);

                if (svids != null)
                {
                    if (svids.Count == 1)
                    {
                        KeyValuePair<string, string> svid = svids.First();
                        await ctx.RespondAsync($"{svid.Key}'s SpookVooper Name: {svid.Value}").ConfigureAwait(false);
                    }
                    else
                    {
                        string msgCont = null;
                        foreach (KeyValuePair<string, string> svid in svids)
                        {
                            msgCont += $"\n{svid.Key}: {svid.Value}";
                        }
                        await ctx.RespondAsync($"List of SVIDs: {msgCont}").ConfigureAwait(false);
                    }
                }
                else
                {
                    await ctx.RespondAsync($"This name does not match to any entity (user/group)!").ConfigureAwait(false);
                }
            }
            else
            {
                ulong discordID = ctx.Member.Id;
                User user = new User(await User.GetSVIDFromDiscordAsync(discordID));

                await ctx.RespondAsync($"{ctx.Member.Username}'s SV Name: {user.Id}").ConfigureAwait(false);
            }
        }
    }
}

