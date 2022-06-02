using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using static Discord.Bot;
using System.Net.Http;
using static Shared.HttpClientExtensions;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using DSharpPlus;
using Discord.Commands;

namespace Discord.Events;

public static class ComponentInteractionEvents
{
    public static async Task XpGraphs(ComponentInteractionCreateEventArgs e)
    {
        string userId = e.Id[28..];
        if (!e.Id.StartsWith("graphType "))
            return;

        var msg = await e.Channel.GetMessageAsync(ulong.Parse(e.Id[10..28]));

        Random rnd = new();
        int randomNumber = rnd.Next(1000000);

        Graphs graphs = new()
        {
            _httpClient = new HttpClient()
        };

        DiscordMessageBuilder message = e.Values.First() switch
        {
            "xp" => await graphs.GenerateCumulativeXpGraph(ulong.Parse(userId)),
            "msg" => await graphs.GenerateCumulativeMessageGraph(ulong.Parse(userId)),
            "ncxp" => new DiscordMessageBuilder() { Content = $"https://ubi.vtech.cf/graph.png?userid={userId}&type=noncumulativexp&random={randomNumber}" },
            "ncmsg" => new DiscordMessageBuilder() { Content = $"https://ubi.vtech.cf/graph.png?userid={userId}&type=noncumulativemessages&random={randomNumber}" },
            _ => new DiscordMessageBuilder() { Content = "Error! Ask Coca why this happening." }
        };

        if (userId != e.User.Id.ToString())
        {
            var response = new DiscordInteractionResponseBuilder(message.WithContent($"<@{userId}>'s Graph")) { IsEphemeral = true };
            var file = message.Files.FirstOrDefault();
            if (file is not null)
                response.AddFile(file.FileName, file.Stream);

            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
            return;
        }

        await msg.ModifyAsync(message, attachments: Enumerable.Empty<DiscordAttachment>());

        await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource);
    }
}

