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
    public partial class LibraryForm : Form
    {
        List<SaveData> saves;
        public SaveData SelectedSave;

        public LibraryForm(List<SaveData> saves)
        {
            InitializeComponent();
            this.saves = saves;
        }

        private void LibraryForm_Load(object sender, EventArgs e)
        {
            int x = 5;
            int y = 5;
            int counter = 0;

            foreach (SaveData save in saves)
            {
                Panel mainPanel = new Panel();
                mainPanel.Location = new Point(x, y);
                mainPanel.Width = 160;
                mainPanel.Height = 160;

                Panel panel = new Panel();
                panel.Location = new Point(5, 5);
                panel.BackColor = Color.Red;
                panel.Width = mainPanel.Width - 5;
                panel.Height = mainPanel.Height - 30;
                panel.BackgroundImageLayout = ImageLayout.Stretch;
                panel.BackgroundImage = save.GetPicture();

                Label label = new Label();
                label.Location = new Point(mainPanel.Width / 4, panel.Height + 5);
                label.Font = new Font("Arial", 11, FontStyle.Bold);
                label.Text = save.GetTitle();

                mainPanel.Controls.Add(panel);
                mainPanel.Controls.Add(label);

                panel.Click += GroupBox_Click;
                panel.Tag = counter++;

                panel1.Controls.Add(mainPanel);

                x += mainPanel.Width + 10;

            }
        }

        private void GroupBox_Click(object sender, EventArgs e)
        {
            Control control = (Panel)sender;
            SelectedSave = saves[(int)control.Tag];
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (SelectedSave != null)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {

        }
    }
}
