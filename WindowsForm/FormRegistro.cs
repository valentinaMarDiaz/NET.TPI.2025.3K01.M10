using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using API.Clients; 
using DTOs;        

namespace WindowsForm
{
    public partial class FormRegistro : Form
    {
       
        public LoginResponseDTO? User { get; private set; }

        readonly Label titulo = new()
        {
            Text = "Registro",
            Dock = DockStyle.Top,
            Height = 50,
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(45, 62, 80),
            TextAlign = ContentAlignment.MiddleCenter
        };

        
        readonly RadioButton rbCliente = new() { Text = "Cliente", Checked = true, AutoSize = true };
        readonly RadioButton rbVendedor = new() { Text = "Vendedor", AutoSize = true };

        
        readonly TextBox txtNombre = new() { Width = 260 };
        readonly TextBox txtApellido = new() { Width = 260 };
        readonly TextBox txtEmail = new() { Width = 260 };
        readonly TextBox txtPassword = new() { Width = 260, UseSystemPasswordChar = true };

        
        readonly FlowLayoutPanel pnlCliente = new()
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown 
        };
        
        readonly TextBox txtTelefono = new() { Width = 260 };
        readonly TextBox txtDireccion = new() { Width = 260 };

      
        readonly Panel pnlVendedor = new() { AutoSize = true, Dock = DockStyle.Fill };
        readonly TextBox txtCuil = new() { Width = 260 };

        readonly Button btnRegistrar = new() { Text = "Registrar", Height = 40, Width = 160 };
        readonly Button btnCancelar = new() { Text = "Cancelar", Height = 40, Width = 160 };

        public FormRegistro()
        {
            
            InitializeComponent();

            Text = "Registro"; Width = 560; Height = 520;
            StartPosition = FormStartPosition.CenterParent; BackColor = Color.FromArgb(245, 247, 250);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(24),
                BackColor = Color.White,
                AutoScroll = true
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            
            var panelRol = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            panelRol.Controls.AddRange(new Control[] { rbCliente, rbVendedor });
            root.Controls.Add(new Label { Text = "Rol", AutoSize = true, Font = new Font("Segoe UI", 10) }, 0, 0);
            root.Controls.Add(panelRol, 1, 0);

            int row = 1;
            
            void add(string label, Control ctrl)
            {
                root.Controls.Add(new Label { Text = label, AutoSize = true, Font = new Font("Segoe UI", 10) }, 0, row);
                root.Controls.Add(ctrl, 1, row);
                row++;
            }

           
            add("Nombre", txtNombre);
            add("Apellido", txtApellido);
            add("Email", txtEmail);
            add("Contraseña", txtPassword);

           
            pnlCliente.Controls.Add(MakeRow("Teléfono (Cliente)", txtTelefono));
            pnlCliente.Controls.Add(MakeRow("Dirección (Cliente)", txtDireccion));
            root.Controls.Add(new Label { Text = "", AutoSize = true }, 0, row);
            root.Controls.Add(pnlCliente, 1, row++);

            
            pnlVendedor.Controls.Add(MakeRow("CUIL (Vendedor)", txtCuil));
            root.Controls.Add(new Label { Text = "", AutoSize = true }, 0, row);
            root.Controls.Add(pnlVendedor, 1, row++);

           
            var botones = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
            botones.Controls.AddRange(new Control[] { btnRegistrar, btnCancelar });
            root.Controls.Add(new Label { Text = "", AutoSize = true }, 0, row);
            root.Controls.Add(botones, 1, row);

            Controls.Add(root);
            Controls.Add(titulo);

            EstiloBotonAzul(btnRegistrar);
            EstiloBotonAzul(btnCancelar);

            
            void TogglePorRol()
            {
                bool esCliente = rbCliente.Checked;
                pnlCliente.Visible = esCliente;
                pnlVendedor.Visible = !esCliente;
            }
            rbCliente.CheckedChanged += (_, __) => TogglePorRol();
            rbVendedor.CheckedChanged += (_, __) => TogglePorRol();
            TogglePorRol();

            
            btnRegistrar.Click += async (_, __) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text) ||
                        string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        MessageBox.Show("Completá Nombre, Apellido, Email y Contraseña."); return;
                    }
                    var email = txtEmail.Text.Trim();
                    var pass = txtPassword.Text;
                    if (rbCliente.Checked)
                    {
                        if (string.IsNullOrWhiteSpace(txtTelefono.Text) || string.IsNullOrWhiteSpace(txtDireccion.Text))
                        {
                            MessageBox.Show("Para cliente, completá Teléfono y Dirección."); return;
                        }
                        var dto = new RegisterClienteDTO
                        {
                            Nombre = txtNombre.Text.Trim(),
                            Apellido = txtApellido.Text.Trim(),
                            Email = email,
                            Password = pass,
                            Telefono = txtTelefono.Text.Trim(),
                            Direccion = txtDireccion.Text.Trim()
                        };
                        await AuthApiClient.RegisterClienteAsync(dto);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(txtCuil.Text))
                        {
                            MessageBox.Show("Para vendedor, completá CUIL."); return;
                        }
                        var dto = new RegisterVendedorDTO
                        {
                            Nombre = txtNombre.Text.Trim(),
                            Apellido = txtApellido.Text.Trim(),
                            Email = email,
                            Password = pass,
                            Cuil = txtCuil.Text.Trim()
                        };
                        await AuthApiClient.RegisterVendedorAsync(dto);
                    }

                    
                    var resp = await AuthApiClient.LoginAsync(new LoginRequestDTO { Email = email, Password = pass });
                    if (resp is null)
                    {
                        MessageBox.Show("Registro OK, pero no se pudo iniciar sesión."); return;
                    }
                    User = resp; 
                    DialogResult = DialogResult.OK; 
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error al registrar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

          
            btnCancelar.Click += (_, __) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
        }

        
        private Control MakeRow(string label, Control input)
        {
            var p = new TableLayoutPanel { ColumnCount = 2, Width = 320, AutoSize = true };
            p.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            p.Controls.Add(new Label { Text = label, AutoSize = true, Font = new Font("Segoe UI", 10) }, 0, 0);
            p.Controls.Add(input, 1, 0);
            return p;
        }
        private void EstiloBotonAzul(Button b)
        {
            b.FlatStyle = FlatStyle.Flat; b.FlatAppearance.BorderSize = 0;
            b.BackColor = Color.FromArgb(52, 152, 219); b.ForeColor = Color.White;
            b.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            b.SizeChanged += (_, __) => {
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
}