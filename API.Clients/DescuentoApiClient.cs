using DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace API.Clients
{
    public static class DescuentoApiClient
    {
        private static readonly HttpClient client;

        static DescuentoApiClient()
        {
            client = new HttpClient
            {
                // ⚠ Ajustá si tu WebAPI corre en otro puerto o con HTTPS
                BaseAddress = new Uri("http://localhost:5247/")
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // =========================
        // ADMIN
        // =========================
        public static async Task<IReadOnlyList<DescuentoDTO>> GetAllAsync(string? producto = null)
        {
            var url = "descuentos";
            if (!string.IsNullOrWhiteSpace(producto))
                url += $"?producto={Uri.EscapeDataString(producto.Trim())}";

            var r = await client.GetAsync(url);
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadFromJsonAsync<List<DescuentoDTO>>() ?? new();
        }

        public static async Task<DescuentoDTO?> GetAsync(int id)
        {
            var r = await client.GetAsync($"descuentos/{id}");
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<DescuentoDTO>();
        }

        public static async Task<DescuentoDTO> AddAsync(DescuentoCUDTO dto)
        {
            var r = await client.PostAsJsonAsync("descuentos", dto);
            r.EnsureSuccessStatusCode();
            return (await r.Content.ReadFromJsonAsync<DescuentoDTO>())!;
        }

        public static async Task UpdateAsync(DescuentoCUDTO dto)
        {
            var r = await client.PutAsJsonAsync("descuentos", dto);
            r.EnsureSuccessStatusCode();
        }

        public static async Task DeleteAsync(int id)
        {
            var r = await client.DeleteAsync($"descuentos/{id}");
            r.EnsureSuccessStatusCode();
        }

        // =========================
        // CLIENTE
        // =========================
        public static async Task<IReadOnlyList<DescuentoDTO>> GetVigentesAsync(string? texto = null)
        {
            var url = "descuentos/vigentes";
            if (!string.IsNullOrWhiteSpace(texto))
                url += $"?texto={Uri.EscapeDataString(texto.Trim())}";

            var resp = await client.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<List<DescuentoDTO>>() ?? new();
        }

        public static async Task<DescuentoDTO?> ValidarCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;

            var resp = await client.GetAsync($"descuentos/validar?codigo={Uri.EscapeDataString(codigo.Trim())}");
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<DescuentoDTO>();
        }
    }
}
