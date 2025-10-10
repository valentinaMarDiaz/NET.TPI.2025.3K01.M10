using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm;

public partial class FormUsuarioLista : Form
{
    DataGridView grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    BindingSource bs = new();
    FlowLayoutPanel pnl = new() { Dock = DockStyle.Top, Height = 48 };
    Button btnAgregar = new() { Text = "Agregar" };
    Button btnModificar = new() { Text = "Modificar" };
    Button btnEliminar = new() { Text = "Eliminar" };
    Button btnSalir = new() { Text = "Salir" };

    public FormUsuarioLista()
    {
        InitializeComponent();
        Text = "Usuarios";
        Width = 900; Height = 520; StartPosition = FormStartPosition.CenterParent;

        pnl.Controls.AddRange(new Control[] { btnAgregar, btnModificar, btnEliminar, btnSalir });
        Controls.Add(grid);
        Controls.Add(pnl);
        grid.DataSource = bs;

        Shown += async (_, __) => await CargarAsync();

        btnAgregar.Click += async (_, __) =>
        {
            using var f = new FormRegistro();
            if (f.ShowDialog(this) == DialogResult.OK) await CargarAsync();
            else await CargarAsync();
        };

        btnModificar.Click += async (_, __) =>
        {
            if (bs.Current is not UsuarioDTO sel) return;
            using var f = new FormUsuariosEditar(sel); // si aún no lo creaste, comentá esta línea
            if (f.ShowDialog(this) == DialogResult.OK) await CargarAsync();
        };

        btnEliminar.Click += async (_, __) =>
        {
            if (bs.Current is not UsuarioDTO sel) return;
            if (MessageBox.Show($"¿Eliminar '{sel.Nombre} {sel.Apellido}'?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await UsuarioApiClient.DeleteAsync(sel.Id);
                await CargarAsync();
            }
        };

        btnSalir.Click += (_, __) => Close();
    }

    private async Task CargarAsync()
        => bs.DataSource = (await UsuarioApiClient.GetAllAsync()).ToList();
}
