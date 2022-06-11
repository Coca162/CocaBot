using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Shared;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using static System.Math;
using System.Net.Http;
using LanguageExt;

namespace Discord.Commands;
public class Jury : BaseCommandModule
{
    public HttpClient httpClient;
    public IUbiUserAPI ubiUsers;
    public IUbiRoles<ulong> gettingUser;
    private readonly Random rnd = new();

    [Command("jury"), Description("Get 4 jurors")]
    public async Task JuryCommand(CommandContext ctx)
        => await JuryCommand(ctx, 4);

    [Command("jury"), Description("Get the amount of jurors you want")]
    public async Task JuryCommand(CommandContext ctx, int amount)
    {
        await ctx.TriggerTypingAsync();

        var everyone = await ubiUsers.GetAllAsyncEnumerable();

        List<(string id, int tickets)> filtered =
            await everyone.Where(x => x.Roles.Contains("Jury")
                                   && (long)x.LastSent > DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 604800)
                          .Select(x => (x.DiscordId, 100 + (int)Pow((int)x.XP, 0.5)))
                          .ToListAsync();

        if (filtered.Count < amount)
        {
            await ctx.RespondAsync("Too many jurors requested!");
            return;
        }

        int sumTickets = filtered.Sum(x => x.tickets);

        List<(string id, int xp)> finals = new();

        while (finals.Count != amount)
        {
            finals.Add(PickUserRandomlyFromTickets(filtered, ref sumTickets));
        }

        string message = $"Selected {amount} jurors to be picked!";

        foreach ((string id, int tickets) user in finals)
        {
            Option<IUbiMember<ulong>> possibleMember = await gettingUser.TryGetMemberAsync(ulong.Parse(user.id));

            int tickets = user.tickets;

            do
            {
                possibleMember.Match(
                member => message += $"\n{member.ToString()} ({tickets} Scaled XP)",
                async () =>
                {
                    (string id, tickets) = PickUserRandomlyFromTickets(filtered, ref sumTickets);

                    possibleMember = await gettingUser.TryGetMemberAsync(ulong.Parse(id));
                });
            }
            while (possibleMember.IsNone);
        }

        await ctx.RespondAsync(message);
    }

    private (string id, int xp) PickUserRandomlyFromTickets(List<(string id, int tickets)> filtered, ref int sumTickets)
    {
        int targetTickets = rnd.Next(sumTickets);

        int totalTickets = 0;
        foreach (var user in filtered)
        {
            totalTickets += user.tickets;
            if (IsUserAboveTarget(targetTickets, totalTickets))
            {
                filtered.Remove(user);
                sumTickets -= user.tickets;
                return user;
            }
        }

        throw new Exception("How");
    }

    private static bool IsUserAboveTarget(int targetTickets, int totalTickets)
    {
        return totalTickets >= targetTickets;
    }
}