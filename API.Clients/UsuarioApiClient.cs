using System.Net.Http.Headers;
using System.Net.Http.Json;
using DTOs;

namespace API.Clients;

public static class UsuarioApiClient
{
    private static readonly HttpClient client;

    static UsuarioApiClient()
    {
        client = new HttpClient { BaseAddress = new Uri("https://localhost:7206/") }; // tu puerto http
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public static async Task<IEnumerable<UsuarioDTO>> GetAllAsync()
    {
        var r = await client.GetAsync("usuarios");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<IEnumerable<UsuarioDTO>>() ?? Enumerable.Empty<UsuarioDTO>();
    }

    public static async Task<UsuarioDTO?> GetAsync(int id)
    {
        var r = await client.GetAsync($"usuarios/{id}");
        if (!r.IsSuccessStatusCode) return null;
        return await r.Content.ReadFromJsonAsync<UsuarioDTO>();
    }

    public static async Task UpdateAsync(UsuarioDTO dto)
    {
        var r = await client.PutAsJsonAsync("usuarios", dto);
        r.EnsureSuccessStatusCode();
    }

    public static async Task DeleteAsync(int id)
    {
        var r = await client.DeleteAsync($"usuarios/{id}");
        r.EnsureSuccessStatusCode();
    }
}
