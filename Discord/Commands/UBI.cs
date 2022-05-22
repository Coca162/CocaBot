using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Shared;
using System.Threading.Tasks;
using static Shared.HttpClientExtensions;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;
using System.Linq;
using Humanizer;
using static Discord.Program;
using static System.Math;
using System.Text.Json;
using System.Net.Http;
using System.Globalization;
using Shared.Models;

namespace Discord.Commands;
public class UBI : BaseCommandModule
{
    public HttpClient httpClient;
    public IUbiUserAPI api;

    [Command("leaderboard"), Aliases("lb"), GeneralBlacklist()]
    public async Task LBString(CommandContext ctx)
    {
        var data = await api.GetTop10();

        DiscordEmbedBuilder embed = new()
        {
            Title = "Leaderboard",
            Description = "[Full List](https://ubi.vtech.cf/leaderboard)",
            Color = new DiscordColor("2CC26C")
        };

        await foreach(UbiUser item in data)
        {
            embed.AddField(item.Name, $"{(int)item.XP} XP ({Round(CalculatePerMinutes(item.MinutesSpoken, item.MessagesSent), 2)} MPM)");
        }

        await ctx.RespondAsync(embed);
    }


    [Group("xp"), Description("Get's xp"), Aliases("do")]
    public class XP : BaseCommandModule
    {
        public HttpClient httpClient;
        public IUbiUserAPI api;

        [GroupCommand, Priority(1)]
        public async Task XPString(CommandContext ctx, DiscordUser user)
        {
            UbiUser data = await api.GetUserAsync(user.Id);

            DiscordEmbedBuilder embed = new()
            {
                Title = $"{(int)data.XP} XP (Rank {data.Rank.Humanize()})",
                Color = new DiscordColor("2CC26C")
            };



            embed.AddField("Messages", data.MessagesSent.ToString());
            embed.AddField("Active Time", TimeFromMinutes(data.MinutesSpoken).ToString());
            embed.AddField("Messages Per Minute", Math.Round(CalculatePerMinutes(data.MinutesSpoken, data.MessagesSent), 2).ToString());
            embed.AddField("Characters Per Minute", Math.Round(CalculatePerMinutes(data.MinutesSpoken, data.TotalChars), 2).ToString());

            await ctx.RespondAsync(embed);
        }

        [GroupCommand, Priority(0)]
        public async Task XPString(CommandContext ctx) 
            => await XPString(ctx, ctx.User);

        [Command("average"), Description("Get the averages for xp data")]
        public async Task XPAvarage(CommandContext ctx)
        {
            var everyone = await api.GetAllAsync();

            DiscordEmbedBuilder embed = new()
            {
                Title = $"Average of {everyone.Count} People",
                Color = new DiscordColor("2CC26C")
            };

            var filtered = everyone.Where(x => x.MessagesSent != 0 && x.MinutesSpoken != 0);
            int xp = (int)filtered.Average(x => x.XP);

            embed.AddField("XP", $"{xp}");
            embed.AddField("Active Time", TimeFromMinutes(filtered.Average(x => x.MinutesSpoken)).ToString());
            embed.AddField("Messages", Round(filtered.Average(x => x.MessagesSent)).ToString());
            embed.AddField("Messages Per Minute", Math.Round(filtered.Average(x => CalculatePerMinutes(x.MinutesSpoken, x.MessagesSent)), 2).ToString());
            embed.AddField("Characters Per Minute", Math.Round(filtered.Average(x => CalculatePerMinutes(x.MinutesSpoken, x.TotalChars)), 2).ToString());

            await ctx.RespondAsync(embed);
        }

        [Command("median"), Description("Get the median for xp data")]
        public async Task XPMedian(CommandContext ctx)
        {
            var everyone = await api.GetAllAsync();

            DiscordEmbedBuilder embed = new()
            {
                Title = $"Median of {everyone.Count} People",
                Color = new DiscordColor("2CC26C")
            };

            var filtered = everyone.Where(x => x.MessagesSent != 0 && x.MinutesSpoken != 0);
            int count = filtered.Count();
            int xp = (int)Median(filtered.Select(x => x.XP), count);

            embed.AddField("XP", $"{xp}");
            embed.AddField("Active Time", TimeFromMinutes(Median(filtered.Select(x => x.MinutesSpoken), count)).ToString());
            embed.AddField("Messages", Round(Median(filtered.Select(x => x.MessagesSent), count)).ToString());
            embed.AddField("Messages Per Minute", Round(Median(filtered.Select(x => CalculatePerMinutes(x.MinutesSpoken, x.MessagesSent)), count), 2).ToString());
            embed.AddField("Characters Per Minute", Round(Median(filtered.Select(x => CalculatePerMinutes(x.MinutesSpoken, x.TotalChars)), count), 2).ToString());

            await ctx.RespondAsync(embed);
        }
    }

    private static float CalculatePerMinutes(int minutes, int value)
        => (float)value / (minutes + 1);

    public static double Median<T>(IEnumerable<T> source, int count)
    {
        source = source.OrderBy(n => n);

        int midpoint = count / 2;
        if (count % 2 == 0)
            return (Convert.ToDouble(source.ElementAt(midpoint - 1)) + Convert.ToDouble(source.ElementAt(midpoint))) / 2.0;
        else
            return Convert.ToDouble(source.ElementAt(midpoint));
    }

    public static string TimeFromMinutes(double minutes)
    {
        TimeSpan timespan = new(0, (int)minutes, 0);
        return timespan.Humanize(2, CultureInfo.InvariantCulture);
    }
}