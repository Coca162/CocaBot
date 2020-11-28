using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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
            User user = new User(await User.GetSVIDFromDiscordAsync(discordID));
            var data = await user.GetSnapshotAsync();

            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = $"{discordName} Statistics",
                IconUrl = discordPFP,
            };


            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = $"Statistics for [{discordName}](https://spookvooper.com/User/Info?svid={user.Id})'s SpookVooper account",
                Color = new DiscordColor("2CC26C"),
                Author = iconURL
            };

            embed.AddField(
                "ID",
                $"SpookVooper Name: {data.UserName}\nSpookVooper ID: {user.Id}\nDiscord ID: {discordID}\nMinecraft ID: {data.minecraft_id}\n Twitch ID: {data.twitch_id}\n NationStates: {data.nationstate}");
            embed.AddField(
                "Discord",
                $"Discord Message XP: {data.discord_message_xp}\nDiscord Messages: {data.discord_message_count}\nDiscord Game XP: {data.discord_game_xp}\nDiscord Commends: {data.discord_commends}\nDiscord Commends Sent: {data.discord_commends_sent}\nDiscord Bans: {data.discord_ban_count}\nDiscord PFP Url: {data.image_url}");
            embed.AddField(
                "SpookVooper",
                $"Description: {data.description}\nBalance: {data.credits}\nDistrict: + {data.district}\nPost Likes: {data.post_likes}\nComment Likes: {data.comment_likes}\nAPI Use: {data.api_use_count}");
            embed.AddField(
                "Twitch",
                $"Twitch XP: {data.twitch_message_xp}\nTwitch Messages: {data.twitch_messages}");

            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("statistics")]
        [Priority(1)]
        public async Task StatisticsUser(CommandContext ctx)
        {
            string discordName = ctx.Member.Username;
            string discordPFP = ctx.Member.AvatarUrl;
            ulong discordID = ctx.Member.Id;
            User user = new User(await User.GetSVIDFromDiscordAsync(discordID));
            var data = await user.GetSnapshotAsync();

            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = $"{discordName} Statistics",
                IconUrl = discordPFP,
            };


            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = $"Statistics for [{discordName}](https://spookvooper.com/User/Info?svid={user.Id})'s SpookVooper account",
                Color = new DiscordColor("2CC26C"),
                Author = iconURL
            };

            embed.AddField(
                "ID",
                $"SpookVooper Name: {data.UserName}\nSpookVooper ID: {user.Id}\nDiscord ID: {discordID}\nMinecraft ID: {data.minecraft_id}\n Twitch ID: {data.twitch_id}\n NationStates: {data.nationstate}");
            embed.AddField(
                "Discord",
                $"Discord Message XP: {data.discord_message_xp}\nDiscord Messages: {data.discord_message_count}\nDiscord Game XP: {data.discord_game_xp}\nDiscord Commends: {data.discord_commends}\nDiscord Commends Sent: {data.discord_commends_sent}\nDiscord Bans: {data.discord_ban_count}\nDiscord PFP Url: {data.image_url}");
            embed.AddField(
                "SpookVooper",
                $"Description: {data.description}\nBalance: {data.credits}\nDistrict: + {data.district}\nPost Likes: {data.post_likes}\nComment Likes: {data.comment_likes}\nAPI Use: {data.api_use_count}");
            embed.AddField(
                "Twitch",
                $"Twitch XP: {data.twitch_message_xp}\nTwitch Messages: {data.twitch_messages}");

            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }
    }
}

