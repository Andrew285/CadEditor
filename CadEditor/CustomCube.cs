using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Runtime.InteropServices;

using SharpGL.SceneGraph;
using SharpGL;
using SharpGL.SceneGraph.Core;

namespace CadEditor
{
    public class CustomCube: SceneElement
    {
        private List<Vertex> vertices;
        private OpenGL gl;
        private int val;
        private float[,] coordinates;
        private float N = 1;

        public CustomCube(OpenGL _gl, int _val = 0)
        {
            //vertices = _vertices;
            gl = _gl;
            val = _val;

            coordinates = new float[24, 3]
            {
                //front side
                {N, -N, -N},
                {N, N, -N},
                {N, N, N},
                {N, -N, N},

                //back side
                {-N, -N, -N},
                {-N, N, -N},
                {-N, N, N},
                {-N, -N, N},

                //up side
                {N, N, -N},
                {-N, N, -N},
                {-N, N, N},
                {N, N, N},

                //bottom side
                {N, -N, -N},
                {-N, -N, -N},
                {-N, -N, N},
                {N, -N, N},

                //right side
                {N, -N, -N},
                {-N, -N, -N},
                {-N, N, -N},
                {N, N, -N},

                //left side
                {N, -N, N},
                {-N, -N, N},
                {-N, N, N},
                {N, N, N},
            };
        }

        public void Draw()
        {
            if (coordinates.Length != 0)
            {
                gl.Color(0.0f, 0.0f, 1.0f);
                gl.Begin(OpenGL.GL_QUADS);

                for(int i = 0; i < coordinates.GetLength(0); i++)
                {
                    gl.Vertex(coordinates[i, 0], coordinates[i, 1], coordinates[i, 2]);
                }
                gl.End();
            }
            else
            {
                throw new ArgumentException("There is nothing to draw");
            }
        }
    }
}
