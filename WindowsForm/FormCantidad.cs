using System.Drawing;
using System.Windows.Forms;

namespace WindowsForm;

public partial class FormCantidad : Form
{
    private readonly NumericUpDown _num = new()
    {
        Minimum = 1,
        Maximum = 1_000_000,
        Value = 1,
        Dock = DockStyle.Top
    };

    private readonly Button _btnOk = new() { Text = "Aceptar", Dock = DockStyle.Left, Width = 120 };
    private readonly Button _btnCancel = new() { Text = "Cancelar", Dock = DockStyle.Right, Width = 120 };

    public int Cantidad => (int)_num.Value;

    public FormCantidad()
    {
        InitializeComponent(); // stub

        Text = "Cantidad";
        Width = 340;
        Height = 150;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var panelBotones = new Panel { Dock = DockStyle.Bottom, Height = 44 };
        panelBotones.Controls.Add(_btnOk);
        panelBotones.Controls.Add(_btnCancel);

        Controls.Add(panelBotones);
        Controls.Add(_num);

        _btnOk.Click += (_, __) =>
        {
            if (_num.Value < 1)
            {
                MessageBox.Show("La cantidad debe ser al menos 1.");
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        };

        _btnCancel.Click += (_, __) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };
    }

    // === Stub para no depender del Designer ===
    private void InitializeComponent() { }
}
