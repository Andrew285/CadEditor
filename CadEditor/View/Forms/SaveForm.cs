using System;
using System.Drawing;
using System.Windows.Forms;

namespace CadEditor
{
    public partial class SaveForm : Form
    {
        public SaveData SaveData { get; set; }

        public SaveForm(Bitmap bmp)
        {
            InitializeComponent();
            panel1.BackgroundImage = bmp;
            this.TopMost = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;

            if (name != "")
            {
                SaveData = new SaveData(new Bitmap(panel1.BackgroundImage), "", name, DateTime.Now);
                DialogResult = DialogResult.OK;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
