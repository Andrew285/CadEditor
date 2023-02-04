using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public class Scene
    {
        private Camera camera;
        private OpenGL gl;
        private List<CustomCube> cubes; 

        public Scene(OpenGL _gl, Camera _camera)
        {
            camera = _camera;
            gl = _gl;
            cubes = new List<CustomCube>();
        }

        public void RotateCameraX()
        {
            camera.RotateAxisX();
        }

        public void RotateCameraY()
        {
            camera.RotateAxisY();
        }

        public void ChangeAxisX(float value)
        {
            camera.utri += value;
        }

        public void ChangeAxisY(float value)
        {
            camera.rtri += value;
        }

        public Vertex? SelectObject(OpenGL gl, int x, int y)
        {
            //	The first thing we do is turn the click into a vector that
            //	is from the camera to the cursor.
            double[] point1 = gl.UnProject((double)x, (double)y, 0);
            double[] point2 = gl.UnProject((double)x, (double)y, 1);

            double[] direction = {point2[0] - point1[0],
                                     point2[1] - point1[1],
                                     point2[2] - point1[2]};

            //	Now we find out the position when the line intersects the
            //	ground level.
            if (direction[1] == 0)
                return null;

            double dist = -6;
            //switch (plane)
            //{
            //    case Plane.xy:
            //        dist = -1.0 * point1[2] / direction[2];
            //        break;
            //    case Plane.xz:
            //        dist = -1.0 * point1[1] / direction[1];
            //        break;
            //    case Plane.yz:
            //        dist = -1.0 * point1[0] / direction[0];
            //        break;
            //}

            double px = point1[0] + dist * direction[0];
            double py = point1[1] + dist * direction[1];
            double pz = point1[2] + dist * direction[2];

            return new Vertex((float)px, (float)py, (float)pz);
        }

        public void DrawScene()
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, 0.0f);

            RotateCameraX();
            RotateCameraY();

            DrawCordinateAxes(3.0f, 20);
            DrawObjects();
        }

        public void DrawCordinateAxes(float lineWidth, float axisLength)
        {
            gl.LineWidth(lineWidth);
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(1f, 0, 0, 0);
            gl.Vertex(-axisLength, 0, 0);
            gl.Vertex(axisLength, 0, 0);

            gl.Color(0, 1f, 0, 0);
            gl.Vertex(0, -axisLength, 0);
            gl.Vertex(0, axisLength, 0);

            gl.Color(0, 0, 1f, 0);
            gl.Vertex(0, 0, -axisLength);
            gl.Vertex(0, 0, axisLength);

            gl.End();
            gl.Flush();
        }

        //public void DrawPyramid()
        //{
        //    // Рисуем треугольники - грани пирамиды
        //    gl.Begin(OpenGL.GL_TRIANGLES);

        //    // Front
        //    gl.Color(1.0f, 0.0f, 0.0f);
        //    gl.Vertex(0.0f, 1.0f, 0.0f);
        //    gl.Color(0.0f, 1.0f, 0.0f);
        //    gl.Vertex(-1.0f, -1.0f, 1.0f);
        //    gl.Color(0.0f, 0.0f, 1.0f);
        //    gl.Vertex(1.0f, -1.0f, 1.0f);
        //    // Right
        //    gl.Color(1.0f, 0.0f, 0.0f);
        //    gl.Vertex(0.0f, 1.0f, 0.0f);
        //    gl.Color(0.0f, 1.0f, 0.0f);
        //    gl.Vertex(1.0f, -1.0f, -1.0f);
        //    gl.Color(0.0f, 0.0f, 1.0f);
        //    gl.Vertex(1.0f, -1.0f, 1.0f);
        //    // Back
        //    gl.Color(1.0f, 0.0f, 0.0f);
        //    gl.Vertex(0.0f, 1.0f, 0.0f);
        //    gl.Color(0.0f, 1.0f, 0.0f);
        //    gl.Vertex(1.0f, -1.0f, -1.0f);
        //    gl.Color(0.0f, 0.0f, 1.0f);
        //    gl.Vertex(-1.0f, -1.0f, -1.0f);
        //    // Left
        //    gl.Color(1.0f, 0.0f, 0.0f);
        //    gl.Vertex(0.0f, 1.0f, 0.0f);
        //    gl.Color(0.0f, 1.0f, 0.0f);
        //    gl.Vertex(-1.0f, -1.0f, 1.0f);
        //    gl.Color(0.0f, 0.0f, 1.0f);
        //    gl.Vertex(-1.0f, -1.0f, -1.0f);

        //    gl.End();
        //    // Контроль полной отрисовки следующего изображения
        //    gl.Flush();
        //}

        public void DrawObjects()
        {
            foreach(CustomCube cube in cubes)
            {
                cube.Draw();
            }
        }

        public void InitObjects()
        {
            cubes.Add(new CustomCube(gl));
        }

        public void AddObject(CustomCube c)
        {
            cubes.Add(c);
        }
    }
}
