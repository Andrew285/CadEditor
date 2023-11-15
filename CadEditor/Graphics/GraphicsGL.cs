using SharpGL;
using SharpGL.SceneGraph.Raytracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Graphics
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

        public static Ray InitializeRay(double x, double y)
        {
            double[] unProject1 = GraphicsGL.GL.UnProject(x, y, 0);
            double[] unProject2 = GraphicsGL.GL.UnProject(x, y, 1);

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

    }
}
