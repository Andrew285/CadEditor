using SharpGL;

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

        public static void SetUpViewMatrix(Camera camera)
        {
            GL.MatrixMode(OpenGL.GL_MODELVIEW);
            GL.LoadIdentity();

            Vector eye = camera.Position;
            Point3D center = camera.Target;
            Vector up = camera.RotationAxis;
            GL.LookAt(eye[0], eye[1], eye[2],
                      center[0], 0, center[2],
                      up[0], up[1], up[2]);
        }

        public static void SetUpProjectionMatrix()
        {
            GL.MatrixMode(OpenGL.GL_PROJECTION);
            GL.LoadIdentity();
            GL.Perspective(45.0, (double)GetWidth() / GetHeight(), 0.1, 100.0);
        }

    }
}
