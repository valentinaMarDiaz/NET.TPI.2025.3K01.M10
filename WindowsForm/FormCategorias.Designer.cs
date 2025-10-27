using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace WindowsForm; 

partial class FormCategorias
{
    private System.ComponentModel.IContainer components = null;
    private TextBox txtNombre;
    private Button btnAgregar;
    private DataGridView grid;
    private BindingSource bs;

  
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

  
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        txtNombre = new TextBox();
        btnAgregar = new Button();
        grid = new DataGridView();
        bs = new BindingSource(components);
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bs).BeginInit();
        SuspendLayout();
        // 
        // txtNombre
        // 
        txtNombre.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtNombre.Location = new System.Drawing.Point(12, 12);
        txtNombre.Name = "txtNombre";
        txtNombre.PlaceholderText = "Nombre";
        txtNombre.Size = new System.Drawing.Size(476, 23);
        // 
        // btnAgregar
        // 
        btnAgregar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAgregar.Location = new System.Drawing.Point(494, 12);
        btnAgregar.Name = "btnAgregar";
        btnAgregar.Size = new System.Drawing.Size(94, 40);
        btnAgregar.Text = "Agregar";
        btnAgregar.UseVisualStyleBackColor = true;
        // 
        // grid
        // 
        grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.Location = new System.Drawing.Point(12, 60); 
        grid.Name = "grid";
        grid.ReadOnly = true;
        grid.RowTemplate.Height = 25;
        grid.Size = new System.Drawing.Size(576, 378); 
        grid.DataSource = bs;
        // 
        // FormCategorias
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(600, 450);
        Controls.Add(grid);
        Controls.Add(btnAgregar);
        Controls.Add(txtNombre);
        Name = "FormCategorias";
        Text = "Categorías";
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        ((System.ComponentModel.ISupportInitialize)bs).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}