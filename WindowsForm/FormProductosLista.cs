using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm;

public partial class FormProductosLista : Form
{
    DataGridView grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    BindingSource bs = new();
    FlowLayoutPanel pnl = new() { Dock = DockStyle.Top, Height = 48 };
    Button btnAgregar = new() { Text = "Agregar" };
    Button btnModificar = new() { Text = "Modificar" };
    Button btnEliminar = new() { Text = "Eliminar" };
    Button btnSalir = new() { Text = "Salir" };

    public FormProductosLista()
    {
        InitializeComponent();
        Text = "Productos";
        Width = 900; Height = 520; StartPosition = FormStartPosition.CenterParent;

        pnl.Controls.AddRange(new Control[] { btnAgregar, btnModificar, btnEliminar, btnSalir });
        Controls.Add(grid);
        Controls.Add(pnl);
        grid.DataSource = bs;

        Shown += async (_, __) => await CargarAsync();

        btnAgregar.Click += async (_, __) =>
        {
            using var f = new FormProductoEditar();
            if (f.ShowDialog(this) == DialogResult.OK) await CargarAsync();
        };

        btnModificar.Click += async (_, __) =>
        {
            if (bs.Current is not ProductoDTO sel) return;
            using var f = new FormProductoEditar(sel);
            if (f.ShowDialog(this) == DialogResult.OK) await CargarAsync();
        };

        btnEliminar.Click += async (_, __) =>
        {
            if (bs.Current is not ProductoDTO sel) return;
            if (MessageBox.Show($"¿Eliminar '{sel.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await ProductoApiClient.DeleteAsync(sel.IdProducto);
                await CargarAsync();
            }
        };

        btnSalir.Click += (_, __) => Close();
    }

    private async Task CargarAsync()
        => bs.DataSource = (await ProductoApiClient.GetAllAsync()).ToList();
}
