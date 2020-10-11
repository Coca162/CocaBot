using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using SpookVooper.Api;
using System;
using System.Linq;
using System.Net.Sockets;
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
            //Make this into "Username: {variable}" when not lazy
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
            //Make this into "Username: {variable}" when not lazy
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

        [Command("balance")]
        public async Task balanceuserSVuser(CommandContext ctx, string opt, [RemainingText] string Inputname)
        {

            if (opt.ToLower() == "group")
            {
                string SVID = await SpookVooperAPI.Groups.GetSVIDFromName(Inputname);
                var Balance = await SpookVooperAPI.Economy.GetBalance(SVID);
                string SVname = await SpookVooperAPI.Groups.GetName(SVID);
                await ctx.Channel.SendMessageAsync(SVname + " Balance: ¢" + Balance).ConfigureAwait(false);
            }
            else if (opt.ToLower() == "user")
            {
                string SVID = await SpookVooperAPI.Users.GetSVIDFromUsername(Inputname);
                string SVname = await SpookVooperAPI.Users.GetUsername(SVID);
                var Balance = await SpookVooperAPI.Economy.GetBalance(SVID);
                await ctx.Channel.SendMessageAsync(SVname + " Balance: ¢" + Balance).ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"{Inputname} is not a user or a group!").ConfigureAwait(false);
            }
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
            //Make this into "Username: {variable}" when not lazy
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("leaderboardloop")]
        public async Task leaderboardLoop(CommandContext ctx)
        {
            var test = ctx.Member.IsOwner;
            if (test == false)
            {
                await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
            }
            else
            {
                var Xboy = "9062fe43-75b0-4f26-a8fd-6a1cdd6883a2";
                var Voopmont = "d88f1221-deb9-4f17-a789-f79f6dc02c11";
                var Tyco = "c9535d5e-1769-40ea-a3d4-6b73775eb086";
                var Dan = "c094e9bd-c021-443f-8138-3433e9ba8b04";
                var Coca = "e1616412-c384-4b00-b443-b8940423df67";

                while (true)
                {
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

                    await ctx.RespondAsync(Xboy_Total_XP + "\n" + Voopmont_Total_XP + "\n" + Dan_Total_XP + "\n" + Tyco_Total_XP + "\n" + Coca_Total_XP).ConfigureAwait(false);
                    await Task.Delay(3600000);
                }
            }
        }

        [Command("balanceloop")]
        public async Task balanceLoop(CommandContext ctx, DiscordUser discordUser)
        {
            var test = ctx.Member.IsOwner;
            if (test == false)
            {
                await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
            }
            else
            {
                var discordID = discordUser.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);

                while (true)
                {
                    var Balance = await SpookVooperAPI.Economy.GetBalance(SVID);
                    await ctx.Channel.SendMessageAsync(SV_Name + " Balance: ¢" + Balance).ConfigureAwait(false);
                    await Task.Delay(3600000);
                }

            }
        }

        [Command("balanceloop")]
        public async Task balanceLoop(CommandContext ctx, string SVID)
        {
            var test = ctx.Member.IsOwner;
            if (test != false)
            {
                string SV_Name = await SpookVooperAPI.Groups.GetName(SVID);

                while (true)
                {
                    var Balance = await SpookVooperAPI.Economy.GetBalance(SVID);
                    await ctx.Channel.SendMessageAsync(SV_Name + " Balance: ¢" + Balance).ConfigureAwait(false);
                    await Task.Delay(3600000);
                }

            }
            else
            {
                await ctx.RespondAsync("You are not server owner").ConfigureAwait(false);
            }
        }

        [Command("verify")]
        public async Task VerifyCitzenAll(CommandContext ctx, DiscordUser discordUser)
        {
            var ServerID = ctx.Guild.Id;
            if (ServerID == 762075097422495784)
            {
                var discordID = discordUser.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                var Data = await SpookVooperAPI.Users.GetUser(SVID);
                var new_yam_role = ctx.Guild.GetRole(762434338847195138);
                var non_citizen_role = ctx.Guild.GetRole(762739003630944296);

                if (Data.district == "New Yam")
                {
                    var discordName = discordUser.Username;
                    var discordPFP = discordUser.AvatarUrl;

                    await ctx.TriggerTypingAsync();
                    var iconURL = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = discordName,
                        IconUrl = discordPFP,
                    };

                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "You now have the New Yam Citizen role!",
                        Color = new DiscordColor(0x64FF),
                        Author = iconURL
                    };
                    await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                    await ctx.Member.GrantRoleAsync(new_yam_role).ConfigureAwait(false);
                    await ctx.Member.RevokeRoleAsync(non_citizen_role).ConfigureAwait(false);
                }
                else
                {
                    var discordName = discordUser.Username;
                    var discordPFP = discordUser.AvatarUrl;

                    await ctx.RespondAsync($"{discordName} is not a New Yam Citizen!").ConfigureAwait(false);
                    await ctx.TriggerTypingAsync();
                    var iconURL = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = discordName,
                        IconUrl = discordPFP,
                    };

                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "You are a Non-Citizen of New Yam!",
                        Color = new DiscordColor(0x64FF),
                        Author = iconURL
                    };
                    await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                    await ctx.Member.GrantRoleAsync(non_citizen_role).ConfigureAwait(false);
                    await ctx.Member.RevokeRoleAsync(new_yam_role).ConfigureAwait(false);
                }
            }
            else
            {
                var ServerName = ctx.Guild.Name;
                await ctx.RespondAsync($"This is {ServerName} not New Yam Community Server!").ConfigureAwait(false);
            }

        }

        [Command("verify")]
        public async Task VerifyCitzen(CommandContext ctx)
        {
            var ServerID = ctx.Guild.Id;
            if (ServerID == 762075097422495784)
            {
                var discordID = ctx.User.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                var Data = await SpookVooperAPI.Users.GetUser(SVID);
                var new_yam_role = ctx.Guild.GetRole(762434338847195138);
                var non_citizen_role = ctx.Guild.GetRole(762739003630944296);

                if (Data.district == "New Yam")
                {
                    var discordName = ctx.User.Username;
                    var discordPFP = ctx.User.AvatarUrl;

                    await ctx.TriggerTypingAsync();
                    var iconURL = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = discordName,
                        IconUrl = discordPFP,
                    };

                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "You now have the New Yam Citizen role!",
                        Color = new DiscordColor(0x64FF),
                        Author = iconURL
                    };
                    await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                    await ctx.Member.GrantRoleAsync(new_yam_role).ConfigureAwait(false);
                    await ctx.Member.RevokeRoleAsync(non_citizen_role).ConfigureAwait(false);
                }
                else
                {
                    var discordName = ctx.User.Username;
                    var discordPFP = ctx.User.AvatarUrl;

                    await ctx.RespondAsync($"{discordName} is not a New Yam Citizen!").ConfigureAwait(false);
                    await ctx.TriggerTypingAsync();
                    var iconURL = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = discordName,
                        IconUrl = discordPFP,
                    };

                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "You are a Non-Citizen of New Yam!",
                        Color = new DiscordColor(0x64FF),
                        Author = iconURL
                    };
                    await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                    await ctx.Member.GrantRoleAsync(non_citizen_role).ConfigureAwait(false);
                    await ctx.Member.RevokeRoleAsync(new_yam_role).ConfigureAwait(false);
                }
            }
            else
            {
                var ServerName = ctx.Guild.Name;
                await ctx.RespondAsync($"This is {ServerName} not New Yam Community Server!").ConfigureAwait(false);
            }

        }
    }
}

