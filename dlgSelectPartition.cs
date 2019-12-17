using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QCOH
{
    public partial class dlgSelectPartition : Form
    {
       public string partionName { get; set; }
        public dlgSelectPartition()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            button1.DialogResult = DialogResult.OK;
        }

        private void dlgSelectPartition_Load(object sender, EventArgs e)
        {
            this.cmbPartionName.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
