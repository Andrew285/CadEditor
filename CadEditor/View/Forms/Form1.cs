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
        private Library library;
        private static Scene scene;
        //private ContextMenuStrip contextMenuStrip;
        private SceneCollection sceneCollection;
        public AxisSystem AttachingAxisSystem { get; private set; }
        private Camera camera;
        private CommandsHistory commandsHistory;
        private Point3D startMovePoint;
        private Point3D endMovePoint;
        private ISceneObject selectedObject;
        private ISceneObject prevObject;
        private AxisCube axisCube;

        private ComplexCube targetCube;
        private ComplexCube attachingCube;
        private CoordinateAxisType targetFacetAxis;
        private CoordinateAxisType attachingFacetAxis;

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
            camera = new Camera();
            sceneCollection = new SceneCollection(treeView1, "Collection");
            scene = new Scene(sceneCollection)
            {
                DrawFacets = checkBox_DrawFacets.Checked
            };
            commandsHistory = new CommandsHistory();

            _applicationController = new ApplicationController();
            _applicationController.SetScene(scene);
            _applicationController.SetSceneCollection(sceneCollection);
            _applicationController.SetCommandHistory(commandsHistory);
            _applicationController.SetCamera(camera);

            //Initializing objects by default
            ComplexCube cube = new ComplexCube(new Point3D(6, 0, 5), new Vector(1, 1, 1), ModelNameProvider.GetInstance().GetNextName(ModelTypes.COMPLEX_CUBE));
            ComplexCube cube2 = new ComplexCube(new Point3D(5, 5, 8), new Vector(1, 1, 1), ModelNameProvider.GetInstance().GetNextName(ModelTypes.COMPLEX_CUBE));
            scene.AddObject(cube);
            scene.AddObject(cube2);

            sceneCollection.AddCube(cube);
            sceneCollection.AddCube(cube2);

            Point3D centerPoint = new Point3D(0, 0, 0);
            camera.SetTarget(centerPoint.X, centerPoint.Y, centerPoint.Z);
            library = new Library();
        }

        public AxisSystem InitializeAttachingAxes(MeshObject3D obj)
        {
            AttachingAxisSystem = new AxisSystem();
            AttachingAxisSystem.AxisLength = 5.0f;
            for (int i = 0; i < obj.Mesh.Facets.Count; i++)
            {
                if (!obj.Mesh.attachedFacets.Contains(i))
                {
                    AttachingAxisSystem.CreateAxis(obj.Mesh.Facets[i].AxisType, obj.Mesh.Facets[i].GetCenterPoint());
                }
            }

            return AttachingAxisSystem;
        }

        private void openGLControl1_OpenGLDraw_1(object sender, RenderEventArgs args)
        {
            GraphicsGL.GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Set up the projection matrix
            GraphicsGL.SetUpProjectionMatrix();

            //// Set up the view matrix
            GraphicsGL.SetUpViewMatrix(camera);

            //Rotate Camera
            camera.Rotate();

            scene.Draw();
            scene.DrawSelectingRay(camera.Position);
        }

		#endregion

		#region ---- Key Events ----

		private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.X:
                    if (AttachingAxisSystem != null)
                    {
                        KeyX_Clicks = ClickKeyAxes(CoordinateAxis.X, KeyX_Clicks);
                    }
                    break;

                case Keys.Y:
                    if (AttachingAxisSystem != null)
                    {
                        KeyY_Clicks = ClickKeyAxes(CoordinateAxis.Y, KeyY_Clicks);
                    }

                    break;

                case Keys.Z:
                    if (AttachingAxisSystem != null)
                    {
                        KeyZ_Clicks = ClickKeyAxes(CoordinateAxis.Z, KeyZ_Clicks);
                    }

                    break;

                case Keys.Space:
                    AttachingCommand attachingCommand = new AttachingCommand(scene, targetCube, attachingCube, targetFacetAxis, attachingFacetAxis);
                    attachingCommand.Execute();
                    commandsHistory.Push(attachingCommand);
                    ComplexStructure complexStructure = attachingCommand.GetComplexStructure();
                    scene.Remove(AttachingAxisSystem);

                    if (complexStructure != null && !scene.Contains(complexStructure))
                    {
                        scene.Remove(targetCube);
                        scene.Remove(attachingCube);
                        DetachObject();
                        NotSetTargetObject();
                        scene.Add(complexStructure);
                        sceneCollection.AddComplexStructure(complexStructure);
                    }
                    else
                    {
                        sceneCollection.RemoveCube(attachingCube);
                        sceneCollection.AddCube(attachingCube, complexStructure);
                        ComplexCube attachingCubeCopy = attachingCube;
                        DetachObject();
                        NotSetTargetObject();
                        scene.Remove(targetCube);
                        scene.Remove(attachingCubeCopy);
                    }
                    AttachingAxisSystem = null;
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

        private int ClickKeyAxes(CoordinateAxis axis, int clicks)
        {
            List<Axis> axes = AttachingAxisSystem.GetAxes(axis);
            if (axes.Count > 0)
            {
                SetAttachingObjectToAxis(axes[clicks % axes.Count]);
            }
            clicks = clicks == 1 ? 0 : 1;
            return clicks;
        }

        public void SetAttachingObjectToAxis(Axis targetAxis)
        {
            Point3D pointToMove = targetAxis.P2;

            //Create AttachingFacetsPair
            foreach (Plane facet in targetCube.Mesh.Facets)
            {
                if (facet.GetCenterPoint() == targetAxis.P1)
                {
                    facet.IsAttached = true;
                    targetFacetAxis = facet.AxisType;
                    break;
                }
            }

            CoordinateAxisType oppositeType = AxisSystem.GetOppositeAxisType(targetFacetAxis);
            foreach (Plane facet in attachingCube.Mesh.Facets)
            {
                if (facet.AxisType == oppositeType)
                {
                    facet.IsAttached = true;
                    attachingFacetAxis = facet.AxisType;
                    break;
                }
            }

            Vector distanceVector = attachingCube.GetCenterPoint() - pointToMove;
            attachingCube.Move(distanceVector * (-1));
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
            MouseController.X = e.X;
            MouseController.Y = e.Y;

            prevObject = selectedObject;
            selectedObject = scene.Select();
            if (e.Button == MouseButtons.Left)
            {
                GraphicsGL.DisableContexMenu();

                CommandList selectionCommandList = new CommandList();

                if (selectedObject != null)
                {
                    if (selectedObject is AxisCube)
                    {
                        SelectionCommand selectionCommand = new SelectionCommand(scene, selectedObject);
                        DeselectionCommand deselectionCommand = new DeselectionCommand(scene, prevObject);
                        DeleteAxesCommand deleteAxesCommand = new DeleteAxesCommand(scene, prevObject);
                        selectionCommand.Execute();
                        deselectionCommand.Execute();
                        commandsHistory.Push(new CommandList(new List<ICommand> { deselectionCommand, deleteAxesCommand}));

                        axisCube = selectedObject as AxisCube;
                        startMovePoint = (Point3D)prevObject.GetCenterPoint().Clone();
                    }
                    else
                    {
                        if (prevObject != null && selectedObject != prevObject)
                        {
                            CommandList commandList = new CommandList(
                                new List<ICommand> {
                                new DeselectionCommand(scene, prevObject),
                                new DeleteAxesCommand(scene, prevObject)
                                });
                            commandList.Execute();
                            commandsHistory.Push(commandList);
                        }

                        selectionCommandList.AddRange(new List<ICommand>
                        {
                            new SelectionCommand(scene, selectedObject),
                            new InitAxesCommand(scene, selectedObject)
                        });
                    }
                }
                else
                {
                    if (prevObject != null)
                    {
                        selectionCommandList.AddRange(new List<ICommand>
                        {
                            new DeselectionCommand(scene, prevObject),
                            new DeleteAxesCommand(scene, prevObject)
                        });
                    }
                    else
                    {
                        scene.DeselectAll();
                    }
                }

                if (!selectionCommandList.IsEmpty())
                {
                    ISceneObject obj = ((UnaryCommand)selectionCommandList[0]).GetSceneObject();
                    selectionCommandList.Execute();

                    if (obj is ComplexCube || obj is ComplexStructure)
                    {
                        commandsHistory.Push(selectionCommandList);
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                GraphicsGL.DisableContexMenu();
                //scene.Select();

                if (selectedObject != null)
                {
                    SelectionCommand selectionCommand = new SelectionCommand(scene, selectedObject);
                    selectionCommand.Execute();
                    _applicationController.UIController.InitContextMenu(MouseController.X, MouseController.Y);
                }
			}
            else if(e.Button == MouseButtons.Middle)
            {
                MouseController.IsMiddleButtonPressed = true;
			}
        }

        public void DeselectAndSaveCommand(Scene scene, ISceneObject obj)
        {
            DeselectionCommand deselectionCommand = new DeselectionCommand(scene, obj);
            deselectionCommand.Execute();
            commandsHistory.Push(deselectionCommand);
        }

        public void SelectAndSaveCommand(Scene scene, ISceneObject obj)
        {
            SelectionCommand selectionCommand = new SelectionCommand(scene, obj);
            selectionCommand.Execute();
            commandsHistory.Push(selectionCommand);
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
			if (MouseController.IsMiddleButtonPressed && scene.IsObjectRotate && scene.SelectedObject != null && scene.SelectedObject.IsSelected && scene.SelectedObject is IRotateable)
            {
                scene.UpdateObjectRotation((IRotateable)scene.SelectedObject, e.X, e.Y);
            }
            else if (MouseController.IsMiddleButtonPressed)
            {
                camera.UpdateRotation(e.X, e.Y);
            }

            //move selected objects towards the selected axis
            if(axisCube != null)
            {
                double sensitivityLevel = 0.01;
                double value = MouseController.GetHorizontalAngle(e.X) * sensitivityLevel;
                double valueY = MouseController.GetVerticalAngle(e.Y) * sensitivityLevel;
                Vector coords = new Vector(3);

                if (axisCube.Axis == CoordinateAxis.X)
                {
                    Scene.ActiveMovingAxis = CoordinateAxis.X;
                    coords = new Vector(value, 0, 0);
                }
                else if(axisCube.Axis == CoordinateAxis.Y)
                {
                    Scene.ActiveMovingAxis = CoordinateAxis.Y;
                    coords = new Vector(0, -valueY, 0);
				}
				else if(axisCube.Axis == CoordinateAxis.Z)
                {
                    Scene.ActiveMovingAxis = CoordinateAxis.Z;
                    coords = new Vector(0, 0, value);
				}

                Scene.MovingVector = coords;

                prevObject.Move(coords);
                scene.MoveCoordinateAxes(coords);
            }

            MouseController.X = e.X;
			MouseController.Y = e.Y;
		}

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            MouseController.IsMiddleButtonPressed = false;

            if (axisCube != null)
            {
                DeselectionCommand deselectionCommand = new DeselectionCommand(scene, axisCube);
                deselectionCommand.Execute();
                axisCube = null;

                endMovePoint = (Point3D)prevObject.GetCenterPoint().Clone();
                Vector moveResult = endMovePoint - startMovePoint;
                MoveCommand moveCommand = new MoveCommand(scene, prevObject, moveResult);
                commandsHistory.Push(moveCommand);
            }
		}

		private void openGLControl_MouseWheel(object sender, MouseEventArgs e)
		{
            //Increase or decrease view by zooming
            camera.Zoom(e.Delta);

            // Limit the camera distance to a reasonable range
            camera.LimitDistance();

            // Redraw the viewport
            GraphicsGL.Invalidate();
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

        private void DetachObject()
        {
            attachingCube.EdgeSelectedColor = Color.Red;
            attachingCube.EdgeNonSelectedColor = Color.Black;
            attachingCube = null;
        }

        private void NotSetTargetObject()
        {
            targetCube.EdgeSelectedColor = Color.Red;
            targetCube.EdgeNonSelectedColor = Color.Black;
            targetCube = null;
            scene.ObjectCollection.Remove(AttachingAxisSystem);
        }

        #endregion

        private void cubeToolStripMenuItem_Click(object sender, EventArgs e)
		{
            _applicationController.AddNewCubeElement();
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
                    AxisSystem axisSystem = new AxisSystem(obj.GetCenterPoint(), Scene.selectingRay);
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

        private void generalTab_checkBoxDrawRay_CheckedChanged(object sender, EventArgs e)
        {
            scene.IsRayDrawable = generalTab_checkBoxDrawRay.Checked ? true : false;
        }

    }
}
