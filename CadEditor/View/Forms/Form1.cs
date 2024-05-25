using CadEditor.Controllers;
using CadEditor.MeshObjects;
using CadEditor.Models.Commands;
using CadEditor.Models.Scene;
using CadEditor.Models.Scene.MeshObjects;
using CadEditor.Properties;
using CadEditor.Settings;
using CadEditor.View.Forms;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.WebSockets;
using System.Windows.Forms;

namespace CadEditor
{

    public partial class Form1 : Form
    {
        private ApplicationController _applicationController;
        private static Form1 instance;
        //private Library library;
        private static Scene scene;
        //private ContextMenuStrip contextMenuStrip;
        private SceneCollection sceneCollection;
        //public AxisSystem AttachingAxisSystem { get; private set; }
        //private Camera camera;
        //private CommandsHistory commandsHistory;

        //private ComplexCube targetCube;
        //private ComplexCube attachingCube;
        //private CoordinateAxisType targetFacetAxis;
        //private CoordinateAxisType attachingFacetAxis;

        private static int KeyX_Clicks = 0;
        private static int KeyY_Clicks = 0;
        private static int KeyZ_Clicks = 0;

        public static Form1 GetInstance()
        {
            if (instance == null)
            {
                instance = new Form1();
            }

            return instance;
        }

        public Form1()
        {
            InitializeComponent();
            instance = this;
            KeyPreview = true;

            //Load Settings
            SettingsController.GetInstance().LoadData(MainSettings.FilePath);
            this.BackColor = ThemeSettings.MainThemeColor;
            this.menuStrip1.BackColor = ThemeSettings.MenuStripBackColor;

            mode_comboBox.Items.AddRange(new string[] { "View Mode", "Edit Mode" });
            mode_comboBox.SelectedItem = mode_comboBox.Items[0];
            checkBox_DrawFacets.Checked = true;

            //Set Events
            GraphicsGL.Control.MouseWheel += new MouseEventHandler(openGLControl_MouseWheel);
        }

		#region ---- OpenGLControl Events ----

		private void openGLControl1_OpenGLInitialized_1(object sender, EventArgs e)
        {

            GraphicsGL.CreateInstance(openGLControl1);
            GraphicsGL.GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);

            //Initializing fundemental objects of scene
            sceneCollection = new SceneCollection(treeView1, "Collection");
            _applicationController = new ApplicationController();
            _applicationController.SetSceneCollection(sceneCollection);
            _applicationController.Initialize();

            scene = _applicationController.SceneController.Scene;
        }

        private void openGLControl1_OpenGLDraw_1(object sender, RenderEventArgs args)
        {
            _applicationController.RenderController.Render();
        }

