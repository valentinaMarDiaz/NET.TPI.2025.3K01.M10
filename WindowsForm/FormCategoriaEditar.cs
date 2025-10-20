using System.Drawing;
using System.Windows.Forms;
using API.Clients;
using DTOs;
using System.Runtime.InteropServices;

namespace WindowsForm;

public partial class FormCategoriaEditar : Form
{
    readonly bool _edit;
    readonly CategoriaDTO _dto;

    // Panel principal para centrar y ordenar todos los elementos
    FlowLayoutPanel pnlMain = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, Padding = new Padding(20), BackColor = Color.White };

    const int CTRL_WIDTH = 300;

    // Controles de entrada
    TextBox txtNombre = new() { PlaceholderText = "Nombre", Height = 25, Width = CTRL_WIDTH };
    TextBox txtDesc = new() { PlaceholderText = "Descripción", Multiline = true, Height = 80, Width = CTRL_WIDTH };

    // Botones con ANCHO FIJO y ALTURA ESTANDAR
    Button btnContinuar = new() { Text = "Continuar", Height = 40, Width = CTRL_WIDTH };
    Button btnVolver = new() { Text = "Volver", Height = 40, Width = CTRL_WIDTH };
    Button btnCancelar = new() { Text = "Cancelar", Height = 40, Width = CTRL_WIDTH };

    public FormCategoriaEditar(CategoriaDTO? existente = null)
    {
        InitializeComponent();
        _edit = existente != null;
        _dto = existente ?? new CategoriaDTO();

        Text = _edit ? "Modificar categoría" : "Agregar categoría";
        Width = 420; Height = 420; StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(245, 247, 250);

        Controls.Add(pnlMain);

        // 1. Agregar los CONTROLES DE ENTRADA (los campos)
        pnlMain.Controls.AddRange(new Control[] { txtNombre, txtDesc });

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

        if (_edit)
        {
            txtNombre.Text = _dto.Nombre;
            txtDesc.Text = _dto.Descripcion;
        }

        btnVolver.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
        btnCancelar.Click += (_, __) => Application.Exit();
        btnContinuar.Click += async (_, __) =>
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Nombre requerido."); return; }
            try
            {
                _dto.Nombre = txtNombre.Text.Trim();
                _dto.Descripcion = txtDesc.Text.Trim();

                if (_edit) await CategoriaApiClient.UpdateAsync(_dto);
                else await CategoriaApiClient.AddAsync(_dto);

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