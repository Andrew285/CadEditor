using SharpGL;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Numerics;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace CadEditor
{
    public partial class Form1 : Form
    {
        float rotation = 0.0f; // переменная, которая будет задавать угол поворота
        float distance = 7;
        float startX;
        float startY;
        float xRotAngle;
        float yRotAngle;

        float camAngleX;
        float camAngleY;
        float camAngleZ;

        float theta;

        bool isRotationStarted;
        List<Vertex> cubeVertices1;
        Cube cube1;
        

        double position = 0;
        double direction = 0;
        double up = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void openglControl1_MouseUp(object sender, MouseEventArgs e)
        {

            isRotationStarted = false;
        }

        private void openglControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isRotationStarted)
            {
                xRotAngle += e.Y - startY;
                yRotAngle += e.X - startX;

                //camAngleX += e.Y - startY;
                //camAngleY += e.X - startX;

                theta += e.X - startX;

                startX = e.X;
                startY = e.Y;
            }

        }

        private void openglControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isRotationStarted = true;
                startX = e.X;
                startY = e.Y;
            }
        }

        private void openGLControl1_OpenGLInitialized_1(object sender, EventArgs e)
        {
            //  Возьмём OpenGL объект
            OpenGL gl = openGLControl1.OpenGL;

            //  Устанавливаем цвет заливки по умолчанию (в данном случае цвет голубой)
            gl.ClearColor(0.1f, 0.5f, 1.0f, 0);

            isRotationStarted = false;
            startX = 0;
            startY = 0;
            xRotAngle = 15;
            yRotAngle = 15;

            cubeVertices1 = new List<Vertex>
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

            cube1 = new Cube(cubeVertices1);
        }

        private void openGLControl1_OpenGLDraw_1(object sender, RenderEventArgs args)
        {
            //  Возьмём OpenGL объект
            OpenGL gl = openGLControl1.OpenGL;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);


            //gl.MatrixMode(SharpGL.Enumerations.MatrixMode.Modelview);

            //ModelMatrix
            cube1.Draw(gl);

            ////  Данная функция позволяет установить камеру и её положение

            ////gl.LookAt(
            ////        position,
            ////        position + direction,
            ////        up
            ////        );    // верх камеры


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
            gl.LookAt(-5, 3, -6,    // позиция самой камеры (x, y, z)
                        0, 0, 0,     // направление, куда мы смотрим
                        0, 2, 0);    // верх камеры

            //  Зададим модель отображения
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void openGLControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Right) 
            {
                position += right 
            }
        }
    }
}
