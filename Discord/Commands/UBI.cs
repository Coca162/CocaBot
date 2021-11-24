using DSharpPlus.CommandsNext;
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
        [JsonPropertyName("messagexp")]
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
            embed.AddField(item.Name, $"{item.XP} XP (1 : {CalculateRatio(item.MessageXP, item.Messages)})");
        }

        ctx.RespondAsync(embed);
    }


    [Group("xp")]
    public class XP : BaseCommandModule
    {
        [GroupCommand, Priority(1), Description("Get's xp"), Aliases("do")]
        public async Task XPString(CommandContext ctx, DiscordUser user)
        {
            JacobUBIXPData data = await GetDataFromJson<JacobUBIXPData>($"https://ubi.vtech.cf/get_xp_info?id={user.Id}");

            LeaderboardUserGet ratioData = await GetDataFromJson<LeaderboardUserGet>($"https://ubi.vtech.cf/api/leaderboard/getuser?id={user.Id}");

            decimal ratioMessagesRounded = CalculateRatio(ratioData.User.MessageXP, ratioData.User.Messages);

            DiscordEmbedBuilder embed = new()
            {
                Title = $"{data.XP} XP (Rank {ratioData.Ranking} {data.CurrentRank})",
                Color = new DiscordColor("2CC26C")
            };

            embed.AddField("Messages", $"{data.MessagesSent}");
            embed.AddField("Message To XP Ratio", $"1 : {ratioMessagesRounded}");
            embed.AddField("Daily UBI", $"¢{data.DailyUBI}");

            ctx.RespondAsync(embed);
        }

        [GroupCommand, Priority(0)]
        public async Task XPString(CommandContext ctx) => XPString(ctx, ctx.User);

        //[Command("avarage"), Description("Get the avarages for xp data")]
        //public async Task XPAvarage(CommandContext ctx)
        //{
        //    var ids = (await GetDataFromJson<JacobHourlyUserData>($"https://ubi.vtech.cf/all_user_data?key={UBIKey}")).Users.Select(x => x.Id);
        //    var everyone = ids.Select(x => );

        //    DiscordEmbedBuilder embed = new()
        //    {
        //        Title = $"Avarage of {ids.Count} People",
        //        Color = new DiscordColor("2CC26C")
        //    };

        //    embed.AddField("XP", $"{data.MessagesSent}");
        //    embed.AddField("Messages", $"{data.MessagesSent}");
        //    embed.AddField("XP To Message", $"{ratioMessagesRounded}%");
        //    embed.AddField("Daily UBI", $"¢{data.DailyUBI}");

        //}
    }

    private static decimal CalculateRatio(int messageXP, int messages)
    {
#pragma warning disable IDE0004
        decimal Ratio_Messages = (decimal)(messageXP) / (decimal)(messages);
#pragma warning restore IDE0004
        decimal multiplier = (decimal)Math.Pow(10, Convert.ToDouble(2));
        decimal ratioMessagesRounded = Math.Ceiling(Ratio_Messages * multiplier) / multiplier;
        return ratioMessagesRounded;
    }

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