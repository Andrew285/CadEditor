namespace CadEditor
{
    partial class DividingCubeForm
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
            this.label_Nx = new System.Windows.Forms.Label();
            this.textBox_Nx = new System.Windows.Forms.TextBox();
            this.textBox_Ny = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Nz = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_Nx
            // 
            this.label_Nx.AutoSize = true;
            this.label_Nx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_Nx.Location = new System.Drawing.Point(108, 29);
            this.label_Nx.Name = "label_Nx";
            this.label_Nx.Size = new System.Drawing.Size(41, 20);
            this.label_Nx.TabIndex = 0;
            this.label_Nx.Text = "N_x";
            // 
            // textBox_Nx
            // 
            this.textBox_Nx.Location = new System.Drawing.Point(155, 27);
            this.textBox_Nx.Name = "textBox_Nx";
            this.textBox_Nx.Size = new System.Drawing.Size(100, 22);
            this.textBox_Nx.TabIndex = 1;
            // 
            // textBox_Ny
            // 
            this.textBox_Ny.Location = new System.Drawing.Point(155, 65);
            this.textBox_Ny.Name = "textBox_Ny";
            this.textBox_Ny.Size = new System.Drawing.Size(100, 22);
            this.textBox_Ny.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(108, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "N_y";
            // 
            // textBox_Nz
            // 
            this.textBox_Nz.Location = new System.Drawing.Point(155, 102);
            this.textBox_Nz.Name = "textBox_Nz";
            this.textBox_Nz.Size = new System.Drawing.Size(100, 22);
            this.textBox_Nz.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(108, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "N_z";
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(95, 170);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(85, 27);
            this.button_OK.TabIndex = 6;
            this.button_OK.Text = "OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Location = new System.Drawing.Point(216, 170);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(89, 30);
            this.button_Cancel.TabIndex = 7;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // DividingCubeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 209);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.textBox_Nz);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_Ny);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_Nx);
            this.Controls.Add(this.label_Nx);
            this.Name = "DividingCubeForm";
            this.Text = "DividingCubeForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Nx;
        private System.Windows.Forms.TextBox textBox_Nx;
        private System.Windows.Forms.TextBox textBox_Ny;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Nz;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button button_Cancel;
    }
}