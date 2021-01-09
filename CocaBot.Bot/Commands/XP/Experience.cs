using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api.Entities;
using System;
using System.Threading.Tasks;

namespace CocaBot.Commands
{
    public class Experience : BaseCommandModule
    {
        [Command("experience"), EnableBlacklist]
        [Priority(1)]
        [Aliases("xp", "x")]
        public async Task ExperienceAll(CommandContext ctx, DiscordUser discordUser)
        {
            string discordName = discordUser.Username;
            string discordPFP = discordUser.AvatarUrl;
            ulong discordID = discordUser.Id;
            string SVID = await User.GetSVIDFromDiscordAsync(discordID);
            User user = new User(SVID);
            var data = await user.GetSnapshotAsync();
            int Total_XP = data.post_likes + data.comment_likes + (data.twitch_message_xp * 4) + (data.discord_commends * 5) + (data.discord_message_xp * 2) + (data.discord_game_xp / 100);
#pragma warning disable IDE0004
            decimal Ratio_Messages = (decimal)data.discord_message_xp / (decimal)data.discord_message_count;
#pragma warning restore IDE0004
            decimal multiplier = (decimal)Math.Pow(10, Convert.ToDouble(2));
            decimal Ratio_Messages_Rounded = (Math.Ceiling(Ratio_Messages * multiplier) / multiplier);

            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = discordName + " XP",
                IconUrl = discordPFP,
            };

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = $"Total XP: {Total_XP}\nMessage to XP: 1 : {Ratio_Messages_Rounded * 2}",
                Description = $"XP for [{discordName}](https://spookvooper.com/User/Info?svid={SVID})'s SpookVooper account",
                Color = new DiscordColor("2CC26C"),
                Author = iconURL
            };
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("experience")]
        [Priority(0)]
        public async Task ExperienceUser(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (Inputname != null)
            {
                string SVID = await User.GetSVIDFromUsernameAsync(Inputname);

                if (SVID == null)
                {
                    await ctx.Channel.SendMessageAsync($"{Inputname} is not a user!").ConfigureAwait(false);
                }
                else
                {
                    User user = new User(SVID);
                    var data = await user.GetSnapshotAsync();
                    string name = data.UserName;
                    string PFP = data.Image_Url;
                    int Total_XP = data.post_likes + data.comment_likes + (data.twitch_message_xp * 4) + (data.discord_commends * 5) + (data.discord_message_xp * 2) + (data.discord_game_xp / 100);
#pragma warning disable IDE0004
                    decimal Ratio_Messages = (decimal)data.discord_message_xp / (decimal)data.discord_message_count;
#pragma warning restore IDE0004
                    decimal multiplier = (decimal)Math.Pow(10, Convert.ToDouble(2));
                    decimal Ratio_Messages_Rounded = (Math.Ceiling(Ratio_Messages * multiplier) / multiplier);

                    await ctx.TriggerTypingAsync();
                    DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = name + " XP",
                        IconUrl = PFP,
                    };

                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Title = $"Total XP: {Total_XP}\nMessage to XP: 1 : {Ratio_Messages_Rounded * 2}",
                        Description = $"XP for [{name}](https://spookvooper.com/User/Info?svid={SVID})'s SpookVooper account",
                        Color = new DiscordColor("2CC26C"),
                        Author = iconURL
                    };
                    await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                }
            }
            else
            {
                string discordName = ctx.Member.Username;
                string discordPFP = ctx.Member.AvatarUrl;
                ulong discordID = ctx.Member.Id;
                string SVID = await User.GetSVIDFromDiscordAsync(discordID);
                User user = new User(SVID);
                var data = await user.GetSnapshotAsync();
                int Total_XP = data.post_likes + data.comment_likes + (data.twitch_message_xp * 4) + (data.discord_commends * 5) + (data.discord_message_xp * 2) + (data.discord_game_xp / 100);
#pragma warning disable IDE0004
                decimal Ratio_Messages = (decimal)data.discord_message_xp / (decimal)data.discord_message_count;
#pragma warning restore IDE0004
                decimal multiplier = (decimal)Math.Pow(10, Convert.ToDouble(2));
                decimal Ratio_Messages_Rounded = (Math.Ceiling(Ratio_Messages * multiplier) / multiplier);

                await ctx.TriggerTypingAsync();
                DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = discordName + " XP",
                    IconUrl = discordPFP,
                };

                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Title = $"Total XP: {Total_XP}\nMessage to Message XP: 1 : {Ratio_Messages_Rounded * 2}",
                    Description = $"XP for [{discordName}](https://spookvooper.com/User/Info?svid={SVID})'s SpookVooper account",
                    Color = new DiscordColor("2CC26C"),
                    Author = iconURL
                };
                await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
            }
        }
    }
}

