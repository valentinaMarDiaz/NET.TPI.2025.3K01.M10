using System.Drawing;
using System.Windows.Forms;
using API.Clients;
using DTOs;
using WindowsForm.Utils;
using System.Runtime.InteropServices;

namespace WindowsForm;

public partial class FormRegistro : Form
{
    // Panel principal para centrar y ordenar todos los elementos
    FlowLayoutPanel pnlMain = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, Padding = new Padding(20), BackColor = Color.White };

    // Ancho estandarizado para todos los controles
    const int CTRL_WIDTH = 300;

    // Controles de entrada (todos con ancho fijo)
    ComboBox cmbTipo = new() { Text = "Seleccione tipo de usuario", DropDownStyle = ComboBoxStyle.DropDownList, Height = 25, Width = CTRL_WIDTH };
    TextBox txtNombre = new() { PlaceholderText = "Nombre", Height = 25, Width = CTRL_WIDTH };
    TextBox txtApellido = new() { PlaceholderText = "Apellido", Height = 25, Width = CTRL_WIDTH };
    TextBox txtEmail = new() { PlaceholderText = "Email", Height = 25, Width = CTRL_WIDTH };
    TextBox txtPass = new() { PlaceholderText = "Contraseña", UseSystemPasswordChar = true, Height = 25, Width = CTRL_WIDTH };

    // Cliente
    TextBox txtTel = new() { PlaceholderText = "Teléfono (solo dígitos)", Visible = false, Height = 25, Width = CTRL_WIDTH };
    TextBox txtDir = new() { PlaceholderText = "Dirección", Visible = false, Multiline = true, Height = 70, Width = CTRL_WIDTH };

    // Vendedor
    TextBox txtCuil = new() { PlaceholderText = "CUIL (solo dígitos)", Visible = false, Height = 25, Width = CTRL_WIDTH };

    // Botones con ANCHO FIJO y ALTURA ESTANDAR
    Button btnContinuar = new() { Text = "Continuar", Height = 40, Width = CTRL_WIDTH };
    Button btnVolver = new() { Text = "Volver", Height = 40, Width = CTRL_WIDTH };
    Button btnCancelar = new() { Text = "Cancelar", Height = 40, Width = CTRL_WIDTH };

    public FormRegistro()
    {
        InitializeComponent();
        Text = "Registro";
        Width = 420; Height = 600; StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(245, 247, 250);

        Controls.Add(pnlMain);

        // 1. Agregar los CONTROLES DE ENTRADA (los campos)
        pnlMain.Controls.AddRange(new Control[] {
            cmbTipo, txtNombre, txtApellido, txtEmail, txtPass, txtTel, txtDir, txtCuil
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

        // Configuración inicial del ComboBox 
        cmbTipo.Items.AddRange(new[] { "Cliente", "Vendedor" });
        cmbTipo.SelectedIndex = 0;

        cmbTipo.SelectedIndexChanged += (_, __) =>
        {
            var t = cmbTipo.SelectedItem?.ToString();
            bool esCliente = t == "Cliente";
            txtTel.Visible = txtDir.Visible = esCliente;
            txtCuil.Visible = !esCliente;
        };

        btnVolver.Click += (_, __) => Close();
        btnCancelar.Click += (_, __) => Application.Exit();

        btnContinuar.Click += async (_, __) =>
        {
            var tipo = cmbTipo.SelectedItem?.ToString();
            if (!WindowsForm.Utils.Validators.TipoUsuarioValido(tipo)) { MessageBox.Show("Tipo inválido."); return; }
            if (!WindowsForm.Utils.Validators.EsEmailValido(txtEmail.Text)) { MessageBox.Show("Email inválido."); txtEmail.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtPass.Text) || txtPass.Text.Length < 4) { MessageBox.Show("Contraseña muy corta."); txtPass.Focus(); return; }

            try
            {
                if (tipo == "Cliente")
                {
                    if (!WindowsForm.Utils.Validators.SoloDigitos(txtTel.Text)) { MessageBox.Show("Teléfono inválido."); txtTel.Focus(); return; }
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
                    if (!WindowsForm.Utils.Validators.SoloDigitos(txtCuil.Text)) { MessageBox.Show("CUIL inválido."); txtCuil.Focus(); return; }

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
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { MessageBox.Show($"Error al registrar: {ex.Message}"); }
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