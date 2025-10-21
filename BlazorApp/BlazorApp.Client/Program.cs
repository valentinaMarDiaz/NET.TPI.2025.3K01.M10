using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http; // Necesario para HttpClient y Uri

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// --- INICIO L�NEA AGREGADA ---
// Registra el HttpClient para que est� disponible en los componentes WASM
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7206/") }); // <-- USA TU PUERTO HTTPS
// --- FIN L�NEA AGREGADA ---

await builder.Build().RunAsync();