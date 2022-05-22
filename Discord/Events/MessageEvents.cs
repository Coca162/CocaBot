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

        Message msg = new(e.Message.Content, e.Author.Id, e.Author.Username, member.Roles.Select(x => x.Name).ToList(), e.Channel.Name, e.Channel.Id, e.Guild.Name, e.Guild.Id);

        HttpRequestMessage httpRequestMessage = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://ubi.vtech.cf/new_message"),
            Headers = {
                { HttpRequestHeader.Authorization.ToString(), ubiKey },
                { HttpRequestHeader.Accept.ToString(), "application/json" },
            },
            Content = new StringContent(JsonSerializer.Serialize(msg))
        };

        HttpClient client = Bot.ServiceProvider.GetRequiredService<HttpClient>();

        await client.SendAsync(httpRequestMessage);
        return;
    }

    public class Message
    {
        public Message(string content, ulong userId, string username, List<string> roles, string channelName, ulong channelId, string serverName, ulong serverId)
        {
            Content = content;
            UserId = userId.ToString();
            Username = username;
            Roles = roles;
            ChannelName = channelName;
            ChannelId = channelId.ToString();
            ServerName = serverName;
            ServerId = serverId.ToString();
        }

        public string Content { get; init; }

        public string UserId { get; init; }

        public string Username { get; init; }

        public List<string> Roles { get; init; }

        public string ChannelName { get; init; }

        public string ChannelId { get; init; }

        public string ServerName { get; init; }

        public string ServerId { get; init; }
    }
}

