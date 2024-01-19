namespace CadEditor.View.Forms
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageInterface = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabPageScene = new System.Windows.Forms.TabPage();
            this.buttonSceneApply = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxAxisCubeDrawVertices = new System.Windows.Forms.CheckBox();
            this.checkBoxAxisCubeDrawEdges = new System.Windows.Forms.CheckBox();
            this.checkBoxAxisCubeDrawFacets = new System.Windows.Forms.CheckBox();
            this.tabPageThemes = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabPageKeymap = new System.Windows.Forms.TabPage();
            this.tabPageFilePaths = new System.Windows.Forms.TabPage();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.tabControl1.SuspendLayout();
            this.tabPageInterface.SuspendLayout();
            this.tabPageScene.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageThemes.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageInterface);
            this.tabControl1.Controls.Add(this.tabPageScene);
            this.tabControl1.Controls.Add(this.tabPageThemes);
            this.tabControl1.Controls.Add(this.tabPageKeymap);
            this.tabControl1.Controls.Add(this.tabPageFilePaths);
            this.tabControl1.Location = new System.Drawing.Point(1, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(554, 341);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageInterface
            // 
            this.tabPageInterface.Controls.Add(this.label1);
            this.tabPageInterface.Controls.Add(this.comboBox1);
            this.tabPageInterface.Location = new System.Drawing.Point(4, 22);
            this.tabPageInterface.Name = "tabPageInterface";
            this.tabPageInterface.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInterface.Size = new System.Drawing.Size(546, 315);
            this.tabPageInterface.TabIndex = 0;
            this.tabPageInterface.Text = "Interface";
            this.tabPageInterface.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(117, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Language";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(191, 17);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // tabPageScene
            // 
            this.tabPageScene.Controls.Add(this.buttonSceneApply);
            this.tabPageScene.Controls.Add(this.groupBox1);
            this.tabPageScene.Location = new System.Drawing.Point(4, 22);
            this.tabPageScene.Name = "tabPageScene";
            this.tabPageScene.Size = new System.Drawing.Size(546, 315);
            this.tabPageScene.TabIndex = 4;
            this.tabPageScene.Text = "Scene";
            this.tabPageScene.UseVisualStyleBackColor = true;
            // 
            // buttonSceneApply
            // 
            this.buttonSceneApply.Location = new System.Drawing.Point(449, 274);
            this.buttonSceneApply.Name = "buttonSceneApply";
            this.buttonSceneApply.Size = new System.Drawing.Size(75, 23);
            this.buttonSceneApply.TabIndex = 1;
            this.buttonSceneApply.Text = "Apply";
            this.buttonSceneApply.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxAxisCubeDrawVertices);
            this.groupBox1.Controls.Add(this.checkBoxAxisCubeDrawEdges);
            this.groupBox1.Controls.Add(this.checkBoxAxisCubeDrawFacets);
            this.groupBox1.Location = new System.Drawing.Point(7, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Axis Cube";
            // 
            // checkBoxAxisCubeDrawVertices
            // 
            this.checkBoxAxisCubeDrawVertices.AutoSize = true;
            this.checkBoxAxisCubeDrawVertices.Location = new System.Drawing.Point(6, 65);
            this.checkBoxAxisCubeDrawVertices.Name = "checkBoxAxisCubeDrawVertices";
            this.checkBoxAxisCubeDrawVertices.Size = new System.Drawing.Size(92, 17);
            this.checkBoxAxisCubeDrawVertices.TabIndex = 2;
            this.checkBoxAxisCubeDrawVertices.Text = "Draw Vertices";
            this.checkBoxAxisCubeDrawVertices.UseVisualStyleBackColor = true;
            this.checkBoxAxisCubeDrawVertices.CheckedChanged += new System.EventHandler(this.checkBoxAxisCubeDrawVertices_CheckedChanged);
            // 
            // checkBoxAxisCubeDrawEdges
            // 
            this.checkBoxAxisCubeDrawEdges.AutoSize = true;
            this.checkBoxAxisCubeDrawEdges.Location = new System.Drawing.Point(6, 42);
            this.checkBoxAxisCubeDrawEdges.Name = "checkBoxAxisCubeDrawEdges";
            this.checkBoxAxisCubeDrawEdges.Size = new System.Drawing.Size(84, 17);
            this.checkBoxAxisCubeDrawEdges.TabIndex = 1;
            this.checkBoxAxisCubeDrawEdges.Text = "Draw Edges";
            this.checkBoxAxisCubeDrawEdges.UseVisualStyleBackColor = true;
            this.checkBoxAxisCubeDrawEdges.CheckedChanged += new System.EventHandler(this.checkBoxAxisCubeDrawEdges_CheckedChanged);
            // 
            // checkBoxAxisCubeDrawFacets
            // 
            this.checkBoxAxisCubeDrawFacets.AutoSize = true;
            this.checkBoxAxisCubeDrawFacets.Location = new System.Drawing.Point(6, 19);
            this.checkBoxAxisCubeDrawFacets.Name = "checkBoxAxisCubeDrawFacets";
            this.checkBoxAxisCubeDrawFacets.Size = new System.Drawing.Size(86, 17);
            this.checkBoxAxisCubeDrawFacets.TabIndex = 0;
            this.checkBoxAxisCubeDrawFacets.Text = "Draw Facets";
            this.checkBoxAxisCubeDrawFacets.UseVisualStyleBackColor = true;
            this.checkBoxAxisCubeDrawFacets.CheckedChanged += new System.EventHandler(this.checkBoxAxisCubeDrawFacets_CheckedChanged);
            // 
            // tabPageThemes
            // 
            this.tabPageThemes.Controls.Add(this.label2);
            this.tabPageThemes.Controls.Add(this.panel1);
            this.tabPageThemes.Location = new System.Drawing.Point(4, 22);
            this.tabPageThemes.Name = "tabPageThemes";
            this.tabPageThemes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageThemes.Size = new System.Drawing.Size(546, 315);
            this.tabPageThemes.TabIndex = 1;
            this.tabPageThemes.Text = "Themes";
            this.tabPageThemes.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(135, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Main Theme";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(223, 37);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(82, 21);
            this.panel1.TabIndex = 0;
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            // 
            // tabPageKeymap
            // 
            this.tabPageKeymap.Location = new System.Drawing.Point(4, 22);
            this.tabPageKeymap.Name = "tabPageKeymap";
            this.tabPageKeymap.Size = new System.Drawing.Size(546, 315);
            this.tabPageKeymap.TabIndex = 2;
            this.tabPageKeymap.Text = "Keymap";
            this.tabPageKeymap.UseVisualStyleBackColor = true;
            // 
            // tabPageFilePaths
            // 
            this.tabPageFilePaths.Location = new System.Drawing.Point(4, 22);
            this.tabPageFilePaths.Name = "tabPageFilePaths";
            this.tabPageFilePaths.Size = new System.Drawing.Size(546, 315);
            this.tabPageFilePaths.TabIndex = 3;
            this.tabPageFilePaths.Text = "File Paths";
            this.tabPageFilePaths.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 342);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabPageInterface.ResumeLayout(false);
            this.tabPageInterface.PerformLayout();
            this.tabPageScene.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageThemes.ResumeLayout(false);
            this.tabPageThemes.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageInterface;
        private System.Windows.Forms.TabPage tabPageThemes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TabPage tabPageKeymap;
        private System.Windows.Forms.TabPage tabPageFilePaths;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tabPageScene;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxAxisCubeDrawVertices;
        private System.Windows.Forms.CheckBox checkBoxAxisCubeDrawEdges;
        private System.Windows.Forms.CheckBox checkBoxAxisCubeDrawFacets;
        private System.Windows.Forms.Button buttonSceneApply;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}