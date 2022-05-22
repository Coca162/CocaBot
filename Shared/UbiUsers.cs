using Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared;

public interface IUbiUserAPI
{
    Task<IAsyncEnumerable<UbiUser>> GetTop10();
    Task<IReadOnlyCollection<UbiUser>> GetAllAsync();
    Task<IAsyncEnumerable<UbiUser>> GetAllAsyncEnumerable();
    Task<UbiUser> GetUserAsync(ulong id);
}

public class UbiUserAPI : IUbiUserAPI
{
    private readonly HttpClient _httpClient;

    public UbiUserAPI(HttpClient client)
    {
        _httpClient = client;
    }

    public async Task<IAsyncEnumerable<UbiUser>> GetTop10() =>
        JsonSerializer.DeserializeAsyncEnumerable<UbiUser>(await _httpClient.GetStreamAsync("https://ubi.vtech.cf/api/leaderboard"));

    public async Task<IAsyncEnumerable<UbiUser>> GetAllAsyncEnumerable() =>
        JsonSerializer.DeserializeAsyncEnumerable<UbiUser>(await GetAllUserStream());

    public async Task<IReadOnlyCollection<UbiUser>> GetAllAsync() =>
        (await JsonSerializer.DeserializeAsync<List<UbiUser>>(await GetAllUserStream())).AsReadOnly();

    private async Task<Stream> GetAllUserStream() =>
        await _httpClient.GetStreamAsync($"https://ubi.vtech.cf/all_user_data");

    public async Task<UbiUser> GetUserAsync(ulong id) =>
        await _httpClient.GetDataAsJson<UbiUser>($"https://ubi.vtech.cf/get_xp_info?id={id}");
}
