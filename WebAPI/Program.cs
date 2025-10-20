using Application.Services;
using DTOs;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;


var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Crear/actualizar DB al inicio (usa TPIContext.OnConfiguring)
try
{
    using var ctx = new TPIContext();
    ctx.Database.Migrate(); // o EnsureCreated() si no usás migraciones
}
catch (Exception ex)
{
    Console.WriteLine("DB init error: " + ex.Message);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


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
// (Alta se hace por /auth/register/...)
// Lista
app.MapGet("/usuarios", () => new UsuarioService().GetAll());

// Detalle
app.MapGet("/usuarios/{id:int}", (int id) =>
{
    var dto = new UsuarioService().Get(id);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
});

// Modificar
app.MapPut("/usuarios", (UsuarioDTO dto) =>
{
    try { return new UsuarioService().Update(dto) ? Results.NoContent() : Results.NotFound(); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

// Eliminar
app.MapDelete("/usuarios/{id:int}", (int id) =>
    new UsuarioService().Delete(id) ? Results.NoContent() : Results.NotFound());

// ---------- PRODUCTOS ----------
app.MapGet("/productos", () => new ProductoService().GetAll());

app.MapGet("/productos/{id:int}", (int id) =>
{
    var dto = new ProductoService().Get(id);
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

// Historial de precios de un producto
app.MapGet("/productos/{id:int}/historial", (int id) =>
    Results.Ok(new ProductoService().GetHistorial(id))
);


// -------- CARRITO --------
app.MapGet("/carrito/{idCliente:int}", (int idCliente) =>
{
    var s = new CarritoService();
    return Results.Ok(s.GetAbierto(idCliente));
}).WithOpenApi();

app.MapPost("/carrito/agregar", (AgregarCarritoDTO dto) =>
{
    try { return Results.Ok(new CarritoService().Add(dto)); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).WithOpenApi();

app.MapDelete("/carrito/item", (int idCliente, int idProducto) =>
{
    try { return Results.Ok(new CarritoService().Remove(new EliminarItemCarritoDTO { IdCliente = idCliente, IdProducto = idProducto })); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).WithOpenApi();

// confirmar carrito ? crea venta
app.MapPost("/carrito/confirmar", (ConfirmarCarritoDTO dto) =>
{
    try { return Results.Ok(new VentaService().Confirmar(dto.IdCliente)); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).WithOpenApi();

// -------- VENTAS --------
app.MapGet("/ventas", (int? idCliente, DateTime? desdeUtc, DateTime? hastaUtc) =>
{
    var filtro = new VentaFiltroDTO { IdCliente = idCliente, DesdeUtc = desdeUtc, HastaUtc = hastaUtc };
    return Results.Ok(new VentaService().List(filtro));
}).WithOpenApi();

app.MapGet("/ventas/{id:int}", (int id) =>
{
    var dto = new VentaService().Get(id);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
}).WithOpenApi();

app.MapDelete("/ventas/{id:int}", (int id) =>
{
    try { new VentaService().Delete(id); return Results.NoContent(); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).WithOpenApi();


app.Run();
