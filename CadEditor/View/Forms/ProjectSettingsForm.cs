using CadEditor.Controllers;
using CadEditor.Properties;
using CadEditor.Settings;
using CadEditor.Tools.Localization;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime;
using System.Windows.Forms;

namespace CadEditor.View.Forms
{
    public partial class ProjectSettingsForm : Form
    {
        private TabControl tabControl;
        private TabPage tabAppearance;
        private TabPage tabFilePaths;
        private TabPage tabLanguage;
        private TabPage tabKeyBindings;
        private TabPage tabOtherSettings;
        public LanguageSettingsControl LanguageSettingsControl;
        private ApplicationController _applicationController;

        public ProjectSettingsForm(ApplicationController applicationController)
        {
            _applicationController = applicationController;
            LanguageSettingsControl = new LanguageSettingsControl(_applicationController);
            InitializeSettingsComponent();
        }

        private void InitializeSettingsComponent()
        {
            // Initialize TabControl and Tabs
            tabControl = new TabControl();
            tabAppearance = new TabPage("Appearance");
            tabFilePaths = new TabPage("File Paths");
            tabLanguage = new TabPage("Language");
            tabKeyBindings = new TabPage("Key Bindings");
            tabOtherSettings = new TabPage("Other Settings");

            tabControl.Dock = DockStyle.Fill;

            // Add tabs to TabControl
            tabControl.TabPages.Add(tabAppearance);
            tabControl.TabPages.Add(tabFilePaths);
            tabControl.TabPages.Add(tabLanguage);
            tabControl.TabPages.Add(tabKeyBindings);
            tabControl.TabPages.Add(tabOtherSettings);

            // Add controls to each tab
            AddAppearanceControls();
            AddFilePathControls();
            AddKeyBindingControls();
            AddOtherSettingsControls();
            LanguageSettingsControl.Initialize(tabLanguage);

            // Add TabControl to the Form
            Controls.Add(tabControl);


            // Create buttons
            Button applyButton = new Button();
            applyButton.Text = "Apply";
            applyButton.Click += ApplyButton_Click;

            Button closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.AutoSize = true;
            closeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            closeButton.AutoEllipsis = false;
            //openNewButton.Click += OpenButton_Click;

            // Create a panel for buttons
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 50;
            buttonPanel.Controls.Add(applyButton);
            buttonPanel.Controls.Add(closeButton);

            // Add button panel to the form
            this.Controls.Add(buttonPanel);

            // Set Form properties
            Text = "Settings";
            Size = new System.Drawing.Size(500, 400);
        }

        private void AddAppearanceControls()
        {
            // Example appearance settings
            Label lblTheme = new Label { Text = "Theme:", Location = new System.Drawing.Point(20, 20) };
            ComboBox cbTheme = new ComboBox { Location = new System.Drawing.Point(120, 20) };
            cbTheme.Items.AddRange(new string[] { "Light", "Dark", "Blue" });

            Label lblGridVisibility = new Label { Text = "Show Grid:", Location = new System.Drawing.Point(20, 60) };
            CheckBox cbGridVisibility = new CheckBox { Location = new System.Drawing.Point(120, 60) };

            tabAppearance.Controls.Add(lblTheme);
            tabAppearance.Controls.Add(cbTheme);
            tabAppearance.Controls.Add(lblGridVisibility);
            tabAppearance.Controls.Add(cbGridVisibility);
        }

        private void AddFilePathControls()
        {
            // Example file path settings
            Label lblModelPath = new Label { Text = "Model Path:", Location = new System.Drawing.Point(20, 20) };
            TextBox txtModelPath = new TextBox { Location = new System.Drawing.Point(120, 20), Width = 250 };
            Button btnBrowseModelPath = new Button { Text = "Browse", Location = new System.Drawing.Point(380, 18) };

            Label lblTexturePath = new Label { Text = "Texture Path:", Location = new System.Drawing.Point(20, 60) };
            TextBox txtTexturePath = new TextBox { Location = new System.Drawing.Point(120, 60), Width = 250 };
            Button btnBrowseTexturePath = new Button { Text = "Browse", Location = new System.Drawing.Point(380, 58) };

            tabFilePaths.Controls.Add(lblModelPath);
            tabFilePaths.Controls.Add(txtModelPath);
            tabFilePaths.Controls.Add(btnBrowseModelPath);
            tabFilePaths.Controls.Add(lblTexturePath);
            tabFilePaths.Controls.Add(txtTexturePath);
            tabFilePaths.Controls.Add(btnBrowseTexturePath);
        }


        private void AddKeyBindingControls()
        {
            // Example key binding settings
            Label lblKeyBind1 = new Label { Text = "Rotate View:", Location = new System.Drawing.Point(20, 20) };
            TextBox txtKeyBind1 = new TextBox { Location = new System.Drawing.Point(120, 20), Width = 100 };

            Label lblKeyBind2 = new Label { Text = "Zoom In/Out:", Location = new System.Drawing.Point(20, 60) };
            TextBox txtKeyBind2 = new TextBox { Location = new System.Drawing.Point(120, 60), Width = 100 };

            tabKeyBindings.Controls.Add(lblKeyBind1);
            tabKeyBindings.Controls.Add(txtKeyBind1);
            tabKeyBindings.Controls.Add(lblKeyBind2);
            tabKeyBindings.Controls.Add(txtKeyBind2);
        }

        private void AddOtherSettingsControls()
        {
            // Example other settings
            Label lblAutoSave = new Label { Text = "Auto Save Interval (min):", Location = new System.Drawing.Point(20, 20) };
            NumericUpDown numAutoSave = new NumericUpDown { Location = new System.Drawing.Point(180, 20), Minimum = 1, Maximum = 60 };

            Label lblShowTips = new Label { Text = "Show Tips on Startup:", Location = new System.Drawing.Point(20, 60) };
            CheckBox cbShowTips = new CheckBox { Location = new System.Drawing.Point(180, 60) };

            tabOtherSettings.Controls.Add(lblAutoSave);
            tabOtherSettings.Controls.Add(numAutoSave);
            tabOtherSettings.Controls.Add(lblShowTips);
            tabOtherSettings.Controls.Add(cbShowTips);
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            LanguageSettingsControl.InvokeEvents();
            _applicationController.SettingsController.SaveData("");
            //_applicationController.UIController.MainForm.Invalidate();
            this.Close();
        }
    }
}
