using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shared.Models;

public class UbiUser
{
    [JsonPropertyName("xp")]
    public decimal XP { get; init; }

    [JsonPropertyName("messages")]
    public int MessagesSent { get; init; }

    [JsonPropertyName("minutes")]
    public int MinutesSpoken { get; init; }

    [JsonPropertyName("totalchars")]
    public int TotalChars { get; init; }

    [JsonPropertyName("roles")]
    public IReadOnlyCollection<string> Roles { get; init; }

    [JsonPropertyName("rank")]
    public string Rank { get; init; }


    [JsonPropertyName("charsthisminute")]
    public int CharsInCurrentMinute { get; init; }

    [JsonPropertyName("LastSent")]
    public double LastSent { get; init; }

    [JsonPropertyName("messagexp")]
    public double MessageXp { get; init; }

    [JsonPropertyName("templinkcode")]
    public string TempLinkCode { get; init; }


    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("svid")]
    public string SVID { get; init; }


    [JsonPropertyName("discordid")]
    public string DiscordId { get; init; }

    [JsonPropertyName("discord_message_xp")]
    public double DiscordMessageXp { get; init; }

    [JsonPropertyName("discordmessages")]
    public int DiscordMessages { get; init; }


    [JsonPropertyName("valour_id")]
    public string ValourId { get; init; }

    [JsonPropertyName("valour_message_xp")]
    public double ValourMessageXp { get; init; }

    [JsonPropertyName("valourmessages")]
    public int ValourMessages { get; init; }
}
