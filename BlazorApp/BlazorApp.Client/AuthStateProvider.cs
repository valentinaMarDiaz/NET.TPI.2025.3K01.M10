using DTOs;
using System.Text.Json; 
using Microsoft.JSInterop; 

namespace BlazorApp.Client 
{
    public class AuthStateProvider
    {
       
        public LoginResponseDTO? CurrentUser { get; private set; }

        
        public event Action? OnChange;

        
        private readonly IJSRuntime _jsRuntime;
        private const string UserStorageKey = "authUser"; 

        public AuthStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        
        public async Task Login(LoginResponseDTO user)
        {
            CurrentUser = user;
            
            try
            {
                var userJson = JsonSerializer.Serialize(user);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserStorageKey, userJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando usuario en LocalStorage: {ex.Message}");
            }
            
            NotifyStateChanged(); 
        }

        
        public async Task Logout()
        {
            CurrentUser = null;
            
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserStorageKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error borrando usuario de LocalStorage: {ex.Message}");
            }
            
            NotifyStateChanged(); 
        }

       
        public async Task LoadUserFromStorage()
        {
            try
            {
                var userJson = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserStorageKey);
                if (!string.IsNullOrEmpty(userJson))
                {
                    CurrentUser = JsonSerializer.Deserialize<LoginResponseDTO>(userJson);
                    NotifyStateChanged(); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando usuario desde LocalStorage: {ex.Message}");
                CurrentUser = null; 
            }
        }
         
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}