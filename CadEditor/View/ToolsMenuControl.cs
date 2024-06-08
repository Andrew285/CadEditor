using CadEditor.Controllers;
using SharpGL;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CadEditor.View
{
    public class ToolsMenuControl: BaseControl
    {
        private Form1 _mainForm;
        private OpenGLControl _openGLControl;
        private ApplicationController _applicationController;
        private MenuStrip _menuStrip;
        private ToolStripMenuItem captureSceneToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem cameraToolStripMenuItem1;
        private ToolStripMenuItem aboutToolStripMenuItem;
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

        public ToolsMenuControl(Form1 form, ApplicationController applicationController, OpenGLControl control)
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
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Image = global::CadEditor.Properties.Resources.install;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Image = global::CadEditor.Properties.Resources.download2;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // openLibraryToolStripMenuItem
            // 
            this.openLibraryToolStripMenuItem.Image = global::CadEditor.Properties.Resources.library;
            this.openLibraryToolStripMenuItem.Name = "openLibraryToolStripMenuItem";
            this.openLibraryToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.openLibraryToolStripMenuItem.Text = "Open Library";
            this.openLibraryToolStripMenuItem.Click += new System.EventHandler(this.openLibraryToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.saveToolStripMenuItem.Text = "Save";
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
            this.sceneToolStripMenuItem.Text = "Scene";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addCubeToolStripMenuItem});
            this.addToolStripMenuItem.Image = global::CadEditor.Properties.Resources.add;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addCubeToolStripMenuItem.Click += new System.EventHandler(this.addCubeToolStripMenuItem_Click);
            // 
            // cubeToolStripMenuItem
            // 
            this.addCubeToolStripMenuItem.Name = "cubeToolStripMenuItem";
            this.addCubeToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.addCubeToolStripMenuItem.Text = "Cube";
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Image = global::CadEditor.Properties.Resources.select_object;
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            // 
            // deselectAllToolStripMenuItem
            // 
            this.deselectAllToolStripMenuItem.Image = global::CadEditor.Properties.Resources.deselect_object;
            this.deselectAllToolStripMenuItem.Name = "deselectAllToolStripMenuItem";
            this.deselectAllToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.deselectAllToolStripMenuItem.Text = "Deselect All";
            // 
            // captureSceneToolStripMenuItem
            // 
            this.captureSceneToolStripMenuItem.Image = global::CadEditor.Properties.Resources.camera;
            this.captureSceneToolStripMenuItem.Name = "captureSceneToolStripMenuItem";
            this.captureSceneToolStripMenuItem.ShortcutKeyDisplayString = "F2";
            this.captureSceneToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.captureSceneToolStripMenuItem.Text = "Capture scene";
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
            this.cameraToolStripMenuItem1.Text = "Camera";
            // 
            // setViewXToolStripMenuItem
            // 
            this.setViewXToolStripMenuItem.Name = "setViewXToolStripMenuItem";
            this.setViewXToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.setViewXToolStripMenuItem.Text = "Set View X";
            this.setViewXToolStripMenuItem.Click += new System.EventHandler(this.setViewXToolStripMenuItem_Click);
            // 
            // setViewYToolStripMenuItem
            // 
            this.setViewYToolStripMenuItem.Name = "setViewYToolStripMenuItem";
            this.setViewYToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.setViewYToolStripMenuItem.Text = "Set View Y";
            this.setViewYToolStripMenuItem.Click += new System.EventHandler(this.setViewYToolStripMenuItem_Click);
            // 
            // setViewZToolStripMenuItem
            // 
            this.setViewZToolStripMenuItem.Name = "setViewZToolStripMenuItem";
            this.setViewZToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.setViewZToolStripMenuItem.Text = "Set View Z";
            this.setViewZToolStripMenuItem.Click += new System.EventHandler(this.setViewZToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalSettingsToolStripMenuItem});
            this.settingsToolStripMenuItem.Image = global::CadEditor.Properties.Resources.tools2;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(85, 28);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // generalSettingsToolStripMenuItem
            // 
            this.generalSettingsToolStripMenuItem.Name = "generalSettingsToolStripMenuItem";
            this.generalSettingsToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.generalSettingsToolStripMenuItem.Text = "General Settings";
            this.generalSettingsToolStripMenuItem.Click += new System.EventHandler(this.generalSettingsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::CadEditor.Properties.Resources.help;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(68, 28);
            this.aboutToolStripMenuItem.Text = "Help";

            _menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.sceneToolStripMenuItem,
            this.cameraToolStripMenuItem1,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            // 
            // mode_comboBox
            // 
            sceneMode.Items.AddRange(new string[] { "View Mode", "Edit Mode" });
            this.sceneMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sceneMode.FormattingEnabled = true;
            this.sceneMode.Location = new System.Drawing.Point(12, 30);
            this.sceneMode.Name = "mode_comboBox";
            this.sceneMode.Size = new System.Drawing.Size(121, 21);
            this.sceneMode.TabIndex = 7;
            this.sceneMode.SelectedIndexChanged += new System.EventHandler(this.mode_comboBox_SelectedIndexChanged);
        
            _mainForm.Controls.Add(_menuStrip);
            _mainForm.Controls.Add(sceneMode);

            sceneMode.SelectedIndex = 0;
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
            //scene.Camera.SetViewByAxis(CoordinateAxis.X);
        }

        private void setViewYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //scene.Camera.SetViewByAxis(CoordinateAxis.Y);
        }

        private void setViewZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //scene.Camera.SetViewByAxis(CoordinateAxis.Z);
        }


        //Settings Tab
        private void generalSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _applicationController.UIController.CreateSettingsForm();
        }



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
    }
}
