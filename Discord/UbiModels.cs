using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Discord;

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
    public UBIUser User { get; set; }
}

public class UBIUser
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