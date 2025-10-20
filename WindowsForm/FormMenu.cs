using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm
{
    public partial class FormMenu : Form
    {
        private readonly LoginResponseDTO _user;

        // ---- expuesto para el ApplicationContext ----
        public bool LogoutRequested { get; private set; } = false;

        private readonly Label lblBienvenida = new()
        {
            Dock = DockStyle.Top,
            Height = 60,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };

        private readonly FlowLayoutPanel panelBotones = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(0, 20, 0, 20),
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.White
        };

        // En vez de “Salir”, ahora es “Cerrar sesión”
        private readonly Button btnCerrarSesion = new() { Text = "Cerrar sesión", Height = 40, Width = 240 };

        // Vendedor
        private readonly Button btnUsuarios = new() { Text = "Lista de usuarios", Height = 40, Width = 240 };
        private readonly Button btnCategorias = new() { Text = "Lista de categorías", Height = 40, Width = 240 };
        private readonly Button btnProductos = new() { Text = "Productos", Height = 40, Width = 240 };
        private readonly Button btnHistorial = new() { Text = "Historial de precios", Height = 40, Width = 240 };
        private readonly Button btnVentas = new() { Text = "Ventas", Height = 40, Width = 240 };

        // Cliente
        private readonly Button btnProductosCliente = new() { Text = "Productos", Height = 40, Width = 240 };
        private readonly Button btnCarrito = new() { Text = "Carrito", Height = 40, Width = 240 };
        private readonly Label lblBadge = new() { Text = "0", AutoSize = true, BackColor = Color.Red, ForeColor = Color.White, Padding = new Padding(6, 2, 6, 2) };

        public FormMenu(LoginResponseDTO user)
        {
            InitializeComponent();

            _user = user;

            Text = "Menú principal";
            Width = 420;
            Height = 460;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);

            lblBienvenida.Text = $"Bienvenido/a {_user.Nombre} {_user.Apellido}";
            lblBienvenida.ForeColor = Color.FromArgb(45, 62, 80);

            panelBotones.Layout += (_, __) =>
            {
                foreach (Control ctrl in panelBotones.Controls)
                    ctrl.Margin = new Padding((panelBotones.ClientSize.Width - ctrl.Width) / 2, 8, 0, 8);
            };

            ConfigurarEstiloBoton(btnUsuarios);
            ConfigurarEstiloBoton(btnCategorias);
            ConfigurarEstiloBoton(btnProductos);
            ConfigurarEstiloBoton(btnHistorial);
            ConfigurarEstiloBoton(btnVentas);

            ConfigurarEstiloBoton(btnProductosCliente);
            ConfigurarEstiloBoton(btnCarrito);
            ConfigurarEstiloBoton(btnCerrarSesion);

            Controls.Add(panelBotones);
            Controls.Add(lblBienvenida);

            if (_user.TipoUsuario == "Cliente")
            {
                btnCarrito.Controls.Add(lblBadge);
                btnCarrito.SizeChanged += (_, __) =>
                {
                    lblBadge.Location = new Point(btnCarrito.Width - lblBadge.Width - 12, 6);
                };

                panelBotones.Controls.AddRange(new Control[]
                {
                    btnProductosCliente,
                    btnCarrito,
                    btnCerrarSesion
                });

                btnProductosCliente.Click += (_, __) =>
                {
                    using var f = new FormProductosClienteLista(_user.Id, RefreshBadgeAsync);
                    f.ShowDialog(this);
                };

                btnCarrito.Click += (_, __) =>
                {
                    using var f = new FormCarrito(_user.Id, RefreshBadgeAsync);
                    f.ShowDialog(this);
                };

                Shown += async (_, __) => await RefreshBadgeAsync();
            }
            else if (_user.TipoUsuario == "Vendedor")
            {
                panelBotones.Controls.AddRange(new Control[]
                {
                    btnUsuarios,
                    btnCategorias,
                    btnProductos,
                    btnHistorial,
                    btnVentas,
                    btnCerrarSesion
                });

                btnUsuarios.Click += (_, __) => { using var f = new FormUsuarioLista(); f.ShowDialog(this); };
                btnCategorias.Click += (_, __) => { using var f = new FormCategoriaLista(); f.ShowDialog(this); };
                btnProductos.Click += (_, __) => { using var f = new FormProductosLista(); f.ShowDialog(this); };
                btnHistorial.Click += (_, __) => { using var f = new FormHistorialPrecios(); f.ShowDialog(this); };
                btnVentas.Click += (_, __) => { using var f = new FormVentasLista(); f.ShowDialog(this); };
            }
            else
            {
                MessageBox.Show("Tipo de usuario desconocido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogoutAndClose();
            }

            // Cerrar sesión (volver al Inicio)
            btnCerrarSesion.Click += (_, __) => LogoutAndClose();

            // Si el usuario cierra con la X, lo tratamos como logout también (opcional)
            FormClosing += (_, e) =>
            {
                if (!LogoutRequested) LogoutRequested = true;
            };
        }

        private void LogoutAndClose()
        {
            LogoutRequested = true;
            Close();                // NO Application.Exit()
        }

        private void ConfigurarEstiloBoton(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(52, 152, 219);
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            boton.SizeChanged += (_, __) =>
            {
                var rgn = CreateRoundRectRgn(0, 0, boton.Width, boton.Height, 15, 15);
                boton.Region = Region.FromHrgn(rgn);
            };
            boton.MouseEnter += (_, __) => boton.BackColor = Color.FromArgb(41, 128, 185);
            boton.MouseLeave += (_, __) => boton.BackColor = Color.FromArgb(52, 152, 219);
        }

        private async Task RefreshBadgeAsync()
        {
            try
            {
                var c = await CarritoApiClient.GetAsync(_user.Id);
                lblBadge.Text = c.CantidadTotal.ToString();
                lblBadge.Location = new Point(btnCarrito.Width - lblBadge.Width - 12, 6);
            }
            catch
            {
                lblBadge.Text = "0";
            }
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);
    }
}
