using System.Windows.Forms;
using DTOs;

namespace WindowsForm;

public partial class FormMenu : Form
{
    readonly LoginResponseDTO _user;
    Button btnUsuarios = new() { Text = "Lista de usuarios", Dock = DockStyle.Top, Height = 40 };
    Button btnCategorias = new() { Text = "Lista de categorías", Dock = DockStyle.Top, Height = 40 };
    Button btnProductos = new() { Text = "Productos", Dock = DockStyle.Top, Height = 40 };
    Button btnHistorial = new() { Text = "Historial de precios", Dock = DockStyle.Top, Height = 40 };
    Button btnSalir = new() { Text = "Salir", Dock = DockStyle.Top, Height = 40 };

    public FormMenu(LoginResponseDTO user)
    {
        InitializeComponent();
        _user = user;
        Text = "Menú principal";
        Width = 380; Height = 220; StartPosition = FormStartPosition.CenterParent;

        Controls.Add(btnSalir);
        Controls.Add(btnHistorial);
        Controls.Add(btnProductos);
        Controls.Add(btnCategorias);
        Controls.Add(btnUsuarios);

        btnUsuarios.Click += (_, __) => { using var f = new FormUsuarioLista(); f.ShowDialog(this); };
        btnCategorias.Click += (_, __) => { using var f = new FormCategoriaLista(); f.ShowDialog(this); };
        btnProductos.Click += (_, __) => { using var f = new FormProductosLista(); f.ShowDialog(this); };
        btnHistorial.Click += (_, __) => { using var f = new FormHistorialPrecios(); f.ShowDialog(this); };
        btnSalir.Click += (_, __) => Application.Exit();
    }
}
