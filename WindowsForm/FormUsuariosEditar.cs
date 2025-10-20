using System.Drawing;
using System.Windows.Forms;
using API.Clients;
using DTOs;
using WindowsForm.Utils;
using System.Runtime.InteropServices;

namespace WindowsForm;

public partial class FormUsuariosEditar : Form
{
    readonly UsuarioDTO _dto;

    // Panel principal para centrar y ordenar todos los elementos
    FlowLayoutPanel pnlMain = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, Padding = new Padding(20), BackColor = Color.White };

    const int CTRL_WIDTH = 300;

    TextBox txtNombre = new() { PlaceholderText = "Nombre", Height = 25, Width = CTRL_WIDTH };
    TextBox txtApellido = new() { PlaceholderText = "Apellido", Height = 25, Width = CTRL_WIDTH };
    TextBox txtEmail = new() { PlaceholderText = "Email", Height = 25, Width = CTRL_WIDTH };

    // Cliente
    TextBox txtTel = new() { PlaceholderText = "Teléfono (solo dígitos)", Visible = false, Height = 25, Width = CTRL_WIDTH };
    TextBox txtDir = new() { PlaceholderText = "Dirección", Visible = false, Multiline = true, Height = 70, Width = CTRL_WIDTH };

    // Vendedor
    TextBox txtCuil = new() { PlaceholderText = "CUIL (solo dígitos)", Visible = false, Height = 25, Width = CTRL_WIDTH };

    Label lblTipo = new() { Height = 25, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Margin = new Padding(0, 5, 0, 5), Width = CTRL_WIDTH };

    // Botones con ANCHO FIJO y ALTURA ESTANDAR
    Button btnContinuar = new() { Text = "Continuar", Height = 40, Width = CTRL_WIDTH };
    Button btnVolver = new() { Text = "Volver", Height = 40, Width = CTRL_WIDTH };
    Button btnCancelar = new() { Text = "Cancelar", Height = 40, Width = CTRL_WIDTH };

    public FormUsuariosEditar(UsuarioDTO existente)
    {
        InitializeComponent();
        _dto = existente;

        Text = "Modificar usuario";
        Width = 420; Height = 500; StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(245, 247, 250);

        Controls.Add(pnlMain);

        // 1. Agregar los CONTROLES DE ENTRADA (los campos)
        pnlMain.Controls.AddRange(new Control[] {
            lblTipo, txtNombre, txtApellido, txtEmail, txtTel, txtDir, txtCuil
        });

        // 2. Agregar los BOTONES
        pnlMain.Controls.AddRange(new Control[] { btnContinuar, btnVolver, btnCancelar });

        // 3. Aplicar ESTILO y CENTRADO
        ConfigurarEstiloBoton(btnContinuar);
        ConfigurarEstiloBoton(btnVolver);
        ConfigurarEstiloBoton(btnCancelar);

        // Lógica de centrado (como en FormMenu)
        pnlMain.Layout += (_, __) =>
        {
            // Centra horizontalmente todos los controles
            foreach (Control ctrl in pnlMain.Controls)
            {
                ctrl.Margin = new Padding((pnlMain.ClientSize.Width - ctrl.Width) / 2, 6, 0, 6);
            }
        };

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
            if (!WindowsForm.Utils.Validators.EsEmailValido(txtEmail.Text)) { MessageBox.Show("Email inválido."); txtEmail.Focus(); return; }

            try
            {
                _dto.Nombre = txtNombre.Text.Trim();
                _dto.Apellido = txtApellido.Text.Trim();
                _dto.Email = txtEmail.Text.Trim();

                if (_dto.TipoUsuario == "Cliente")
                {
                    if (!WindowsForm.Utils.Validators.SoloDigitos(txtTel.Text)) { MessageBox.Show("Teléfono inválido."); txtTel.Focus(); return; }
                    if (string.IsNullOrWhiteSpace(txtDir.Text)) { MessageBox.Show("Dirección requerida."); txtDir.Focus(); return; }
                    _dto.Telefono = txtTel.Text.Trim();
                    _dto.Direccion = txtDir.Text.Trim();
                }
                else
                {
                    if (!WindowsForm.Utils.Validators.SoloDigitos(txtCuil.Text)) { MessageBox.Show("CUIL inválido."); txtCuil.Focus(); return; }
                    _dto.Cuil = txtCuil.Text.Trim();
                }

                await UsuarioApiClient.UpdateAsync(_dto);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
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