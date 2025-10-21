using Application.Services; // Necesario para los Services
using DTOs;                // Necesario para los DTOs
using Data;                // Necesario para TPIContext
using Microsoft.EntityFrameworkCore; // Necesario para Migrate()/EnsureCreated()
using System;               // Necesario para Exception y Console

var builder = WebApplication.CreateBuilder(args);

// --- INICIO CONFIGURACIÓN CORS ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7282") // <-- USA EL PUERTO HTTPS DE TU BLAZOR APP
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
// --- FIN CONFIGURACIÓN CORS ---

// Swagger (si lo usas)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Crear/actualizar DB al inicio
try
{
    using var ctx = new TPIContext(); // Asegúrate que TPIContext esté en el namespace Data
    // Descomenta la línea que corresponda a tu enfoque:
    // ctx.Database.Migrate(); // Si usas Migraciones
    ctx.Database.EnsureCreated(); // Si usas EnsureCreated
}
catch (Exception ex)
{
    Console.WriteLine("DB init error: " + ex.Message);
    // Considera manejar este error de forma más robusta en producción
}

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- INICIO APLICAR CORS ---
app.UseCors(MyAllowSpecificOrigins); // Aplicamos la política CORS
// --- FIN APLICAR CORS ---

// --- INICIO ENDPOINTS MINIMAL API ---

// ---------------- AUTH ----------------
app.MapPost("/auth/register/cliente", (RegisterClienteDTO dto) =>
{
    try { return Results.Ok(new AuthService().RegisterCliente(dto)); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

app.MapPost("/auth/register/vendedor", (RegisterVendedorDTO dto) =>
{
    try { return Results.Ok(new AuthService().RegisterVendedor(dto)); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

app.MapPost("/auth/login", (LoginRequestDTO dto) =>
{
    var resp = new AuthService().Login(dto);
    return resp is null ? Results.Unauthorized() : Results.Ok(resp);
});


// -------------- CATEGORÍAS --------------
app.MapGet("/categorias", () => new CategoriaService().GetAll());

app.MapGet("/categorias/{id:int}", (int id) =>
{
    var dto = new CategoriaService().Get(id);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
});

app.MapPost("/categorias", (CategoriaDTO dto) =>
{
    try { return Results.Created($"/categorias", new CategoriaService().Add(dto)); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

app.MapPut("/categorias", (CategoriaDTO dto) =>
{
    try { return new CategoriaService().Update(dto) ? Results.NoContent() : Results.NotFound(); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

app.MapDelete("/categorias/{id:int}", (int id) =>
    new CategoriaService().Delete(id) ? Results.NoContent() : Results.NotFound());


// ---------------- USUARIOS ----------------
app.MapGet("/usuarios", () => new UsuarioService().GetAll());

app.MapGet("/usuarios/{id:int}", (int id) =>
{
    var dto = new UsuarioService().Get(id);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
});

app.MapPut("/usuarios", (UsuarioDTO dto) =>
{
    try { return new UsuarioService().Update(dto) ? Results.NoContent() : Results.NotFound(); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

app.MapDelete("/usuarios/{id:int}", (int id) =>
    new UsuarioService().Delete(id) ? Results.NoContent() : Results.NotFound());


// ---------- PRODUCTOS ----------
app.MapGet("/productos", () => new ProductoService().GetAll());

// --- ENDPOINT DE DETALLE CORREGIDO ---
app.MapGet("/productos/{id:int}", (int id) =>
{
    var servicio = new ProductoService();
    var dto = servicio.Get(id);
    return dto is null ? Results.NotFound() : Results.Ok(dto); // Devuelve 404 si no existe, 200 OK con el DTO si existe
});
// --- FIN CORRECCIÓN ---

app.MapPost("/productos", (ProductoDTO dto) =>
{
    try { return Results.Created($"/productos", new ProductoService().Add(dto)); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

app.MapPut("/productos", (ProductoDTO dto) =>
{
    try { return new ProductoService().Update(dto) ? Results.NoContent() : Results.NotFound(); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

app.MapDelete("/productos/{id:int}", (int id) =>
    new ProductoService().Delete(id) ? Results.NoContent() : Results.NotFound());

app.MapGet("/productos/{id:int}/historial", (int id) =>
    Results.Ok(new ProductoService().GetHistorial(id))
);


// -------- CARRITO --------
app.MapGet("/carrito/{idCliente:int}", (int idCliente) =>
{
    var s = new CarritoService();
    return Results.Ok(s.GetAbierto(idCliente));
});

app.MapPost("/carrito/agregar", (AgregarCarritoDTO dto) =>
{
    try { return Results.Ok(new CarritoService().Add(dto)); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

app.MapDelete("/carrito/item", (int idCliente, int idProducto) =>
{
    try { return Results.Ok(new CarritoService().Remove(new EliminarItemCarritoDTO { IdCliente = idCliente, IdProducto = idProducto })); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

app.MapPost("/carrito/confirmar", (ConfirmarCarritoDTO dto) =>
{
    try { return Results.Ok(new VentaService().Confirmar(dto.IdCliente)); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});


// -------- VENTAS --------
app.MapGet("/ventas", (int? idCliente, DateTime? desdeUtc, DateTime? hastaUtc) =>
{
    var filtro = new VentaFiltroDTO { IdCliente = idCliente, DesdeUtc = desdeUtc, HastaUtc = hastaUtc };
    return Results.Ok(new VentaService().List(filtro));
});

app.MapGet("/ventas/{id:int}", (int id) =>
{
    var dto = new VentaService().Get(id);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
});

app.MapDelete("/ventas/{id:int}", (int id) =>
{
    try { new VentaService().Delete(id); return Results.NoContent(); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

// --- FIN ENDPOINTS MINIMAL API ---

app.Run();