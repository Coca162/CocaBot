using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using System;
using SpookVooper.Api.Entities;
using System.Timers;

namespace CocaBot.Commands
{
    public class Experience : BaseCommandModule
    {
        Timer timer;
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
                Color = new DiscordColor("#965d4a"),
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
                    string name = ctx.Member.Username;
                    string PFP = data.image_url;
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
                        Color = new DiscordColor("#965d4a"),
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
                    Color = new DiscordColor("#965d4a"),
                    Author = iconURL
                };
                await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
            }
        }

        [Command("experienceloop"), EnableBlacklist]
        [Aliases("xploop", "xpl", "xloop", "xl")]
        public async Task ExperienceUserLoop(CommandContext ctx, float time, float start, [RemainingText] string Inputname)
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
                    float delay = (DateTime.Now.Minute + (DateTime.Now.Second / 60) - start) * 60000;
                    if (delay < 0) { delay = (float)(start + (60 - (DateTime.Now.Minute + (DateTime.Now.Second / 60))) * 60000); };
                    await Task.Delay((int)delay);
#pragma warning disable CS4014
                    ExperienceUpdaterAsync(ctx, SVID);
#pragma warning restore CS4014
                    if (time == 60) { time = 0; };
                    timer = new Timer();
                    timer.Interval = (float)(time * 60000);
                    timer.Enabled = true;
#pragma warning disable CS4014
                    timer.Elapsed += (sender, e) => ExperienceUpdaterAsync(ctx, SVID);
#pragma warning restore CS4014
                }
            }
        }

        private async Task ExperienceUpdaterAsync(CommandContext ctx, string SVID)
        {
            User user = new User(SVID);
            var data = await user.GetSnapshotAsync();
            int Total_XP = data.post_likes + data.comment_likes + (data.twitch_message_xp * 4) + (data.discord_commends * 5) + (data.discord_message_xp * 2) + (data.discord_game_xp / 100); string name = ctx.Member.Username;
#pragma warning disable IDE0004
            decimal Ratio_Messages = (decimal)data.discord_message_xp / (decimal)data.discord_message_count;
#pragma warning restore IDE0004
            decimal multiplier = (decimal)Math.Pow(10, Convert.ToDouble(2));
            decimal Ratio_Messages_Rounded = (Math.Ceiling(Ratio_Messages * multiplier) / multiplier);

            DateTime time = DateTime.Now;
            await ctx.Channel.SendMessageAsync($"{time.Day}/{time.Month}/{time.Year} {time.Hour}:{time.Minute}:{time.Second}\nXP: {Total_XP}\nRatio: {Ratio_Messages_Rounded * 2}\nMessage Count: {data.discord_message_count}").ConfigureAwait(false);
        }
    }
}

