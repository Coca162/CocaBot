using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using SpookVooper.Api;
using System;
using System.Threading.Tasks;
namespace CocaBot.Commands
{
    public class statistics : BaseCommandModule
    {
        [Command("stats")]
        public async Task Statistics(CommandContext ctx)
        {
            var discordName = ctx.Member.Username;
            var discordPFP = ctx.Member.AvatarUrl;
            var discordID = ctx.Member.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);
            var Data = await SpookVooperAPI.Users.GetUser(SVID);

            await ctx.TriggerTypingAsync();

            var iconURL = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = discordName + " Statistics",
                IconUrl = discordPFP,
            };


            var embed = new DiscordEmbedBuilder
            {
                Description = "Statistics for [" + discordName + "](https://spookvooper.com/User/Info?svid=" + SVID + ")'s SpookVooper account",
                Color = new DiscordColor(0x64FF),
                Author = iconURL
            };
            embed.AddField("ID",
                "SpookVooper Name: " + SV_Name + "\nSpookVooper ID: " + SVID + "\nDiscord ID: " + discordID + "\nMinecraft ID: " + Data.minecraft_id + "\n Twitch ID: " + Data.twitch_id + "\n NationStates: " + Data.nationstate);
            embed.AddField("Discord",
                "Discord Message XP: " + Data.discord_message_xp + "\nDiscord Messages: " + Data.discord_message_count + "\nDiscord Game XP: " + Data.discord_game_xp + "\nDiscord Commends: " + Data.discord_commends + "\nDiscord Commends Sent: " + Data.discord_commends_sent + "\nDiscord Bans: " + Data.discord_ban_count);
            embed.AddField("SpookVooper",
               "Description: " + Data.description + "\nBalance: " + Data.credits + "\nDistrict: " + Data.district + "\nPost Likes: " + Data.post_likes + "\nComment Likes" + Data.comment_likes + "\nAPI Use: " + Data.api_use_count);
            embed.AddField("Twitch",
                "Twicth XP: " + Data.twitch_message_xp + "\nTwicth Messages: " + Data.twitch_messages);
            await ctx.RespondAsync(embed: embed);
        }
    }

}
