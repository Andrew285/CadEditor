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
using System.Numerics;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CadEditor
{

    public partial class Form1 : Form
    {
        private OpenGL gl;
        private Scene scene;

        private int mouseX;
        private int mouseY;
        private bool isLeftButtonPressed;

        public Form1()
        {
            InitializeComponent();
			KeyPreview = true;
            openGLControl1.MouseDown += new MouseEventHandler(this.openGLControl1_MouseDown);
        }

        private void openGLControl1_OpenGLInitialized_1(object sender, EventArgs e)
        {
            //  Возьмём OpenGL объект
            gl = openGLControl1.OpenGL;
            Camera camera = new Camera(gl, new Vector(new double[]{ 0, 0, 0}));
            scene = new Scene(gl, camera);

            scene.InitializeObjects();
        }

        private void openGLControl1_OpenGLDraw_1(object sender, RenderEventArgs args)
        {
            scene.DrawCube(openGLControl1.Width, openGLControl1.Height);
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
            gl.LookAt(2, 2, -6,    // позиция самой камеры (x, y, z)
                        0, 0, 0,     // направление, куда мы смотрим
                        0, 1, 0);    // верх камеры

            //  Зададим модель отображения
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                scene.ChangeAxisY(-3.0f);
            }
            else if (e.KeyCode == Keys.D)
            {
                scene.ChangeAxisY(3.0f);

            }
            else if (e.KeyCode == Keys.W)
            {
                scene.ChangeAxisX(-3.0f);

            }
            else if (e.KeyCode == Keys.S)
            {
                scene.ChangeAxisX(3.0f);
            }

            
        }

		private SceneElement selectedSceneElement = null;

		/// <summary>
		/// Gets or sets the selected scene element.
		/// </summary>
		/// <value>
		/// The selected scene element.
		/// </value>
		public SceneElement SelectedSceneElement
		{
			get { return selectedSceneElement; }
			set
			{
				selectedSceneElement = value;
			}
		}

		private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {


            if (e.Button == MouseButtons.Left)
            {
                //Vertex? vertex = scene.SelectObject(gl, MousePosition.X, MousePosition.Y);
                //CustomCube cube = new CustomCube(gl, 5);
                //scene.AddObject(cube);
                //scene.CreateRay(MousePosition.X, MousePosition.Y, camera);
                //CustomCube selectedCube = scene.SelectObject(MousePosition.X, MousePosition.Y, camera, openGLControl1);
                //scene.DrawRayToMouse(MousePosition.X, MousePosition.Y);
                //if (selectedCube != null)
                //{
                //    Console.WriteLine("User selected a cube");
                //}
                //else
                //{
                //    Console.WriteLine("User didn't select a cube");
                //}

                //SelectedSceneElement = null;
                //            int mouseX = MousePosition.X;
                //            int mouseY = MousePosition.Y;
                //var items = scene.DoHitTest(mouseX, mouseY);
                //            foreach(var item in items)
                //            {
                //                Console.WriteLine("The item is clicked");
                //            }


                //bool isSelected = scene.SelectElement(e.X, openGLControl1.Height - e.Y, gl);
                scene.SelectElement(e.X, openGLControl1.Height - e.Y, gl);
                //bool isSelected = scene.SelectElement(MousePosition.X, MousePosition.Y, gl);
                //if (isSelected)
                //{
                //    Console.WriteLine("True");
                //}
            }
            else if (e.Button == MouseButtons.Right)
            {
                mouseX = e.X;
                mouseY = e.Y;
                isLeftButtonPressed = true;
            }

        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            float sensitivity = 0.5f;


			if (isLeftButtonPressed)
            {
				double horizontalAngle = (e.X - mouseX) * sensitivity;
				double verticalAngle = (e.Y - mouseY) * sensitivity;

				scene.ChangeAxisY(horizontalAngle);
                scene.ChangeAxisX(verticalAngle);
                mouseX = e.X;
                mouseY = e.Y;

				//camera.UpdateCameraPosition(horizontalAngle, verticalAngle);
				scene.Camera.UpdateOpenGLCameraPosition(gl, horizontalAngle, verticalAngle);
			}

			//camera.UpdatePosition();
		}

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            isLeftButtonPressed = false;
        }
    }
}
