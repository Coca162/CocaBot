using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api;
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
        [Priority(0)]
        [Aliases("xp", "x")]
        public async Task ExperienceAll(CommandContext ctx, DiscordUser discordUser)
        {
            string discordName = discordUser.Username;
            string discordPFP = discordUser.AvatarUrl;
            ulong discordID = discordUser.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            User data = await SpookVooperAPI.Users.GetUser(SVID);
            int Total_XP = data.GetTotalXP();
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
        [Priority(1)]
        public async Task ExperienceUser(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (Inputname != null)
            {
                string SVID = await SpookVooperAPI.Users.GetSVIDFromUsername(Inputname);

                if (SVID == null)
                {
                    await ctx.Channel.SendMessageAsync($"{Inputname} is not a user!").ConfigureAwait(false);
                }
                else
                {
                    User data = await SpookVooperAPI.Users.GetUser(SVID);
                    string name = ctx.Member.Username;
                    string PFP = data.Image_Url;
                    int Total_XP = data.GetTotalXP();
#pragma warning disable IDE0004
                    decimal Ratio_Messages = (decimal)data.discord_message_xp / (decimal)data.discord_message_count;
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
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                User data = await SpookVooperAPI.Users.GetUser(SVID);
                int Total_XP = data.GetTotalXP();
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

        [Command("experienceloop"), EnableBlacklist]
        [Aliases("xploop", "xpl", "xloop", "xl")]
        public async Task ExperienceUserLoop(CommandContext ctx, float time, float start, [RemainingText] string Inputname)
        {
            if (Inputname != null)
            {
                string SVID = await SpookVooperAPI.Users.GetSVIDFromUsername(Inputname);

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
            else
            {
                ulong discordID = ctx.Member.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
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
                string discordName = ctx.Member.Username;
                string discordPFP = ctx.Member.AvatarUrl;
                User data = await SpookVooperAPI.Users.GetUser(SVID);
                int Total_XP = data.GetTotalXP();
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

        private async Task ExperienceUpdaterAsync(CommandContext ctx, string SVID)
        {
            User data = await SpookVooperAPI.Users.GetUser(SVID);
            string name = ctx.Member.Username;
            string PFP = data.Image_Url;
            int Total_XP = data.GetTotalXP();
#pragma warning disable IDE0004
            decimal Ratio_Messages = (decimal)data.discord_message_xp / (decimal)data.discord_message_count;
            decimal multiplier = (decimal)Math.Pow(10, Convert.ToDouble(2));
            decimal Ratio_Messages_Rounded = (Math.Ceiling(Ratio_Messages * multiplier) / multiplier);

            await ctx.Channel.SendMessageAsync($"XP").ConfigureAwait(false);
        }
    }
}

