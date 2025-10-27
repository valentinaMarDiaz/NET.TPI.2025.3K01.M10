using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using BlazorApp.Client; // <-- Asegúrate que este sea tu namespace correcto para AuthStateProvider

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7206/") });


builder.Services.AddSingleton<AuthStateProvider>();


var host = builder.Build();


var authStateProvider = host.Services.GetRequiredService<AuthStateProvider>();

await authStateProvider.LoadUserFromStorage();

await host.RunAsync(); 