using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace CocaBot.Commands
{
    public class ShortBasic : BaseCommandModule
    {
        [Command("p")]
        public async Task P(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Pong! {ctx.Client.Ping}ms").ConfigureAwait(false);
        }

        [Command("ver")]
        public async Task Ver(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("This CocaBot is version 1.4.1").ConfigureAwait(false);
        }

        [Command("v")]
        public async Task V(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("This CocaBot is version 1.4.1").ConfigureAwait(false);
        }
    }
}
