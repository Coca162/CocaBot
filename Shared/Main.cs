using Microsoft.EntityFrameworkCore.Internal;
using System.Text.Json;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Shared;
public class Main
{
    public static HttpClient client { get; set; } = new();
    public static Platform platform { get; set; }
    public static DefaultConfig config { get; set; }
    //public static Dictionary<ulong, string> IdSVIDs { get; set; }
    //public static Dictionary<ulong, string> IdTokens { get; set; }

    public static readonly string[] Districts = { "new yam", "voopmont", "san vooperisco", "medievala", "old yam", "new vooperis", "isle of servers past", "server past", "servers past", "los vooperis", "queensland", "netherlands", "vooperia city", "new spudland", "landing cove", "old king", "corgi" };

    public static async Task<T> GetConfig<T>() where T : DefaultConfig
    {
        FileStream fs = File.OpenRead("secret.json");
        T config = await JsonSerializer.DeserializeAsync<T>(fs, new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        });

        Main.config = config;

        return config;
    }

    public enum Platform
    {
        Valour,
        Discord
    }
}