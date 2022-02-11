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

    public static HelperList Helpers { 
        get
        { 
            if (helpers != null) return helpers;

            helpers = JsonSerializer.Deserialize<HelperList>(File.ReadAllText("moi.json"));
            return helpers;
        }
    }

    public class HelperList : List<ulong>
    {
        public new void Add(ulong item)
        {
            base.Add(item);
            File.WriteAllText("moi.json", JsonSerializer.Serialize(this));
        }

        public new void Remove(ulong item)
        {
            base.Add(item);
            File.WriteAllText("moi.json", JsonSerializer.Serialize(this));
        }
    }


    public static async Task HandleSVJoin(GuildMemberAddEventArgs args)
    {
        var member = args.Member;
        var server = args.Guild;

        if (member.IsBot || server.Id != 798307000206360588) return;

        var moi = server.GetChannel(870371308393873498);

        DiscordDmChannel dms;
        try
        {
            dms = await member.CreateDmChannelAsync();
        }
        catch (NullReferenceException)
        {
            await moi.SendMessageAsync($"Failure to send message to {member.Username}#{member.Discriminator} because member doesn't exist (what? get over here <@388454632835514380>).");
            return;
        }
        try
        {
            await dms.SendMessageAsync("this is the message!");
        }
        catch (UnauthorizedException)
        {
            await moi.SendMessageAsync($"Failure to send message to {member.Username}#{member.Discriminator} because the user has DMs from server members turned off.");
            return;
        }
    }
}

