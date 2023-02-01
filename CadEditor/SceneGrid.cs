using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public static class SceneGrid
    {
        public static void Init(OpenGL gl)
        {
            int N = 20;
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Begin(OpenGL.GL_LINES);
            for (int i = 0; i < N; i++)
            {
                gl.Vertex(N, -2, 1*i);
                gl.Vertex(-N, -2, 1*i);

                gl.Vertex(N, -2, -1 * i);
                gl.Vertex(-N, -2, -1 * i);


                gl.Vertex(1 * i, -2, N);
                gl.Vertex(1 * i, -2, -N);

                gl.Vertex(-1 * i, -2, N);
                gl.Vertex(-1 * i, -2, -N);
            }

            gl.End();
            gl.Flush();
        }
    }
}
