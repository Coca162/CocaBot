﻿using System.Linq;
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
using Shared;
using LanguageExt;

namespace Discord.Events;

public static class MessageEvents
{
    public static async Task HandleMessage(string ubiKey, DiscordClient discordClient, MessageCreateEventArgs e)
    {
        if (e.Author.IsBot) return;

        DiscordGuild server = await discordClient.GetGuildAsync(798307000206360588);

        DiscordMember member = await server.GetMemberAsync(e.Author.Id);

        await Task.Delay(10000);

        await e.Channel.GetMessageAsync(e.Message.Id);

        Message msg = new(e.Message.Content, e.Author.Id, e.Author.Username, member.Roles.Select(x => x.Name), e.Channel.Name, e.Channel.Id, e.Guild.Name, e.Guild.Id, e.Message.Attachments.Select(x => x.MediaType));

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
        public Message(string content, ulong userId, string username, IEnumerable<string> roles, string channelName, ulong channelId, string serverName, ulong serverId, IEnumerable<string> attachmentTypes)
        {
            Content = content;
            UserId = userId.ToString();
            Username = username;
            Roles = roles.ToList();
            ChannelName = channelName;
            ChannelId = channelId.ToString();
            ServerName = serverName;
            ServerId = serverId.ToString();
            AttachmentTypes = attachmentTypes.ToList();

        }

        public string Content { get; init; }

        public string UserId { get; init; }

        public string Username { get; init; }

        public IReadOnlyList<string> Roles { get; init; }

        public string ChannelName { get; init; }

        public string ChannelId { get; init; }

        public string ServerName { get; init; }

        public string ServerId { get; init; }

        public IReadOnlyList<string> AttachmentTypes { get; init; }
    }
}

