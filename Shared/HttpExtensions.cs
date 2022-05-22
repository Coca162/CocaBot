using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Shared.Main;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Text.Json;
using System.Net.Http;

namespace Shared;

public static class HttpClientExtensions
{
    public static async Task<T> GetDataAsJson<T>(this HttpClient httpClient, string url)
    {
        var response = await httpClient.GetStreamAsync(url);
        return await JsonSerializer.DeserializeAsync<T>(response);
    }

    public static async Task<IAsyncEnumerable<T>> GetIAsyncEnumerableFromJson<T>(this HttpClient httpClient, string url)
    {
        var response = await httpClient.GetStreamAsync(url);
        return JsonSerializer.DeserializeAsyncEnumerable<T>(response);
    }
}