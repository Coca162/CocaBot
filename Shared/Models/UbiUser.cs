using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shared.Models;

public class UbiUser
{
    [JsonPropertyName("xp")]
    public decimal XP { get; set; }

    [JsonPropertyName("messages")]
    public int MessagesSent { get; set; }

    [JsonPropertyName("minutes")]
    public int MinutesSpoken { get; set; }

    [JsonPropertyName("totalchars")]
    public int TotalChars { get; set; }

    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; }

    [JsonPropertyName("rank")]
    public string Rank { get; set; }


    [JsonPropertyName("charsthisminute")]
    public int CharsInCurrentMinute { get; set; }

    [JsonPropertyName("LastSent")]
    public double LastSent { get; set; }

    [JsonPropertyName("messagexp")]
    public double MessageXp { get; set; }

    [JsonPropertyName("templinkcode")]
    public string TempLinkCode { get; set; }


    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("svid")]
    public string SVID { get; set; }


    [JsonPropertyName("discordid")]
    public string DiscordId { get; set; }

    [JsonPropertyName("discord_message_xp")]
    public double DiscordMessageXp { get; set; }

    [JsonPropertyName("discordmessages")]
    public int DiscordMessages { get; set; }


    [JsonPropertyName("valour_id")]
    public string ValourId { get; set; }

    [JsonPropertyName("valour_message_xp")]
    public double ValourMessageXp { get; set; }

    [JsonPropertyName("valourmessages")]
    public int ValourMessages { get; set; }
}
