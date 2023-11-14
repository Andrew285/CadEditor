using SharpGL;
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

        public GraphicsGL(OpenGL gl)
        {
            GL = gl;
        }

        public static OpenGL getInstance()
        {
            if (GraphicsGL.GL == null)
            {
                return new OpenGL();
            }
            else
            {
                return GL;
            }

        }

    }
}
