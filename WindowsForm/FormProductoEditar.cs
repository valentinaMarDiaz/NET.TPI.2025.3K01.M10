using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using API.Clients;
using DTOs;
using System.Linq;
using System.Runtime.InteropServices;

namespace WindowsForm;

public partial class FormProductoEditar : Form
{
    readonly bool _edit;
    readonly ProductoDTO _dto;

    // Panel principal para centrar y ordenar todos los elementos
    FlowLayoutPanel pnlMain = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, Padding = new Padding(20), BackColor = Color.White };

    const int CTRL_WIDTH = 300;

    // Controles de entrada
    ComboBox cmbCategoria = new() { Text = "Seleccione Categoría", DropDownStyle = ComboBoxStyle.DropDownList, Height = 25, Width = CTRL_WIDTH };
    TextBox txtNombre = new() { PlaceholderText = "Nombre", Height = 25, Width = CTRL_WIDTH };
    TextBox txtDescripcion = new() { PlaceholderText = "Descripción", Multiline = true, Height = 70, Width = CTRL_WIDTH };
    TextBox txtStock = new() { PlaceholderText = "Stock (entero >=0)", Height = 25, Width = CTRL_WIDTH };
    TextBox txtPrecio = new() { PlaceholderText = "Precio actual (decimal >=0)", Height = 25, Width = CTRL_WIDTH };

    // Botones con ANCHO FIJO y ALTURA ESTANDAR
    Button btnContinuar = new() { Text = "Continuar", Height = 40, Width = CTRL_WIDTH };
    Button btnVolver = new() { Text = "Volver", Height = 40, Width = CTRL_WIDTH };
    Button btnCancelar = new() { Text = "Cancelar", Height = 40, Width = CTRL_WIDTH };

    record Item(int Id, string Nombre);

    public FormProductoEditar(ProductoDTO? existente = null)
    {
        InitializeComponent();
        _edit = existente != null;
        _dto = existente ?? new ProductoDTO();

        Text = _edit ? "Modificar producto" : "Agregar producto";
        Width = 520; Height = 560; StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(245, 247, 250);

        Controls.Add(pnlMain);

        // 1. Agregar los CONTROLES DE ENTRADA (los campos)
        pnlMain.Controls.AddRange(new Control[] {
            cmbCategoria, txtNombre, txtDescripcion, txtStock, txtPrecio
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

        Shown += async (_, __) =>
        {
            var cats = (await CategoriaApiClient.GetAllAsync()).Select(c => new Item(c.IdCatProducto, c.Nombre)).ToList();
            cmbCategoria.DataSource = cats;
            cmbCategoria.DisplayMember = nameof(Item.Nombre);
            cmbCategoria.ValueMember = nameof(Item.Id);

            if (_edit)
            {
                txtNombre.Text = _dto.Nombre;
                txtDescripcion.Text = _dto.Descripcion;
                txtStock.Text = _dto.Stock.ToString();
                txtPrecio.Text = _dto.PrecioActual.ToString(CultureInfo.InvariantCulture);
                var idx = cats.FindIndex(x => x.Id == _dto.IdCatProducto);
                if (idx >= 0) cmbCategoria.SelectedIndex = idx;
            }
            else
            {
                cmbCategoria.SelectedIndex = -1;
            }
        };

        btnVolver.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
        btnCancelar.Click += (_, __) => Application.Exit();

        btnContinuar.Click += async (_, __) =>
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Nombre requerido"); return; }
            if (cmbCategoria.SelectedItem is not Item sel) { MessageBox.Show("Seleccione categoría"); return; }
            if (!int.TryParse(txtStock.Text, out var stock) || stock < 0) { MessageBox.Show("Stock inválido"); return; }
            if (!decimal.TryParse(txtPrecio.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var precio) || precio < 0)
            { MessageBox.Show("Precio inválido (use punto para decimales)"); return; }

            try
            {
                _dto.Nombre = txtNombre.Text.Trim();
                _dto.Descripcion = txtDescripcion.Text.Trim();
                _dto.Stock = stock;
                _dto.PrecioActual = precio;
                _dto.IdCatProducto = sel.Id;

                if (_edit) await ProductoApiClient.UpdateAsync(_dto);
                else _ = await ProductoApiClient.AddAsync(_dto);

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