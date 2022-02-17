using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ValourSharp
{
    public class CommandInfo
    {
        public CommandInfo(MethodInfo method)
        {
            Method = method;
            Parameters = method.GetParameters();
            AllowBots = Method.GetCustomAttribute(typeof(AllowBots), false) is not null;
            AllowSelf = Method.GetCustomAttribute(typeof(AllowSelf), false) is not null;
        }

        public CommandInfo(Dictionary<string, CommandInfo> groupcCommands)
            => GroupCommands = new Dictionary<string, CommandInfo>(groupcCommands, StringComparer.InvariantCultureIgnoreCase);

        public CommandInfo(Dictionary<string, CommandInfo> groupcCommands, MethodInfo method) : this(method)
            => GroupCommands = new Dictionary<string, CommandInfo>(groupcCommands, StringComparer.InvariantCultureIgnoreCase);


        public readonly Dictionary<string, CommandInfo>? GroupCommands { get; }

        public readonly MethodInfo? Method { get; }

        public readonly ParameterInfo[] Parameters { get; }

        public readonly bool AllowBots { get; }

        public readonly bool AllowSelf { get; }
    }
}
