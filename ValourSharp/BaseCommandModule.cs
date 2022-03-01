using Valour.Api.Items.Messages;

namespace ValourSharp;

public abstract class BaseCommandModule
{ 
    public virtual Task BeforeCommandAsync(PlanetMessage ctx)
        => Task.CompletedTask;

    public virtual Task AfterCommandAsync(PlanetMessage ctx)
        => Task.CompletedTask;
}
