using Valour.Api.Items.Messages;

namespace ValourSharp;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class Command : Attribute
{
    public string Name { get; set; }

    public Command(string name)
    {
        this.Name = name;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class GroupCommand : Attribute
{
    public GroupCommand() {}
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class Aliases : Attribute
{
    public string[] Names { get; set; }

    public Aliases(params string[] aliases)
    {
        this.Names = aliases;
    }
}

//TODO
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class Group : Attribute
{
    public string[] Names { get; set; }

    public Group(params string[] names)
    {
        this.Names = names;
    }
}

//TODO
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class Priority : Attribute
{
    public int Ranking { get; set; }

    public Priority(int ranking)
    {
        this.Ranking = ranking;
    }
}

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public class Remainder : Attribute
{
    public Remainder() { }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class AllowBots : Attribute
{
    public AllowBots() { }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class AllowSelf : Attribute
{
    public AllowSelf() { }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public abstract class CheckBaseAttribute : Attribute
{
    public abstract Task<bool> ExecuteCheckAsync(PlanetMessage ctx);
}