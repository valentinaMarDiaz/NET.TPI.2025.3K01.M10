using System.Windows.Forms;
using API.Clients;
using DTOs;
using WindowsForm.Utils;

namespace WindowsForm;

public partial class FormRegistro : Form
{
    ComboBox cmbTipo = new() { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
    TextBox txtNombre = new() { PlaceholderText = "Nombre", Dock = DockStyle.Top };
    TextBox txtApellido = new() { PlaceholderText = "Apellido", Dock = DockStyle.Top };
    TextBox txtEmail = new() { PlaceholderText = "Email", Dock = DockStyle.Top };
    TextBox txtPass = new() { PlaceholderText = "Contraseña", UseSystemPasswordChar = true, Dock = DockStyle.Top };

    // Cliente
    TextBox txtTel = new() { PlaceholderText = "Teléfono (solo dígitos)", Dock = DockStyle.Top, Visible = false };
    TextBox txtDir = new() { PlaceholderText = "Dirección", Dock = DockStyle.Top, Visible = false };

    // Vendedor
    TextBox txtCuil = new() { PlaceholderText = "CUIL (solo dígitos)", Dock = DockStyle.Top, Visible = false };

    Button btnContinuar = new() { Text = "Continuar", Dock = DockStyle.Top, Height = 40 };
    Button btnVolver = new() { Text = "Volver", Dock = DockStyle.Top, Height = 40 };
    Button btnCancelar = new() { Text = "Cancelar", Dock = DockStyle.Top, Height = 40 };

    public FormRegistro()
    {
        InitializeComponent();
        Text = "Registro";
        Width = 420; Height = 450; StartPosition = FormStartPosition.CenterParent;
        cmbTipo.Items.AddRange(new[] { "Cliente", "Vendedor" });

        Controls.Add(btnCancelar);
        Controls.Add(btnVolver);
        Controls.Add(btnContinuar);
        Controls.Add(txtCuil);
        Controls.Add(txtDir);
        Controls.Add(txtTel);
        Controls.Add(txtPass);
        Controls.Add(txtEmail);
        Controls.Add(txtApellido);
        Controls.Add(txtNombre);
        Controls.Add(cmbTipo);

        cmbTipo.SelectedIndexChanged += (_, __) =>
        {
            var t = cmbTipo.SelectedItem?.ToString();
            bool esCliente = t == "Cliente";
            txtTel.Visible = txtDir.Visible = esCliente;
            txtCuil.Visible = !esCliente;
        };
        cmbTipo.SelectedIndex = 0;

        btnVolver.Click += (_, __) => Close();
        btnCancelar.Click += (_, __) => Application.Exit();

        btnContinuar.Click += async (_, __) =>
        {
            var tipo = cmbTipo.SelectedItem?.ToString();
            if (!Validators.TipoUsuarioValido(tipo)) { MessageBox.Show("Tipo inválido."); return; }
            if (!Validators.EsEmailValido(txtEmail.Text)) { MessageBox.Show("Email inválido."); txtEmail.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtPass.Text) || txtPass.Text.Length < 4) { MessageBox.Show("Contraseña muy corta."); txtPass.Focus(); return; }

            try
            {
                if (tipo == "Cliente")
                {
                    if (!Validators.SoloDigitos(txtTel.Text)) { MessageBox.Show("Teléfono inválido."); txtTel.Focus(); return; }
                    if (string.IsNullOrWhiteSpace(txtDir.Text)) { MessageBox.Show("Dirección requerida."); txtDir.Focus(); return; }

                    var dto = new RegisterClienteDTO
                    {
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Password = txtPass.Text,
                        Telefono = txtTel.Text.Trim(),
                        Direccion = txtDir.Text.Trim()
                    };
                    await AuthApiClient.RegisterClienteAsync(dto);
                    MessageBox.Show("Cliente registrado.");
                }
                else
                {
                    if (!Validators.SoloDigitos(txtCuil.Text)) { MessageBox.Show("CUIL inválido."); txtCuil.Focus(); return; }

                    var dto = new RegisterVendedorDTO
                    {
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Password = txtPass.Text,
                        Cuil = txtCuil.Text.Trim()
                    };
                    var resp = await AuthApiClient.RegisterVendedorAsync(dto);
                    MessageBox.Show($"Vendedor registrado. Legajo {resp.Legajo}.");
                }
                Close();
            }
            catch (Exception ex) { MessageBox.Show($"Error al registrar: {ex.Message}"); }
        };
    }
}
