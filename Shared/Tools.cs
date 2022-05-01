using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Shared.Main;
using static Shared.Tools;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Text.Json;
using System.Net.Http;

namespace Shared;

public static class Tools
{
    public static async Task<T> GetDataFromJson<T>(string url)
    {
        var client = new HttpClient();
        var response = await client.GetStreamAsync(url);
        try
        {
            return await JsonSerializer.DeserializeAsync<T>(response);
        }
        catch (Exception e)
        {
            throw new Exception($"HTTP Error for GetDataFromJson!\n{e}");
        }
    }

    public static async Task<string> GetData(string url)
    {
        var client = new HttpClient();
        var httpResponse = await client.GetAsync(url);
        string response = await httpResponse.Content.ReadAsStringAsync();

        return httpResponse.IsSuccessStatusCode ? response : $"HTTP Error: {httpResponse.StatusCode}; {response}";
    }
}