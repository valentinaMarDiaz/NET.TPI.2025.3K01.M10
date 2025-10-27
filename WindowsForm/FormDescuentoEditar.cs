using System.Drawing;
using System.Windows.Forms;
using API.Clients;
using DTOs;

namespace WindowsForm
{
    public partial class FormDescuentoEditar : Form
    {
        readonly DescuentoCUDTO dto;

        readonly ComboBox cmbProducto = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };
        readonly DateTimePicker dpInicio = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm 'UTC'" };
        readonly DateTimePicker dpCaduca = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm 'UTC'" };
        readonly TextBox txtDescripcion = new() { Width = 300 };
        readonly TextBox txtCodigo = new() { Width = 200, CharacterCasing = CharacterCasing.Upper };
        readonly NumericUpDown numPorcentaje = new() { Minimum = 1, Maximum = 100, DecimalPlaces = 2, Increment = 0.5M, Width = 100 };

        readonly Button btnGuardar = new() { Text = "Guardar", Height = 36, Width = 140 };
        readonly Button btnCancelar = new() { Text = "Cancelar", Height = 36, Width = 140 };

        public FormDescuentoEditar(DescuentoDTO? original = null)
        {
            Text = original == null ? "Nuevo descuento" : $"Editar {original.Codigo}";
            Width = 640; Height = 420; StartPosition = FormStartPosition.CenterParent;

            dto = new DescuentoCUDTO
            {
                IdDescuento = original?.IdDescuento ?? 0,
                IdProducto = original?.IdProducto ?? 0,
                FechaInicioUtc = original?.FechaInicioUtc ?? DateTime.UtcNow,
                FechaCaducidadUtc = original?.FechaCaducidadUtc ?? DateTime.UtcNow.AddDays(7),
                Descripcion = original?.Descripcion ?? "",
                Codigo = original?.Codigo ?? "",
                Porcentaje = original?.Porcentaje ?? 10
            };

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(20) };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            int row = 0;
            void add(string label, Control c)
            {
                root.Controls.Add(new Label { Text = label, AutoSize = true, Font = new Font("Segoe UI", 10) }, 0, row);
                root.Controls.Add(c, 1, row);
                row++;
            }

            add("Producto", cmbProducto);
            add("Fecha inicio (UTC)", dpInicio);
            add("Fecha caducidad (UTC)", dpCaduca);
            add("Descripción", txtDescripcion);
            add("Código", txtCodigo);
            add("Porcentaje (%)", numPorcentaje);

            var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
            buttons.Controls.AddRange(new Control[] { btnGuardar, btnCancelar });
            root.Controls.Add(new Label { Text = "" }, 0, row);
            root.Controls.Add(buttons, 1, row);

            Controls.Add(root);

            dpInicio.Value = DateTime.SpecifyKind(dto.FechaInicioUtc, DateTimeKind.Utc).ToLocalTime();
            dpCaduca.Value = DateTime.SpecifyKind(dto.FechaCaducidadUtc, DateTimeKind.Utc).ToLocalTime();
            txtDescripcion.Text = dto.Descripcion;
            txtCodigo.Text = dto.Codigo;
            numPorcentaje.Value = dto.Porcentaje;

            Shown += async (_, __) =>
            {
                var productos = await ProductoApiClient.GetAllAsync(); 
                cmbProducto.DisplayMember = "Nombre";
                cmbProducto.ValueMember = "IdProducto";
                cmbProducto.DataSource = productos.ToList();
                if (dto.IdProducto != 0) cmbProducto.SelectedValue = dto.IdProducto;
            };

            btnGuardar.Click += async (_, __) =>
            {
                try
                {
                    dto.IdProducto = (int)(cmbProducto.SelectedValue ?? 0);
                    dto.FechaInicioUtc = dpInicio.Value.ToUniversalTime();
                    dto.FechaCaducidadUtc = dpCaduca.Value.ToUniversalTime();
                    dto.Descripcion = txtDescripcion.Text.Trim();
                    dto.Codigo = txtCodigo.Text.Trim().ToUpperInvariant();
                    dto.Porcentaje = decimal.Round(numPorcentaje.Value, 2, MidpointRounding.AwayFromZero);

                    if (dto.IdDescuento == 0) await DescuentoApiClient.AddAsync(dto);
                    else await DescuentoApiClient.UpdateAsync(dto);

                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnCancelar.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
        }
    }
}
