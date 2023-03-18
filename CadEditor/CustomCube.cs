using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Runtime.InteropServices;
using SharpGL.SceneGraph;
using SharpGL;
using SharpGL.SceneGraph.Core;
using System.Linq.Expressions;

namespace CadEditor
{
    public class CustomCube: SceneElement
    {
        private OpenGL gl;
		private Mesh mesh;
		private string cubeName;
		private const int FACETS_AMOUNT = 6;
		private const int EDGES_AMOUNT = 12;
		private const int VERTICES_AMOUNT = 8;

		public Mesh Mesh
		{
			get { return mesh; }
		}

		public string CubeName
		{
			get { return cubeName; }
		}

		public CustomCube(OpenGL _gl, string _cubeName)
		{
			gl = _gl;
			mesh = new Mesh();
			cubeName = _cubeName;
			mesh.Facets = new Facet[FACETS_AMOUNT]
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

			List<Edge> edges = new List<Edge>();
			for(int i = 0; i < mesh.Facets.Length; i++)
			{
				Facet currentFacet = mesh.Facets[i];
				for(int j = 0; j < 4; j++)
				{
					Edge newEdge = new Edge(currentFacet.Vertices[j], currentFacet[(j+1)%4]);
					if(edges.Count != 0)
					{
						if (!edges.Contains(newEdge))
						{
							edges.Add(newEdge);
						}
					}
					else
					{
						edges.Add(newEdge);
					}
				}
			}
			mesh.Edges = edges.ToArray();
		}

		public void Draw()
		{
			//Draw Facets
			gl.Begin(OpenGL.GL_QUADS);
			for (int i = 0; i < mesh.Facets.Length; i++)
			{
				Facet currectFacet = mesh.Facets[i];
				if (currectFacet.IsSelected)
				{
					gl.Color(currectFacet.SelectedColor);
				}
				else
				{
					gl.Color(currectFacet.NonSelectedColor);
				}

				gl.Vertex(currectFacet.Vertices[0].X, currectFacet.Vertices[0].Y, currectFacet.Vertices[0].Z);
				gl.Vertex(currectFacet.Vertices[1].X, currectFacet.Vertices[1].Y, currectFacet.Vertices[1].Z);
				gl.Vertex(currectFacet.Vertices[2].X, currectFacet.Vertices[2].Y, currectFacet.Vertices[2].Z);
				gl.Vertex(currectFacet.Vertices[3].X, currectFacet.Vertices[3].Y, currectFacet.Vertices[3].Z);
			}
			gl.End();
			gl.Flush();

			//Draw Edges
			gl.Begin(OpenGL.GL_LINES);
			for (int i = 0; i < mesh.Edges.Length; i++)
			{
				Edge currectEdge = mesh.Edges[i];
				if (currectEdge.IsSelected)
				{
					gl.Color(currectEdge.SelectedColor);
				}
				else
				{
					gl.Color(currectEdge.NonSelectedColor);
				}

				gl.Vertex(currectEdge.V1.X, currectEdge.V1.Y, currectEdge.V1.Z);
				gl.Vertex(currectEdge.V2.X, currectEdge.V2.Y, currectEdge.V2.Z);
			}
			gl.End();
			gl.Flush();

			//Draw Vertexes

		}

		public void SelectCompletely()
		{
			foreach(Facet facet in Mesh.Facets)
			{
				facet.IsSelected = true;
			}
		}
	}
}
