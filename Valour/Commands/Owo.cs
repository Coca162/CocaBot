using System.Threading.Tasks;
using Valour.Net.CommandHandling;
using Valour.Net.CommandHandling.Attributes;

namespace Valour.Commands
{
    class Owo : CommandModuleBase
    {
        [Command("owo")]
        public async Task OWO(CommandContext ctx, [Remainder] string input)
        {
            await ctx.ReplyAsync(Owoify.Owoifier.Owoify(input, Owoify.Owoifier.OwoifyLevel.Owo)).ConfigureAwait(false);
        }

        [Command("uwu")]
        public async Task UWU(CommandContext ctx, [Remainder] string input)
        {
            await ctx.ReplyAsync(Owoify.Owoifier.Owoify(input, Owoify.Owoifier.OwoifyLevel.Uwu)).ConfigureAwait(false);
        }

        [Command("uvu")]
        public async Task UVU(CommandContext ctx, [Remainder] string input)
        {
            await ctx.ReplyAsync(Owoify.Owoifier.Owoify(input, Owoify.Owoifier.OwoifyLevel.Uvu)).ConfigureAwait(false);
        }

        [Command("iwi")]
        public async Task IWI(CommandContext ctx, int times, string type ,[Remainder] string input)
        {
            string owofied = null;
            switch(type.ToLower())
            {
                case "owo":
                    owofied = Owoify.Owoifier.Owoify(input, Owoify.Owoifier.OwoifyLevel.Owo);
                    break;
                case "uwu":
                    owofied = Owoify.Owoifier.Owoify(input, Owoify.Owoifier.OwoifyLevel.Uwu);
                    break;
                case "uvu":
                    owofied = Owoify.Owoifier.Owoify(input, Owoify.Owoifier.OwoifyLevel.Uvu);
                    break;
                default:
                    await ctx.ReplyAsync("not a valid type!").ConfigureAwait(false);
                    return;
            }
            for (int i = 0; i != times; i++)
            {
                await ctx.ReplyAsync(owofied).ConfigureAwait(false);
            }
        }
    }
}
