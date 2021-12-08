using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Valour.Shared.Users;
using Valour.Api.Messages;
using Valour.Api.Planets;
using static Shared.Main;
using static Valour.Api.Client.ValourClient;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace Valour;
class Program
{
    public static bool prod;
    public static List<string> prefix;

    static async Task Main()
    {
        ValourConfig config = await GetConfig<ValourConfig>();
        prod = config.Production;
        prefix = config.Prefix;
        //if (prod) LoadSVIDNameCache();

        HttpClient http = new()
        {
            BaseAddress = new Uri("https://valour.gg/")
        };
        SetHttpClient(http);

        TokenRequest content = new()
        {
            Email = config.Email,
            Password = config.BotPassword
        };

        var response = await Http.PostAsJsonAsync($"api/user/requesttoken", content);

        var message = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Failed to request user token.");
            Console.WriteLine(message);
            return;
        }
        else
        {
            var result = await InitializeUser(message);
            Console.WriteLine(result.Message);
        }

        await InitializeSignalR("https://valour.gg" + "/planethub");

        OnMessageRecieved += MessageHandler;

        await Task.Delay(-1);
    }

    private static async Task MessageHandler(PlanetMessage arg)
    {
        if (string.IsNullOrWhiteSpace(arg.Content)) return;

        var message = arg.Content.Trim();
        string commandprefix = "";
        commandprefix = prefix.SingleOrDefault(prefix => message[..prefix.Length] == prefix);

        if (commandprefix is null) return;

        //await ReplyMessage(arg, "");
    }

    public static async Task ReplyMessage(PlanetMessage respondent, string response)
    {
        PlanetMessage message = new()
        {
            Channel_Id = respondent.Channel_Id,
            Content = response,
            TimeSent = DateTime.UtcNow,
            Author_Id = Self.Id,
            Planet_Id = respondent.Planet_Id,
            Member_Id = (await Member.FindAsync(respondent.Planet_Id, Self.Id)).Id,
            Fingerprint = Guid.NewGuid().ToString()
        };

        StringContent content = new(JsonSerializer.Serialize(message));

        HttpResponseMessage httpresponse = await Http.PostAsync($"api/channel/{respondent.Channel_Id}/messages", content);

        string res = await httpresponse.Content.ReadAsStringAsync();

        Console.WriteLine("Message post: " + res);
    }
}