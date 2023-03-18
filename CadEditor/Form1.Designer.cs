namespace CadEditor
{
    partial class Form1
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
			this.openGLControl1 = new SharpGL.OpenGLControl();
			this.sceneControl1 = new SharpGL.SceneControl();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			((System.ComponentModel.ISupportInitialize)(this.openGLControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sceneControl1)).BeginInit();
			this.SuspendLayout();
			// 
			// openGLControl1
			// 
			this.openGLControl1.DrawFPS = true;
			this.openGLControl1.Location = new System.Drawing.Point(9, 9);
			this.openGLControl1.Name = "openGLControl1";
			this.openGLControl1.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
			this.openGLControl1.RenderContextType = SharpGL.RenderContextType.DIBSection;
			this.openGLControl1.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
			this.openGLControl1.Size = new System.Drawing.Size(731, 457);
			this.openGLControl1.TabIndex = 0;
			this.openGLControl1.OpenGLInitialized += new System.EventHandler(this.openGLControl1_OpenGLInitialized_1);
			this.openGLControl1.OpenGLDraw += new SharpGL.RenderEventHandler(this.openGLControl1_OpenGLDraw_1);
			this.openGLControl1.Resized += new System.EventHandler(this.openGLControl1_Resized_1);
			this.openGLControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.openGLControl1_MouseDown);
			this.openGLControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.openGLControl1_MouseMove);
			this.openGLControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.openGLControl1_MouseUp);
			// 
			// sceneControl1
			// 
			this.sceneControl1.DrawFPS = false;
			this.sceneControl1.Location = new System.Drawing.Point(12, 472);
			this.sceneControl1.Name = "sceneControl1";
			this.sceneControl1.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
			this.sceneControl1.RenderContextType = SharpGL.RenderContextType.DIBSection;
			this.sceneControl1.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
			this.sceneControl1.Size = new System.Drawing.Size(381, 208);
			this.sceneControl1.TabIndex = 1;
			// 
			// treeView1
			// 
			this.treeView1.Location = new System.Drawing.Point(426, 472);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(225, 109);
			this.treeView1.TabIndex = 2;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Location = new System.Drawing.Point(426, 587);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(225, 120);
			this.propertyGrid1.TabIndex = 3;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(747, 710);
			this.Controls.Add(this.propertyGrid1);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.sceneControl1);
			this.Controls.Add(this.openGLControl1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "Form1";
			this.Text = "Form1";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.openGLControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sceneControl1)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private SharpGL.OpenGLControl openGLControl1;
        private SharpGL.SceneControl sceneControl1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}

