using System.Reflection;
using static ValourSharp.CommandHandler;

namespace ValourSharp;

public static class Registration
{
    public static void RegisterCommand(MethodInfo command, params string[] names) =>
        Commands.Add(names, command);

    public static void RegisterCommands()
    {
        var assembly = Assembly.GetCallingAssembly();
        var methods = assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)).ToList();

        methods.RemoveAll(x => x.IsAbstract | !x.GetCustomAttributes(typeof(Command)).Any());

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttributes(typeof(Command)).Single() as Command;
            Commands.Add(attribute.Names, method);
            //TODO make it so it checks if you used the remainder incorrectly
        }
    }
}
