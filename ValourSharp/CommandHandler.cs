using System.Reflection;
using System.ComponentModel;
using Valour.Shared.Items.Messages.Mentions;
using Valour.Api.Client;
using Valour.Api.Items.Users;
using Valour.Api.Items.Messages;
using Valour.Api.Items.Planets.Members;
using Valour.Api.Items.Planets.Channels;
using static ValourSharp.Start;

namespace ValourSharp;

public static class CommandHandler
{
    public static Dictionary<string, CommandModule> Commands { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);

    public static async Task MessageHandler(PlanetMessage ctx)
    {
        var sender = await (await ctx.GetAuthorAsync()).GetUserAsync();

        var matches = Prefixes!.Where(prefix => ctx.Content.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));

        if (!matches.Any()) return;

        int prefixLength = matches.Max()!.Length;

        List<string> stringArgs = ctx.Content[prefixLength..].Split(null).ToList();

        var tuple = GetModule(ref stringArgs);

        if (tuple is null) return;
        var (module, name) = tuple.Value;

        foreach (var check in module!.Checks)
            if (!await check.ExecuteCheckAsync(ctx)) return;

        foreach (var command in module!.ModuleCommands)
        {
            if ((sender.Bot && command.AllowBots) ||
               (ValourClient.Self == sender && command.AllowSelf)) return;

            object[] args = new object[command.Parameters.Length];

            int length = prefixLength + name!.Length + 1;
            List<Mention> mentions = ctx.Mentions;
            int contexts = 0;
            for (int i = 0; i < command.Parameters.Length; i++)
            {
                ParameterInfo parameter = command.Parameters[i];

                if (parameter.ParameterType == typeof(PlanetMessage))
                {
                    contexts++;
                    args[i] = ctx;
                    continue;
                }

                var position = i - contexts;
                if (parameter.ParameterType == typeof(string) && parameter.GetCustomAttribute(typeof(Remainder), false) is not null)
                {
                    args[i] = string.Join(' ', stringArgs.GetRange(position, stringArgs.Count - position));
                    continue;
                }

                string rawargs = stringArgs[position];
                if (parameter.IsValourType())
                {
                    Mention? mention = mentions.FirstOrDefault();

                    if (mention is null || mention.Position != length + 3) break;

                    object? type = await mention.ConvertToObject(parameter);
                    if (type is null) break;
                    args[i] = type;

                    mentions.RemoveAt(0);
                }
                else
                {
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(parameter.ParameterType);
                    var arg = typeConverter.ConvertFromInvariantString(rawargs);
                  
                    if (arg is null) break;
                    args[i] = arg;
                }

                length += rawargs.Length + 1;
            }

            ConstructorInfo? constructor = command.Method.DeclaringType!.GetConstructor(Type.EmptyTypes);
            object classObject = constructor!.Invoke(Array.Empty<object>());

            if (classObject is BaseCommandModule before)
                await before.BeforeCommandAsync(ctx);

            command.Method.Invoke(classObject, args);

            if (classObject is BaseCommandModule after)
                await after.AfterCommandAsync(ctx);
        }
    }

    private static (CommandModule module, string name)? GetModule(ref List<string> stringArgs)
    {
        string name = stringArgs[0];
        if (!Commands.TryGetValue(name, out CommandModule? module)) return null;
        stringArgs.RemoveAt(0);

        for (var modules = module.SubGroups; module.SubGroups is not null && stringArgs.Count != 0; modules = module.SubGroups)
        {
            string possibleName = stringArgs[0];
            if (!modules!.TryGetValue(possibleName, out CommandModule? possibleModule)) break;
            name = possibleName;
            module = possibleModule;
            stringArgs.RemoveAt(0);
        }

        return (module, name);
    }

    private static bool GetCommands(ref List<string> stringArgs, Dictionary<string, CommandModule> modules, out CommandModule? module, out string names)
    {
        names = stringArgs[0];
        if (!modules.TryGetValue(names, out module)) return false;
        stringArgs.RemoveAt(0);

        if (module.SubGroups is not null && module.SubGroups.Count != 0 && stringArgs.Count != 0)
        {
            List<string> currentArgs = new(stringArgs);
            var currentModule = module;
            if (GetCommands(ref stringArgs, module.SubGroups, out module, out names)) return true;

            stringArgs = currentArgs;
            module = currentModule;
            names = stringArgs[0];
        };

        return true;
    }

    private static bool IsValourType(this ParameterInfo parameter) => 
        parameter.ParameterType == typeof(User) || parameter.ParameterType == typeof(PlanetMember) || parameter.ParameterType == typeof(PlanetCategory) || parameter.ParameterType == typeof(PlanetChatChannel) || parameter.ParameterType == typeof(PlanetRole);

    private static async Task<object?> ConvertToObject(this Mention mention, ParameterInfo parameter) => mention.Type switch
    {
        //User
        MentionType.Member when parameter.ParameterType == typeof(User) => await (await PlanetMember.FindAsync(mention.Target_Id)).GetUserAsync(),
        //Member
        MentionType.Member when parameter.ParameterType == typeof(PlanetMember) => await PlanetMember.FindAsync(mention.Target_Id),
        MentionType.Category when parameter.ParameterType == typeof(PlanetCategory) => await PlanetCategory.FindAsync(mention.Target_Id),
        MentionType.Channel when parameter.ParameterType == typeof(PlanetChatChannel) => await PlanetChatChannel.FindAsync(mention.Target_Id),
        MentionType.Role when parameter.ParameterType == typeof(PlanetRole) => await PlanetRole.FindAsync(mention.Target_Id),
        _ => null
    };
}
