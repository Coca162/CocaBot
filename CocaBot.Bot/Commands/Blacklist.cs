using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class EnableBlacklist : CheckBaseAttribute
{
    public EnableBlacklist() { }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        List<float> blacklist = new List<float>();
        blacklist.Add(470203136771096596); //Asdia

        bool blacklisted = false;
        foreach(var id in blacklist)
        {
            if (id == ctx.User.Id)
                blacklisted = true;
        }
        return Task.FromResult(blacklisted == false);
    }
}