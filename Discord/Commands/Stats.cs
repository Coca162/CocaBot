using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpookVooper.Api.Entities;
using SpookVooper.Api.Entities.Groups;
using static Discord.DiscordTools;
using static Shared.Commands.Get;
using static Shared.Commands.Register;
using static Shared.Commands.Verify;
using static Shared.Main;
using static Shared.Tools;

namespace Discord.Commands
{
    public class Stats : BaseCommandModule
    {
        [Command("statistics"), Aliases("stat", "stats", "statistic")]
        [Description("Gets basic information about a entity")]
        [Priority(1)]
        public async Task StatisticsDiscord(CommandContext ctx, [Description("A User (works with only id)")] DiscordUser discordUser)
        {
            string discord = await DiscordToSVID(discordUser.Id);
            Dictionary<SVIDTypes, string> svids = new();
            if (discord != "") svids.Add(SVIDTypes.User, discord);
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

        public static async Task Statistics(Dictionary<SVIDTypes, string> svids, DiscordMessage message)
        {
            foreach(var svid in svids)
            {
                switch (svid.Key)
                {
                    case SVIDTypes.Group:
                        Group group = new Group(svid.Value);
                        GroupSnapshot groupSnapshot = await group.GetSnapshotAsync();
                        string defaultRoleId = groupSnapshot.Default_Role_Id != null ? groupSnapshot.Default_Role_Id.ToString() : "none";

                        DiscordEmbedBuilder.EmbedAuthor groupIconURL = new DiscordEmbedBuilder.EmbedAuthor { Name = $"{groupSnapshot.Name} Statistics" };
                        DiscordEmbedBuilder.EmbedThumbnail groupThumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = groupSnapshot.Image_Url };

                        DiscordEmbedBuilder groupEmbed = new DiscordEmbedBuilder
                        {
                            Description = $"Statistics for Group [{groupSnapshot.Name}](https://spookvooper.com/User/Info?svid={group.Id})",
                            Color = new DiscordColor("2CC26C"),
                            Author = groupIconURL,
                            Thumbnail = groupThumbnail
                        };

                        string groupDescription;
                        if (string.IsNullOrEmpty(groupSnapshot.Description)) groupDescription = " ";
                        else
                            groupDescription = groupSnapshot.Description.Length <= 1024 ? groupSnapshot.Description : groupSnapshot.Description.Substring(0, 1024);

                        groupEmbed.AddField(
                            "IDs",
                            $"Name: {groupSnapshot.Name}\nSpookVooper ID: {group.Id}\nOwner SVID: {groupSnapshot.Owner_Id}\nDisctrict: {groupSnapshot.District_Id}\nDefault Role Id: {defaultRoleId}");
                        groupEmbed.AddField(
                            "Group",
                            $"Joinable: {groupSnapshot.Open}\nType: {groupSnapshot.Group_Category}\n[Profile Image]({groupSnapshot.Image_Url})");
                        if (EcoGroups.Contains(groupSnapshot.Group_Category))
                        {
                            groupEmbed.AddField(
                            "Economy",
                            $"Balance: ¢{groupSnapshot.Credits}\nInvested: ¢{groupSnapshot.Credits_Invested}");
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
                        User user = new User(svid.Value);
                        UserSnapshot userSnapshot = await user.GetSnapshotAsync();
                        string userName = await user.GetNameAsync();
                        string discordId = userSnapshot.discord_id != null ? userSnapshot.discord_id.ToString() : "none";

                        DiscordEmbedBuilder.EmbedAuthor userIconURL = new DiscordEmbedBuilder.EmbedAuthor { Name = $"{userName} Statistics" };
                        DiscordEmbedBuilder.EmbedThumbnail userThumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = userSnapshot.Image_Url };

                        DiscordEmbedBuilder userEmbed = new DiscordEmbedBuilder
                        {
                            Description = $"Statistics for [{userName}](https://spookvooper.com/User/Info?svid={user.Id})'s SpookVooper account",
                            Color = new DiscordColor("2CC26C"),
                            Author = userIconURL,
                            Thumbnail = userThumbnail
                        };

                        userEmbed.AddField(
                            "IDs",
                            $"SpookVooper Name: {userSnapshot.UserName}\nSpookVooper ID: {user.Id}\nDiscord ID: {discordId}\nMinecraft ID: {userSnapshot.minecraft_id}\n Twitch ID: {userSnapshot.twitch_id}\n NationStates: {userSnapshot.nationstate}");
                        userEmbed.AddField(
                            "Discord",
                            $"Message XP: {userSnapshot.discord_message_xp}\nMessages: {userSnapshot.discord_message_count}\nGame XP: {userSnapshot.discord_game_xp}\nCommends: {userSnapshot.discord_commends}\nCommends Sent: {userSnapshot.discord_commends_sent}\nBans: {userSnapshot.discord_ban_count}");
                        userEmbed.AddField(
                            "SpookVooper",
                            $"Balance: ¢{userSnapshot.Credits}\nDistrict: {userSnapshot.district}\nPost Likes: {userSnapshot.post_likes}\nComment Likes: {userSnapshot.comment_likes}\nAPI Use: {userSnapshot.api_use_count}\n[Profile Image]({userSnapshot.Image_Url})");
                        userEmbed.AddField(
                            "Twitch",
                            $"Twitch XP: {userSnapshot.twitch_message_xp}\nTwitch Messages: {userSnapshot.twitch_messages}");
                        if (!string.IsNullOrEmpty(userSnapshot.description))
                        {
                            userEmbed.AddField(
                                "User's Description",
                                userSnapshot.description.Length <= 1024 ? userSnapshot.description : userSnapshot.description.Substring(0, 1024));
                        }

                        await message.RespondAsync(userEmbed).ConfigureAwait(false);
                        break;
                }
            }
        }
    }
}
