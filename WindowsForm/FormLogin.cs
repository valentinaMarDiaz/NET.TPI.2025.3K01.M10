using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm;

public partial class FormLogin : Form
{
    public LoginResponseDTO? User { get; private set; }

    readonly Label titulo = new()
    {
        Text = "Iniciar sesión",
        Dock = DockStyle.Top,
        Height = 50,
        Font = new Font("Segoe UI", 16, FontStyle.Bold),
        ForeColor = Color.FromArgb(45, 62, 80),
        TextAlign = ContentAlignment.MiddleCenter
    };

    readonly TextBox txtEmail = new() { Width = 260 };
    readonly TextBox txtPassword = new() { Width = 260, UseSystemPasswordChar = true };

    readonly Button btnIniciar = new() { Text = "Iniciar", Height = 40, Width = 140 };
    readonly Button btnCancelar = new() { Text = "Cancelar", Height = 40, Width = 140 };

    public FormLogin()
    {
        InitializeComponent();

        Text = "Login";
        Width = 460;
        Height = 280;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(245, 247, 250);

        var formPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(24),
            BackColor = Color.White
        };
        formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        formPanel.Controls.Add(new Label { Text = "Email", AutoSize = true, Font = new Font("Segoe UI", 10) }, 0, 0);
        formPanel.Controls.Add(txtEmail, 1, 0);
        formPanel.Controls.Add(new Label { Text = "Contraseña", AutoSize = true, Font = new Font("Segoe UI", 10) }, 0, 1);
        formPanel.Controls.Add(txtPassword, 1, 1);

        var botones = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
        botones.Controls.AddRange(new Control[] { btnIniciar, btnCancelar });
        formPanel.Controls.Add(botones, 1, 2);

        Controls.Add(formPanel);
        Controls.Add(titulo);

        EstiloBotonAzul(btnIniciar);
        EstiloBotonAzul(btnCancelar);

        btnIniciar.Click += async (_, __) =>
        {
            try
            {
                var resp = await AuthApiClient.LoginAsync(new LoginRequestDTO
                {
                    Email = txtEmail.Text.Trim(),
                    Password = txtPassword.Text
                });

                if (resp is null)
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Login",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                User = resp;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de login", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        btnCancelar.Click += (_, __) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };
    }

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

    private void InitializeComponent() { }
}
