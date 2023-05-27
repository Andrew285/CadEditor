﻿using CadEditor.Graphics;
using SharpGL;
using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace CadEditor
{

    public partial class Form1 : Form
    {
        private OpenGL gl;
        private Scene scene;

        private ComplexCube selectedCube;
        private ComplexCube SelectedNonDrawableCube;
        private int mouseX;
        private int mouseY;
		private bool isMiddleButtonPressed;
        private AxisCube SelectedAxisCubeEditMode;
        private IGraphics SelectedObject;
		private float sensitivity = 0.5f;

		public Form1()
        {
            InitializeComponent();
			KeyPreview = true;

            mode_comboBox.Items.AddRange(new string[] { "View Mode", "Edit Mode" });
            mode_comboBox.SelectedItem = mode_comboBox.Items[0];
            checkBox_DrawFacets.Checked = true;

			openGLControl1.MouseWheel += new MouseEventHandler(openGLControl_MouseWheel);
		}

		#region ---- OpenGLControl Events ----

		private void openGLControl1_OpenGLInitialized_1(object sender, EventArgs e)
        {
            gl = openGLControl1.OpenGL;
            gl.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);

            //Initializing fundemental objects of scene
            Camera camera = new Camera(gl, new Vector(new double[]{ 0, 0, 0}));
            SceneCollection sceneCollection = new SceneCollection(treeView1, "Collection");
            scene = new Scene(gl, camera, sceneCollection);
            scene.DrawFacets = checkBox_DrawFacets.Checked;

            //Initializing objects by default
            scene.InitializeObjects();
        }

        private void openGLControl1_OpenGLDraw_1(object sender, RenderEventArgs args)
        {
            scene.DrawScene(openGLControl1.Width, openGLControl1.Height);
            SceneGrid.Init(gl);
        }

        private void openGLControl1_Resized_1(object sender, EventArgs e)
        {
            OpenGL gl = openGLControl1.OpenGL;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);
            gl.LookAt(2, 2, scene.Camera.CameraDistance,
                      0, 0, 0,
                      0, 1, 0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

		#endregion

		#region ---- Key Events ----

		private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                scene.Camera.UpdateAxisY(-3.0f);
            }
            else if (e.KeyCode == Keys.D)
            {
                scene.Camera.UpdateAxisY(3.0f);

            }
            else if (e.KeyCode == Keys.W)
            {
                scene.Camera.UpdateAxisX(-3.0f);

            }
            else if (e.KeyCode == Keys.S)
            {
                scene.Camera.UpdateAxisX(3.0f);
            }
            
        }

		private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{

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

    //        if (keyData == (Keys.Tab | Keys.Shift))
    //        {
				//return true;
    //        }

            return baseResult;
        }

		#endregion

		#region ---- Mouse Events ----

		private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
				openGLControl1.ContextMenu = null;

                IGraphics selectedCubeElement = scene.CheckSelectedElement(e.X, openGLControl1.Height - e.Y, gl);

                //check type of selected object
                if (selectedCubeElement != null)
                {
                    selectedCubeElement.Select();

                    if (selectedCubeElement is AxisCube)
                    {
                        SelectedAxisCubeEditMode = (AxisCube)selectedCubeElement;
                    }
                    else if (!(selectedCubeElement is AxisCube))
                    {
                        SelectedObject = selectedCubeElement;
                        SelectedAxisCubeEditMode = null;

                        if(SelectedObject is Point && ((Point)SelectedObject).ParentCube is ComplexCube)
                        {
							SelectedNonDrawableCube = scene.FindNonDrawableCube((ComplexCube)((Point)SelectedObject).ParentCube);
						}
                        else
                        {
                            SelectedNonDrawableCube = null;
                        }

						scene.InitSelectingCoordAxes(SelectedObject, 2.8f, 1.0);
                    }
                    else
                    {
                        scene.DeleteSelectingCoordAxes();
                    }
                }

            }
            else if (e.Button == MouseButtons.Right)
            {
				openGLControl1.ContextMenu = null;
				selectedCube = scene.GetSelectedCube(e.X, openGLControl1.Height - e.Y, gl);
                if(selectedCube != null)
                {
                    openGLControl1.ContextMenu = new ContextMenu();
                    openGLControl1.ContextMenu.MenuItems.Add("Select Object", Select_Object_click);
                    openGLControl1.ContextMenu.MenuItems.Add("Deselect Object", Deselect_Object_click);
                    openGLControl1.ContextMenu.MenuItems.Add("Delete", Delete_Object_click);

                    openGLControl1.ContextMenu.Show(openGLControl1, new System.Drawing.Point(e.X, e.Y));
                }
			}
            else if(e.Button == MouseButtons.Middle)
            {
				mouseX = e.X;
				mouseY = e.Y;
				isMiddleButtonPressed = true;
			}
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
			double horizontalAngle = (e.X - mouseX) * sensitivity;
			double verticalAngle = (e.Y - mouseY) * sensitivity;

			if (isMiddleButtonPressed)
            {
				scene.Camera.UpdateAxisY(horizontalAngle);
                scene.Camera.UpdateAxisX(verticalAngle);
			}

            //move selected objects towards the selected axis
            if(SelectedAxisCubeEditMode != null)
            {
                double sensitivityLevel = 0.01;
                double value = horizontalAngle * sensitivityLevel;
                int index = -1;


                //int temp = a;
                //a = b;
                //b = temp;

                //SelectedObject = NonDrawable;
                double[] coords = new double[3];
                if (SelectedAxisCubeEditMode.Axis == CoordinateAxis.X)
                {
                    coords = new double[] { value, 0, 0};
                    SelectedObject.Move(coords[0], coords[1], coords[2]);
					scene.MoveCoordinateAxes(coords[0], coords[1], coords[2]);
                    index = 0;
                }
                else if(SelectedAxisCubeEditMode.Axis == CoordinateAxis.Y)
                {
					coords = new double[] { 0, -value, 0 };
					SelectedObject.Move(coords[0], coords[1], coords[2]);
					scene.MoveCoordinateAxes(coords[0], coords[1], coords[2]);
                    index = 1;
				}
				else if(SelectedAxisCubeEditMode.Axis == CoordinateAxis.Z)
                {
					coords = new double[] { 0, 0, -value };
					SelectedObject.Move(coords[0], coords[1], coords[2]);
					scene.MoveCoordinateAxes(coords[0], coords[1], coords[2]);
                    index = 2;
				}

                if(index >= 0)
                {
                    int indexOfPoint = ((Point)SelectedObject).ParentCube.Mesh.GetIndexOfPoint((Point)SelectedObject);
                    Point p = SelectedNonDrawableCube.Mesh.Vertices[indexOfPoint];
                    p.Move(coords[0], coords[1], coords[2]);
					((ComplexCube)((Point)SelectedObject).ParentCube).Transform(index, SelectedNonDrawableCube);
				}
			}

			mouseX = e.X;
			mouseY = e.Y;
		}

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            isMiddleButtonPressed = false;

            if (SelectedAxisCubeEditMode != null)
            {
				SelectedAxisCubeEditMode.Deselect();
                SelectedAxisCubeEditMode = null;
			}
			//SelectedFacetViewMode = null;
		}

		private void openGLControl_MouseWheel(object sender, MouseEventArgs e)
		{
			// Calculate the new camera distance based on the mouse wheel delta
			scene.Camera.CameraDistance += e.Delta * -0.01f;

			// Limit the camera distance to a reasonable range
			scene.Camera.CameraDistance = Math.Max(scene.Camera.CameraDistance, 1.0f);
			scene.Camera.CameraDistance = Math.Min(scene.Camera.CameraDistance, 20.0f);

			// Redraw the viewport
			openGLControl1.Invalidate();
		}

		#endregion

		#region ---- RightClick ----

		private void Select_Object_click(object sender, EventArgs e)
		{
            selectedCube.Select();
		}

		private void Deselect_Object_click(object sender, EventArgs e)
		{
			selectedCube.Deselect();
		}

		private void Delete_Object_click(object sender, EventArgs e)
		{
		    scene.DeleteCompletely(selectedCube);
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

        private void openGLControl1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.TopMost = true;
        }

        private void checkBox_DrawFacets_CheckedChanged(object sender, EventArgs e)
        {
            scene.DrawFacets = checkBox_DrawFacets.Checked;
        }

        private void button_DivideCube_Click(object sender, EventArgs e)
        {
            if(SelectedObject != null && SelectedObject is ComplexCube)
            {
                ComplexCube customCube= (ComplexCube)SelectedObject;
                DividingCubeForm form = new DividingCubeForm();
                form.TopMost = true;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimizeBox = false;
                form.MaximizeBox = false;

                DialogResult result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    int[] nValues = form.nValues;
                    customCube.Divide(nValues);
                    scene.NonDrawableCubes.Add(customCube, customCube.Clone());
                }
            }
            else
            {
                Output.ShowMessageBox("Warning", "There is no selected cube");
            }
            
        }

        public static DialogResult InputBox()
        {
            Form form = new Form();
            Label label_Nx = new Label();
            Label label_Ny = new Label();
            Label label_Nz = new Label();
            TextBox textBox_Nx = new TextBox();
            TextBox textBox_Ny = new TextBox();
            TextBox textBox_Nz = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            form.Text = "Dividing Cube";
            label_Nx.Text = "N_x: ";
            label_Ny.Text = "N_y: ";
            label_Nz.Text = "N_z: ";

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            //Set Bounds for textBoxes and labels
            label_Nx.SetBounds(16, 26, 37, 13);
            label_Nx.AutoSize = true;
            textBox_Nx.SetBounds(56, 26, 50, 20);

            label_Ny.SetBounds(16, 56, 37, 13);
            label_Ny.AutoSize = true;
            textBox_Ny.SetBounds(56, 56, 50, 20);

            label_Nz.SetBounds(16, 86, 37, 13);
            label_Nz.AutoSize = true;
            textBox_Nz.SetBounds(56, 86, 50, 20);

            buttonOk.SetBounds(228, 160, 160, 60);
            buttonCancel.SetBounds(400, 160, 160, 60);

            form.ClientSize = new Size(796, 307);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.TopMost = true;

            form.Controls.AddRange(new Control[] { label_Nx, label_Ny, label_Nz, textBox_Nx, textBox_Ny, textBox_Nz, buttonOk, buttonCancel });
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();

            return dialogResult;
        }
    }
}
