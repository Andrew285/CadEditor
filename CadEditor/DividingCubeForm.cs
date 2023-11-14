using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadEditor
{
    public partial class DividingCubeForm : Form
    {
        public Vector nValues;

        public DividingCubeForm()
        {
            InitializeComponent();
            textBox_Nx.Text = "4";
            textBox_Ny.Text = "4";
            textBox_Nz.Text = "4";
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            string NxString = textBox_Nx.Text;
            string NyString = textBox_Ny.Text;
            string NzString = textBox_Nz.Text;

            if (NxString != "" && NyString != "" && NzString != "")
            {
                int Nx = Convert.ToInt32(NxString);
                int Ny = Convert.ToInt32(NyString);
                int Nz = Convert.ToInt32(NzString);

                nValues = new Vector(Nx, Ny, Nz);
                DialogResult = DialogResult.OK;
            }
            else
            {
                throw new Exception("To divide cube, all input fields should be not empty");
            }
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
