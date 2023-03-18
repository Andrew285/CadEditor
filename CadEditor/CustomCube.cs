using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Runtime.InteropServices;

using SharpGL.SceneGraph;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Core;

namespace CadEditor
{
    public class CustomCube: SceneElement
    {
        private List<SharpGL.SceneGraph.Vertex> vertices;
        private OpenGL gl;
        private float[,] coordinates;
        private float N = 1;
		private Mesh mesh;
		private const int facetsAmount = 6;
		private const int verticesAmount = 8;

		public Mesh Mesh
		{
			get { return mesh; }
		}

		//public CustomCube(OpenGL _gl)
  //      {
		//	//vertices = _vertices;
		//	gl = _gl;

  //          coordinates = new float[24, 3]
  //          {
  //              //front side
  //              {N, -N, -N},
  //              {N, N, -N},
  //              {N, N, N},
  //              {N, -N, N},

  //              //back side
  //              {-N, -N, -N},
  //              {-N, N, -N},
  //              {-N, N, N},
  //              {-N, -N, N},

  //              //up side
  //              {N, N, -N},
  //              {-N, N, -N},
  //              {-N, N, N},
  //              {N, N, N},

  //              //bottom side
  //              {N, -N, -N},
  //              {-N, -N, -N},
  //              {-N, -N, N},
  //              {N, -N, N},

  //              //right side
  //              {N, -N, -N},
  //              {-N, -N, -N},
  //              {-N, N, -N},
  //              {N, N, -N},

  //              //left side
  //              {N, -N, N},
  //              {-N, -N, N},
  //              {-N, N, N},
  //              {N, N, N},
  //          };
  //      }

		public CustomCube(OpenGL _gl)
		{
			gl = _gl;
			mesh = new Mesh();
			mesh.Facets = new Facet[facetsAmount]
			{
				new Facet(new Vertex[]
				{
					new Vertex(-1.0f, -1.0f, 1.0f),
					new Vertex(1.0f, -1.0f, 1.0f),
					new Vertex(1.0f, 1.0f, 1.0f),
					new Vertex(-1.0f, 1.0f, 1.0f)
				}),
				new Facet(new Vertex[]
				{
					new Vertex(-1.0f, -1.0f, -1.0f),
					new Vertex(-1.0f, 1.0f, -1.0f),
					new Vertex(1.0f, 1.0f, -1.0f),
					new Vertex(1.0f, -1.0f, -1.0f)
				}),
				new Facet(new Vertex[]
				{
					new Vertex(-1.0f, 1.0f, -1.0f),
					new Vertex(-1.0f, 1.0f, 1.0f),
					new Vertex(1.0f, 1.0f, 1.0f),
					new Vertex(1.0f, 1.0f, -1.0f)
				}),
				new Facet(new Vertex[]
				{
					new Vertex(-1.0f, -1.0f, -1.0f),
					new Vertex(1.0f, -1.0f, -1.0f),
					new Vertex(1.0f, -1.0f, 1.0f),
					new Vertex(-1.0f, -1.0f, 1.0f)
				}),
				new Facet(new Vertex[]
				{
					new Vertex(1.0f, -1.0f, -1.0f),
					new Vertex(1.0f, 1.0f, -1.0f),
					new Vertex(1.0f, 1.0f, 1.0f),
					new Vertex(1.0f, -1.0f, 1.0f)
				}),
				new Facet(new Vertex[]
				{
					new Vertex(-1.0f, -1.0f, -1.0f),
					new Vertex(-1.0f, -1.0f, 1.0f),
					new Vertex(-1.0f, 1.0f, 1.0f),
					new Vertex(-1.0f, 1.0f, -1.0f)
				})
			};
		}


