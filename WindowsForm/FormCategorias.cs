using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using API.Clients;   
using DTOs;         

namespace WindowsForm;

public partial class FormCategorias : Form
{
    public FormCategorias()
    {
        InitializeComponent();

     
        this.Load += async (_, __) => await CargarAsync();
        btnAgregar.Click += async (_, __) => await AgregarAsync();

       
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
            bs.DataSource = categorias;              
            grid.AutoGenerateColumns = true;         
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
