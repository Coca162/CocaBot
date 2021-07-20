using SpookVooper.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;

namespace Valour.Commands
{
    class Leaderboard : CommandModuleBase
    {
        // this is fine for now but it should be migrated to the CocoBot Shared system so discord can use it

        readonly Dictionary<string, string> usersDict = new Dictionary<string, string>
        {
                { "Xboy", "u-9062fe43-75b0-4f26-a8fd-6a1cdd6883a2" },
                { "Voopmont", "u-d88f1221-deb9-4f17-a789-f79f6dc02c11" },
                { "Tyco", "u-c9535d5e-1769-40ea-a3d4-6b73775eb086" },
                { "Dan", "u-c094e9bd-c021-443f-8138-3433e9ba8b04" },
                { "Coca", "u-e1616412-c384-4b00-b443-b8940423df67" }
        };

        [Command("leaderboard")]
        [Alias("lb", "l")]
        public async Task LeaderboardMain(CommandContext ctx)
        {
            Dictionary<string, Tuple<int, decimal>> XPDict = new Dictionary<string, Tuple<int, decimal>>();

            foreach (var item in usersDict)
            {
                User user = new User(item.Value);
                var data = await user.GetSnapshotAsync();
                int Total_XP = data.post_likes + data.comment_likes + (data.twitch_message_xp * 4) + (data.discord_commends * 5) + (data.discord_message_xp * 2) + (data.discord_game_xp / 100);
#pragma warning disable IDE0004
                decimal Ratio_Messages = (decimal)data.discord_message_xp / (decimal)data.discord_message_count;
#pragma warning restore IDE0004
                decimal multiplier = (decimal)Math.Pow(10, Convert.ToDouble(2));
                decimal Ratio_Messages_Rounded = (Math.Ceiling(Ratio_Messages * multiplier) / multiplier);
                XPDict.Add(item.Key, Tuple.Create(Total_XP, Ratio_Messages_Rounded));
            }

            string msg = "SpookVooper XP Leaderboard (https://spookvooper.com/Leaderboard/Index/0):\n |User|XP|Message:XP|\n|-|-|-|";

            IOrderedEnumerable<KeyValuePair<string, Tuple<int, decimal>>> sortedDict = from entry in XPDict orderby entry.Value.Item1 ascending select entry;

            foreach ((string key, Tuple<int, decimal> value) in sortedDict.Reverse())
            {
                msg += $"\n|{key}|{value.Item1}|1:{value.Item2 * 2}|";
            }

            await ctx.ReplyAsync(msg).ConfigureAwait(false);
        }
    }
}
