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

        //SetHttpClient(new HttpClient()
        //{
        //    BaseAddress = new Uri("https://valour.gg/")
        //});

        //TokenRequest content = new(config.Email, config.Password);

        //var response = await Http.PostAsJsonAsync($"api/user/requesttoken", content);

        //var message = await response.Content.ReadAsStringAsync();

        //if (!response.IsSuccessStatusCode)
        //{
        //    Console.WriteLine("Failed to request user token.");
        //    Console.WriteLine(message);
        //    return;
        //}

        //await InitializeSignalR("https://valour.gg" + "/planethub");

        //await InitializeUser(message);

        OnMessageRecieved += async (message) =>
            await MessageHandler(message);

        RegisterCommands();

        await Task.Delay(-1);
    }

    private static async Task MessageHandler(PlanetMessage ctx)
    {
        var sender = await (await ctx.GetAuthorAsync()).GetUserAsync();
        //if (!sender.Bot || Self != sender)
        //{
        //    CocaBotContext db = new();

        //    string id = await ValourToSVID(sender.Id, db);

        //    await GetData($"https://ubi.vtech.cf/new_valour_message?svid={id}&valour_id={sender.Id}&key={UBIKey}");
        //}

        var matches = prefixes.Where(prefix => ctx.Content.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));

        if (!matches.Any()) return;

        int prefixLength = matches.Max().Length;

        List<string> stringArgs = ctx.Content[prefixLength..].Split(' ').ToList();

        string commandname = stringArgs[0].ToLowerInvariant();
        stringArgs.RemoveAt(0);

        var command = commands.SingleOrDefault(cmd => cmd.Key.Contains(commandname)).Value;

        if (command is null || 
           (sender.Bot && command.GetCustomAttribute(typeof(AllowBots)) is not null) ||
           (Self == sender && command.GetCustomAttribute(typeof(AllowSelf)) is not null)) return;

        var parms = command.GetParameters();

        List<object> args = new(parms.Length);

        int length = prefixLength + commandname.Length + 1;
        List<Mention> mentions = ctx.Mentions;
        int contexts = 0;
        for (int i = 0; i < parms.Length; i++)
        {
            ParameterInfo parameter = parms[i];

            if (parameter.ParameterType == typeof(PlanetMessage))
            {
                contexts++;
                args.Add(ctx);
                continue;
            }

            var position = i - contexts;
            string rawargs = stringArgs[position];

            if (parameter.ParameterType == typeof(string) && parameter.GetCustomAttribute(typeof(Remainder), false) is not null)
            {
                args.Add(string.Join(' ', stringArgs.GetRange(position, stringArgs.Count - position)));
                continue;
            }

            if (parameter.ParameterType == typeof(Api.Items.Users.User) || parameter.ParameterType == typeof(PlanetMember) || parameter.ParameterType == typeof(PlanetCategory) || parameter.ParameterType == typeof(PlanetChatChannel) || parameter.ParameterType == typeof(PlanetRole))
            {
                var mention = ctx.Mentions.FirstOrDefault();

                if (mention is null || mention.Position != length + 3) continue;
                
                mentions.RemoveAt(0);

                switch (mention.Type)
                {
                    //User
                    case MentionType.Member when parameter.ParameterType == typeof(Api.Items.Users.User):
                        args.Add(await (await PlanetMember.FindAsync(mention.Target_Id)).GetUserAsync());
                        break;
                    //Member
                    case MentionType.Member when parameter.ParameterType == typeof(PlanetMember):
                        args.Add(await PlanetMember.FindAsync(mention.Target_Id));
                        break;
                    case MentionType.Category when parameter.ParameterType == typeof(PlanetCategory):
                        args.Add(await PlanetCategory.FindAsync(mention.Target_Id));
                        break;
                    case MentionType.Channel when parameter.ParameterType == typeof(PlanetChatChannel):
                        args.Add(await PlanetChatChannel.FindAsync(mention.Target_Id));
                        break;
                    case MentionType.Role when parameter.ParameterType == typeof(PlanetRole):
                        args.Add(await PlanetRole.FindAsync(mention.Target_Id));
                        break;
                }
                continue;
            }

            TypeConverter typeConverter = TypeDescriptor.GetConverter(parameter.ParameterType);
            args.Add(typeConverter.ConvertFrom(rawargs));

            length =+ rawargs.Length + 1;
        }

        command.Invoke(null, args.ToArray());


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