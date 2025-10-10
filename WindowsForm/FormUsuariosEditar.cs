using System.Windows.Forms;
using API.Clients;
using DTOs;
using WindowsForm.Utils;

namespace WindowsForm;

public partial class FormUsuariosEditar : Form
{
    readonly UsuarioDTO _dto;

    TextBox txtNombre = new() { PlaceholderText = "Nombre", Dock = DockStyle.Top };
    TextBox txtApellido = new() { PlaceholderText = "Apellido", Dock = DockStyle.Top };
    TextBox txtEmail = new() { PlaceholderText = "Email", Dock = DockStyle.Top };

    // Cliente
    TextBox txtTel = new() { PlaceholderText = "Teléfono (solo dígitos)", Dock = DockStyle.Top, Visible = false };
    TextBox txtDir = new() { PlaceholderText = "Dirección", Dock = DockStyle.Top, Visible = false };

    // Vendedor
    TextBox txtCuil = new() { PlaceholderText = "CUIL (solo dígitos)", Dock = DockStyle.Top, Visible = false };

    Label lblTipo = new() { Dock = DockStyle.Top, Height = 20, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };

    Button btnContinuar = new() { Text = "Continuar", Dock = DockStyle.Top, Height = 40 };
    Button btnVolver = new() { Text = "Volver", Dock = DockStyle.Top, Height = 40 };
    Button btnCancelar = new() { Text = "Cancelar", Dock = DockStyle.Top, Height = 40 };

    public FormUsuariosEditar(UsuarioDTO existente)
    {
        InitializeComponent();
        _dto = existente;

        Text = "Modificar usuario";
        Width = 420; Height = 420; StartPosition = FormStartPosition.CenterParent;

        Controls.Add(btnCancelar);
        Controls.Add(btnVolver);
        Controls.Add(btnContinuar);
        Controls.Add(txtCuil);
        Controls.Add(txtDir);
        Controls.Add(txtTel);
        Controls.Add(txtEmail);
        Controls.Add(txtApellido);
        Controls.Add(txtNombre);
        Controls.Add(lblTipo);

        lblTipo.Text = $"Tipo: {_dto.TipoUsuario}";
        txtNombre.Text = _dto.Nombre;
        txtApellido.Text = _dto.Apellido;
        txtEmail.Text = _dto.Email;

        if (_dto.TipoUsuario == "Cliente")
        {
            txtTel.Visible = txtDir.Visible = true;
            txtTel.Text = _dto.Telefono ?? "";
            txtDir.Text = _dto.Direccion ?? "";
        }
        else
        {
            txtCuil.Visible = true;
            txtCuil.Text = _dto.Cuil ?? "";
        }

        btnVolver.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
        btnCancelar.Click += (_, __) => Application.Exit();

        btnContinuar.Click += async (_, __) =>
        {
            if (!Validators.EsEmailValido(txtEmail.Text)) { MessageBox.Show("Email inválido."); txtEmail.Focus(); return; }

            try
            {
                _dto.Nombre = txtNombre.Text.Trim();
                _dto.Apellido = txtApellido.Text.Trim();
                _dto.Email = txtEmail.Text.Trim();

                if (_dto.TipoUsuario == "Cliente")
                {
                    if (!Validators.SoloDigitos(txtTel.Text)) { MessageBox.Show("Teléfono inválido."); txtTel.Focus(); return; }
                    if (string.IsNullOrWhiteSpace(txtDir.Text)) { MessageBox.Show("Dirección requerida."); txtDir.Focus(); return; }
                    _dto.Telefono = txtTel.Text.Trim();
                    _dto.Direccion = txtDir.Text.Trim();
                }
                else
                {
                    if (!Validators.SoloDigitos(txtCuil.Text)) { MessageBox.Show("CUIL inválido."); txtCuil.Focus(); return; }
                    _dto.Cuil = txtCuil.Text.Trim();
                }

                await UsuarioApiClient.UpdateAsync(_dto);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        };
    }
}
