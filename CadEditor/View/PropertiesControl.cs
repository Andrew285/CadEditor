using CadEditor.Controllers;
using CadEditor.Tools;
using System;
using System.Windows.Forms;

namespace CadEditor.View
{
    public class PropertiesControl: BaseControl
    {
        private Form1 _mainForm;
        private ApplicationController _applicationController;
        private CheckBox checkBox_DrawFacets;
        private TabControl propertiesTabControl;
        private TabPage generalTab;
        private CheckBox generalTab_checkBoxDrawRay;
        private TabPage tabPage2;
        private int propertiesTabControlWidth = 230;
        private int propertiesnTabControlHeight = 257;
        private int propertiesTabControlStartX = 854;
        private int propertiesTabControlStartY = 383;

        public PropertiesControl(Form1 form, ApplicationController applicationController) 
        {
            _mainForm = form;
            _applicationController = applicationController;
            this.checkBox_DrawFacets = new System.Windows.Forms.CheckBox();
            this.propertiesTabControl = new System.Windows.Forms.TabControl();
            this.generalTab = new System.Windows.Forms.TabPage();
            this.generalTab_checkBoxDrawRay = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
        }

        public void Initialize()
        {
            // 
            // checkBox_DrawFacets
            // 
            this.checkBox_DrawFacets.AutoSize = true;
            this.checkBox_DrawFacets.Location = new System.Drawing.Point(5, 5);
            this.checkBox_DrawFacets.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_DrawFacets.Name = "checkBox_DrawFacets";
            this.checkBox_DrawFacets.Size = new System.Drawing.Size(86, 17);
            this.checkBox_DrawFacets.TabIndex = 9;
            this.checkBox_DrawFacets.Text = "Draw Facets";
            this.checkBox_DrawFacets.UseVisualStyleBackColor = true;
            this.checkBox_DrawFacets.CheckedChanged += new System.EventHandler(this.checkBox_DrawFacets_CheckedChanged);
            // 
            // tabControl1
            // 
            this.propertiesTabControl.Controls.Add(this.generalTab);
            this.propertiesTabControl.Controls.Add(this.tabPage2);
            this.propertiesTabControl.Location = new System.Drawing.Point(propertiesTabControlStartX, propertiesTabControlStartY);
            this.propertiesTabControl.Name = "tabControl1";
            this.propertiesTabControl.SelectedIndex = 0;
            this.propertiesTabControl.Size = new System.Drawing.Size(propertiesTabControlWidth, propertiesnTabControlHeight);
            this.propertiesTabControl.TabIndex = 10;
            // 
            // generalTab
            // 
            this.generalTab.Controls.Add(this.generalTab_checkBoxDrawRay);
            this.generalTab.Controls.Add(this.checkBox_DrawFacets);
            this.generalTab.Location = new System.Drawing.Point(4, 22);
            this.generalTab.Name = "generalTab";
            this.generalTab.Padding = new System.Windows.Forms.Padding(3);
            this.generalTab.Size = new System.Drawing.Size(222, 231);
            this.generalTab.TabIndex = 0;
            this.generalTab.Text = "General";
            this.generalTab.UseVisualStyleBackColor = true;
            // 
            // generalTab_checkBoxDrawRay
            // 
            this.generalTab_checkBoxDrawRay.AutoSize = true;
            this.generalTab_checkBoxDrawRay.Location = new System.Drawing.Point(5, 26);
            this.generalTab_checkBoxDrawRay.Margin = new System.Windows.Forms.Padding(2);
            this.generalTab_checkBoxDrawRay.Name = "generalTab_checkBoxDrawRay";
            this.generalTab_checkBoxDrawRay.Size = new System.Drawing.Size(120, 17);
            this.generalTab_checkBoxDrawRay.TabIndex = 10;
            this.generalTab_checkBoxDrawRay.Text = "Draw Selecting Ray";
            this.generalTab_checkBoxDrawRay.UseVisualStyleBackColor = true;
            this.generalTab_checkBoxDrawRay.CheckedChanged += new System.EventHandler(this.generalTab_checkBoxDrawRay_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(222, 231);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;

            _mainForm.Controls.Add(this.propertiesTabControl);

            checkBox_DrawFacets.Checked = true;
        }

        public void Resize(object sender, EventArgs e)
        {
            int newX = _mainForm.Width - SceneCollectionControl.collectionGroupBoxWidth - UITools.GAP;
            int newY = SceneCollectionControl.collectionGroupBoxStartY + SceneCollectionControl.collectionGroupBoxHeight + UITools.GAP;
            propertiesTabControl.Location = new System.Drawing.Point(newX, newY);
        }

        private void checkBox_DrawFacets_CheckedChanged(object sender, EventArgs e)
        {
            _applicationController.RenderController.DrawFacets = checkBox_DrawFacets.Checked;
        }

        private void generalTab_checkBoxDrawRay_CheckedChanged(object sender, EventArgs e)
        {
            _applicationController.RenderController.IsRayDrawable = generalTab_checkBoxDrawRay.Checked ? true : false;
        }
    }
}
