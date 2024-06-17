using CadEditor.Controllers;
using SharpGL;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CadEditor.Tools.Localization;

namespace CadEditor.View
{
    public class MenuToolsControl: BaseControl
    {
        private Form1 _mainForm;
        private OpenGLControl _openGLControl;
        private ApplicationController _applicationController;
        private MenuStrip _menuStrip;
        private ToolStripMenuItem captureSceneToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem cameraToolStripMenuItem1;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem documentationToolStripMenuItem;
        private ToolStripMenuItem gitHubToolStripMenuItem;
        private ToolStripMenuItem setViewXToolStripMenuItem;
        private ToolStripMenuItem setViewYToolStripMenuItem;
        private ToolStripMenuItem setViewZToolStripMenuItem;
        private ToolStripMenuItem generalSettingsToolStripMenuItem;
        public ToolStripMenuItem sceneToolStripMenuItem;
        private ToolStripMenuItem openLibraryToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem importToolStripMenuItem;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem addCubeToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripMenuItem deselectAllToolStripMenuItem;
        private ComboBox sceneMode;

        public MenuToolsControl(Form1 form, ApplicationController applicationController, OpenGLControl control)
        {
            _openGLControl = control;
            _applicationController = applicationController;
            _mainForm = form;
            _menuStrip = new MenuStrip();
            _menuStrip.BackColor = System.Drawing.SystemColors.ActiveBorder;
            _menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            _menuStrip.ImeMode = ImeMode.NoControl;
            _menuStrip.Location = new System.Drawing.Point(0, 0);
            _menuStrip.Name = "menuStrip1";
            _menuStrip.Padding = new Padding(4, 2, 0, 2);
            _menuStrip.ShowItemToolTips = true;
            _menuStrip.Size = new System.Drawing.Size(1094, 32);
            _menuStrip.TabIndex = 4;
            _menuStrip.Text = "menuStrip1";
            _menuStrip.Visible = true;

            this.fileToolStripMenuItem = new ToolStripMenuItem();
            this.importToolStripMenuItem = new ToolStripMenuItem();
            this.exportToolStripMenuItem = new ToolStripMenuItem();
            this.openLibraryToolStripMenuItem = new ToolStripMenuItem();
            this.saveToolStripMenuItem = new ToolStripMenuItem();
            this.sceneToolStripMenuItem = new ToolStripMenuItem();
            this.addToolStripMenuItem = new ToolStripMenuItem();
            this.addCubeToolStripMenuItem = new ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new ToolStripMenuItem();
            this.deselectAllToolStripMenuItem = new ToolStripMenuItem();
            this.captureSceneToolStripMenuItem = new ToolStripMenuItem();
            this.cameraToolStripMenuItem1 = new ToolStripMenuItem();
            this.setViewXToolStripMenuItem = new ToolStripMenuItem();
            this.setViewYToolStripMenuItem = new ToolStripMenuItem();
            this.setViewZToolStripMenuItem = new ToolStripMenuItem();
            this.settingsToolStripMenuItem = new ToolStripMenuItem();
            this.generalSettingsToolStripMenuItem = new ToolStripMenuItem();
            this.aboutToolStripMenuItem = new ToolStripMenuItem();
            this.documentationToolStripMenuItem = new ToolStripMenuItem();
            this.gitHubToolStripMenuItem = new ToolStripMenuItem();
            sceneMode = new ComboBox();
        }

