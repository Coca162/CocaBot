using System.Reflection;
using static ValourSharp.CommandHandler;

namespace ValourSharp;

public static class Registration
{
    public static void RegisterCommand(MethodInfo command, params string[] names)
    {
        foreach (string name in names)
        {
            Commands.TryAdd(name, new CommandInfo(command));
        }
    }

    public static void RegisterCommands()
    {
        var assembly = Assembly.GetCallingAssembly();
        var methods = assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)).ToList();

        methods.RemoveAll(x => x.IsAbstract || x.GetCustomAttribute(typeof(Command)) is null || (x.DeclaringType is null && x.DeclaringType.GetCustomAttribute(typeof(Group)) is null));

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute(typeof(Command)) as Command;
            RegisterCommand(method, attribute.Names);
            //TODO make it so it checks if you used the remainder incorrectly
        }

        //DO GROUP COMMANDS HERE
    }
}
