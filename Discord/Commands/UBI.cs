﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Shared;
using System.Threading.Tasks;
using static Discord.DiscordTools;
using static Shared.Commands.Balance;
using System.Text.Json.Serialization;
using static SpookVooper.Api.SpookVooperAPI;
using System.Collections.Generic;
using System;
using System.Linq;
using static Discord.Program;

namespace Discord.Commands;
public class UBI : BaseCommandModule
{
    public CocaBotWebContext db { private get; set; }

    public class JacobUBILeaderboardItem
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("xp")]
        public int XP { get; set; }

        [JsonPropertyName("messages")]
        public int Messages { get; set; }

        [JsonPropertyName("messagexp")]
        public int MessageXP { get; set; }
    }

    public class JacobUBILeaderboardData
    {
        [JsonPropertyName("Users")]
        public List<JacobUBILeaderboardItem> Users { get; set; }
    }

    public class JacobUBIXPData
    {
        [JsonPropertyName("XP")]
        public int XP { get; set; }

        [JsonPropertyName("Messages Sent")]
        public int MessagesSent { get; set; }
        [JsonPropertyName("Message Xp")]
        public int MessageXP { get; set; }

        [JsonPropertyName("Current Rank")]
        public string CurrentRank { get; set; }

        [JsonPropertyName("Daily UBI")]
        public int DailyUBI { get; set; }
    }

    public class LeaderboardUserGet
    {
        [JsonPropertyName("position")]
        public int Ranking { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }
    }

    public class User
    {
        [JsonPropertyName("xp")]
        public int XP { get; set; }

        [JsonPropertyName("LastSent")]
        public double LastSent { get; set; }

        [JsonPropertyName("XP")]
        public string SVID { get; set; }

        [JsonPropertyName("rank")]
        public string Rank { get; set; }

        [JsonPropertyName("messages")]
        public int Messages { get; set; }

        [JsonPropertyName("stars")]
        public int Stars { get; set; }

        [JsonPropertyName("discordid")]
        public long DiscordId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }

        [JsonPropertyName("messagexp")]
        public int MessageXP { get; set; }
    }

    [Command("leaderboard"), Aliases("lb"), GeneralBlacklist()]
    public async Task LBString(CommandContext ctx)
    {
        JacobUBILeaderboardData data = await GetDataFromJson<JacobUBILeaderboardData>($"https://ubi.vtech.cf/api/leaderboard");

        DiscordEmbedBuilder embed = new()
        {
            Title = "Leaderboard",
            Description = "[Full List](https://ubi.vtech.cf/leaderboard)",
            Color = new DiscordColor("2CC26C")
        };

        foreach(JacobUBILeaderboardItem item in data.Users)
        {
            embed.AddField(item.Name, $"{item.XP} XP (1 : {Math.Round(CalculateRatio(item.MessageXP, item.Messages), 2)})");
        }

        ctx.RespondAsync(embed);
    }


    [Group("xp"), Description("Get's xp"), Aliases("do")]
    public class XP : BaseCommandModule
    {
        [GroupCommand, Priority(1)]
        public async Task XPString(CommandContext ctx, DiscordUser user)
        {
            JacobUBIXPData data = await GetDataFromJson<JacobUBIXPData>($"https://ubi.vtech.cf/get_xp_info?id={user.Id}");

            int ranking = (await GetDataFromJson<LeaderboardUserGet>($"https://ubi.vtech.cf/api/leaderboard/getuser?id={user.Id}")).Ranking;

            DiscordEmbedBuilder embed = new()
            {
                Title = $"{data.XP} XP (Rank {ranking} {data.CurrentRank})",
                Color = new DiscordColor("2CC26C")
            };

            embed.AddField("Messages", $"{data.MessagesSent}");
            embed.AddField("Message To XP Ratio", $"1 : {Math.Round(CalculateRatio(data.MessageXP, data.MessagesSent), 2)}");
            embed.AddField("Daily UBI", $"¢{data.DailyUBI}");

            ctx.RespondAsync(embed);
        }

        [GroupCommand, Priority(0)]
        public async Task XPString(CommandContext ctx) => XPString(ctx, ctx.User);

        [Command("average"), Description("Get the averages for xp data")]
        public async Task XPAvarage(CommandContext ctx)
        {
            var everyone = (await GetDataFromJson<JacobHourlyUserData>($"https://ubi.vtech.cf/all_user_data?key={UBIKey}")).Users;

            DiscordEmbedBuilder embed = new()
            {
                Title = $"Average of {everyone.Count} People",
                Color = new DiscordColor("2CC26C")
            };

            embed.AddField("XP", $"{Math.Round(everyone.Average(x => x.Xp))}");
            embed.AddField("Messages", $"{Math.Round(everyone.Average(x => x.Messages))}");
            embed.AddField("XP To Message", $"1 : {Math.Round(everyone.Average(x => CalculateRatio(x.MessageXp, x.Messages)), 2)}");
            embed.AddField("Daily UBI", $"¢{Math.Round(everyone.Average(x => x.DailyUBI), 2)}");
            
            ctx.RespondAsync(embed);
        }
    }

    private static decimal CalculateRatio(int messageXP, int messages) =>
#pragma warning disable IDE0004
        (decimal)(messageXP) / (decimal)(messages + 1);
#pragma warning restore IDE0004

    public static double Median<T>(IEnumerable<T> source)
    {
        int count = source.Count();

        source = source.OrderBy(n => n);

        int midpoint = count / 2;
        if (count % 2 == 0)
            return (Convert.ToDouble(source.ElementAt(midpoint - 1)) + Convert.ToDouble(source.ElementAt(midpoint))) / 2.0;
        else
            return Convert.ToDouble(source.ElementAt(midpoint));
    }
}