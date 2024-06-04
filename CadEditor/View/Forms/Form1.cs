using CadEditor.Controllers;
using CadEditor.MeshObjects;
using CadEditor.View;
using SharpGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace CadEditor
{
    public partial class Form1 : Form
    {
        private ApplicationController _applicationController;
        private static int KeyX_Clicks = 0;
        private static int KeyY_Clicks = 0;
        private static int KeyZ_Clicks = 0;

        private OpenGLController _openGLController;

        public Form1()
        {
            InitializeComponent();

            _applicationController = new ApplicationController(this);
            _applicationController.Initialize();
            _openGLController = new OpenGLController(GetOpenGLControl(), _applicationController);

            KeyPreview = true;
            mode_comboBox.Items.AddRange(new string[] { "View Mode", "Edit Mode" });
            mode_comboBox.SelectedItem = mode_comboBox.Items[0];
            checkBox_DrawFacets.Checked = true;
        }

        public OpenGLControl GetOpenGLControl()
        {
            return openGLControl1;
        }

        public TreeView GetTreeView()
        {
            return treeView1;
        }

        public MenuStrip GetMenuStrip()
        {
            return menuStrip1;
        }

		#region ---- Key Events ----

		private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.X:
                    KeyX_Clicks = _applicationController.SetObjectToAxis(CoordinateAxis.X, KeyX_Clicks);
                    break;

                case Keys.Y:
                    KeyY_Clicks = _applicationController.SetObjectToAxis(CoordinateAxis.Y, KeyY_Clicks);
                    break;

                case Keys.Z:
                    KeyZ_Clicks = _applicationController.SetObjectToAxis(CoordinateAxis.Z, KeyZ_Clicks);
                    break;

                case Keys.Space:
                    _applicationController.AttachElements();
                    break;


                case Keys.ControlKey:
                    _applicationController.SceneController.Scene.IsObjectRotate = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    _applicationController.SceneController.Scene.IsObjectRotate = false;
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool baseResult = base.ProcessCmdKey(ref msg, keyData);
            if (keyData == Keys.Tab)
            {
                int oppositeViewIndex = _applicationController.HandlePressTab(mode_comboBox.SelectedIndex);
                mode_comboBox.SelectedItem = mode_comboBox.Items[oppositeViewIndex];
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Z))
            {
                _applicationController.HandlePressCtrlZ();
            }
            else if (keyData == (Keys.Control | Keys.Shift | Keys.Z))
            {
                _applicationController.HandlePressCtrlShiftZ();
            }

            return baseResult;
        }
		#endregion

        private void cubeToolStripMenuItem_Click(object sender, EventArgs e)
		{
            //_applicationController.AddNewCubeElement(new Point3D(0, 0, 0));
		}

		private void mode_comboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
            _applicationController.HandlePressTab(mode_comboBox.SelectedIndex);
        }

        private void checkBox_DrawFacets_CheckedChanged(object sender, EventArgs e)
        {
            _applicationController.RenderController.DrawFacets = checkBox_DrawFacets.Checked;
        }

        //public void DeselectAndSaveCommand(Scene scene, ISceneObject obj)
        //{
        //    DeselectionCommand deselectionCommand = new DeselectionCommand(_applicationController, obj);
        //    deselectionCommand.Execute();
        //    _applicationController.CommandsHistory.Push(deselectionCommand);
        //}

        //public void SelectAndSaveCommand(Scene scene, ISceneObject obj)
        //{
        //    SelectionCommand selectionCommand = new SelectionCommand(_applicationController, obj);
        //    selectionCommand.Execute();
        //    _applicationController.CommandsHistory.Push(selectionCommand);
        //}

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
    //        scene.DeselectAll();

    //        TreeNode selectedTreeNode = sceneCollection.GetSelectedNode();
    //        if(selectedTreeNode != null)
    //        {
    //            ISceneObject obj = sceneCollection.GetObjectByNode(selectedTreeNode, scene.ObjectCollection);
    //            if(obj != null)
    //            {
    //                scene.SelectedObject = obj;
    //                obj.Select();
    //                //scene.InitSelectingCoordAxes(nodeObjects[0], 2.8f, 1.0);
    //                AxisSystem axisSystem = new AxisSystem(obj.GetCenterPoint(), RenderController.selectingRay);
    //                scene.ObjectCollection.Insert(0, axisSystem);
				//}
    //        }
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
            Bitmap bmp = CaptureScreen();
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

        private Bitmap CaptureScreen()
        {
            Control c = openGLControl1;
            Bitmap bmp = new System.Drawing.Bitmap(c.Width, c.Height);
            c.DrawToBitmap(bmp, c.ClientRectangle);
            return bmp;
        }

        private void openLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LibraryForm libraryForm = new LibraryForm(_applicationController.Library.GetAllSaves());
            DialogResult result = libraryForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                SaveData saveData = libraryForm.SelectedSave;
                string[] lines = File.ReadAllLines(saveData.GetFilePath());
                _applicationController.SceneController.Scene.Import(lines);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp = CaptureScreen();

            SaveForm form = new SaveForm(bmp);
            DialogResult result = form.ShowDialog();

            if (result == DialogResult.OK)
            {
                string nameOfSave = form.SaveData.GetTitle();
                string exportString = _applicationController.SceneController.Scene.Export();

                bmp.Save(@"D:\Projects\VisualStudio\CadEditor\CadEditor\LibrarySaves\Screenshots\" + nameOfSave + ".jpeg", ImageFormat.Jpeg);
                string filePath = @"D:\Projects\VisualStudio\CadEditor\CadEditor\LibrarySaves\Scene\" + nameOfSave + ".txt";

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(exportString);
                    writer.Close(); // Close the writer to flush and release resources
                }
                _applicationController.Library.AddSave(bmp, filePath, nameOfSave);
            }
        }


        public void UpdateFormColor(Color color)
        {
            this.BackColor = color;
        }

        public void UpdateMenuBackColor(Color color)
        {
            this.menuStrip1.BackColor = color;
        }

        private void generalTab_checkBoxDrawRay_CheckedChanged(object sender, EventArgs e)
        {
            _applicationController.RenderController.IsRayDrawable = generalTab_checkBoxDrawRay.Checked ? true : false;
        }

    }
}
