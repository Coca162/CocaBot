using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DSharpPlus.Entities;

namespace Discord.Commands;

[Group("time")]
public class Time : BaseCommandModule
{
    //Gets January 1st, 1970 but to Vooper time in seconds from January 1st, 1970
    public const long vooperEpoch = 7194873600;

    [GroupCommand]
    [Description("time to sv time")]
    public async Task SVTime(CommandContext ctx, [RemainingText] string date)
    {
        if (string.IsNullOrWhiteSpace(date))
        {
            await SVTime(ctx, DateTimeOffset.Now);
            return;
        }
        else if (DateTimeOffset.TryParse(date, out DateTimeOffset converted))
        {
            await SVTime(ctx, converted);
            return;
        }

        var eventPair = Events.SingleOrDefault(x => x.Key.ToLowerInvariant() == date.ToLowerInvariant());

        if (eventPair.Key is null) return;
        var (additional, owner) = eventPair.Value;

        long now = ConvertToSVTime();
        long ceiled = VooperYearCeiled();

        long time = ceiled - (31536000 - additional);
        if (time < now) time = ceiled + additional;

        await ctx.RespondAsync($"{eventPair.Key} will be on <t:{time}:F> which is <t:{time}:R>");
    }

    [Command("sv")]
    [Description("sv time to normal time")]
    public async Task ReverseSVTime(CommandContext ctx, [RemainingText] string date)
    {
        if (string.IsNullOrWhiteSpace(date)) return;

        long time = new DateTimeOffset(Convert.ToDateTime(date)).ToUnixTimeSeconds();

        long reversed = ReverseSVTime(time);

        await ctx.RespondAsync($"Reversed Time: <t:{reversed}:F>");
    }

    private static long VooperYearCeiled() => 
        new DateTimeOffset(new DateTime(DateTimeOffset.FromUnixTimeSeconds(ConvertToSVTime()).Year + 1, 1, 1)).ToUnixTimeSeconds();

    private static async Task SVTime(CommandContext ctx, DateTimeOffset time)
    {
        //Adjust unix epoch date by adding on January 1st, 1970
        //modified by however much we are doing (1 : 3 in this case)
        //and then add on today in unix time by that much as well/time newyear
        long vooperTime = ConvertToSVTime(time);

        await ctx.RespondAsync($"Vooperian Time: <t:{vooperTime}:F>");
    }

    private static long ConvertToSVTime() =>
        ConvertToSVTime(DateTimeOffset.UtcNow);

    private static long ConvertToSVTime(DateTimeOffset time) => 
        vooperEpoch + (time.ToUnixTimeSeconds() * 3);

    private static long ReverseSVTime(long time) =>
        (time - vooperEpoch) / 3;

    private static JsonDictionary<string, (long, ulong)> _events;

    public static JsonDictionary<string, (long additional, ulong owner)> Events
    {
        get
        {
            if (_events is not null) return _events;
            _events = new("events.json", JsonSerializer.Deserialize<Dictionary<string, (long, ulong)>>(File.ReadAllText("events.json"), new JsonSerializerOptions { IncludeFields = true }), StringComparer.InvariantCultureIgnoreCase);
            return _events;
        }
    }

    [Group("event")]
    public class Event : BaseCommandModule
    {
        [Command("list"), GeneralBlacklist()]
        public async Task List(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
            {
                Description = "Events:",
                Color = new DiscordColor("2CC26C"),
            };

            foreach ((string name, (long difference, ulong owner)) in Events)
            {
                var date = DateTimeOffset.FromUnixTimeSeconds(31536000 - difference);
                embed.AddField(name, $"Date: {date:dd MMMM}\nOwner: <@{owner}>");
            }

            await ctx.RespondAsync(embed);
        }

        [Command("add"), Priority(0)]
        public async Task Add(CommandContext ctx, string date, [RemainingText] string name)
        {
            if (!DateTime.TryParse(date, out DateTime converted)) return;

            long seconds = (converted.DayOfYear * 86400) + (converted.Hour * 3600) + (converted.Minute * 60) + converted.Second;

            await Add(ctx, seconds, name);
        }

        [Command("add"), Priority(1)]
        public async Task Add(CommandContext ctx, long additional, [RemainingText] string name)
        {
            if (Events.ContainsKey(name))
            {
                await ctx.RespondAsync("Already a event!");
                return;
            }

            await Events.Add(name, (additional, ctx.User.Id));

            var date = DateTimeOffset.FromUnixTimeSeconds(additional);
            await ctx.RespondAsync($"Added event for the date {date:dd MMMM}!");
        }

        [Command("remove")]
        public async Task Remove(CommandContext ctx, [RemainingText] string name)
        {
            if (!Events.ContainsKey(name))
            {
                await ctx.RespondAsync("Not a event!");
                return;
            }

            if (!Events.Any(x => x.Key == name && (x.Value.owner == ctx.User.Id) || ctx.User.Id == 388454632835514380))
            {
                await ctx.RespondAsync("You are not the owner of this event! Do `;time event list` to find the owner!");
                return;
            }

            await Events.Remove(name);
            await ctx.RespondAsync("Removed event!");
        }
    }
}