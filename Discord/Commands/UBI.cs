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

namespace Discord.Commands;
public class UBI : BaseCommandModule
{
    public CocaBotWebContext db { private get; set; }

    public class JacobUBILeaderboardItem
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        // hehe lowercase
        [JsonPropertyName("value")]
        public string value { get; set; }
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

        [JsonPropertyName("Current Rank")]
        public string CurrentRank { get; set; }

        [JsonPropertyName("Daily UBI")]
        public int DailyUBI { get; set; }
    }

    [Command("leaderboard"), Aliases("lb")]
    public async Task LBString(CommandContext ctx)
    {

        JacobUBILeaderboardData data = await GetDataFromJson<JacobUBILeaderboardData>($"https://ubi.vtech.cf/leaderboard");

        DiscordEmbedBuilder Embed = new();

        Embed.Title = $"Leaderboard";

        foreach(JacobUBILeaderboardItem item in data.Users)
        {
            Embed.AddField(item.Name, item.value);
        }

        ctx.RespondAsync(Embed);
    }

    [Command("xp")]
    [Priority(1)]
    public async Task XPString(CommandContext ctx, DiscordUser user)
    {

        JacobUBIXPData data = await GetDataFromJson<JacobUBIXPData>($"https://ubi.vtech.cf/get_xp_info?id={user.Id}");

        DiscordEmbedBuilder Embed = new();

        Embed.Title = $"{user.Username}'s xp";

        Embed.AddField("XP", data.XP.ToString());
        Embed.AddField("Messages Sent", data.MessagesSent.ToString());
        Embed.AddField("Current Rank", data.CurrentRank);
        Embed.AddField("Daily UBI", $"¢{data.DailyUBI}");

        ctx.RespondAsync(Embed);
    }

    [Command("xp")]
    [Priority(0)]
    public async Task XPString(CommandContext ctx)
    {
        XPString(ctx, ctx.User);
    }
}
