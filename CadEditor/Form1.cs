using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Effects;
using SharpGL.SceneGraph.Lighting;
using SharpGL.SceneGraph.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace CadEditor
{

    public partial class Form1 : Form
    {
        private OpenGL gl;
        private Scene scene;

        private CustomCube selectedCube;
        private int mouseX;
        private int mouseY;
		private bool isMiddleButtonPressed;
        private ISelectable SelectedFacetViewMode;
        private Vertex SelectedVertexEditMode;
		private float sensitivity = 0.5f;

		public Form1()
        {
            InitializeComponent();
			KeyPreview = true;

            mode_comboBox.Items.AddRange(new string[] { "View Mode", "Edit Mode" });
            mode_comboBox.SelectedItem = mode_comboBox.Items[0];


			openGLControl1.MouseWheel += new MouseEventHandler(openGLControl_MouseWheel);
		}

        private void openGLControl1_OpenGLInitialized_1(object sender, EventArgs e)
        {
            gl = openGLControl1.OpenGL;

            //Initializing fundemental objects of scene
            Camera camera = new Camera(gl, new Vector(new double[]{ 0, 0, 0}));
            SceneCollection sceneCollection = new SceneCollection(treeView1, "Collection");
            scene = new Scene(gl, camera, sceneCollection);

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
            //  Возьмём OpenGL объект
            OpenGL gl = openGLControl1.OpenGL;

            //  Зададим матрицу проекции
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Единичная матрица для последующих преобразований
            gl.LoadIdentity();

            //  Преобразование
            gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);

            ////  Данная функция позволяет установить камеру и её положение
            gl.LookAt(2, 2, scene.Camera.CameraDistance,    // позиция самой камеры (x, y, z)
                        0, 0, 0,     // направление, куда мы смотрим
                        0, 1, 0);    // верх камеры

            //  Зададим модель отображения
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

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

		private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
				openGLControl1.ContextMenu = null;
                scene.CheckSelectedCoordinateAxes();
				ISelectable selectedObject =  scene.CheckSelectedElement(e.X, openGLControl1.Height - e.Y, gl);

                //if (selectedObject is Facet && scene.SceneMode == SceneMode.VIEW)
                //{
                //    SelectedFacetViewMode = selectedObject;
                //}

                if(selectedObject is Vertex && scene.SceneMode == SceneMode.EDIT)
                {
                    SelectedVertexEditMode = (Vertex)selectedObject;
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

                    openGLControl1.ContextMenu.Show(openGLControl1, new Point(e.X, e.Y));
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
            else if(SelectedVertexEditMode != null)
            {
                SelectedVertexEditMode.X += (float)(horizontalAngle * 0.01);
                SelectedVertexEditMode.Y -= (float)(verticalAngle * 0.01);
                Console.WriteLine(String.Format("{0} += {1}", SelectedVertexEditMode.X, (float)horizontalAngle));
                Console.WriteLine(String.Format("{0} += {1}", SelectedVertexEditMode.Y, (float)verticalAngle));
			    
            }

			mouseX = e.X;
			mouseY = e.Y;

			//else if (SelectedFacetViewMode != null && scene.SceneMode == SceneMode.VIEW)
			//{
			//    Facet facet = (Facet)SelectedFacetViewMode;
			//    Vector facetNormal = facet.CalculateNormal();

			//    if (Math.Abs(facetNormal[0]) == 1)
			//    {

			//    }
			//}
		}

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            isMiddleButtonPressed = false;
            SelectedVertexEditMode = null;
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

		private void Select_Object_click(object sender, EventArgs e)
		{
            selectedCube.SelectAll();
		}

		private void Deselect_Object_click(object sender, EventArgs e)
		{
			selectedCube.DeselectAll();
		}

		private void Delete_Object_click(object sender, EventArgs e)
		{
		    scene.DeleteCompletely(selectedCube);
		}

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
		}
	}
}
