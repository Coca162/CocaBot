﻿using LanguageExt;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Shared;

public interface IUbiRoles<T>
{
    ReadOnlyDictionary<string, T> RankNamesToIds { get; }

    Task<Option<IUbiMember<T>>> TryGetMemberAsync(T id);
}

public interface IUbiMember<T>
{
    T Id { get; }

    IEnumerable<T> Roles { get; }

    Task GrantRoleAsync(T id);

    Task RevokeRangeAsync(IEnumerable<T> ToRemove);
}