using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm;

public partial class FormLogin : Form
{
    TextBox txtEmail = new() { PlaceholderText = "Email", Dock = DockStyle.Top };
    TextBox txtPass = new() { PlaceholderText = "Contraseña", UseSystemPasswordChar = true, Dock = DockStyle.Top };
    Button btnContinuar = new() { Text = "Continuar", Dock = DockStyle.Top, Height = 40 };
    Button btnVolver = new() { Text = "Volver", Dock = DockStyle.Top, Height = 40 };
    Button btnCancelar = new() { Text = "Cancelar", Dock = DockStyle.Top, Height = 40 };

    public FormLogin()
    {
        InitializeComponent();
        Text = "Login";
        Width = 360; Height = 260; StartPosition = FormStartPosition.CenterParent;
        Controls.Add(btnCancelar);
        Controls.Add(btnVolver);
        Controls.Add(btnContinuar);
        Controls.Add(txtPass);
        Controls.Add(txtEmail);

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
}
