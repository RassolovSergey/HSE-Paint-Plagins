using System;
using System.Windows.Forms;

namespace WinForms_v1
{
    public partial class TextInputForm : Form
    {
        public string InputText { get; private set; }

        public TextInputForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            InputText = txtInput.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
