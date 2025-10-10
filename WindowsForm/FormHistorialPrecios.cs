using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm;

public partial class FormHistorialPrecios : Form
{
    // Lista con expansión por producto
    readonly TreeView tv = new() { Dock = DockStyle.Fill };
    readonly Button btnActualizar = new() { Text = "Actualizar", Dock = DockStyle.Bottom, Height = 36 };
    readonly Button btnSalir = new() { Text = "Salir", Dock = DockStyle.Bottom, Height = 36 };

    public FormHistorialPrecios()
    {
        InitializeComponent();
        Text = "Historial de precios";
        Width = 700; Height = 600; StartPosition = FormStartPosition.CenterParent;

        Controls.Add(tv);
        Controls.Add(btnSalir);
        Controls.Add(btnActualizar);

        Shown += async (_, __) => await CargarProductosAsync();
        btnActualizar.Click += async (_, __) => await CargarProductosAsync();
        btnSalir.Click += (_, __) => Close();

        // Carga perezosa del historial al expandir el producto
        tv.BeforeExpand += async (_, e) =>
        {
            // Si el nodo representa un producto y aún tiene el "dummy", lo cargamos
            if (e.Node.Tag is ProductoDTO p &&
                e.Node.Nodes.Count == 1 &&
                e.Node.Nodes[0].Tag as string == "dummy")
            {
                e.Node.Nodes.Clear();
                var historial = (await ProductoApiClient.GetHistorialAsync(p.IdProducto))
                                .OrderByDescending(h => h.FechaModificacionUtc)
                                .ToList();

                if (historial.Count == 0)
                {
                    e.Node.Nodes.Add(new TreeNode("Sin movimientos"));
                    return;
                }

                foreach (var h in historial)
                {
                    var fechaLocal = DateTime.SpecifyKind(h.FechaModificacionUtc, DateTimeKind.Utc).ToLocalTime();
                    var nodo = new TreeNode($"{fechaLocal:dd/MM/yyyy HH:mm}  -  {h.Valor:N2}") { Tag = h };
                    e.Node.Nodes.Add(nodo);
                }
            }
        };
    }

    private async Task CargarProductosAsync()
    {
        var productos = (await ProductoApiClient.GetAllAsync()).ToList();

        tv.BeginUpdate();
        tv.Nodes.Clear();

        foreach (var p in productos)
        {
            // Nodo principal: muestra Id y Nombre
            var nodo = new TreeNode($"[{p.IdProducto}] {p.Nombre}") { Tag = p };
            // Agrego hijo “dummy” para que aparezca el triángulo de expandir y se cargue on-demand
            nodo.Nodes.Add(new TreeNode("...") { Tag = "dummy" });
            tv.Nodes.Add(nodo);
        }

        tv.EndUpdate();
    }
}
