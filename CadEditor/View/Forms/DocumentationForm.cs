using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadEditor.View.Forms
{
    public partial class DocumentationForm : Form
    {
        private System.Windows.Forms.WebBrowser webBrowserDocumentation;

        public DocumentationForm()
        {
            Initialize();
            LoadDocumentation();
        }

        private void LoadDocumentation()
        {
            // Specify the path to your HTML file
            string htmlFilePath = @"D:\Projects\VisualStudio\CadEditor\CadEditor\Configuration\documentation.html";
            webBrowserDocumentation.Navigate(htmlFilePath);
        }

        private void Initialize()
        {
            this.webBrowserDocumentation = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowserDocumentation
            // 
            this.webBrowserDocumentation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserDocumentation.Location = new System.Drawing.Point(0, 0);
            this.webBrowserDocumentation.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserDocumentation.Name = "webBrowserDocumentation";
            this.webBrowserDocumentation.Size = new System.Drawing.Size(800, 450);
            this.webBrowserDocumentation.TabIndex = 0;
            // 
            // DocumentationForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 450);
            this.Controls.Add(this.webBrowserDocumentation);
            this.Name = "DocumentationForm";
            this.Text = "Documentation";
            this.ResumeLayout(false);
        }

    }
}