        public void Initialize()
        {
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.openLibraryToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Image = global::CadEditor.Properties.Resources.document;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(61, 28);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Image = global::CadEditor.Properties.Resources.install;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Image = global::CadEditor.Properties.Resources.download2;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // openLibraryToolStripMenuItem
            // 
            this.openLibraryToolStripMenuItem.Image = global::CadEditor.Properties.Resources.library;
            this.openLibraryToolStripMenuItem.Name = "openLibraryToolStripMenuItem";
            this.openLibraryToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.openLibraryToolStripMenuItem.Click += new System.EventHandler(this.openLibraryToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // sceneToolStripMenuItem
            // 
            this.sceneToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.deselectAllToolStripMenuItem,
            this.captureSceneToolStripMenuItem});
            this.sceneToolStripMenuItem.Image = global::CadEditor.Properties.Resources.accelerometer;
            this.sceneToolStripMenuItem.Name = "sceneToolStripMenuItem";
            this.sceneToolStripMenuItem.Size = new System.Drawing.Size(74, 28);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addCubeToolStripMenuItem});
            this.addToolStripMenuItem.Image = global::CadEditor.Properties.Resources.add;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.addCubeToolStripMenuItem.Click += new System.EventHandler(this.addCubeToolStripMenuItem_Click);
            // 
            // cubeToolStripMenuItem
            // 
            this.addCubeToolStripMenuItem.Name = "cubeToolStripMenuItem";
            this.addCubeToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Image = global::CadEditor.Properties.Resources.select_object;
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            // 
            // deselectAllToolStripMenuItem
            // 
            this.deselectAllToolStripMenuItem.Image = global::CadEditor.Properties.Resources.deselect_object;
            this.deselectAllToolStripMenuItem.Name = "deselectAllToolStripMenuItem";
            this.deselectAllToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            // 
            // captureSceneToolStripMenuItem
            // 
            this.captureSceneToolStripMenuItem.Image = global::CadEditor.Properties.Resources.camera;
            this.captureSceneToolStripMenuItem.Name = "captureSceneToolStripMenuItem";
            this.captureSceneToolStripMenuItem.ShortcutKeyDisplayString = "F2";
            this.captureSceneToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.captureSceneToolStripMenuItem.Click += new System.EventHandler(this.captureSceneToolStripMenuItem_Click);
            // 
            // cameraToolStripMenuItem1
            // 
            this.cameraToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setViewXToolStripMenuItem,
            this.setViewYToolStripMenuItem,
            this.setViewZToolStripMenuItem});
            this.cameraToolStripMenuItem1.Image = global::CadEditor.Properties.Resources.video_camera;
            this.cameraToolStripMenuItem1.Name = "cameraToolStripMenuItem1";
            this.cameraToolStripMenuItem1.Size = new System.Drawing.Size(84, 28);
            // 
            // setViewXToolStripMenuItem
            // 
            this.setViewXToolStripMenuItem.Name = "setViewXToolStripMenuItem";
            this.setViewXToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.setViewXToolStripMenuItem.Click += new System.EventHandler(this.setViewXToolStripMenuItem_Click);
            // 
            // setViewYToolStripMenuItem
            // 
            this.setViewYToolStripMenuItem.Name = "setViewYToolStripMenuItem";
            this.setViewYToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.setViewYToolStripMenuItem.Click += new System.EventHandler(this.setViewYToolStripMenuItem_Click);
            // 
            // setViewZToolStripMenuItem
            // 
            this.setViewZToolStripMenuItem.Name = "setViewZToolStripMenuItem";
            this.setViewZToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.setViewZToolStripMenuItem.Click += new System.EventHandler(this.setViewZToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalSettingsToolStripMenuItem});
            this.settingsToolStripMenuItem.Image = global::CadEditor.Properties.Resources.tools2;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(85, 28);
            // 
            // generalSettingsToolStripMenuItem
            // 
            this.generalSettingsToolStripMenuItem.Name = "generalSettingsToolStripMenuItem";
            this.generalSettingsToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.generalSettingsToolStripMenuItem.Click += new System.EventHandler(this.generalSettingsToolStripMenuItem_Click);
            // 
            // documentationToolStripMenuItem
            // 
            this.documentationToolStripMenuItem.Name = "documentationToolStripMenuItem";
            this.documentationToolStripMenuItem.Image = global::CadEditor.Properties.Resources.documentation_icon;
            this.documentationToolStripMenuItem.Size = new System.Drawing.Size(85, 28);
            this.documentationToolStripMenuItem.Click += new System.EventHandler(this.documentationToolStripMenuItem_Click);
            // 
            // gitHubToolStripMenuItem
            // 
            this.gitHubToolStripMenuItem.Name = "gitHubToolStripMenuItem";
            this.gitHubToolStripMenuItem.Image = global::CadEditor.Properties.Resources.github_icon;
            this.gitHubToolStripMenuItem.Size = new System.Drawing.Size(85, 28);
            this.gitHubToolStripMenuItem.Click += new System.EventHandler(this.gitHubToolStripMenuItem_Click);


            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] 
            {
                documentationToolStripMenuItem,
                gitHubToolStripMenuItem,
            });
            this.aboutToolStripMenuItem.Image = global::CadEditor.Properties.Resources.help;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(68, 28);

            _menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.sceneToolStripMenuItem,
            this.cameraToolStripMenuItem1,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            // 
            // mode_comboBox
            // 

            this.sceneMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sceneMode.FormattingEnabled = true;
            this.sceneMode.Location = new System.Drawing.Point(12, 30);
            this.sceneMode.Name = "mode_comboBox";
            this.sceneMode.Size = new System.Drawing.Size(121, 21);
            this.sceneMode.TabIndex = 7;
            this.sceneMode.SelectedIndexChanged += new System.EventHandler(this.mode_comboBox_SelectedIndexChanged);
        
            _mainForm.Controls.Add(_menuStrip);
            _mainForm.Controls.Add(sceneMode);

            SetTextToControls();
        }

        public ComboBox GetSceneModeComboBox()
        {
            return sceneMode;
        }

        private void addCubeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _applicationController.AddNewCubeElement(new Point3D(0, 0, 0));
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog.Title = "Save File";
            saveFileDialog.FileName = "MyFile.txt"; // Default file name

            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                string exportString = _applicationController.SceneController.Scene.Export();

                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine(exportString);
                        writer.Close(); // Close the writer to flush and release resources
                    }

                    MessageBox.Show("File saved successfully.", "Save File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while saving the file: " + ex.Message, "Save File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.Title = "Open File";

            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    string[] lines = File.ReadAllLines(filePath);
                    _applicationController.SceneController.Scene.Import(lines);
                    _applicationController.SceneCollection.Import(_applicationController.SceneController.Scene.ObjectCollection);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while reading the file: " + ex.Message, "Read File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Scene Tab
        private void captureSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp = _applicationController.RenderController.CaptureScreen();
            bmp.Save(@"D:\Projects\VisualStudio\CadEditor\CadEditor\LibrarySaves\Screenshots\", ImageFormat.Jpeg);
        }


        //Camera Tab
        private void setViewXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _applicationController.RenderController.Camera.SetCameraAt(90, 0, 0);
        }

        private void setViewYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _applicationController.RenderController.Camera.SetCameraAt(0, 90, 0);
        }

        private void setViewZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _applicationController.RenderController.Camera.SetCameraAt(-90, 0, 0);
        }


        //Settings Tab
        private void generalSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _applicationController.UIController.CreateSettingsForm();
        }

        //About Tab
        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _applicationController.OpenDocumentation();
        }

        //About Tab
        private void gitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _applicationController.OpenGitHubRepo();
        }

        //Project Tab
        private void openLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _applicationController.OpenLibrary();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _applicationController.SaveProject();
        }

        private void mode_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _applicationController.HandlePressTab(sceneMode.SelectedIndex);
        }

        public void Resize(object sender, EventArgs e)
        {

        }

        public void SetTextToControls()
        {
            this.fileToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.file_tool);
            this.importToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.import_tool);
            this.exportToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.export_tool);
            this.openLibraryToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.open_library_tool);
            this.saveToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.save_tool);
            this.sceneToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.scene_tool);
            this.addToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.add_element_tool);
            this.addCubeToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.add_cube_element_tool);
            this.selectAllToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.select_all_tool);
            this.captureSceneToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.capture_screen_tool);
            this.deselectAllToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.deselect_all_tool);
            this.cameraToolStripMenuItem1.Text = _applicationController.Localization.GetTranslationOf(Strings.camera_tool);
            this.setViewXToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.set_view_x_tool);
            this.setViewYToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.set_view_y_tool);
            this.setViewZToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.set_view_z_tool);
            this.settingsToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.settings_tool);
            this.generalSettingsToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.general_settings_tool);
            this.aboutToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.help_tool);
            this.documentationToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.documentation_tool);
            this.gitHubToolStripMenuItem.Text = _applicationController.Localization.GetTranslationOf(Strings.gitHub_tool);

            sceneMode.Items.Clear();
            sceneMode.Items.AddRange(new string[]
            {
                _applicationController.Localization.GetTranslationOf(Strings.view_mode),
                _applicationController.Localization.GetTranslationOf(Strings.edit_mode)
            });
            sceneMode.SelectedIndex = 0;
        }
    }
}
