using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using System;
using System.IO;
using Microcharts;
using SkiaSharp;
using static Shared.HttpClientExtensions;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;

namespace Discord.Commands;

[Group("graph"), Description("Get's graphs. Default is XP")]
public class Graphs : BaseCommandModule
{
    public HttpClient _httpClient;
    private static readonly SKColor LineColour = SKColor.Parse("#3498db");
    private static readonly SKColor Grey = SKColor.Parse("#808080");
    private static readonly SKTypeface Font = SKTypeface.FromFile("font.ttf");
    private static readonly SKPaint LabelPaint = new SKPaint
    { 
        TextSize = 16,
        TextAlign = SKTextAlign.Center,
        Typeface = Font,
        Color = Grey,
        IsAntialias = true
    };

    [GroupCommand, Priority(1)]
    public async Task GraphDefault(CommandContext ctx, DiscordUser user)
    {
        await ctx.TriggerTypingAsync();

        DiscordMessageBuilder message = await GenerateCumulativeXpGraph(user.Id);

        var graphmsg = await ctx.RespondAsync(message);

        var selectsmsg = new DiscordMessageBuilder();

        selectsmsg.WithContent("Change Graph");

        var comp = new DiscordSelectComponent($"graphType {graphmsg.Id}{ctx.User.Id}", "Select Graph Type", new DiscordSelectComponentOption[] { new DiscordSelectComponentOption("XP", "xp"), new DiscordSelectComponentOption("Messages", "msg"), new DiscordSelectComponentOption("Non Cumulative XP", "ncxp"), new DiscordSelectComponentOption("Non Cumulative Messages", "ncmsg") });

        selectsmsg.AddComponents(comp);

        await graphmsg.RespondAsync(selectsmsg);
    }

    public async Task<DiscordMessageBuilder> GenerateCumulativeXpGraph(ulong id)
    {
        float[][] rawData = await GetData(id);

        var xpData = rawData.Select(x => x[1]);

        return GenerateLineGraphInMessage(xpData, rawData.Length);
    }

    public async Task<DiscordMessageBuilder> GenerateCumulativeMessageGraph(ulong id)
    {
        float[][] rawData = await GetData(id);

        var messageData = rawData.Select(x => x[0]);

        return GenerateLineGraphInMessage(messageData, rawData.Length);
    }

    private async Task<float[][]> GetData(ulong id)
        => await _httpClient.GetDataAsJson<float[][]>($"https://ubi.vtech.cf/pastdata?userid={id}");


    private DiscordMessageBuilder GenerateLineGraphInMessage(IEnumerable<float> data, int count)
    {
        ChartEntry[] entries = CreateCumulativeEntries(data, count);

        return new DiscordMessageBuilder().WithFile("graph.png", GenerateLineGraph(entries));
    }

    private static ChartEntry[] CreateCumulativeEntries(IEnumerable<float> data, int count)
    {
        using var enumerator = data.GetEnumerator();
        //enumerator.MoveNext();

        ChartEntry[] entries = new ChartEntry[30];
        int gap = 30 - count;

        for (int i = 0; i < 30; i++)
        {
            if (gap > i)
            {
                ChartEntry empty = new(0)
                {
                    Label = (i - 30 + 1).ToString(),
                    Color = LineColour,
                    ValueLabel = "0",
                    TextColor = Grey,
                    ValueLabelColor = Grey
                };

                entries[i] = empty;
                continue;
            }

            enumerator.MoveNext();

            float point = enumerator.Current;

            ChartEntry entry = new(point)
            {
                Label = (i - 30 + 1).ToString(),
                Color = LineColour,
                ValueLabel = ((int)point).ToString(),
                TextColor = Grey,
                ValueLabelColor = Grey
            };

            entries[i] = entry;
        }

        return entries;
    }

    private static ChartEntry[] CreateNonCumulativeEntries(IEnumerable<float> data, int count)
    {
        using var enumerator = data.GetEnumerator();

        enumerator.MoveNext();
        float previous = enumerator.Current;

        ChartEntry[] entries = new ChartEntry[30];
        int gap = 30 - count + 1;

        for (int i = 0; i < 30; i++)
        {
            if (gap > i)
            {
                ChartEntry empty = new(0)
                {
                    Label = (i - 30 + 1).ToString(),
                    Color = LineColour,
                    ValueLabel = "0",
                    TextColor = Grey,
                    ValueLabelColor = Grey
                };

                entries[i] = empty;
                continue;
            }

            enumerator.MoveNext();

            float point = enumerator.Current - previous;
            previous = enumerator.Current;

            ChartEntry entry = new(point)
            {
                Label = (i - 30 + 1).ToString(),
                Color = LineColour,
                ValueLabel = ((int)point).ToString(),
                TextColor = Grey,
                ValueLabelColor = Grey
            };

            entries[i] = entry;
        }

        return entries;
    }

    [GroupCommand, Priority(0)]
    public async Task GraphDefault(CommandContext ctx)
        => await GraphDefault(ctx, ctx.User);

    private Stream GenerateLineGraph(ChartEntry[] entries)
    {
        AlwaysRoundUp((int)entries[^1].Value, out int rounded, out int biggestDigitPlace, out int yAxisLabelAmount);

        LineChart chart = new()
        {
            Entries = entries,
            LabelOrientation = Orientation.Horizontal,
            PointMode = PointMode.Circle,
            LineMode = LineMode.Straight,
            MaxValue = rounded,
            MinValue = 0,
            ValueLabelOrientation = Orientation.Vertical,
            EnableYFadeOutGradient = false,
            Margin = 30,
            Typeface = Font,
            LabelTextSize = 16,
            IsAnimated = false,
            AnimationDuration = TimeSpan.Zero
        };

        SKBitmap bitmap = new(1700, 700);
        using SKCanvas canvas = new(bitmap);
        chart.Draw(canvas, 1700, 700);
        //canvas.DrawLine(new SKPoint(20, 90), new SKPoint(20, 595), new SKPaint());

        if (yAxisLabelAmount <= 3)
        {
            biggestDigitPlace /= 2;
            yAxisLabelAmount *= 2;
        }
        
        int spaceBetweenYLabels = (595 - 90) / yAxisLabelAmount;

        int backwardsI = yAxisLabelAmount;
        for (int i = 0; i < yAxisLabelAmount; i++)
        { 
            canvas.DrawText((backwardsI * biggestDigitPlace).ToString(), new SKPoint(20, i * spaceBetweenYLabels + 95), LabelPaint) ;
            backwardsI--;
        }

        return SKImage.FromBitmap(bitmap).Encode().AsStream();
    }

    private void AlwaysRoundUp(int i, out int rounded, out int biggestDigitPlace, out int roundedFirstDigit)
    {
        int digitCount = GetAllDigitCountButFirstDigit(i);
        biggestDigitPlace = BiggestDigitPlace(digitCount);
        roundedFirstDigit = GetCeilingOfFirstDigit(i, biggestDigitPlace);
        rounded = roundedFirstDigit * biggestDigitPlace;
    }

    private static int GetCeilingOfFirstDigit(float i, int unit)
        => (int)Math.Ceiling((double)i / unit);

    private static int BiggestDigitPlace(int digits)
        => (int)Math.Pow(10, digits);

    private static int GetAllDigitCountButFirstDigit(int i) 
        => (int)Math.Floor(Math.Log10(i));
}