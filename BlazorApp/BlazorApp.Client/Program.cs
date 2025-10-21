using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http; // Necesario para HttpClient y Uri

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// --- INICIO LÍNEA AGREGADA ---
// Registra el HttpClient para que esté disponible en los componentes WASM
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7206/") }); // <-- USA TU PUERTO HTTPS
// --- FIN LÍNEA AGREGADA ---

await builder.Build().RunAsync();