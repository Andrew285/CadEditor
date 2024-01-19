using CadEditor.Controllers;
using CadEditor.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadEditor.View.Forms
{
    public partial class SettingsForm : Form
    {

        public SettingsForm()
        {
            InitializeComponent();

            //SceneTab
            checkBoxAxisCubeDrawFacets.Checked = SceneSettings.AxisCubeDrawFacets;
            checkBoxAxisCubeDrawEdges.Checked = SceneSettings.AxisCubeDrawEdges;
            checkBoxAxisCubeDrawVertices.Checked = SceneSettings.AxisCubeDrawVertices;

            //ThemeTab
            panel1.BackColor = ThemeSettings.MainThemeColor;
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SettingsController.GetInstance().SaveData(MainSettings.FilePath);
        }


        //SceneTab
        private void checkBoxAxisCubeDrawVertices_CheckedChanged(object sender, EventArgs e)
        {
            SceneSettings.AxisCubeDrawVertices = checkBoxAxisCubeDrawVertices.Checked;
        }

        private void checkBoxAxisCubeDrawEdges_CheckedChanged(object sender, EventArgs e)
        {
            SceneSettings.AxisCubeDrawEdges = checkBoxAxisCubeDrawEdges.Checked;

        }

        private void checkBoxAxisCubeDrawFacets_CheckedChanged(object sender, EventArgs e)
        {
            SceneSettings.AxisCubeDrawFacets = checkBoxAxisCubeDrawFacets.Checked;
        }


        //ThemeTab
        private void panel1_Click(object sender, EventArgs e)
        {
            colorDialog1.FullOpen = true;

            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            ThemeSettings.MainThemeColor = colorDialog1.Color;
            Form1.GetInstance().UpdateFormColor(ThemeSettings.MainThemeColor);
            panel1.BackColor = ThemeSettings.MainThemeColor;
        }
    }
}
