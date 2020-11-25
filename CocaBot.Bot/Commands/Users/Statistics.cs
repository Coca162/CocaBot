using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api;
using System.Threading.Tasks;
using SpookVooper.Api.Entities;

namespace CocaBot.Commands
{
    public class Statistics : BaseCommandModule
    {
        [Command("statistics"), EnableBlacklist]
        [Aliases("stats", "stat", "st")]
        [Priority(0)]
        public async Task StatisticsAll(CommandContext ctx, DiscordUser discordUser)
        {
            string discordName = discordUser.Username;
            string discordPFP = discordUser.AvatarUrl;
            ulong discordID = discordUser.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);
            User Data = await SpookVooperAPI.Users.GetUser(SVID);

            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = $"{discordName} Statistics",
                IconUrl = discordPFP,
            };


            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = $"Statistics for [{discordName}](https://spookvooper.com/User/Info?svid={SVID})'s SpookVooper account",
                Color = new DiscordColor("2CC26C"),
                Author = iconURL
            };

            embed.AddField(
                "ID",
                $"SpookVooper Name: {SV_Name}\nSpookVooper ID: {SVID}\nDiscord ID: {discordID}\nMinecraft ID: {Data.minecraft_id}\n Twitch ID: {Data.twitch_id}\n NationStates: {Data.nationstate}");
            embed.AddField(
                "Discord",
                $"Discord Message XP: {Data.discord_message_xp}\nDiscord Messages: {Data.discord_message_count}\nDiscord Game XP: {Data.discord_game_xp}\nDiscord Commends: {Data.discord_commends}\nDiscord Commends Sent: {Data.discord_commends_sent}\nDiscord Bans: {Data.discord_ban_count}\nDiscord PFP Url: {Data.Image_Url}");
            embed.AddField(
                "SpookVooper",
                $"Description: {Data.description}\nBalance: {Data.Credits}\nDistrict: + {Data.district}\nPost Likes: {Data.post_likes}\nComment Likes: {Data.comment_likes}\nAPI Use: {Data.api_use_count}");
            embed.AddField(
                "Twitch",
                $"Twitch XP: {Data.twitch_message_xp}\nTwitch Messages: {Data.twitch_messages}");

            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("statistics")]
        [Priority(1)]
        public async Task StatisticsUser(CommandContext ctx)
        {
            string discordName = ctx.Member.Username;
            string discordPFP = ctx.Member.AvatarUrl;
            ulong discordID = ctx.Member.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);
            User Data = await SpookVooperAPI.Users.GetUser(SVID);

            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = $"{discordName} Statistics",
                IconUrl = discordPFP,
            };


            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = $"Statistics for [{discordName}](https://spookvooper.com/User/Info?svid={SVID})'s SpookVooper account",
                Color = new DiscordColor("2CC26C"),
                Author = iconURL
            };

            embed.AddField(
                "ID",
                $"SpookVooper Name: {SV_Name}\nSpookVooper ID: {SVID}\nDiscord ID: {discordID}\nMinecraft ID: {Data.minecraft_id}\n Twitch ID: {Data.twitch_id}\n NationStates: {Data.nationstate}");
            embed.AddField(
                "Discord",
                $"Discord Message XP: {Data.discord_message_xp}\nDiscord Messages: {Data.discord_message_count}\nDiscord Game XP: {Data.discord_game_xp}\nDiscord Commends: {Data.discord_commends}\nDiscord Commends Sent: {Data.discord_commends_sent}\nDiscord Bans: {Data.discord_ban_count}\nDiscord PFP Url: {Data.Image_Url}");
            embed.AddField(
                "SpookVooper",
                $"Description: {Data.description}\nBalance: {Data.Credits}\nDistrict: + {Data.district}\nPost Likes: {Data.post_likes}\nComment Likes: {Data.comment_likes}\nAPI Use: {Data.api_use_count}");
            embed.AddField(
                "Twitch",
                $"Twitch XP: {Data.twitch_message_xp}\nTwitch Messages: {Data.twitch_messages}");

            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }
    }
}

