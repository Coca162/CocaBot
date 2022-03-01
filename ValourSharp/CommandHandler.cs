using System.Reflection;
using System.ComponentModel;
using Valour.Shared.Items.Messages.Mentions;
using Valour.Api.Client;
using Valour.Api.Items.Users;
using Valour.Api.Items.Messages;
using Valour.Api.Items.Planets.Members;
using Valour.Api.Items.Planets.Channels;
using System.Collections.Concurrent;
using static ValourSharp.Start;
using System.Collections.ObjectModel;

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

        bool isCommand = GetCommands(ref stringArgs, Commands, out CommandModule? module, out string commandName);

        if (!isCommand) return;

        foreach (var check in module!.Checks)
            if (!await check.ExecuteCheckAsync(ctx)) return;

        foreach (var command in module!.ModuleCommands)
        {
            int paramAmount = command.Parameters.SkipWhile(x => x.ParameterType == typeof(PlanetMessage)).Count();
            if (paramAmount != stringArgs.Count ||
               (sender.Bot && command.AllowBots) ||
               (ValourClient.Self == sender && command.AllowSelf)) return;

            object[] args = new object[command.Parameters.Length];

            int length = prefixLength + commandName.Length + 1;
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
                string rawargs = stringArgs[position];

                if (parameter.ParameterType == typeof(string) && parameter.GetCustomAttribute(typeof(Remainder), false) is not null)
                {
                    args[i] = string.Join(' ', stringArgs.GetRange(position, stringArgs.Count - position));
                }
                else if (parameter.IsValourType())
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

            if (stringArgs.Count != args.Length - contexts) return;

            ConstructorInfo? constructor = command.Method.DeclaringType!.GetConstructor(Type.EmptyTypes);
            object classObject = constructor!.Invoke(Array.Empty<object>());


            if (classObject is BaseCommandModule before)
                await before.BeforeCommandAsync(ctx);

            command.Method.Invoke(classObject, args);

            if (classObject is BaseCommandModule after)
                await after.AfterCommandAsync(ctx);
        }
    }

    private static bool GetCommands(ref List<string> stringArgs, Dictionary<string, CommandModule> modules, out CommandModule? module, out string commandName)
    {
        module = null;
        commandName = stringArgs[0];
        stringArgs.RemoveAt(0);

        if (!modules.TryGetValue(commandName, out CommandModule? commandModule)) return false;
        module = commandModule;

        if (commandModule.Submodules is not null && commandModule.Submodules.Count != 0 && stringArgs.Count != 0)
        {
            var currentArgs = stringArgs;
            var currentModule = module;
            var currentCommandName = commandName;
            if (GetCommands(ref stringArgs, commandModule.Submodules, out module, out commandName)) return true;

            stringArgs = currentArgs;
            module = currentModule;
            commandName = currentCommandName;
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
