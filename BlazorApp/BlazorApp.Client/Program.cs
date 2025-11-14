using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using BlazorApp.Client;
using API.Clients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.Services.AddScoped<AuthTokenHandler>();


builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://localhost:7206/");
})
.AddHttpMessageHandler<AuthTokenHandler>(); 


builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));


builder.Services.AddSingleton<AuthStateProvider>();

var host = builder.Build();

var authStateProvider = host.Services.GetRequiredService<AuthStateProvider>();
await authStateProvider.LoadUserFromStorage();

await host.RunAsync();