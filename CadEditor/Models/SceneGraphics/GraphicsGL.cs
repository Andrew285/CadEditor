using SharpGL;
using SharpGL.SceneGraph.Cameras;
using System.Threading;
using System;
using System.Numerics;
using System.Diagnostics;

namespace CadEditor
{
    public class GraphicsGL
    {
        public static OpenGL GL { get; set; }
        public static OpenGLControl Control { get; set; }

        public GraphicsGL(OpenGLControl control)
        {
            if(control != null)
            {
                Control = control;
                GL = Control.OpenGL;
            }
        }

        public static void CreateInstance(OpenGLControl openGL)
        {
            if (GraphicsGL.GL == null)
            {
                new GraphicsGL(openGL);
            }

        }

        public static int GetHeight()
        {
            return Control.Height;
        }

        public static int GetWidth()
        {
            return Control.Width;
        }

        public static Ray InitializeRay(double mouseX, double mouseY)
        {
            //double[] modelviewMatrix = new double[16];
            //double[] projectionMatrix = new double[16];
            //int[] viewport = new int[4];

            //GL.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelviewMatrix);
            //GL.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projectionMatrix);
            //GL.GetInteger(OpenGL.GL_VIEWPORT, viewport);

            //double x = 0, y = 0, z = 0;
            //GL.UnProject(mouseX, mouseY, 0.0, modelviewMatrix, projectionMatrix, viewport, ref x, ref y, ref z);

            //// Camera position
            //Vector3 cameraPos = Scene.GetInstance().Camera.Position;

            //// Direction from camera to clicked point
            //double directionX = x - cameraPos.X;
            //double directionY = y - cameraPos.Y;
            //double directionZ = z - cameraPos.Z;

            //// You now have the direction vector from the camera position to the clicked point
            //// You can use this information for various purposes, such as ray casting.

            //// Example: Normalize the direction vector
            //double length = Math.Sqrt(directionX * directionX + directionY * directionY + directionZ * directionZ);
            //directionX /= length;
            //directionY /= length;
            //directionZ /= length;

            //// Now you have a normalized direction vector

            //// Example: Use the direction vector for ray casting or other calculations
            //return new Ray(new Vector(cameraPos.X, cameraPos.Y, cameraPos.Z), new Vector((float)directionX, (float)directionY, (float)directionZ));


            double[] unProject1 = GraphicsGL.GL.UnProject(mouseX, mouseY, 0);
            double[] unProject2 = GraphicsGL.GL.UnProject(mouseX, mouseY, 1);

            Vector near = new Vector(unProject1);
            Vector far = new Vector(unProject2);
            Vector direction = (far - near).Normalize();

            //initialize a ray
            return new Ray(near, direction);
        }

        public static void DisableContexMenu()
        {
            Control.ContextMenu = null;
        }

        public static void Invalidate()
        {
            Control.Invalidate();
        }

        public static void SetUpViewMatrix(Camera camera)
        {
            GL.MatrixMode(OpenGL.GL_MODELVIEW);
            GL.LoadIdentity();

            GL.LookAt(
                    camera.Position.X, camera.Position.Y, camera.GetDistance(),
                    camera.Target.X, camera.Target.Y, camera.Target.Z,
                    camera.Up.X, camera.Up.Y, camera.Up.Z
                );
        }

        public static void SetUpProjectionMatrix()
        {
            GL.MatrixMode(OpenGL.GL_PROJECTION);
            GL.LoadIdentity();
            GL.Perspective(45.0, (double)GetWidth() / GetHeight(), 0.1, 100.0);
        }

    }
}
