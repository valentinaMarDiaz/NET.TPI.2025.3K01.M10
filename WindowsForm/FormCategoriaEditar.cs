using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm;

public partial class FormCategoriaEditar : Form
{
    readonly bool _edit;
    readonly CategoriaDTO _dto;

    TextBox txtNombre = new() { PlaceholderText = "Nombre", Dock = DockStyle.Top };
    TextBox txtDesc = new() { PlaceholderText = "Descripción", Dock = DockStyle.Top, Multiline = true, Height = 80 };
    Button btnContinuar = new() { Text = "Continuar", Dock = DockStyle.Top, Height = 40 };
    Button btnVolver = new() { Text = "Volver", Dock = DockStyle.Top, Height = 40 };
    Button btnCancelar = new() { Text = "Cancelar", Dock = DockStyle.Top, Height = 40 };

    public FormCategoriaEditar(CategoriaDTO? existente = null)
    {
        InitializeComponent(); // <-- ahora existe (lo crea el Designer.cs)
        _edit = existente != null;
        _dto = existente ?? new CategoriaDTO();

        Text = _edit ? "Modificar categoría" : "Agregar categoría";
        Width = 420; Height = 320; StartPosition = FormStartPosition.CenterParent;

        Controls.Add(btnCancelar);
        Controls.Add(btnVolver);
        Controls.Add(btnContinuar);
        Controls.Add(txtDesc);
        Controls.Add(txtNombre);

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
}
