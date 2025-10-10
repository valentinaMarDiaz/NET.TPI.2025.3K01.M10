using System.Globalization;
using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm;

public partial class FormProductoEditar : Form
{
    readonly bool _edit;
    readonly ProductoDTO _dto;

    ComboBox cmbCategoria = new() { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
    TextBox txtNombre = new() { PlaceholderText = "Nombre", Dock = DockStyle.Top };
    TextBox txtDescripcion = new() { PlaceholderText = "Descripción", Dock = DockStyle.Top, Multiline = true, Height = 70 };
    TextBox txtStock = new() { PlaceholderText = "Stock (entero >=0)", Dock = DockStyle.Top };
    TextBox txtPrecio = new() { PlaceholderText = "Precio actual (decimal >=0)", Dock = DockStyle.Top };

    Button btnContinuar = new() { Text = "Continuar", Dock = DockStyle.Top, Height = 40 };
    Button btnVolver = new() { Text = "Volver", Dock = DockStyle.Top, Height = 40 };
    Button btnCancelar = new() { Text = "Cancelar", Dock = DockStyle.Top, Height = 40 };

    record Item(int Id, string Nombre);

    public FormProductoEditar(ProductoDTO? existente = null)
    {
        InitializeComponent();
        _edit = existente != null;
        _dto = existente ?? new ProductoDTO();

        Text = _edit ? "Modificar producto" : "Agregar producto";
        Width = 520; Height = 560; StartPosition = FormStartPosition.CenterParent;

        Controls.Add(btnCancelar);
        Controls.Add(btnVolver);
        Controls.Add(btnContinuar);
        Controls.Add(txtPrecio);
        Controls.Add(txtStock);
        Controls.Add(txtDescripcion);
        Controls.Add(txtNombre);
        Controls.Add(cmbCategoria);

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
}
