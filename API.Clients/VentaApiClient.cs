using System.Net.Http.Headers;
using System.Net.Http.Json;
using DTOs;

namespace API.Clients;

public static class VentaApiClient
{
    static readonly HttpClient client;
    static VentaApiClient()
    {
        client = new HttpClient { BaseAddress = new Uri("https://localhost:7206/") }; 
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public static async Task<IEnumerable<VentaDTO>> ListAsync(int? idCliente = null, DateTime? desdeUtc = null, DateTime? hastaUtc = null)
    {
        var qs = new List<string>();
        if (idCliente.HasValue) qs.Add($"idCliente={idCliente.Value}");
        if (desdeUtc.HasValue) qs.Add($"desdeUtc={desdeUtc.Value:o}");
        if (hastaUtc.HasValue) qs.Add($"hastaUtc={hastaUtc.Value:o}");
        var url = "ventas" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");
        return await client.GetFromJsonAsync<IEnumerable<VentaDTO>>(url) ?? Enumerable.Empty<VentaDTO>();
    }

    public static async Task<VentaDTO?> GetAsync(int id)
        => await client.GetFromJsonAsync<VentaDTO>($"ventas/{id}");

    public static async Task DeleteAsync(int id)
    {
        var r = await client.DeleteAsync($"ventas/{id}");
        r.EnsureSuccessStatusCode();
    }
}
