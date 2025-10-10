using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using API.Clients;   // Cliente HTTP hacia la WebAPI
using DTOs;         // CategoriaDTO

namespace WindowsForm;

public partial class FormCategorias : Form
{
    public FormCategorias()
    {
        InitializeComponent();

        // Eventos
        this.Load += async (_, __) => await CargarAsync();
        btnAgregar.Click += async (_, __) => await AgregarAsync();

        // Enter para agregar rápido
        txtNombre.KeyDown += async (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                await AgregarAsync();
            }
        };
    }

    private async Task CargarAsync()
    {
        try
        {
            var categorias = (await CategoriaApiClient.GetAllAsync()).ToList();
            bs.DataSource = categorias;              // 'bs' está declarado en el Designer
            grid.AutoGenerateColumns = true;         // por si no lo dejaste en el Designer
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar categorías:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task AgregarAsync()
    {
        var nombre = (txtNombre.Text ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MessageBox.Show("Ingresá un nombre de categoría.",
                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtNombre.Focus();
            return;
        }

        try
        {
            await CategoriaApiClient.AddAsync(new CategoriaDTO { Nombre = nombre });
            txtNombre.Clear();
            await CargarAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo crear la categoría:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
