using System.Windows.Forms;

namespace WindowsForm;

public partial class FormInicio : Form
{
    Button btnLogin = new() { Text = "Iniciar sesión", Dock = DockStyle.Top, Height = 40 };
    Button btnRegistro = new() { Text = "Registrarse", Dock = DockStyle.Top, Height = 40 };
    Button btnCancelar = new() { Text = "Cancelar", Dock = DockStyle.Top, Height = 40 };

    public FormInicio()
    {
        InitializeComponent();
        Text = "SmartTienda - Inicio";
        Width = 360; Height = 220; StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(btnCancelar);
        Controls.Add(btnRegistro);
        Controls.Add(btnLogin);

        btnLogin.Click += (_, __) => { using var f = new FormLogin(); f.ShowDialog(this); };
        btnRegistro.Click += (_, __) => { using var f = new FormRegistro(); f.ShowDialog(this); };
        btnCancelar.Click += (_, __) => Close();
    }
}
