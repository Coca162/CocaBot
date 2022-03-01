using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ValourSharp
{
    public class CommandModule
    {

        public CommandModule(string[] names, CommandInfo[] groupCommands, CheckBaseAttribute[] checks)
        {
            Names = names;
            ModuleCommands = groupCommands;
            _checks = checks;
        }

        public CommandModule(string[] names, CommandInfo[] groupCommands, CheckBaseAttribute[] checks, Dictionary<string, CommandModule> submodules) : this(names, groupCommands, checks)
            => Submodules = submodules;

        public string[] Names { get; }

        private CheckBaseAttribute[] _checks;

        public CheckBaseAttribute[] Checks { get => _checks; }

        public CommandModule CombineChecks(IEnumerable<CheckBaseAttribute> newChecks)
        {
            _checks = _checks.Concat(newChecks).ToArray();
            return this;
        }

        public CommandInfo[] ModuleCommands { get; }

        public Dictionary<string, CommandModule>? Submodules { get; }
    }

    public class CommandInfo
    {
        public CommandInfo(string name, MethodInfo method)
        {
            Name = name;
            Method = method;

            var priorityAttribute = Method.GetCustomAttribute(typeof(Priority), false) as Priority;
            Priority = priorityAttribute is not null ? priorityAttribute.Ranking : 0;

            Parameters = method.GetParameters();
            AllowBots = Method.GetCustomAttribute(typeof(AllowBots), false) is not null;
            AllowSelf = Method.GetCustomAttribute(typeof(AllowSelf), false) is not null;
        }

        public string Name { get; }

        public int Priority { get; }

        public MethodInfo Method { get; }

        public ParameterInfo[] Parameters { get; }

        public bool AllowBots { get; }

        public bool AllowSelf { get; }
    }
}
