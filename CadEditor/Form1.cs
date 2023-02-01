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
        private CustomCube cube1;
        private Cube cube;
        private Camera camera;
        private Scene scene;
        private OpenGL gl;

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
           
        }

        private void openGLControl1_OpenGLInitialized_1(object sender, EventArgs e)
        {
            //  Возьмём OpenGL объект
            gl = openGLControl1.OpenGL;
            camera = new Camera(gl);
            scene = new Scene(sceneControl1, treeView1, propertyGrid1);

            //  Устанавливаем цвет заливки по умолчанию (в данном случае цвет голубой)
            gl.ClearColor(0.0f, 0.0f, 0.0f, 0);

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
            //gl.Ortho(200.0, 200.0, 1.0, 1, -1.0, 1.0);
            cube = new Cube();
            scene.AddElement(cube);
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



            // Очистка экрана и буфера глубин
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Пирамида /////////////////////////////
            // Сбрасываем модельно-видовую матрицу
            gl.LoadIdentity();
            // Сдвигаем перо влево от центра и вглубь экрана
            gl.Translate(0.0f, 0.0f, 0.0f);

            camera.RotateAxisY();
            camera.RotateAxisX();

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
            gl.LookAt(0, 0, -6,    // позиция самой камеры (x, y, z)
                        0, 0, 0,     // направление, куда мы смотрим
                        0, 2, 0);    // верх камеры

            //  Зададим модель отображения
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                // Меняем угол поворота 
                camera.rtri -= 3.0f;
            }
            else if (e.KeyCode == Keys.D)
            {
                camera.rtri += 3.0f;
            }
            else if (e.KeyCode == Keys.W)
            {
                camera.utri -= 3.0f;
            }
            else if (e.KeyCode == Keys.S)
            {
                camera.utri += 3.0f;
            }
        }

    }
}
