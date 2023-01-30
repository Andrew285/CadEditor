using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Runtime.InteropServices;

using SharpGL.SceneGraph;
using SharpGL;

namespace CadEditor
{
    public class Cube
    {
        private List<Vertex> vertices;

        public Cube(List<Vertex> _vertices)
        {
            vertices = _vertices;
        }

        public void Draw(OpenGL gl)
        {
            int red = 100;
            int green = 0;
            int blue = 0;
            Random random = new Random();
            if(vertices.Count != 0)
            {
                gl.Begin(OpenGL.GL_QUADS);

                for (int i = 0; i < vertices.Count / 4; i++)
                {
                    //int red = random.Next(0, 256);
                    //int green = random.Next(0, 256);
                    //int blue = random.Next(0, 256);

                    for (int j = 0; j < 4; j++)
                    {
                        gl.Color((float)red, (float)green, (float)blue);
                        gl.Vertex(vertices[4*i+j]);
                    }
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
