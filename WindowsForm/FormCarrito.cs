using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm
{
    public partial class FormCarrito : Form

    {
        private readonly int _idCliente;
        private readonly Func<Task> _refrescarBadge;

        private readonly BindingSource bs = new();
        private readonly DataGridView grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        private readonly FlowLayoutPanel pnlTop = new() { Dock = DockStyle.Top, Height = 52, Padding = new Padding(12, 10, 12, 10) };
        private readonly Button btnEliminar = new() { Text = "Eliminar producto", Height = 34, Width = 160 };
        private readonly Button btnAplicarCodigo = new() { Text = "Aplicar código", Height = 34, Width = 140 };
        private readonly Button btnVolver = new() { Text = "Volver", Height = 34, Width = 100 };

        private readonly FlowLayoutPanel pnlBottom = new() { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(12, 10, 12, 10) };
        private readonly Label lblTotal = new() { Text = "Total: $0,00", AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
        private readonly Button btnConfirmar = new() { Text = "Confirmar compra", Height = 36, Width = 180 };

        public FormCarrito(int idCliente, Func<Task> refrescarBadge)
        {
            _idCliente = idCliente;
            _refrescarBadge = refrescarBadge;

            Text = "Carrito";
            Width = 980;
            Height = 560;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);

           
            ConfigurarEstiloBoton(btnEliminar);
            ConfigurarEstiloBoton(btnAplicarCodigo);
            ConfigurarEstiloBoton(btnVolver);
            ConfigurarEstiloBoton(btnConfirmar);

            
            pnlTop.Controls.AddRange(new Control[] { btnEliminar, btnAplicarCodigo, btnVolver });
            pnlBottom.Controls.Add(lblTotal);
            pnlBottom.Controls.Add(new Label() { Width = 20 }); 
            pnlBottom.Controls.Add(btnConfirmar);

            Controls.Add(grid);
            Controls.Add(pnlTop);
            Controls.Add(pnlBottom);

           
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductoNombre",
                HeaderText = "Producto"
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Cantidad",
                HeaderText = "Cant.",
                Width = 70
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PrecioUnitario",
                HeaderText = "Precio",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CodigoDescuento",
                HeaderText = "Código"
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Porcentaje",
                HeaderText = "%",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" },
                Width = 70
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Subtotal",
                HeaderText = "Subtotal",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            grid.DataSource = bs;

            
            Shown += async (_, __) => await CargarAsync();

            btnEliminar.Click += async (_, __) =>
            {
                if (bs.Current is not CarritoItemDTO sel) return;
                try
                {
                    await CarritoApiClient.RemoveAsync(_idCliente, sel.IdProducto);
                    await CargarAsync();
                    await _refrescarBadge();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnAplicarCodigo.Click += async (_, __) =>
            {
                while (true)
                {
                    var codigo = PedirCodigo();
                    if (codigo == null) break; 

                    try
                    {
                        var d = await DescuentoApiClient.ValidarCodigoAsync(codigo);
                        if (d == null)
                        {
                            MessageBox.Show("El código no existe o está vencido.", "Descuento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }

                        await CarritoApiClient.AplicarCodigoAsync(_idCliente, codigo);
                        await CargarAsync();

                        var otro = MessageBox.Show("Código aplicado. ¿Querés ingresar otro?", "Descuento",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (otro == DialogResult.No) break;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        
                    }
                }
            };

            btnConfirmar.Click += async (_, __) =>
            {
                try
                {
                    var venta = await CarritoApiClient.ConfirmarAsync(_idCliente);
                    await _refrescarBadge();
                    MessageBox.Show($"¡Venta confirmada!\nID: {venta.IdVenta}\nTotal: {venta.Total:N2}", "OK",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnVolver.Click += (_, __) => Close();
        }

        private async Task CargarAsync()
        {
            var c = await CarritoApiClient.GetAsync(_idCliente);
            bs.DataSource = c.Items.ToList();
            lblTotal.Text = $"Total: {c.Total:N2}";
        }

        
        private void ConfigurarEstiloBoton(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(52, 152, 219);
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            boton.SizeChanged += (_, __) =>
            {
                var rgn = CreateRoundRectRgn(0, 0, boton.Width, boton.Height, 12, 12);
                boton.Region = Region.FromHrgn(rgn);
            };
            boton.MouseEnter += (_, __) => boton.BackColor = Color.FromArgb(41, 128, 185);
            boton.MouseLeave += (_, __) => boton.BackColor = Color.FromArgb(52, 152, 219);
        }

        private string? PedirCodigo()
        {
            using var dlg = new Form
            {
                Text = "Código de descuento",
                Width = 380,
                Height = 160,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var lbl = new Label { Text = "Ingresá el código:", Left = 12, Top = 15, AutoSize = true };
            var txt = new TextBox { Left = 12, Top = 40, Width = 340 };
            var ok = new Button { Text = "Aceptar", Left = 190, Top = 75, Width = 80, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Cancelar", Left = 272, Top = 75, Width = 80, DialogResult = DialogResult.Cancel };

            ConfigurarEstiloBoton(ok);
            ConfigurarEstiloBoton(cancel);

            dlg.Controls.AddRange(new Control[] { lbl, txt, ok, cancel });
            dlg.AcceptButton = ok;
            dlg.CancelButton = cancel;

            return dlg.ShowDialog(this) == DialogResult.OK ? txt.Text.Trim() : null;
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);
    }
}
