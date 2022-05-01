using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using static Discord.Bot;
using static Shared.Main;
using Timer = System.Timers.Timer;

namespace Discord.Events;

public class TimedEvents
{
    private static readonly Timer timer = new(1000 * 60 * 15);

    private static bool UBIRoleCheck = true;

    public static async Task SetTimer()
    {
        await OnTimedEvent();

        timer.Elapsed += async (object source, ElapsedEventArgs e) => await OnTimedEvent();
        timer.Enabled = true;
    }

    private static async Task OnTimedEvent()
    {
        await using CocaBotContext db = new();

        await Client.UpdateStatusAsync(new DiscordActivity($"{Client.Guilds.Count} Guilds! | ;help", ActivityType.Watching));


        if (UBIRoleCheck)
        {
            await UBIRoles.UpdateHourly();
            UBIRoleCheck = false;
        }
        else UBIRoleCheck = true;
    }
}

