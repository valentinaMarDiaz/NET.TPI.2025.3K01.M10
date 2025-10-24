using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DTOs;

namespace API.Clients
{
    public static class AuthApiClient
    {
        static readonly HttpClient client = new();

        static AuthApiClient()
        {
            client.BaseAddress = new Uri("https://localhost:7206/"); // usa el puerto de tu WebAPI
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO dto)
        {
            var resp = await client.PostAsJsonAsync("auth/login", dto);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<LoginResponseDTO>();
        }

        public static async Task RegisterClienteAsync(RegisterClienteDTO dto)
        {
            var resp = await client.PostAsJsonAsync("auth/register/cliente", dto);
            resp.EnsureSuccessStatusCode();
        }

        public static async Task RegisterVendedorAsync(RegisterVendedorDTO dto)
        {
            var resp = await client.PostAsJsonAsync("auth/register/vendedor", dto);
            resp.EnsureSuccessStatusCode();
        }
    }
}
