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

        await InitializeBot(config.Email, config.BotPassword);

        OnMessageRecieved += async (message) =>
            await MessageHandler(message);

        Registration.RegisterCommands();

        await Task.Delay(-1);
    }

    private static async Task MessageHandler(PlanetMessage ctx)
    {
        var sender = await (await ctx.GetAuthorAsync()).GetUserAsync();
        if (!sender.Bot || Self != sender)
        {
            CocaBotContext db = new();

            string id = await ValourToSVID(sender.Id, db);

            await GetData($"https://ubi.vtech.cf/new_valour_message?svid={id}&valour_id={sender.Id}&key={UBIKey}");
        }

        //if ((ctx.Content.StartsWith('/') || ctx.Content.StartsWith("c/", StringComparison.InvariantCultureIgnoreCase) || ctx.Content.StartsWith(';'))
        //    && !ctx.Content.Contains("conf", StringComparison.InvariantCultureIgnoreCase))
        //{

        //    var user = db.Users.Where(x => x.ValourName == sender.Name).FirstOrDefault();
        //    if (user is null) return;
        //    user.Valour = sender.Id;
        //    user.ValourName = null;
        //    await db.SaveChangesAsync();
        //    await ctx.ReplyAsync("Linked Account!");
        //}

        //if (string.IsNullOrWhiteSpace(arg.Content)) return;

        //var message = arg.Content.Trim();
        //string commandprefix = "";
        //commandprefix = prefix.SingleOrDefault(prefix => message[..prefix.Length] == prefix);

        //if (commandprefix is null) return;

        ////await ReplyMessage(arg, "");
    }
}