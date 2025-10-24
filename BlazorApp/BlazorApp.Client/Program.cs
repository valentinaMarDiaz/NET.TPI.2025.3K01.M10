using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using BlazorApp.Client; // <-- Asegúrate que este sea tu namespace correcto para AuthStateProvider

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Registra el HttpClient (ya lo tenías)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7206/") }); // <-- Usa tu puerto HTTPS

// --- LÍNEA ESENCIAL ---
// Registra nuestro AuthStateProvider como un servicio 'Singleton' EN EL CLIENTE
builder.Services.AddSingleton<AuthStateProvider>();
// --- FIN LÍNEA ESENCIAL ---

var host = builder.Build();

// Obtiene el servicio recién registrado
var authStateProvider = host.Services.GetRequiredService<AuthStateProvider>();
// Intenta cargar el usuario desde LocalStorage al iniciar
await authStateProvider.LoadUserFromStorage();

await host.RunAsync(); // Corre la aplicación