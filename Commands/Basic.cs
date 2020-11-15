using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace CocaBot.Commands
{
    public class Basic : BaseCommandModule
    {
        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Pong! {ctx.Client.Ping}ms").ConfigureAwait(false);
        }

        [Command("version")]
        public async Task Version(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("This CocaBot is version 1.4.1").ConfigureAwait(false);
        }
    }
}
