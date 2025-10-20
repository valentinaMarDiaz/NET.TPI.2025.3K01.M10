using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm;

public partial class FormCarrito : Form
{
    readonly int _idCliente;
    readonly Func<Task> _refrescarBadge;

    DataGridView grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    BindingSource bs = new();
    FlowLayoutPanel pnlTop = new() { Dock = DockStyle.Top, Height = 48 };
    Button btnEliminar = new() { Text = "Eliminar producto" };
    Button btnVolver = new() { Text = "Volver" };

    FlowLayoutPanel pnlBottom = new() { Dock = DockStyle.Bottom, Height = 60 };
    Label lblTotal = new() { Text = "Total: $0,00", AutoSize = true, Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold) };
    Button btnConfirmar = new() { Text = "Confirmar compra", Width = 180 };

    public FormCarrito(int idCliente, Func<Task> refrescarBadge)
    {
        InitializeComponent();
        _idCliente = idCliente;
        _refrescarBadge = refrescarBadge;

        Text = "Carrito";
        Width = 900; Height = 520; StartPosition = FormStartPosition.CenterParent;

        pnlTop.Controls.AddRange(new Control[] { btnEliminar, btnVolver });
        pnlBottom.Controls.Add(lblTotal);
        pnlBottom.Controls.Add(btnConfirmar);
        Controls.Add(grid); Controls.Add(pnlTop); Controls.Add(pnlBottom);

        grid.DataSource = bs;

        Shown += async (_, __) => await CargarAsync();

        btnEliminar.Click += async (_, __) =>
        {
            if (bs.Current is not CarritoItemDTO sel) return;
            await CarritoApiClient.RemoveAsync(_idCliente, sel.IdProducto);
            await CargarAsync();
            await _refrescarBadge();
        };

        btnConfirmar.Click += async (_, __) =>
        {
            try
            {
                var venta = await CarritoApiClient.ConfirmarAsync(_idCliente);
                await _refrescarBadge();
                MessageBox.Show($"¡Venta confirmada! ID: {venta.IdVenta}\nTotal: {venta.Total:N2}");
                Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        };

        btnVolver.Click += (_, __) => Close();
    }

    private async Task CargarAsync()
    {
        var c = await CarritoApiClient.GetAsync(_idCliente);
        bs.DataSource = c.Items.ToList();
        lblTotal.Text = $"Total: {c.Total:N2}";
    }
}
