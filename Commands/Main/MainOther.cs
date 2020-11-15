﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using SpookVooper.Api;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SpookVooper.Api.Entities;

namespace CocaBot.Commands
{
    public class MainOther : BaseCommandModule
    {
        [Command("svid")]
        public async Task SVIDUser(CommandContext ctx, DiscordUser discordUser)
        {
            if (ctx.User.Id != 470203136771096596)
            {
                ulong discordID = discordUser.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);

                await ctx.Channel.SendMessageAsync($"{SV_Name}'s SVID: {SVID}").ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }

        [Command("svid")]
        public async Task SVID(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (ctx.User.Id != 470203136771096596)
            {
                if (Inputname != null)
                {
                    string gSVID = await SpookVooperAPI.Groups.GetSVIDFromName(Inputname);
                    string uSVID = await SpookVooperAPI.Users.GetSVIDFromUsername(Inputname);

                    if (uSVID == null && gSVID == null)
                    {
                        await ctx.Channel.SendMessageAsync($"{Inputname} is not a user or a group!").ConfigureAwait(false);
                    }
                    else
                    {
                        await ctx.Channel.SendMessageAsync($"SVID: {gSVID}{uSVID}").ConfigureAwait(false);
                    }
                }
                else
                {
                    ulong discordID = ctx.Member.Id;
                    string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                    string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);

                    await ctx.Channel.SendMessageAsync($"{SV_Name}'s SVID: {SVID}").ConfigureAwait(false);
                }
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }

        [Command("name")]
        public async Task NameUser(CommandContext ctx, DiscordUser discordUser)
        {
            if (ctx.User.Id != 470203136771096596)
            {
                ulong discordID = discordUser.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);

                await ctx.Channel.SendMessageAsync($"{discordUser.Username}'s SV Name: {SV_Name}").ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }

        [Command("name")]
        public async Task Name(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (ctx.User.Id != 470203136771096596)
            {
                if (Inputname != null)
                {
                    string gname = await SpookVooperAPI.Groups.GetName(Inputname);
                    string uname = await SpookVooperAPI.Users.GetUsername(Inputname);

                    if (gname == null && uname == null)
                    {
                        await ctx.Channel.SendMessageAsync($"{Inputname} is not a SVID for a user or a group!").ConfigureAwait(false);
                    }
                    else
                    {
                        await ctx.Channel.SendMessageAsync($"SVID: {gname}{uname}").ConfigureAwait(false);
                    }
                }
                else
                {
                    ulong discordID = ctx.Member.Id;
                    string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                    string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);

                    await ctx.Channel.SendMessageAsync($"{ctx.Member.Username}'s SV Name: {SV_Name}").ConfigureAwait(false);
                }
            }
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }

        [Command("verify")]
        public async Task VerifyUser(CommandContext ctx, string type)
        {
            if (ctx.User.Id != 470203136771096596)
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
                    bool if_senate_role = await SpookVooperAPI.Users.HasDiscordRole(SVID, senate_role);

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
                            Color = new DiscordColor("2CC26C"),
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
                            Color = new DiscordColor("2CC26C"),
                            Author = iconURL
                        };
                        await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                        await ctx.Member.GrantRoleAsync(non_citizen_role).ConfigureAwait(false);
                        await ctx.Member.RevokeRoleAsync(district_role).ConfigureAwait(false);
                    }
                    else if ((if_senate_role == true) && ((type.ToLower() == "senator")))
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
                            Color = new DiscordColor("2CC26C"),
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
                            Color = new DiscordColor("2CC26C"),
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
            else
            {
                await ctx.RespondAsync("fuck off Asdia").ConfigureAwait(false);
            }
        }
    }
}
