using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using static Discord.Bot;
using static Shared.Main;
using Timer = System.Timers.Timer;

namespace Discord.Events;

public class TimedEvents
{
    private static readonly Timer UbiRoleTimer = new(1000 * 60 * 30);

    private static readonly Timer StatusTimer = new(1000 * 20);

    public static async Task SetTimer()
    {
        await EstimateOnTheSvMinute();

        await OnTimedStatusEvent();

        StatusTimer.Elapsed += async (object source, ElapsedEventArgs e) => await OnTimedStatusEvent();
        StatusTimer.Enabled = true;

        var roleupdate = new UBIRoleUpdater(new UbiRoles(Client));
        await roleupdate.UpdateHourly();
        UbiRoleTimer.Elapsed += async (object source, ElapsedEventArgs e) => await roleupdate.UpdateHourly();
        UbiRoleTimer.Enabled = true;
    }

    private static async Task OnTimedStatusEvent()
    {
        await Client.UpdateStatusAsync(new DiscordActivity(SVTimeNow(), ActivityType.Playing));
    }

    private static async Task EstimateOnTheSvMinute()
    {
        long svTime = Commands.Time.ConvertToSVTime(DateTimeOffset.UtcNow);
        int roundedToLastMinute = 60 - (int)(svTime % 60);
        if (roundedToLastMinute == 60) return;
        int irlEstimatedWaitTime = roundedToLastMinute / 3;

        await Task.Delay(irlEstimatedWaitTime * 1000 - 100);
    }

    private static string SVTimeNow()
    {
        //Adjust unix epoch date by adding on January 1st, 1970
        //modified by however much we are doing (1 : 3 in this case)
        //and then add on today in unix time by that much as well/time newyear
        long vooperTime = Commands.Time.ConvertToSVTime(DateTimeOffset.UtcNow);

        var date = DateTimeOffset.FromUnixTimeSeconds(vooperTime).UtcDateTime;
        return $"{date:HH:mm dd/MM/yyyy} SV Time";
    }
}

