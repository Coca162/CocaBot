using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Shared;
using System.Threading.Tasks;
using static Shared.HttpClientExtensions;
using System.Collections.Generic;
using System;
using System.Linq;
using static Discord.Program;
using static System.Math;
using System.Net.Http;

namespace Discord.Commands;
public class Jury : BaseCommandModule
{
    public HttpClient httpClient;
    public IUbiUserAPI api;

    [Command("jury"), Description("Get 4 jurors")]
    public async Task JuryCommand(CommandContext ctx) 
        => await JuryCommand(ctx, 4);

    [Command("jury"), Description("Get the amount of jurors you want")]
    public async Task JuryCommand(CommandContext ctx, int amount)
    {
        var everyone = await api.GetAllAsyncEnumerable();

        List<(string id, int xp)> filtered = new();
        int sum = 0;

        await foreach (var item in everyone.Where(x => x.Roles.Contains("Jury")))
        {
            sum += (int)item.XP;
            filtered.Add((item.DiscordId, 100 + (int)Pow((int)item.XP, 0.5)));
        }

        if (filtered.Count < amount) throw new Exception("Too big jury request!");


        List<(string id, int xp)> finals = new();

        while (finals.Count != amount)
        {
            Random rnd = new();
            int number = rnd.Next(0, sum);

            int xpTotal = 0;
            foreach (var item in filtered)
            {
                xpTotal += item.xp;
                if (xpTotal > number)
                {
                    finals.Add(item);
                    filtered.Remove(item);
                    sum -= item.xp;
                    break;
                }
            }
        }

        string message = $"Selected {amount} jurors to be picked!";

        foreach (var (id, xp) in finals)
        {
            DiscordMember member = null;
            try
            {
                member = await ctx.Guild.GetMemberAsync(ulong.Parse(id));
            }
            catch (DSharpPlus.Exceptions.NotFoundException) { }

            message += $"\n{member.Username}#{member.Discriminator} ({xp} Scaled XP)";
        }

        await ctx.RespondAsync(message);
    }
}