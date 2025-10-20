using System.Drawing;
using System.Windows.Forms;
using DTOs;

namespace WindowsForm;

public partial class FormMenu : Form
{
    readonly LoginResponseDTO _user;
    Button btnUsuarios = new() { Text = "Lista de usuarios", Height = 40, Width = 200 };
    Button btnCategorias = new() { Text = "Lista de categorías", Height = 40, Width = 200 };
    Button btnProductos = new() { Text = "Productos", Height = 40, Width = 200 };
    Button btnHistorial = new() { Text = "Historial de precios", Height = 40, Width = 200 };
    Button btnSalir = new() { Text = "Salir", Height = 40, Width = 200 };
    Label lblBienvenida = new() { Dock = DockStyle.Top, Height = 60, Font = new Font("Segoe UI", 14, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };

    public FormMenu(LoginResponseDTO user)
    {
        InitializeComponent();
        _user = user;
        Text = "Menú principal";
        Width = 400;
        Height = 400;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(245, 247, 250);

        // Label superior
        lblBienvenida.Text = $"Bienvenido/a {_user.Nombre} {_user.Apellido}";
        lblBienvenida.ForeColor = Color.FromArgb(45, 62, 80);

        // Panel contenedor centrado
        var panelBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(0, 20, 0, 20),
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.White,
        };

        // Centrar horizontalmente
        panelBotones.Layout += (_, __) =>
        {
            foreach (Control ctrl in panelBotones.Controls)
                ctrl.Margin = new Padding((panelBotones.ClientSize.Width - ctrl.Width) / 2, 6, 0, 6);
        };

        // Aplicar estilo a cada botón
        ConfigurarEstiloBoton(btnUsuarios);
        ConfigurarEstiloBoton(btnCategorias);
        ConfigurarEstiloBoton(btnProductos);
        ConfigurarEstiloBoton(btnHistorial);
        ConfigurarEstiloBoton(btnSalir);

        // Agregar botones
        panelBotones.Controls.AddRange(new Control[]
        {
            btnUsuarios,
            btnCategorias,
            btnProductos,
            btnHistorial,
            btnSalir
        });

        Controls.Add(panelBotones);
        Controls.Add(lblBienvenida);

        // Eventos
        btnUsuarios.Click += (_, __) => { using var f = new FormUsuarioLista(); f.ShowDialog(this); };
        btnCategorias.Click += (_, __) => { using var f = new FormCategoriaLista(); f.ShowDialog(this); };
        btnProductos.Click += (_, __) => { using var f = new FormProductosLista(); f.ShowDialog(this); };
        btnHistorial.Click += (_, __) => { using var f = new FormHistorialPrecios(); f.ShowDialog(this); };
        btnSalir.Click += (_, __) => Application.Exit();

        ConfigurarMenuPorRol();
    }

    private void ConfigurarEstiloBoton(Button boton)
    {
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.BackColor = Color.FromArgb(52, 152, 219);
        boton.ForeColor = Color.White;
        boton.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        boton.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, boton.Width, boton.Height, 15, 15));

        // Hover
        boton.MouseEnter += (_, __) => boton.BackColor = Color.FromArgb(41, 128, 185);
        boton.MouseLeave += (_, __) => boton.BackColor = Color.FromArgb(52, 152, 219);
    }

    [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse);

    private void ConfigurarMenuPorRol()
    {
        if (_user.TipoUsuario == "Cliente")
        {
            btnProductos.Visible = true;
            btnUsuarios.Visible = false;
            btnCategorias.Visible = false;
            btnHistorial.Visible = false;
        }
        else if (_user.TipoUsuario == "Vendedor")
        {
            btnProductos.Visible = true;
            btnUsuarios.Visible = true;
            btnCategorias.Visible = true;
            btnHistorial.Visible = true;
        }
        else
        {
            btnProductos.Visible = false;
            btnUsuarios.Visible = false;
            btnCategorias.Visible = false;
            btnHistorial.Visible = false;
            MessageBox.Show("Tipo de usuario desconocido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
