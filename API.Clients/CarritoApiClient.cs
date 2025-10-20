using System.Net.Http.Headers;
using System.Net.Http.Json;
using DTOs;

namespace API.Clients;

public static class CarritoApiClient
{
    static readonly HttpClient client;
    static CarritoApiClient()
    {
        client = new HttpClient { BaseAddress = new Uri("http://localhost:5247/") }; // ajustar puerto
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public static async Task<CarritoDTO> GetAsync(int idCliente)
        => await client.GetFromJsonAsync<CarritoDTO>($"carrito/{idCliente}") ?? new CarritoDTO { IdCliente = idCliente };

    public static async Task<CarritoDTO> AddAsync(int idCliente, int idProducto, int cantidad)
    {
        var r = await client.PostAsJsonAsync("carrito/agregar", new AgregarCarritoDTO { IdCliente = idCliente, IdProducto = idProducto, Cantidad = cantidad });
        if (!r.IsSuccessStatusCode) throw new Exception(await r.Content.ReadAsStringAsync());
        return (await r.Content.ReadFromJsonAsync<CarritoDTO>())!;
    }

    public static async Task<CarritoDTO> RemoveAsync(int idCliente, int idProducto)
    {
        var r = await client.DeleteAsync($"carrito/item?idCliente={idCliente}&idProducto={idProducto}");
        if (!r.IsSuccessStatusCode) throw new Exception(await r.Content.ReadAsStringAsync());
        return (await r.Content.ReadFromJsonAsync<CarritoDTO>())!;
    }

    public static async Task<VentaDTO> ConfirmarAsync(int idCliente)
    {
        var r = await client.PostAsJsonAsync("carrito/confirmar", new ConfirmarCarritoDTO { IdCliente = idCliente });
        if (!r.IsSuccessStatusCode) throw new Exception(await r.Content.ReadAsStringAsync());
        return (await r.Content.ReadFromJsonAsync<VentaDTO>())!;
    }
}
