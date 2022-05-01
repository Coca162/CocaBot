using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Shared;
using System.Threading.Tasks;
using static Shared.Tools;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;
using System.Linq;
using static Discord.Program;
using static System.Math;
using System.Text.Json;
using System.Net.Http;

namespace Discord.Commands;
public class UBI : BaseCommandModule
{
    public CocaBotPoolContext db { private get; set; }

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
            embed.AddField(item.Name, $"{item.XP} XP ({Round(CalculateWPM(item.MessageXP, item.Messages), 2)} WPM)");
        }

        await ctx.RespondAsync(embed);
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
            embed.AddField("Words Per Minute", $"{Round(CalculateWPM(data.MessageXP, data.MessagesSent), 2)}");
            embed.AddField("Daily UBI", $"¢{data.DailyUBI}");

            await ctx.RespondAsync(embed);
        }

        [GroupCommand, Priority(0)]
        public async Task XPString(CommandContext ctx) 
            => await XPString(ctx, ctx.User);

        [Command("average"), Description("Get the averages for xp data")]
        public async Task XPAvarage(CommandContext ctx)
        {
            var everyone = (await GetDataFromJson<JacobHourlyUserData>($"https://ubi.vtech.cf/all_user_data?key={UBIKey}")).Users;

            DiscordEmbedBuilder embed = new()
            {
                Title = $"Average of {everyone.Count} People",
                Color = new DiscordColor("2CC26C")
            };

            embed.AddField("XP", $"{Round(everyone.Average(x => x.Xp))}");
            embed.AddField("Messages", $"{Round(everyone.Average(x => x.Messages))}");
            embed.AddField("Words Per Minute", $"{Round(everyone.Average(x => CalculateWPM(x.MessageXp, x.Messages)), 2)}");
            embed.AddField("Daily UBI", $"¢{Round(everyone.Average(x => x.DailyUBI), 2)}");

            await ctx.RespondAsync(embed);
        }

        [Command("median"), Description("Get the median for xp data")]
        public async Task XPMedian(CommandContext ctx)
        {
            var everyone = (await GetDataFromJson<JacobHourlyUserData>($"https://ubi.vtech.cf/all_user_data?key={UBIKey}")).Users;

            DiscordEmbedBuilder embed = new()
            {
                Title = $"Median of {everyone.Count} People",
                Color = new DiscordColor("2CC26C")
            };

            embed.AddField("XP", $"{Round(Median(everyone.Select(x=>x.Xp), everyone.Count))}");
            embed.AddField("Messages", $"{Round(Median(everyone.Select(x => x.Messages), everyone.Count))}");
            embed.AddField("Words Per Minute", $"{Median(everyone.Select(x => CalculateWPM(x.MessageXp, x.Messages)), everyone.Count)}");
            embed.AddField("Daily UBI", $"¢{Median(everyone.Select(x => x.DailyUBI), everyone.Count)}");

            await ctx.RespondAsync(embed);
        }
    }

    private static float CalculateWPM(int messageXP, int messages)
        => 2f / ((float)messageXP / (messages + 1));

    public static double Median<T>(IEnumerable<T> source, int count)
    {
        source = source.OrderBy(n => n);

        int midpoint = count / 2;
        if (count % 2 == 0)
            return (Convert.ToDouble(source.ElementAt(midpoint - 1)) + Convert.ToDouble(source.ElementAt(midpoint))) / 2.0;
        else
            return Convert.ToDouble(source.ElementAt(midpoint));
    }
}