using CadEditor.Graphics;
using CadEditor.MeshObjects;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CadEditor
{

    public partial class Form1 : Form
    {
        private static Scene scene;
        private static ContextMenu contextMenu;

        private static MenuItem selectItem = new MenuItem("Select Object", Select_Object_click);
        private static MenuItem deselctItem = new MenuItem("Deselect Object", Deselect_Object_click);
        private static MenuItem divideItem = new MenuItem("Divide", Divide_Object_click);
        private static MenuItem deleteItem = new MenuItem("Delete", Delete_Object_click);
        private static MenuItem attachItem = new MenuItem("Attach", Attach_Object_click);
        private static MenuItem detachItem = new MenuItem("Detach", Detach_Object_click);
        private static MenuItem setTargetItem = new MenuItem("Set as Target", SetTarget_Object_click);



        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;

            contextMenu = InitContextMenu();
            mode_comboBox.Items.AddRange(new string[] { "View Mode", "Edit Mode" });
            mode_comboBox.SelectedItem = mode_comboBox.Items[0];
            checkBox_DrawFacets.Checked = true;

            GraphicsGL.Control.MouseWheel += new MouseEventHandler(openGLControl_MouseWheel);
		}

        public ContextMenu InitContextMenu()
        {
            if(contextMenu == null)
            {
                openGLControl1.ContextMenu = new ContextMenu();

                openGLControl1.ContextMenu.MenuItems.AddRange(new MenuItem[]
                {
                    selectItem, deselctItem, divideItem, deleteItem, attachItem
                });

                contextMenu = openGLControl1.ContextMenu;
            }

            return contextMenu;
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
        }

        private void openGLControl1_OpenGLDraw_1(object sender, RenderEventArgs args)
        {
            scene.Draw();
        }

        private void openGLControl1_Resized_1(object sender, EventArgs e)
        {
            // Set up the projection matrix
            GraphicsGL.SetUpProjectionMatrix();

            // Set up the view matrix
            GraphicsGL.SetUpViewMatrix(scene.Camera);
        }

		#endregion

		#region ---- Key Events ----

		private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Action<double> updateCamera = null;
            float value = 0;

            switch(e.KeyCode)
            {
                case Keys.A:
                    value = -3.0f;
                    updateCamera = scene.Camera.UpdateAxisY;
                    break;

                case Keys.D:
                    value = 3.0f;
                    updateCamera = scene.Camera.UpdateAxisY;
                    break;

                case Keys.W:
                    value = -3.0f;
                    updateCamera = scene.Camera.UpdateAxisX;
                    break;

                case Keys.S:
                    value = 3.0f;
                    updateCamera = scene.Camera.UpdateAxisX;
                    break;
            }

            updateCamera(value);
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
                    scene.DeselectAll();
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
            if (scene.AttachingPair.Count == 0)
            {
                contextMenu.MenuItems.Remove(detachItem);
                contextMenu.MenuItems.Add(attachItem);
            }
            else if(scene.AttachingPair.Contains(scene.SelectedObject) && scene.AttachingPair.Count == 1)
            {
                contextMenu.MenuItems.Add(detachItem);
                contextMenu.MenuItems.Remove(attachItem);

            }
            else if(scene.AttachingPair.Count == 1)
            {
                contextMenu.MenuItems.Remove(attachItem);
                contextMenu.MenuItems.Remove(detachItem);
                contextMenu.MenuItems.Add(setTargetItem);
            }
            else if(scene.AttachingPair.Count == 2)
            {
                contextMenu.MenuItems.Remove(detachItem);
                contextMenu.MenuItems.Remove(setTargetItem);
                contextMenu.MenuItems.Add(attachItem);
            }

            contextMenu.Show(openGLControl1, new System.Drawing.Point(x, y));
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
			if (MouseController.IsMiddleButtonPressed)
            {
                scene.Camera.Update(e.X, e.Y);
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
			if (scene.SelectedObject != null && scene.SelectedObject is MeshObject3D)
			{
				ComplexCube customCube = (ComplexCube)scene.SelectedObject;
                DividingCubeForm form = InitializeDividingForm();

				DialogResult result = form.ShowDialog();

				if (result == DialogResult.OK)
				{
					Vector nValues = form.nValues;
					customCube.Divide(nValues);
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
            scene.AttachingPair.Add(scene.SelectedObject);
            ((MeshObject3D)scene.SelectedObject).EdgeSelectedColor = Color.Green;
            ((MeshObject3D)scene.SelectedObject).EdgeNonSelectedColor = Color.Green;
        }

        private static void Detach_Object_click(object sender, EventArgs e)
        {
            scene.AttachingPair.Remove(scene.SelectedObject);
            ((MeshObject3D)scene.SelectedObject).EdgeSelectedColor = Color.Red;
            ((MeshObject3D)scene.SelectedObject).EdgeNonSelectedColor = Color.Black;
        }

        private static void SetTarget_Object_click(object sender, EventArgs e)
        {
            scene.AttachingPair.Add(scene.SelectedObject);
            ((MeshObject3D)scene.SelectedObject).EdgeSelectedColor = Color.Blue;
            ((MeshObject3D)scene.SelectedObject).EdgeNonSelectedColor = Color.Blue;
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

            TreeNode selectedTreeNode = treeView1.SelectedNode;
            if(selectedTreeNode != null)
            {
                List<ISceneObject> nodeObjects = scene.SceneCollection.FindObjectByTreeNode(selectedTreeNode, scene);
                if(nodeObjects != null)
                {
                    if(nodeObjects.Count > 1)
                    {
						foreach (ISceneObject graphicsObject in nodeObjects)
						{
							graphicsObject.Select();
						}
					}
                    else
                    {
                        scene.SelectedObject = nodeObjects[0];
						nodeObjects[0].Select();
                        //scene.InitSelectingCoordAxes(nodeObjects[0], 2.8f, 1.0);
                        AxisSystem axisSystem = new AxisSystem(nodeObjects[0]);
                        scene.ObjectCollection.Insert(0, axisSystem);
					}
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


        //Set view by axis
        private void viewByXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scene.Camera.SetViewByAxis(CoordinateAxis.X);
        }

        private void viewYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scene.Camera.SetViewByAxis(CoordinateAxis.Y);
        }

        private void viewZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scene.Camera.SetViewByAxis(CoordinateAxis.Z);
        }

    }
}
