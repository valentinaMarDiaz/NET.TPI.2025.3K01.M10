using DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace API.Clients;

public static class CategoriaApiClient
{
    private static readonly HttpClient client;
    static CategoriaApiClient()
    {
        client = new HttpClient { BaseAddress = new Uri("https://localhost:7206/") };
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public static async Task<IEnumerable<CategoriaDTO>> GetAllAsync()
        => await client.GetFromJsonAsync<IEnumerable<CategoriaDTO>>("categorias") ?? new List<CategoriaDTO>();

    public static async Task AddAsync(CategoriaDTO dto)
    {
        var res = await client.PostAsJsonAsync("categorias", dto);
        res.EnsureSuccessStatusCode();
    }

    public static async Task UpdateAsync(CategoriaDTO dto)
    {
        var res = await client.PutAsJsonAsync("categorias", dto);
        res.EnsureSuccessStatusCode();
    }

    public static async Task DeleteAsync(int id)
    {
        var res = await client.DeleteAsync($"categorias/{id}");
        res.EnsureSuccessStatusCode();
    }
}
