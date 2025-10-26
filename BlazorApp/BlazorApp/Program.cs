// Asegúrate de tener esta línea al principio para usar HttpClient y Uri
using System.Net.Http;
// Estas líneas ya las tenías
using BlazorApp.Client.Pages;
using BlazorApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// --- LÍNEA AGREGADA CON TU PUERTO ---
// Configura el HttpClient para que apunte a tu WebAPI usando el puerto 7206
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5247/") });
// --- FIN LÍNEA AGREGADA ---
builder.Services.AddScoped<BlazorApp.Client.AuthStateProvider>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp.Client._Imports).Assembly);

app.Run();