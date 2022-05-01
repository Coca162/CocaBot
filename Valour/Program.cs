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
using static Valour.Commands.Misc;
using System.ComponentModel;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ValourSharp;
using ValourSharp.Classes;
using Valour.Api.Items.Planets.Members;
using static Shared.Tools;

namespace Valour;

class Program
{
    public static bool prod { get; private set; }
    public static string UBIKey { get; private set; }
    public static IServiceProvider Service { get; private set; }

    static async Task Main()
    {
        ValourConfig config = await GetConfig<ValourConfig>();
        prod = config.Production;
        IEnumerable<string> prefixes = config.Prefix;
        UBIKey = config.JacobUBIKey;

        ValourSharpConfig valourConfig = new()
        {
            Email = config.Email,
            Password = config.BotPassword,
            Prefixes = prefixes,
            Services = Service
        };

        Registration.RegisterCommands(Assembly.GetExecutingAssembly());

        OnMessageRecieved += MessageHandler;

        await Initialize.Start(valourConfig);

        await Task.Delay(-1);
    }

    private static async Task MessageHandler(PlanetMessage ctx)
    {
        var sender = await ctx.GetAuthorUserAsync();
        if (sender.Bot && Self == sender) return;

        await using CocaBotContext db = new();
        string id = await ValourToSVID(sender.Id, db);
        if (id is null) return;

        await GetData($"https://ubi.vtech.cf/new_valour_message?svid={id}&valour_id={sender.Id}&key={UBIKey}");
    }
}