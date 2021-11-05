using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class GeneralBlacklist : CheckBaseAttribute
{
    public GeneralBlacklist() { }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return Task.FromResult(!(ctx.Channel.Name == "general") || help);
    }
}