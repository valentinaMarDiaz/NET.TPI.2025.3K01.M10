// API.Clients/CarritoApiClient.cs
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DTOs;

namespace API.Clients
{
    public static class CarritoApiClient
    {
        static readonly HttpClient client = new();

        static CarritoApiClient()
        {
            client.BaseAddress = new Uri("http://localhost:5247/"); // puerto de tu WebAPI
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<CarritoDTO> GetAsync(int idCliente)
        {
            var r = await client.GetAsync($"carrito/{idCliente}");
            r.EnsureSuccessStatusCode();
            return (await r.Content.ReadFromJsonAsync<CarritoDTO>())!;
        }

        public static async Task RemoveAsync(int idCliente, int idProducto)
        {
            var r = await client.DeleteAsync($"carrito/{idCliente}/producto/{idProducto}");
            r.EnsureSuccessStatusCode();
        }

        public static async Task AplicarCodigoAsync(int idCliente, string codigo)
        {
            var url = $"carrito/{idCliente}/aplicar-codigo?codigo={Uri.EscapeDataString(codigo)}";
            var r = await client.PostAsync(url, content: null);
            r.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Confirma el carrito. Tolera tanto JSON camelCase (idVenta/total) como PascalCase (IdVenta/Total).
        /// </summary>
        public static async Task<(int IdVenta, decimal Total)> ConfirmarAsync(int idCliente)
        {
            var r = await client.PostAsync($"carrito/{idCliente}/confirmar", null);

            // Si falla, leo el cuerpo para darte un error más claro.
            if (!r.IsSuccessStatusCode)
            {
                var cuerpo = await r.Content.ReadAsStringAsync();
                throw new HttpRequestException($"HTTP {(int)r.StatusCode} {r.ReasonPhrase}: {cuerpo}");
            }

            // 1) Intento tipado primero (sirve si la API devuelve PascalCase)
            var tipado = await r.Content.ReadFromJsonAsync<ConfirmarResponse>();
            if (tipado != null && (tipado.Total != 0m || tipado.IdVenta != 0))
                return (tipado.IdVenta, tipado.Total);

            // 2) Fallback: diccionario tolerante a mayúsc/minúsc
            var anon = await r.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            if (anon is null) throw new InvalidOperationException("Respuesta vacía del servidor.");

            string idVentaKey =
                anon.ContainsKey("IdVenta") ? "IdVenta" :
                anon.ContainsKey("idVenta") ? "idVenta" :
                throw new KeyNotFoundException("La respuesta no contiene 'IdVenta'/'idVenta'.");

            string totalKey =
                anon.ContainsKey("Total") ? "Total" :
                anon.ContainsKey("total") ? "total" :
                throw new KeyNotFoundException("La respuesta no contiene 'Total'/'total'.");

            int idVenta = Convert.ToInt32(anon[idVentaKey]);
            decimal total = Convert.ToDecimal(anon[totalKey]);

            return (idVenta, total);
        }

        public static async Task AddAsync(int idCliente, int idProducto, int cantidad)
        {
            // POST /carrito/{idCliente}/item?producto=..&cantidad=..
            var url = $"carrito/{idCliente}/item?producto={idProducto}&cantidad={cantidad}";
            var r = await client.PostAsync(url, content: null);
            r.EnsureSuccessStatusCode();
        }
    }

    // DTO opcional para deserialización tipada (lo uso primero en ConfirmarAsync)
    public sealed class ConfirmarResponse
    {
        public int IdVenta { get; set; }
        public decimal Total { get; set; }
    }
}
