using DTOs;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorApp.Client
{
  
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;

        public AuthTokenHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
               
                var userJson = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authUser");
                if (!string.IsNullOrEmpty(userJson))
                {
             
                    var user = JsonSerializer.Deserialize<LoginResponseDTO>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (user != null && !string.IsNullOrEmpty(user.Token))
                    {
                    
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar token: {ex.Message}");
               
            }

     
            return await base.SendAsync(request, cancellationToken);
        }
    }
}