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

        public CustomCube(OpenGL _gl, int _val = 0)
        {
            //vertices = _vertices;
            gl = _gl;
            val = _val;
        }

        public void Draw()
        {
            //Random random = new Random();
            //if(vertices.Count != 0)
            //{
            //    gl.Begin(OpenGL.GL_QUADS);

            //    for (int i = 0; i < vertices.Count / 4; i++)
            //    {


            //        for (int j = 0; j < 4; j++)
            //        {
            //            int red = random.Next(0, 256);
            //            int green = random.Next(0, 256);
            //            int blue = random.Next(0, 256);

            //            gl.Color((float)red, (float)green, (float)blue);
            //            gl.Vertex(vertices[4*i+j]);
            //        }
            //    }

            //    gl.End();

            //}
            //else
            //{
            //    throw new ArgumentException("There is nothing to draw");
            //}


            //gl.Begin(OpenGL.GL_QUADS);

            //// Front
            //gl.Color(1.0f, 0.0f, 0.0f);
            //gl.Vertex(0.0f, 1.0f + val, 0.0f);
            //gl.Color(0.0f, 1.0f, 0.0f);
            //gl.Vertex(-1.0f + val, -1.0f + val, 1.0f + val);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(1.0f + val, -1.0f + val, 1.0f);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(1.0f + val, -1.0f + val, 1.0f + val);
            //// Right
            //gl.Color(1.0f, 0.0f, 0.0f);
            //gl.Vertex(0.0f, 1.0f + val, 0.0f);
            //gl.Color(0.0f, 1.0f, 0.0f);
            //gl.Vertex(1.0f + val, -1.0f + val, -1.0f + val);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(1.0f + val, -1.0f + val, 1.0f + val);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(1.0f + val, -1.0f + val, 1.0f + val);
            //// Back
            //gl.Color(1.0f, 0.0f, 0.0f);
            //gl.Vertex(0.0f, 1.0f + val, 0.0f);
            //gl.Color(0.0f, 1.0f, 0.0f);
            //gl.Vertex(1.0f + val, -1.0f + val, -1.0f + val);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(-1.0f + val, -1.0f + val, -1.0f + val);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(-1.0f + val, -1.0f + val, -1.0f + val);
            //// Left
            //gl.Color(1.0f, 0.0f, 0.0f);
            //gl.Vertex(0.0f, 1.0f + val, 0.0f);
            //gl.Color(0.0f, 1.0f, 0.0f);
            //gl.Vertex(-1.0f + val, -1.0f + val, 1.0f + val);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(-1.0f + val, -1.0f + val, -1.0f + val);
            //gl.Color(0.0f, 0.0f, 1.0f);
            //gl.Vertex(-1.0f + val, -1.0f + val, -1.0f + val);

            //gl.End();
            //// Контроль полной отрисовки следующего изображения
            //gl.Flush();


            gl.Begin(OpenGL.GL_QUADS);                 // Start Drawing The Cube

            gl.Vertex(-1.0f + val, -1.0f + val, 1.0f + val);   // Bottom Left Of The Texture and Quad
            gl.Vertex(1.0f + val, -1.0f + val, 1.0f + val);    // Bottom Right Of The Texture and Quad
            gl.Vertex(1.0f + val, 1.0f + val, 1.0f + val); // Top Right Of The Texture and Quad
            gl.Vertex(-1.0f + val, 1.0f + val, 1.0f + val);    // Top Left Of The Texture and Quad

            // Back Face
            gl.Vertex(-1.0f + val, -1.0f + val, -1.0f + val);  // Bottom Right Of The Texture and Quad
            gl.Vertex(-1.0f + val, 1.0f + val, -1.0f + val);   // Top Right Of The Texture and Quad
            gl.Vertex(1.0f + val, 1.0f + val, -1.0f + val);    // Top Left Of The Texture and Quad
            gl.Vertex(1.0f + val, -1.0f + val, -1.0f + val);   // Bottom Left Of The Texture and Quad

            // Top Face
            gl.Vertex(-1.0f + val, 1.0f + val, -1.0f + val);   // Top Left Of The Texture and Quad
            gl.Vertex(-1.0f + val, 1.0f + val, 1.0f + val);    // Bottom Left Of The Texture and Quad
            gl.Vertex(1.0f + val, 1.0f + val, 1.0f + val); // Bottom Right Of The Texture and Quad
            gl.Vertex(1.0f + val, 1.0f + val, -1.0f + val);    // Top Right Of The Texture and Quad

            // Bottom Face
            gl.Vertex(-1.0f + val, -1.0f + val, -1.0f + val);  // Top Right Of The Texture and Quad
            gl.Vertex(1.0f + val, -1.0f + val, -1.0f + val);   // Top Left Of The Texture and Quad
            gl.Vertex(1.0f + val, -1.0f + val, 1.0f + val);    // Bottom Left Of The Texture and Quad
            gl.Vertex(-1.0f + val, -1.0f + val, 1.0f + val);   // Bottom Right Of The Texture and Quad

            // Right face
            gl.Vertex(1.0f + val, -1.0f + val, -1.0f + val);   // Bottom Right Of The Texture and Quad
            gl.Vertex(1.0f + val, 1.0f + val, -1.0f + val);    // Top Right Of The Texture and Quad
            gl.Vertex(1.0f + val, 1.0f + val, 1.0f + val); // Top Left Of The Texture and Quad
            gl.Vertex(1.0f + val, -1.0f + val, 1.0f + val);    // Bottom Left Of The Texture and Quad

            // Left Face
            gl.Vertex(-1.0f + val, -1.0f + val, -1.0f + val);  // Bottom Left Of The Texture and Quad
            gl.Vertex(-1.0f + val, -1.0f + val, 1.0f + val);   // Bottom Right Of The Texture and Quad
            gl.Vertex(-1.0f + val, 1.0f + val, 1.0f + val);    // Top Right Of The Texture and Quad
            gl.Vertex(-1.0f + val, 1.0f + val, -1.0f + val);   // Top Left Of The Texture and Quad

            gl.End();
            //  Flush OpenGL.
            gl.Flush();
        }
    }
}
