using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm
{
    public partial class FormDescuentosVigentes : Form
    {
        // UI
        private readonly TextBox txtBuscar = new() { PlaceholderText = "Buscar por producto o código...", Width = 260 };
        private readonly Button btnBuscar = new() { Text = "Buscar", Width = 90 };
        private readonly Button btnVolver = new() { Text = "Volver", Width = 90 };
        private readonly DataGridView grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = true,
            AllowUserToAddRows = false
        };
        private readonly BindingSource bs = new();
        private readonly FlowLayoutPanel pnlTop = new() { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8) };

        public FormDescuentosVigentes()
        {
            InitializeComponent();

            Text = "Descuentos vigentes";
            Width = 900;
            Height = 520;
            StartPosition = FormStartPosition.CenterParent;

            pnlTop.Controls.Add(txtBuscar);
            pnlTop.Controls.Add(btnBuscar);
            pnlTop.Controls.Add(btnVolver);

            Controls.Add(grid);
            Controls.Add(pnlTop);

            grid.DataSource = bs;

            btnBuscar.Click += async (_, __) => await CargarAsync();
            txtBuscar.KeyDown += async (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await CargarAsync(); } };
            btnVolver.Click += (_, __) => Close();

            Shown += async (_, __) => await CargarAsync();
        }

        private async Task CargarAsync()
        {
            try
            {
                var filtro = txtBuscar.Text?.Trim();
                var list = await DescuentoApiClient.GetVigentesAsync(string.IsNullOrWhiteSpace(filtro) ? null : filtro);

                bs.DataSource = list.ToList();

                // Formato amigable de columnas si existen
                if (grid.Columns["FechaInicioUtc"] is not null)
                    grid.Columns["FechaInicioUtc"].HeaderText = "Inicio (UTC)";
                if (grid.Columns["FechaCaducidadUtc"] is not null)
                    grid.Columns["FechaCaducidadUtc"].HeaderText = "Caduca (UTC)";
                if (grid.Columns["Porcentaje"] is not null)
                {
                    grid.Columns["Porcentaje"].DefaultCellStyle.Format = "N2";
                    grid.Columns["Porcentaje"].HeaderText = "%";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
