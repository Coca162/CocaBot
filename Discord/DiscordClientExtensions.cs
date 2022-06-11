using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Events;

namespace Discord;

public static class DiscordClientExtensions
{
    public static void SetUpEvents(this DiscordClient client)
    {
        client.Ready += async (s, _) =>
        {
            Console.WriteLine("CocaBot on!");
        };

        client.ComponentInteractionCreated += (_, eventArgs) => 
            Task.Run(() => ComponentInteractionEvents.XpGraphs(eventArgs));

        if (!Program.prod) return;

        client.Ready += (client, _) =>
        {
            Task.Run(async () =>
            {
                await TimedEvents.SetTimer(client);
            });
            return Task.CompletedTask;
        };

        client.MessageCreated += (client, eventArgs) =>
            Task.Run(() => MessageEvents.HandleMessage(Program.UBIKey, client, eventArgs));
    }
}