using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm;

public partial class FormVentasLista : Form
{
    BindingSource bs = new();
    DataGridView grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };

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

        btnBuscar.Click += async (_, __) => await BuscarAsync();
        btnVerDetalle.Click += async (_, __) =>
        {
            if (grid.CurrentRow?.DataBoundItem is VentaDTO v)
            {
                var det = await VentaApiClient.GetAsync(v.IdVenta);
                MessageBox.Show(det is null ? "Sin datos" :
                    $"Venta #{det.IdVenta}\nCliente: {det.ClienteNombre}\nFecha: {det.FechaHoraVentaUtc.ToLocalTime():g}\n" +
                    string.Join(Environment.NewLine, det.Detalles.Select(d => $"- {d.ProductoNombre}: {d.Cantidad} x {d.PrecioUnitario:N2} = {d.Subtotal:N2}")) +
                    $"\nTOTAL: {det.Total:N2}");
            }
        };
        btnEliminar.Click += async (_, __) =>
        {
            if (grid.CurrentRow?.DataBoundItem is VentaDTO v)
            {
                if (MessageBox.Show("¿Eliminar la venta y restaurar stock?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    await VentaApiClient.DeleteAsync(v.IdVenta);
                    await BuscarAsync();
                }
            }
        };
        btnSalir.Click += (_, __) => Close();

        Shown += async (_, __) => await BuscarAsync();
    }

    private async Task BuscarAsync()
    {
        int? idCliente = int.TryParse(txtIdCliente.Text, out var id) ? id : null;
        DateTime? d = chkDesde.Checked ? DateTime.SpecifyKind(dtDesde.Value, DateTimeKind.Local).ToUniversalTime() : null;
        DateTime? h = chkHasta.Checked ? DateTime.SpecifyKind(dtHasta.Value, DateTimeKind.Local).ToUniversalTime() : null;

        var lista = await VentaApiClient.ListAsync(idCliente, d, h);
        bs.DataSource = lista.ToList();
    }
}
