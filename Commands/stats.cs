using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using SpookVooper.Api;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System;
using SpookVooper.Api.Entities;

namespace CocaBot.Commands
{
    public class statistics : BaseCommandModule
    {
        [Command("stats")]
        public async Task Statisticsall(CommandContext ctx, DiscordUser discordUser)
        {
            string discordName = discordUser.Username;
            string discordPFP = discordUser.AvatarUrl;
            ulong discordID = discordUser.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);
            User Data = await SpookVooperAPI.Users. GetUser(SVID);

            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = $"{discordName} Statistics",
                IconUrl = discordPFP,
            };


            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = $"Statistics for [{discordName}](https://spookvooper.com/User/Info?svid={SVID})'s SpookVooper account",
                Color = new DiscordColor(0x64FF),
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
                $"Description: {Data.description}\nBalance: {Data.credits}\nDistrict: + {Data.district}\nPost Likes: {Data.post_likes}\nComment Likes: {Data.comment_likes}\nAPI Use: {Data.api_use_count}");
            embed.AddField(
                "Twitch",
                $"Twitch XP: {Data.twitch_message_xp}\nTwitch Messages: {Data.twitch_messages}");

            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("stats")]
        public async Task Statisticsuser(CommandContext ctx)
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
                Color = new DiscordColor(0x64FF),
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
                $"Description: {Data.description}\nBalance: {Data.credits}\nDistrict: + {Data.district}\nPost Likes: {Data.post_likes}\nComment Likes: {Data.comment_likes}\nAPI Use: {Data.api_use_count}");
            embed.AddField(
                "Twitch",
                $"Twitch XP: {Data.twitch_message_xp}\nTwitch Messages: {Data.twitch_messages}");

            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("balance")]
        public async Task balanceall(CommandContext ctx, DiscordUser discordUser)
        {
            ulong discordID = discordUser.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);
            decimal Balance = await SpookVooperAPI.Economy.GetBalance(SVID);

            await ctx.Channel.SendMessageAsync(SV_Name + " Balance: ¢" + Balance).ConfigureAwait(false);
        }


        [Command("balance")]
        public async Task balanceuserSVuser(CommandContext ctx, string opt, [RemainingText] string Inputname)
        {

            if (opt.ToLower() == "group")
            {
                string SVID = await SpookVooperAPI.Groups.GetSVIDFromName(Inputname);
                decimal Balance = await SpookVooperAPI.Economy.GetBalance(SVID);
                string SVname = await SpookVooperAPI.Groups.GetName(SVID);
                await ctx.Channel.SendMessageAsync($"{SVname} Balance: ¢{Balance}").ConfigureAwait(false);
            }
            else if (opt.ToLower() == "user")
            {
                string SVID = await SpookVooperAPI.Users.GetSVIDFromUsername(Inputname);
                string SVname = await SpookVooperAPI.Users.GetUsername(SVID);
                decimal Balance = await SpookVooperAPI.Economy.GetBalance(SVID);
                await ctx.Channel.SendMessageAsync($"{SVname} Balance: ¢{Balance}").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"{Inputname} is not a user or a group!").ConfigureAwait(false);
            }
        }

        [Command("balance")]
        public async Task balanceuser(CommandContext ctx)
        {
            ulong discordID = ctx.Member.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SVname = await SpookVooperAPI.Users.GetUsername(SVID);
            decimal Balance = await SpookVooperAPI.Economy.GetBalance(SVID);

            await ctx.Channel.SendMessageAsync($"{SVname} Balance: ¢{Balance}").ConfigureAwait(false);
        }


        [Command("xp")]
        public async Task xpall(CommandContext ctx, DiscordUser discordUser)
        {
            string discordName = discordUser.Username;
            string discordPFP = discordUser.AvatarUrl;
            ulong discordID = discordUser.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);
            User Data = await SpookVooperAPI.Users.GetUser(SVID);
            int Total_XP = Data.post_likes + Data.comment_likes + (Data.twitch_message_xp * 4) + (Data.discord_commends * 5) + (Data.discord_message_xp * 2) + (Data.discord_game_xp / 100);

            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = discordName + " XP",
                IconUrl = discordPFP,
            };

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = "Total_XP: " + Total_XP,
                Description = $"XP for [{discordName}](https://spookvooper.com/User/Info?svid={SVID})'s SpookVooper account",
                Color = new DiscordColor(0x64FF),
                Author = iconURL
            };
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("xp")]
        public async Task xpuser(CommandContext ctx)
        {
            string discordName = ctx.Member.Username;
            string discordPFP = ctx.Member.AvatarUrl;
            ulong discordID = ctx.Member.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            User Data = await SpookVooperAPI.Users.GetUser(SVID);
            int Total_XP = Data.post_likes + Data.comment_likes + (Data.twitch_message_xp * 4) + (Data.discord_commends * 5) + (Data.discord_message_xp * 2) + (Data.discord_game_xp / 100);

            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = discordName + " XP",
                IconUrl = discordPFP,
            };

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = "Total XP: " + Total_XP,
                Description = $"XP for [{discordName}](https://spookvooper.com/User/Info?svid={SVID})'s SpookVooper account",
                Color = new DiscordColor(0x64FF),
                Author = iconURL
            };
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("leaderboard")]
        public async Task leaderboard(CommandContext ctx)
        {
            string Xboy = "9062fe43-75b0-4f26-a8fd-6a1cdd6883a2";
            string Voopmont = "d88f1221-deb9-4f17-a789-f79f6dc02c11";
            string Tyco = "c9535d5e-1769-40ea-a3d4-6b73775eb086";
            string Dan = "c094e9bd-c021-443f-8138-3433e9ba8b04";
            string Coca = "e1616412-c384-4b00-b443-b8940423df67";

            User Xboy_Data = await SpookVooperAPI.Users.GetUser(Xboy);
            User Voopmont_Data = await SpookVooperAPI.Users.GetUser(Voopmont);
            User Dan_Data = await SpookVooperAPI.Users.GetUser(Dan);
            User Tyco_Data = await SpookVooperAPI.Users.GetUser(Tyco);
            User Coca_Data = await SpookVooperAPI.Users.GetUser(Coca);

            int Xboy_Total_XP = Xboy_Data.post_likes + Xboy_Data.comment_likes + (Xboy_Data.twitch_message_xp * 4) + (Xboy_Data.discord_commends * 5) + (Xboy_Data.discord_message_xp * 2) + (Xboy_Data.discord_game_xp / 100);
            int Voopmont_Total_XP = Voopmont_Data.post_likes + Voopmont_Data.comment_likes + (Voopmont_Data.twitch_message_xp * 4) + (Voopmont_Data.discord_commends * 5) + (Voopmont_Data.discord_message_xp * 2) + (Voopmont_Data.discord_game_xp / 100);
            int Dan_Total_XP = Dan_Data.post_likes + Dan_Data.comment_likes + (Dan_Data.twitch_message_xp * 4) + (Dan_Data.discord_commends * 5) + (Dan_Data.discord_message_xp * 2) + (Dan_Data.discord_game_xp / 100);
            int Tyco_Total_XP = Tyco_Data.post_likes + Tyco_Data.comment_likes + (Tyco_Data.twitch_message_xp * 4) + (Tyco_Data.discord_commends * 5) + (Tyco_Data.discord_message_xp * 2) + (Tyco_Data.discord_game_xp / 100);
            int Coca_Total_XP = Coca_Data.post_likes + Coca_Data.comment_likes + (Coca_Data.twitch_message_xp * 4) + (Coca_Data.discord_commends * 5) + (Coca_Data.discord_message_xp * 2) + (Coca_Data.discord_game_xp / 100);

            Dictionary<string, int> myDict = new Dictionary<string, int>();
            myDict.Add("Xboy", Xboy_Total_XP);
            myDict.Add("Voopmont", Voopmont_Total_XP);
            myDict.Add("Tyco", Tyco_Total_XP);
            myDict.Add("Dan", Dan_Total_XP);
            myDict.Add("Coca", Coca_Total_XP);

            IOrderedEnumerable<KeyValuePair<string, int>> sortedDict = from entry in myDict orderby entry.Value ascending select entry;

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = "SpookVooper Leaderboard",
                Description = "XP Leaderboard from [SpookVooper.com](https://spookvooper.com/Leaderboard/Index/0)",
                Color = new DiscordColor(0x64FF)
            };

            foreach (var item in sortedDict)
                embed.AddField($"{item.Key}:",
                    $"{item.Value} XP"); 
        }

        [Command("leaderboardloop")]
        public async Task leaderboardLoop(CommandContext ctx)
        {
            bool test = ctx.Member.IsOwner;
            if (test == false)
            {
                await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
            }
            else
            {
                string Xboy = "9062fe43-75b0-4f26-a8fd-6a1cdd6883a2";
                string Voopmont = "d88f1221-deb9-4f17-a789-f79f6dc02c11";
                string Tyco = "c9535d5e-1769-40ea-a3d4-6b73775eb086";
                string Dan = "c094e9bd-c021-443f-8138-3433e9ba8b04";
                string Coca = "e1616412-c384-4b00-b443-b8940423df67";

                while (true)
                {
                    User Xboy_Data = await SpookVooperAPI.Users.GetUser(Xboy);
                    User Voopmont_Data = await SpookVooperAPI.Users.GetUser(Voopmont);
                    User Dan_Data = await SpookVooperAPI.Users.GetUser(Dan);
                    User Tyco_Data = await SpookVooperAPI.Users.GetUser(Tyco);
                    User Coca_Data = await SpookVooperAPI.Users.GetUser(Coca);

                    int Xboy_Total_XP = Xboy_Data.post_likes + Xboy_Data.comment_likes + (Xboy_Data.twitch_message_xp * 4) + (Xboy_Data.discord_commends * 5) + (Xboy_Data.discord_message_xp * 2) + (Xboy_Data.discord_game_xp / 100);
                    int Voopmont_Total_XP = Voopmont_Data.post_likes + Voopmont_Data.comment_likes + (Voopmont_Data.twitch_message_xp * 4) + (Voopmont_Data.discord_commends * 5) + (Voopmont_Data.discord_message_xp * 2) + (Voopmont_Data.discord_game_xp / 100);
                    int Dan_Total_XP = Dan_Data.post_likes + Dan_Data.comment_likes + (Dan_Data.twitch_message_xp * 4) + (Dan_Data.discord_commends * 5) + (Dan_Data.discord_message_xp * 2) + (Dan_Data.discord_game_xp / 100);
                    int Tyco_Total_XP = Tyco_Data.post_likes + Tyco_Data.comment_likes + (Tyco_Data.twitch_message_xp * 4) + (Tyco_Data.discord_commends * 5) + (Tyco_Data.discord_message_xp * 2) + (Tyco_Data.discord_game_xp / 100);
                    int Coca_Total_XP = Coca_Data.post_likes + Coca_Data.comment_likes + (Coca_Data.twitch_message_xp * 4) + (Coca_Data.discord_commends * 5) + (Coca_Data.discord_message_xp * 2) + (Coca_Data.discord_game_xp / 100);

                    Dictionary<string, int> myDict = new Dictionary<string, int>();
                    myDict.Add("Xboy", Xboy_Total_XP);
                    myDict.Add("Voopmont", Voopmont_Total_XP);
                    myDict.Add("Tyco", Tyco_Total_XP);
                    myDict.Add("Dan", Dan_Total_XP);
                    myDict.Add("Coca", Coca_Total_XP);

                    IOrderedEnumerable<KeyValuePair<string, int>> sortedDict = from entry in myDict orderby entry.Value ascending select entry;

                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Title = "SpookVooper Leaderboard",
                        Description = "XP Leaderboard from [SpookVooper.com](https://spookvooper.com/Leaderboard/Index/0)",
                        Color = new DiscordColor(0x64FF)
                    };

                    foreach (var item in sortedDict)
                        embed.AddField($"{item.Key}:",
                            $"{item.Value} XP");
                }
            }
        }

        [Command("balanceloop")]
        public async Task balanceLoop(CommandContext ctx, DiscordUser discordUser)
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

        [Command("balanceloop")]
        public async Task BalanceLoop(CommandContext ctx, string SVID)
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

        [Command("verify")]
        public async Task VerifyUser(CommandContext ctx, string type)
        {
            string json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            ConfigJson ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            ulong GuildID = ctx.Guild.Id;
            if (GuildID == ConfigJson.ServerID)
            {
                ulong discordID = ctx.User.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                User Data = await SpookVooperAPI.Users.GetUser(SVID);
                string senate_role = "Senator";
                string if_senate_role = await SpookVooperAPI.Users.HasDiscordRole(SVID, senate_role);

                if ((Data.district == "New Yam") && (type.ToLower() == "citizen"))
                {
                    string discordName = ctx.User.Username;
                    string discordPFP = ctx.User.AvatarUrl;
                    DiscordRole district_role = ctx.Guild.GetRole(ConfigJson.CitizenID);
                    DiscordRole non_citizen_role = ctx.Guild.GetRole(ConfigJson.NonCitizenID);

                    await ctx.TriggerTypingAsync();
                    DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = discordName,
                        IconUrl = discordPFP,
                    };

                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Title = "You now have the New Yam Citizen role!",
                        Color = new DiscordColor(0x64FF),
                        Author = iconURL
                    };
                    await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                    await ctx.Member.GrantRoleAsync(district_role).ConfigureAwait(false);
                    await ctx.Member.RevokeRoleAsync(non_citizen_role).ConfigureAwait(false);
                }
                else if (type.ToLower() == "citizen")
                {
                    string discordName = ctx.User.Username;
                    string discordPFP = ctx.User.AvatarUrl;
                    DiscordRole district_role = ctx.Guild.GetRole(ConfigJson.CitizenID);
                    DiscordRole non_citizen_role = ctx.Guild.GetRole(ConfigJson.NonCitizenID);

                    await ctx.RespondAsync($"{discordName} is not a New Yam Citizen!").ConfigureAwait(false);
                    await ctx.TriggerTypingAsync();
                    DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = discordName,
                        IconUrl = discordPFP,
                    };

                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Title = "You are a Non-Citizen of New Yam!",
                        Color = new DiscordColor(0x64FF),
                        Author = iconURL
                    };
                    await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                    await ctx.Member.GrantRoleAsync(non_citizen_role).ConfigureAwait(false);
                    await ctx.Member.RevokeRoleAsync(district_role).ConfigureAwait(false);
                }
                else if ((if_senate_role == "true") && ((type.ToLower() == "senator")))
                {
                    string discordName = ctx.User.Username;
                    string discordPFP = ctx.User.AvatarUrl;
                    DiscordRole senator_role_id = ctx.Guild.GetRole(ConfigJson.SenateID);

                    DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = discordName,
                        IconUrl = discordPFP,
                    };

                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Title = "You are a Senator!",
                        Color = new DiscordColor(0x64FF),
                        Author = iconURL
                    };
                    await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                    await ctx.Member.GrantRoleAsync(senator_role_id).ConfigureAwait(false);
                }
                else if (type.ToLower() == "senator")
                {
                    string discordName = ctx.User.Username;
                    string discordPFP = ctx.User.AvatarUrl;
                    DiscordRole senator_role_id = ctx.Guild.GetRole(ConfigJson.SenateID);

                    DiscordEmbedBuilder.EmbedAuthor iconURL = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = discordName,
                        IconUrl = discordPFP,
                    };

                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Title = "You are not a Senator!",
                        Color = new DiscordColor(0x64FF),
                        Description = "If you are a Senator VoopAI has probably not updated",
                        Author = iconURL
                    };
                    await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                    await ctx.Member.RevokeRoleAsync(senator_role_id).ConfigureAwait(false);
                }
                else
                {
                    await ctx.RespondAsync("The verify type is not Senator or Citizen!").ConfigureAwait(false);
                }
            }
            else
            {
                string ServerName = ctx.Guild.Name;
                await ctx.RespondAsync($"This is {ServerName} not New Yam Community Server!").ConfigureAwait(false);
            }
        }
    }
}

