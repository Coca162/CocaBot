using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using static Discord.Bot;
using System.Net.Http;
using static Shared.Tools;

namespace Discord.Events;

public static class MessageEvents
{
    public static async Task HandleMessage(string ubiKey, MessageCreateEventArgs e)
    {
        if (e.Author.IsBot) return;
        
        // send role data too for senator/gov pay & for district level UBI

        DiscordGuild server = await Client.GetGuildAsync(798307000206360588);

        DiscordMember member = await server.GetMemberAsync(e.Author.Id);

        await Task.Delay(10000);

        await e.Channel.GetMessageAsync(e.Message.Id);

        if (member is null)
        {
            await GetData($"https://ubi.vtech.cf/new_message?id={e.Author.Id}&name={e.Author.Username}&key={ubiKey}");
            return;
        }

        string end = "";

        foreach (string rolename in member.Roles.Select(x => x.Name))
        {
            end += $"{rolename}|";
        }

        // removes the last "|" symbol
        end = end[0..^1];

        await GetData($"https://ubi.vtech.cf/new_message?id={e.Author.Id}&name={e.Author.Username}&key={ubiKey}&roledata={end}");
    }
}

