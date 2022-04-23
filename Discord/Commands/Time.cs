using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

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
            await SVTime(ctx, DateTimeOffset.Now);
        else if (DateTimeOffset.TryParse(date, out DateTimeOffset converted))
            await SVTime(ctx, converted);
    }

    [Command("sv")]
    [Description("sv time to normal time")]
    public async Task ReverseSVTime(CommandContext ctx, [RemainingText] string date)
    {
        if (string.IsNullOrWhiteSpace(date)) return;

        long time = new DateTimeOffset(Convert.ToDateTime(date)).ToUnixTimeSeconds();

        long reversed = ReverseSVTime(time);

        var final = DateTimeOffset.FromUnixTimeSeconds(reversed).UtcDateTime;
        await ctx.RespondAsync($"Reversed Time: {final:dddd, dd MMMM yyyy HH:mm:ss}");
    }

    private static async Task SVTime(CommandContext ctx, DateTimeOffset time)
    {
        //Adjust unix epoch date by adding on January 1st, 1970
        //modified by however much we are doing (1 : 3 in this case)
        //and then add on today in unix time by that much as well/time newyear
        long vooperTime = ConvertToSVTime(time);

        var date = DateTimeOffset.FromUnixTimeSeconds(vooperTime).UtcDateTime;
        await ctx.RespondAsync($"Vooperian Time: {date:dddd, dd MMMM yyyy HH:mm:ss}");
    }

    private static long ConvertToSVTime(DateTimeOffset time) => 
        vooperEpoch + (time.ToUnixTimeSeconds() * 3);

    private static long ReverseSVTime(long time) =>
        (time - vooperEpoch) / 3;
}