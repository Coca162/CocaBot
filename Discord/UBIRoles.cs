using DSharpPlus;
using DSharpPlus.Entities;
using LanguageExt;
using Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Discord;

public class UbiRoles : IUbiRoles<ulong>
{
    private readonly DiscordClient _discordClient;

    public UbiRoles(DiscordClient client) =>
        _discordClient = client;

    public ReadOnlyDictionary<string, ulong> RankNamesToIds { get; } = new(new Dictionary<string, ulong>()
    {
        { "spleen", 894632235326656552 },
        { "crab", 894632423776731157 },
        { "corgi", 894632541552791632 },
        { "gaty", 894632641477894155 },
        { "oof", 894632682330423377 }
    });

    public async Task<Option<IUbiMember<ulong>>> TryGetMemberAsync(ulong id)
    {
        DiscordGuild guild = await _discordClient.GetGuildAsync(798307000206360588);
        DiscordMember member;
        try
        {
            member = await guild.GetMemberAsync(id);
        }
        catch (DSharpPlus.Exceptions.DiscordException)
        {
            return Option<IUbiMember<ulong>>.None;
        }
        return new DiscordUbiMember(member);
    }
}

public class DiscordUbiMember : IUbiMember<ulong>
{
    public DiscordUbiMember(DiscordMember member)
    {
        Member = member;
        Id = Member.Id;
        Roles = member.Roles.Select(x => x.Id);
    }

    public DiscordMember Member { get; }

    public ulong Id { get; }

    public IEnumerable<ulong> Roles { get; }

    public async Task GrantRoleAsync(ulong id)
    {
        DiscordRole role = Member.Guild.GetRole(id);
        await Member.GrantRoleAsync(role);
    }

    public async Task RevokeRangeAsync(IEnumerable<ulong> ToRemove)
    {
        foreach (ulong id in ToRemove)
        {
            DiscordRole role = Member.Guild.GetRole(id);
            await Member.RevokeRoleAsync(role);
        }
    }

    public override string ToString() 
        => $"{Member.Username}#{Member.Discriminator}";
}