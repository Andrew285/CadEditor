using CadEditor.Graphics;
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
        public static void Init()
        {
            OpenGL gl = GraphicsGL.GL;

            int N = 20;
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.LineWidth(1.0f);
            gl.Begin(OpenGL.GL_LINES);
            for (int i = 0; i < N; i++)
            {
                gl.Vertex(N, 0, 1*i);
                gl.Vertex(-N, 0, 1*i);

                gl.Vertex(N, 0, -1 * i);
                gl.Vertex(-N, 0, -1 * i);


                gl.Vertex(1 * i, 0, N);
                gl.Vertex(1 * i, 0, -N);

                gl.Vertex(-1 * i, 0, N);
                gl.Vertex(-1 * i, 0, -N);
            }

            gl.End();
            gl.Flush();
        }
    }
}
