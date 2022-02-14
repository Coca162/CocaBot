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

namespace ValourSharp;

public static class CommandHandler
{
    public static Dictionary<string, CommandInfo> Commands = new(StringComparer.InvariantCultureIgnoreCase);

    public static async Task MessageHandler(PlanetMessage ctx)
    {
        var sender = await (await ctx.GetAuthorAsync()).GetUserAsync();

        var matches = Prefixes.Where(prefix => ctx.Content.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));

        if (!matches.Any()) return;

        int prefixLength = matches.Max().Length;

        List<string> stringArgs = ctx.Content[prefixLength..].Split(null).ToList();

        bool isCommand = GetCommand(ref stringArgs, Commands, out CommandInfo? command, out string commandName);

        if (!isCommand || command.Method is null ||
       command.Parameters.SkipWhile(x => x.ParameterType == typeof(PlanetMessage)).Count() != stringArgs.Count ||
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
                var mention = mentions.FirstOrDefault();

                if (mention is null || mention.Position != length + 3) continue;

                args[i] = await mention.ConvertToObject(parameter);

                mentions.RemoveAt(0);
            }
            else
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(parameter.ParameterType);
                args[i] = typeConverter.ConvertFrom(rawargs);
            }

            length += rawargs.Length + 1;
        }

        command.Method.Invoke(null, args);
    }

    private static bool GetCommand(ref List<string> stringArgs, Dictionary<string, CommandInfo> commands, out CommandInfo? command, out string commandName)
    {
        commandName = stringArgs[0];
        stringArgs.RemoveAt(0);

        if (!commands.TryGetValue(commandName, out command)) return false;

        if (command.GroupCommands is not null)
        {
            var currentArgs = stringArgs;
            var currentCommand = command;
            if (GetCommand(ref stringArgs, command.GroupCommands, out command, out commandName)) return true;

            stringArgs = currentArgs;
            command = currentCommand;
        };

        return true;
    }

    private static bool IsValourType(this ParameterInfo parameter) => 
        parameter.ParameterType == typeof(User) || parameter.ParameterType == typeof(PlanetMember) || parameter.ParameterType == typeof(PlanetCategory) || parameter.ParameterType == typeof(PlanetChatChannel) || parameter.ParameterType == typeof(PlanetRole);

    private static async Task<object> ConvertToObject(this Mention mention, ParameterInfo parameter) => mention.Type switch
    {
        //User
        MentionType.Member when parameter.ParameterType == typeof(User) => await (await PlanetMember.FindAsync(mention.Target_Id)).GetUserAsync(),
        //Member
        MentionType.Member when parameter.ParameterType == typeof(PlanetMember) => await PlanetMember.FindAsync(mention.Target_Id),
        MentionType.Category when parameter.ParameterType == typeof(PlanetCategory) => await PlanetCategory.FindAsync(mention.Target_Id),
        MentionType.Channel when parameter.ParameterType == typeof(PlanetChatChannel) => await PlanetChatChannel.FindAsync(mention.Target_Id),
        MentionType.Role when parameter.ParameterType == typeof(PlanetRole) => await PlanetRole.FindAsync(mention.Target_Id),
        _ => throw new NotSupportedException("Unrecognized mention and paramater pair"),
    };
}
