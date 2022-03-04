using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Api.Items.Messages;

namespace Valour;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class MyAttribute : ValourSharp.CheckBaseAttribute
{
    public override async Task<bool> ExecuteCheckAsync(PlanetMessage ctx)
    {
        await ctx.ReplyAsync("this is a attribute!");
        return true;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class MyAttribute2 : ValourSharp.CheckBaseAttribute
{
    public override async Task<bool> ExecuteCheckAsync(PlanetMessage ctx)
    {
        await ctx.ReplyAsync("this is another attribute!");
        return false;
    }
}