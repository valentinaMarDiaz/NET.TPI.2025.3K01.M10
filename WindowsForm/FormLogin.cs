using System.Drawing;
using System.Windows.Forms;
using API.Clients;
using DTOs;
using System.Runtime.InteropServices; // Necesario para CreateRoundRectRgn

namespace WindowsForm;

public partial class FormLogin : Form
{
    TextBox txtEmail = new() { PlaceholderText = "Email", Height = 25 };
    TextBox txtPass = new() { PlaceholderText = "Contraseña", UseSystemPasswordChar = true, Height = 25 };
    Button btnContinuar = new() { Text = "Continuar", Height = 40 };
    Button btnVolver = new() { Text = "Volver", Height = 40 };
    Button btnCancelar = new() { Text = "Cancelar", Height = 40 };

    public FormLogin()
    {
        InitializeComponent();
        Text = "Login";
        Width = 360; Height = 280; StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(245, 247, 250); // Fondo del formulario

        // Panel contenedor para centrar y agrupar
        var panelContenedor = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(20, 20, 20, 20),
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.White
        };
        Controls.Add(panelContenedor);

        // Asegurar ancho de los inputs y añadirlos
        txtEmail.Width = txtPass.Width = 300;
        txtEmail.Margin = txtPass.Margin = new Padding(0, 5, 0, 5);
        panelContenedor.Controls.AddRange(new Control[] { txtEmail, txtPass });

        // Asegurar ancho de los botones y aplicar estilo
        foreach (var btn in new[] { btnContinuar, btnVolver, btnCancelar })
        {
            btn.Width = 300;
            ConfigurarEstiloBoton(btn);
            btn.Margin = new Padding(0, 5, 0, 5);
            panelContenedor.Controls.Add(btn);
        }

        btnVolver.Click += (_, __) => Close();
        btnCancelar.Click += (_, __) => Application.Exit();

        btnContinuar.Click += async (_, __) =>
        {
            try
            {
                var resp = await AuthApiClient.LoginAsync(new LoginRequestDTO { Email = txtEmail.Text.Trim(), Password = txtPass.Text });
                if (resp is null) { MessageBox.Show("Email o contraseña incorrectos."); return; }
                Hide();
                using var f = new FormMenu(resp);
                f.ShowDialog(this);
                Close();
            }
            catch (Exception ex) { MessageBox.Show($"Error de login: {ex.Message}"); }
        };
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