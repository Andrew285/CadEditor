using CadEditor.Controllers;
using CadEditor.MeshObjects;
using CadEditor.Properties;
using CadEditor.Settings;
using CadEditor.View.Forms;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace CadEditor
{

    public partial class Form1 : Form
    {
        private static Form1 instance;
        private Library library;
        private static Scene scene;
        private ContextMenuStrip contextMenuStrip;

        private static ToolStripMenuItem selectItem = new ToolStripMenuItem("Select Object");
        private static ToolStripMenuItem deselectItem = new ToolStripMenuItem("Deselect Object");
        private static ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete Object");
        private static ToolStripMenuItem attachItem = new ToolStripMenuItem("Attach Object");
        private static ToolStripMenuItem detachItem = new ToolStripMenuItem("Detach Object");
        private static ToolStripMenuItem setTargetItem = new ToolStripMenuItem("Set as Target");
        private static ToolStripMenuItem notSetTargetItem = new ToolStripMenuItem("Deselect Target");
        private static ToolStripMenuItem divideItem = new ToolStripMenuItem("Divide");
        
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

            //Load Controls
            contextMenuStrip = openGLControl1.ContextMenuStrip;
            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.AddRange(new ToolStripMenuItem[]
            {
                selectItem, deselectItem, deleteItem, divideItem, attachItem, detachItem, setTargetItem, notSetTargetItem
            });

            mode_comboBox.Items.AddRange(new string[] { "View Mode", "Edit Mode" });
            mode_comboBox.SelectedItem = mode_comboBox.Items[0];
            checkBox_DrawFacets.Checked = true;

            //Set Events
            GraphicsGL.Control.MouseWheel += new MouseEventHandler(openGLControl_MouseWheel);

            selectItem.Click += Select_Object_click;
            selectItem.Image = Resources.select_object;

            deselectItem.Click += Deselect_Object_click;
            deselectItem.Image = Resources.deselect_object;

            deleteItem.Click += Delete_Object_click;
            deleteItem.Image = Resources.remove;

            attachItem.Click += Attach_Object_click;
            detachItem.Click += Detach_Object_click;
            setTargetItem.Click += SetTarget_Object_click;
            notSetTargetItem.Click += NotSetTarget_Object_click;
            divideItem.Click += Divide_Object_click;
        }

		#region ---- OpenGLControl Events ----

		private void openGLControl1_OpenGLInitialized_1(object sender, EventArgs e)
        {
            GraphicsGL.CreateInstance(openGLControl1);
            GraphicsGL.GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);

            //Initializing fundemental objects of scene
            Camera camera = new Camera();
            SceneCollection sceneCollection = new SceneCollection(treeView1, "Collection");
            scene = new Scene(camera, sceneCollection)
            {
                DrawFacets = checkBox_DrawFacets.Checked
            };

            //Initializing objects by default
            scene.InitializeObjects();

            library = new Library();
        }

        private void openGLControl1_OpenGLDraw_1(object sender, RenderEventArgs args)
        {
            scene.Draw();
        }

		#endregion

		#region ---- Key Events ----

		private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.X:
                    if (scene.AttachingAxisSystem != null)
                    {
                        KeyX_Clicks = ClickKeyAxes(CoordinateAxis.X, KeyX_Clicks);
                    }
                    break;

                case Keys.Y:
                    if (scene.AttachingAxisSystem != null)
                    {
                        KeyY_Clicks = ClickKeyAxes(CoordinateAxis.Y, KeyY_Clicks);
                    }

                    break;

                case Keys.Z:
                    if (scene.AttachingAxisSystem != null)
                    {
                        KeyZ_Clicks = ClickKeyAxes(CoordinateAxis.Z, KeyZ_Clicks);
                    }

                    break;

                case Keys.Space:
                    scene.AttachCubes();
                    break;
            }
        }

        private int ClickKeyAxes(CoordinateAxis axis, int clicks)
        {
            List<Axis> axes = scene.AttachingAxisSystem.GetAxes(axis);
            scene.SetAttachingObjectToAxis(axes[clicks%axes.Count]);
            clicks = clicks == 1 ? 0 : 1;
            return clicks;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool baseResult = base.ProcessCmdKey(ref msg, keyData);

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
				scene.Update();

				return true;
            }
            else if (keyData == (Keys.Control | Keys.Z))
            {
                ActionHistoryController.GetInstance().InvokePreviousAction();
            }

            return baseResult;
        }

		#endregion

		#region ---- Mouse Events ----

		private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseController.X = e.X;
            MouseController.Y = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                GraphicsGL.DisableContexMenu();
                scene.Select();

                if(scene.SelectedObject == null)
                {
                    ISceneObject obj = scene.GetPreviousSelectedObject();
                    if (obj != null)
                    {
                        ActionHistoryController.GetInstance().AddAction(SceneAction.DESELECT, obj);
                    }
                    scene.DeselectAll();
                }
                else
                {
                    ActionHistoryController.GetInstance().AddAction(SceneAction.SELECT, scene.SelectedObject);
                }

                if (scene.SelectedAxisCube != null)
                {
                    ActionHistoryController.GetInstance().MovingInstance.StartPoint =
                        scene.SelectedObject.GetCenterPoint().Clone();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                GraphicsGL.DisableContexMenu();
                scene.Select();

                if (scene.SelectedObject != null)
                {
                    InitContextMenu(MouseController.X, MouseController.Y);
                }
			}
            else if(e.Button == MouseButtons.Middle)
            {
				MouseController.IsMiddleButtonPressed = true;
			}
        }

        private void InitContextMenu(int x, int y)
        {
            if (scene.SelectedObject != null) 
            {
                if (scene.SelectedObject is IDivideable)
                {
                    divideItem.Visible = true;
                }
                else
                {
                    divideItem.Visible = false;
                }


                if (scene.AttachingController.IsAttaching(scene.SelectedObject))
                {
                    detachItem.Visible = true;
                    attachItem.Visible = false;
                    setTargetItem.Visible = false;
                    notSetTargetItem.Visible = false;
                }
                else if (scene.AttachingController.IsEmpty())
                {
                    attachItem.Visible = true;
                    setTargetItem.Visible = true;
                    detachItem.Visible = false;
                    notSetTargetItem.Visible = false;
                }
                else if (scene.AttachingController.IsTarget(scene.SelectedObject))
                {
                    notSetTargetItem.Visible = true;
                    setTargetItem.Visible = false;
                    attachItem.Visible = false;
                    detachItem.Visible = false;
                }
                else if (!scene.AttachingController.IsTarget(scene.SelectedObject) &&
                         scene.AttachingController.GetTargetObject() == null)
                {
                    setTargetItem.Visible = true;
                    notSetTargetItem.Visible = false;
                    attachItem.Visible = false;
                    detachItem.Visible = false;
                }
                else if (!scene.AttachingController.IsAttaching(scene.SelectedObject) &&
                         scene.AttachingController.GetAttachingObject() == null)
                {
                    attachItem.Visible = true;
                    detachItem.Visible = false;
                    notSetTargetItem.Visible = false;
                    setTargetItem.Visible= false;
                }
            }

            contextMenuStrip.Show(openGLControl1, new System.Drawing.Point(x, y));
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
			if (MouseController.IsMiddleButtonPressed)
            {
                //scene.Camera.Update(e.X, e.Y);
                scene.Camera.UpdateRotation(e.X, e.Y);
			}

            //move selected objects towards the selected axis
            if(scene.SelectedAxisCube != null)
            {
                double sensitivityLevel = 0.01;
                double value = MouseController.GetHorizontalAngle(e.X) * sensitivityLevel;
                double valueY = MouseController.GetVerticalAngle(e.Y) * sensitivityLevel;
                Vector coords = new Vector(3);

                if (scene.SelectedAxisCube.Axis == CoordinateAxis.X)
                {
                    Scene.ActiveMovingAxis = CoordinateAxis.X;
                    coords = new Vector(value, 0, 0);
                }
                else if(scene.SelectedAxisCube.Axis == CoordinateAxis.Y)
                {
                    Scene.ActiveMovingAxis = CoordinateAxis.Y;
                    coords = new Vector(0, -valueY, 0);
				}
				else if(scene.SelectedAxisCube.Axis == CoordinateAxis.Z)
                {
                    Scene.ActiveMovingAxis = CoordinateAxis.Z;
                    coords = new Vector(0, 0, value);
				}

                Scene.MovingVector = coords;

                scene.SelectedObject.Move(coords);
                ActionHistoryController.GetInstance().AddAction(SceneAction.MOVE, scene.SelectedObject, coords);
                scene.MoveCoordinateAxes(coords);
            }

            MouseController.X = e.X;
			MouseController.Y = e.Y;
		}

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            MouseController.IsMiddleButtonPressed = false;

            if (scene.SelectedAxisCube != null)
            {
                scene.SelectedAxisCube.Deselect();
                scene.SelectedAxisCube = null;

                ActionHistoryController controller = ActionHistoryController.GetInstance();
                controller.MovingInstance.EndPoint =
                    scene.SelectedObject.GetCenterPoint().Clone();

                controller.AddAction(SceneAction.MOVE, scene.SelectedObject, controller.MovingInstance.GetMovingVector());
            }
		}

		private void openGLControl_MouseWheel(object sender, MouseEventArgs e)
		{
            //Increase or decrease view by zooming
            scene.Camera.Zoom(e.Delta);

            // Limit the camera distance to a reasonable range
            scene.Camera.LimitDistance();

            // Redraw the viewport
            GraphicsGL.Invalidate();
		}

		#endregion

		#region ---- RightClick ----

		private static void Select_Object_click(object sender, EventArgs e)
		{
            scene.SelectedObject.Select();
		}

		private static void Deselect_Object_click(object sender, EventArgs e)
		{
            scene.SelectedObject.Deselect();
		}

		private static void Divide_Object_click(object sender, EventArgs e)
		{
			if (scene.SelectedObject != null)
			{
				IDivideable divideable = (IDivideable)scene.SelectedObject;
                DividingCubeForm form = InitializeDividingForm();

				DialogResult result = form.ShowDialog();

				if (result == DialogResult.OK)
				{
					Vector nValues = form.nValues;
                    divideable.Divide(nValues);
				}
			}
			else
			{
				Output.ShowMessageBox("Warning", "There is no selected cube");
			}
		}

        private static void Delete_Object_click(object sender, EventArgs e)
        {
            scene.DeleteCompletely(scene.SelectedObject);
        }

        private static void Attach_Object_click(object sender, EventArgs e)
        {
            scene.AttachingController.DoAttach(scene.SelectedObject);
        }

        private static void Detach_Object_click(object sender, EventArgs e)
        {
            scene.AttachingController.DoDetach();
        }

        private static void SetTarget_Object_click(object sender, EventArgs e)
        {
            scene.AttachingController.DoSetTarget(scene.SelectedObject);
            scene.InitializeAttachingAxes((MeshObject3D)scene.SelectedObject);
        }

        private static void NotSetTarget_Object_click(object sender, EventArgs e)
        {
            scene.AttachingController.DoNotSetTarget();
            scene.ObjectCollection.Remove(scene.AttachingAxisSystem);
        }

        private static DividingCubeForm InitializeDividingForm()
        {
            DividingCubeForm form = new DividingCubeForm();
            form.TopMost = true;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            return form;
        }



        #endregion

        private void cubeToolStripMenuItem_Click(object sender, EventArgs e)
		{
            scene.AddCube();
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
			scene.Update();
		}

        private void checkBox_DrawFacets_CheckedChanged(object sender, EventArgs e)
        {
            scene.DrawFacets = checkBox_DrawFacets.Checked;
        }

		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
            scene.DeselectAll();

            TreeNode selectedTreeNode = scene.SceneCollection.GetSelectedNode();
            if(selectedTreeNode != null)
            {
                ISceneObject obj = scene.SceneCollection.GetObjectByNode(selectedTreeNode, scene.ObjectCollection);
                if(obj != null)
                {
                    scene.SelectedObject = obj;
                    obj.Select();
                    //scene.InitSelectingCoordAxes(nodeObjects[0], 2.8f, 1.0);
                    AxisSystem axisSystem = new AxisSystem(obj);
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
            LibraryForm libraryForm = new LibraryForm(library.GetAllSaves());
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
                library.AddSave(bmp, filePath, nameOfSave);
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

    }
}
