using DSharpPlus;
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
        public async Task Statisticsall(CommandContext ctx, DiscordUser discordUser)
        {
            var discordName = discordUser.Username;
            var discordPFP = discordUser.AvatarUrl;
            var discordID = discordUser.Id;
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
               "Description: " + Data.description + "\nBalance: " + Data.credits + "\nDistrict: " + Data.district + "\nPost Likes: " + Data.post_likes + "\nComment Likes: " + Data.comment_likes + "\nAPI Use: " + Data.api_use_count);
            embed.AddField("Twitch",
                "Twitch XP: " + Data.twitch_message_xp + "\nTwitch Messages: " + Data.twitch_messages);
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("stats")]
        public async Task Statisticsuser(CommandContext ctx)
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
               "Description: " + Data.description + "\nBalance: " + Data.credits + "\nDistrict: " + Data.district + "\nPost Likes: " + Data.post_likes + "\nComment Likes: " + Data.comment_likes + "\nAPI Use: " + Data.api_use_count);
            embed.AddField("Twitch",
                "Twitch XP: " + Data.twitch_message_xp + "\nTwitch Messages: " + Data.twitch_messages);
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("balance")]
        public async Task balanceall(CommandContext ctx, DiscordUser discordUser)
        {
            var discordID = discordUser.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);
            var Balance = await SpookVooperAPI.Economy.GetBalance(SVID);

            await ctx.Channel.SendMessageAsync(SV_Name + " Balance: ¢" + Balance).ConfigureAwait(false);
        }

        [Command("balance")]
        public async Task balanceuser(CommandContext ctx)
        {
            var discordID = ctx.Member.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);
            var Balance = await SpookVooperAPI.Economy.GetBalance(SVID);

            await ctx.Channel.SendMessageAsync(SV_Name + " Balance: ¢" + Balance).ConfigureAwait(false);
        }

        [Command("xp")]
        public async Task xpall(CommandContext ctx, DiscordUser discordUser)
        {
            var discordName = discordUser.Username;
            var discordPFP = discordUser.AvatarUrl;
            var discordID = discordUser.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);
            var Data = await SpookVooperAPI.Users.GetUser(SVID);
            var Total_XP = Data.post_likes + Data.comment_likes + (Data.twitch_message_xp * 4) + (Data.discord_commends * 5) + (Data.discord_message_xp * 2) + (Data.discord_game_xp / 100);

            await ctx.TriggerTypingAsync();
            var iconURL = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = discordName + " XP",
                IconUrl = discordPFP,
            };

            var embed = new DiscordEmbedBuilder
            {
                Title = "Total_XP: " + Total_XP,
                Description = "XP for [" + discordName + "](https://spookvooper.com/User/Info?svid=" + SVID + ")'s SpookVooper account",
                Color = new DiscordColor(0x64FF),
                Author = iconURL
            };
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("xp")]
        public async Task xpuser(CommandContext ctx)
        {
            var discordName = ctx.Member.Username;
            var discordPFP = ctx.Member.AvatarUrl;
            var discordID = ctx.Member.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);
            var Data = await SpookVooperAPI.Users.GetUser(SVID);
            var Total_XP = Data.post_likes + Data.comment_likes + (Data.twitch_message_xp * 4) + (Data.discord_commends * 5) + (Data.discord_message_xp * 2) + (Data.discord_game_xp / 100);

            await ctx.TriggerTypingAsync();
            var iconURL = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = discordName + " XP",
                IconUrl = discordPFP,
            };

            var embed = new DiscordEmbedBuilder
            {
                Title = "Total XP: " + Total_XP,
                Description = "XP for [" + discordName + "](https://spookvooper.com/User/Info?svid=" + SVID + ")'s SpookVooper account",
                Color = new DiscordColor(0x64FF),
                Author = iconURL
            };
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("leaderboard")]
        public async Task leaderboard(CommandContext ctx)
        {
            var Xboy = "9062fe43-75b0-4f26-a8fd-6a1cdd6883a2";
            var Voopmont = "d88f1221-deb9-4f17-a789-f79f6dc02c11";
            var Tyco = "c9535d5e-1769-40ea-a3d4-6b73775eb086";
            var Dan = "c094e9bd-c021-443f-8138-3433e9ba8b04";
            var Coca = "e1616412-c384-4b00-b443-b8940423df67";

            var Xboy_Data = await SpookVooperAPI.Users.GetUser(Xboy);
            var Voopmont_Data = await SpookVooperAPI.Users.GetUser(Voopmont);
            var Dan_Data = await SpookVooperAPI.Users.GetUser(Dan);
            var Tyco_Data = await SpookVooperAPI.Users.GetUser(Tyco);
            var Coca_Data = await SpookVooperAPI.Users.GetUser(Coca);

            var Xboy_Total_XP = Xboy_Data.post_likes + Xboy_Data.comment_likes + (Xboy_Data.twitch_message_xp * 4) + (Xboy_Data.discord_commends * 5) + (Xboy_Data.discord_message_xp * 2) + (Xboy_Data.discord_game_xp / 100);
            var Voopmont_Total_XP = Voopmont_Data.post_likes + Voopmont_Data.comment_likes + (Voopmont_Data.twitch_message_xp * 4) + (Voopmont_Data.discord_commends * 5) + (Voopmont_Data.discord_message_xp * 2) + (Voopmont_Data.discord_game_xp / 100);
            var Dan_Total_XP = Dan_Data.post_likes + Dan_Data.comment_likes + (Dan_Data.twitch_message_xp * 4) + (Dan_Data.discord_commends * 5) + (Dan_Data.discord_message_xp * 2) + (Dan_Data.discord_game_xp / 100);
            var Tyco_Total_XP = Tyco_Data.post_likes + Tyco_Data.comment_likes + (Tyco_Data.twitch_message_xp * 4) + (Tyco_Data.discord_commends * 5) + (Tyco_Data.discord_message_xp * 2) + (Tyco_Data.discord_game_xp / 100);
            var Coca_Total_XP = Coca_Data.post_likes + Coca_Data.comment_likes + (Coca_Data.twitch_message_xp * 4) + (Coca_Data.discord_commends * 5) + (Coca_Data.discord_message_xp * 2) + (Coca_Data.discord_game_xp / 100);

            var embed = new DiscordEmbedBuilder
            {
                Title = "SpookVooper Leaderboard",
                Description = "XP Leaderboard from [" + "SpookVooper.com" + "](https://spookvooper.com/Leaderboard/Index/0)",
                Color = new DiscordColor(0x64FF)
            };
            embed.AddField("Top 5:",
                "1. Xboy: " + Xboy_Total_XP + "\n2. Voopmont: " + Voopmont_Total_XP + "\n3. Tyco: " + Tyco_Total_XP + "\n4. Dan: " + Dan_Total_XP + "\n5. Coca: " + Coca_Total_XP);
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }
    }
}

