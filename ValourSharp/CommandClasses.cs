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
        public CommandModule(string[] names, CommandInfo[] commands, CheckBaseAttribute[] checks)
        {
            Names = names;
            ModuleCommands = commands;
            _checks = checks;
        }

        public CommandModule(string[] names, CommandInfo[] groupCommands, CheckBaseAttribute[] checks, Dictionary<string, CommandModule> subGroups) : this(names, groupCommands, checks)
            => SubGroups = subGroups;

        public string[] Names { get; }

        private CheckBaseAttribute[] _checks;

        public CheckBaseAttribute[] Checks { get => _checks; }

        public CommandModule CombineChecks(IEnumerable<CheckBaseAttribute> newChecks)
        {
            _checks = _checks.Concat(newChecks).ToArray();

            if (SubGroups is not null)
                foreach (var module in SubGroups.DistinctBy(x => x.Value))
                    module.Value.CombineChecks(newChecks);

            return this;
        }

        public CommandInfo[] ModuleCommands { get; }

        public Dictionary<string, CommandModule>? SubGroups { get; }
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
