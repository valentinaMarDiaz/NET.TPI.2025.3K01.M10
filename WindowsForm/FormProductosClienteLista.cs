using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm;

public partial class FormProductosClienteLista : Form
{
    private readonly int _idCliente;
    private readonly Func<Task> _refrescarBadge;

    private readonly DataGridView _grid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoGenerateColumns = true,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false
    };

    private readonly BindingSource _bs = new();

    private readonly FlowLayoutPanel _pnlTop = new()
    {
        Dock = DockStyle.Top,
        Height = 48
    };

    private readonly Button _btnAgregar = new() { Text = "Agregar al carrito", Width = 150, Height = 32 };
    private readonly Button _btnVolver = new() { Text = "Volver", Width = 120, Height = 32 };

    public FormProductosClienteLista(int idCliente, Func<Task> refrescarBadge)
    {
        InitializeComponent(); 

        _idCliente = idCliente;
        _refrescarBadge = refrescarBadge;

        Text = "Productos";
        Width = 900;
        Height = 520;
        StartPosition = FormStartPosition.CenterParent;

        _pnlTop.Controls.Add(_btnAgregar);
        _pnlTop.Controls.Add(_btnVolver);

        Controls.Add(_grid);
        Controls.Add(_pnlTop);

        _grid.DataSource = _bs;

        Shown += async (_, __) => await CargarAsync();

        _btnAgregar.Click += async (_, __) =>
        {
            if (_bs.Current is not ProductoDTO p)
            {
                MessageBox.Show("Seleccioná un producto.");
                return;
            }

            using var f = new FormCantidad();
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    await CarritoApiClient.AddAsync(_idCliente, p.IdProducto, f.Cantidad);
                    await _refrescarBadge();
                    MessageBox.Show("Producto agregado al carrito.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        };

        _btnVolver.Click += (_, __) => Close();
    }

    private async Task CargarAsync()
    {
        try
        {
            var productos = (await ProductoApiClient.GetAllAsync()).ToList();
            _bs.DataSource = productos;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo cargar la lista de productos.\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

   
}
