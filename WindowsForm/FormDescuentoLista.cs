using System.Drawing;
using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm
{
    public partial class FormDescuentoLista : Form
    {
        readonly TextBox txtFiltroProd = new() { Width = 220, PlaceholderText = "Filtrar por producto..." };
        readonly Button btnBuscar = new() { Text = "Buscar", Width = 100, Height = 32 };
        readonly Button btnAgregar = new() { Text = "Agregar", Width = 100, Height = 32 };
        readonly Button btnModificar = new() { Text = "Modificar", Width = 100, Height = 32 };
        readonly Button btnEliminar = new() { Text = "Eliminar", Width = 100, Height = 32 };
        readonly Button btnSalir = new() { Text = "Salir", Width = 100, Height = 32 };
        readonly DataGridView grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

        public FormDescuentoLista()
        {
            Text = "Descuentos";
            Width = 950;
            Height = 540;
            StartPosition = FormStartPosition.CenterParent;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight };
            top.Controls.AddRange(new Control[] { txtFiltroProd, btnBuscar, btnAgregar, btnModificar, btnEliminar, btnSalir });

            Controls.Add(grid);
            Controls.Add(top);

            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = "Codigo", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Producto", DataPropertyName = "ProductoNombre", Width = 200 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "%", DataPropertyName = "Porcentaje", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Inicio (UTC)", DataPropertyName = "FechaInicioUtc", Width = 130 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Caduca (UTC)", DataPropertyName = "FechaCaducidadUtc", Width = 130 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descripción", DataPropertyName = "Descripcion", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            btnBuscar.Click += async (_, __) => await CargarAsync();
            btnSalir.Click += (_, __) => Close();

            btnAgregar.Click += async (_, __) =>
            {
                using var f = new FormDescuentoEditar();
                if (f.ShowDialog(this) == DialogResult.OK)
                    await CargarAsync();
            };

            btnModificar.Click += async (_, __) =>
            {
                if (grid.CurrentRow?.DataBoundItem is DescuentoDTO dto)
                {
                    using var f = new FormDescuentoEditar(dto);
                    if (f.ShowDialog(this) == DialogResult.OK)
                        await CargarAsync();
                }
            };

            btnEliminar.Click += async (_, __) =>
            {
                if (grid.CurrentRow?.DataBoundItem is not DescuentoDTO dto) return;
                if (MessageBox.Show($"¿Eliminar descuento {dto.Codigo}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try { await DescuentoApiClient.DeleteAsync(dto.IdDescuento); await CargarAsync(); }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            };

            Shown += async (_, __) => await CargarAsync();
        }

        private async Task CargarAsync()
        {
            try
            {
                var list = await DescuentoApiClient.GetAllAsync(txtFiltroProd.Text);
                grid.DataSource = list.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
