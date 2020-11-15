using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using SpookVooper.Api.Entities;

namespace CocaBot.Commands
{
    public class MainXP : BaseCommandModule
    {
        [Command("statistics")]
        public async Task StatisticsAll(CommandContext ctx, DiscordUser discordUser)
        {
            if (ctx.User.Id != 470203136771096596)
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
                    $"Description: {Data.description}\nBalance: {Data.credits}\nDistrict: + {Data.district}\nPost Likes: {Data.post_likes}\nComment Likes: {Data.comment_likes}\nAPI Use: {Data.api_use_count}");
                embed.AddField(
                    "Twitch",
                    $"Twitch XP: {Data.twitch_message_xp}\nTwitch Messages: {Data.twitch_messages}");

                await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }

        [Command("statistics")]
        public async Task StatisticsUser(CommandContext ctx)
        {
            if (ctx.User.Id != 470203136771096596)
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
                    $"Description: {Data.description}\nBalance: {Data.credits}\nDistrict: + {Data.district}\nPost Likes: {Data.post_likes}\nComment Likes: {Data.comment_likes}\nAPI Use: {Data.api_use_count}");
                embed.AddField(
                    "Twitch",
                    $"Twitch XP: {Data.twitch_message_xp}\nTwitch Messages: {Data.twitch_messages}");

