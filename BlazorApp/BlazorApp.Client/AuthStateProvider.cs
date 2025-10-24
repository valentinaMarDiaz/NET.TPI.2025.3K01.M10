using DTOs; // Para usar LoginResponseDTO
using System.Text.Json; // Para serializar/deserializar
using Microsoft.JSInterop; // Para interactuar con LocalStorage (opcional)

namespace BlazorApp.Client // Asegúrate que este sea tu namespace
{
    public class AuthStateProvider
    {
        // Guardaremos la información del usuario aquí. Null si nadie está logueado.
        public LoginResponseDTO? CurrentUser { get; private set; }

        // Evento para notificar a los componentes cuando el estado de autenticación cambie
        public event Action? OnChange;

        // --- OPCIONAL: Para persistir el login entre recargas usando LocalStorage ---
        private readonly IJSRuntime _jsRuntime;
        private const string UserStorageKey = "authUser"; // Clave para guardar en LocalStorage

        public AuthStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        // --- FIN OPCIONAL ---

        // Método para ser llamado después de un login exitoso
        public async Task Login(LoginResponseDTO user)
        {
            CurrentUser = user;
            // --- OPCIONAL: Guardar en LocalStorage ---
            try
            {
                var userJson = JsonSerializer.Serialize(user);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserStorageKey, userJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando usuario en LocalStorage: {ex.Message}");
            }
            // --- FIN OPCIONAL ---
            NotifyStateChanged(); // Notifica a los componentes que el usuario cambió
        }

        // Método para cerrar sesión
        public async Task Logout()
        {
            CurrentUser = null;
            // --- OPCIONAL: Borrar de LocalStorage ---
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserStorageKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error borrando usuario de LocalStorage: {ex.Message}");
            }
            // --- FIN OPCIONAL ---
            NotifyStateChanged(); // Notifica que el usuario ya no está
        }

        // --- OPCIONAL: Método para cargar el usuario desde LocalStorage al iniciar la app ---
        public async Task LoadUserFromStorage()
        {
            try
            {
                var userJson = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserStorageKey);
                if (!string.IsNullOrEmpty(userJson))
                {
                    CurrentUser = JsonSerializer.Deserialize<LoginResponseDTO>(userJson);
                    NotifyStateChanged(); // Notifica que se cargó un usuario
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando usuario desde LocalStorage: {ex.Message}");
                CurrentUser = null; // Asegura que no quede un estado inválido
            }
        }
        // --- FIN OPCIONAL ---


        // Método privado para disparar el evento OnChange
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}