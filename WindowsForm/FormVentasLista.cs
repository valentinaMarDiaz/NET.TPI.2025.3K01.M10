using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm;

public partial class FormVentasLista : Form
{
    BindingSource bs = new();
    DataGridView grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false };

    FlowLayoutPanel top = new() { Dock = DockStyle.Top, Height = 64 };
    TextBox txtIdCliente = new() { PlaceholderText = "Id Cliente (opcional)", Width = 140 };
    DateTimePicker dtDesde = new() { Width = 180, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm" };
    DateTimePicker dtHasta = new() { Width = 180, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm" };
    CheckBox chkDesde = new() { Text = "Desde", AutoSize = true };
    CheckBox chkHasta = new() { Text = "Hasta", AutoSize = true };
    Button btnBuscar = new() { Text = "Buscar" };
    Button btnVerDetalle = new() { Text = "Ver detalle" };
    Button btnEliminar = new() { Text = "Eliminar venta" };
    Button btnSalir = new() { Text = "Salir" };

    public FormVentasLista()
    {
        InitializeComponent();
        Text = "Ventas";
        Width = 1000; Height = 600; StartPosition = FormStartPosition.CenterParent;

        top.Controls.AddRange(new Control[] { txtIdCliente, chkDesde, dtDesde, chkHasta, dtHasta, btnBuscar, btnVerDetalle, btnEliminar, btnSalir });
        Controls.Add(grid); Controls.Add(top);
        grid.DataSource = bs;

        // === columnas explícitas para evitar nombres errados ===
        grid.Columns.Clear();
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "IdVenta",
            DataPropertyName = "IdVenta",
            Width = 80
        });
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "IdCliente",
            DataPropertyName = "IdCliente",
            Width = 80
        });
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Cliente",
            DataPropertyName = "ClienteNombre",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Fecha",
            DataPropertyName = "FechaHoraVentaUtc",
            DefaultCellStyle = { Format = "g" }, // se mostrará UTC; si querés local, transformalo en el DTO o al bindear
            Width = 140
        });
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Total",
            DataPropertyName = "TotalListado",   // <<-- clave: usa el total precalculado para la LISTA
            DefaultCellStyle = { Format = "N2" },
            Width = 110
        });

        btnBuscar.Click += async (_, __) => await BuscarAsync();
        btnVerDetalle.Click += async (_, __) => await VerDetalleAsync();
        btnEliminar.Click += async (_, __) => await EliminarAsync();
        btnSalir.Click += (_, __) => Close();

        Shown += async (_, __) => await BuscarAsync();
    }

    private async Task BuscarAsync()
    {
        int? idCliente = int.TryParse(txtIdCliente.Text, out var id) ? id : null;
        DateTime? d = chkDesde.Checked ? DateTime.SpecifyKind(dtDesde.Value, DateTimeKind.Local).ToUniversalTime() : null;
        DateTime? h = chkHasta.Checked ? DateTime.SpecifyKind(dtHasta.Value, DateTimeKind.Local).ToUniversalTime() : null;

        var lista = (await VentaApiClient.ListAsync(idCliente, d, h)).ToList();
        // Si tu API devuelve Fecha en UTC y querés mostrar LOCAL en la grilla:
        foreach (var v in lista) v.FechaHoraVentaUtc = v.FechaHoraVentaUtc.ToLocalTime();

        bs.DataSource = lista;
    }

    private async Task VerDetalleAsync()
    {
        if (grid.CurrentRow?.DataBoundItem is not VentaDTO v)
            return;

        var det = await VentaApiClient.GetAsync(v.IdVenta);
        if (det is null)
        {
            MessageBox.Show("Sin datos", "Detalle", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Armo un texto claro con descuento (si tuvo)
        var lineas = det.Detalles.Select(d =>
        {
            var baseLinea = $"- {d.ProductoNombre}: {d.Cantidad} x {d.PrecioUnitario:N2}";
            if (d.PorcentajeDescuento.HasValue && !string.IsNullOrWhiteSpace(d.CodigoDescuento))
                baseLinea += $"  |  Cód: {d.CodigoDescuento}  %: {d.PorcentajeDescuento:N2}";
            baseLinea += $"  =>  {d.SubtotalConDescuento:N2}";
            return baseLinea;
        });

        var cuerpo =
            $"Venta #{det.IdVenta}\n" +
            $"Cliente: {det.ClienteNombre}\n" +
            $"Fecha:   {det.FechaHoraVentaUtc.ToLocalTime():g}\n\n" +
            string.Join(Environment.NewLine, lineas) +
            $"\n\nTOTAL: {det.Total:N2}";

        MessageBox.Show(cuerpo, "Detalle de venta", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async Task EliminarAsync()
    {
        if (grid.CurrentRow?.DataBoundItem is not VentaDTO v) return;

        if (MessageBox.Show("¿Eliminar la venta y restaurar stock?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            await VentaApiClient.DeleteAsync(v.IdVenta);
            await BuscarAsync();
        }
    }
}
