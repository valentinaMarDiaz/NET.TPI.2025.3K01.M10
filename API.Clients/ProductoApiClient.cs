using System.Net.Http.Headers;
using System.Net.Http.Json;
using DTOs;

namespace API.Clients;

public static class ProductoApiClient
{
    private static readonly HttpClient client;

    static ProductoApiClient()
    {
        client = new HttpClient { BaseAddress = new Uri("http://localhost:5247/") }; // tu puerto
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public static async Task<IEnumerable<ProductoDTO>> GetAllAsync()
    {
        var r = await client.GetAsync("productos");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<IEnumerable<ProductoDTO>>() ?? Enumerable.Empty<ProductoDTO>();
    }

    public static async Task<ProductoDTO?> GetAsync(int id)
    {
        var r = await client.GetAsync($"productos/{id}");
        if (!r.IsSuccessStatusCode) return null;
        return await r.Content.ReadFromJsonAsync<ProductoDTO>();
    }

    public static async Task<ProductoDTO> AddAsync(ProductoDTO dto)
    {
        var r = await client.PostAsJsonAsync("productos", dto);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<ProductoDTO>() ?? new ProductoDTO();
    }

    public static async Task UpdateAsync(ProductoDTO dto)
    {
        var r = await client.PutAsJsonAsync("productos", dto);
        r.EnsureSuccessStatusCode();
    }

    public static async Task DeleteAsync(int id)
    {
        var r = await client.DeleteAsync($"productos/{id}");
        r.EnsureSuccessStatusCode();
    }

    public static async Task<IEnumerable<PrecioProductoDTO>> GetHistorialAsync(int idProducto)
    {
        var r = await client.GetAsync($"productos/{idProducto}/historial");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<IEnumerable<PrecioProductoDTO>>() ?? Enumerable.Empty<PrecioProductoDTO>();
    }
}
