using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Discord.Commands;
public class Sudo : BaseCommandModule
{
    [Command("sudo"), Description("Executes a command as another user."), Hidden, RequireOwner, DevOnly, DevModeOnly]
    public async Task Command(CommandContext ctx, 
        [Description("Member to execute as.")] DiscordMember member, 
        [RemainingText, Description("Command text to execute.")] string command)
    {
        if (ctx.Channel.Id != 850578492185509898) return;

        // note the [RemainingText] attribute on the argument.
        // it will capture all the text passed to the command

        // let's trigger a typing indicator to let
        // users know we're working
        await ctx.TriggerTypingAsync();

        // get the command service, we need this for
        // sudo purposes
        var cmds = ctx.CommandsNext;

        // retrieve the command and its arguments from the given string
        var cmd = cmds.FindCommand(command, out var customArgs);

        // create a fake CommandContext
        var fakeContext = cmds.CreateFakeContext(member, ctx.Channel, command, ctx.Prefix, cmd, customArgs);

        // and perform the sudo
        await cmds.ExecuteCommandAsync(fakeContext);
    }
}