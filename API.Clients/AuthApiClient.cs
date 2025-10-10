using System.Net.Http.Headers;
using System.Net.Http.Json;
using DTOs;

namespace API.Clients;

public static class AuthApiClient
{
    private static readonly HttpClient client;

    static AuthApiClient()
    {
        client = new HttpClient { BaseAddress = new Uri("http://localhost:5247/") }; // <-- tu puerto http
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public static async Task<LoginResponseDTO> RegisterClienteAsync(RegisterClienteDTO dto)
    {
        var res = await client.PostAsJsonAsync("auth/register/cliente", dto);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<LoginResponseDTO>() ?? new LoginResponseDTO();
    }

    public static async Task<LoginResponseDTO> RegisterVendedorAsync(RegisterVendedorDTO dto)
    {
        var res = await client.PostAsJsonAsync("auth/register/vendedor", dto);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<LoginResponseDTO>() ?? new LoginResponseDTO();
    }

    public static async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO dto)
    {
        var res = await client.PostAsJsonAsync("auth/login", dto);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<LoginResponseDTO>();
    }
}
