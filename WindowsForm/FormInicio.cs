using System.Drawing;
using System.Windows.Forms;
using DTOs; // Necesario para FormMenu
using System.Runtime.InteropServices; // Necesario para CreateRoundRectRgn

namespace WindowsForm;

public partial class FormInicio : Form
{
    Button btnLogin = new() { Text = "Iniciar sesión", Height = 40, Width = 300 };
    Button btnRegistro = new() { Text = "Registrarse", Height = 40, Width = 300 };
    Button btnCancelar = new() { Text = "Cancelar", Height = 40, Width = 300 };

    public FormInicio()
    {
        InitializeComponent();
        Text = "SmartTienda - Inicio";
        Width = 360; Height = 260; StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(245, 247, 250); // Fondo del formulario

        // Panel contenedor para centrar y agrupar
        var panelContenedor = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(20, 30, 20, 20),
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.White
        };
        Controls.Add(panelContenedor);

        // Aplicar estilo a los botones
        foreach (var btn in new[] { btnLogin, btnRegistro, btnCancelar })
        {
            ConfigurarEstiloBoton(btn);
            btn.Margin = new Padding(0, 6, 0, 6);
            panelContenedor.Controls.Add(btn);
        }

        // Eventos
        btnLogin.Click += (_, __) => { using var f = new FormLogin(); f.ShowDialog(this); };
        btnRegistro.Click += (_, __) => { using var f = new FormRegistro(); f.ShowDialog(this); };
        btnCancelar.Click += (_, __) => Close();
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

    [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse);
}