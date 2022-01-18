using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Valour.Api.Items.Messages;
using Valour.Api.Items.Planets.Members;

namespace Valour;

public static class Commands
{
    public static Dictionary<string[], MethodInfo> commands = new();

    public static void RegisterCommand(MethodInfo command, params string[] names) => 
        commands.Add(names, command);

    public static void RegisterCommands()
    {
        var assembly = Assembly.GetCallingAssembly();
        var methods = assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)).ToList();

        methods.RemoveAll(x => x.IsAbstract | !x.GetCustomAttributes(typeof(Command)).Any());

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttributes(typeof(Command)).Single() as Command;
            commands.Add(attribute.Names, method);
            //TODO make it so it checks if you used the remainder incorrectly
        }
    }

    [Command("confirm", "conf")]
    public static async Task Confirm(PlanetMessage ctx)
    {
        //do shit
        await ctx.ReplyAsync("Linked Account!");
    }

    [Command("call")]
    public static async Task Call(PlanetMessage ctx, [Remainder] string call)
    {
        await ctx.ReplyAsync(call);
    }

    [Command("call1")]
    public static async Task Call1(PlanetMessage ctx, string call)
    {
        await ctx.ReplyAsync(call);
    }

    [Command("decimal")]
    public static async Task Decimal(PlanetMessage ctx, decimal number)
    {
        await ctx.ReplyAsync(number.ToString());
    }

    [Command("member")]
    public static async Task Member(PlanetMessage ctx, Api.Items.Users.User member)
    {
        await ctx.ReplyAsync(member.Id + member.Name);
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class Command : Attribute
{
    public string[] Names { get; set; }

    public Command(params string[] names)
    {
        this.Names = names;
    }
}

[AttributeUsage(AttributeTargets.Parameter)]
public class Remainder : Attribute
{
    public Remainder() { }
}

[AttributeUsage(AttributeTargets.Method)]
public class AllowBots : Attribute
{
    public AllowBots() { }
}

[AttributeUsage(AttributeTargets.Method)]
public class AllowSelf : Attribute
{
    public AllowSelf() { }
}