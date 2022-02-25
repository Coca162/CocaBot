using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static Shared.Main;
using Shared;
using Shared.Models;
using static Valour.Api.Client.ValourClient;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using Valour.Api.Items.Users;
using Valour.Api.Items.Messages;
using Valour.Api.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Valour.Api.Items.Planets.Channels;
using Valour.Api.Items.Planets;
using static Valour.ValourTools;
using static Valour.Commands;
using static SpookVooper.Api.SpookVooperAPI;
using Valour.Api.Items.Users;
using System.ComponentModel;
using System.Globalization;
using Valour.Api.Items.Planets.Members;
using Valour.Shared.Items.Messages.Mentions;
using System.Reflection;
using ValourSharp;

namespace Valour;
class Program
{
    public static bool prod;
    public static List<string> prefixes;
    public static string UBIKey;

    static async Task Main()
    {
        ValourConfig config = await GetConfig<ValourConfig>();
        prod = config.Production;
        prefixes = config.Prefix;
        UBIKey = config.JacobUBIKey;
        //if (prod) LoadSVIDNameCache();

        Registration.RegisterCommands();

        await Start.Initialize(config.Email, config.BotPassword, prefixes.ToArray());

        OnMessageRecieved += async (message) =>
            await MessageHandler(message);

        await Task.Delay(-1);
    }

    private static async Task MessageHandler(PlanetMessage ctx)
    {
        var sender = await (await ctx.GetAuthorAsync()).GetUserAsync();
        if (sender.Bot && Self == sender) return;

        CocaBotContext db = new();
        string id = await ValourToSVID(sender.Id, db);
        if (id is null) return;

        await GetData($"https://ubi.vtech.cf/new_valour_message?svid={id}&valour_id={sender.Id}&key={UBIKey}");
    }
}