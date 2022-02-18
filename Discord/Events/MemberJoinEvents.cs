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
    private static HelperList helpers = null;
    public static int NextHelper { get; set; }

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

    public static HelperList Helpers
    {
        get
        {
            if (helpers != null) return helpers;

            helpers = JsonSerializer.Deserialize<HelperList>(File.ReadAllText("moi.json"));
            return helpers;
        }
    }

    public class HelperList : List<ulong>
    {
        public new async Task Add(ulong item)
        {
            base.Add(item);
            await JsonSerializer.SerializeAsync(File.Create("moi.json"), this);
        }

        public new async Task Remove(ulong item)
        {
            base.Add(item);
            await JsonSerializer.SerializeAsync(File.Create("moi.json"), this);
        }
    }
}

