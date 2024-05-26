using CadEditor.Controllers;
using CadEditor.Settings;
using System;
using System.Windows.Forms;

namespace CadEditor.View.Forms
{
    public partial class SettingsForm : Form
    {
        private ApplicationController _applicationController;

        public SettingsForm(ApplicationController appController)
        {
            InitializeComponent();
            _applicationController = appController;

            //SceneTab
            checkBoxAxisCubeDrawFacets.Checked = SceneSettings.AxisCubeDrawFacets;
            checkBoxAxisCubeDrawEdges.Checked = SceneSettings.AxisCubeDrawEdges;
            checkBoxAxisCubeDrawVertices.Checked = SceneSettings.AxisCubeDrawVertices;

            //ThemeTab
            panel1.BackColor = ThemeSettings.MainThemeColor;
            panelMenuBackColor.BackColor = ThemeSettings.MenuStripBackColor;

            //Set Events
            panel1.Click += Panel1_Click;
            panelMenuBackColor.Click += panelMenuBackColor_Click;
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
        private void Panel1_Click(object sender, EventArgs e)
        {
            colorDialog1.FullOpen = true;

            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            ThemeSettings.MainThemeColor = colorDialog1.Color;
            _applicationController.UIController.MainForm.UpdateFormColor(ThemeSettings.MainThemeColor);
            panel1.BackColor = ThemeSettings.MainThemeColor;
        }

        private void panelMenuBackColor_Click(object sender, EventArgs e)
        {
            colorDialog1.FullOpen = true;

            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            ThemeSettings.MenuStripBackColor = colorDialog1.Color;
            _applicationController.UIController.MainForm.UpdateMenuBackColor(ThemeSettings.MenuStripBackColor);
            panelMenuBackColor.BackColor = ThemeSettings.MenuStripBackColor;
        }
    }
}
