using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using BlazorApp.Client; // <-- Aseg�rate que este sea tu namespace correcto para AuthStateProvider

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Registra el HttpClient (ya lo ten�as)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7206/") }); // <-- Usa tu puerto HTTPS

// --- L�NEA ESENCIAL ---
// Registra nuestro AuthStateProvider como un servicio 'Singleton' EN EL CLIENTE
builder.Services.AddSingleton<AuthStateProvider>();
// --- FIN L�NEA ESENCIAL ---

var host = builder.Build();

// Obtiene el servicio reci�n registrado
var authStateProvider = host.Services.GetRequiredService<AuthStateProvider>();
// Intenta cargar el usuario desde LocalStorage al iniciar
await authStateProvider.LoadUserFromStorage();

await host.RunAsync(); // Corre la aplicaci�n