
using System.Drawing;
using System.Windows.Forms;

namespace WindowsForm
{
    public partial class FormCarrito : Form
    {
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(900, 520);
            this.Name = "FormCarrito";
            this.Text = "Carrito";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
        }
    }
}
