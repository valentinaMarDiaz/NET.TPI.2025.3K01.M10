using System.Drawing;
using System.Windows.Forms;
using API.Clients;
using DTOs;
using System.Linq;
using System.Runtime.InteropServices; 

namespace WindowsForm;

public partial class FormHistorialPrecios : Form
{
    
    readonly TreeView tv = new() { Dock = DockStyle.Fill, BackColor = Color.White };

  
    readonly Button btnActualizar = new() { Text = "Actualizar", Height = 40, Width = 120 };
    readonly Button btnSalir = new() { Text = "Salir", Height = 40, Width = 120 };

    public FormHistorialPrecios()
    {
        InitializeComponent();
        Text = "Historial de precios";
        Width = 700; Height = 600; StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(245, 247, 250); 
       
        FlowLayoutPanel pnlBotones = new() { Dock = DockStyle.Bottom, Height = 56, Padding = new Padding(10, 10, 0, 0), BackColor = Color.FromArgb(245, 247, 250) };

        
        ConfigurarEstiloBoton(btnActualizar);
        ConfigurarEstiloBoton(btnSalir);

        
        foreach (var btn in new[] { btnActualizar, btnSalir })
        {
            btn.Margin = new Padding(0, 0, 10, 0);
        }

        pnlBotones.Controls.AddRange(new Control[] { btnActualizar, btnSalir });

        Controls.Add(tv);
        Controls.Add(pnlBotones);

        Shown += async (_, __) => await CargarProductosAsync();
        btnActualizar.Click += async (_, __) => await CargarProductosAsync();
        btnSalir.Click += (_, __) => Close();

        
        tv.BeforeExpand += async (_, e) =>
        {
            
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
          
            var nodo = new TreeNode($"[{p.IdProducto}] {p.Nombre}") { Tag = p };
            
            nodo.Nodes.Add(new TreeNode("...") { Tag = "dummy" });
            tv.Nodes.Add(nodo);
        }

        tv.EndUpdate();
    }

    private void ConfigurarEstiloBoton(Button boton)
    {
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.BackColor = Color.FromArgb(52, 152, 219);
        boton.ForeColor = Color.White;
        boton.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        boton.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, boton.Width, boton.Height, 15, 15));

   
        boton.MouseEnter += (_, __) => boton.BackColor = Color.FromArgb(41, 128, 185);
        boton.MouseLeave += (_, __) => boton.BackColor = Color.FromArgb(52, 152, 219);
    }

    [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse);
}