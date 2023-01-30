using SharpGL;
using SharpGL.SceneGraph;
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
        
        public float rtri = 0;
        public float utri = 0;
        public CustomCube cube1;
        private ArcBallEffect arcBallEffect = new ArcBallEffect();
        public Cube cube;
        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;

            sceneControl1.MouseDown += new MouseEventHandler(Form1_MouseDown);
            sceneControl1.MouseMove += new MouseEventHandler(Form1_MouseMove);
            sceneControl1.MouseUp += new MouseEventHandler(sceneControl1_MouseUp);

            //  Add some design-time primitives.
            sceneControl1.Scene.SceneContainer.AddChild(new
                SharpGL.SceneGraph.Primitives.Grid());
            sceneControl1.Scene.SceneContainer.AddChild(new
                SharpGL.SceneGraph.Primitives.Axies());

            //  Create a light.
            Light light = new Light()
            {
                On = true,
                Position = new Vertex(3, 10, 3),
                GLCode = OpenGL.GL_LIGHT0
            };

            //  Add the light.
            sceneControl1.Scene.SceneContainer.AddChild(light);

            //  Create a sphere.
            cube = new Cube();
            cube.AddEffect(arcBallEffect);

            //  Add it.
            sceneControl1.Scene.SceneContainer.AddChild(cube);

            //  Add the root element to the tree.
            AddElementToTree(sceneControl1.Scene.SceneContainer, treeView1.Nodes);
        }

        private void openglControl1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void openglControl1_MouseMove(object sender, MouseEventArgs e)
        {
            //if (isRotationStarted)
            //{
            //    xRotAngle += e.Y - startY;
            //    yRotAngle += e.X - startX;

            //    //camAngleX += e.Y - startY;
            //    //camAngleY += e.X - startX;

            //    theta += e.X - startX;

            //    startX = e.X;
            //    startY = e.Y;
            //}

        }

        private void openglControl1_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Right)
            //{
            //    isRotationStarted = true;
            //    startX = e.X;
            //    startY = e.Y;
            //}
        }

        private void openGLControl1_OpenGLInitialized_1(object sender, EventArgs e)
        {
            //  Возьмём OpenGL объект
            OpenGL gl = openGLControl1.OpenGL;

            //  Устанавливаем цвет заливки по умолчанию (в данном случае цвет голубой)
            gl.ClearColor(0.1f, 0.5f, 1.0f, 0);

            //isRotationStarted = false;
            //startX = 0;
            //startY = 0;
            //xRotAngle = 15;
            //yRotAngle = 15;

            List<Vertex>cubeVertices1 = new List<Vertex>
            {
                //front side

                new Vertex(2, 0, 2),
                new Vertex(2, 0, 0),
                new Vertex(2, 2, 0),
                new Vertex(2, 2, 2),

                //right side
                new Vertex(0, 2, 0),
                new Vertex(2, 2, 0),
                new Vertex(2, 2, 2),
                new Vertex(0, 2, 2),

                //left side
                new Vertex(2, 0, 2),
                new Vertex(2, 0, 0),
                new Vertex(0, 0, 0),
                new Vertex(0, 0, 2),

                //back side
                new Vertex(0, 0, 0),
                new Vertex(0, 0, 2),
                new Vertex(0, 2, 0),
                new Vertex(0, 2, 2),

                //bottom side
                new Vertex(0, 0, 0),
                new Vertex(0, 2, 0),
                new Vertex(2, 2, 0),
                new Vertex(2, 0, 0),

                //top side
                new Vertex(0, 0, 2),
                new Vertex(0, 2, 2),
                new Vertex(2, 2, 2),
                new Vertex(2, 0, 2)
            };

            cube1 = new CustomCube(cubeVertices1);
        }

        private void openGLControl1_OpenGLDraw_1(object sender, RenderEventArgs args)
        {
            ////  Возьмём OpenGL объект
            //OpenGL gl = openGLControl1.OpenGL;

            //gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);


            ////gl.MatrixMode(SharpGL.Enumerations.MatrixMode.Modelview);

            ////ModelMatrix
            //cube1.Draw(gl);

            //////  Данная функция позволяет установить камеру и её положение


            //gl.LookAt();


            //viewMatrix
            //gl.Translate(0.0f, 0.0f, 1.0f);

            //ProjectionMatrix
            //double FoV = 60;
            //gl.Perspective(
            //    FoV,         // The horizontal Field of View, in degrees : the amount of "zoom". Think "camera lens".
            //                 //Usually between 90° (extra wide) and 30° (quite zoomed in)
            //    4.0f / 3.0f, // Aspect Ratio. Depends on the size of your window. Notice that 4/3 == 800/600 ==
            //                 //1280 / 960, sounds familiar ?
            //    0.1f, // Near clipping plane. Keep as big as possible, or you'll get precision issues.
            //    100.0f // Far clipping plane. Keep as little as possible.
            //);



            // Создаем экземпляр
            OpenGL gl = this.openGLControl1.OpenGL;
            // Очистка экрана и буфера глубин
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Пирамида /////////////////////////////
            // Сбрасываем модельно-видовую матрицу
            gl.LoadIdentity();
            // Сдвигаем перо влево от центра и вглубь экрана
            gl.Translate(0.0f, 0.0f, 0.0f);
            // Вращаем пирамиду вокруг ее оси Y
            gl.Rotate(rtri, 0.0f, 1.0f, 0.0f);

            // Вращаем пирамиду вокруг ее оси Y
            gl.Rotate(utri, 1.0f, 0.0f, 0.0f);
            // Рисуем треугольники - грани пирамиды
            gl.Begin(OpenGL.GL_TRIANGLES);

            // Front
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            // Right
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            // Back
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            // Left
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);

            gl.End();
            // Контроль полной отрисовки следующего изображения
            gl.Flush();

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
            gl.LookAt(0, 0, -6,    // позиция самой камеры (x, y, z)
                        0, 0, 0,     // направление, куда мы смотрим
                        0, 2, 0);    // верх камеры

            //  Зададим модель отображения
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void openGLControl1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                // Меняем угол поворота 
                rtri -= 3.0f;
            }
            else if (e.KeyCode == Keys.D)
            {
                rtri += 3.0f;
            }
            else if (e.KeyCode == Keys.W)
            {
                utri -= 3.0f;
            }
            else if (e.KeyCode == Keys.S)
            {
                utri += 3.0f;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void sceneControl1_MouseUp(object sender, MouseEventArgs e)
        {
            arcBallEffect.ArcBall.MouseUp(e.X, e.Y);

        }

        private void sceneControl1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void sceneControl1_MouseDown(object sender, MouseEventArgs e)
        {

        }


        /// <summary>
        /// Adds the element to tree.
        /// </summary>
        /// <param name="sceneElement">The scene element.</param>
        /// <param name="nodes">The nodes.</param>
        private void AddElementToTree(SceneElement sceneElement, TreeNodeCollection nodes)
        {
            //  Add the element.
            TreeNode newNode = new TreeNode()
            {
                Text = sceneElement.Name,
                Tag = sceneElement
            };
            nodes.Add(newNode);

            //  Add each child.
            foreach (var element in sceneElement.Children)
                AddElementToTree(element, newNode.Nodes);
        }

        /// <summary>
        /// Handles the AfterSelect event of the treeView1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectedSceneElement = e.Node.Tag as SceneElement;
        }

        /// <summary>
        /// Called when [selected scene element changed].
        /// </summary>
        private void OnSelectedSceneElementChanged()
        {
            propertyGrid1.SelectedObject = SelectedSceneElement;
        }

        /// <summary>
        /// The selected scene element.
        /// </summary>
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
                OnSelectedSceneElementChanged();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                arcBallEffect.ArcBall.MouseMove(e.X, e.Y);
            }

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            arcBallEffect.ArcBall.SetBounds(sceneControl1.Width, sceneControl1.Height);
            arcBallEffect.ArcBall.MouseDown(e.X, e.Y);
        }
    }
}