		#endregion

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
                    scene.IsObjectRotate = true;
                    break;
            }
        }


        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    scene.IsObjectRotate = false;
                    break;
            }
        }



        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool baseResult = base.ProcessCmdKey(ref msg, keyData);
            Scene scene = _applicationController.SceneController.Scene;
            CommandsHistory commandsHistory = _applicationController.CommandsHistory;

            if (keyData == Keys.Tab)
            {
				if (mode_comboBox.SelectedIndex == 1)
				{
					scene.SceneMode = SceneMode.VIEW;
                    mode_comboBox.SelectedItem = mode_comboBox.Items[0];
				}
				else if (mode_comboBox.SelectedIndex == 0)
				{
					scene.SceneMode = SceneMode.EDIT;
					mode_comboBox.SelectedItem = mode_comboBox.Items[1];
				}
				//scene.Update();
                _applicationController.SceneController.UpdateScene();

				return true;
            }
            else if (keyData == (Keys.Control | Keys.Z))
            {
                if (!commandsHistory.IsEmpty())
                {
                    commandsHistory.Peek()?.Undo();
                    commandsHistory.StepBackward();
                }
            }
            else if (keyData == (Keys.Control | Keys.Shift | Keys.Z))
            {
                if (!commandsHistory.IsEmpty())
                {
                    commandsHistory.StepForward();
                    commandsHistory.Peek()?.Redo();
                }
            }

            return baseResult;
        }

		#endregion

		#region ---- Mouse Events ----

		private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            _applicationController.MouseController.UpdateMousePosition(e.X, e.Y);
            _applicationController.HandleMouseDown(e.Button);
        }

        public void DeselectAndSaveCommand(Scene scene, ISceneObject obj)
        {
            DeselectionCommand deselectionCommand = new DeselectionCommand(_applicationController, obj);
            deselectionCommand.Execute();
            _applicationController.CommandsHistory.Push(deselectionCommand);
        }

        public void SelectAndSaveCommand(Scene scene, ISceneObject obj)
        {
            SelectionCommand selectionCommand = new SelectionCommand(_applicationController, obj);
            selectionCommand.Execute();
            _applicationController.CommandsHistory.Push(selectionCommand);
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            _applicationController.HandleMouseMove(e.X, e.Y);
		}

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            _applicationController.HandleMouseUp();
		}

		private void openGLControl_MouseWheel(object sender, MouseEventArgs e)
		{
            _applicationController.RenderController.Camera.ZoomBy(e.Delta);
		}

		#endregion

		#region ---- RightClick ----

		private void Select_Object_click(object sender, EventArgs e)
		{
            _applicationController.SelectElement();
		}

		private void Deselect_Object_click(object sender, EventArgs e)
		{
            _applicationController.DeselectElement();
		}

		private void Divide_Object_click(object sender, EventArgs e)
		{
            _applicationController.DivideElement();
		}

        private void Unite_Object_click(object sender, EventArgs e)
        {
            _applicationController.UniteElement();
        }

        private void Delete_Object_click(object sender, EventArgs e)
        {
            _applicationController.DeleteElement();
        }

        private void Attach_Object_click(object sender, EventArgs e)
        {
            _applicationController.MakeAttachableElement();
        }

        private void Detach_Object_click(object sender, EventArgs e)
        {
            _applicationController.MakeNonAttachableElement();
        }

        private void SetTarget_Object_click(object sender, EventArgs e)
        {
            _applicationController.MakeTargetableElement();
        }

        private void NotSetTarget_Object_click(object sender, EventArgs e)
        {
            _applicationController.MakeNonTargetableElement();
        }

        #endregion

        private void cubeToolStripMenuItem_Click(object sender, EventArgs e)
		{
            _applicationController.AddNewCubeElement(new Point3D(0, 0, 0));
		}

		private void mode_comboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
            if(mode_comboBox.SelectedIndex == 0)
            {
                scene.SceneMode = SceneMode.VIEW;
            }
            else if (mode_comboBox.SelectedIndex == 1)
            {
                scene.SceneMode = SceneMode.EDIT;
			}
			_applicationController.SceneController.UpdateScene();
		}

        private void checkBox_DrawFacets_CheckedChanged(object sender, EventArgs e)
        {
            scene.DrawFacets = checkBox_DrawFacets.Checked;
            //_applicationController.RenderController.DrawFacets = checkBox_DrawFacets.Checked;
        }

		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
            scene.DeselectAll();

            TreeNode selectedTreeNode = sceneCollection.GetSelectedNode();
            if(selectedTreeNode != null)
            {
                ISceneObject obj = sceneCollection.GetObjectByNode(selectedTreeNode, scene.ObjectCollection);
                if(obj != null)
                {
                    scene.SelectedObject = obj;
                    obj.Select();
                    //scene.InitSelectingCoordAxes(nodeObjects[0], 2.8f, 1.0);
                    AxisSystem axisSystem = new AxisSystem(obj.GetCenterPoint(), RenderController.selectingRay);
                    scene.ObjectCollection.Insert(0, axisSystem);
				}
            }
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
				string exportString = scene.Export();

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
                    scene.Import(lines);
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
            SettingsForm settingsForm = new SettingsForm();
            DialogResult result = settingsForm.ShowDialog();
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
                scene.Import(lines);
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
                string exportString = scene.Export();

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