                await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }

        [Command("experience")]
        public async Task ExperienceAll(CommandContext ctx, DiscordUser discordUser)
        {
            if (ctx.User.Id != 470203136771096596)
            {
                string discordName = discordUser.Username;
                string discordPFP = discordUser.AvatarUrl;
                ulong discordID = discordUser.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                User data = await SpookVooperAPI.Users.GetUser(SVID);
                int Total_XP = data.post_likes + data.comment_likes + (data.twitch_message_xp * 4) + (data.discord_commends * 5) + (data.discord_message_xp * 2) + (data.discord_game_xp / 100);
                 #pragma warning disable IDE0004
                decimal Ratio_Messages = (decimal)data.discord_message_xp / (decimal)data.discord_message_count;
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
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }

        [Command("experience")]
        public async Task ExperienceUser(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (ctx.User.Id != 470203136771096596)
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
                        int Total_XP = data.post_likes + data.comment_likes + (data.twitch_message_xp * 4) + (data.discord_commends * 5) + (data.discord_message_xp * 2) + (data.discord_game_xp / 100);
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
                    int Total_XP = data.post_likes + data.comment_likes + (data.twitch_message_xp * 4) + (data.discord_commends * 5) + (data.discord_message_xp * 2) + (data.discord_game_xp / 100);
                    #pragma warning disable IDE0004
                    decimal Ratio_Messages = (decimal)data.discord_message_xp / (decimal)data.discord_message_count;
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
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }
        /*
        [Command("leaderboard")]
        public async Task Leaderboard(CommandContext ctx, int remove)
        {
            if (ctx.User.Id != 470203136771096596)
            {
                IDictionary<string, Tuple<string, string>> usersDict = new Dictionary<string, Tuple<string, string>>();

                HtmlWeb web = new HtmlWeb();

                HtmlDocument doc = web.Load("https://spookvooper.com/Leaderboard/Index/0");

                List<string> listimages = new List<string>();
                List<string> listnames = new List<string>();

                foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table[@class='table']"))
                {
                    ///This is the table.    
                    foreach (HtmlNode rows in table.SelectNodes("tbody"))
                    {
                        ///This is the row.
                        foreach (HtmlNode row in rows.SelectNodes("tr"))
                        {
                            ///This the cell.
                            foreach (HtmlNode column in row.SelectNodes("td"))
                            {
                                foreach (HtmlNode pfp in column.SelectNodes("//img[@class='govpfp']"))
                                {
                                    listimages.Add(pfp.GetAttributeValue("src", ""));
                                }
                                foreach (HtmlNode name in column.SelectNodes("//a"))
                                {
                                    if (name.HasClass("nav - link") == false)
                                    {
                                        listnames.Add(name.InnerText.Trim());
                                    }
                                }
                            }
                        }
                    }
                }

                listnames.RemoveAll(t => t == "SpookVooper" || t == "Home" || t == "News" || t == "Game" || t == "Exchange" || t == "Community" || t == "Next" || t == "Privacy" || t == "Forums" || t == "Government" || t == "Leaderboard" || t == "Districts" || t == "People" || t == "My Account" || t == "My Page" || t == "Find" || t == "Groups" || t == "Create" || t == "My Groups" || t == "Register" || t == "Log in");

                var cleannames = listnames.Distinct().ToList();
                var cleanpfps = listimages.Distinct().ToList();

                var namespfps = cleannames.Zip(cleanpfps, (k, v) => new { Key = k, Value = v })
                    .ToDictionary(x => x.Key, x => x.Value);


                foreach (var item in namespfps)
                {
                    string SVID = await SpookVooperAPI.Users.GetSVIDFromUsername(item.Value);
                    usersDict.Add(item.Key, Tuple.Create(SVID, item.Value));
                }

                IDictionary<string, Tuple<int, decimal, string>> XPDict = new Dictionary<string, Tuple<int, decimal, string>>();

                foreach (var item in usersDict)
                {
                    User Data = await SpookVooperAPI.Users.GetUser(item.Value.Item1);
                    int Total_XP = Data.post_likes + Data.comment_likes + (Data.twitch_message_xp * 4) + (Data.discord_commends * 5) + (Data.discord_message_xp * 2) + (Data.discord_game_xp / 100);
                    decimal Ratio_Messages = (decimal)Data.discord_message_xp / (decimal)Data.discord_message_count;
                    decimal multiplier = (decimal)Math.Pow(10, Convert.ToDouble(2));
                    decimal Ratio_Messages_Rounded = (decimal)(Math.Ceiling(Ratio_Messages * multiplier) / multiplier);
                    XPDict.Add(item.Key, Tuple.Create(Total_XP, Ratio_Messages_Rounded, item.Value.Item2));
                }

                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Title = "SpookVooper Leaderboard",
                    Description = "XP Leaderboard from [SpookVooper.com](https://spookvooper.com/Leaderboard/Index/0)",
                    Color = new DiscordColor("2CC26C")
                };

                var sortedDict = from entry in XPDict orderby entry.Value.Item1 ascending select entry;

                foreach (var item in sortedDict.Skip(25 - remove).Reverse())
                {
                    embed.AddField($"{item.Key}:",
                        $"{item.Value.Item1} XP\n1:{item.Value.Item2} Message to Message XP");
                }

                await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }
        */
        [Command("leaderboardloop")]
        public async Task LeaderboardLoop(CommandContext ctx)
        {
            if (ctx.User.Id != 470203136771096596)
            {
                bool test = ctx.Member.IsOwner;
                if (test == false)
                {
                    await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
                }
                else
                {
                    while (true)
                    {
                        Dictionary<string, string> usersDict = new Dictionary<string, string>
            {
                { "Xboy", "9062fe43-75b0-4f26-a8fd-6a1cdd6883a2" },
                { "Voopmont", "d88f1221-deb9-4f17-a789-f79f6dc02c11" },
                { "Tyco", "c9535d5e-1769-40ea-a3d4-6b73775eb086" },
                { "Dan", "c094e9bd-c021-443f-8138-3433e9ba8b04" },
                { "Coca", "e1616412-c384-4b00-b443-b8940423df67" }
            };

                        IDictionary<string, Tuple<int, decimal>> XPDict = new Dictionary<string, Tuple<int, decimal>>();

                        foreach (var item in usersDict)
                        {
                            User Data = await SpookVooperAPI.Users.GetUser(item.Value);
                            int Total_XP = Data.post_likes + Data.comment_likes + (Data.twitch_message_xp * 4) + (Data.discord_commends * 5) + (Data.discord_message_xp * 2) + (Data.discord_game_xp / 100);
                            decimal Ratio_Messages = (decimal)Data.discord_message_xp / (decimal)Data.discord_message_count;
                            decimal multiplier = (decimal)Math.Pow(10, Convert.ToDouble(2));
                            decimal Ratio_Messages_Rounded = (decimal)(Math.Ceiling(Ratio_Messages * multiplier) / multiplier);
                            XPDict.Add(item.Key, Tuple.Create(Total_XP, Ratio_Messages_Rounded));
                        }

                        DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                        {
                            Title = "SpookVooper Leaderboard",
                            Description = "XP Leaderboard from [SpookVooper.com](https://spookvooper.com/Leaderboard/Index/0)",
                            Color = new DiscordColor("2CC26C")
                        };

                        IOrderedEnumerable<KeyValuePair<string, Tuple<int, decimal>>> sortedDict = from entry in XPDict orderby entry.Value.Item1 ascending select entry;

                        foreach (var item in sortedDict.Reverse())
                        {
                            embed.AddField($"{item.Key}:",
                                $"{item.Value.Item1} XP\n1:{item.Value.Item2} Message to Message XP");
                        }

                        await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                    }
                }
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }
    }
}

