using Application.Services; // Services
using DTOs;                 // DTOs
using Data;                 // TPIContext
using Microsoft.EntityFrameworkCore; // Migrate()
using System;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CORS: una sola policy, y con http/https del Blazor host (7282) ---
var allowedOrigins = new[] {
    "https://localhost:7282",
    "http://localhost:7282"
};
builder.Services.AddCors(o =>
{
    o.AddPolicy("Frontend", p => p
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Crear/actualizar DB al inicio
try
{
    using var ctx = new TPIContext();
    ctx.Database.Migrate();      // usa migraciones
    // ctx.Database.EnsureCreated(); // alternativo si no usás migraciones
}
catch (Exception ex)
{
    Console.WriteLine("DB init error: " + ex.Message);
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");

// ===================== ENDPOINTS MINIMAL API =====================

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

app.MapGet("/productos/{id:int}", (int id) =>
{
    var servicio = new ProductoService();
    var dto = servicio.Get(id);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
});

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
    Results.Ok(new ProductoService().GetHistorial(id)));

// --- CARRITO ---
app.MapGet("/carrito/{idCliente:int}", (int idCliente) =>
{
    var s = new CarritoService();
    return Results.Ok(s.Get(idCliente));
});

app.MapDelete("/carrito/{idCliente:int}/producto/{idProducto:int}", (int idCliente, int idProducto) =>
{
    new CarritoService().Remove(idCliente, idProducto);
    return Results.NoContent();
});

app.MapPost("/carrito/{idCliente:int}/aplicar-codigo", (int idCliente, string codigo) =>
{
    new CarritoService().AplicarCodigo(idCliente, codigo);
    return Results.NoContent();
});

app.MapPost("/carrito/{idCliente:int}/confirmar", (int idCliente) =>
{
    var s = new VentaService();
    var venta = s.Confirmar(idCliente); // crea Venta + Detalles y descuenta stock
    return Results.Ok(new { IdVenta = venta.IdVenta, Total = venta.Total });
});

app.MapPost("/carrito/{idCliente:int}/item", (int idCliente, int producto, int cantidad) =>
{
    try
    {
        var s = new Application.Services.CarritoService();
        s.AddOrIncreaseItem(idCliente, producto, cantidad);
        return Results.NoContent();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
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

// ----- DESCUENTOS -----
// Listado (si tu servicio soporta filtro, podés cambiar a: (string? producto) => Results.Ok(new DescuentoService().GetAll(producto)))
app.MapGet("/descuentos", () =>
{
    return Results.Ok(new DescuentoService().GetAll());
});

// *** NUEVO: detalle por ID (lo que faltaba) ***
app.MapGet("/descuentos/{id:int}", (int id) =>
{
    var dto = new DescuentoService().Get(id);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
});

// Crear: recibo CUDTO del cliente y lo mapeo al DTO del service
app.MapPost("/descuentos", (DescuentoCUDTO cu) =>
{
    try
    {
        var dto = new DescuentoDTO
        {
            IdProducto = cu.IdProducto,
            FechaInicioUtc = cu.FechaInicioUtc,
            FechaCaducidadUtc = cu.FechaCaducidadUtc,
            Descripcion = cu.Descripcion,
            Codigo = cu.Codigo,
            Porcentaje = cu.Porcentaje
        };
        var creado = new DescuentoService().Add(dto);
        return Results.Created("/descuentos", creado);
    }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

// Actualizar: idem, mapear CUDTO -> DTO
app.MapPut("/descuentos", (DescuentoCUDTO cu) =>
{
    try
    {
        var dto = new DescuentoDTO
        {
            IdDescuento = cu.IdDescuento,
            IdProducto = cu.IdProducto,
            FechaInicioUtc = cu.FechaInicioUtc,
            FechaCaducidadUtc = cu.FechaCaducidadUtc,
            Descripcion = cu.Descripcion,
            Codigo = cu.Codigo,
            Porcentaje = cu.Porcentaje
        };
        return new DescuentoService().Update(dto) ? Results.NoContent() : Results.NotFound();
    }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

app.MapDelete("/descuentos/{id:int}", (int id) =>
{
    return new DescuentoService().Delete(id) ? Results.NoContent() : Results.NotFound();
});

// ----- DESCUENTOS (CLIENTE) -----
app.MapGet("/descuentos/vigentes", (string? texto) =>
{
    var svc = new Application.Services.DescuentoService();
    return Results.Ok(svc.GetVigentes(texto, DateTime.UtcNow));
})
.WithName("GetDescuentosVigentes")
.Produces<List<DTOs.DescuentoDTO>>(StatusCodes.Status200OK)
.WithOpenApi();

app.MapGet("/descuentos/validar", (string codigo) =>
{
    var svc = new Application.Services.DescuentoService();
    var dto = svc.GetByCodigoVigente(codigo);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
})
.WithName("ValidarDescuento")
.Produces<DTOs.DescuentoDTO>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi();

// ===================== FIN ENDPOINTS =====================

app.Run();
