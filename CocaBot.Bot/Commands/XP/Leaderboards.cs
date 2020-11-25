using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using SpookVooper.Api.Entities;
using System.Timers;

namespace CocaBot.Commands
{
    public class Leaderboards : BaseCommandModule
    {
        Timer timer;

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
                    int Total_XP = 

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
                        $"{item.Value.Item1} XP\n1:{item.Value.Item2 * 2} Message to XP");
                }

                await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }
        */

        [Command("leaderboard"), EnableBlacklist]
        [Aliases("lb", "l")]
        public async Task Leaderboard(CommandContext ctx)
        {
            bool test = ctx.Member.IsOwner;
            if (test == false)
            {
                await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
            }
            else
            {
                Console.WriteLine(DateTime.Now.Minute);
                Console.WriteLine("fuck");
                Dictionary<string, string> usersDict = new Dictionary<string, string>
                                        {
                { "Xboy", "9062fe43-75b0-4f26-a8fd-6a1cdd6883a2" },
                { "Voopmont", "d88f1221-deb9-4f17-a789-f79f6dc02c11" },
                { "Tyco", "c9535d5e-1769-40ea-a3d4-6b73775eb086" },
                { "Dan", "c094e9bd-c021-443f-8138-3433e9ba8b04" },
                { "Coca", "e1616412-c384-4b00-b443-b8940423df67" }
            };


                Dictionary<string, Tuple<int, decimal>> XPDict = new Dictionary<string, Tuple<int, decimal>>();

                foreach (var item in usersDict)
                {
                    User Data = await SpookVooperAPI.Users.GetUser(item.Value);
                    int Total_XP = Data.GetTotalXP();
#pragma warning disable IDE0004
                    decimal Ratio_Messages = (decimal)Data.discord_message_xp / (decimal)Data.discord_message_count;
#pragma warning restore IDE0004
                    decimal multiplier = (decimal)Math.Pow(10, Convert.ToDouble(2));
                    decimal Ratio_Messages_Rounded = (Math.Ceiling(Ratio_Messages * multiplier) / multiplier);
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
                        $"{item.Value.Item1} XP\n1:{item.Value.Item2 * 2} Message to XP");
                }

                await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
            }
        }

        [Command("leaderboardloop"), EnableBlacklist]
        [Aliases("lbloop", "ll", "lbl")]
        public async Task LeaderboardLoop(CommandContext ctx, float time, float start)
        {
            bool test = ctx.Member.IsOwner;
            if (test == false)
            {
                await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
            }
            else
            {
                float delay = (DateTime.Now.Minute + (DateTime.Now.Second / 60) - start) * 60000;
                if (delay < 0) { delay = (float)(start + (60 - (DateTime.Now.Minute + (DateTime.Now.Second / 60))) * 60000); };
                await Task.Delay((int)delay);
                LeadeboardUpdaterAsync(ctx);
                if (time == 60) { time = 0; };
                timer = new Timer();
                timer.Interval = (float)(time * 60000);
                timer.Enabled = true;
                timer.Elapsed += (sender, e) => LeadeboardUpdaterAsync(ctx);
            }
        }
        private async void LeadeboardUpdaterAsync(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.Minute);
            Console.WriteLine("fuck");
            Dictionary<string, string> usersDict = new Dictionary<string, string>
                                        {
                { "Xboy", "9062fe43-75b0-4f26-a8fd-6a1cdd6883a2" },
                { "Voopmont", "d88f1221-deb9-4f17-a789-f79f6dc02c11" },
                { "Tyco", "c9535d5e-1769-40ea-a3d4-6b73775eb086" },
                { "Dan", "c094e9bd-c021-443f-8138-3433e9ba8b04" },
                { "Coca", "e1616412-c384-4b00-b443-b8940423df67" }
            };


            Dictionary<string, Tuple<int, decimal>> XPDict = new Dictionary<string, Tuple<int, decimal>>();

            foreach (var item in usersDict)
            {
                User Data = await SpookVooperAPI.Users.GetUser(item.Value);
                int Total_XP = Data.GetTotalXP();
#pragma warning disable IDE0004
                decimal Ratio_Messages = (decimal)Data.discord_message_xp / (decimal)Data.discord_message_count;
#pragma warning restore IDE0004
                decimal multiplier = (decimal)Math.Pow(10, Convert.ToDouble(2));
                decimal Ratio_Messages_Rounded = (Math.Ceiling(Ratio_Messages * multiplier) / multiplier);
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
                    $"{item.Value.Item1} XP\n1:{item.Value.Item2 * 2} Message to XP");
            }

            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }
    }
}