		public void Draw()
		{
			//if (coordinates.Length != 0)
			//{
			//    gl.Color(0.0f, 0.0f, 1.0f);
			//    gl.Begin(OpenGL.GL_QUADS);

			//    for(int i = 0; i < coordinates.GetLength(0); i++)
			//    {
			//        gl.Vertex(coordinates[i, 0], coordinates[i, 1], coordinates[i, 2]);
			//    }
			//    gl.End();
			//}
			//else
			//{
			//    throw new ArgumentException("There is nothing to draw");
			//}

			// Draw the cube
			gl.Begin(OpenGL.GL_QUADS);

			// Front face
			gl.Color(1.0f, 0.0f, 0.0f);
			gl.Vertex(-1.0f, -1.0f, 1.0f);
			gl.Vertex(1.0f, -1.0f, 1.0f);
			gl.Vertex(1.0f, 1.0f, 1.0f);
			gl.Vertex(-1.0f, 1.0f, 1.0f);

			// Back face
			gl.Color(0.0f, 1.0f, 0.0f);
			gl.Vertex(-1.0f, -1.0f, -1.0f);
			gl.Vertex(-1.0f, 1.0f, -1.0f);
			gl.Vertex(1.0f, 1.0f, -1.0f);
			gl.Vertex(1.0f, -1.0f, -1.0f);

			// Top face
			gl.Color(0.0f, 0.0f, 1.0f);
			gl.Vertex(-1.0f, 1.0f, -1.0f);
			gl.Vertex(-1.0f, 1.0f, 1.0f);
			gl.Vertex(1.0f, 1.0f, 1.0f);
			gl.Vertex(1.0f, 1.0f, -1.0f);

			// Bottom face
			gl.Color(1.0f, 1.0f, 0.0f);
			gl.Vertex(-1.0f, -1.0f, -1.0f);
			gl.Vertex(1.0f, -1.0f, -1.0f);
			gl.Vertex(1.0f, -1.0f, 1.0f);
			gl.Vertex(-1.0f, -1.0f, 1.0f);

			// Right face
			gl.Color(1.0f, 0.0f, 1.0f);
			gl.Vertex(1.0f, -1.0f, -1.0f);
			gl.Vertex(1.0f, 1.0f, -1.0f);
			gl.Vertex(1.0f, 1.0f, 1.0f);
			gl.Vertex(1.0f, -1.0f, 1.0f);

			gl.End();
			gl.Flush();
		}

		public void DrawComplete()
		{
			gl.Begin(OpenGL.GL_QUADS);
			for (int i = 0; i < mesh.Facets.Length; i++)
			{
				if (mesh.Facets[i].IsSelected)
				{
					gl.Color(1.0f, 0.0f, 1.0f);
				}
				else
				{
					gl.Color(1.0f, 1.0f, 0.0f);
				}

				gl.Vertex(mesh.Facets[i].Vertices[0].X, mesh.Facets[i].Vertices[0].Y, mesh.Facets[i].Vertices[0].Z);
				gl.Vertex(mesh.Facets[i].Vertices[1].X, mesh.Facets[i].Vertices[1].Y, mesh.Facets[i].Vertices[1].Z);
				gl.Vertex(mesh.Facets[i].Vertices[2].X, mesh.Facets[i].Vertices[2].Y, mesh.Facets[i].Vertices[2].Z);
				gl.Vertex(mesh.Facets[i].Vertices[3].X, mesh.Facets[i].Vertices[3].Y, mesh.Facets[i].Vertices[3].Z);
			}
			gl.End();
			gl.Flush();
		}

        public List<Vector> CreateListOfVertices()
        {
            List<Vector> cubeVertices = new List<Vector>();

            // Define the vertices of the cube
            Vector v1 = new Vector(new double[] { -1, -1, -1 });
            Vector v2 = new Vector(new double[] { -1, 1, -1 });
            Vector v3 = new Vector(new double[] { 1, 1, -1 });
            Vector v4 = new Vector(new double[] { 1, -1, -1 });
            Vector v5 = new Vector(new double[] { -1, -1, 1 });
			Vector v6 = new Vector(new double[] { -1, 1, 1 });
			Vector v7 = new Vector(new double[] { 1, 1, 1 });
			Vector v8 = new Vector(new double[] { 1, -1, 1 });

			// Add the vertices to the list in the correct order
			cubeVertices.Add(v1);
			cubeVertices.Add(v2);
			cubeVertices.Add(v3);
			cubeVertices.Add(v4);

			cubeVertices.Add(v4);
			cubeVertices.Add(v3);
			cubeVertices.Add(v7);
			cubeVertices.Add(v8);

			cubeVertices.Add(v8);
			cubeVertices.Add(v7);
			cubeVertices.Add(v6);
			cubeVertices.Add(v5);

			cubeVertices.Add(v5);
			cubeVertices.Add(v6);
			cubeVertices.Add(v2);
			cubeVertices.Add(v1);

			cubeVertices.Add(v2);
			cubeVertices.Add(v6);
			cubeVertices.Add(v7);
			cubeVertices.Add(v3);

			cubeVertices.Add(v5);
			cubeVertices.Add(v1);
			cubeVertices.Add(v4);
			cubeVertices.Add(v8);

            return cubeVertices;
		}
    }
}
