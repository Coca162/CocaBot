using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpookVooper.Api.Entities;
using SpookVooper.Api.Entities.Groups;
using static Discord.DiscordTools;
using static Shared.Tools;
using Shared;

namespace Discord.Commands;
public class Stats : BaseCommandModule
{
    public CocaBotWebContext db { private get; set; }

    [Command("statistics"), Aliases("stat", "stats", "statistic")]
    [Description("Gets basic information about a entity"), GeneralBlacklist()]
    [Priority(1)]
    public async Task StatisticsDiscord(CommandContext ctx, [Description("A User (works with only id)")] DiscordUser discordUser)
    {
        string discord = await DiscordToSVID(discordUser.Id, db);
        List<string> svids = new();
        if (discord != "") svids.Add(discord);
        else svids = await ConvertToSVIDs(discordUser.Username);

        await Statistics(svids, ctx.Message).ConfigureAwait(false);
    }

    [Command("statistics")]
    [Priority(0)]
    public async Task StatisticsAll(CommandContext ctx,
        [RemainingText, Description("A Entity (Either SVID, Name or if empty just you)")] string input)
    {
        if (input == null)
        {
            await StatisticsDiscord(ctx, ctx.User).ConfigureAwait(false); return;
        }

        var svids = await ConvertToSVIDs(input);

        await Statistics(svids, ctx.Message).ConfigureAwait(false);
    }

    public static string[] EcoGroups =
    {
            "Companies",
            "District"
        };

    public static async Task Statistics(List<string> svids, DiscordMessage message)
    {
        foreach (var svid in svids)
        {
            switch (SVIDToType(svid))
            {
                case SVIDTypes.Group:
                    Group group = new(svid);
                    GroupSnapshot groupSnapshot = await group.GetSnapshotAsync();
                    string defaultRoleId = groupSnapshot.DefaultRoleId != null ? groupSnapshot.DefaultRoleId.ToString() : "none";

                    DiscordEmbedBuilder.EmbedAuthor groupIconUrl = new() { Name = $"{groupSnapshot.Name} Statistics" };
                    DiscordEmbedBuilder.EmbedThumbnail groupThumbnail = new() { Url = groupSnapshot.Avatar };

                    DiscordEmbedBuilder groupEmbed = new()
                    {
                        Description = $"Statistics for Group [{groupSnapshot.Name}](https://spookvooper.com/User/Info?svid={group.Id})",
                        Color = new DiscordColor("2CC26C"),
                        Author = groupIconUrl,
                        Thumbnail = groupThumbnail
                    };

                    string groupDescription;
                    if (string.IsNullOrEmpty(groupSnapshot.Description)) groupDescription = " ";
                    else
                        groupDescription = groupSnapshot.Description.Length <= 1024 ? groupSnapshot.Description : groupSnapshot.Description.Substring(0, 1024);

                    groupEmbed.AddField(
                        "IDs",
                        $"Name: {groupSnapshot.Name}\nSpookVooper ID: {group.Id}\nOwner SVID: {groupSnapshot.OwnerId}\nDisctrict: {groupSnapshot.DistrictId}\nDefault Role Id: {defaultRoleId}");
                    groupEmbed.AddField(
                        "Group",
                        $"Joinable: {groupSnapshot.Open}\nType: {groupSnapshot.GroupCategory}\n[Profile Image]({groupSnapshot.Avatar})");
                    if (EcoGroups.Contains(groupSnapshot.GroupCategory))
                    {
                        groupEmbed.AddField(
                        "Economy",
                        $"Balance: ¢{groupSnapshot.Credits}\nInvested: ¢{groupSnapshot.CreditsInvested}");
                    }
                    if (!string.IsNullOrEmpty(groupSnapshot.Description))
                    {
                        groupEmbed.AddField(
                            "Groups's Description",
                            groupSnapshot.Description.Length <= 1024 ? groupSnapshot.Description : groupSnapshot.Description.Substring(0, 1024));
                    }

                    await message.RespondAsync(groupEmbed).ConfigureAwait(false);
                    break;
                case SVIDTypes.User:
                    SpookVooper.Api.Entities.User user = new(svid);
                    UserSnapshot userSnapshot = await user.GetSnapshotAsync();
                    string userName = await user.GetNameAsync();
                    string discordId = userSnapshot.DiscordID != null ? userSnapshot.DiscordID.ToString() : "none";

                    DiscordEmbedBuilder.EmbedAuthor userIconUrl = new() { Name = $"{userName} Statistics" };
                    DiscordEmbedBuilder.EmbedThumbnail userThumbnail = new() { Url = userSnapshot.Avatar };

                    DiscordEmbedBuilder userEmbed = new()
                    {
                        Description = $"Statistics for [{userName}](https://spookvooper.com/User/Info?svid={user.Id})'s SpookVooper account",
                        Color = new DiscordColor("2CC26C"),
                        Author = userIconUrl,
                        Thumbnail = userThumbnail
                    };

                    userEmbed.AddField(
                        "IDs",
                        $"SpookVooper Name: {userSnapshot.Username}\nSpookVooper ID: {user.Id}\nDiscord ID: {discordId}\nMinecraft ID: {userSnapshot.MinecraftId}\n Twitch ID: {userSnapshot.TwitchID}\n NationStates: {userSnapshot.Nationstate}");
                    userEmbed.AddField(
                        "Discord",
                        $"Message XP: {userSnapshot.DiscordMessageXp}\nMessages: {userSnapshot.DiscordMessageCount}\nGame XP: {userSnapshot.DiscordGameXP}\nCommends: {userSnapshot.DiscordCommends}\nCommends Sent: {userSnapshot.DiscordCommendsSent}\nBans: {userSnapshot.DiscordBanCount}");
                    userEmbed.AddField(
                        "SpookVooper",
                        $"Balance: ¢{userSnapshot.Credits}\nDistrict: {userSnapshot.District}\nPost Likes: {userSnapshot.PostLikes}\nComment Likes: {userSnapshot.CommentLikes}\nAPI Use: {userSnapshot.ApiUseCount}\n[Profile Image]({userSnapshot.Avatar})");
                    userEmbed.AddField(
                        "Twitch",
                        $"Twitch XP: {userSnapshot.TwitchMessageXP}\nTwitch Messages: {userSnapshot.TwitchMessages}");
                    if (!string.IsNullOrEmpty(userSnapshot.Description))
                    {
                        userEmbed.AddField(
                            "User's Description",
                            userSnapshot.Description.Length <= 1024 ? userSnapshot.Description : userSnapshot.Description.Substring(0, 1024));
                    }

                    await message.RespondAsync(userEmbed).ConfigureAwait(false);
                    break;
            }
        }
    }
}
