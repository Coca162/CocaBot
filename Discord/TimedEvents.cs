using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;
using SpookVooper.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static Discord.Bot;
using static Shared.Main;
using static Shared.Tools;
using Timer = System.Timers.Timer;
using Shared.Models;

namespace Discord;

public class TimedEvents
{
    private static Timer timer = new(1000 * 60 * 15);

    private static Entity coca;
    private static bool UBIRoleCheck = true;

    private static DiscordChannel CBTChannel;
    private static DiscordChannel ServerChannel;
    private static DiscordChannel WeeklyChannel;
    private static DiscordChannel DailyChannel;

    public static async Task SetTimer(DiscordConfig ConfigJson)
    {
        CBTChannel = await Client.GetChannelAsync(894634546824376391);
        //ServerChannel = await Client.GetChannelAsync(896605140428128296);
        //WeeklyChannel = await Client.GetChannelAsync(896654987013259324);
        //DailyChannel = await Client.GetChannelAsync(896655216085192784);

        CocaBotContext db = new();
        string svid = "u-e1616412-c384-4b00-b443-b8940423df67";
        coca = new(svid, db.Users.Find(svid).Token + "|" + config.OauthSecret);

        await OnTimedEvent(ConfigJson.JacobUBIKey);
        // Hook up the Elapsed event for the timer. 
        timer.Elapsed += async (object source, ElapsedEventArgs e) => await OnTimedEvent(ConfigJson.JacobUBIKey);
        timer.Enabled = true;
    }

    private static async Task OnTimedEvent(string UBIKey)
    {
        CocaBotContext db = new();

        await Client.UpdateStatusAsync(new DiscordActivity($"{db.Users.Count()} Users!", ActivityType.Watching));

        await CheckTransactionLogger(db);

        if (UBIRoleCheck)
        {
            UBIRoles.UpdateHourly(UBIKey);
            UBIRoleCheck = false;
        }
        else UBIRoleCheck = true;

        //ServerChannel.ModifyAsync(x => x.Name = "Servers: " + Client.Guilds.Count);

        //DateTimeOffset now = DateTimeOffset.Now;

        //int dayOfWekk = now.DayOfWeek == 0 ? 6 : (int)now.DayOfWeek - 1;

        //long lastWeek = now.Subtract(new TimeSpan(dayOfWekk, now.Hour, now.Minute, now.Second, now.Millisecond)).ToUnixTimeSeconds();

        //long lastDay = now.Subtract(new TimeSpan(0, now.Hour, now.Minute, now.Second, now.Millisecond)).ToUnixTimeSeconds();

        //decimal weeklyTransactions = await db.Transactions
        //                                     .Where(x => x.Timestamp > lastWeek || x.Detail.Contains("CocaBot"))
        //                                     .SumAsync(x => x.Amount);

        //decimal dailyTransactions = await db.Transactions
        //                                    .Where(x => x.Timestamp > lastDay || x.Detail.Contains("CocaBot"))
        //                                    .SumAsync(x => x.Amount);

        //WeeklyChannel.ModifyAsync(x => x.Name = "Weekly: " + Humanize((double)weeklyTransactions));

        //DailyChannel.ModifyAsync(x => x.Name = "Daily: " + Humanize((double)dailyTransactions));
    }

    private static async Task CheckTransactionLogger(CocaBotContext db)
    {
        await coca.SendCreditsAsync((decimal)0.00000000000000000000000001, new Entity("g-oldyam"), "CBT Checker");

        await Task.Delay(1000);

        long timestamp = (await db.Transactions.Where(x => x.Detail == "CBT Checker")
                                               .OrderByDescending(x => x.Count)
                                               .FirstAsync())
                                               .Timestamp;

        if (DateTimeOffset.Now - DateTimeOffset.FromUnixTimeSeconds(timestamp) >= TimeSpan.FromMinutes(1))
        {
            await CBTChannel.SendMessageAsync("CBT is down! (<@388454632835514380>)");
        }
    }
}

