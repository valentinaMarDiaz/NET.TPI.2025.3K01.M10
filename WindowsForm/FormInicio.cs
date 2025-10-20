using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DTOs;

namespace WindowsForm;

public partial class FormInicio : Form
{
    public LoginResponseDTO? AuthUser { get; private set; }
    public bool WasCancelled { get; private set; }

    readonly Label titulo = new()
    {
        Text = "Smartienda",
        Dock = DockStyle.Top,
        Height = 60,
        Font = new Font("Segoe UI", 18, FontStyle.Bold),
        ForeColor = Color.FromArgb(45, 62, 80),
        TextAlign = ContentAlignment.MiddleCenter
    };

    readonly Button btnLogin = new() { Text = "Iniciar sesión", Height = 40, Width = 240 };
    readonly Button btnRegistro = new() { Text = "Registrarse", Height = 40, Width = 240 };
    readonly Button btnCancelar = new() { Text = "Cancelar", Height = 40, Width = 240 };

    public FormInicio()
    {
        InitializeComponent();

        Text = "Inicio";
        Width = 420;
        Height = 320;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(245, 247, 250);

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(0, 20, 0, 20),
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.White
        };
        panel.Layout += (_, __) =>
        {
            foreach (Control c in panel.Controls)
                c.Margin = new Padding((panel.ClientSize.Width - c.Width) / 2, 8, 0, 8);
        };

        EstiloBotonAzul(btnLogin);
        EstiloBotonAzul(btnRegistro);
        EstiloBotonAzul(btnCancelar);

        Controls.Add(panel);
        Controls.Add(titulo);

        panel.Controls.AddRange(new Control[] { btnLogin, btnRegistro, btnCancelar });

        btnLogin.Click += (_, __) =>
        {
            using var f = new FormLogin();
            if (f.ShowDialog(this) == DialogResult.OK && f.User != null)
            {
                AuthUser = f.User;
                Close();
            }
        };

        btnRegistro.Click += (_, __) =>
        {
            using var f = new FormRegistro();
            if (f.ShowDialog(this) == DialogResult.OK && f.User != null)
            {
                AuthUser = f.User;
                Close();
            }
        };

        btnCancelar.Click += (_, __) =>
        {
            WasCancelled = true;
            Close();
        };
    }

    // Estilo común
    private void EstiloBotonAzul(Button b)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 0;
        b.BackColor = Color.FromArgb(52, 152, 219);
        b.ForeColor = Color.White;
        b.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        b.SizeChanged += (_, __) =>
        {
            var h = CreateRoundRectRgn(0, 0, b.Width, b.Height, 15, 15);
            b.Region = Region.FromHrgn(h);
        };
        b.MouseEnter += (_, __) => b.BackColor = Color.FromArgb(41, 128, 185);
        b.MouseLeave += (_, __) => b.BackColor = Color.FromArgb(52, 152, 219);
    }

    [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

    // Stub para no depender del Designer
    private void InitializeComponent() { }
}
