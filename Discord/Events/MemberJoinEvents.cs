using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using System;
using System.Text.Json;
using DSharpPlus;
using System.Collections.Generic;
using System.IO;

namespace Discord.Events;

public static class MemberJoinEvents
{
    public static int NextHelper { get; set; }

    private static JsonList<ulong> _helpers;

    public static JsonList<ulong> Helpers
    {
        get
        {
            if (_helpers is not null) return _helpers;

            _helpers = new JsonList<ulong>("moi.json", JsonSerializer.Deserialize<IEnumerable<ulong>>(File.ReadAllText("moi.json")));
            return _helpers;
        }
    }

    public static async Task HandleSVJoin(GuildMemberAddEventArgs args)
    {
        await Task.Delay(10000);

        var member = args.Member;
        var server = args.Guild;

        if (member.IsBot || server.Id != 798307000206360588) return;

        DiscordDmChannel dms;
        try
        {
            dms = await member.CreateDmChannelAsync();
        }
        catch (NullReferenceException)
        {
            var channels = await server.GetChannelsAsync();
            var moi = channels.Where(x => x.Id == 870371308393873498).Single();
            await moi.SendMessageAsync($"Failure to send message to {member.Username}#{member.Discriminator} because member doesn't exist (what? get over here <@388454632835514380>).");
            return;
        }
        try
        {
            await dms.SendMessageAsync($"Hi there! Welcome to SpookVooper, a roleplay planet and community with a government and other cool things! If you need any help or have any questions, contact <@{Helpers[NextHelper]}>. Enjoy your stay!");
            NextHelper++;
        }
        catch (UnauthorizedException)
        {
            var channels = await server.GetChannelsAsync();
            var moi = channels.Where(x => x.Id == 870371308393873498).Single();
            await moi.SendMessageAsync($"Failure to send message to {member.Username}#{member.Discriminator} because the user has DMs from server members turned off.");
            return;
        }
    }
}