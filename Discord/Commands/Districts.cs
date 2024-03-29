using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using Shared;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Discord.Commands;

[Group("district")] // let's mark this class as a command group
[Description("District related commands")] // give it a description for help purposes
public class Districts : BaseCommandModule
{
    private static DiscordGuild sv = null;

    private static List<DiscordRole> districts = null;

    [Command("populations"), GeneralBlacklist()]
    [Description("Gets the member count of district's role")]
    [Aliases("population", "pops", "pop")]
    public async Task Populations(CommandContext ctx)
    {
        if (districts is null)
        {
            sv = await ctx.Client.GetGuildAsync(798307000206360588);

            districts = new()
            {
                sv.GetRole(840302532165500959), //Voopmont
                sv.GetRole(840303040572424252), //Katonia
                sv.GetRole(840303004347531285), //New Avalon
                sv.GetRole(840303014362742784), //Landing Cove
                sv.GetRole(840303153564090388), //Los Vooperis
                sv.GetRole(840303011271671828), //Lanatia
                sv.GetRole(840303023925755944), //Kōgi
                sv.GetRole(840303033635962921), //Ardenti Terra
                sv.GetRole(840303017391423508), //New Spudland
                sv.GetRole(840303030443966524), //New Vooperis
                sv.GetRole(840303007886475265), //San Vooperisco
                sv.GetRole(840303044037836870), //Old King Peninsula
                sv.GetRole(840303026979864617), //Avalon
                sv.GetRole(840303020855918602), //Novastella
                sv.GetRole(840303037162455090), //Thesonica
            };
        }

        var members = await sv.GetAllMembersAsync();

        IOrderedEnumerable<(string Name, int Count)> fields = 
            districts.Select(role => (role.Name, members.Where(X => X.Roles.Contains(role)).Count()))
                     .OrderByDescending(x => x.Item2);

        DiscordEmbedBuilder embed = new()
        {
            Title = $"Discord District Populations ({(int)fields.Average(x => x.Count)} Average)",
            Color = new DiscordColor("2CC26C")
        };

        var array = fields.Select(x => (double)x.Count).ToArray();
        var median = fields.ElementAt(7).Count;
        var lower = Percentile(array, 75);
        var upper = Percentile(array, 25);
        embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
        {
            Text = $"Lower Quartile: {lower}\nUpper Quartile: {upper}"
        };

        int i = 0;
        int realI = 0;
        int count = 0;
        foreach (var (Name, Count) in fields) 
        {
            realI++;
            if (count != Count) i = realI;
            count = Count;

            embed.AddField($"{i}. {Name}" + (median == Count ? " (Median)" : ""), Count.ToString());
        }

        await ctx.RespondAsync(embed);
    }

    /// <summary>
    /// Calculate percentile of a sorted data set
    /// </summary>
    /// <param name="sortedData"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    internal static double Percentile(double[] sortedData, double p)
    {
        // algo derived from Aczel pg 15 bottom
        if (p >= 100.0d) return sortedData[sortedData.Length - 1];

        double position = (sortedData.Length + 1) * p / 100.0;
        double leftNumber = 0.0d, rightNumber = 0.0d;

        double n = p / 100.0d * (sortedData.Length - 1) + 1.0d;

        if (position >= 1)
        {
            leftNumber = sortedData[(int)Math.Floor(n) - 1];
            rightNumber = sortedData[(int)Math.Floor(n)];
        }
        else
        {
            leftNumber = sortedData[0]; // first data
            rightNumber = sortedData[1]; // first data
        }

        //if (leftNumber == rightNumber)
        if (Equals(leftNumber, rightNumber))
            return leftNumber;
        double part = n - Math.Floor(n);
        return leftNumber + part * (rightNumber - leftNumber);
    } // end of internal function percentile
}
